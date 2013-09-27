<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReflectiveList.ascx.cs" Inherits="DAOTest.ReflectiveList" %>
<asp:GridView runat="server" ID="gvContents" HeaderStyle-BackColor="LightGray" AlternatingRowStyle-BackColor="#EEEEEE" SelectedRowStyle-BackColor="LightGreen" OnSelectedIndexChanged="OnIndex">
    <Columns>
        <asp:CommandField ShowSelectButton="True" SelectText="&gt;"/>
    </Columns>
</asp:GridView>