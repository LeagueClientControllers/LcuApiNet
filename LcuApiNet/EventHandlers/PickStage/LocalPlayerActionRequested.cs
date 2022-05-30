using LcuApiNet.Model.Enums;
using LcuApiNet.Utilities;

namespace LcuApiNet.EventHandlers.PickStage;

public delegate void UserActionRequestedEventHandler(object sender, UserActionRequestedEventArgs e); 
public class UserActionRequestedEventArgs
{
    public int ActionId { get; }
    public ActionType ActionType { get; }
    
    public UserActionRequestedEventArgs(int actionId, ActionType actionType)
    {
        ActionId = actionId;
        ActionType = actionType;
    }
}