using LcuApiNet.Model.Enums;

namespace LcuApiNet.EventHandlers.PickStage;

public delegate void ChampionHoveredHandler(object sender, ChampionHoveredEventArgs e); 

public class ChampionHoveredEventArgs
{
    public bool IsAllyAction { get; }
    
    public int PlayerSelectPosition { get; }
    
    public int ChampionId { get; }
    
    public int SessionActionId { get; }
    
    public ActionType ActionType { get; } 
    
    public ChampionHoveredEventArgs(bool isAllyAction, int playerSelectPosition, int championId, ActionType actionType, int sessionActionId)
    {
        IsAllyAction = isAllyAction;
        PlayerSelectPosition = playerSelectPosition;
        ChampionId = championId;
        ActionType = actionType;
        SessionActionId = sessionActionId;
    }

}