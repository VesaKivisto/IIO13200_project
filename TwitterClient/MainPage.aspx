<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MainPage.aspx.cs" Inherits="MainPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script>
        function SetButtonStatus() {
            var maxLength = 140;
            var tweetText = document.getElementById('<%=txtTweet.ClientID%>').value;
            var tweetLengthLabel = document.getElementById('<%=lblTweetLength.ClientID%>');
            var sendButton = document.getElementById('<%=btnSendTweet.ClientID%>');
            if (tweetText.length > 0 && tweetText.length <= 140) {
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
            if (tweetText.length >= 0 && tweetText.length <= 140) {
                tweetLengthLabel.style.color = "black";
            }
            tweetLengthLabel.innerHTML = maxLength - tweetText.length;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="height:750px;width:1500px;position:absolute;top:0">
        <!-- Main container -->
        <div style="width:20%;float:left;margin-left:20px;">
            <!-- New tweet container -->
            <div style="height:250px;">
                <asp:TextBox ID="txtTweet" runat="server" TextMode="MultiLine" Placeholder="New tweet" Rows="10" Columns="36" class="textarea" style="margin:10px 10px 5px 12px;"
                    oninput="SetButtonStatus()" />
                <br />
                <asp:Label ID="lblTweetLength" runat="server" Text="140" style="margin:10px 10px 5px 12px;" />
                <asp:Button ID="btnSendTweet" runat="server" Text="Send tweet" OnClick="btnSendTweet_Click" class="btn disabled light-blue" style="float:right;margin-right:48px;" />
            </div>
            <!-- Profile container -->
            <div>
                <!-- Profile image -->
                <div style="display:inline-block;margin-left:12px;">
                    <asp:Image ID="imgProfilePicture" runat="server" src="" />
                </div>
                <!-- Name -->
                <div style="display:inline-block;position:relative;bottom:10px;left:10px;height:70px;">
                    <asp:Label ID="lblScreenName" runat="server" Font-Bold="true" Font-Size="Larger"/>
                    <br />
                    <asp:Label ID="lblUserName" runat="server" />
                </div>
            </div>
        </div>
        <!-- Timeline container -->
        <div class="timeLineContainer" id="divTimeline" runat="server" >
            <div class="headerDiv" >
                <h4>Timeline</h4>
            </div>
            <!-- Dynamically added stuff here -->
        </div>
        <!-- Mentions container -->
        <div class="mentionsContainer" id="divMentions" runat="server" >
            <div class="headerDiv" >
                <h4>Mentions</h4>
            </div>
            <!-- Dynamically added stuff here -->
        </div>
    </div>
</asp:Content>

