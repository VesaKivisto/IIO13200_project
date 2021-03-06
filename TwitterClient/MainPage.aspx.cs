﻿using System;
using Tweetinvi;
using Tweetinvi.Models;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public partial class MainPage : System.Web.UI.Page
{
    // Some variables we need
    private static IEnumerable<ITweet> latestTweets;
    private static IEnumerable<IMention> userMentions;
    private static IEnumerable<ITweet> lastQueriedTimeline;
    private static IEnumerable<ITweet> lastQueriedMentions;
    private static IAuthenticatedUser user;
    private static ICredentialsRateLimits rateLimits;
    private static long tweetIdToReplyTo = 0;
    private static bool isFavorited = false;
    private static bool isRetweeted = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        // For some reason this app throws random error when loading the page first time. I made this dirty fix to wait a little before actually loading the page.
        // It seems to help
        Thread.Sleep(500);
        // Enable RateLimit Tracking. Tracks and waits, so in worst case the app stops working for 15 minutes as tokens are gained in 15 minute intervals
        // Couldn't get TrackOnly method to work
        RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
        if (!IsPostBack)
        {
            try
            {
                // Redirect to log in page if session user is null
                if (Session["user"] == null)
                {
                    Response.Redirect("LogIn.aspx");
                }
                user = (IAuthenticatedUser)Session["user"];
                InitUserData(user);
                // Gets 200 latest tweets and mentions
                latestTweets = user.GetHomeTimeline(200);
                userMentions = user.GetMentionsTimeline(200);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
        GenerateTimeline();
    }
    // Set user data
    protected void InitUserData(IAuthenticatedUser user)
    {
        try
        {
            lblUserName.Text = user.Name;
            lblScreenName.Text = "<a href=ProfilePage.aspx?user=" + user.ScreenName + " target='_blank' style='text-decoration:none;color:#828282;'>@" + user.ScreenName + "</a>";
            imgProfilePicture.Attributes["src"] = user.ProfileImageUrl;
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    // This sends the new tweet.
    protected void btnSendTweet_Click(object sender, EventArgs e)
    {
        try
        {
            // Send tweet
            if (tweetIdToReplyTo != 0)
            {
                Tweet.PublishTweetInReplyTo(txtTweet.Text, tweetIdToReplyTo);
            }
            else
            {
                Tweet.PublishTweet(txtTweet.Text);
            }
            txtTweet.Text = "";
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    // Timeline generation. Used with normal and mentions timeline. Has ratelimit checks, which is not exactly how Tweetinvi suggests to do it
    // but I couldn't get the actual way to implement this to work so we have this
    protected void GenerateTimeline()
    {
        try
        {
            // Generate home timeline
            foreach (var tweetData in latestTweets)
            {
                GenerateTweet(tweetData, "divTimeline");
                lastQueriedTimeline = latestTweets;
            }
            // Generate mentions timeline
            foreach (var tweetData in userMentions)
            {
                GenerateTweet(tweetData, "divMentions");
                lastQueriedMentions = userMentions;
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            // Last timeline is stored in variable and generated from this if rate limits are exceeded
            if (lastQueriedMentions != null)
            {
                foreach (var tweetData in lastQueriedTimeline)
                {
                    GenerateTweet(tweetData, "divTimeline");
                }
            }
            if (lastQueriedTimeline != null)
            {
                foreach (var tweetData in lastQueriedMentions)
                {
                    GenerateTweet(tweetData, "divMentions");
                }
            }
        }
    }
    // Tweet generating. The code here could be a bit cleaner and nicer
    protected void GenerateTweet(ITweet tweetData, string divIdentifier)
    {
        BLTwitterClient twitterClient = new BLTwitterClient();
        // Find the div to append to. Different for mentions and normal timeline
        Control divToAppend = Master.FindControl("ContentPlaceHolder1").FindControl(divIdentifier);

        HtmlGenericControl tweetSenderPictureContainer = twitterClient.GenerateTweetSenderPictureContainer(tweetData);
        HtmlGenericControl tweetControlsContainer = twitterClient.GenerateTweetControlsContainer();
        HtmlGenericControl tweetContainer = twitterClient.GenerateTweetContainer(tweetData);
        HtmlGenericControl tweetDataContainer = twitterClient.GenerateTweetDataContainer();
        #region Rest of the tweetControlsContainer
        // Buttons are generated here. I ran into some logic problems when trying to generate buttons in BL, so they're here
        // Reply button
        Button replyButton = new Button();
        replyButton.Text = " ";
        replyButton.Attributes["class"] = "reply";
        replyButton.Attributes["onClick"] = "replyButton_Click";
        replyButton.Command += replyButton_Click;
        replyButton.CommandArgument = tweetData.ToJson();
        // Retweet button
        Button retweetButton = new Button();
        retweetButton.Text = tweetData.RetweetCount.ToString();
        retweetButton.Attributes["class"] = "retweet";
        retweetButton.Command += retweetButton_Click;
        retweetButton.CommandArgument = tweetData.Id.ToString();
        if (tweetData.Retweeted)
        {
            isRetweeted = true;
            retweetButton.Style.Add("background-image", "url('../images/retweetGreen.png')");
            retweetButton.Style.Add("color", "#19CF8D");
        }
        else
        {
            retweetButton.Style.Add("background-image", "url('../images/retweet.png')");
        }
        // Like button
        Button likeButton = new Button();
        // Using tweetData.Retweeted didn't work here as it did above, so I had to check if RetweetedTweet is empty or not
        if (tweetData.RetweetedTweet != null)
        {
            // This throws an exception all the time. FavoriteCount is in every retweet, and you can get it without try but it still throws an exception
            // I have no idea why it does this, but this fixes it in a way
            try
            {
                likeButton.Text = tweetData.RetweetedTweet.FavoriteCount.ToString();
            }
            catch (Exception)
            {

            }
        }
        else
        {
            likeButton.Text = tweetData.FavoriteCount.ToString();
        }
        likeButton.Attributes["class"] = "like";
        likeButton.Command += likeButton_Click;
        likeButton.CommandArgument = tweetData.Id.ToString();
        if (tweetData.Favorited)
        {
            isFavorited = true;
            likeButton.Style.Add("background-image", "url('../images/likeRed.png')");
            likeButton.Style.Add("color", "#E2264D");
        }
        else
        {
            likeButton.Style.Add("background-image", "url('../images/like.png')");
        }
        // Delete button
        if (tweetData.CreatedBy.ScreenName == ((IAuthenticatedUser)Session["user"]).ScreenName)
        {
            Button deleteButton = new Button();
            deleteButton.Text = " ";
            deleteButton.Attributes["class"] = "delete";
            deleteButton.Command += deleteButton_Click;
            deleteButton.CommandArgument = tweetData.Id.ToString();
            deleteButton.OnClientClick = "return confirm('Are you sure you want to delete this tweet?');";
            deleteButton.Style.Add("background-image", "url('../images/delete.png')");
            tweetControlsContainer.Controls.Add(deleteButton);
        }
        #endregion
        // Add everything to controls
        tweetControlsContainer.Controls.Add(replyButton);
        tweetControlsContainer.Controls.Add(retweetButton);
        tweetControlsContainer.Controls.Add(likeButton);
        tweetContainer.Controls.Add(tweetControlsContainer);
        tweetDataContainer.Controls.Add(tweetSenderPictureContainer);
        tweetDataContainer.Controls.Add(tweetContainer);
        divToAppend.Controls.Add(tweetDataContainer);
    }
    // Function for reply button. Converts the tweet to JSON and parses everything we need from it
    protected void replyButton_Click(object sender, CommandEventArgs e)
    {
        try
        {
            var argument = ((Button)sender).CommandArgument;
            JObject json = JObject.Parse(argument);
            tweetIdToReplyTo = long.Parse(json["id"].ToString());
            txtTweet.Text = "@" + json["user"]["screen_name"].ToString() + " ";
            JArray userMentions = JArray.Parse(json["entities"]["user_mentions"].ToString());
            foreach (var mention in json["entities"]["user_mentions"])
            {
                if (mention["screen_name"].ToString() != json["user"]["screen_name"].ToString())
                {
                    txtTweet.Text += "@" + mention["screen_name"] + " ";
                }
            }
            lblTweetLength.Text = (140 - txtTweet.Text.Length).ToString();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    // Function for retweet button. Gets tweet ID as variable via CommandArgument
    protected void retweetButton_Click(object sender, CommandEventArgs e)
    {
        try
        {
            var argument = ((Button)sender).CommandArgument;
            if (isRetweeted)
            {
                Tweet.UnRetweet(long.Parse(argument));
            }
            else
            {
                Tweet.PublishRetweet(long.Parse(argument));
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    // Function for like button. Gets tweet ID as variable via CommandArgument
    protected void likeButton_Click(object sender, CommandEventArgs e)
    {
        try
        {
            var argument = ((Button)sender).CommandArgument;
            if (isFavorited)
            {
                Tweet.UnFavoriteTweet(long.Parse(argument));
            }
            else
            {
                Tweet.FavoriteTweet(long.Parse(argument));
            }
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('" + ex.Message + "')</script>");
        }
    }
    // Function for delete button. Gets tweet ID as variable via CommandArgument
    protected void deleteButton_Click(object sender, CommandEventArgs e)
    {
        try
        {
            var argument = ((Button)sender).CommandArgument;
            Tweet.DestroyTweet(long.Parse(argument));
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void btnLogOut_Click(object sender, EventArgs e)
    {
        Response.Redirect("LogIn.aspx");
    }
}