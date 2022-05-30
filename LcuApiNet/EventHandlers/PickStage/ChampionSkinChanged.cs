namespace LcuApiNet.EventHandlers.PickStage;

public delegate void ChampionSkinChangedHandler(object sender, ChampionSkinChangedEventArgs e);

public class ChampionSkinChangedEventArgs
{
    public int SkinId { get; }
    public int ChampionId { get; }
    public int PlayerSelectPosition { get; }

    public ChampionSkinChangedEventArgs(int skinId, int championId, int playerSelectPosition)
    {
        SkinId = skinId;
        ChampionId = championId;
        PlayerSelectPosition = playerSelectPosition;
    }

}