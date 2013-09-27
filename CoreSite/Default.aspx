<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CoreSite.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Repeater runat="server" ID="rptNews" OnItemDataBound="onBindNews">
        <ItemTemplate>
            <div style="padding-top: 10px;">
                <div style="float: left; width: 25%; text-align: center;">
                    <div>
                        <asp:Label runat="server" ID="lblTitle" Text='<%# Eval("PostTitle") %>' />
                    </div>
                    <div>
                        <asp:Label runat="server" ID="lblDate" Text='<%# string.Format("{0:MM/dd/yyyy}",Eval("PostDate")) %>' />
                    </div>
                </div>
                <div style="float: right; width: 75%;">
                    <asp:Repeater runat="server" ID="rptSections" OnItemDataBound="onBindSection">
                        <ItemTemplate>
                            <div>
                            <asp:Label runat="server" ID="sectionTxt" Text='<%# Eval("Content") %>' />
                            <div style="text-align: center;">
                            <asp:Image runat="server" ID="sectionImage" Visible="false" />
                            </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div style="clear: both;"></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
