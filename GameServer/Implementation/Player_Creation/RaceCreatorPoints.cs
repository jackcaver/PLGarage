using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;

namespace GameServer.Implementation.Player_Creation
{
    public static class RaceCreatorPoints
    {
        public const int PerRace = 25; // remember to come back for xp modifiers

        public static void Add(Database database, PlayerCreationData creation)
        {

            if (creation == null || !creation.IsMNR) return;

            database.PlayerCreationPoints.Add(new PlayerCreationPoint
            {
                PlayerCreationId = creation.PlayerCreationId,
                PlayerId = creation.PlayerId,
                Platform = creation.Platform,
                Type = creation.Type,
                CreatedAt = TimeUtils.Now,
                Amount = PerRace
            });
        }
    }
}