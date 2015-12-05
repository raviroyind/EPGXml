<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="XmlParser._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function clearForm() {
            document.getElementById('<%=txtUserId.ClientID %>').value = '';
            document.getElementById('<%=txtPassword.ClientID %>').value = '';
            document.getElementById('<%=divMsg.ClientID %>').style.display = 'none';
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
                    <span class="user-links"></span>
                </div>
            </div>

            <!-- END Header -->

            <div class="container" style="width: 40%; text-align: center; margin-top: 15%;">
                <div class="well" style="background-color: #5295b0;">
                    <h2 style="color: #ffffff;">Login</h2>
                </div>
                <div class="well" style="padding-top: 0; text-align: center;">
                    <br />
                    <div class="form-horizontal">

                        <div class="row">
                            <div class="col-xs-12">
                                <asp:ValidationSummary runat="server" ID="valSum" ForeColor="Red" DisplayMode="List" ShowSummary="True" ShowMessageBox="False" />
                                <asp:Label runat="server" ID="lblMsg" CssClass="alert-danger"></asp:Label>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-xs-4"></div>
                            <div class="col-xs-4">
                                <asp:TextBox ID="txtUserId" placeholder="User Name" CssClass="form-control" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ID="reqId" ControlToValidate="txtUserId" ErrorMessage="User Id is required!" Text="!"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-xs-4"></div>
                        </div>
                        <div class="row">
                            <div class="col-xs-4"></div>
                            <div class="col-xs-4">
                                <asp:TextBox ID="txtPassword" placeholder="Password" CssClass="form-control" TextMode="Password" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ID="reqPass" ControlToValidate="txtPassword" ErrorMessage="Password is required!" Text="!"></asp:RequiredFieldValidator>

                            </div>
                            <div class="col-xs-4"></div>
                        </div>

                        <div class="row">
                            <div class="col-xs-4"></div>
                            <div class="col-xs-4">
                                <asp:LinkButton runat="server" ID="btnLogin" CausesValidation="True" class="btn btn-info" OnClick="btnLogin_Click">Logon</asp:LinkButton>
                                <asp:LinkButton runat="server" ID="btnCancel" OnClientClick="clearForm();return false;" class="btn btn-info">Cancel</asp:LinkButton>

                            </div>
                            <div class="col-xs-4"></div>
                        </div>
                        <div class="row">
                            <div class="col-xs-12">
                                <div style="display: none;" id="divMsg" runat="server"><span style="color: red">Invalid credentials. Please try again!</span></div>
                                <br/>
                                <div class="alert alert-info">
                                    Un-authorized use is prohibited.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                </div>

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
                            </div>
                        </div>
                    </div>
                    <!-- END Content -->
                </div>

                <div id="suit-left" class="suit-column">
                    <div class="left-nav" id="left-nav" style="margin-left: 15px;">
                    </div>
                </div>
            </div>
        </div>
        <!-- Sticky footer push -->
        <div id="push"></div>
    </div>
</asp:Content>
