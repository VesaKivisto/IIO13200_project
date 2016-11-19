using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Tweetinvi;
using Tweetinvi.Models;

public partial class ProfilePage : System.Web.UI.Page
{
    private static IEnumerable<ITweet> latestTweets;
    private static IEnumerable<ITweet> lastQueriedTimeline;
    private static ICredentialsRateLimits rateLimits;
    protected static long tweetIdToReplyTo = 0;
    private static bool isFavorited = false;
    private static bool isRetweeted = false;
    private static IUser user;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var screenName = Request.Params.Get("user");
            if (!String.IsNullOrEmpty(screenName))
            {
                user = Tweetinvi.User.GetUserFromScreenName(screenName);
            }
            InitUserData(user);
            // Enable RateLimit Tracking
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;
            rateLimits = RateLimit.GetCurrentCredentialsRateLimits();
            try
            {
                latestTweets = user.GetUserTimeline(200);
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "')</script>");
            }
        }
        GenerateTimeline(user);
    }
    protected void InitUserData(IUser user)
    {
        imgProfilePicture.Attributes["src"] = user.ProfileImageUrl;
        lblUserName.Text = user.Name;
        lblScreenName.Text = "@" + user.ScreenName;
        lblTweets.Text = "Tweets: " + user.StatusesCount.ToString();
        lblFollowing.Text = "Following: " + user.FriendsCount.ToString();
        lblFollowers.Text = "Followers: " + user.FollowersCount.ToString();
    }
    /*
    protected void btnSendTweet_Click(object sender, EventArgs e)
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
    */
    protected void GenerateTimeline(IUser user)
    {
        try
        {
            // Generate home timeline
            foreach (var tweetData in latestTweets)
            {
                GenerateTweet(tweetData, "divTimeline");
                lastQueriedTimeline = latestTweets;
            }
        }
        catch (Exception)
        {
            TimeSpan time = TimeSpan.FromSeconds(Math.Round(rateLimits.StatusesHomeTimelineLimit.ResetDateTimeInSeconds));
            Response.Write("<script>alert('Rate limits exceeded! Please wait " + time.Minutes + " minutes and " + time.Seconds + " seconds.')</script>");
            if (lastQueriedTimeline != null)
            {
                foreach (var tweetData in lastQueriedTimeline)
                {
                    GenerateTweet(tweetData, "divTimeline");
                }
            }
        }
    }
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
        /*
        Button replyButton = new Button();
        replyButton.Text = " ";
        replyButton.Attributes["class"] = "reply";
        replyButton.Attributes["onClick"] = "replyButton_Click";
        replyButton.Command += replyButton_Click;
        replyButton.CommandArgument = tweetData.ToJson();
        */
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

        Button likeButton = new Button();
        likeButton.Text = tweetData.FavoriteCount.ToString();
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
        #endregion
        //tweetControlsContainer.Controls.Add(replyButton);
        tweetControlsContainer.Controls.Add(retweetButton);
        tweetControlsContainer.Controls.Add(likeButton);
        tweetContainer.Controls.Add(tweetControlsContainer);
        tweetDataContainer.Controls.Add(tweetSenderPictureContainer);
        tweetDataContainer.Controls.Add(tweetContainer);
        divToAppend.Controls.Add(tweetDataContainer);
    }
    /*
    protected void replyButton_Click(object sender, CommandEventArgs e)
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
    */
    protected void retweetButton_Click(object sender, CommandEventArgs e)
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
    protected void likeButton_Click(object sender, CommandEventArgs e)
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
}