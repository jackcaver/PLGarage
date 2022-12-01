using System;
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class NewsFeedController : Controller
    {
        [Route("news_feed.xml")]
        public IActionResult NewsFeed(int page, int per_page) 
        {
            Log.Information($"somebody is trying to get news at page: {page}, items per page {per_page}");
            var resp = new Response<List<activities>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<activities> { new activities { page = page, row_start = 1, row_end = per_page, total = 1, total_pages = 1, 
                    ActivityList = new List<activity> { 
                        new activity { type = "system_activity", 
                            events = new List<Event> {
                                    new Event { 
                                        topic = "system_event", 
                                        type = "announcement", 
                                        details = "Congratulations on getting the keys to your very own Kart!  We certainly hope you'll enjoy your time cruising the streets of the Imagisphere - perhaps even leaning to the side like you're really from the Sack 'hood. Before you get too caught up in being cool, remember to check out the fruits of your fellow Creators' labor in Community, where you can take the wheel in some truly wild and imaginative events.", 
                                        creator_username = "", 
                                        creator_id = 3390271, 
                                        timestamp = "2012-11-04T17:49:20+00:00", 
                                        seconds_ago = 173328284, 
                                        tags = "",
                                        subject = "Welcome!",
                                        image_url = "", 
                                        image_md5 = "" 
                                    }
                                } 
                            } 
                        } 
                    } 
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("news_feed/tally.xml")]
        public IActionResult NewsFeedTally() 
        {
            var resp = new Response<List<NewsFeedTally>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<NewsFeedTally> { new NewsFeedTally { total = 1} }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("activity_log.xml")]
        public IActionResult ActivityLog(int page, int per_page, int player_id) 
        {
            Log.Information($"{Request.Cookies["username"]} is trying to get activity of player id {player_id} at page: {page}, items per page {per_page}");
            var resp = new Response<List<activities>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<activities> { new activities { page = page, row_start = 1, row_end = per_page, total = 1, total_pages = 1, 
                    ActivityList = new List<activity> {} 
                    } 
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}