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
    private static IAuthenticatedUser user;
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
            // Enable RateLimit Tracking
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            user = (IAuthenticatedUser)Session["user"];
            InitUserData(user);
            latestTweets = user.GetHomeTimeline(200);
            userMentions = user.GetMentionsTimeline(200);
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
    protected void GenerateTimeline(IAuthenticatedUser user)
    {
        // Generate home timeline
        foreach (var tweetData in latestTweets)
        {
            GenerateTweet(tweetData, "divTimeline");
        }
        // Generate mentions timeline
        foreach (var tweetData in userMentions)
        {
            GenerateTweet(tweetData, "divMentions");
        }
    }
    protected void GenerateTweet(ITweet tweetData, string divIdentifier)
    {
        Label lblRetweetedBy = new Label();
        if (tweetData.RetweetedTweet != null)
        {
            var retweetedBy = tweetData.CreatedBy;
            tweetData = tweetData.RetweetedTweet;
            lblRetweetedBy.Text = retweetedBy.Name + " retweeted<br />";
            lblRetweetedBy.Attributes["class"] = "retweetedByUser";
        }

        // Find the div to append to. Different for mentions and normal timeline
        Control divToAppend = FindControl(divIdentifier);

        HtmlGenericControl tweetDataContainer = new HtmlGenericControl("div");
        tweetDataContainer.Attributes["class"] = "card horizontal";

        HtmlGenericControl tweetSenderPictureContainer = new HtmlGenericControl("div");
        tweetSenderPictureContainer.Attributes["class"] = "card-image";

        HtmlImage imgTweetSenderPicture = new HtmlImage();
        imgTweetSenderPicture.Attributes["src"] = tweetData.CreatedBy.ProfileImageUrl;

        tweetSenderPictureContainer.Controls.Add(imgTweetSenderPicture);

        HtmlGenericControl tweetContainer = new HtmlGenericControl("div");
        tweetContainer.Attributes["class"] = "card-stacked";

        HtmlGenericControl tweetPublicationData = new HtmlGenericControl("div");
        tweetPublicationData.Attributes["class"] = "card-content tweet-sender";

        Label lblTweetSenderUserName = new Label();
        lblTweetSenderUserName.Text = tweetData.CreatedBy.Name;
        lblTweetSenderUserName.Attributes["class"] = "tweetSenderUserName";

        Label lblTweetSenderScreenName = new Label();
        lblTweetSenderScreenName.Text = "<a href=ProfilePage.aspx?user=" + tweetData.CreatedBy.ScreenName + " target='_blank' style='text-decoration:none;color:#828282;'>@" + tweetData.CreatedBy.ScreenName + "</a>";
        lblTweetSenderScreenName.Attributes["class"] = "tweetSenderScreenName";

        Label lblTweetPublishDate = new Label();
        lblTweetPublishDate.Attributes["class"] = "tweetPublicationDate";
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        DateTime createdAt = tweetData.CreatedAt;
        DateTime now = DateTime.Now;
        TimeSpan span = now.Subtract(createdAt);
        // Time conversions, checks how long time ago the tweet was createad
        if (span.TotalMinutes < 1)
        {
            lblTweetPublishDate.Text = span.Seconds.ToString() + "s";
        }
        else if (span.TotalHours < 1)
        {
            lblTweetPublishDate.Text = span.Minutes.ToString() + "m";
        }
        else if (span.TotalDays < 1)
        {
            lblTweetPublishDate.Text = span.Hours.ToString() + "h";
        }
        else if (span.TotalDays > 365)
        {
            lblTweetPublishDate.Text = tweetData.CreatedAt.ToString("MMM d yyy");
        }
        else
        {
            lblTweetPublishDate.Text = tweetData.CreatedAt.ToString("MMM d");
        }

        tweetPublicationData.Controls.Add(lblRetweetedBy);
        tweetPublicationData.Controls.Add(lblTweetSenderUserName);
        tweetPublicationData.Controls.Add(lblTweetSenderScreenName);
        tweetPublicationData.Controls.Add(lblTweetPublishDate);

        HtmlGenericControl divTweetText = new HtmlGenericControl("div");
        divTweetText.Attributes["class"] = "card-content";
        // Replace new lines with br tag
        divTweetText.InnerHtml = tweetData.Text.Replace("\n", "<br />");
        // Find all links and replace them with hyper links
        divTweetText.InnerHtml = Regex.Replace(divTweetText.InnerHtml,
                                        @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                                        "<a target='_blank' href='$1'>$1</a>");
        divTweetText.InnerHtml = Regex.Replace(divTweetText.InnerHtml,
                                        @"@(\w+)",
                                        "<a target='_blank' href=ProfilePage.aspx?user=$1>@$1</a>");

        HtmlGenericControl tweetControlsContainer = new HtmlGenericControl("div");
        tweetControlsContainer.Attributes["class"] = "card-action";

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

        tweetControlsContainer.Controls.Add(replyButton);
        tweetControlsContainer.Controls.Add(retweetButton);
        tweetControlsContainer.Controls.Add(likeButton);

        tweetContainer.Controls.Add(tweetPublicationData);
        tweetContainer.Controls.Add(divTweetText);
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