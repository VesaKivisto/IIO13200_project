<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="LogIn.aspx.cs" Inherits="LogIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="divSignIn">
        <asp:ImageButton ID="btnSignIn" runat="server" ImageUrl="~/images/sign-in-with-twitter-link.png" OnClick="btnSignIn_Click" />
    </div>
</asp:Content>