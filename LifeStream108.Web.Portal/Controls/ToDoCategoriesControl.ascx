<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToDoCategoriesControl.ascx.cs" Inherits="LifeStream108.Web.Portal.Controls.ToDoCategoriesControl" %>

<div class="navbar navbar-inverse navbar-fixed-top">
    <br />
    <div class="container">
        <div class="row">
            <table style="width:100%">
                <tr>
                    <td>
                        <asp:PlaceHolder runat="server" ID="holderCategories"></asp:PlaceHolder>
                    </td>
                    <td style="text-align:right">
                        <% if (deletedTaskId > 0) { %>
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnUndoDeleteTask" runat="server" Text="Activate task" CssClass="btn btn-warning" OnClick="btnUndoDeleteTask_Click" />
                        <% } %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <br />
</div>
