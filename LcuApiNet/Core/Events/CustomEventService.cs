using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Channels;
using LcuApiNet.EventHandlers.PickStage;
using LcuApiNet.Model;
using LcuApiNet.Model.ClientModels;
using LcuApiNet.Model.ClientModels.ChampSelectModels;
using LcuApiNet.Model.Enums;
using LcuApiNet.Utilities;
using LcuApiNet.Utilities.Converters;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LcuApiNet.Core.Events;

public class CustomEventService
{
    private ILcuApi _api;

    public event SelectStageStartedHandler? SelectStageStarted;
    public event ChampionHoveredHandler? ChampionHovered;
    public event ChampionBannedHandler? ChampionBanned;
    public event ChampionPickedHandler? ChampionPicked;
    public event ActionRequestedHandler? ActionRequested;
    public event ChampionSkinChangedHandler? ChampionSkinChanged;
    public event PlanningStageEndedHandler? PlanningStageEnded;
    public event UserActionRequestedEventHandler? UserActionRequested;
    public event SelectStageEndedHandler? SelectStageEnded;

    private TeamMember[] _previousAllyTeamInfo = Array.Empty<TeamMember>();

    private ObservableCollection<SessionAction[]> CurrentSessionActionList = new ObservableCollection<SessionAction[]>();
    private List<TeamMember> CurrentTeamsData = new List<TeamMember>();

    // private int _currentSessionActionCount;
    private int _userCellId = 0;
    private long _userId;
    
    private bool _isPlanningStage;
    // private bool _isBanningStage;
    // private bool _isPickStage = false;
    // private bool _userActionCompleted;
    // private bool _isSelectStageEnded;
    private bool _champSelectInProgress;
    public CustomEventService(ILcuApi api)
    {
        _api = api;
        _api.LeagueEvents.ChampSelectSessionChanged += HandleChampSelectSessionChanged;
        _api.Client.StateChanged += async (_, args) => {
            if (args.State) {
                _userId = _api.Summoner.GetLocalUserId().Result;
            }
        };

        _api.LeagueEvents.GameflowPhaseChanged += (_, _, args) => {
            if (args == GameflowPhase.ChampSelect) {
                _champSelectInProgress = true;
            } else if (_champSelectInProgress) {
                _champSelectInProgress = false;
                SelectStageEnded?.Invoke(this, new SelectStageEndedEventArgs(true));
            }
        };

        CurrentSessionActionList.CollectionChanged += (_, args) => {
            if (args.Action == NotifyCollectionChangedAction.Add) {
                foreach (SessionAction[] sessionActions in args.NewItems!) {
                    foreach (SessionAction sessionAction in sessionActions) {
                        //Console.WriteLine($"[DEBUG: New action] sender = |{sessionAction.ActorCellId}, {sessionAction.Type}|, isAllyAction = {sessionAction.IsAllyAction}");
                    }
                }
            }
        };
    }

    private async void HandleChampSelectSessionChanged(object sender, LeagueInternalEventType eventType, ChampSelectSession args)
    {
        if (eventType == LeagueInternalEventType.Delete) {
            return;
        }
        
        if (args.Timer.Phase == "PLANNING") {
            _isPlanningStage = true;
        }
        
        if (eventType == LeagueInternalEventType.Create) {
            CurrentSessionActionList.Clear();
            foreach (SessionAction[] sessionActions in args.Actions) {
                AddSessionActions(sessionActions);
            }

            CurrentTeamsData.Clear();
            AddTeamData(args.MyTeam);

            Debug.WriteLine(JsonConvert.SerializeObject(args.Actions, Formatting.Indented));
            List<SelectStageMember> allyPickOrder = new List<SelectStageMember>();
            foreach (TeamMember member in args.MyTeam) {
                allyPickOrder.Add(new SelectStageMember(member.SummonerId.ToString(), member.AssignedPosition == "" ? Role.Any : RoleConverter.FromStringToEnum(member.AssignedPosition)));
                if (member.SummonerId == _userId) {
                    _userCellId = member.CellId;
                }
            }
            
            int[] availableChampionIds = await _api.Pick.GetAvailableChampionIds().ConfigureAwait(false);
            QueueType queueType = await _api.Lobby.GetQueueType().ConfigureAwait(false);

            SelectStageStarted?.Invoke(this, new SelectStageStartedEventArgs(allyPickOrder, 
                args.MyTeam[0].TeamSide, args.TheirTeam.Length, _userCellId, 
                args.Actions.Any(sa => sa.Any(a => a.Type == ActionType.Ban)), 
                queueType, availableChampionIds, 
                CurrentSessionActionList.SelectMany(a => a)
                    .First(a => 
                        a.ActorCellId == _userCellId && a.IsAllyAction && a.Type == ActionType.Pick).Id));

            _isPlanningStage = false;

            // _isSelectStageEnded = false;
            // _userActionCompleted = false;
            // _currentSessionActionCount = 0;
        }
        
        TeamMember[] test = Array.Empty<TeamMember>();
        for (int i = 0; i < CurrentTeamsData.Count; i++) {
            CurrentTeamsData[i].ApplyChanges(args.MyTeam[i]);
        }
        
        
        // TeamMember[] currentAllyTeamInfo = args.MyTeam;
        //
        // if (_previousAllyTeamInfo.Length == currentAllyTeamInfo.Length && eventType != LeagueInternalEventType.Create) {
        //     if (!_previousAllyTeamInfo.SequenceEqual(currentAllyTeamInfo, TeamMemberComparer.Instance)) {
        //         for (int i = 0; i < currentAllyTeamInfo.Length; i++) {
        //             if (currentAllyTeamInfo[i].SelectedSkinId != _previousAllyTeamInfo[i].SelectedSkinId) {
        //                 ChampionSkinChanged?.Invoke(sender, new ChampionSkinChangedEventArgs(Convert.ToInt32(currentAllyTeamInfo[i].SelectedSkinId.ToString().Length > 3 ? currentAllyTeamInfo[i].SelectedSkinId.ToString()[^3..] : 0), 
        //                     currentAllyTeamInfo[i].ChampionId, currentAllyTeamInfo[i].CellId));
        //             }
        //         }
        //     }
        // }
        // _previousAllyTeamInfo = currentAllyTeamInfo;
        
        if (args.Actions.Length > CurrentSessionActionList.Count) {
            foreach (SessionAction[] sessionActions in args.Actions.Skip(CurrentSessionActionList.Count)) {     
                AddSessionActions(sessionActions);
            }
        }
        
        for (int i = 0; i < CurrentSessionActionList.Count; i++) {
            for (int j = 0; j < CurrentSessionActionList[i].Length; j++) {
                CurrentSessionActionList[i][j].ApplyChanges(args.Actions[i][j]);
            }
        }

        
        if (args.Timer.Phase == "BAN_PICK" && _isPlanningStage) {
            _isPlanningStage = false;
            ActionRequested?.Invoke(this,new ActionRequestedEventArgs(true, 
                args.MyTeam.Select(m => GetActorPosition(m.CellId)).ToArray(), ActionType.Ban));
            UserActionRequested?.Invoke(this, new UserActionRequestedEventArgs(
                CurrentSessionActionList.SelectMany(a => a).First(a => a.Type == ActionType.Ban && a.ActorCellId == _userCellId && a.IsAllyAction).Id,
                ActionType.Ban));
        }
    }

