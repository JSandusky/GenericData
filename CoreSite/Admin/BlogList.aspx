<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BlogList.aspx.cs" Inherits="CoreSite.Admin.BlogList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button runat="server" ID="newPost" Text="Create Post" OnClick="onNewPost" />
    <asp:Repeater runat="server" ID="rptExisting" OnItemDataBound="onRptBind">
    <ItemTemplate>
    <div style="padding-top: 10px;">
        <b><asp:Hyperlink runat="server" ID="lnkTitle" Text='<%# Eval("PostTitle") %>' /></b>
    </div>
    <div>
        <asp:Label runat="server" ID="lblDate" Text='<%# Eval("PostDate") %>' />
    </div>
    <div>
        <asp:Repeater runat="server" ID="rptTags">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblTagName" Text='<%# Eval("TagName") %>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </ItemTemplate>
    </asp:Repeater>
</asp:Content>
