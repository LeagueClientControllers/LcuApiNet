namespace LcuApiNet.EventHandlers.PickStage;

public delegate void ChampionPickedHandler(object sender, ChampionPickedEventArgs e); 


public class ChampionPickedEventArgs
{
    public bool IsAllyAction { get; }
    
    public int PlayerSelectPosition { get; }
    
    public int ChampionId { get; }

    public ChampionPickedEventArgs(bool isAllyAction, int playerSelectPosition, int championId)
    {
        IsAllyAction = isAllyAction;
        PlayerSelectPosition = playerSelectPosition;
        ChampionId = championId;
    }
}