<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>
        Post Test
    </title>
    <script type="text/javascript" src="js/jquery-1.11.3.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript" src="js/jquery.maskedinput.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnSubmit").click(CallWebService);
            $("#txtActTime").mask("9999-99-99 99:99:99");
        });

        function CallWebService() {

            var strStation = $("#txtStation")[0].value;
            var strLocation = $("#txtLocation")[0].value;
            var strEtc = $("#txtEtc")[0].value;
            var strTime = $("#txtTime")[0].value;

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
    <form name="fromPostData" method="post" action="Disaster.aspx" id="fromPostData">
        <div>
            <input type="hidden" name="__VIEWSTATE" id="__VIEWSTATE" value="/wEPDwUKLTIwMDYwMzMwNWRkRdGTV9i+GGmhLoC3qhSJ/H8TEcnNvSU7Lfq5CBAmsNk=" />
        </div>

        <div>

            <input type="hidden" name="__VIEWSTATEGENERATOR" id="__VIEWSTATEGENERATOR" value="53890B13" />
            <input type="hidden" name="__EVENTVALIDATION" id="__EVENTVALIDATION" value="/wEdAAI6mtFx+j/tfAPKRXPdbOKAPOaW1pQztoQA36D1w/+bXZUPt9SC9YdyAjGrnsV/jhrRlLdiXTlWxFh2dP8GTi1U" />
        </div>
        <div>
            <table border="0">
                <tr>
                    <td style="text-align: right;width: 100px;">Job ID : </td>
                    <td><input id="txtStation" type="text" /></td>
                </tr>
                <tr>
                    <td style="text-align: right">Act ID : </td>
                    <td><input id="txtLocation" type="text" /></td>
                </tr>
                <tr>
                    <td style="text-align: right">Date : </td>
                    <td><input id="txtEtc" type="text" /></td>
                </tr>
                <tr>
                    <td style="text-align: right">JDesc : </td>
                    <td><input id="txtTime" type="text" /></td>
                </tr>
            </table>
            <input name="btnSubmit" type="button" id="btnSubmit" value="Call WebService" />
        </div>
    </form>
</body>
</html>
