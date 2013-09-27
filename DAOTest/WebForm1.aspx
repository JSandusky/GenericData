<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="DAOTest.WebForm1" %>
<%@ Register Src="~/ReflectiveForm.ascx" TagName="ReflectiveForm" TagPrefix="GUI" %>
<%@ Register Src="~/ReflectiveList.ascx" TagName="ReflectiveList" TagPrefix="GUI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        .HiddenColumn{display:none;}
    </style>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <act:ToolkitScriptManager ID="ToolkitScriptManager2" runat="server"/>
    <div style="float:left;">
        <asp:Button runat="server" ID="newButton" Text="Create New" OnClick="onNew"/>
        <GUI:ReflectiveList runat="server" id="listFrom" OnSelectionChanged="OnGridSelectionChanged"/>
    </div>
    <div style="float:left; margin-left: 20px;">
        <GUI:ReflectiveForm runat="server" id="formThing" DisplayOnly="false"/>
    </div>
    <div style="clear:both;"></div>
    <asp:TextBox runat="server" ID="txtCSV" />

    </form>
</body>
</html>
