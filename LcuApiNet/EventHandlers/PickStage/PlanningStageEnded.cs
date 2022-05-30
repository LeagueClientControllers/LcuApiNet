namespace LcuApiNet.EventHandlers.PickStage;

public delegate void PlanningStageEndedHandler(object sender, PlanningStageEndedEventArgs e);


public class PlanningStageEndedEventArgs
{
    public bool IsPlanningStageEnded { get; }
    
    public PlanningStageEndedEventArgs(bool isPlanningStageEnded)
    {
        IsPlanningStageEnded = isPlanningStageEnded;
    }

}