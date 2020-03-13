<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LifeStream108.Web.Portal._Default" %>

<%@ Import Namespace="LifeStream108.Libs.Entities.ToDoEntities" %>
<%@ Register Src="~/Controls/ToDoCategoriesControl.ascx" TagPrefix="uc1" TagName="ToDoCategoriesControl" %>
<%@ Register Src="~/Controls/ToDoListsControl.ascx" TagPrefix="uc1" TagName="ToDoListsControl" %>
<%@ Register Src="~/Controls/ToDoTasksControl.ascx" TagPrefix="uc1" TagName="ToDoTasksControl" %>
<%@ Register Src="~/Controls/ToDoTaskInfo.ascx" TagPrefix="uc1" TagName="ToDoTaskInfo" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <hr />
    <uc1:ToDoCategoriesControl runat="server" ID="categoriesControl" OnCategoryChanged="categoriesControl_CategoryChanged" />
    <hr />
    <table>
        <tr>
            <td style="vertical-align:top">
                <uc1:ToDoListsControl runat="server" ID="listsControl" />
            </td>
            <td>&nbsp;</td>
            <td style="vertical-align:top">
                <uc1:ToDoTasksControl runat="server" ID="tasksControl" />
            </td>
            <td>
                <uc1:ToDoTaskInfo runat="server" ID="taskInfoControl" />
            </td>
        </tr>
    </table>
</asp:Content>
