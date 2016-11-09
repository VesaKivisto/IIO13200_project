using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Web.UI.WebControls;

public partial class Index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Testing user authentication and stuff
        Auth.SetUserCredentials("bZ2HRNQNOoHn8bFBnsUIXjogl", "MOLEBx5NZGnl8eOYcWnl9xxychGacU0Oh8LGsqCUFiPfxmYPGl", "411057460-2L7OPNPrLj7x54glPuf177wFDVULHMTzZXkFJljW", "HU3CxjJGhTl3TlnnEIT3OsFA7ZafMK0BvUdB4dmALVjUx");
        var user = Tweetinvi.User.GetAuthenticatedUser();
        lblScreenName.Text = user.Name;
        lblUserName.Text = "@" + user.ScreenName;
        imgProfilePicture.Attributes["src"] = user.ProfileImageUrl;
        lblTweets.Text = "Tweets: " + user.StatusesCount.ToString();
        lblFollowing.Text = "Following: " + user.FriendsCount.ToString();
        lblFollowers.Text = "Followers: " + user.FollowersCount.ToString();
        GenerateTweet(user);
    }

    protected void btnSendTweet_Click(object sender, EventArgs e)
    {
        // Send tweet
        Tweet.PublishTweet(txtTweet.Text);
    }

    protected void GenerateTweet(IAuthenticatedUser user)
    {
        // Testing tweet layout
        var latestTweets = user.GetUserTimeline(4);
        var tweetData = latestTweets.Last();
        // Generate tweets. Layout breaks with multiple tweets currently.
        //foreach (var tweetData in latestTweets)
        //{
            string tweet = tweetData.Text;
            tweet = tweet.Replace("\n", "<br />");

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
            lblTweetPublishDate.Text = tweetData.CreatedAt.ToString();
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

            divTimeline.Controls.Add(tweetDataContainer);
        //}
    }
}