<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BlogEditor.aspx.cs" Inherits="CoreSite.Admin.BlogEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="padding: 10px;">
    <div style="float: left;">
        Post Title: 
    <asp:TextBox runat="server" ID="txtTitle" />
    </div>
    <div style="float: right;">
        <asp:Button runat="server" ID="btnSave" OnClick="onSave" Text="Save"/>
    </div>
    <div style="clear: both;"></div>
    <div>
        <asp:CheckBox runat="server" ID="chkVis" Text="Visible"/><asp:CheckBox runat="server" ID="chkNews" Text="News"/>
    </div>
    <div style="padding-top: 10px;">
        <asp:Button runat="server" ID="btnAddSection" Text="Add Section" OnClick="onAddSection" />
    </div>
    <div>
        <asp:Repeater runat="server" ID="rptSections" OnItemDataBound="contentDataBind">
        <ItemTemplate>
            <div>Text:</div>
            <div><asp:TextBox runat="server" ID="txtContent" TextMode="MultiLine" Width="400" /></div>
            <asp:Image runat="server" ID="img" Visible="false" />
            <div>Attach Image: <asp:FileUpload runat="server" ID="flUpload" /></div>
        </ItemTemplate>
        <SeparatorTemplate><hr /></SeparatorTemplate>
        </asp:Repeater>
    </div>
</div>
</asp:Content>
