using System;
using Tweetinvi;
using Tweetinvi.Models;

public partial class TwitterAuth : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["authCtx"] == null)
        {
            Response.Redirect("LogIn.aspx");
        }
        // Get authentication context from session variable
        IAuthenticationContext authenticationContext = (IAuthenticationContext)Session["authCtx"];
        // Get oauth verifier code
        var verifierCode = Request.Params.Get("oauth_verifier");
        // Create the user credentials. Set user credentials, get user from them and store the user to session variable
        var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(verifierCode, authenticationContext);
        IAuthenticatedUser user = Tweetinvi.User.GetAuthenticatedUser(userCreds);
        Session["user"] = user;
        // Redirect to main page
        Response.Redirect("MainPage.aspx");
    }
}