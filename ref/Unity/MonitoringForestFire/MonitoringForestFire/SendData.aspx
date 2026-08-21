<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendData.aspx.cs" Inherits="MonitoringForestFire.SendData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Post Test</title>
    <script type="text/javascript" src="js/jquery-1.11.3.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript" src="js/jquery.maskedinput.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnSubmit").click(CallWebService);
            $("#txtActTime").mask("9999-99-99 99:99:99");
        });

        function CallWebService() {

            var JobID = $("#txtJobID")[0].value;
            var ActionID = $("#txtActionID")[0].value;
            var ActTime = $("#txtActTime")[0].value;
            var Description = $("#txtDescription")[0].value;

            var targetMethod = "";

            if ($("#rdoStart")[0].checked == true) {
                targetMethod = "StartAction";
            }
            else {
                targetMethod = "EndAction";
            }

            var params = JSON.stringify({ nJobID: JobID, nActionID: ActionID, dtActTime: ActTime, strDescription: Description });

            $.ajax({
                type: 'POST',
                url: 'http://unes.iptime.org:19050/WS/MonitoringForestFireWS.asmx/' + targetMethod,
                data: params,
                contentType: 'application/json;charset=UTF-8',
                dataType: 'json',
                async: false,
                success: function (msg) {
                    SuccessFunc(msg);
                },
                error: function (msg) {
                    FailFunc(msg);
                }
            });
        }


        function SuccessFunc(msg) {
            alert("Success : " + msg.d);
        }

        function FailFunc(msg) {
            alert(msg.status + " : " + msg.statusText);   
        }

    </script>
</head>
<body>
    <form id="fromPostData" runat="server">
    <div>
        <table border="0">
            <tr>
                <td style="text-align: right;width: 100px;">Job ID : </td>
                <td><input id="txtJobID" type="text" /></td>
            </tr>
            <tr>
                <td style="text-align: right">Act ID : </td>
                <td><input id="txtActionID" type="text" /></td>
            </tr>
            <tr>
                <td style="text-align: right">Date : </td>
                <td><input id="txtActTime" type="text" /></td>
            </tr>
            <tr>
                <td style="text-align: right">JDesc : </td>
                <td><input id="txtDescription" type="text" /></td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;">
                    <input id="rdoStart" name="BeginEnd" type="radio" value="true" checked="checked" /> Begin
                    &nbsp;&nbsp;&nbsp;
                    <input id="rdoEnd" name="BeginEnd" type="radio" value="false "/>End
                </td>
            </tr>
        </table>
        <input id="btnSubmit" type="button" value="Call WebService" runat="server" />
    </div>
    </form>
</body>
</html>
