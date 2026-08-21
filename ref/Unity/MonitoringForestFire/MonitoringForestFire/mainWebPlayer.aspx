<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mainWebPlayer.aspx.cs" Inherits="MonitoringForestFire.mainWebControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Monitoring Forest Fires</title>
    <link rel="stylesheet" type="text/css" href="css/loading.css" />
    <style>
        h1 {
            font-family: Helvetica, Verdana, Arial, sans-serif;
        }
        table {
            padding: 0px;
            border-spacing: 0px;
            margin-left:auto;
            margin-right:auto;
        }
        th, td {
            padding: 0px;
            border-spacing: 0px;
            margin: 0px 0px 0px 0px;
        }
        html, body {
            height: 100%;
            margin:0px 0px 0px 0px;
            align-content:center;
            vertical-align: middle;
            overflow:hidden;
        }
        iframe {
            padding: 0px;
            margin: 0px 0px 0px 0px auto;
            height: 100%;
            width: 100%;
            border: none;
        }
        div.divIframe {
            padding: 0px;
            margin: 0px 0px 0px 0px auto;
        }
    </style>
    <script type="text/javascript" src="js/jquery-1.11.3.js"></script>
    <script type="text/javascript">

        var isLoadedFlowTimeLine = false;
        var isLoadedMonitoringDrone = false;

        function LoadedFlowTimeLine() {
            isLoadedFlowTimeLine = true;

            LoadedUnity();
        }


        function LoadedMonitoringDrone() {
            isLoadedMonitoringDrone = true;

            LoadedUnity();
        }


        function LoadedUnity() {
            if (isLoadedFlowTimeLine == true
                && isLoadedMonitoringDrone == true) {
                EndProgress();
            }
        }


        function EndProgress() {
            $("#backGround").fadeOut(1500);
        }


        function SendMessage(txt) {
            document.getElementById("RightFrame").contentWindow.SendMessage("ScrollView", "AddFlowUserData", txt);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 0 auto;text-align:center;">
            <h1>
                Monitoring Forest Fires
            </h1>

            <div style="margin-left:auto;margin-right:auto;width:1330px;height:810px;" >
                <div class="divIframe" style="float:left;height:810px;width:710px;">
                    <iframe id="LeftFrame" src="MonitoringDrone/WebPlayer/MonitoringDrone.aspx" border="0" scrolling="no"></iframe>
                </div>
                <div class="divIframe" style="float:left;height:810px;width:610px;">
                    <iframe id="RightFrame" src="FlowTimeLine/WebPlayer/FlowTimeLine.aspx" border="0" scrolling="no"></iframe>
                </div>
            </div>

            <input id="btnCallUnity" type="button" value="Call Unity" style="visibility:hidden;" width="100" onclick="javascript: SendMessage('Call Unity');" />
        </div>
        
        <div id="backGround" style="position:absolute;top:0px;left:0px;z-index:999;">
            <div id="fountainG">
	            <div id="fountainG_1" class="fountainG"></div>
	            <div id="fountainG_2" class="fountainG"></div>
	            <div id="fountainG_3" class="fountainG"></div>
	            <div id="fountainG_4" class="fountainG"></div>
	            <div id="fountainG_5" class="fountainG"></div>
	            <div id="fountainG_6" class="fountainG"></div>
	            <div id="fountainG_7" class="fountainG"></div>
	            <div id="fountainG_8" class="fountainG"></div>
            </div>
        </div>

    </form>
</body>
</html>
