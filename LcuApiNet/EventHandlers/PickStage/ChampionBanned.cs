namespace LcuApiNet.EventHandlers.PickStage;

public delegate void ChampionBannedHandler(object sender, ChampionBannedEventArgs e);
public class ChampionBannedEventArgs
{
    public bool IsAllyAction { get; }
    
    public int PlayerSelectPosition { get; }
    
    public int ChampionId { get; }

    public ChampionBannedEventArgs(bool isAllyAction, int championId, int playerSelectPosition)
    {
        IsAllyAction = isAllyAction;
        ChampionId = championId;
        PlayerSelectPosition = playerSelectPosition;
    }
    
}