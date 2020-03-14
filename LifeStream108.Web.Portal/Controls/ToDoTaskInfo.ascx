<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToDoTaskInfo.ascx.cs" Inherits="LifeStream108.Web.Portal.Controls.ToDoTaskInfo" %>

<div class="bd-example">
    <asp:Button ID="btnSaveTask" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveTask_Click" />

    <div class="input-group">
        <div class="input-group-prepend">
            <span class="input-group-text">Title</span>
        </div>
        <asp:TextBox ID="txtTitle" runat="server" Rows="7" Columns="30" TextMode="MultiLine" CssClass="form-control" />
    </div>
    <div class="input-group">
        <div class="input-group-prepend">
            <span class="input-group-text">Note</span>
        </div>
        <asp:TextBox ID="txtNote" runat="server" Rows="20" Columns="30" TextMode="MultiLine" CssClass="form-control" />
    </div>
</div>
