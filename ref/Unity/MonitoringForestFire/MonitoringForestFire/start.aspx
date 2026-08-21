<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="start.aspx.cs" Inherits="MonitoringForestFire.start" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Monitoring Forest Fires</title>
    <style>
        html, body {
            height: 100%;
            width: 100%;
            overflow:hidden;
            margin:0px 0px 0px 0px;
            align-content:center;
            vertical-align: middle;
        }
    </style>
    
    <script type="text/javascript">
        function LoadPage() {
            var agent = navigator.userAgent.toLowerCase();

            if (agent.indexOf("safari") != -1 || agent.indexOf("firefox") != -1 || agent.indexOf("chrome") != -1) {
                location.replace("http://unes.iptime.org:19050/mainWebGL.aspx");
            }
            else {
                location.replace("http://unes.iptime.org:19050/mainWebPlayer.aspx");
            }
        }
    </script>
</head>
<body onload="javascript:LoadPage();">
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>

