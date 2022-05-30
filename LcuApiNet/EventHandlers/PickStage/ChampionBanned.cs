namespace LcuApiNet.EventHandlers.PickStage;

public delegate void ChampionBannedHandler(object sender, ChampionBannedEventArgs e);
public class ChampionBannedEventArgs
{
    public int PlayerSelectPosition { get; }
    
    public int ChampionId { get; }

    public ChampionBannedEventArgs(int playerSelectPosition, int championId)
    {
        PlayerSelectPosition = playerSelectPosition;
        ChampionId = championId;
    }
    
}