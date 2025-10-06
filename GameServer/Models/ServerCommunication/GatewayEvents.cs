namespace GameServer.Models.ServerCommunication;

public class GatewayEvents
{
    public const string ServerInfo = "SERVER_INFO";
    public const string EventStarted = "EVENT_STARTED";
    public const string EventFinished = "EVENT_FINISHED";
    public const string PlayerUpdated = "PLAYER_UPDATED";
    public const string PlayerQuit = "PLAYER_QUIT";
    public const string UpdatePlayerCount = "UPDATE_PLAYER_COUNT";
    public const string PlayerSessionCreated = "PLAYER_SESSION_CREATED";
    public const string PlayerSessionDestroyed = "PLAYER_SESSION_DESTROYED";
    public const string HotSeatPlaylistReset = "HOT_SEAT_PLAYLIST_RESET";
}