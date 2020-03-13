<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LifeStream108.Web.Portal._Default" %>

<%@ Import Namespace="LifeStream108.Libs.Entities.ToDoEntities" %>
<%@ Register Src="~/Controls/ToDoCategoriesControl.ascx" TagPrefix="uc1" TagName="ToDoCategoriesControl" %>
<%@ Register Src="~/Controls/ToDoListsControl.ascx" TagPrefix="uc1" TagName="ToDoListsControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <hr />
    <uc1:ToDoCategoriesControl runat="server" ID="categoriesControl" OnCategoryChanged="categoriesControl_CategoryChanged" />
    <hr />
    <uc1:ToDoListsControl runat="server" ID="listsControl" />
</asp:Content>
