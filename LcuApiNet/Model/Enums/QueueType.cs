using Ardalis.SmartEnum;


namespace LcuApiNet.Model.Enums;

// public enum QueueType
// {
//     Flex = 440,
//     BlindPick = 430,
//     Ranked = 420,
//     DraftPick = 400
// }

public class QueueType : SmartEnum<QueueType> {
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType Custom = new QueueType("Custom", 0);
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType DraftPick = new QueueType("DraftPick", 400);
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType RankedSolo = new QueueType("RankedSolo", 420);
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType BlindPick = new QueueType("BlindPick", 430);
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType RankedFlex = new QueueType("RankedFlex", 440);
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType Aram = new QueueType("Aram", 450);
    
    /// <summary>
    /// 
    /// </summary>
    public static QueueType Clash = new QueueType("Clash", 700);
    public QueueType(string name, int value) : 
        base(name, value) {
    }
}
