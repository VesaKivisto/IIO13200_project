using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Tweetinvi;
using Tweetinvi.Models;

/// <summary>
/// Summary description for BLTwitterClient
/// </summary>
public class BLTwitterClient
{
    private bool isRetweeted;
    private bool isFavorited;
    private long tweetIdToReplyTo;
    private List<Control> controls;
    public BLTwitterClient()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public HtmlGenericControl GenerateTweetSenderPictureContainer(ITweet tweetData)
    {
        #region ImgTweetSenderPicture
        HtmlImage imgTweetSenderPicture = new HtmlImage();
        imgTweetSenderPicture.Attributes["src"] = tweetData.CreatedBy.ProfileImageUrl;
        #endregion
        #region TweetSenderPictureContainer
        HtmlGenericControl tweetSenderPictureContainer = new HtmlGenericControl("div");
        tweetSenderPictureContainer.Attributes["class"] = "card-image";
        tweetSenderPictureContainer.Attributes["id"] = "divCardImage";
        #endregion
        tweetSenderPictureContainer.Controls.Add(imgTweetSenderPicture);
        return tweetSenderPictureContainer;
    }
    public HtmlGenericControl GenerateTweetControlsContainer()
    {
        #region TweetControlsContainer
        HtmlGenericControl tweetControlsContainer = new HtmlGenericControl("div");
        tweetControlsContainer.Attributes["class"] = "card-action";
        tweetControlsContainer.Attributes["id"] = "divTweetControls";
        #endregion
        return tweetControlsContainer;
    }
    public HtmlGenericControl GenerateTweetContainer(ITweet tweetData)
    {
        #region LblRetweetedBy
        Label lblRetweetedBy = new Label();
        if (tweetData.RetweetedTweet != null)
        {
            var retweetedBy = tweetData.CreatedBy;
            tweetData = tweetData.RetweetedTweet;
            lblRetweetedBy.Text = retweetedBy.Name + " retweeted<br />";
            lblRetweetedBy.Attributes["class"] = "retweetedByUser";
        }
        #endregion
        #region LblTweetSenderUserName
        Label lblTweetSenderUserName = new Label();
        lblTweetSenderUserName.Text = tweetData.CreatedBy.Name;
        lblTweetSenderUserName.Attributes["class"] = "tweetSenderUserName";
        #endregion
        #region LblTweetSenderScreenName
        Label lblTweetSenderScreenName = new Label();
        lblTweetSenderScreenName.Text = "<a href=ProfilePage.aspx?user=" + tweetData.CreatedBy.ScreenName + " target='_blank' style='text-decoration:none;color:#828282;'>@" + tweetData.CreatedBy.ScreenName + "</a>";
        lblTweetSenderScreenName.Attributes["class"] = "tweetSenderScreenName";
        #endregion
        #region LblTweetPublishDate
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
        #endregion
        #region TweetPublicationData
        HtmlGenericControl tweetPublicationData = new HtmlGenericControl("div");
        tweetPublicationData.Attributes["class"] = "card-content tweet-sender";
        tweetPublicationData.Attributes["id"] = "divTweetSender";
        #endregion
        tweetPublicationData.Controls.Add(lblRetweetedBy);
        tweetPublicationData.Controls.Add(lblTweetSenderUserName);
        tweetPublicationData.Controls.Add(lblTweetSenderScreenName);
        tweetPublicationData.Controls.Add(lblTweetPublishDate);
        #region DivTweetText
        HtmlGenericControl divTweetText = new HtmlGenericControl("div");
        divTweetText.Attributes["class"] = "card-content";
        divTweetText.Attributes["id"] = "divCardContent";
        // Replace new lines with br tag
        divTweetText.InnerHtml = tweetData.Text.Replace("\n", "<br />");
        // Find all links and replace them with hyper links
        divTweetText.InnerHtml = Regex.Replace(divTweetText.InnerHtml,
                                        @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                                        "<a target='_blank' href='$1'>$1</a>");
        divTweetText.InnerHtml = Regex.Replace(divTweetText.InnerHtml,
                                        @"@(\w+)",
                                        "<a target='_blank' href=ProfilePage.aspx?user=$1>@$1</a>");
        #endregion
        #region TweetContainer
        HtmlGenericControl tweetContainer = new HtmlGenericControl("div");
        tweetContainer.Attributes["class"] = "card-stacked";
        tweetContainer.Attributes["id"] = "divCardStacked";
        #endregion
        tweetContainer.Controls.Add(tweetPublicationData);
        tweetContainer.Controls.Add(divTweetText);
        return tweetContainer;
    }
    public HtmlGenericControl GenerateTweetDataContainer()
    {
        #region TweetDataContainer
        HtmlGenericControl tweetDataContainer = new HtmlGenericControl("div");
        tweetDataContainer.Attributes["class"] = "card horizontal";
        tweetDataContainer.Attributes["id"] = "divTweetData";
        #endregion
        return tweetDataContainer;
    }
}