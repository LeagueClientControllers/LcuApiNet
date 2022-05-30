using LcuApiNet.Model.Enums;

namespace LcuApiNet.EventHandlers.PickStage;

public delegate void ActionRequestedHandler(object sender, ActionRequestedEventArgs e);

public class ActionRequestedEventArgs
{
    public bool Team { get; }
        
    public int[] PlayerSelectPositions { get; }

    public ActionType ActionType { get; }


    public ActionRequestedEventArgs(bool team, int[] playerSelectPositions, ActionType actionType)
    {
        Team = team;
        PlayerSelectPositions = playerSelectPositions;
        ActionType = actionType;
    }

}