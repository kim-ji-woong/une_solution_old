<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeoMap.aspx.cs" Inherits="SmartEyeWeb.SampleWeb.GeoMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript" src="http://openapi.map.naver.com/openapi/naverMap.naver?ver=2.0&key=ec798318e99305f15fd6bcb9d256df80"></script>
    <div id="map" style="border:1px solid #000;">
        <asp:Label ID="Label1" runat="server" Text="이동 좌표(위도, 경도) : "></asp:Label>
        <asp:TextBox ID="textBoxCoord" runat="server"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnMove" runat="server" OnClick="btnMove_Click" Text="이동" />
        </div>

        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers> 
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" /> 
            </Triggers>
        </asp:UpdatePanel>

    <script type="text/javascript">
        //var strCoord = document.getElementById("textBoxCoord").value;
        //var strArray = strCoord.split(',');

        var latitude = 37.5675451;
        var longitude = 126.9773356;

        /*if (strArray.length == 2) {
            var lat = parseFloat(strArray[0].trim());
            var lon = parseFloat(strArray[1].trim());

            if (lat != NaN && lon != NaN) {
                latitude = lat;
                longitude = lon;
            }
        }*/

        var oInitPoint = new nhn.api.map.LatLng(latitude, longitude);
        var defaultLevel = 11;
        var oMap = new nhn.api.map.Map(document.getElementById('map'), {
            point: oInitPoint,
            zoom: defaultLevel,
            enableWheelZoom: true,
            enableDragPan: true,
            enableDblClickZoom: false,
            mapMode: 0,
            activateTrafficMap: false,
            activateBicycleMap: false,
            minMaxLevel: [1, 14],
            size: new nhn.api.map.Size(800, 480)
        });
        var oSlider = new nhn.api.map.ZoomControl();
        oMap.addControl(oSlider);
        oSlider.setPosition({
            top: 10,
            left: 10
        });

        var oMapTypeBtn = new nhn.api.map.MapTypeBtn();
        oMap.addControl(oMapTypeBtn);
        oMapTypeBtn.setPosition({
            bottom: 10,
            right: 80
        });

        var oSize = new nhn.api.map.Size(28, 37);
        var oOffset = new nhn.api.map.Size(14, 37);
        var oIcon = new nhn.api.map.Icon('http://static.naver.com/maps2/icons/pin_spot2.png', oSize, oOffset);

        var oInfoWnd = new nhn.api.map.InfoWindow();
        oInfoWnd.setVisible(false);
        oMap.addOverlay(oInfoWnd);

        oInfoWnd.setPosition({
            top: 20,
            left: 20
        });

        var oLabel = new nhn.api.map.MarkerLabel(); // - 마커 라벨 선언.
        oMap.addOverlay(oLabel); // - 마커 라벨 지도에 추가. 기본은 라벨이 보이지 않는 상태로 추가됨.

        oInfoWnd.attach('changeVisible', function (oCustomEvent) {
            if (oCustomEvent.visible) {
                oLabel.setVisible(false);
            }
        });

        oMap.attach('mouseenter', function (oCustomEvent) {

            var oTarget = oCustomEvent.target;
            // 마커위에 마우스 올라간거면
            if (oTarget instanceof nhn.api.map.Marker) {
                var oMarker = oTarget;
                oLabel.setVisible(true, oMarker); // - 특정 마커를 지정하여 해당 마커의 title을 보여준다.
            }
        });

        oMap.attach('mouseleave', function (oCustomEvent) {

            var oTarget = oCustomEvent.target;
            // 마커위에서 마우스 나간거면
            if (oTarget instanceof nhn.api.map.Marker) {
                oLabel.setVisible(false);
            }
        });

        oMap.clearOverlay();

        var oMarker = new nhn.api.map.Marker(oIcon, {});
        oMarker.setPoint(oInitPoint);
        oMap.addOverlay(oMarker);

        /*oMap.attach('click', function (oCustomEvent) {
            oMap.setCenterAndLevel(oCustomEvent.point, 11);
        });*/

        /*oMap.attach('click', function (oCustomEvent) {
            var oPoint = oCustomEvent.point;
            var oTarget = oCustomEvent.target;
            oInfoWnd.setVisible(false);
            // 마커 클릭하면
            if (oTarget instanceof nhn.api.map.Marker) {
                // 겹침 마커 클릭한거면
                if (oCustomEvent.clickCoveredMarker) {
                    return;
                }
                // - InfoWindow 에 들어갈 내용은 setContent 로 자유롭게 넣을 수 있습니다. 외부 css를 이용할 수 있으며,
                // - 외부 css에 선언된 class를 이용하면 해당 class의 스타일을 바로 적용할 수 있습니다.
                // - 단, DIV 의 position style 은 absolute 가 되면 안되며,
                // - absolute 의 경우 autoPosition 이 동작하지 않습니다.
                oInfoWnd.setContent('<DIV style="border-top:1px solid; border-bottom:2px groove black; border-left:1px solid; border-right:2px groove black;margin-bottom:1px;color:black;background-color:white; width:auto; height:auto;">' +
                    '<span style="color: #000000 !important;display: inline-block;font-size: 12px !important;font-weight: bold !important;letter-spacing: -1px !important;white-space: nowrap !important; padding: 2px 5px 2px 2px !important">' +
                    'Hello World <br /> ' + oTarget.getPoint()
                    + '<span></div>');
                oInfoWnd.setPoint(oTarget.getPoint());
                oInfoWnd.setPosition({ right: 15, top: 30 });
                oInfoWnd.setVisible(true);
                oInfoWnd.autoPosition();
                return;
            }

            oMap.clearOverlay();

            var oMarker = new nhn.api.map.Marker(oIcon, { title: '좌표 : ' + oPoint.toString() });
            oMarker.setPoint(oPoint);
            oMap.addOverlay(oMarker);
        });*/
    </script>
    </form>
</body>
</html>
