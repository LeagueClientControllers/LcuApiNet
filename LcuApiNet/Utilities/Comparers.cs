using LcuApiNet.Model.ClientModels.ChampSelectModels;

namespace LcuApiNet.Utilities;

public class SessionActionComparer : IEqualityComparer<SessionAction>
{
    public bool Equals(SessionAction? x, SessionAction? y)
    {
        return x.Id == y.Id && x.ChampionId == y.ChampionId && 
               x.ActorCellId == y.ActorCellId && x.Completed == y.Completed && 
               x.Type == y.Type && x.IsAllyAction == y.IsAllyAction && x.IsInProgress == y.IsInProgress;
    }

    public int GetHashCode(SessionAction obj)
    {
        return obj.Id.GetHashCode() ^ obj.ChampionId.GetHashCode() ^ 
               obj.ActorCellId.GetHashCode() ^ obj.Completed.GetHashCode() ^ 
               obj.Type.GetHashCode() ^ obj.IsAllyAction.GetHashCode() ^ obj.IsInProgress.GetHashCode(); 
    }

    public static SessionActionComparer Instance = new SessionActionComparer();
}

public class TeamMemberComparer : IEqualityComparer<TeamMember>
{
    public bool Equals(TeamMember? x, TeamMember? y)
    {
        return x.CellId == y.CellId && x.ChampionId == y.ChampionId && x.Spell1Id == y.Spell1Id &&
               x.Spell2Id == y.Spell2Id && x.SummonerId == y.SummonerId &&
               x.ChampionPickIntent == y.ChampionPickIntent && x.SelectedSkinId == y.SelectedSkinId &&
               x.WardSkinId == y.WardSkinId && x.AssignedPosition == y.AssignedPosition &&
               x.EntitledFeatureType == y.EntitledFeatureType && x.TeamSide == y.TeamSide;
    }

    public int GetHashCode(TeamMember obj)
    {
        return obj.CellId.GetHashCode() ^ obj.ChampionId.GetHashCode() ^ obj.Spell1Id.GetHashCode() ^ 
               obj.Spell2Id.GetHashCode() ^ obj.SummonerId.GetHashCode() ^ obj.ChampionPickIntent.GetHashCode() ^
               obj.SelectedSkinId.GetHashCode() ^ obj.WardSkinId.GetHashCode() ^ obj.AssignedPosition.GetHashCode() ^ 
               obj.EntitledFeatureType.GetHashCode() ^ obj.TeamSide.GetHashCode();
    }
    
    public static TeamMemberComparer Instance = new TeamMemberComparer();

}

