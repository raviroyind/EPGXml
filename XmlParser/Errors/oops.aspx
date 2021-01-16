<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="oops.aspx.cs" Inherits="XmlParser.Errors.oops" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
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
    <div class="container">
        <img src="../Images/errimg.png" /><span class="bold">Something went wrong.</span>
        <div class="alert-error h3">
            <i class="glyphicon glyphicon-info-sign"></i>Unfortunately we're having trouble loading the page you are looking for.
            An email has already been despatched to technical staff containing details of the issue.
        </div>
        <div id="errorMsg" runat="server" class="alert alert-dismissable alert-warning"></div>
        <button name="btnClose" class="btn btn-warning btn-large" value="Close"onclick="javascript:window.close(); return false;">Close</button>
    </div>
 </asp:Content>
