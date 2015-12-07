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
    <!-- Modal -->

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
                    <div class="row">
                        <div >
                            <i class="glyphicon glyphicon-user"></i>
                            <asp:Label ID="lblUser" runat="server" class="btn-high bold"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="pull-right">
                            <i class="glyphicon glyphicon-log-out"></i>
                            <asp:HyperLink runat="server" ID="hypUser" NavigateUrl="../default.aspx?id=lg" Text="Logout"></asp:HyperLink>
                        </div>
                    </div>
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
                                                                                        <asp:HiddenField runat="server" ID="hidSourceId" />
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
                                                                                        <asp:LinkButton ID="btnLnkImport" runat="server" ValidationGroup="imp" CausesValidation="True" ToolTip="Import source from a comma separated text file!" CssClass="btn btn-warning" Text="Import" OnClick="btnImport_OnClick" >
                                                                                            <i class="glyphicon glyphicon-import"></i> <span class="bold"> Import</span>
                                                                                        </asp:LinkButton>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <br />


                                                                            <asp:ValidationSummary runat="server" ID="ValidationSummary1" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="add" ForeColor="red" />
                                                                            <table class="table table-striped table-hover table-condensed table-mptt" style="border-collapse: collapse; width: 60%;">
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
                                                                                        <asp:LinkButton ID="btnAddLink" runat="server" ValidationGroup="add" CausesValidation="True" ToolTip="Add source Url!" 
                                                                                            CssClass="btn btn-primary" Text="Add" OnClick="btnAdd_OnClick" ><i class="glyphicon glyphicon-plus"></i> <span class="bold">  Add</span></asp:LinkButton>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                             
                                                                            <div style="width: 50%;">
                                                                                <hr />
                                                                                <strong><span class="alert-info">Select one or more URLs below & Click on 'Generate Output' to process.</span></strong>
                                                                                <hr />
                                                                                <div class="alert" role="alert">
                                                                                   <strong>Important:</strong> If "Select Channels" button is red it means no channels are selected<br/> for corressponding source.
                                                                                    Please click on "Select Channels" button to set the channels.
                                                                                </div>
                                                                                <div style="float: right; margin-right: 12px;">
                                                                                    <asp:LinkButton ID="btnGetenerate" runat="server" CausesValidation="True" CssClass="btn btn-info" ValidationGroup="sel"
                                                                                        Text="Generate Output" OnClientClick="ValidateGrid" OnClick="btnGetenerate_OnClick"
                                                                                        ToolTip="Generate EPG Xml for each channel in selected sources!">
                                                                                        <i class="glyphicon glyphicon-cog"></i> <span class="bold">  Generate Xml</span>
                                                                                    </asp:LinkButton>
                                                                                </div>
                                                                                <br/>
                                                                                <asp:ValidationSummary runat="server" ID="ValidationSummary3" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="grid" ForeColor="red" />
                                                                                <asp:CustomValidator ID="CustomValidator1" ValidationGroup="sel" runat="server" ErrorMessage="Please select at least one record."
                                                                                    ClientValidationFunction="ValidateGrid" ForeColor="Red"></asp:CustomValidator>
                                                                                <asp:GridView ID="gvXMLSource" runat="server" Width="100%" DataKeyNames="Srno" AutoGenerateColumns="False" HeaderStyle-CssClass="visible-desktop" CssClass="table table-striped table-hover table-condensed table-mptt" GridLines="None"
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
                                                                                                <asp:Label ID="lblSourceURL" runat="server" CssClass="text-nowrap" Text='<%# Eval("URL") %>'></asp:Label>
                                                                                            </ItemTemplate>
                                                                                            <EditItemTemplate>
                                                                                                <asp:TextBox ID="txtSourceURL" runat="server" Text='<%# Eval("URL") %>'></asp:TextBox>
                                                                                                <asp:RequiredFieldValidator runat="server" ID="reqGridSourceUrl" ErrorMessage="Source Url is required!"
                                                                                                    Text="!" ForeColor="Red" ValidationGroup="grid" ControlToValidate="txtSourceURL"></asp:RequiredFieldValidator>
                                                                                            </EditItemTemplate>
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Source Type" ItemStyle-Width="120">
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
                                                                                            ShowDeleteButton="true" ItemStyle-Width="100">
                                                                                            <ItemStyle HorizontalAlign="Right" />

                                                                                        </asp:CommandField>
                                                                                        <asp:TemplateField HeaderText="" ItemStyle-Width="120">
                                                                                            <ItemTemplate>
                                                                                                <asp:HyperLink ID="hypChannel" runat="server" CssClass="btn btn-danger" ToolTip="No channels selected!" Width="120" NavigateUrl='<%# Eval("Srno", "ChannelSelection.aspx?id={0}") %>'>Select Channels</asp:HyperLink>
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>

                                                                                    </Columns>
                                                                                </asp:GridView>
                                                                                <div style="text-align: center;">
                                                                                    <asp:LinkButton ID="lnkClear" runat="server" CausesValidation="False" CssClass="btn-lg btn-info"
                                                                                        Text="Reset" OnClientClick="SelectAllCheckboxes(false);return false;">
                                                                                        <i class="glyphicon glyphicon-refresh"> Reset</i>
                                                                                    </asp:LinkButton>
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
                    <div class="left-nav actions active" id="left-nav">
                        <ul style="margin-left: 40px;">
                            <li>
                                <asp:HyperLink runat="server" CssClass="user-links" ID="hypHome" NavigateUrl="../Admin/Dashboard.aspx" Text="Home"></asp:HyperLink>
                            </li>
                            <li>
                                <a href="../Secured/index.aspx" class="bold info">Xml Generation</a>
                            </li>
                            <li>
                                <a href="../Secured/XmlHistory.aspx" class="user-links">View Output Xmls</a>
                            </li>
                            <li>
                                <a href="ChannelSelection.aspx" class="user-links">Channel Selection</a>
                            </li>
                        </ul>
                    </div>
                </div>

            </div>


            <!-- Sticky footer push -->
            <div id="push"></div>
        </div>
    </div>
     <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>

    <script type="text/javascript">
       
        function blink(elem, times, speed) {
             
            if (times > 0 || times < 0) {
                if ($(elem).hasClass("btn btn-default")) {
                    $(elem).removeClass("btn btn-default");
                    $(elem).addClass("btn btn-danger");
                } else {
                    $(elem).removeClass("btn btn-danger");
                    $(elem).addClass("btn btn-default");
                }
            }

            clearTimeout(function () { blink(elem, times, speed); });

            if (times > 0 || times < 0) {
                setTimeout(function () { blink(elem, times, speed); }, speed);
                times -= .5;
            }
        }
    </script>
</asp:Content>
