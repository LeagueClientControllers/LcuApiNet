using Ardalis.SmartEnum;
namespace LcuApiNet.Model.Enums;

public class Role : SmartEnum<Role> {
    
    /// <summary>
    /// 
    /// </summary>
    public static Role Top = new Role("Top", 1);
    
    /// <summary>
    /// 
    /// </summary>
    public static Role Jungle = new Role("Jungle", 2);
    
    /// <summary>
    /// 
    /// </summary>
    public static Role Middle = new Role("Middle", 3);
    
    /// <summary>
    /// 
    /// </summary>
    public static Role Bottom = new Role("Bottom", 4);
    
    /// <summary>
    /// 
    /// </summary>
    public static Role Support = new Role("Support", 5);
    
    /// <summary>
    /// 
    /// </summary>
    public static Role Fill = new Role("Fill", 6);

    /// <summary>
    /// 
    /// </summary>
    public static Role Any = new Role("Any", 7);
    
    public Role(string name, int value) :
        base(name, value) { }
}