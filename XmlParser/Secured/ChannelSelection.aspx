<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="ChannelSelection.aspx.cs" Inherits="XmlParser.Secured.ChannelSelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../CSS/bootstrap.min.css" rel="stylesheet" />
    <link href="../CSS/bootstrap-theme.css" rel="stylesheet" />
    <link href="../CSS/duallist.css" rel="stylesheet" />
    <link href="../CSS/prettify.css" rel="stylesheet" />

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

                        <span style="color: #ffffff; font-size: 18px; padding-left: 10px;">Xml Generation</span>
                        <div class="pull-right" style="color: #ffffff; padding-right: 10px;">
                            <asp:Label ID="lblUser" runat="server"></asp:Label>
                            <i class="clearfix"></i>
                            <asp:HyperLink runat="server" ID="hypUser" NavigateUrl="../default.aspx?id=lg" Text="Logout"></asp:HyperLink>
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
                            <span class="divider"><span class="alert-info">» Filter Channels.</span></span>
                        </li>
                    </ul>
                    <!-- Content -->
                    <div id="content" class="row-fluid">
                        <ol class="breadcrumb">
                            <li><strong>Source Xml:</strong></li>
                            <li>
                                <asp:DropDownList runat="server" data-style="btn btn-default" CssClass="selectpicker" ID="ddlSourceXml" AutoPostBack="True" OnSelectedIndexChanged="ddlSourceXml_SelectedIndexChanged" /></li>
                        </ol>
                        
                        <div class="row">
                            <div class="col-xs-5">
                            </div>
                            <div class="col-xs-2">
                                 <asp:Label ID="lblMsg" runat="server" style="color: crimson; font-weight: bold; margin-left:10%;"></asp:Label>
                                <br/>
                            </div>
                            <div class="col-xs-5">
                                 
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-xs-5">
                                <div class="alert alert-info" role="alert">Inactive Channels</div>
                            </div>
                            <div class="col-xs-2">
                            </div>
                            <div class="col-xs-5">
                                <div class="alert alert-success" role="alert">Active Channels</div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-xs-5">
                                <asp:ListBox ID="undo_redo" runat="server" CssClass="form-control" Rows="13" SelectionMode="Multiple"></asp:ListBox>
                                <asp:HiddenField runat="server" ID="hidLeft" />
                            </div>

                            <div class="col-xs-2">

                                <button type="button" id="undo_redo_rightAll" runat="server" class="btn btn-default btn-block"><i class="glyphicon glyphicon-forward"></i></button>
                                <button type="button" id="undo_redo_rightSelected" runat="server" class="btn btn-default btn-block"><i class="glyphicon glyphicon-chevron-right"></i></button>
                                <button type="button" id="undo_redo_leftSelected" runat="server" class="btn btn-default btn-block"><i class="glyphicon glyphicon-chevron-left"></i></button>
                                <button type="button" id="undo_redo_leftAll" runat="server" class="btn btn-default btn-block"><i class="glyphicon glyphicon-backward"></i></button>

                            </div>

                            <div class="col-xs-5">
                                <asp:ListBox ID="undo_redo_to" runat="server" CssClass="form-control" Rows="13" SelectionMode="Multiple"></asp:ListBox>
                                <asp:HiddenField runat="server" ID="hidRight" />
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-xs-12"></div>
                        </div>
                        <div class="row">
                            <div class="col-xs-5"></div>
                            <div class="col-xs-2 right">

                                <asp:LinkButton runat="server" CssClass="btn btn-info btn-block" OnClick="OnClick" Text="Submit">
                                    <i class="glyphicon glyphicon-ok warning"></i> Submit
                                </asp:LinkButton>
                                <asp:HyperLink runat="server" CssClass="btn btn-warning btn-block" Text="Back" NavigateUrl="index.aspx">
                                    <i class="glyphicon glyphicon-backward"></i> Back
                                </asp:HyperLink>

                            </div>
                            <div class="col-xs-5"></div>
                        </div>
                        <div class="row">
                            <div class="col-xs-12">
                            </div>
                        </div>
                        <div class="clear-fix">
                            <br />
                        </div>
                        <div class="row">
                            <div class="col-xs-4">
                            </div>
                            <div class="col-xs-4">
                                <div class="alert" role="alert">
                                    Changes made will take effect after clicking on "Submit" button. 
                                </div>
                            </div>
                            <div class="col-xs-4">
                            </div>
                        </div>


                    </div>
                    <!-- END Content -->
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
        </div>
        <!-- Sticky footer push -->
        <div id="push"></div>
        <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
        <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>
        <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/prettify/r298/prettify.min.js"></script>
        <script src="../CSS/multiselect.min.js"></script>
        <script type="text/javascript">
            jQuery(document).ready(function ($) {
                $("#FeaturedContent_undo_redo").multiselect({
                    search: {
                        left: "<input type=\"text\" name=\"q\" class=\"form-control\" placeholder=\"Search...\" />",
                        right: "<input type=\"text\" name=\"q\" class=\"form-control\" placeholder=\"Search...\" />"
                    },
                    moveToRight: function (multiselect, options, event, silent, skipStack) {

                        document.getElementById("<%=hidLeft.ClientID%>").value = '';

                    for (var i = 0; i < options.length; i++) {
                        if (document.getElementById("<%=hidRight.ClientID%>").value.indexOf(options[i].value) == -1)
                                document.getElementById("<%=hidRight.ClientID%>").value += options[i].value + ",";
                        }

                    var button = $(event.currentTarget).attr('id');

                    if (button == 'FeaturedContent_undo_redo_rightSelected') {
                        var left_options = multiselect.left.find('option:selected');
                        multiselect.right.eq(0).append(left_options);

                        if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                            multiselect.right.eq(0).find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.right.eq(0));
                        }
                    } else if (button == 'FeaturedContent_undo_redo_rightAll') {
                        var left_options = multiselect.left.find('option');
                        multiselect.right.eq(0).append(left_options);

                        if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                            multiselect.right.eq(0).find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.right.eq(0));
                        }
                    } else if (button == 'FeaturedContent_undo_redo_rightSelected_2') {
                        var left_options = multiselect.left.find('option:selected');
                        multiselect.right.eq(1).append(left_options);

                        if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                            multiselect.right.eq(1).find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.right.eq(1));
                        }
                    } else if (button == 'FeaturedContent_undo_redo_rightAll_2') {
                        var left_options = multiselect.left.find('option');
                        multiselect.right.eq(1).append(left_options);

                        if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                            multiselect.right.eq(1).eq(1).find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.right.eq(1));
                        }
                    }
                },


                moveToLeft: function (multiselect, options, event, silent, skipStack) {

                    document.getElementById("<%=hidRight.ClientID%>").value = '';

                        for (var i = 0; i < options.length; i++) {
                            if (document.getElementById("<%=hidLeft.ClientID%>").value.indexOf(options[i].value) == -1)
                                document.getElementById("<%=hidLeft.ClientID%>").value += options[i].value + ",";
                        }

                        var button = $(event.currentTarget).attr('id');
                        if (button == 'FeaturedContent_undo_redo_leftAll') {
                            var right_options = multiselect.right.eq(0).find('option');
                            multiselect.left.append(right_options);

                            if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                                multiselect.left.find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.left);
                            }
                        } else if (button == 'FeaturedContent_undo_redo_leftSelected') {
                            var right_options = multiselect.right.eq(0).find('option:selected');
                            multiselect.left.append(right_options);

                            if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                                multiselect.left.find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.left);
                            }
                        } else if (button == 'FeaturedContent_leftSelected_2') {
                            var right_options = multiselect.right.eq(1).find('option:selected');
                            multiselect.left.append(right_options);

                            if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                                multiselect.left.find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.left);
                            }
                        } else if (button == 'FeaturedContent_leftAll_2') {
                            var right_options = multiselect.right.eq(1).find('option');
                            multiselect.left.append(right_options);

                            if (typeof multiselect.callbacks.sort == 'function' && !silent) {
                                multiselect.left.find('option').sort(multiselect.callbacks.sort).appendTo(multiselect.left);
                            }
                        }
                    }

            });
        });

        </script>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
