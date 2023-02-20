using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.Games;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers.Common
{
    public class GamesController : Controller
    {
        private readonly Database database;

        public GamesController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("single_player_games/create_finish_and_post_stats.xml")]
        public IActionResult PostSinglePlayerGameStats(Game game, GamePlayer game_player, GamePlayerStats game_player_stats)
        {
            int GameID = database.Games.Count() + 1,
                GamePlayerID = database.GamePlayers.Count() + 1,
                GamePlayerStatsID = database.GamePlayerStats.Count() + 1;
            string FormScore = Request.Form["game_player_stats[score]"];
            string FormFinishTime = Request.Form["game_player_stats[finish_time]"];
            string FormPoints = Request.Form["game_player_stats[points]"];
            string FormVolatility = Request.Form["game_player_stats[volatility]"];
            string FormDeviation = Request.Form["game_player_stats[deviation]"];

            if (FormScore != null)
                game_player_stats.score = float.Parse(FormScore, CultureInfo.InvariantCulture.NumberFormat);
            if (FormFinishTime != null)
                game_player_stats.finish_time = float.Parse(FormFinishTime, CultureInfo.InvariantCulture.NumberFormat);
            if (FormPoints != null)
                game_player_stats.points = float.Parse(FormPoints, CultureInfo.InvariantCulture.NumberFormat);
            if (FormVolatility != null)
                game_player_stats.volatility = float.Parse(FormVolatility, CultureInfo.InvariantCulture.NumberFormat);
            if (FormDeviation != null)
                game_player_stats.deviation = float.Parse(FormDeviation, CultureInfo.InvariantCulture.NumberFormat);

            var score = database.Scores.FirstOrDefault(match => match.PlayerId == game.host_player_id
                && match.SubKeyId == game.track_idx
                && match.SubGroupId == (int)game.game_type + 700
                && match.Platform == game.platform
                && match.PlaygroupSize == game_player_stats.playgroup_size);

            database.Games.Add(new GameData
            {
                Id = GameID,
                GameState = game.game_state,
                GameType = game.game_type,
                HostPlayerId = game.host_player_id,
                IsRanked = game.is_ranked,
                Name = game.name,
                Platform = game.platform,
                TrackIdx = game.track_idx
            });
            database.GamePlayers.Add(new GamePlayerData
            {
                Id = GamePlayerID,
                GameId = GameID,
                GameState = game_player.game_state,
                PlayerId = game_player.player_id,
                TeamId = game_player.team_id
            });
            database.GamePlayerStats.Add(new GamePlayerStatsData
            {
                Id = GamePlayerStatsID,
                GameId = GameID,
                Deviation = game_player_stats.deviation,
                FinishPlace = game_player_stats.finish_place,
                FinishTime = game_player_stats.finish_time,
                IsComplete = game_player_stats.is_complete,
                IsWinner = game_player_stats.is_winner,
                LapsCompleted = game_player_stats.laps_completed,
                NumKills = game_player_stats.num_kills,
                PlaygroupSize = game_player_stats.playgroup_size,
                Points = game_player_stats.points,
                Score = game_player_stats.score,
                Stat1 = game_player_stats.stat_1,
                Stat2 = game_player_stats.stat_2,
                Volatility = game_player_stats.volatility
            });
            if (score != null)
            {
                if (score.FinishTime > game_player_stats.finish_time)
                {
                    score.FinishTime = game_player_stats.finish_time;
                    score.UpdatedAt = DateTime.UtcNow;
                }
                if (score.Points < game_player_stats.score)
                {
                    score.Points = game_player_stats.score;
                    score.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                database.Scores.Add(new Score
                {
                    CreatedAt = DateTime.UtcNow,
                    FinishTime = game_player_stats.finish_time,
                    Platform = game.platform,
                    PlayerId = game.host_player_id,
                    PlaygroupSize = game_player_stats.playgroup_size,
                    Points = game_player_stats.score,
                    SubGroupId = (int)game.game_type + 700,
                    SubKeyId = game.track_idx,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            database.SaveChanges();

            var resp = new Response<List<game>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<game> { new game {
                    id = database.Games.Count(),
                    game_player_id = database.GamePlayers.Count(),
                    game_player_stats_id = database.GamePlayerStats.Count()
                } }
            };

            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}