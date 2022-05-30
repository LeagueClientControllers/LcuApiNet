namespace LcuApiNet.EventHandlers.PickStage;

public delegate void SelectStageEndedHandler(object sender, SelectStageEndedEventArgs e);

public class SelectStageEndedEventArgs
{
    public bool SelectStageEnded { get; }

    public SelectStageEndedEventArgs(bool selectStageEnded)
    {
        SelectStageEnded = selectStageEnded;
    }
}