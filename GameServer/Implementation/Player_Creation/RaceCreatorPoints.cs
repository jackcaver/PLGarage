using System.Collections.Generic;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;

namespace GameServer.Implementation.Player_Creation
{
    public static class RaceCreatorPoints
    {
        public const int PerRace = 25; // remember this for xp modifier
        private const int MaxChainLength = 50; // limit for how many to go back in case an infinite loop is here, can adjust if needed but 50 should be ok?

        public static void Add(Database database, PlayerCreationData creation)
        {

            if (creation == null || !creation.IsMNR) return;

            var credited = new HashSet<int>();

            // Current remixer
            Award(database, creation, creation.PlayerId, credited);

            
            List<PlayerCreationData> chain = RemixChain(database, creation);

            // Previous remixer
            PlayerCreationData previous = chain.FirstOrDefault();
            if (previous != null && previous.PlayerId == creation.ParentPlayerId)
                Award(database, previous, creation.ParentPlayerId, credited);

            // Original uploader (only if they are registered under this instance)
            PlayerCreationData original = chain.LastOrDefault(c => c.PlayerId == creation.OriginalPlayerId);
            if (original != null)
                Award(database, original, creation.OriginalPlayerId, credited);
        }

        private static List<PlayerCreationData> RemixChain(Database database, PlayerCreationData creation)
        {
            var chain = new List<PlayerCreationData>();
            var seen = new HashSet<int> { creation.PlayerCreationId };
            int parentId = creation.ParentCreationId;

            while (parentId > 0 && seen.Add(parentId) && chain.Count < MaxChainLength)
            {
                PlayerCreationData parent = database.PlayerCreations.FirstOrDefault(c => c.PlayerCreationId == parentId);
                if (parent == null) break;   // original deleted or something

                chain.Add(parent);
                parentId = parent.ParentCreationId;
            }

            return chain;
        }

        private static void Award(Database database, PlayerCreationData earnedOn, int playerId, HashSet<int> credited)
        {
            // only award once
            if (playerId <= 0 || !earnedOn.IsMNR || !credited.Add(playerId)) return;

            if (!database.Users.Any(u => u.UserId == playerId && !u.IsBanned && u.Username != "ufg")) return;

            database.PlayerCreationPoints.Add(new PlayerCreationPoint
            {
                PlayerCreationId = earnedOn.PlayerCreationId,
                PlayerId = playerId,
                Platform = earnedOn.Platform,
                Type = earnedOn.Type,
                CreatedAt = TimeUtils.Now,
                Amount = PerRace
            });
        }
    }
}