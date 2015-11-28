<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="XmlParser.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function SelectAllCheckboxesSpecific(spanChk) {
            var isChecked = spanChk.checked;
            var chk = spanChk;
            Parent = document.getElementById('<%=gvXMLSource.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != chk && items[i].type == "checkbox") {
                    if (items[i].checked != isChecked) {
                        items[i].click();
                    }
                }
            }
        }
        function HighlightRow(chkB) {
            var IsChecked = chkB.checked;
            if (IsChecked) {
                chkB.parentElement.parentElement.style.backgroundColor = '#228b22';
                chkB.parentElement.parentElement.style.color = 'white';
            } else {
                chkB.parentElement.parentElement.style.backgroundColor = 'white';
                chkB.parentElement.parentElement.style.color = 'black';
            }
        }

        function ValidateGrid(sender, args) {
            var gridView = document.getElementById("<%=gvXMLSource.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    args.IsValid = true;
                    return;
                }
            }
            args.IsValid = false;
        }
        function SelectAllCheckboxes(spanChk) {
            var IsChecked = spanChk.checked;
            var cbxAll = spanChk;
            var Parent = document.getElementById('<%=gvXMLSource.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != cbxAll.id && items[i].type == 'checkbox') {
                    items[i].checked = IsChecked;
                }
            }

            document.getElementById('<%=grid.ClientID %>').innerHTML = '';

            document.getElementById('<%=lblMsg.ClientID %>').value = '';

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Sticky footer wrap -->
    <div id="wrap">
        <!-- Container -->

        <div id="container">
            <!-- Header -->
            <div id="header" class="header">
                <div id="branding">
                    <a href="#">
                        <h1 id="site-name">Xml Generation</h1>
                    </a>
                </div>

                <div class="header-content header-content-first">
                    <div class="header-column icon">
                        <i class="icon-time"></i>
                    </div>
                    <div class="header-column">
                    </div>
                </div>

                <div class="header-content">
                    <div class="header-column icon">
                        <i class="icon-home"></i>
                        <br>
                        <i class="icon-comment"></i>
                    </div>
                    <div class="header-column">
                    </div>
                </div>
                <div id="user-tools">
                    <span class="user-links">
                        <i class="icon-home"></i>
                        <asp:Label ID="lblUser" runat="server" class="btn-high bold"></asp:Label>
                        <i class="icon-lock" style="color: #ffffff;"></i>
                        <asp:HyperLink runat="server" ID="hypUser" NavigateUrl="../default.aspx?id=lg" Text="Logout"></asp:HyperLink>
                    </span>
                </div>
            </div>

            <!-- END Header -->


            <div class="suit-columns two-columns">
                <div id="suit-center" class="suit-column">
                    <ul class="breadcrumb">
                        <li>
                            <a href="#"></a>
                            <span class="divider"><span class="alert-info">» Xml Source & Generation.</span></span>
                        </li>
                        <li class="active"></li>
                    </ul>

                    <!-- Content -->
                    <div id="content" class="flex row-fluid">
                        <div id="content-main">
                            <div class="inner-center-column">
                                <div class="module" id="changelist">
                                    <div class="toolbar-content clearfix">
                                        <div class="object-tools">
                                        </div>
                                        <div id="toolbar" class="clearfix">
                                        </div>

                                    </div>
                                    <div>
                                        <div id="divResults" runat="server" style="display: block;" class="results">
                                            <div class="breadcrumb">
                                                <span class="alert-info"></span>
                                                <br />
                                                <div class="form-horizontal" style="padding-top: 20px;">
                                                    <asp:ValidationSummary runat="server" ID="valSum" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ForeColor="Red" />
                                                    <div>
                                                        <asp:Label runat="server" ID="lblMsg" CssClass="alert-danger"></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="two-columns">
                                                    <div class="left-column"></div>
                                                    <div class="right-column">
                                                        <div style="display: block;" class="results">
                                                            <div class="breadcrumb">
                                                                <span class="alert-info"></span>
                                                                <br />
                                                                <div class="form-horizontal" style="padding-top: 10px;">

                                                                    <div class="two-columns">
                                                                        <div class="left-column"></div>
                                                                        <div class="right-column">
                                                                            <asp:ValidationSummary runat="server" ID="ValidationSummary2" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="imp" ForeColor="red" />
                                                                            <table class="table table-striped table-bordered table-hover table-condensed table-mptt" style="border-collapse: collapse; width: 60%;">
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <i class="add-row">+</i>Import Source Urls
                                                                                    </td>
                                                                                </tr>
                                                                                <tr nowrap="nowrap">
                                                                                    <td nowrap="nowrap">
                                                                                        <span class="control-label">Select a comma separated text file:</span>

                                                                                        <asp:FileUpload ID="fileUploadCtl" runat="server" Width="350px" Style="cursor: pointer; margin-left: 100px;" CssClass="input-xxlarge" />

                                                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ErrorMessage="Please select a txt file to import URLs!"
                                                                                            Text="!" ForeColor="Red" ValidationGroup="imp" ControlToValidate="fileUploadCtl"></asp:RequiredFieldValidator>

                                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.txt)$"
                                                                                            ControlToValidate="fileUploadCtl" ValidationGroup="imp" runat="server" ForeColor="Red" Text="!" ErrorMessage="Please select a valid txt file."
                                                                                            Display="Dynamic" />
                                                                                    </td>

                                                                                    <td style="width: 80px;">
                                                                                        <asp:Button ID="btnImport" runat="server" ValidationGroup="imp" CausesValidation="True" CssClass="btn btn-warning" Text="+ Import" OnClick="btnImport_OnClick" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <br />


                                                                            <asp:ValidationSummary runat="server" ID="ValidationSummary1" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="add" ForeColor="red" />
                                                                            <table class="table table-striped table-bordered table-hover table-condensed table-mptt" style="border-collapse: collapse; width: 60%;">
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <i class="add-row">+</i>Add New Source
                                                                                    </td>
                                                                                </tr>
                                                                                <tr nowrap="nowrap">
                                                                                    <td nowrap="nowrap">
                                                                                        <span>Source URL:</span><asp:TextBox ID="txtSourceURLAdd" runat="server" Width="250" />
                                                                                        <asp:RequiredFieldValidator runat="server" ID="reqUserId" ErrorMessage="Source URL is required!"
                                                                                            Text="!" ForeColor="Red" ValidationGroup="add" ControlToValidate="txtSourceURLAdd"></asp:RequiredFieldValidator>
                                                                                    </td>
                                                                                    <td nowrap="nowrap">
                                                                                        <span class="control-label">Source Type:</span><asp:DropDownList ID="ddlSourceType" CssClass="dropdown" runat="server" Width="80">
                                                                                            <asp:ListItem Selected="True">Select</asp:ListItem>
                                                                                            <asp:ListItem>Xml</asp:ListItem>
                                                                                            <asp:ListItem>Zip</asp:ListItem>
                                                                                        </asp:DropDownList>
                                                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ErrorMessage="Source Type is required!"
                                                                                            Text="!" ForeColor="Red" ValidationGroup="add" InitialValue="Select" ControlToValidate="ddlSourceType"></asp:RequiredFieldValidator>
                                                                                    </td>
                                                                                    <td style="width: 80px;">
                                                                                        <asp:Button ID="btnAdd" runat="server" ValidationGroup="add" CausesValidation="True" CssClass="btn btn-primary" Text="+ Add   " OnClick="btnAdd_OnClick" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>

                                                                            <div style="width: 50%;">
                                                                                <hr />
                                                                                <strong><span class="alert-info">Select one or more URLs below & Click on 'Generate Output' to process.</span></strong><div style="float: right;">
                                                                                    <asp:LinkButton ID="btnGetenerate" runat="server" CausesValidation="True" CssClass="btn btn-info" ValidationGroup="sel"
                                                                                        Text="Generate Output" OnClientClick="ValidateGrid" OnClick="btnGetenerate_OnClick"></asp:LinkButton>
                                                                                </div>
                                                                                <hr />
                                                                                <asp:ValidationSummary runat="server" ID="ValidationSummary3" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="grid" ForeColor="red" />
                                                                                <asp:CustomValidator ID="CustomValidator1" ValidationGroup="sel" runat="server" ErrorMessage="Please select at least one record."
                                                                                    ClientValidationFunction="ValidateGrid" ForeColor="Red"></asp:CustomValidator>
                                                                                <asp:GridView ID="gvXMLSource" runat="server" Width="100%" DataKeyNames="Srno" AutoGenerateColumns="False" HeaderStyle-CssClass="visible-desktop" CssClass="table table-striped table-bordered table-hover table-condensed table-mptt" GridLines="None"
                                                                                    CellPadding="0" border="0" AllowPaging="False" EmptyDataText="No records has been added."
                                                                                    OnRowDataBound="gvXMLSource_OnRowDataBound" OnRowEditing="gvXMLSource_OnRowEditing" OnRowCancelingEdit="gvXMLSource_OnRowCancelingEdit"
                                                                                    OnRowUpdating="gvXMLSource_OnRowUpdating" OnRowDeleting="gvXMLSource_OnRowDeleting">
                                                                                    <AlternatingRowStyle CssClass="row2" />
                                                                                    <RowStyle CssClass="row1" />
                                                                                    <HeaderStyle CssClass="sortable" />
                                                                                    <Columns>
                                                                                        <asp:TemplateField HeaderText="Roles" ItemStyle-Width="40">
                                                                                            <HeaderTemplate>
                                                                                                <asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecific(this);" runat="server" />
                                                                                            </HeaderTemplate>
                                                                                            <ItemTemplate>
                                                                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="Source URL" ItemStyle-Width="400">
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lblSourceURL" runat="server" Text='<%# Eval("URL") %>'></asp:Label>
                                                                                            </ItemTemplate>
                                                                                            <EditItemTemplate>
                                                                                                <asp:TextBox ID="txtSourceURL" runat="server" Text='<%# Eval("URL") %>'></asp:TextBox>
                                                                                                <asp:RequiredFieldValidator runat="server" ID="reqGridSourceUrl" ErrorMessage="Source Url is required!"
                                                                                                    Text="!" ForeColor="Red" ValidationGroup="grid" ControlToValidate="txtSourceURL"></asp:RequiredFieldValidator>
                                                                                            </EditItemTemplate>
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Source Type" ItemStyle-Width="100">
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lblSourceType" runat="server" Text='<%# Eval("Type") %>'></asp:Label>
                                                                                            </ItemTemplate>
                                                                                            <EditItemTemplate>
                                                                                                <asp:DropDownList ID="ddlGridSourceType" CssClass="dropdown" runat="server" Width="80">
                                                                                                    <asp:ListItem Selected="True">Select</asp:ListItem>
                                                                                                    <asp:ListItem>Xml</asp:ListItem>
                                                                                                    <asp:ListItem>Zip</asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                                <asp:RequiredFieldValidator runat="server" ID="reqGridSourceType" ErrorMessage="Source Type is required!"
                                                                                                    Text="!" ForeColor="Red" ValidationGroup="grid" InitialValue="Select" ControlToValidate="ddlGridSourceType"></asp:RequiredFieldValidator>
                                                                                            </EditItemTemplate>
                                                                                        </asp:TemplateField>

                                                                                        <asp:CommandField ButtonType="Image" EditImageUrl="../CSS/edit.png"
                                                                                            DeleteImageUrl="../CSS/delete.png" DeleteText="Delete"
                                                                                            EditText="Edit" ShowEditButton="true"
                                                                                            CancelImageUrl="../CSS/cancel.png" CancelText="Cancel"
                                                                                            UpdateImageUrl="../CSS/Save.png" ValidationGroup="grid"
                                                                                            CausesValidation="True" UpdateText="Save"
                                                                                            ShowDeleteButton="true" ItemStyle-Width="70">
                                                                                            <ItemStyle HorizontalAlign="Right" />
                                                                                        </asp:CommandField>
                                                                                    </Columns>
                                                                                </asp:GridView>
                                                                                <div style="float: right;">
                                                                                    <asp:LinkButton ID="lnkClear" runat="server" CausesValidation="False" CssClass="btn btn-success"
                                                                                        Text="  Reset  " OnClientClick="SelectAllCheckboxes(false);return false;"></asp:LinkButton>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>

                                                    <div id="grid" runat="server">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- END Content -->
                    </div>


                </div>


                <div id="suit-left" class="suit-column">
                    <div class="left-nav" id="left-nav" style="margin-left: 15px;">
                        <ul>
                            <li class="alert-info">
                                <asp:HyperLink runat="server" CssClass="user-links" ID="hypHome" NavigateUrl="../Admin/Dashboard.aspx" Text="Home"></asp:HyperLink>
                            </li>
                            <li class="alert-info">
                                <a href="../Secured/XmlHistory.aspx" class="user-links">View Output Xmls</a>
                            </li>
                        </ul>
                    </div>
                </div>



            </div>


            <!-- Sticky footer push -->
            <div id="push"></div>
        </div>
    </div>

</asp:Content>
