<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReflectiveForm.ascx.cs" Inherits="DAOTest.ReflectiveForm" %>
<%@ Register TagPrefix="ctrl" TagName="DataObjectPicker" Src="DataObjectPicker.ascx" %>
<div style="border: solid 1px #CDCDCD; padding: 5px; -webkit-border-radius: 10px;-moz-border-radius: 10px;border-radius: 10px;">
    <div style="border: solid 1px #DDDDDD; -webkit-border-radius: 0px 0px 10px 10px;-moz-border-radius: 0px 0px 10px 10px;border-radius: 0px 0px 10px 10px; background-color:#F7F7F7; margin-top: -6px; margin-left: 5px; margin-right: 5px;">
        <h3 style="text-align: center;"><asp:Label runat="server" ID="lblTitle" Text="Edit "></asp:Label></h3>
    </div>
    <div style="border: solid 1px #DDDDDD; background-color: #F7F7F7; padding: 5px; -webkit-border-radius: 10px;-moz-border-radius: 10px;border-radius: 10px;margin-top: 10px;">
        <asp:Repeater ID="contents" runat="server" OnItemDataBound="onDataBind">
            <ItemTemplate>
                <div style="width: 400px">
                    <div style="float: left; width: 100px;">
                        <asp:Panel ID="contentPanel" runat="server" />
                    </div>
                    <div style="float: right; position:relative; left: -10px; width: 250px;">
                        <asp:Panel ID="controlPanel" runat="server" />
                    </div>
                    <div style="clear: both"></div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div style="width:300px; padding: 5px;">
            <div style="float: left;">
                <asp:Button ID="save" runat="server" OnClick="OnSave" Text="Save" />
            </div>
            <div style="float: right;">
                <asp:Button ID="Button1" runat="server" OnClick="OnDelete" Text="Delete" />
                <asp:Button ID="cancel" runat="server" OnClick="OnCancel" Text="Cancel" />
            </div>
            <div style="clear: both;"></div>
    </div>

    <script type="text/javascript">
        //This code must be placed below the ScriptManager, otherwise the "Sys" cannot be used because it is undefined.
        Sys.Application.add_init(function () {
            // Store the color validation Regex in a "static" object off of
            // AjaxControlToolkit.ColorPickerBehavior.  If this _colorRegex object hasn't been
            // created yet, initialize it for the first time.
            if (!Sys.Extended.UI.ColorPickerBehavior._colorRegex) {
                Sys.Extended.UI.ColorPickerBehavior._colorRegex = new RegExp('^[A-Fa-f0-9]{6}$');
            }
        });
        function colorChanged(sender) {
            sender.get_element().style.color =
       "#" + sender.get_selectedColor();
        }
    </script>
</div>