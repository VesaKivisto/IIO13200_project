<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MainPage.aspx.cs" Inherits="MainPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function SetButtonStatus() {
            var maxLength = 140;
            var tweetText = document.getElementById('<%=txtTweet.ClientID%>').value;
            var tweetLengthLabel = document.getElementById('<%=lblTweetLength.ClientID%>');
            if ( tweetText.length > 0 && tweetText.length <= 140) {
                $('#btnSendTweet').addClass("waves-effect waves-light submit").removeClass('disabled');
            }
            else {
                $('#btnSendTweet').removeClass("waves-effect waves-light submit").addClass('disabled');
                tweetLengthLabel.style.color = "red";
            }
            if (tweetText.length >= 0) {
                tweetLengthLabel.style.color = "black";
            }
            tweetLengthLabel.innerHTML = maxLength - tweetText.length;
        }
    </script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/css/materialize.min.css" />
    <link rel="stylesheet" type="text/css" href="css/master.css" />
    <!--
    <link rel="stylesheet" type="text/css" href="css/style.css" />
        -->
</head>
<body>
    <form id="form1" runat="server">
        <div style="height:750px;width:1500px;">
            <!-- Main container -->
            <div style="width:20%;float:left;margin-right:4px;">
                <!-- New tweet container -->
                <div style="height:250px;">
                    <asp:TextBox ID="txtTweet" runat="server" TextMode="MultiLine" Placeholder="New tweet" Rows="10" Columns="36" style="margin:10px 10px 5px 12px;resize:none;"
                        oninput="SetButtonStatus()"/>
                    <br />
                    <asp:Label ID="lblTweetLength" runat="server" Text="140" style="margin:10px 10px 5px 12px;" />
                    <asp:Button ID="btnSendTweet" runat="server" Text="Send tweet" OnClick="btnSendTweet_Click" class="btn disabled light-blue" />
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
    </form>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/js/materialize.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
</body>
</html>
