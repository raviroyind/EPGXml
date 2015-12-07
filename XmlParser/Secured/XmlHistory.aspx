<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="XmlHistory.aspx.cs" Inherits="XmlParser.Secured.XmlHistory" %>

<%@ Register Src="../Controls/Paging.ascx" TagName="Paging" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="https://code.jquery.com/ui/1.11.4/jquery-ui.min.js"></script>
    <link href="../CSS/bootstrap.min.css" rel="stylesheet" />
    <link href="../CSS/jquery-ui.css" rel="stylesheet" />
    <script type="text/javascript">

        function SelectAllCheckboxesSpecific(spanChk) {
            var isChecked = spanChk.checked;
            var chk = spanChk;
            Parent = document.getElementById('<%=gvFiles.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != chk && items[i].type == "checkbox") {
                    if (items[i].checked != isChecked) {
                        items[i].click();
                    }
                }
            }
        }
        function ValidateGrid(sender, args) {
            var gridView = document.getElementById("<%=gvFiles.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    args.IsValid = true;
                    return;
                }
            }
            args.IsValid = false;
        }

        $(function () {
            $(".date-input").datepicker();
        });

        function SetPagenoValue(txt1, txt2) {
            return AllowKeyPress(event, document.getElementById(txt1), document.getElementById(txt2));
        }


        function ShowFilter(contl) {

            var obj = document.getElementById(contl);

            if (obj.style.display == "block")
                obj.style.display = "none";
            else
                obj.style.display = "block";
        }

        $(document.ready()(function () {
            $("chklstRptKeys").multiselect().multiselectfilter();
        }));
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
    <!-- Sticky footer wrap -->
    <div class="container-fluid">
        <!-- Container -->

        <div class="container-fluid">
            <!-- Header -->
            <header>
                <nav class="navbar navbar-inverse" style="height: 45px; width: 100%; padding: 0 0 0 0; background-color: #2f3334;">
                    <div id="branding" style="width: 100%;">

                        <span style="color: #ffffff; font-size: 18px; padding-left: 10px; border-right: 1px solid #2f3334; padding-top: 10px;">Xml Generation</span>
                        <div class="pull-right" style="color: #ffffff; padding-right: 10px;">
                            <i class="glyphicon glyphicon-user"></i><asp:Label ID="lblUser" runat="server"></asp:Label><br/>
                            <div style="text-align: right;"> 
                            <i class="glyphicon glyphicon-log-out"></i>
                            <asp:HyperLink runat="server" ID="hypUser" NavigateUrl="../default.aspx?id=lg" Text="Logout"></asp:HyperLink>
                                </div>
                        </div>
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
                </nav>
            </header>
            <!-- END Header -->

            <div class="suit-columns two-columns">
                <div id="suit-center" class="suit-column">
                    <ul class="breadcrumb">
                        <li>
                            <a href="#"></a>
                            <span class="divider"><span class="alert-info">» Generated Xmls.</span></span>
                        </li>
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
                                        <div>
                                            <table border="0" style="padding: 0px 5px 0px 5px; width: 100%;">
                                                <tr>
                                                    <td>
                                                        <div class="panel panel-default">
                                                            <div class="panel-heading">
                                                                <span>Search</span>
                                                            </div>
                                                            <div class="panel-body">
                                                                <div class="row">
                                                                    <div class="col-lg-1">
                                                                        <label style="padding-top: 8px;">Channel Name:</label>
                                                                    </div>
                                                                    <div class="col-lg-1">
                                                                        <asp:TextBox runat="server" ID="txtChannelName" CssClass="input-xlarge" Height="35" Style="font-weight: bold;"></asp:TextBox>
                                                                    </div>
                                                                  
                                                                    <div class="col-lg-3">
                                                                        <label style="padding-left: 20px; padding-top: 8px; text-align: right; margin-left: 60%;">Import Date:</label>
                                                                    </div>

                                                                    <div class="col-md-2">
                                                                        <span class="text-lowercase"></span>
                                                                        <span class="btn btn-default" style="vertical-align: top;">
                                                                            <span class="glyphicon glyphicon-calendar"></span>
                                                                        </span>
                                                                        <asp:TextBox runat="server" ID="txtImportDtFrom" placeholder="from" Height="35" Style="font-weight: bold;" CssClass="form-control date-input" Width="120"></asp:TextBox>
                                                                    </div>

                                                                    <div class="col-md-2">
                                                                        <span class="text-lowercase"></span>
                                                                        <span class="btn btn-default" style="vertical-align: top;">
                                                                            <span class="glyphicon glyphicon-calendar"></span>
                                                                        </span>
                                                                        <asp:TextBox runat="server" ID="txtImportDtTo" placeholder="to" Height="35" Style="font-weight: bold;" CssClass="form-control date-input" Width="120"></asp:TextBox>
                                                                    </div>

                                                                    <div class="col-md-1 text-nowrap">
                                                                        <asp:LinkButton ID="lnkBtnSearch"
                                                                            runat="server" CssClass="btn btn-default" OnClick="lnkBtnSearch_OnClick"><span aria-hidden="true" class="glyphicon glyphicon-search"></span> Search
                                                                        </asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkBtnResetSearch"
                                                                            runat="server" CssClass="btn btn-info" OnClick="lnkBtnResetSearch_Click"><span aria-hidden="true" class="glyphicon glyphicon-refresh"></span> Reset
                                                                        </asp:LinkButton>
                                                                    </div>
                                                                    
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="hdnRptKeysFilter" runat="server" />
                                                        <uc:Paging ID="ucPaging" runat="server" Align="right" PageSize="50" OnNavigator_Click="ImgbtnNavigator_Click" ShowNoOfRecordsDropDown="True"
                                                            OnNoOfRecords_SelectedIndexChanged="ddlNoOfRecords_IndexChanged"
                                                            OnPageNo_Changed="txtPageNo_Changed" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div>
                                                            <div>
                                                                <asp:LinkButton ID="lnkBtnDownload"
                                                                    runat="server" CssClass="btn btn-info" CausesValidation="True" ValidationGroup="grid" OnClick="lnkBtnDownload_OnClick"><span aria-hidden="true" class="glyphicon glyphicon-download-alt"></span> Download
                                                                </asp:LinkButton>
                                                                <asp:CustomValidator ID="CustomValidator1" ValidationGroup="grid" runat="server" ErrorMessage="Please select at least one file to download."
                                                                    ClientValidationFunction="ValidateGrid" ForeColor="Red"></asp:CustomValidator>
                                                            </div>
                                                            <span class="alert-info">(Click on a column name to sort by.it.)</span>
                                                            <asp:GridView ID="gvFiles" runat="server" Width="100%" AutoGenerateColumns="False" HeaderStyle-CssClass="visible-desktop"
                                                                CssClass="table table-striped table-bordered table-hover table-condensed" GridLines="None" DataKeyNames="URL" EmptyDataText="No records found."
                                                                CellPadding="0" border="0" OnSorting="gvFiles_Sorting" OnRowDataBound="gvFiles_RowDataBound">
                                                                <AlternatingRowStyle CssClass="row2" />
                                                                <RowStyle CssClass="row1" />
                                                                <HeaderStyle CssClass="sortable" />
                                                                <EmptyDataTemplate>
                                                                    <span class="alert-info">No records found.</span>
                                                                </EmptyDataTemplate>
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="40">
                                                                        <HeaderTemplate>
                                                                            <asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecific(this);" runat="server" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>



                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:LinkButton ID="lnkbtnSourceURL" runat="server" Text="Source" CommandArgument="SourceURL"
                                                                                OnClick="gvFiles_Sorting" CssClass="headerCol" ForeColor="#000000" Width="100px"></asp:LinkButton>
                                                                            <asp:ImageButton ID="btnSort_SourceURL" runat="server" OnClick="gvFilesImg_Sorting"
                                                                                CommandArgument="SourceURL" ImageUrl="../Images/but/sort03.gif" />
                                                                            <asp:ImageButton ID="imgbtnFilter" runat="server" AlternateText="Filter data" ImageUrl="~/Images/but/filter.png" Height="16px" /><br />
                                                                            <div id="divProductNameFilter" runat="server" style="display: none; z-index: 101; position: absolute; width: 150px;">
                                                                                <table border="0" cellpadding="0" class="" cellspacing="0">
                                                                                    <tr class="header-row">
                                                                                        <td align="left" style="border: none;">
                                                                                            <asp:LinkButton ID="imgbtnRptRemoveFilter" runat="server" Text="Clear" CommandArgument="SourceURL"
                                                                                                OnClick="imgbtnRemoveFilter_OnClick"></asp:LinkButton>
                                                                                        </td>
                                                                                        <td align="right" style="border: none;">
                                                                                            <asp:LinkButton ID="imgbtnRptFilter" runat="server" Text="Filter" CommandArgument="SourceURL"
                                                                                                OnClick="imgbtnFilter_OnClick"></asp:LinkButton>
                                                                                        </td>
                                                                                        <td align="right" valign="top" style="border: none; width: 10%">
                                                                                            <asp:ImageButton ID="imgbtnCloseFilter" runat="server" ToolTip="Close" ImageUrl="../Images/but/001.gif" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="3" valign="top" class="search-container">
                                                                                            <div id="divChklstRpt" runat="server" style="overflow: auto; border: solid 1px #d1d1d1; background-color: White; padding: 0;">
                                                                                                <asp:CheckBoxList ID="chklstRptKeys" runat="server" CssClass="text-nowrap">
                                                                                                </asp:CheckBoxList>
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>

                                                                            </div>
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink runat="server" ID="hypSourceUrl" Target="_blank" Text='<%# Eval("SourceURL").ToString().Substring(Eval("SourceURL").ToString().LastIndexOf("/", System.StringComparison.Ordinal)+1) %>'
                                                                                NavigateUrl='<%# Eval("SourceURL") %>'></asp:HyperLink>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>



                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:LinkButton ID="lnkbtnXmlFileName" runat="server" CommandArgument="XmlFileName" OnClick="gvFiles_Sorting"
                                                                                Text="Channel Name" CssClass="header-column" ForeColor="#000000" Width="110px"></asp:LinkButton>
                                                                            <asp:ImageButton ID="btnSort_XmlFileName" runat="server" OnClick="gvFilesImg_Sorting"
                                                                                CommandArgument="XmlFileName" ImageUrl="../Images/but/sort03.gif" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <%#Eval("XmlFileName").ToString().Length > 4?Eval("XmlFileName").ToString().Substring(0,Eval("XmlFileName").ToString().Length-4):Eval("XmlFileName").ToString() %>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:LinkButton ID="lnkbtnEpgStartDt" runat="server" CommandArgument="EpgStartDt" OnClick="gvFiles_Sorting"
                                                                                Text="Epg Start Date" CssClass="headerCol" ForeColor="#000000" Width="115px"></asp:LinkButton>
                                                                            <asp:ImageButton ID="btnSort_EpgStartDt" runat="server" OnClick="gvFilesImg_Sorting"
                                                                                CommandArgument="EpgStartDt" ImageUrl="../Images/but/sort03.gif" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <%# string.IsNullOrEmpty(Eval("EpgStartDt","{0:dd/MM/yyyy}"))?"": Convert.ToDateTime(Eval("EpgStartDt")).ToString("M/dd/yyyy") %>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:LinkButton ID="lnkbtnEpgEndDt" runat="server" CommandArgument="EpgEndDt" OnClick="gvFiles_Sorting"
                                                                                Text="Epg End Date" CssClass="headerCol" ForeColor="#000000" Width="90px"></asp:LinkButton>
                                                                            <asp:ImageButton ID="btnSort_EpgEndDt" runat="server" OnClick="gvFilesImg_Sorting"
                                                                                CommandArgument="EpgEndDt" ImageUrl="../Images/but/sort03.gif" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <%# string.IsNullOrEmpty(Eval("EpgEndDt","{0:dd/MM/yyyy}"))?"": Convert.ToDateTime(Eval("EpgEndDt")).ToString("MM/dd/yy") %>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:LinkButton ID="lnkbtnImportDate" runat="server" CommandArgument="ImportDate" OnClick="gvFiles_Sorting"
                                                                                Text="Import Date" CssClass="headerCol" ForeColor="#000000" Width="90px"></asp:LinkButton>
                                                                            <asp:ImageButton ID="btnSort_ImportDate" runat="server" OnClick="gvFilesImg_Sorting"
                                                                                CommandArgument="ImportDate" ImageUrl="../Images/but/sort03.gif" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <%# string.IsNullOrEmpty(Eval("ImportDate","{0:dd/MM/yyyy}"))?"": Convert.ToDateTime(Eval("ImportDate")).ToString("M/dd/yyyy") %>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="hypView" Target="_blank" runat="server" Text="View Xml" NavigateUrl='<%# Eval("URL") %>'></asp:HyperLink>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Additional Xml">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="hypView2" Target="_blank" runat="server" Text="View Xml" NavigateUrl='<%# Eval("URL2") %>'></asp:HyperLink>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>

                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <uc:Paging ID="ucPaging1" runat="server" Align="right" ShowNoOfRecordsDropDown="true" Visible="False"
                                                            OnNavigator_Click="ImgbtnNavigator1_Click" OnPageNo_Changed="txtPageNo1_Changed" OnNoOfRecords_SelectedIndexChanged="ddlNoOfRecords_IndexChanged" />

                                                    </td>
                                                </tr>

                                            </table>
                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>


                    </div>
                    <!-- END Content -->
                </div>

                <div id="suit-left" class="suit-column">
                    <div class="left-nav actions active" id="left-nav">
                        <ul style="margin-left: 40px;">
                            <li>
                                <asp:HyperLink runat="server" CssClass="user-links" ID="hypHomeLink" NavigateUrl="../Admin/Dashboard.aspx" Text="Home"></asp:HyperLink>
                            </li>
                            <li>
                                <a href="index.aspx" class="user-links">Xml Generation</a>
                            </li>
                            <li>
                                <a href="XmlHistory.aspx" class="bold info">View Output Xmls</a>
                            </li>
                            <li>
                                <a href="ChannelSelection.aspx" class="user-links">Channel Selection</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!-- Sticky footer push -->
        <div id="push"></div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>

