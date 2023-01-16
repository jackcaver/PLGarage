namespace GameServer.Models.PlayerData.Games
{
    public enum GameState
    {
        PENDING,
        ACTIVE,
        FINISHED,
        QUIT,
        QUIT_ON,
        CONCEDE,
        CONCEDE_ON,
        DISCONNECTED,
        DISCONNECTED_ON,
        DIVERGENCE,
        CANCELLED,
        FORFEIT,
        FORFEIT_ON,
        FRIENDLY_QUIT,
        FRIENDLY_QUIT_ON,
        PROCESSED
    }
}
