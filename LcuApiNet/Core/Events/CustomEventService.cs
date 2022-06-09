using System.Collections.ObjectModel;
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
    private SessionAction[][] _previousSessionAction = Array.Empty<SessionAction[]>();
    private List<SessionAction> _currentPickingSelectPositions = new List<SessionAction>();
    private List<SessionAction> _previousPickingSelectPositions = new List<SessionAction>();    
   
    private List<SessionAction> _currentBanningSelectPositions = new List<SessionAction>();
    private List<SessionAction> _previousBanningSelectPositions = new List<SessionAction>();

    
    private int _currentSessionActionCount;
    private int _userCellId = 0;
    private long _userId;
    
    private bool _isPlanningStage;
    private bool _isPickStage = false;
    private bool _userActionCompleted;
    private bool _isSelectStageEnded;
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

        _api.LeagueEvents.GameflowPhaseChanged += (_, eventType, args) => {
            if (args == GameflowPhase.ChampSelect) {
                _champSelectInProgress = true;
            } else if (_champSelectInProgress) {
                _champSelectInProgress = false;
                SelectStageEnded?.Invoke(this, new SelectStageEndedEventArgs(true));
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
            List<SelectStageMember> allyPickOrder = new List<SelectStageMember>();
            foreach (TeamMember member in args.MyTeam) {
                allyPickOrder.Add(new SelectStageMember(member.SummonerId.ToString(), member.AssignedPosition == "" ? Role.Any : RoleConverter.FromStringToEnum(member.AssignedPosition)));
                if (member.SummonerId == _userId) {
                    _userCellId = member.CellId;
                }
            }
       
            SessionAction? inProgressSessionAction = new SessionAction();
            if (!_isPlanningStage) {
                inProgressSessionAction = args.Actions.Select(x => x.Where(s => s.IsInProgress).ToList()[0]).ToList()[0];
            }
            
            bool banStagePlanned = false;
            args.Actions.ToList().ForEach(x => x.ToList().ForEach(s => {
                if (s.Type == ActionType.Ban) {
                    banStagePlanned = true;
                    return;
                }
            }));
            
            int userCellId = _userCellId < 5 ? _userCellId : _userCellId - 5;
            ActionRequestedEventArgs? actionRequestedEventArgs = _isPlanningStage
                ? null
                : new ActionRequestedEventArgs(inProgressSessionAction.IsAllyAction,
                    new[] {
                        inProgressSessionAction.ActorCellId < 5
                            ? inProgressSessionAction.ActorCellId
                            : inProgressSessionAction.ActorCellId - 5
                    }, inProgressSessionAction.Type);        
            
            SelectStageStarted?.Invoke(sender, 
                new SelectStageStartedEventArgs(allyPickOrder, args.MyTeam[0].TeamSide, args.TheirTeam.Length, userCellId, banStagePlanned, actionRequestedEventArgs));
            
            _isPlanningStage = false;
            _isSelectStageEnded = false;
            _userActionCompleted = false;
            _currentSessionActionCount = 0;
            _currentPickingSelectPositions.Clear();
            _previousPickingSelectPositions.Clear();
            _currentBanningSelectPositions.Clear();
            _previousBanningSelectPositions.Clear();
        }
        
        TeamMember[] currentAllyTeamInfo = args.MyTeam;
        
        if (_previousAllyTeamInfo.Length == currentAllyTeamInfo.Length && eventType != LeagueInternalEventType.Create) {
            if (!_previousAllyTeamInfo.SequenceEqual(currentAllyTeamInfo, TeamMemberComparer.Instance)) {
                for (int i = 0; i < currentAllyTeamInfo.Length; i++) {
                    if (currentAllyTeamInfo[i].SelectedSkinId != _previousAllyTeamInfo[i].SelectedSkinId) {
                        ChampionSkinChanged?.Invoke(sender, new ChampionSkinChangedEventArgs(Convert.ToInt32(currentAllyTeamInfo[i].SelectedSkinId.ToString().Length > 3 ? currentAllyTeamInfo[i].SelectedSkinId.ToString()[^3..] : 0), 
                            currentAllyTeamInfo[i].ChampionId, currentAllyTeamInfo[i].CellId));
                    }
                }
            }
        }
        _previousAllyTeamInfo = currentAllyTeamInfo;
        
        if (args.Timer.Phase == "BAN_PICK" && _isPlanningStage) {
            PlanningStageEnded?.Invoke(sender, new PlanningStageEndedEventArgs(true));
            _isPlanningStage = false;
        }        

        SessionAction[][]? newActions;

        if (args.Actions.Length == _currentSessionActionCount) {
            List<List<SessionAction>> changedInnerActions = new List<List<SessionAction>>();
            for (int i = 0; i < args.Actions.Length; i++) {
                changedInnerActions.Add(args.Actions[i].Except(_previousSessionAction[i], SessionActionComparer.Instance).ToList());
            }
            
            if (changedInnerActions.Count == 0) {
                return;
            }
            newActions = changedInnerActions.Select(x => x.ToArray()).ToArray();
        } else {
            newActions = args.Actions[_currentSessionActionCount..];
        }

        _previousSessionAction = args.Actions;
        foreach (SessionAction[] actions in newActions) {
            foreach (SessionAction action in actions) {
                action.ActorCellId = action.ActorCellId >= 5 ? action.ActorCellId - 5 : action.ActorCellId;
                if (!action.Completed) {
                    if (action.ChampionId != 0) {
                        ChampionHovered?.Invoke(sender, 
                            new ChampionHoveredEventArgs(action.IsAllyAction, action.ActorCellId, action.ChampionId, action.Type, action.Id < 5 ? action.Id : action.Id - 5));
                    }
                    // if (action.Type == ActionType.Pick) {
                    //     if (action.IsInProgress) {
                    //         if (_currentActionSelectPositions.Where(a => a.Id == action.Id).ToList().Count == 0) {  
                    //             _currentActionSelectPositions.Add(action);
                    //         }
                    //     }
                    // }
                    if (action.IsInProgress) {
                        if (action.Type == ActionType.Pick) {
                            if (_currentPickingSelectPositions.Where(a => a.Id == action.Id).ToList().Count == 0 || _currentPickingSelectPositions.Count == 0) {  
                                _currentPickingSelectPositions.Add(action);

                            }

                            _isPickStage = true;
                        }

                        if (action.Type == ActionType.Ban) {
                            if (_currentBanningSelectPositions.Where(a => a.Id == action.Id).ToList().Count == 0) {  
                                _currentBanningSelectPositions.Add(action);
                            }

                            _isPickStage = false;
                        }
                        
                        if (action.ActorCellId == _userCellId && !_userActionCompleted) {
                            _userActionCompleted = true;
                            UserActionRequested?.Invoke(sender, new UserActionRequestedEventArgs(action.Id, action.Type));
                        }
                    }
                    
                    continue;
                }
                
                if (action.Type == ActionType.Pick) {
                    _userActionCompleted = false;
                    ChampionPicked?.Invoke(sender, 
                        new ChampionPickedEventArgs(action.IsAllyAction, action.ActorCellId, action.ChampionId));
                } else if (action.Type == ActionType.Ban && !_isPickStage) {
                    _userActionCompleted = false;
                    ChampionBanned?.Invoke(sender, 
                        new ChampionBannedEventArgs(action.ActorCellId, action.ChampionId));
                }
            }
        }

        if (eventType != LeagueInternalEventType.Create) {
            if (_currentPickingSelectPositions.Count != _previousPickingSelectPositions.Count) {
                IEnumerable<SessionAction> except = _currentPickingSelectPositions.Except(_previousPickingSelectPositions);
                if (except.ToList().Count != 0 && _currentPickingSelectPositions.Count != 0) {
                    ActionRequested?.Invoke(sender, new ActionRequestedEventArgs(_currentPickingSelectPositions[^1].IsAllyAction, 
                        except.Select(x => x.ActorCellId).ToArray(), ActionType.Pick));
                }
                _previousPickingSelectPositions.AddRange(_currentPickingSelectPositions);
            }
            
            if (_currentBanningSelectPositions.Count != _previousBanningSelectPositions.Count && !_isPickStage) {
                IEnumerable<SessionAction> except = _currentBanningSelectPositions.Except(_previousBanningSelectPositions);
                if (except.ToList().Count != 0 && _currentBanningSelectPositions.Count != 0) {
                    List<SessionAction> allyBanRequested = except.Where(s => s.IsAllyAction).ToList();
                    List<SessionAction> enemyBanRequested = except.Where(s => !s.IsAllyAction).ToList();
                    
                    ActionRequested?.Invoke(sender, new ActionRequestedEventArgs(true, 
                        allyBanRequested.Select(x => x.ActorCellId).ToArray(), ActionType.Ban));

                    if (enemyBanRequested.Count != 0) {
                        ActionRequested?.Invoke(sender, new ActionRequestedEventArgs(false, 
                            enemyBanRequested.Select(x => x.ActorCellId).ToArray(), ActionType.Ban));
                    }
                }
                _previousBanningSelectPositions.AddRange(_currentBanningSelectPositions);
            }
        }

        if (_isPickStage) {
            _currentBanningSelectPositions.Clear();
        }
        
        _currentSessionActionCount = args.Actions.Length;
    }
}