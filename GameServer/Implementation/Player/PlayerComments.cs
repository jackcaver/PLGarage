﻿using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameServer.Implementation.Player
{
    public class PlayerComments
    {
        public static string ListComments(Database database, string username, int page, int per_page, int limit, SortColumn sort_column,
            Platform platform, string PlayerIDFilter, string AuthorIDFilter)
        {
            var Comments = new List<PlayerCommentData> { };
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == username);

            foreach (string id in PlayerIDFilter.Split(','))
            {
                Comments.AddRange(database.PlayerComments.Where(match => match.PlayerId.ToString() == id).ToList());
            }

            //sorting
            if (sort_column == SortColumn.created_at)
                Comments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            var commentsList = new List<player_comment> { };

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Comments.Count);

            if (pageEnd > Comments.Count)
                pageEnd = Comments.Count;

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Comment = Comments[i];
                if (Comment != null)
                {
                    commentsList.Add(new player_comment
                    {
                        author_id = Comment.AuthorId,
                        author_username = Comment.AuthorUsername,
                        body = Comment.Body,
                        created_at = Comment.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        updated_at = Comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        id = Comment.Id,
                        platform = Comment.Platform.ToString(),
                        player_id = Comment.PlayerId,
                        username = Comment.Username,
                        rating_down = Comment.RatingDown,
                        rating_up = Comment.RatingUp,
                        rated_by_me = Comment.IsRatedByMe(requestedBy.UserId)
                    });
                }
            }

            var resp = new Response<List<player_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_comments> { new player_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = Comments.Count,
                    total_pages = totalPages,
                    PlayerCommentList = commentsList
                } }
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, string username, PlayerComment player_comment)
        {
            var author = database.Users.FirstOrDefault(match => match.Username == username);
            var user = database.Users.FirstOrDefault(match => match.UserId == player_comment.player_id);

            if (user == null || author == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerComments.Add(new PlayerCommentData
            {
                AuthorId = author.UserId,
                Body = player_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                PlayerId = player_comment.player_id
            });
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RemoveComment(Database database, string username, int id)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            var comment = database.PlayerComments.FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (comment.AuthorId != user.UserId && comment.PlayerId != user.UserId)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerComments.Remove(comment);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RateComment(Database database, string username, PlayerCommentRating player_comment_rating)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            var comment = database.PlayerComments.FirstOrDefault(match => match.Id == player_comment_rating.player_comment_id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCommentRatings.FirstOrDefault(match => match.PlayerCommentId == player_comment_rating.player_comment_id
                && match.PlayerId == user.UserId);

            if (rating == null)
            {
                database.PlayerCommentRatings.Add(new PlayerCommentRatingData
                {
                    PlayerCommentId = player_comment_rating.player_comment_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow
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
    }
}