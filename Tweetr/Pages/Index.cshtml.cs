﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tweetr.Interfaces;
using Tweetr.Models;

namespace Tweetr.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private ITweet TweetHandler;
        
        
        public Customer customer { get; set; }
        public bool NotLoggedIn { get; set; }
        [BindProperty]
        public List<Tweet> TweetsPublic { get; set; }
        [BindProperty]
        public List<Tweet> TweetsPrivate { get; set; }
        [BindProperty]
        public Tweet Tweet { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ITweet iTweet)
        {
            _logger = logger;
            TweetHandler = iTweet;
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("user"));
                TweetsPrivate = TweetHandler.GetAllFriendsTweets(customer.Id);
            }
            else
            {
                NotLoggedIn = true;
            }

            TweetsPublic = TweetHandler.GetAllPublicTweets();
        }

        public IActionResult OnPost()
        {
            if (Tweet.Text.Length > 1)
            {
                Tweet.customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("user"));
                
                TweetHandler.Create(Tweet);
                return Page();
            }
            return Page();
        }

        public IActionResult OnPostLikes(int id)
        {
            Tweet tweet = TweetHandler.GetTweet(id);
            TweetHandler.UpdateTweet(id,tweet);
            if (TweetsPublic != null)
            {
                tweet.Likes.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("user")).Id);
            }
            if (TweetsPrivate != null)
            {
                tweet.Likes.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("user")).Id);
            }
            return Page();
        }
    }
}
