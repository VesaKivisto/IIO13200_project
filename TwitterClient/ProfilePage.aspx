<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ProfilePage.aspx.cs" Inherits="ProfilePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script>
        // Function to set button status when writing a new tweet. Checks the tweet length and enables or disables the button when needed
        // Tried to put this on Master Page, but it didn't find any elements
        function SetButtonStatus() {
            var maxLength = 140;
            var txtTweet = document.getElementById('<%=txtTweet.ClientID%>').value;
            var tweetLengthLabel = document.getElementById('<%=lblTweetLength.ClientID%>');
            var sendButton = document.getElementById('<%=btnSendTweet.ClientID%>');
            if (txtTweet.length > 0 && txtTweet.length <= 140) {
                sendButton.classList.add("waves-effect");
                sendButton.classList.add("waves-light");
                sendButton.classList.add("submit");
                sendButton.classList.remove("disabled");
            }
            else {
                sendButton.classList.remove("waves-effect");
                sendButton.classList.remove("waves-light");
                sendButton.classList.remove("submit");
                sendButton.classList.add("disabled");
                tweetLengthLabel.style.color = "red";
            }
            if (txtTweet.length >= 0 && txtTweet.length <= 140) {
                tweetLengthLabel.style.color = "black";
            }
            tweetLengthLabel.innerHTML = maxLength - tweetText.length;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager runat="server" EnablePageMethods="true" EnablePartialRendering="true"></asp:ScriptManager>
    <div style="height:750px;width:1500px;">
        <!-- Main container -->
        <div style="width:20%;float:left;margin-left:20px;">
            <!-- Profile container -->
            <div style="margin-top:15px;">
                <!-- Profile image -->
                <div style="display:inline-block;margin-left:12px;">
                    <asp:Image ID="imgProfilePicture" runat="server" src="" />
                </div>
                <!-- Name -->
                <div style="display:inline-block;position:relative;bottom:10px;left:10px;height:70px;">
                    <asp:Label ID="lblUserName" runat="server" Font-Bold="true" Font-Size="Larger"/>
                    <br />
                    <asp:Label ID="lblScreenName" runat="server" />
                </div>
                <!-- User description -->
                <div id="userDescr" runat="server" style="margin-left:12px;margin-bottom:15px;">

                </div>
                <!-- Tweets, followers, following -->
                <div style="margin-left:12px;">
                    <asp:Label ID="lblTweets" runat="server" />
                    <br />
                    <asp:Label ID="lblFollowing" runat="server" />
                    <br />
                    <asp:Label ID="lblFollowers" runat="server" />
                    <br />
                </div>
            </div>
        </div>
        <!-- Timeline container -->
        <div class="timeLineContainer" id="divTimeline" runat="server" >
            <div class="headerDiv" >
                <h4>Tweets</h4>
            </div>
            <!-- Dynamically added stuff here -->
        </div>
    </div>
    <!-- Exception container -->
    <div style="margin-left:30px;margin-top:50px;">
        <asp:Label ID="lblError" runat="server" />
    </div>
    <!--
        Reply modal, maybe trying to get this work. Or not, most likely scrapped idea for now
    <div>
        <div id="divReply" class="modal" style="width:30%;">
            <div class="modal-content">
                <asp:TextBox ID="txtTweet" runat="server" TextMode="MultiLine" Placeholder="New tweet" Rows="10" Columns="36" style="margin:10px 10px 5px 12px;resize:none;"
                    oninput="SetButtonStatus()"/>
                <asp:Label ID="lblTweetLength" runat="server" Text="140" style="margin:10px 10px 5px 12px;" />
                <asp:Button ID="btnSendTweet" runat="server" Text="Send tweet" class="btn disabled light-blue" style="float:right;margin-right:48px;" />
            </div>
        </div>    
    </div>
    -->
    <!--
        Retweet modal, maybe trying to get this work. Or not, most likely scrapped idea for now
    <div>
        <div id="divRetweet" class="modal">
            <div class="modal-content">
                <div class="card horizontal">
                    <div class="card-image">
                        <img src="http://pbs.twimg.com/profile_images/579341575907602433/_hNgwA56_normal.png">
                    </div>
                    <div class="card-stacked">
                        <div class="card-content tweet-sender">
                            <span></span>
                            <span class="tweetSenderUserName">Vesa Kivistö</span>
                            <span class="tweetSenderScreenName">
                                <a href="ProfilePage.aspx?user=Troikku" target="_blank" style="text-decoration:none;color:#828282;">@Troikku</a>
                            </span>
                            <span class="tweetPublicationDate">Nov 15</span>
                        </div>
                        <div class="card-content">Se tunne kun Windowsin päivitys rikkoo koko asennuksen. Vuosi 2016 ja vielä käy näin.</div>
                        <div class="card-action">
                            <a href="#divRetweet">asd</a>
                            <input type="submit" name="ctl00$ContentPlaceHolder1$ctl12" value="0" class="retweet" style="background-image:url('../images/retweet.png'););">
                            <input type="submit" name="ctl00$ContentPlaceHolder1$ctl13" value="1" class="like" style="background-image:url('../images/like.png'););">
                        </div>
                    </div>
                </div>
            </div>
        </div>    
    </div>
    -->
</asp:Content>