<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LifeStream108.Web.Portal._Default" %>

<%@ Import Namespace="LifeStream108.Libs.Entities.ToDoEntities" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-4">
            <h2>Categories</h2>
            <asp:PlaceHolder runat="server" ID="categoryButtonsHolder"></asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
