<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LifeStream108.Web.Portal.Login" %>

<%@ Register Src="~/Controls/ShowInfoControl.ascx" TagPrefix="uc1" TagName="ShowInfoControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:ShowInfoControl runat="server" ID="showInfoControl" />

    <div class="form-horizontal">
        <div class="form-group">
            <span class="col-md-2 control-label">Email</span>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <span class="col-md-2 control-label">Password</span>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-default" OnClick="btnLogin_Click" />
            </div>
        </div>
    </div>
</asp:Content>
