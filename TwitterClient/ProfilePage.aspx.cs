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
// Profile page has no reply feature, as I couldn't get replying work with a popup window
public partial class ProfilePage : System.Web.UI.Page
{
    // Some variables
    private static IEnumerable<ITweet> latestTweets;
    private static IEnumerable<ITweet> lastQueriedTimeline;
    private static ICredentialsRateLimits rateLimits;
    protected static long tweetIdToReplyTo = 0;
    private static bool isFavorited = false;
    private static bool isRetweeted = false;
    private static IUser user;
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
                var screenName = Request.Params.Get("user");
                if (!String.IsNullOrEmpty(screenName))
                {
                    user = Tweetinvi.User.GetUserFromScreenName(screenName);
                }
                InitUserData(user);
                
                // Gets 200 latest tweets
                latestTweets = user.GetUserTimeline(200);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
        GenerateTimeline();
    }
    // Sets user data. Profile picture, name and tweet, follower and following counts
    protected void InitUserData(IUser user)
    {
        try
        {
            imgProfilePicture.Attributes["src"] = user.ProfileImageUrl;
            lblUserName.Text = user.Name;
            lblScreenName.Text = lblScreenName.Text = "<a href=ProfilePage.aspx?user=" + user.ScreenName + " target='_blank' style='text-decoration:none;color:#828282;'>@" + user.ScreenName + "</a>";
            userDescr.InnerHtml = user.Description;
            userDescr.InnerHtml = Regex.Replace(userDescr.InnerHtml,
                                            @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                                            "<a target='_blank' href='$1'>$1</a>");
            userDescr.InnerHtml = Regex.Replace(userDescr.InnerHtml,
                                            @"@(\w+)",
                                            "<a target='_blank' href=ProfilePage.aspx?user=$1>@$1</a>");
            userDescr.InnerHtml = Regex.Replace(userDescr.InnerHtml,
                                            @"#(\w+)",
                                            "<a target='_blank' href=Search.aspx?query=%23$1>#$1</a>");
            lblTweets.Text = "Tweets: " + user.StatusesCount.ToString();
            lblFollowing.Text = "Following: " + user.FriendsCount.ToString();
            lblFollowers.Text = "Followers: " + user.FollowersCount.ToString();
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
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            if (lastQueriedTimeline != null)
            {
                // Last timeline is stored in variable and generated from this if rate limits are exceeded
                foreach (var tweetData in lastQueriedTimeline)
                {
                    GenerateTweet(tweetData, "divTimeline");
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
        tweetControlsContainer.Controls.Add(retweetButton);
        tweetControlsContainer.Controls.Add(likeButton);
        tweetContainer.Controls.Add(tweetControlsContainer);
        tweetDataContainer.Controls.Add(tweetSenderPictureContainer);
        tweetDataContainer.Controls.Add(tweetContainer);
        divToAppend.Controls.Add(tweetDataContainer);
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
            lblError.Text = ex.Message;
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
}