﻿using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using System.Collections.Generic;
using GameServer.Implementation.Common;

namespace GameServer.Implementation.Player_Creation
{
    public class FavoritePlayerCreations
    {
        public static string AddToFavorites(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!Creation.IsHeartedByMe(user.UserId))
            {
                database.HeartedPlayerCreations.Add(new HeartedPlayerCreation
                {
                    HeartedPlayerCreationId = Creation.PlayerCreationId,
                    UserId = user.UserId,
                    HeartedAt = DateTime.UtcNow,
                });
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RemoveFromFavorites(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var HeartedCreation = database.HeartedPlayerCreations.FirstOrDefault(match => match.HeartedPlayerCreationId == id && match.UserId == user.UserId);

            if (HeartedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.HeartedPlayerCreations.Remove(HeartedCreation);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string ListFavorites(Database database, string player_id_or_username)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == player_id_or_username || match.UserId.ToString() == player_id_or_username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var favoriteCrations = database.HeartedPlayerCreations.Where(match => match.UserId == user.UserId).ToList();
            List<favorite_player_creation> favoriteCreationsList = new List<favorite_player_creation> { };

            foreach (var Creation in favoriteCrations)
            {
                var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.HeartedPlayerCreationId);
                if (creation != null)
                {
                    favoriteCreationsList.Add(new favorite_player_creation
                    {
                        player_creation_id = Creation.HeartedPlayerCreationId,
                        player_creation_name = creation.Name
                    });
                }
            }

            var resp = new Response<List<favorite_player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<favorite_player_creations> { new favorite_player_creations {
                    total = favoriteCreationsList.Count,
                    PlayerCreations = favoriteCreationsList
                } }
            };
            return resp.Serialize();
        }
    }
}
