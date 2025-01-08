using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using AutoMapper;
using GameServer.Models.Common;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Response;

namespace GameServer.Models.Profiles
{
    public class PlayerCreationProfile : Profile
    {
        public PlayerCreationProfile()
        {
            #region Variables

            User requestedBy = null;

            #endregion

            #region PlayerCreations

            CreateMap<PlayerCreationData, PlayerCreation>() // TODO: Can be projection?
                .ForMember(dto => dto.Coolness, cfg => cfg.MapFrom(db => (db.Ratings.Count(match => match.Type == RatingType.YAY) - db.Ratings.Count(match => match.Type == RatingType.BOO)) +
                            ((db.RacesStarted.Count() + db.RacesFinished) / 2) + db.Hearts.Count()))
                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                // TODO: DLCKeys? (Creation.DLCKeys != null ? Creation.DLCKeys : "")

                .ForMember(dto => dto.Downloads, cfg => cfg.MapFrom(db => db.Downloads.Count()))
                .ForMember(dto => dto.DownloadsLastWeek, cfg => cfg.MapFrom(db => db.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.DownloadsThisWeek, cfg => cfg.MapFrom(db => db.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.FirstPublished, cfg => cfg.MapFrom(db => db.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.LastPublished, cfg => cfg.MapFrom(db => db.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz")))

                .ForMember(dto => dto.Hearts, cfg => cfg.MapFrom(db => db.Hearts.Count()))

                // TODO: Is below correct? its just tostring on get?
                .ForMember(dto => dto.PlayerCreationType, cfg => cfg.MapFrom(db => db.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK.ToString() : db.Type.ToString()))

                .ForMember(dto => dto.RacesStarted, cfg => cfg.MapFrom(db => db.RacesStarted.Count()))
                .ForMember(dto => dto.RacesStartedThisMonth, cfg => cfg.MapFrom(db => db.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow)))
                .ForMember(dto => dto.RacesStartedThisWeek, cfg => cfg.MapFrom(db => db.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow)))
                // TODO: rank

                .ForMember(dto => dto.RatingDown, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.BOO)))
                .ForMember(dto => dto.RatingUp, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.YAY)))

