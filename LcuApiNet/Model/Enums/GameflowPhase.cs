namespace LcuApiNet.Model.Enums
{
    /// <summary>
    /// Indicates phase of the game flow in league client.
    /// </summary>
    public enum GameflowPhase
    {
        /// <summary>
        /// User wandering around client.
        /// </summary>
        None,

        /// <summary>
        /// User in the lobby.
        /// </summary>
        Lobby,

        /// <summary>
        /// Matchmaking in progress.
        /// </summary>
        Matchmaking,

        CheckedIntoTournament,
        /// <summary>
        /// User should accept or decline match.
        /// </summary>
        ReadyCheck,

        /// <summary>
        /// User selecting and/or banning champions.
        /// </summary>
        ChampSelect,

        /// <summary>
        /// 
        /// </summary>
        GameStart,

        /// <summary>
        /// 
        /// </summary>
        FailedToLaunch,

        /// <summary>
        /// Game in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// 
        /// </summary>
        Reconnect,

        /// <summary>
        /// Waiting for post game stats.
        /// </summary>
        WaitingForStats,

        /// <summary>
        /// When game is ended but user has not returned to the lobby yet.
        /// </summary>
        PreEndOfGame,

        /// <summary>
        /// Game is ended.
        /// </summary>
        EndOfGame,

        /// <summary>
        /// 
        /// </summary>
        TerminatedInError
    }
}
