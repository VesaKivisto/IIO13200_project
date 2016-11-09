using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using System.Linq;

public partial class Index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Testing user authentication and stuff
        Auth.SetUserCredentials("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");
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
        var latestTweet = user.GetUserTimeline(3);
        var tweetData = latestTweet.Last();
        string tweet = tweetData.Text;
        tweet = tweet.Replace("\n", "<br />");
        divTweet.InnerHtml = tweet;
        imgTweetSenderPicture.Attributes["src"] = tweetData.CreatedBy.ProfileImageUrl;
        lblTweetSenderScreenName.Text = tweetData.CreatedBy.ScreenName;
        lblTweetSenderUserName.Text = tweetData.CreatedBy.Name;
        lblTweetPublishDate.Text = tweetData.CreatedAt.ToString();
        lblReply.Text = "Reply";
        lblRetweet.Text = "Retweet " + tweetData.RetweetCount.ToString();
        lblLike.Text = "Like " + tweetData.FavoriteCount.ToString();
    }
}