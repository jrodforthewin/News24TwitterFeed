<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="News24TwitterFeed._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        
    </style>
    <div class="jumbotron">
        <h1>View Twitter Feed</h1>
    </div>
    <div class="contaier">
        <div class="row">
            <div class="col-md-1">
                <asp:Button runat="server" data-title="btnGetTwitterFeed" ID="btnGetTwitterFeed" class="btn btn-success" OnClick="btnGetTwitterFeed_OnClick" Text="Get Twitter Feed"></asp:Button>
            </div>

        </div>
        <br />
        <div class="row">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h3 class="panel-title">Latest twitter feeds from News24 <span class="glyphicon glyphicon-refresh glyphicon-spin hidden"></span></h3>
                </div>
                <div class="panel-body">
                    <% foreach (TwitterFeed tw in lTwitterFeeds)
                        {
                    %>

                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="row">
                                <%= tw.Text %>
                            </div>
                            <div class="row">
                                <span class="label label-danger">Favourite Count: <%= tw.FavouriteCount %></span>
                                <span class="label label-info">Tweet Count: <%= tw.FavouriteCount %></span>
                            </div>
                        </div>
                    </div>

                    <%

                        }
                    %>
                </div>
            </div>
        </div>
        <div class="row" id="divTwitter"></div>
    </div>
    <script type="text/javascript">

        function DisableButton() {
            $("input[data-title='btnGetTwitterFeed'").attr("disabled", true);
            $("span").removeClass("hidden").show();
            $(".panel-body").text("");

        }
        window.onbeforeunload = DisableButton;

    </script>
</asp:Content>
