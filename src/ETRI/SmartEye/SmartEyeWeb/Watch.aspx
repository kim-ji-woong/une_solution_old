<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Watch.aspx.cs" Inherits="SmartEyeWeb.Watch" %>

<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>SMART EYE SIMULATION SYSTEM</title>
	<!--meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta charset="utf-8"-->
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<link rel="stylesheet" type="text/css" href="./CSS/style.css" />

    <script type="text/javascript" src="js/jquery-1.11.3.js"></script>
    <script type="text/javascript" src="js/jquery.maskedinput.min.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript">

        function ProcessMove(nPosition) {
            $(function () {
                $("#process_pointer").css("width", nPosition + "px");
            });
        }

        function SetImage(tagID, imageURL)
        {
            $(function () {
                $("#" + tagID).attr("src", imageURL);
            });
        }

        function SetBackgroundImage(tagID, imageURL)
        {
            $(function () {
                $("#" + tagID).css("background", imageURL);
            });
        }

        function SetBorderColor(tagID, color)
        {
            $(function () {
                $("#" + tagID).css("border", color);
            });
        }

        function SetFontColor(tagID, color) {
            $(function () {
                $("#" + tagID).css("color", color);
            });
        }

        var isButtonEnable = false;

        function SetClass(tagID, className)
        {
            if (className == "btn_reset2")
                isButtonEnable = false;
            else
                isButtonEnable = true;

            $(function () {
                $("#" + tagID).attr("class", className);

            });
        }

        $(function () {
            $("#btnReset").click(function () {

                if (isButtonEnable == false)
                    return;

                var targetMethod = "EndDisaster";
                var params = JSON.stringify({ disasterID: 0 });

                $.ajax({
                    type: 'POST',
                    url: 'http://unes.iptime.org:19050/SmartEye.asmx/' + targetMethod,
                    data: params,
                    contentType: 'application/json',
                    dataType: 'json',
                    async: false,
                    success: function (msg) {
                        SuccessFunc(msg);
                    },
                    error: function (msg) {
                        FailFunc(msg);
                    }
                });
            });

        });

        function SuccessFunc(msg) {
            //alert("Success : " + msg.d);
        }

        function FailFunc(msg) {
            alert(msg.status + " : " + msg.statusText);
        }

        function PopupImage(link, windowName)
        {
            var href;
            if (typeof(link) == 'string')
                href = link;
            else
                href = link.href;

            var w = window.open(href, "", "width=800, height=400, scrollbars=yes, resizable=1");
            SetWindowTitle(w);
            //window.open(href, "대응이미지", "width=800, height=400, scrollbars=yes");
        }

        function SetWindowTitle(mapWin)
        {
            if (mapWin.document) // If loaded
            {
                mapWin.document.title = "Oil Field Map";
            }
            else // If not loaded yet
            {
                setTimeout(setWindowTitle, 10); // Recheck again every 10 ms
            }
        }

        function SetText(tagID, str) {
            $(function () {
                $("#" + tagID).text(str);
            });
        }

    </script>

</head>

