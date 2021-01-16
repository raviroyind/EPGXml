<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="accessDenied.aspx.cs" Inherits="XmlParser.accessDenied" %>
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
    <div class="container" style="margin-top: 5%;">
        <img src="Images/accessdenied.jpg" />
        <div class="alert-error h3">
            <i class="glyphicon glyphicon-info-sign"></i> You must be logged in to access the requested area.
        </div>
        <button name="btnClose" class="btn btn-warning btn-large" value="Close"onclick="javascript:window.close(); return false;">Close</button>
    </div>
</asp:Content>
