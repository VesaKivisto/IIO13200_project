<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProfilePage.aspx.cs" Inherits="ProfilePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/css/materialize.min.css" />
    <link rel="stylesheet" type="text/css" href="css/master.css" />
    <link rel="stylesheet" type="text/css" href="css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div style="height:750px;width:1500px;">
            <!-- Main container -->
            <div style="width:20%;float:left;margin-right:4px;">
                <!-- Profile container -->
                <div style="margin:15px;">
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
                    <!-- Tweets, followers, following -->
                    <div style="margin-left:12px;">
                        <asp:Label ID="lblTweets" runat="server" />
                        <br />
                        <asp:Label ID="lblFollowing" runat="server" />
                        <br />
                        <asp:Label ID="lblFollowers" runat="server" />
                        <br />
                        <asp:Label ID="lblMessages" runat="server" />
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
    </form>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/js/materialize.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
</body>
</html>
