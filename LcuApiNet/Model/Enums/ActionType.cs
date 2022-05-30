using Ardalis.SmartEnum;

namespace LcuApiNet.Model.Enums;

public class ActionType : SmartEnum<ActionType> {
    
    /// <summary>
    /// 
    /// </summary>
    public static ActionType Pick = new ActionType("pick", 1);
    
    /// <summary>
    /// 
    /// </summary>
    public static ActionType Ban = new ActionType("ban", 2);
    
    /// <summary>
    /// 
    /// </summary>
    public static ActionType TenBansReveal = new ActionType("ten_bans_reveal", 3);
    
    public ActionType(string name, int value) : 
        base(name, value) {
    }
}
