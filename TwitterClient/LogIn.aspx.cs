using System;
using Tweetinvi;
using Tweetinvi.Models;

public partial class LogIn : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] != null)
        {
            Session["user"] = null;
        }
        IAuthenticationContext authenticationContext;
        // App credentials; Key + Secret
        var appCreds = new ConsumerCredentials("YIlzibzr7LNJlsgMX6BpqWdpn", "ihK9Z2jEmEuDHqeMmrojSM5nza8wtHOxX8EKQFTrD2TRQuwEtj");
        // Set URL to redirect to after authentication
        var redirectURL = "http://" + Request.Url.Authority + "/TwitterAuth.aspx";
        // Authentication context, used when signing in with Twitter. This will also be stored to session variable for later use
        authenticationContext = AuthFlow.InitAuthentication(appCreds, redirectURL);
        Session["authCtx"] = authenticationContext;
        // Redirect to Twitter authentication
        Response.Redirect(authenticationContext.AuthorizationURL);
    }
}