    private void AddSessionActions(SessionAction[] sessionActions)
    {
        foreach (SessionAction sessionAction in sessionActions) {
            sessionAction.ActorCellId = GetActorPosition(sessionAction.ActorCellId);
            sessionAction.PropertyChanged += (o, e) => OnSessionActionChanged((o! as SessionAction)!, e.PropertyName!);
        }
        CurrentSessionActionList.Add(sessionActions);
    }

    private void AddTeamData(TeamMember[] team)
    {
        foreach (TeamMember member in team) {
            member.CellId = GetActorPosition(member.CellId);
            member.PropertyChanged += (o, e) => OnTeamMemberChanged((o! as TeamMember)!, e.PropertyName!);
        }
        CurrentTeamsData.AddRange(team);
    }

    private void OnSessionActionChanged(SessionAction sender, string propertyName)
    {
        switch (propertyName) {
            case nameof(SessionAction.ChampionId):
                if (!sender.IsAllyAction) {
                    break;
                }
                ChampionHovered?.Invoke(this, new ChampionHoveredEventArgs(sender.IsAllyAction, 
                    sender.ActorCellId, sender.ChampionId, sender.Type, sender.Id));
                break;
            
            case nameof(SessionAction.Completed):
                if (sender.Type == ActionType.Ban) {
                    ChampionBanned?.Invoke(this, new ChampionBannedEventArgs(sender.IsAllyAction, sender.ChampionId, sender.ActorCellId));
                } else if (sender.Type == ActionType.Pick) {
                    ChampionPicked?.Invoke(this, new ChampionPickedEventArgs(sender.IsAllyAction, sender.ActorCellId, sender.ChampionId));
                }
                break;
            
            case nameof(SessionAction.IsInProgress):
                if (!sender.IsInProgress || sender.Type == ActionType.TenBansReveal) {
                    break;
                }
                
                SessionAction[] actions = CurrentSessionActionList.First(a => a.Contains(sender));
                if (Array.IndexOf(actions, sender) > 0) {
                    break;
                }
                
                int[] actors = actions
                    .Select(a => GetActorPosition(a.ActorCellId)).OrderBy(p => p).ToArray();
                
                SessionAction? userAction = actions.FirstOrDefault(a => a.ActorCellId == _userCellId && a.IsAllyAction);
                if (userAction != null) {
                    UserActionRequested?.Invoke(this, new UserActionRequestedEventArgs(userAction.Id, userAction.Type));
                }
                
                ActionRequested?.Invoke(this, new ActionRequestedEventArgs(sender.IsAllyAction, actors, sender.Type));
                break;
        }
    }

    private void OnTeamMemberChanged(TeamMember sender, string propertyName)
    {
        switch (propertyName) {
            case nameof(TeamMember.SelectedSkinId):
                ChampionSkinChanged?.Invoke(this, new ChampionSkinChangedEventArgs(Convert.ToInt32(sender.SelectedSkinId.ToString().Length > 3 ? sender.SelectedSkinId.ToString()[^3..] : 0), sender.ChampionId, sender.CellId));
                break;
        }
        
    }

    private int GetActorPosition(int cellId) =>
        cellId >= 5 ? cellId - 5 : cellId;

}