using System;
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
        if (!IsPostBack)
        {
            if (Session["user"] == null)
            {
                Response.Redirect("LogIn.aspx");
            }
            user = (IAuthenticatedUser)Session["user"];
            InitUserData(user);
            // Enable RateLimit Tracking
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;
            rateLimits = RateLimit.GetCurrentCredentialsRateLimits();
            try
            {
                latestTweets = user.GetHomeTimeline(5);
                userMentions = user.GetMentionsTimeline(5);
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "')</script>");
            }
        }
        GenerateTimeline(user);
    }

    protected void InitUserData(IAuthenticatedUser user)
    {
        lblScreenName.Text = user.Name;
        lblUserName.Text = "@" + user.ScreenName;
        imgProfilePicture.Attributes["src"] = user.ProfileImageUrl;
    }
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
            Response.Write("<script>alert('" + ex.Message + "')</script>");
        }
    }
    protected void GenerateTimeline(IAuthenticatedUser user)
    {
        bool exception = false;
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
        catch (Exception)
        {
            exception = true;
            TimeSpan time = TimeSpan.FromSeconds(Math.Round(rateLimits.StatusesHomeTimelineLimit.ResetDateTimeInSeconds));
            Response.Write("<script>alert('RateLimits exceeded! Please wait " + time.Minutes + " minutes and " + time.Seconds + " seconds.')</script>");
        }
        finally
        {
            if (exception && lastQueriedMentions != null && lastQueriedTimeline != null)
            {
                foreach (var tweetData in lastQueriedTimeline)
                {
                    GenerateTweet(tweetData, "divTimeline");
                }
                foreach (var tweetData in lastQueriedMentions)
                {
                    GenerateTweet(tweetData, "divMentions");
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
        Button replyButton = new Button();
        replyButton.Text = " ";
        replyButton.Attributes["class"] = "reply";
        replyButton.Attributes["onClick"] = "replyButton_Click";
        replyButton.Command += replyButton_Click;
        replyButton.CommandArgument = tweetData.ToJson();

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
        tweetControlsContainer.Controls.Add(replyButton);
        tweetControlsContainer.Controls.Add(retweetButton);
        tweetControlsContainer.Controls.Add(likeButton);
        tweetContainer.Controls.Add(tweetControlsContainer);
        tweetDataContainer.Controls.Add(tweetSenderPictureContainer);
        tweetDataContainer.Controls.Add(tweetContainer);
        divToAppend.Controls.Add(tweetDataContainer);
    }
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