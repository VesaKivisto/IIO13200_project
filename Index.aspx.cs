using System;
using Tweetinvi;
using Tweetinvi.Models;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class Index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Testing user authentication and stuff
        Auth.SetUserCredentials("La23vsEKuSQGbK5XQEngmpxsT", "PYvX3TrKz701FmjdZSBoutTj4bNTDhpOriBPgyd2R87MST5wFl", "411057460-wclDRe8viNRDzw4xVvdSyrATkFIMovKY8dFNTf5C", "mD9my4ztmQCthQNtoR1D5DwS0FGwy2S9eeXKeJTW0dyX0");
        var user = Tweetinvi.User.GetAuthenticatedUser();
        lblScreenName.Text = user.Name;
        lblUserName.Text = "@" + user.ScreenName;
        imgProfilePicture.Attributes["src"] = user.ProfileImageUrl;
        lblTweets.Text = "Tweets: " + user.StatusesCount.ToString();
        lblFollowing.Text = "Following: " + user.FriendsCount.ToString();
        lblFollowers.Text = "Followers: " + user.FollowersCount.ToString();
        GenerateTimeline(user);
    }

    protected void btnSendTweet_Click(object sender, EventArgs e)
    {
        // Send tweet
        Tweet.PublishTweet(txtTweet.Text);
    }

    protected void GenerateTimeline(IAuthenticatedUser user)
    {
        var latestTweets = user.GetHomeTimeline();
        var userMentions = user.GetMentionsTimeline();
        // Generate home timeline
        foreach (var tweetData in latestTweets)
        {
            if (tweetData.RetweetedTweet != null)
            {
                GenerateTweet(tweetData.RetweetedTweet, "divTimeline");
            }
            else
            {
                GenerateTweet(tweetData, "divTimeline");
            }
        }
        // Generate mentions timeline
        foreach (var tweetData in userMentions)
        {
            GenerateTweet(tweetData, "divMentions");
        }
    }

    protected void GenerateTweet(ITweet tweetData, string divIdentifier)
    {
        string tweet = tweetData.Text;
        tweet = tweet.Replace("\n", "<br />");

        // Find all links and replace them with hyperlinks. For some reason the API doesn't give all images as media entities, and they don't have any media urls as non-media entities.
        tweet = Regex.Replace(tweet,
                @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                "<a target='_blank' href='$1'>$1</a>");

        // Find the div to append to. Different for mentions and normal timeline
        Control divToAppend = FindControl(divIdentifier);

        HtmlGenericControl tweetDataContainer = new HtmlGenericControl("div");
        tweetDataContainer.Attributes["class"] = "tweetDataContainer";

        HtmlGenericControl tweetSenderPictureContainer = new HtmlGenericControl("div");
        tweetSenderPictureContainer.Attributes["class"] = "tweetSenderPictureContainer";

        HtmlImage imgTweetSenderPicture = new HtmlImage();
        imgTweetSenderPicture.Attributes["src"] = tweetData.CreatedBy.ProfileImageUrl;

        tweetSenderPictureContainer.Controls.Add(imgTweetSenderPicture);

        HtmlGenericControl tweetContainer = new HtmlGenericControl("div");
        tweetContainer.Attributes["class"] = "tweetContainer";

        HtmlGenericControl tweetPublicationData = new HtmlGenericControl("div");
        tweetPublicationData.Attributes["class"] = "tweetPublicationData";

        Label lblTweetSenderUserName = new Label();
        lblTweetSenderUserName.Text = tweetData.CreatedBy.Name;
        lblTweetSenderUserName.Font.Bold = true;

        Label lblTweetSenderScreenName = new Label();
        lblTweetSenderScreenName.Text = "@" + tweetData.CreatedBy.ScreenName;
        lblTweetSenderScreenName.ForeColor = Color.LightGray;

        Label lblTweetPublishDate = new Label();
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        DateTime createdAt = tweetData.CreatedAt;
        DateTime now = DateTime.Now;
        TimeSpan span = now.Subtract(createdAt);

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
        lblTweetPublishDate.Attributes["class"] = "tweetPublicationDate";

        tweetPublicationData.Controls.Add(lblTweetSenderUserName);
        tweetPublicationData.Controls.Add(lblTweetSenderScreenName);
        tweetPublicationData.Controls.Add(lblTweetPublishDate);

        Label lblTweetText = new Label();
        lblTweetText.Text = tweet;

        HtmlGenericControl tweetControlsContainer = new HtmlGenericControl("div");
        tweetControlsContainer.Attributes["class"] = "tweetControlsContainer";

        Label lblReply = new Label();
        lblReply.Text = "Reply";

        Label lblRetweet = new Label();
        lblRetweet.Text = "Retweet " + tweetData.RetweetCount.ToString();

        Label lblLike = new Label();
        lblLike.Text = "Like " + tweetData.FavoriteCount.ToString();

        tweetControlsContainer.Controls.Add(lblReply);
        tweetControlsContainer.Controls.Add(lblRetweet);
        tweetControlsContainer.Controls.Add(lblLike);

        tweetContainer.Controls.Add(tweetPublicationData);
        tweetContainer.Controls.Add(lblTweetText);
        tweetContainer.Controls.Add(tweetControlsContainer);

        tweetDataContainer.Controls.Add(tweetSenderPictureContainer);
        tweetDataContainer.Controls.Add(tweetContainer);

        divToAppend.Controls.Add(tweetDataContainer);
    }
}