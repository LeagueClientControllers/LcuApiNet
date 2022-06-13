using LcuApiNet.Model.Enums;

namespace LcuApiNet.EventHandlers.PickStage;

public delegate void SelectStageStartedHandler(object sender, SelectStageStartedEventArgs e);
public class SelectStageStartedEventArgs
{
    public List<SelectStageMember> AllyPickOrder { get; }

    public int EnemiesCount { get; }
    public int UserPosition { get; }
    public bool UserOnBlueSide { get; }

    public bool BanStagePlanned { get; }

    public SelectStageStartedEventArgs(List<SelectStageMember> allyPickOrder, bool userOnBlueSide, int enemiesCount, int userPosition,  bool banStagePlanned)
    {
        AllyPickOrder  = allyPickOrder;
        UserOnBlueSide = userOnBlueSide;
        EnemiesCount   = enemiesCount;
        UserPosition   = userPosition;
        BanStagePlanned = banStagePlanned;
    }
}

public class SelectStageMember
{
    public string UserId { get; }
    public Role Role { get; }
    
    public SelectStageMember(string userId, Role role)
    {
        UserId = userId;
        Role = role;
    }
}