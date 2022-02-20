using LcuApiNet.EventHandlers;
using LcuApiNet.Exceptions;
using LcuApiNet.Model;
using LcuApiNet.Model.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LcuApiNet.Core.Events
{
    public class LeagueEventService
    {
        private ILcuApi _api;
        private List<LeagueEventType> _subscriptions = new List<LeagueEventType>();
        private Regex _rQuote = new Regex("\"");

#pragma warning disable CS0067
        [LeagueEventRepeater(LeagueEventType.GameflowPhaseChanged, "OnJsonApiEvent_lol-gameflow_v1_gameflow-phase", typeof(GameflowPhase), "/lol-gameflow/v1/gameflow-phase")]
        public event LeagueEventHandler<GameflowPhase>? GameflowPhaseChanged;

        [LeagueEventRepeater(LeagueEventType.LobbyInfoChanged, "OnJsonApiEvent_lol-lobby_v2_lobby", typeof(LobbyInfo), "/lol-lobby/v2/lobby")]
        public event LeagueEventHandler<LobbyInfo>? LobbyInfoChanged;
#pragma warning restore CS0067

        public LeagueEventService(ILcuApi api)
        {
            _api = api;

            _api.Socket.AttachWampEventHandler(EventFired);

            ExаmineEvents();
        }


        private void EventFired(string message) {
            LeagueEvent? @event = JsonConvert.DeserializeObject<LeagueEvent>(message);

            if (@event == null) {
                throw new WampSocketExсeption($"Invalid wamp event message: [{message}]");
            }
            
            foreach(KeyValuePair<LeagueEventRepeaterAttribute, EventInfo> elem in _events) {
                if (elem.Key.Uri == @event.Uri) { 
                    if (elem.Key.DataType.IsEnum) {
                        object? enumValue = Enum.Parse(elem.Key.DataType, (string)@event.Data!);
                        RaiseReflectionEvent(elem.Value, enumValue);
                    } else if (elem.Key.DataType.IsPrimitive || elem.Key.DataType == typeof(string) || elem.Key.DataType == typeof(decimal)) { 
                        RaiseReflectionEvent(elem.Value, @event.Data);
                    } else {
                        JObject? dataObject = (JObject?)@event.Data;
                        RaiseReflectionEvent(elem.Value, dataObject?.ToObject(elem.Key.DataType));
                    }

                    return;
                }
            }
        }

        public void Subscribe(LeagueEventType leagueEventType) 
        { 
            if(_subscriptions.Contains(leagueEventType)) {
                return;
            }

            _subscriptions.Add(leagueEventType);
            
            if (_api.Client.IsReady) {
                string eventCategory = _eventEndpoints[leagueEventType];
                _api.Socket.Subscribe(eventCategory);
            }
        }

        public void Unsubscribe(LeagueEventType leagueEventType)
        {
            if (!_subscriptions.Contains(leagueEventType)) {
                throw new InvalidOperationException($"Attempted to unsubscribe from the event [{leagueEventType}] that wasn't listened");
            }

            if (_api.Client.IsReady) {
                _api.Socket.Unsubscribe(_eventEndpoints[leagueEventType]);
            }
        }
        
        public void UnsubscribeAll() 
        {
            if (_api.Client.IsReady) {
                foreach (LeagueEventType eventType in _subscriptions) {
                    _api.Socket.Unsubscribe(_eventEndpoints[eventType]);
                }
            }
            _subscriptions.Clear();
        }
        
        public void SubscribeAll()
        {
            
            foreach (LeagueEventType eventType in _eventEndpoints.Keys.Except(_subscriptions)) {
                string eventCategory = _eventEndpoints[eventType];
                if (_api.Client.IsReady) {
                    _api.Socket.Subscribe(eventCategory);
                }
                _subscriptions.Add(eventType);
            }
        }

        internal void ResubscribeAll()
        {
            foreach (LeagueEventType eventCategory in _subscriptions) {
                _api.Socket.Subscribe(_eventEndpoints[eventCategory]);
            }
        }

        private void ExаmineEvents()
        {
            foreach(EventInfo eventInfo in typeof(LeagueEventService).GetEvents()) {
                LeagueEventRepeaterAttribute? attribute = eventInfo.GetCustomAttribute<LeagueEventRepeaterAttribute>();

                if (attribute == null) {
                    continue;
                }

                _events.Add(attribute, eventInfo);
                _eventEndpoints.Add(attribute.EventType, attribute.CategoryUri);
            }
        }

        private void RaiseReflectionEvent(EventInfo eventInfo, object? dataObject)
        {
            FieldInfo? eventField = this.GetType().GetField(eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance);
            MulticastDelegate? eventValue = (MulticastDelegate?)eventField!.GetValue(this);
            if (eventValue == null) {
                return;
            }
            foreach (Delegate elem in eventValue.GetInvocationList()) {
                elem.Method?.Invoke(elem.Target, new object?[] { this, dataObject });
            }
        }


        private Dictionary<LeagueEventRepeaterAttribute, EventInfo> _events = new Dictionary<LeagueEventRepeaterAttribute, EventInfo>();
        private Dictionary<LeagueEventType, string> _eventEndpoints = new Dictionary<LeagueEventType, string>();
    }
}
