<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function check(obj)
        {
            if (obj.value.trim() != "") {
                document.getElementById("btnSendTweet").disabled = false;
            }
            else {
                document.getElementById("btnSendTweet").disabled = true;
            }
        }
    </script>
    <link rel="stylesheet" type="text/css" href="css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div style="height:750px;width:1500px;">
            <!-- Profile container -->
            <div style="width:20%;float:left;margin-right:4px;">
                <!-- New tweet container -->
                <div style="height:250px;">
                    <asp:TextBox ID="txtTweet" runat="server" TextMode="MultiLine" Placeholder="New tweet" Rows="10" Columns="36" style="margin:10px 10px 5px 12px;resize:none;"
                        oninput="return check(this);"/>
                    <asp:Button ID="btnSendTweet" runat="server" Text="Send tweet" Enabled="False" OnClick="btnSendTweet_Click" style="float:right;margin-right:13px;"/>
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
                    <!-- Tweets, followers, following -->
                    <div style="margin-left:12px;">
                        <asp:Label ID="lblTweets" runat="server" />
                        <br />
                        <asp:Label ID="lblFollowing" runat="server" />
                        <br />
                        <asp:Label ID="lblFollowers" runat="server" />
                    </div>
                </div>
            </div>
            <!-- Timeline container -->
            <div class="timeLineContainer" id="divTimeline" runat="server" >
                
            </div>
            <!-- Notifications container -->
            <!-- Apparently Twitter API doesn't allow fetching likes and follow notifications, so this div will be only used for mentions -->
            <div class="mentionsContainer" id="divMentions" runat="server" >

            </div>
        </div>
    </form>
</body>
</html>
