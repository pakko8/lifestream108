<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToDoTaskInfoControl.ascx.cs" Inherits="LifeStream108.Web.Portal.Controls.ToDoTaskInfoControl" %>

<div class="bd-example">
    <asp:Button ID="btnSaveTask" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveTask_Click" />
    <hr />

    <div class="input-group">
        <div class="input-group-prepend">
            <h4>Title</h4>
        </div>
        <asp:TextBox ID="txtTitle" runat="server" Rows="7" Columns="30" TextMode="MultiLine" CssClass="form-control" />
    </div>
    <br />

    <div class="input-group mb-3">
        <asp:TextBox ID="txtReminderTime" runat="server" CssClass="form-control" placeholder="dd.MM.yyyy HH:mm"></asp:TextBox>
    </div>

    <div class="input-group mb-3">
        <div class="input-group-prepend">
            <h4>Remind every</h4>
        </div>
        <table>
            <tr>
                <td><asp:TextBox ID="txtReminderRepeatValue" runat="server" CssClass="form-control input-mini" placeholder="0"></asp:TextBox></td>
                <td><asp:DropDownList ID="ddlReminderRepeatType" runat="server" CssClass="form-control" /></td>
            </tr>
        </table>
    </div>
    <br />

    <div class="input-group">
        <div class="input-group-prepend">
            <h4>Note</h4>
        </div>
        <asp:TextBox ID="txtNote" runat="server" Rows="20" Columns="30" TextMode="MultiLine" CssClass="form-control" />
    </div>
    <hr />

    <asp:Button ID="btnDeleteTask" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDeleteTask_Click" />
</div>
