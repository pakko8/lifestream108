<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToDoTasksControl.ascx.cs" Inherits="LifeStream108.Web.Portal.Controls.ToDoTasksControl" %>

<div class="bd-example">
    <table>
        <tr>
            <td>
                <asp:TextBox ID="txtNewTaskTitle" runat="server" placeholder="New Task Title" CssClass="form-control input-large" />
            </td>
            <td>
                <asp:Button ID="btnAddNewTask" runat="server" Text="Add Task" OnClick="btnAddNewTask_Click" CssClass="btn btn-outline-secondary" />
            </td>
        </tr>
    </table>

    <div id="divTasks" runat="server" class="btn-group-vertical" role="group" aria-label="Vertical button group" />
</div>
