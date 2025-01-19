using GameServer.Models.Config;
using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using GameServer.Models.Request;
using GameServer.Utils;
using GameServer.Implementation.Common;
using System.Globalization;
using GameServer.Models.PlayerData;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player
{
    // TODO: Move POI to DB?
    public class ModMileImpl
    {
        public static string TravelAwards(Database database, Guid SessionID, int per_page, int page)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users
                .Include(x => x.AwardUnlocks)
                .FirstOrDefault(match => match.Username == session.Username);

            // TODO: Should we have an error if user not found here?

            int globalPoints = database.TravelPoints.Sum(p => p.Amount);
            int travelPoints = user != null ? user.TravelPoints.Sum(p => p.Amount) : 0;

            // TODO: Can below be consolidated?
            var awardList = new List<TravelAward>();
            foreach (var award in ModMileConfig.Instance.TravelAwards)
            {
                bool unlocked = award.IsGlobalPoints ? (globalPoints >= award.Points) : (travelPoints >= award.Points);
                bool new_unlock = unlocked && (user == null || user.AwardUnlocks.FirstOrDefault(match => match.Name == award.Name) == null);
                if (new_unlock && user != null)
                {
                    database.AwardUnlocks.Add(new AwardUnlock
                    {
                        Player = user,
                        Name = award.Name
                    });
                    database.SaveChanges();
                }
                awardList.Add(new TravelAward
                {
                    AwardHash = award.Name,
                    AwardType = award.Type,
                    GlobalPoints = award.IsGlobalPoints ? award.Points : 0,
                    IndividualPoints = award.IsGlobalPoints ? 0 : award.Points,
                    IsGlobalType = award.IsGlobalPoints,
                    Name = award.Name,
                    Unlocked = unlocked,
                    NewUnlock = new_unlock
                });
            }
            var resp = new Response<TravelAwardsResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new TravelAwardsResponse { TravelAwards = awardList }
            };
            return resp.Serialize();
        }

        // TODO: This can be cleaned up further, but is fine for now
        public static string FeaturedCities(Database database, Guid SessionID, int per_page, int page)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users
                .Include(x => x.AwardUnlocks)
                .FirstOrDefault(match => match.Username == session.Username);

            var poiKeys = ModMileConfig.Instance.PointsOfInterest
                .Select(x => x.Key)
                .ToArray();
            var poiCounts = database.POIVisits
                .Where(match => poiKeys.Contains(match.PointOfInterestId))  // TODO: Does this slow down the database fetch? This sanity check isnt strictly needed imo
                .GroupBy(match => match.PointOfInterestId)
                .ToDictionary(match => match.Key, match => match.Count());

            var cityList = new List<City>();
            foreach (var city in ModMileConfig.Instance.Cities)
            {
                bool new_unlock = false;
                foreach (var poi in ModMileConfig.Instance.PointsOfInterest.Where(match => match.Value.CityId == city.Key))
                {
                    int visits = poiCounts[poi.Key];
                    foreach (var award in poi.Value.Awards)
                    {
                        new_unlock = visits >= award.CheckIns && (user == null || user.AwardUnlocks.FirstOrDefault(match => match.Name == award.Name) == null);
                        if (new_unlock) break;
                    }
                    if (new_unlock) break;
                }

                cityList.Add(new City
                {
                    Name = city.Value.Name,
                    HasNewUnlocked = new_unlock,
                    Id = city.Key,
                    Latitude = city.Value.Latitude,
                    Longitude = city.Value.Longitude,
                    U = city.Value.U,
                    V = city.Value.V
                });
            }
            var resp = new Response<CitiesResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new CitiesResponse { Cities = cityList }
            };
            return resp.Serialize();
        }

        // TODO: This can be cleaned up further, but is fine for now
        public static string POIList(Database database, Guid SessionID, int per_page, int page, int city_id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users
                .Include(x => x.AwardUnlocks)
                .FirstOrDefault(match => match.Username == session.Username);

            if (!ModMileConfig.Instance.Cities.ContainsKey(city_id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new EmptyResponse()
                };
                return errorResp.Serialize();
            }

            var poiKeys = ModMileConfig.Instance.PointsOfInterest
                .Where(x => x.Value.CityId == city_id)
                .Select(x => x.Key)
                .ToArray();
            var poiCounts = database.POIVisits
                .Where(match => poiKeys.Contains(match.PointOfInterestId))  // TODO: Does this slow down the database fetch? This sanity check isnt strictly needed imo
                .GroupBy(match => match.PointOfInterestId)
                .ToDictionary(match => match.Key, match => match.Count());

            var poiList = new List<POI>();
            foreach (var poi in ModMileConfig.Instance.PointsOfInterest.Where(match => match.Value.CityId == city_id))
            {
                bool new_unlock = false;
                int visits = poiCounts[poi.Key];
                int unlocked = 0;
                foreach (var award in poi.Value.Awards)
                    if (visits >= award.CheckIns)
                        unlocked++;
                foreach (var award in poi.Value.Awards)
                {
                    new_unlock = visits >= award.CheckIns && (user == null || user.AwardUnlocks.FirstOrDefault(match => match.Name == award.Name) == null);
                    if (new_unlock) break;
                }
                poiList.Add(new POI
                {
                    Name = poi.Value.Name,
                    GlobalCheckinCount = visits,
                    Id = poi.Key,
                    Latitude = poi.Value.Latitude,
                    Longitude = poi.Value.Longitude,
                    U = poi.Value.U,
                    V = poi.Value.V,
                    Locked = unlocked < poi.Value.Awards.Count,
                    NewUnlock = new_unlock
                });
            }
            var resp = new Response<POIListResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new POIListResponse { PointsOfInterest = poiList }
            };
            return resp.Serialize();
        }

        // TODO: This can be cleaned up further, but is fine for now
        public static string POIShow(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users
                .Include(x => x.AwardUnlocks)
                .FirstOrDefault(match => match.Username == session.Username);

            if (!ModMileConfig.Instance.PointsOfInterest.ContainsKey(id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new EmptyResponse()
                };
                return errorResp.Serialize();
            }

            int visits = database.POIVisits.Count(match => match.PointOfInterestId == id);

            var poiAwards = new List<POIAward>();
            foreach (var award in ModMileConfig.Instance.PointsOfInterest[id].Awards)
            {
                bool new_unlock = visits >= award.CheckIns && (user == null || user.AwardUnlocks.FirstOrDefault(match => match.Name == award.Name) == null);
                if (new_unlock && user != null)
                {
                    database.AwardUnlocks.Add(new AwardUnlock
                    {
                        Player = user,
                        Name = award.Name
                    });
                    database.SaveChanges();
                }
                poiAwards.Add(new POIAward
                {
                    AwardHash = award.Name,
                    AwardType = award.Type,
                    Name = award.Name,
                    RequiredCheckins = award.CheckIns,
                    Locked = visits < award.CheckIns,
                    NewUnlock = new_unlock
                });
            }
            var resp = new Response<List<POIResponse>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new POIResponse { Awards = poiAwards }]
            };
            return resp.Serialize();
        }

        public static string CheckinStatus(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users
                .Include(x => x.POIVisits)
                .FirstOrDefault(match => match.Username == session.Username);

            if (!ModMileConfig.Instance.PointsOfInterest.ContainsKey(id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new EmptyResponse()
                };
                return errorResp.Serialize();
            }

            var poiVisit = user.POIVisits
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault(match => match.PointOfInterestId == id);

            if (user != null && (poiVisit == null
                || (!ModMileConfig.Instance.RequireUniquePlayersToCheckIn
                    && poiVisit.CreatedAt <= DateTime.UtcNow.AddHours(-2))))
            {
                database.POIVisits.Add(new POIVisit
                {
                    CreatedAt = DateTime.UtcNow,
                    Player = user,
                    PointOfInterestId = id,
                });
                database.SaveChanges();
            }

            var poi = ModMileConfig.Instance.PointsOfInterest[id];

            var resp = new Response<List<CheckinStatus>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new CheckinStatus
                {
                    Id = id,
                    Latitude = poi.Latitude,
                    Longitude = poi.Longitude,
                    Name = poi.Name,
                    Radius = poi.Radius,
                    U = poi.U,
                    V = poi.V
                } ]
            };
            return resp.Serialize();
        }

        // TODO: This can be cleaned up further, but is fine for now
        public static string CheckinCreate(Database database, Guid SessionID, float latitude, float longitude)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            latitude = float.Parse(latitude.ToString("0.000", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); //gps isn't 100% accurate so here is my way to get around it
            longitude = float.Parse(longitude.ToString("0.000", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            float distance = 0;
            if (user.HasCheckedInBefore)
                distance = GetDistance(user.LastLatitude, user.LastLongitude, latitude, longitude);
            int travelPoints = (int)(distance / 1000);

            user.LastLatitude = latitude;
            user.LastLongitude = longitude;
            user.ModMiles += distance;
            if (travelPoints != 0)
            {
                database.TravelPoints.Add(new TravelPoint
                {
                    CreatedAt = DateTime.UtcNow,
                    Amount = travelPoints,
                    Player = user
                });
            }
            if (!user.HasCheckedInBefore)
                user.HasCheckedInBefore = true;
            database.SaveChanges();

            // TODO: I honestly cbf to look at this right now, we need to look at this later and optimise
            var poiKeys = ModMileConfig.Instance.PointsOfInterest
                .Select(x => x.Key)
                .ToArray();
            var poiCounts = database.POIVisits
                .Where(match => poiKeys.Contains(match.PointOfInterestId))  // TODO: Does this slow down the database fetch? This sanity check isnt strictly needed imo
                .GroupBy(match => match.PointOfInterestId)
                .ToDictionary(match => match.Key, match => match.Count());

            bool new_unlock = false;
            foreach (var city in ModMileConfig.Instance.Cities)
            {
                foreach (var poi in ModMileConfig.Instance.PointsOfInterest.Where(match => match.Value.CityId == city.Key))
                {
                    int visits = poiCounts[poi.Key];
                    foreach (var award in poi.Value.Awards)
                    {
                        new_unlock = visits < award.CheckIns && (user == null || user.AwardUnlocks.FirstOrDefault(match => match.Name == award.Name) == null);
                        if (new_unlock) break;
                    }
                    if (new_unlock) break;
                }
                if (new_unlock) break;
            }

            var resp = new Response<List<CheckinCreate>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new CheckinCreate
                {
                    Id = GetPOI(latitude, longitude),
                    GlobalMiles = database.Users.Where(match => match.PlayedMNR).Sum(p => p.ModMiles),//Global Mod Miles Distance
                    GlobalPoints = database.TravelPoints.Sum(p => p.Amount),//Global Travel Points
                    LastMiles = distance,//Mod Miles Distance
                    LastPoints = travelPoints,//Travel Points Earned
                    TotalMiles = user.ModMiles,//Total Mod Miles Distance
                    TravelPoints = user.TravelPoints.Sum(p => p.Amount),//Total Travel Points and also My Travel Points
                    U = GetU(longitude),
                    V = GetV(latitude),
                    NewUnlock = new_unlock
                } ]
            };
            return resp.Serialize();
        }

        public static string LeaderboardCities(Database database, int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order)
        {
            var leaderboard = new List<ModMileLeaderboardStat>();



            foreach (var city in ModMileConfig.Instance.Cities)
            {
                int visits = 0;

                if (timespan == Timespan.last_week)
                    foreach (var poi in ModMileConfig.Instance.PointsOfInterest.Where(match => match.Value.CityId == city.Key))
                        visits += database.POIVisits.Count(match => match.PointOfInterestId == poi.Key && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7));
                else if (timespan == Timespan.this_week)
                    foreach (var poi in ModMileConfig.Instance.PointsOfInterest.Where(match => match.Value.CityId == city.Key))
                        visits += database.POIVisits.Count(match => match.PointOfInterestId == poi.Key && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow);
                else
                    foreach (var poi in ModMileConfig.Instance.PointsOfInterest.Where(match => match.Value.CityId == city.Key))
                        visits += database.POIVisits.Count(match => match.PointOfInterestId == poi.Key);

                leaderboard.Add(new ModMileLeaderboardStat
                {
                    City = city.Value.Name,
                    Country = city.Value.Country,
                    Destination = "",
                    Player = "",
                    Rank = city.Key,
                    TravelPoints = 0,
                    Visits = visits
                });
            }

            leaderboard.Sort((curr, prev) => curr.visits.CompareTo(prev.visits));
            if (sort_order == SortOrder.desc)
                leaderboard.Reverse();

            if (sort_order == SortOrder.asc)
                leaderboard.Reverse();
            foreach (var city in leaderboard)
            {
                city.rank = leaderboard.FindIndex(match => match.city == city.city)+1;
            }
            if (sort_order == SortOrder.asc)
                leaderboard.Reverse();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, leaderboard.Count);

            if (pageEnd > leaderboard.Count)
                pageEnd = leaderboard.Count;

            var scores = new List<ModMileLeaderboardStat>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var score = leaderboard[i];
                if (score != null)
                    scores.Add(score);
            }

            var resp = new Response<CitiesLeaderboardResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new CitiesLeaderboardResponse
                {
                    player_stats = new ModMileLeaderboardStat(),
                    cities_leaderboard = new ModMileLeaderboard
                    {
                        total = leaderboard.Count,
                        page = page,
                        total_pages = totalPages,
                        Scores = scores
                    }
                }
            };
            return resp.Serialize();
        }

        public static string LeaderboardDestinations(Database database, int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order)
        {
            var leaderboard = new List<ModMileLeaderboardStat>();

            foreach (var poi in ModMileConfig.Instance.PointsOfInterest)
            {
                int visits = 0;

                if (timespan == Timespan.last_week)
                    visits = database.POIVisits.Count(match => match.PointOfInterestId == poi.Key && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7));
                else if (timespan == Timespan.this_week)
                    visits = database.POIVisits.Count(match => match.PointOfInterestId == poi.Key && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow);
                else
                    visits = database.POIVisits.Count(match => match.PointOfInterestId == poi.Key);

                leaderboard.Add(new ModMileLeaderboardStat
                {
                    city = ModMileConfig.Instance.Cities[poi.Value.CityId].Name,
                    country = ModMileConfig.Instance.Cities[poi.Value.CityId].Country,
                    destination = poi.Value.Name,
                    player = "",
                    rank = poi.Key,
                    travel_points = 0,
                    visits = visits
                });
            }

            leaderboard.Sort((curr, prev) => curr.visits.CompareTo(prev.visits));
            if (sort_order == SortOrder.desc)
                leaderboard.Reverse();

            if (sort_order == SortOrder.asc)
                leaderboard.Reverse();
            foreach (var poi in leaderboard)
            {
                poi.rank = leaderboard.FindIndex(match => match.destination == poi.destination) + 1;
            }
            if (sort_order == SortOrder.asc)
                leaderboard.Reverse();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, leaderboard.Count);

            if (pageEnd > leaderboard.Count)
                pageEnd = leaderboard.Count;

            var scores = new List<ModMileLeaderboardStat>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var score = leaderboard[i];
                if (score != null)
                    scores.Add(score);
            }

            var resp = new Response<DestinationsLeaderboardResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new DestinationsLeaderboardResponse
                {
                    player_stats = new ModMileLeaderboardStat(),
                    destinations_leaderboard = new ModMileLeaderboard
                    {
                        total = leaderboard.Count,
                        page = page,
                        total_pages = totalPages,
                        Scores = scores
                    }
                }
            };
            return resp.Serialize();
        }

        public static string LeaderboardPlayers(Database database, Guid SessionID, int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order, string username)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var leaderboard = new List<ModMileLeaderboardStat>();

            foreach (var player in database.Users.Where(match => match.HasCheckedInBefore).ToList())
            {
                int travel_points = 0;
                int visits = 0;

                if (timespan == Timespan.last_week)
                {
                    travel_points = player.TravelPointsLastWeek;
                    visits = player.VisitsLastWeek;
                }
                else if (timespan == Timespan.this_week)
                {
                    travel_points = player.TravelPointsThisWeek;
                    visits = player.VisitsThisWeek;
                }
                else
                {
                    travel_points = player.TravelPoints;
                    visits = player.Visits;
                }

                leaderboard.Add(new ModMileLeaderboardStat
                {
                    city = "",
                    country = "",
                    destination = "",
                    player = player.Username,
                    rank = player.UserId,
                    travel_points = travel_points,
                    visits = visits
                });
            }

            if (sort_column == SortColumn.visits)
                leaderboard.Sort((curr, prev) => curr.visits.CompareTo(prev.visits));

            if (sort_column == SortColumn.travel_points)
                leaderboard.Sort((curr, prev) => curr.travel_points.CompareTo(prev.travel_points));

            if (sort_order == SortOrder.desc)
                leaderboard.Reverse();

            if (sort_order == SortOrder.asc)
                leaderboard.Reverse();
            foreach (var player in leaderboard)
            {
                player.rank = leaderboard.FindIndex(match => match.player == player.player) + 1;
            }
            if (sort_order == SortOrder.asc)
                leaderboard.Reverse();

            var mystats = new ModMileLeaderboardStat();

            if (user != null)
                mystats = leaderboard.FirstOrDefault(match => match.player == user.Username);

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, leaderboard.Count);

            if (pageEnd > leaderboard.Count)
                pageEnd = leaderboard.Count;

            var scores = new List<ModMileLeaderboardStat>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var score = leaderboard[i];
                if (score != null && (username == null || username.Split(',').Contains(score.player)))
                    scores.Add(score);
            }

            var resp = new Response<PlayersLeaderboardResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new PlayersLeaderboardResponse
                {
                    player_stats = mystats,
                    players_leaderboard = new ModMileLeaderboard
                    {
                        total = leaderboard.Count,
                        page = page,
                        total_pages = totalPages,
                        Scores = scores
                    }
                }
            };
            return resp.Serialize();
        }

        private static int GetPOI(float latitude, float longitude)
        {
            foreach (var poi in ModMileConfig.Instance.PointsOfInterest)
            {
                if (latitude >= poi.Value.Latitude - (poi.Value.Radius / 100f) 
                    && latitude <= poi.Value.Latitude + (poi.Value.Radius / 100f)
                    && longitude >= poi.Value.Longitude - (poi.Value.Radius / 100f)
                    && longitude <= poi.Value.Longitude + (poi.Value.Radius / 100f)) return poi.Key;
            }
            return 0;
        }

        private static float ToRadians(float degrees)
        {
            return degrees * float.Pi / 180f;
        }

        private static float GetDistance(float prevLatitude, float prevLongitude, float latitude, float longitude)
        {
            float radius = 6378137;
            float distanceLat = ToRadians(latitude - prevLatitude);
            float distanceLong = ToRadians(longitude - prevLongitude);
            float a = float.Sin(distanceLat / 2) * float.Sin(distanceLat / 2) +
                float.Cos(ToRadians(prevLatitude)) * float.Cos(ToRadians(latitude)) *
                float.Sin(distanceLong / 2) * float.Sin(distanceLong / 2);
            float c = 2 * float.Atan2(float.Sqrt(a), float.Sqrt(1 - a));
            return radius * c;
        }

        public static float GetU(float longitude)
        {
            float radius = 512 / (2 * float.Pi);
            return ToRadians(longitude + 180) * radius;
        }

        public static float GetV(float latitude)
        {
            float radius = 512 / (2 * float.Pi);
            float offset = radius * float.Log(float.Tan(float.Pi / 4 + ToRadians(latitude) / 2));
            return 512 / 2 - offset;
        }
    }
}
