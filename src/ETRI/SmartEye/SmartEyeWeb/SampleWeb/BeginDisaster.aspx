<!DOCTYPE html>

<html lang="ko">
<head><meta charset="utf-8" />
    <title>Post Test</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" >
    <script type="text/javascript" src="js/jquery-1.11.3.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript" src="js/jquery.maskedinput.min.js"></script>
    <script type="text/javascript">
        function CallWebService() {

            var strStation = document.getElementById("txtStation").value;
            var strLocation = document.getElementById("txtLocation").value;
            var strEtc = document.getElementById("txtEtc").value;
            var strTime = document.getElementById("txtTime").value;

            var targetMethod = "BeginDisaster";
            var params = JSON.stringify({ station: strStation, location: strLocation, etc: strEtc, time: strTime });

            $.ajax({
                type: 'POST',
                url: 'http://unes.iptime.org:19050/SmartEye.asmx/' + targetMethod,
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
    <form id="form1" runat="server">
        <div>
            <table border="0">
                <tr>
                    <td style="text-align: right;width: 100px;">station : </td>
                    <td><input id="txtStation" type="text" /></td>
                </tr>
                <tr>
                    <td style="text-align: right">location : </td>
                    <td><input id="txtLocation" type="text" /></td>
                </tr>
                <tr>
                    <td style="text-align: right">etc : </td>
                    <td><input id="txtEtc" type="text" /></td>
                </tr>
                <tr>
                    <td style="text-align: right">time : </td>
                    <td><input id="txtTime" type="text" /></td>
                </tr>
            </table>
            <input id="btnSubmit" type="button" onclick="CallWebService();" value="Call WebService" runat="server" />
        </div>
    </form>
</body>
</html>
