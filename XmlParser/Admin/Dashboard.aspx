<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="XmlParser.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
                    <i class="icon-home"></i>
                    <asp:Label ID="lblUser" runat="server" class="btn-high bold"></asp:Label>
                    <i class="icon-lock" style="color: #ffffff;"></i>
                    <asp:HyperLink runat="server" ID="hypUser" NavigateUrl="../default.aspx?id=lg" Text="Logout"></asp:HyperLink>
                </div>
            </div>

            <!-- END Header -->


            <div class="suit-columns two-columns">
                <div id="suit-center" class="suit-column">
                    <ul class="breadcrumb">
                        <li>
                            <a href="#"></a>
                            <span class="divider"><span class="alert-info">» User Management.</span></span>
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

                                                    <div class="two-columns">
                                                        <div class="left-column"></div>
                                                        <div class="right-column">
                                                            <asp:Label runat="server" ID="lblMsg" CssClass="alert-danger"></asp:Label>
                                                            <asp:ValidationSummary runat="server" ID="valSum" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="add" ForeColor="red" />
                                                            <table class="table table-striped table-bordered table-hover table-condensed table-mptt" style="border-collapse: collapse">
                                                                <tr>
                                                                    <td colspan="6">
                                                                        <i class="add-row"></i>Add New User
                                                                    </td>
                                                                </tr>
                                                                <tr nowrap="nowrap">
                                                                    <td nowrap="nowrap">
                                                                        <span>User Id:</span><asp:TextBox ID="txtName" runat="server" Width="140" />
                                                                        <asp:RequiredFieldValidator runat="server" ID="reqUserId" ErrorMessage="User Id is required!" Text="!" ForeColor="Red" ValidationGroup="add" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <span>Email Id:</span>
                                                                        <asp:TextBox ID="txtUserEmailAddress" runat="server" Width="140" />
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <span>Password:</span>
                                                                        <asp:TextBox ID="txtPassword" runat="server" Width="140" />
                                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ErrorMessage="Password Id is required!" Text="!" ForeColor="Red" ValidationGroup="add" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <span>Confirm:</span>
                                                                        <asp:TextBox ID="txtPasswordConfirm" runat="server" Width="140" />
                                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ErrorMessage="Confirm Password Id is required!" Text="!" ForeColor="Red" ValidationGroup="add" ControlToValidate="txtPasswordConfirm"></asp:RequiredFieldValidator>
                                                                        <asp:CompareValidator runat="server" ID="compVal" ControlToValidate="txtPassword" ValidationGroup="add" ControlToCompare="txtPasswordConfirm" Type="String"
                                                                            ErrorMessage="Password do not match" Text="!" ForeColor="Red"></asp:CompareValidator>
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <span>User Type:</span>
                                                                        <asp:DropDownList runat="server" CssClass="dropdown" ID="ddlUserType" Width="80px">
                                                                            <asp:ListItem Selected="True" Value="S">Select</asp:ListItem>
                                                                            <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                                                            <asp:ListItem Value="User">User</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ErrorMessage="User type is required!" Text="!" ForeColor="Red" ValidationGroup="add" InitialValue="S" ControlToValidate="ddlUserType"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="btnAdd" runat="server" ValidationGroup="add" CausesValidation="True" CssClass="btn-small btn-primary" Text="+ Add" OnClick="Insert" />
                                                                    </td>
                                                                </tr>
                                                            </table>

                                                            <div id="grid" runat="server">
                                                                <asp:ValidationSummary runat="server" ID="ValidationSummaryGrid" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" ValidationGroup="grv" ForeColor="red" />
                                                                <asp:GridView ID="gvUsers" runat="server" Width="100%" DataKeyNames="UserName" AutoGenerateColumns="False" HeaderStyle-CssClass="visible-desktop" CssClass="table table-striped table-bordered table-hover table-condensed table-mptt" GridLines="None"
                                                                    CellPadding="0" border="0" AllowPaging="False" EmptyDataText="No records has been added."
                                                                    OnRowDataBound="gvUsers_OnRowDataBound" OnRowEditing="gvUsers_OnRowEditing" OnRowCancelingEdit="gvUsers_OnRowCancelingEdit"
                                                                    OnRowUpdating="gvUsers_OnRowUpdating" OnRowDeleting="gvUsers_OnRowDeleting">
                                                                    <AlternatingRowStyle CssClass="row2" />
                                                                    <RowStyle CssClass="row1" />
                                                                    <HeaderStyle CssClass="sortable" />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="User Id" ItemStyle-Width="150">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox ID="txtName" ReadOnly="True" Enabled="False" runat="server" Text='<%# Eval("UserName") %>'></asp:TextBox>
                                                                                <asp:RequiredFieldValidator runat="server" ID="reqUserId" ErrorMessage="User Id is required!" Text="!" ForeColor="Red" ValidationGroup="grv" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                                            </EditItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="EmailId" ItemStyle-Width="150">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUserEmailAddress" runat="server" Text='<%# Eval("UserEmailAddress") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox ID="txtUserEmailAddress" runat="server" Text='<%# Eval("UserEmailAddress") %>'></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Password" ItemStyle-Width="150">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblPassword" runat="server" Text='<%# Eval("Password") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox ID="txtPassword" runat="server" Text='<%# Eval("Password") %>'></asp:TextBox>
                                                                                <asp:RequiredFieldValidator runat="server" ID="reqGrvgrvPass" ErrorMessage="Password Id is required!" Text="!" ForeColor="Red" ValidationGroup="grv" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                                                                            </EditItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="User Type" ItemStyle-Width="150">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUserType" runat="server" Text='<%# Eval("UserType") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <EditItemTemplate>
                                                                                <asp:DropDownList runat="server" CssClass="dropdown" ID="ddlUserType" Width="80px">
                                                                                    <asp:ListItem Selected="True" Value="S">Select</asp:ListItem>
                                                                                    <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                                                                    <asp:ListItem Value="User">User</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                <asp:RequiredFieldValidator runat="server" ID="reqGrUserType" ErrorMessage="User type is required!" Text="!" ForeColor="Red" ValidationGroup="grv" InitialValue="S" ControlToValidate="ddlUserType"></asp:RequiredFieldValidator>
                                                                            </EditItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:CommandField ButtonType="Image" EditImageUrl="../CSS/edit.png" DeleteImageUrl="../CSS/delete.png" DeleteText="Delete" EditText="Edit" ShowEditButton="true" CancelImageUrl="../CSS/cancel.png" CancelText="Cancel" UpdateImageUrl="../CSS/Save.png" ValidationGroup="grv" CausesValidation="True" UpdateText="Save" ShowDeleteButton="true" ItemStyle-Width="150" />
                                                                    </Columns>
                                                                </asp:GridView>

                                                            </div>
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
                </div>
                <div id="suit-left" class="suit-column">
                    <div class="left-nav" id="left-nav" style="margin-left: 15px;">
                        <ul>
                            <li class="alert-info">
                                <a href="../Secured/index.aspx" class="actions">Xml Generation</a>
                            </li>
                            <li class="alert-info">
                                <a href="../Secured/XmlHistory.aspx" class="user-links">View Output Xmls</a>
                            </li>
                        </ul>
                    </div>
                </div>

                <!-- Sticky footer push -->
                <div id="push"></div>
            </div>
        </div>
    </div>
</asp:Content>

