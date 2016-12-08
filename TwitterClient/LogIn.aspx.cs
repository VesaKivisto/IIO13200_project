using System;
using Tweetinvi;
using Tweetinvi.Models;

public partial class LogIn : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This is the page that the user starts from, so set user variable in session to null
        Session["user"] = null;
    }
    // Simple sign in button. Redirects user to page that checks authentication.
    protected void btnSignIn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        IAuthenticationContext authenticationContext;
        // App credentials; Key + Secret. These should be queried from a database, but I couldn't access my database so they're visible here. It's bad I know.
        var appCreds = new ConsumerCredentials("eF3RYgI97aNmDambiGsYT3W0q", "uTjmvgNematqGeqwU7O5yZHlTpiRwwtqrsevLNnA1Pi3TsRe4m");
        // Set URL to redirect to after authentication
        var redirectURL = "http://" + Request.Url.Authority + "/TwitterAuth.aspx";
        // Authentication context, used when signing in with Twitter. This will also be stored to session variable for later use
        authenticationContext = AuthFlow.InitAuthentication(appCreds, redirectURL);
        Session["authCtx"] = authenticationContext;
        // Redirect to Twitter authentication
        Response.Redirect(authenticationContext.AuthorizationURL);
    }
}