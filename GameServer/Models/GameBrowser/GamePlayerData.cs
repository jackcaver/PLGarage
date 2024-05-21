namespace GameServer.Models.GameBrowser
{
    public class GamePlayerData
    {
        public int PlayerId { get; set; }
        public bool HasFinished => State == GameState.FINISHED;
        public GameState State { get; set; }
    }
}