<body>
    <form id="formWatch" runat="server">
        <asp:ScriptManager id="ScriptManager1" runat="server" />
        <asp:Timer id="timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>
        <div id="wrap">
            <div id="header">
                <h1 class="logo"><img src="./Images/logo.png" alt="ETRI 한국전자통신연구원 - SMART EYE SIMULATION SYSTEM" /></h1>
            </div>
            <div id="container">
                <div class="top_view_area">
                    <!-- s: 20160126 -->
			        <div class="view_box st01 clfix"> <!-- 20160126 // class : st01 추가-->
                        <div class="left_box st01"> <!-- class : st01 추가-->
                            <p id="leftBox"><img id="droneImageTitle" src="./Images/txt_smov_off.png" alt="현재 수집 영상" /></p>
                            <div>
                                <img id="droneImage" src="./Images/img01.jpg" width="500" height="401" style="border: 5px solid white;" /> <!-- 20160126 // 이미지 사이즈 수정 -->
                                <span id="droneImageTimer" class="time">00:00:00</span>
                                <%--<asp:UpdatePanel id="UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <contenttemplate>
                                        <img id="droneImage" src="./Images/img01.jpg" width="537" height="431" style="border: 5px solid white;" />
                                    </contenttemplate>
                                    <triggers>
                                        <asp:asyncpostbacktrigger controlid="timer1" eventname="Tick" />
                                    </triggers>
                                </asp:UpdatePanel>--%>
                                <!--span class="time">00:00:00</span-->
                            </div>
                        </div>
                        <div class="center_box st01"> <!-- class : st01 추가-->
					        <p id="middleBox"><img id="mapTitle" src="./Images/txt_spot.png" alt="현재 위치" /></p>  <!-- 20160126 // 현재 재난위치 이미지가 없어서 현재 위치로 넣어두었습니다. -->
					        <div id="map" style="border:5px solid white;"></div>
				        </div>
                        <div class="right_box st01"> <!-- class : st01 추가-->
                            <p id="rightImageBox"><img id="disasterImageTitle" src="images/txt_jmov_off.png" alt="현재 재난 영상" /></p>
                            <div><img id="disasterImage" src="./Images/img02.jpg" width="500" height="401" style="border: 5px solid white;" /></div> <!-- 20160126 // 이미지 사이즈 수정 -->                            
                        </div>
                    </div>
                    <!-- e: 20160126 -->
                </div>
                <div class="bottom_view_area">
                    <div class="view_box clfix">
                        <div class="left_b_box">
                            <p><img id="statusInfo" src="./Images/txt_info.png" alt="현재 상황 정보" /></p>
                            <div id="LeftBox">
                            <asp:UpdatePanel id="UpdatePanelStatus" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
                                    <asp:Repeater id="repeaterStatus" runat="server">
                                        <headertemplate>
                                            <ul>
                                        </headertemplate>
                                        <itemtemplate>
                                            <li>• <%# DataBinder.Eval(Container.DataItem, "StatusInfo") %></li>
                                        </itemtemplate>
                                        <footertemplate>
                                            </ul>
                                        </footertemplate>
                                    </asp:Repeater>
                                </contenttemplate>
                                <triggers>
                                    <asp:asyncpostbacktrigger controlid="timer1" eventname="Tick" />
                                </triggers>
                            </asp:UpdatePanel>
                            </div>
                            <!--ul>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                            </ul-->
                        </div>
                        <div class="right_b_box">
                            <p><img id="actionInfo" src="./Images/txt_act.png" alt="현재 대응 정보" /></p>
                            <div id="RightBox">
                            <asp:UpdatePanel id="UpdatePanelAction" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
                                    <asp:Repeater id="repeaterAction" runat="server">
                                        <headertemplate>
                                            <ul>
                                        </headertemplate>
                                        <itemtemplate>
                                            <li <%# String.IsNullOrWhiteSpace(Eval("URLColumnName").ToString()) == false ? "style='cursor:pointer;text-decoration:underline;' onclick='javascript:return PopupImage(\"" + Eval("URLColumnName").ToString() + "\", \"대응이미지\");'": " " %> >• <%# DataBinder.Eval(Container.DataItem, "ActionInfo") %></li>
                                            <%--<li style="cursor:pointer;font" onclick="return PopupImage('http://img.etoday.co.kr/pto_db/2016/01/20160104033804_789400_600_338.jpg', '대응이미지')">대응 이미지</li>--%>
                                            <%--<li>• <%# DataBinder.Eval(Container.DataItem, "ActionInfo") %></li>--%>
                                        </itemtemplate>
                                        <footertemplate>
                                            </ul>
                                        </footertemplate>
                                    </asp:Repeater>
                                </contenttemplate>
                                <triggers>
                                    <asp:asyncpostbacktrigger controlid="timer1" eventname="Tick" />
                                </triggers>
                            </asp:UpdatePanel>
                            </div>
                            <!--ul>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                                <li><a href="#">센서1(가시광선) 정보: 화염 및 연기 감지 정보</a></li>
                            </ul-->
                        </div>
                        <input id="btnReset" type="button" class="btn_reset" title="초기화" />
                        
                        <div class="state_bar">
                            <p><img src="./Images/txt_step.png" alt="진행상황" /></p>
                            <div class="n_state">
                                <div class="process_bar"><span id="process_pointer" style="width:10px"></span></div> <!-- span 의 width값을 조절해주시면 게이지가 차는것처럼 보입니다. -->
                                <!--div class="process_bar"><span style="width:835px"></span></div--> <!-- span 의 width값을 조절해주시면 게이지가 차는것처럼 보입니다. (167 * n) -->
                                <ul>
                                    <li>
                                        <img id="collect" src="./Images/step_off.png" alt="" />
                                        <span>수집</span>
                                    </li>
                                    <li>
                                        <img id="analys" src="./Images/step_off.png" alt="" />
                                        <span>분석</span>
                                    </li>
                                    <li>
                                        <img id="predict" src="./Images/step_off.png" alt="" />
                                        <span>예측</span>
                                    </li>
                                    <li>
                                        <img id="warning" src="./Images/step_off.png" alt="" />
                                        <span>경보</span>
                                    </li>
                                    <li class="last">
                                        <img id="reaction" src="./Images/step_off.png" alt="" />
                                        <span>대응</span>
                                    </li>
                                    <!--li>
                                        <img src="./Images/step_off.png" alt="" />
                                        <span>감지</span>
                                    </li>
                                    <li>
                                        <img src="./Images/step_off.png" alt="" />
                                        <span>예측</span>
                                    </li>
                                    <li>
                                        <img src="./Images/step_off.png" alt="" />
                                        <span>대응</span>
                                    </li>
                                    <li>
                                        <img src="./Images/step_off.png" alt="" />
                                        <span>대응2</span>
                                    </li>
                                    <li>
                                        <img src="./Images/step_off.png" alt="" />
                                        <span>대응3</span>
                                    </li>
                                    <li>
                                        <img src="./Images/step_on.gif" alt="" />
                                        <span>대응4</span>
                                    </li>
                                    <li class="last">
                                        <img src="./Images/step_off.png" alt="" />
                                        <span>완료</span>
                                    </li-->
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </form>

    <script type="text/javascript" src="http://openapi.map.naver.com/openapi/naverMap.naver?ver=2.0&key=ec798318e99305f15fd6bcb9d256df80"></script>
    <script type="text/javascript">
                                var latitude = 37.5675451;
                                var longitude = 126.9773356;

                                var oInitPoint = new nhn.api.map.LatLng(latitude, longitude);
                                var defaultLevel = 11;
                                var oMap = new nhn.api.map.Map(document.getElementById('map'), {
                                    point: oInitPoint,
                                    zoom: defaultLevel,
                                    enableWheelZoom: true,
                                    enableDragPan: true,
                                    enableDblClickZoom: false,
                                    mapMode: 1,
                                    activateTrafficMap: false,
                                    activateBicycleMap: false,
                                    minMaxLevel: [1, 14],
                                    size: new nhn.api.map.Size(500, 401)
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
                            </script>
</body>
</html>