                .ForMember(dto => dto.UniqueRacerCount, cfg => cfg.MapFrom(db => db.UniqueRacers.Count()))
                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.Author.Username))

                .ForMember(dto => dto.Views, cfg => cfg.MapFrom(db => db.Views.Count()))
                .ForMember(dto => dto.ViewsLastWeek, cfg => cfg.MapFrom(db => db.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.ViewsThisWeek, cfg => cfg.MapFrom(db => db.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.Votes, cfg => cfg.MapFrom(db => db.Ratings.Count(match => !true || match.Rating != 0)))  // IsMNR?

                .ForMember(dto => dto.Points, cfg => cfg.MapFrom(db => db.Points.Count()))
                .ForMember(dto => dto.PointsLastWeek, cfg => cfg.MapFrom(db => db.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount)))
                .ForMember(dto => dto.PointsThisWeek, cfg => cfg.MapFrom(db => db.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount)))
                .ForMember(dto => dto.PointsToday, cfg => cfg.MapFrom(db => db.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount)))
                .ForMember(dto => dto.PointsYesterday, cfg => cfg.MapFrom(db => db.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount)))

                .ForMember(dto => dto.Rating, cfg => cfg.MapFrom(db => (db.Ratings.Count() != 0 ? (float)db.Ratings.Average(r => r.Rating) : 0).ToString("0.0", CultureInfo.InvariantCulture)))
                .ForMember(dto => dto.StarRating, cfg => cfg.MapFrom(db => (db.Ratings.Count() != 0 ? (float)db.Ratings.Average(r => r.Rating) : 0).ToString("0.0", CultureInfo.InvariantCulture)))

                // TODO: Below can be null!
                .ForMember(dto => dto.OriginalPlayerUsername, cfg => cfg.MapFrom(db => db.ParentPlayer.Username))
                .ForMember(dto => dto.ParentCreationName, cfg => cfg.MapFrom(db => db.ParentCreation.Name))
                .ForMember(dto => dto.ParentPlayerUsername, cfg => cfg.MapFrom(db => db.ParentPlayer.Username));


            CreateMap<PlayerCreationData, Track>() // TODO: Can be projection? ALSO !!! IMPORTANT !!! NOTICED SCHEMA NAMING DIFFERENCES, INVESTIGATE
                .ForMember(dto => dto.Coolness, cfg => cfg.MapFrom(db => (db.Ratings.Count(match => match.Type == RatingType.YAY) - db.Ratings.Count(match => match.Type == RatingType.BOO)) +
                            ((db.RacesStarted.Count() + db.RacesFinished) / 2) + db.Hearts.Count()))
                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                // TODO: DLCKeys? (Creation.DLCKeys != null ? Creation.DLCKeys : "")

                .ForMember(dto => dto.Downloads, cfg => cfg.MapFrom(db => db.Downloads.Count()))
                .ForMember(dto => dto.DownloadsLastWeek, cfg => cfg.MapFrom(db => db.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.DownloadsThisWeek, cfg => cfg.MapFrom(db => db.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.FirstPublished, cfg => cfg.MapFrom(db => db.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.LastPublished, cfg => cfg.MapFrom(db => db.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz")))

                .ForMember(dto => dto.Hearts, cfg => cfg.MapFrom(db => db.Hearts.Count()))

                // TODO: Is below correct? its just tostring on get? Can we also consolidate with above?
                .ForMember(dto => dto.PlayerCreationType, cfg => cfg.MapFrom(db => db.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK.ToString() : db.Type.ToString()))

                .ForMember(dto => dto.RacesStarted, cfg => cfg.MapFrom(db => db.RacesStarted.Count()))
                .ForMember(dto => dto.RacesStartedThisMonth, cfg => cfg.MapFrom(db => db.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow)))
                .ForMember(dto => dto.RacesStartedThisWeek, cfg => cfg.MapFrom(db => db.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow)))
                // TODO: rank

                .ForMember(dto => dto.RatingDown, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.BOO)))
                .ForMember(dto => dto.RatingUp, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.YAY)))

                .ForMember(dto => dto.UniqueRacerCount, cfg => cfg.MapFrom(db => db.UniqueRacers.Count()))
                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.Author.Username))

                .ForMember(dto => dto.Views, cfg => cfg.MapFrom(db => db.Views.Count()))
                .ForMember(dto => dto.ViewsLastWeek, cfg => cfg.MapFrom(db => db.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.ViewsThisWeek, cfg => cfg.MapFrom(db => db.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.Votes, cfg => cfg.MapFrom(db => db.Ratings.Count(match => !true || match.Rating != 0)))  // IsMNR?

                .ForMember(dto => dto.HeartedByMe, cfg => cfg.MapFrom(db => db.Hearts.Any(match => match.User.UserId == 0).ToString().ToLower()))   // TODO: Can be null, requestedBy???
                .ForMember(dto => dto.QueuedByMe, cfg => cfg.MapFrom(db => db.Bookmarks.Any(match => match.User.UserId == 0).ToString().ToLower()))   // TODO: Can be null, requestedBy???
                .ForMember(dto => dto.ReviewedByMe, cfg => cfg.MapFrom(db => db.Reviews.Any(match => match.User.UserId == 0).ToString().ToLower()));   // TODO: Can be null, requestedBy???

            // TODO: DTO mappings for activites, comments, leaderboard, photos and reviews

            CreateProjection<PlayerCreationData, Photo>()
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.Author.Username));

            CreateProjection<PlayerCreationData, PlayerCreationToVerify>()
                .ForMember(dto => dto.SuggestedAction, cfg => cfg.MapFrom(db => db.ModerationStatus == ModerationStatus.BANNED || db.ModerationStatus == ModerationStatus.ILLEGAL ? "allow" : "ban"));

            #endregion

            #region PlayerCreationReviews

            CreateProjection<PlayerCreationReview, Review>()
                .ForMember(dto => dto.Mine, cfg => cfg.MapFrom(db => requestedBy != null ? (db.User.UserId == requestedBy.UserId).ToString().ToLower() : "false"))

                .ForMember(dto => dto.PlayerCreationId, cfg => cfg.MapFrom(db => db.Creation.Id))
                .ForMember(dto => dto.PlayerCreationName, cfg => cfg.MapFrom(db => db.Creation.Name))
                .ForMember(dto => dto.PlayerCreationUsername, cfg => cfg.MapFrom(db => db.Creation.Author.Username))

                .ForMember(dto => dto.PlayerId, cfg => cfg.MapFrom(db => db.User.UserId))

                .ForMember(dto => dto.RatedByMe, cfg => cfg.MapFrom(db => requestedBy != null ? db.ReviewRatings.Any(match => match.Player.UserId == requestedBy.UserId).ToString().ToLower() : "false"))
                .ForMember(dto => dto.RatingDown, cfg => cfg.MapFrom(db => db.ReviewRatings.Count(match => match.Type == RatingType.BOO)))
                .ForMember(dto => dto.RatingUp, cfg => cfg.MapFrom(db => db.ReviewRatings.Count(match => match.Type == RatingType.YAY)))

                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.User.Username))
                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")));

            #endregion

            #region PlayerCreationRatings

            CreateProjection<PlayerCreationRatingData, PlayerCreationRating>()  // TODO: !!! IMPORTANT !!! DTO naming scheme mismatches
                .ForMember(dto => dto.PlayerId, cfg => cfg.MapFrom(db => db.Player.UserId))
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.Player.Username));

            #endregion

            #region PlayerCreationComments

            CreateProjection<PlayerCreationCommentData, PlayerCreationComment>()
                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))

                .ForMember(dto => dto.PlayerCreationId, cfg => cfg.MapFrom(db => db.Creation.Id))

                .ForMember(dto => dto.PlayerId, cfg => cfg.MapFrom(db => db.Player.UserId))
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.Player.Username))

                .ForMember(dto => dto.RatingDown, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.BOO)))
                .ForMember(dto => dto.RatingUp, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.YAY)))
                .ForMember(dto => dto.RatedByMe, cfg => cfg.MapFrom(db => requestedBy != null ? db.Ratings.Any(match => match.Player.UserId == requestedBy.UserId) : false));   // TODO: does bool serialise correctly? Why is everything not bool?

            #endregion

            #region FavoritePlayerCreations

            CreateProjection<HeartedPlayerCreation, FavoritePlayerCreation>()
                .ForMember(dto => dto.PlayerCreationId, cfg => cfg.MapFrom(db => db.HeartedCreation.Id))
                .ForMember(dto => dto.PlayerCreationName, cfg => cfg.MapFrom(db => db.HeartedCreation.Name));

            #endregion
        }
    }
}
