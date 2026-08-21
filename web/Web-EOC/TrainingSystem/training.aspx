<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="training.aspx.cs" Inherits="TrainingSystem.training" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">        
    <meta content="IE=edge" http-equiv="X-UA-Compatible"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"/>
    <title>한국 지역난방공사</title>
    <link rel="stylesheet" href="./css/kdhc.css"/>
    <link rel="stylesheet" href="./js/themes/default/style.min.css" />
    <script src="https://code.jquery.com/jquery-3.0.0.js"></script>
    <script src="https://code.jquery.com/jquery-migrate-3.0.0.js"></script>
    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
    <![endif]-->
    <script src="./js/kdhc.js"></script>    
    <script src="./js/hashmap.js"></script>   
    <script src="./js/jstree.min.js"></script>
    <script>
        window.onload = function () {
            Init();
            GetBuildingData();
        };
    </script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9" >          <![endif]-->
<!--[if (gt IE 9) ]> <body>       <![endif]-->
<div id="outline">
  <div id="wrap">
    <div id="header" class="clear">
      <h1><img src="images/logo.png" /></h1>
      <div class="range">
        <h2 class="dropdown">전체</h2>
        <ul>
          <li><a href="">전체</a></li>
          <li><a href="">지역</a></li>
        </ul>
      </div>
      <div class="datetime" id="currentDateTime">2017-07-25 11:20</div>
    </div><!-- header-->
    <div class="container">
        <div class="gnb">
            <div><a href="status.aspx">현황</a></div>
            <div class="active"><a href="training.aspx">훈련</a></div>
            <div><a href="main.aspx">관리</a></div>
        </div><!-- gnb -->

        <div class="content traning-container">
          <div class="title-underline"><h2 class="">광교지사</h2></div>

          <form class="search-form clear" runat="server">     
              <asp:ScriptManager ID="ScriptManager2" runat="server" EnablePageMethods="true"/>         
                <asp:HiddenField runat="server" ID="hdf_Test"/>
              <div class="input-row ">
                <label for="selectBuilding">건물구역</label>
                <select name="selectBuildingGroup" onclick="" onchange="UpdateBuildingDropDown();" id="buildingGroupDropDown">
                </select>
            </div>
            <div class="input-row ">
                <label for="selectBuilding">건물</label>
                <select name="selectBuilding" onclick="" onchange="UpdateEquipZoneDropDown();" id="buildingDropDown">
                </select>
            </div>
            <div class="input-row ">
                <label for="select3">재난</label>
                <select name="select3" id="sensorTypeDropDown" onchange="UpdateEquipZoneDropDown();">
                    <option value="101">화재</option>
                    <option value="11">누출</option>
                </select>
            </div>
            <div class="input-row ">
                <label for="select3">센서구역</label>
                <select name="select1" id="equipZoneDropDown" onchange="UpdateZoneMap();">
                    <option value=""></option>
                </select>
            </div>
            <div class="input-row search-btn">
                <a href="#" class="btn-blue" id="btnGuide" onclick="ShowGuideDialog();" >안내</a>
                <a href="#" class="btn-blue" id="btnRun" onclick="ShowTrainningDialog();">실행</a>
            </div>
          </form>

          <div class="drawing">
              <canvas id="myCanvas" width="1024" height="648" style="border:1px solid #000000;">
            </canvas>            
          </div>

        </div><!-- content -->


  <div class="popup " id="popup">
    <a href="#" class="btn_popup_close" onclick="$('#popup').hide();"><img src="./images/btn_popup_close.png" width="13" height="12" alt="닫기" /></a>
    <div class="message ">
        <h2>광교지사</h2>
        <div class="con">
          <h3>[<span class="text-red">“재난종류"</span> 상황 발생]</h3>
          <p>- 2017-01-23   14:23:24</p>
          <p>- ㅇㅇㅇ (위치)</p>
        </div>
    </div> <!-- message -->
  </div> <!-- popup -->
  <div class="popup popup-alert " id="popup_run_trainning">
      <a href="#" class="btn_popup_close" onclick="$('#popup_alert').hide();"><img src="./images/btn_popup_close_w.png" width="13" height="12" alt="닫기" /></a>
      <h2>훈련 안내 문자 발송</h2>
        <div class="message " id="runTrainningText">
          <p><span class='text-red' >{지사-location} 사업소 {location} 센서</span>를 작동하여 훈련을 시작하시겠습니까?</p>
        </div>
        <div class="btn-list">
          <a href="#" class="btn-black" onclick="runTrainning();">확인</a>
          <a href="#" class="btn-black" onclick="$('#popup_run_trainning').hide();">취소</a>
        </div>
     </div><!-- popup-alert -->

    <div class="popup popup-alert " id="popup_sms1" style="left: 560px;top:194px">
      <a href="#" class="btn_popup_close" onclick="$('#popup_sms1').hide();"><img src="./images/btn_popup_close_w.png" width="13" height="12" alt="닫기" /></a>
      <h2 >훈련 안내 문자 발송</h2>
      <div class="con-sms ">
        <div class="tab-menu">
          <ul class="clear">
            <li class="active"><a href="#sms_con_1">안내방송</a></li>
            <li><a href="#sms_con_2">SMS</a></li>
          </ul>
        </div>
         <div class="tab_menu_target active" id="sms_con_1">
            <form class="form" id="sms_con_form_1">
                <!-- 안내방송문 넣는 부분 -->
              <div class="form-input"><textarea name="" id="broadcastMessage"></textarea></div>
              <ul>
                <li>
                  <input type="checkbox" value="0" id="radio_siren" name="alert1" class="checkbox" checked="checked" />
                  <label for="checkbox_1" class="active">사이렌 알람</label>
                </li>
                <li>
                  <input type="radio" value="0" id="radio_1" name="alertRepeat" class="radio" checked="checked"/>
                          <label for="radio_1" class="active">방송 반복없음</label>
                          <input type="radio" value="1" id="radio_2" name="alertRepeat" class="radio" />
                          <label for="radio_2" class="active">1회 반복</label>
                          <input type="radio" value="2" id="radio_3" name="alertRepeat" class="radio"  />
                          <label for="radio_3" class="active">2회 반복</label>
                </li>
              </ul>
            </form>
             <div class="text-right">
          <a href="#" class="btn-black" onclick="sendBroadcast();">방송실시</a>
         </div>
         </div><!-- sms_con_1 -->
         <div class="tab_menu_target" id="sms_con_2">
            <form class="form" id="sms_con_form_2">
                <!--SMS 넣는 부분-->
              <div class="form-input"><textarea name="" id="sms"></textarea></div>
              <div class="receive-btn">
                <a href="#" class="btn-white" onclick="setSmsReceiver();">수신자 지정</a>
              </div>

            </form>
         <div class="text-right">
          <a href="#" class="btn-black" onclick="sendSms();">메시지발송</a>
         </div>
        </div><!-- sms_con_2 -->
      </div><!-- con-sms -->
    </div><!-- popup_sms1 -->
    <div class="popup popup-alert  " id="popup_sense" >
      <a href="#" class="btn_popup_close" onclick="$('#popup_sense').hide();"><img src="./images/btn_popup_close_w.png" width="13" height="12" alt="닫기" /></a>
      <h2 >전체 센서 목록</h2>
      <div class="con-sense ">
          <form class="form search-form" >
            <div>
              <input type="text" name="keyword" id="serachSensorZone" class="input-text" placeholder="검색할 내용을 입력하세요" />
              <input type="image" src="images/btn_search.png" class="btn-search-submit" />
            </div>
          </form>
          <form class="form" id="sms_con_form_2">
            <div id="jstree_demo_div" style="overflow:scroll; width:500px; height:400px;"">
                <!-- in this example the tree is populated from inline HTML -->
            </div>
             <div class="btn-list">
              <a href="#" class="btn-black" onclick="UpdateSensorZoneByPopupWindow();">불러오기</a>
              <a href="#" class="btn-black" onclick="$('#popup_sense').hide();">취소</a>
            </div>
          </form>
      </div><!-- con-sms -->
    </div><!-- popup_sense -->
  </div> <!-- wrap -->
</div><!-- outline -->
<!-- javascipt 에서 asp.net 데이터 전달용 hidden field -->
<input type="hidden" ID="accomodationAnswer"/>
</body>      
    <script>
        var map = new HashMap();
        var sensorZoneImageMap = new HashMap();
        var buildingData = "";
        var showCount = 0;
        var selectedNodeId = "";

        //var broadcastLocationName = "";
        //var broadcastMateiralName = "";

        setInterval(writeCurrentDateTime, 1000);

        function writeCurrentDateTime() {
            var currentDateTime = document.getElementById("currentDateTime");

            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();

            var hour = today.getHours();
            var min = today.getMinutes();
            var sec = today.getSeconds();

            currentDateTime.innerHTML = yyyy + "-" + mm + "-" + dd + " " + hour + ":" + min + ":" + sec;
            //$("currentDateTime").update(yyyy + "-" + mm + "-" + dd + " " + hour + ":" + min + ":" + sec);
        }

        function Init()
        {
            $('#popup').hide();
            $('#popup_run_trainning').hide();
            $('#popup_sms1').hide();
            $('#popup_sense').hide();
            writeCurrentDateTime();

            setInterval(checkDisaster, 3000);

            function checkDisaster() {
                PageMethods.CheckDisaster(onSuccessCheckDisaster, onError);
            }

            function onSuccessCheckDisaster(result)
            {
                if(result.length > 0)
                    location.href = "status.aspx";
            }

            //Add custom handler on show event and print message
            $('#popup_sense').on('show', function () {
                if (showCount == 0)
                {
                    var sensorTypeDropDown = document.getElementById("sensorTypeDropDown");

                    var sensorType = sensorTypeDropDown.options[sensorTypeDropDown.selectedIndex].value;

                    PageMethods.GetTotalSensorZoneList("3", sensorType, onSucessGetTotalSensorZoneList, onError);
                }                

                showCount++;
            });
        }

        function sendBroadcast()
        {
            var broadcastMessageTextbox = document.getElementById("broadcastMessage");
            var text = broadcastMessageTextbox.value;
            var useSirenCheckBox = document.getElementById("radio_siren");
            var useSiren = useSirenCheckBox.checked;
            var repeatRadio = document.getElementsByName("alertRepeat");
            
            var radioIndex = 0;

            for(i=0;i<repeatRadio.length;i++)
            {
                if(repeatRadio[i].checked)
                {
                    radioIndex = i + 1; // 0회 반복일 경우 1이 들어가야 한다.
                    break;
                }
            }            

            PageMethods.InsertBroadcastMessage(text, useSiren ? "TRUE" : "FALSE", radioIndex.toString(),"3", onSuccessInsertBroadcastMessage, onError);
            
        }

        function onSuccessInsertBroadcastMessage(result)
        {
            //do nothing
            alert("안내 방송을 수행합니다.")
        }

        function runTrainning()
        {
            var equipZoneDropDown = document.getElementById("equipZoneDropDown");
            var equipZoneId = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].value;

            var sensorTypeDropDown = document.getElementById("sensorTypeDropDown");
            var sensorTypeId = sensorTypeDropDown.options[sensorTypeDropDown.selectedIndex].value;
            

            $('#popup_run_trainning').hide();

            //PageMethods.GetSensorZoneList(equipZoneId, onSuccessGetSensorZoneList, onError);

            PageMethods.InsertTrainningSensorActivation(equipZoneId, sensorTypeId, onSuccessSensorHistory, onError);
        }

        //function onSuccessGetSensorZoneList(result)
        //{
        //    var sensorZoneIdList = result.split("###");

        //    for(i=0;i<sensorZoneIdList.length;i++)
        //    {
        //        if (sensorZoneIdList[i].length > 0)
        //            PageMethods.InsertTrainningSensorActivation(sensorZoneIdList[i], onSuccessInsertTrainningSensorActivation, onError);
        //    }
            
        //}

        function onSuccessInsertTrainningSensorActivation(result)
        {
            //do nothing
        }

        function onSuccessSensorHistory(result)
        {

        }

        function setSmsReceiver()
        {

        }

        function sendSms()
        {
            if (confirm('메시지를 발송하겠습니까?'))
            {
                var equipZoneDropDown = document.getElementById("equipZoneDropDown");

                var equipZoneId = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].value;

                var smsTextbox = document.getElementById("sms");
                var text = smsTextbox.value;

                PageMethods.SendSms(text, "01045414731");
            }

        }

        function onSucessGetTotalSensorZoneList(result)
        {
            var zoneList = result.split("@@@");

            var counter = 0;

            for(i=0;i<zoneList.length;i++)
            {
                var zoneString = zoneList[i];

                if (zoneString.length > 0)
                {
                    //var zoneListElement = zoneString.split(",,");
                    var sensorZoneList = zoneString.split("###");

                    if (sensorZoneList.length > 0)
                    {
                        //zone
                        var sensorZoneListElement = sensorZoneList[0].split(",,");
                        var zoneId = "zone"+sensorZoneListElement[0];
                        $('#jstree_demo_div').jstree().create_node('#', { "id": zoneId, "text": sensorZoneListElement[1] }, "last");
                        //sensor zone
                        for (j = 1; j < sensorZoneList.length; j++)
                        {
                            var sensorZoneListElement = sensorZoneList[j].split(",,");
                            var sensorZoneId = sensorZoneListElement[0];
                            var sensorZoneName = sensorZoneListElement[1];

                            $('#jstree_demo_div').jstree().create_node(zoneId, { "id": sensorZoneId, "text": sensorZoneName }, "last");                       
                        }
                    }                      
                }
            }
        }

        function GetBuildingData()
        {
            //alert("update building");
            buildingData = "<%=UpdateBuildingDropDownServerSide()%>";
            //alert(buildingData);

            UpdateBuildingGroupDropdown();

            //UpdateBuildingDropDown();
        }

        function ShowGuideDialog()
        {
            $('#popup_sms1').show();

            var sensorTypeDropDown = document.getElementById("sensorTypeDropDown");

            var sensorType = sensorTypeDropDown.options[sensorTypeDropDown.selectedIndex].value;

            var disasterCategory = "2"; // 2 means fire, 3 means leak

            if (sensorType == "101" || sensorType == "102") //fire sensor
            {
                disasterCategory = "2";
            }
            else if (sensorType == "11") //유해화학물질 누출감지 센서
            {
                disasterCategory = "3"; //leak
            }
            

            PageMethods.GetBroadcastMessage("3", disasterCategory, onSuccessGetBroadcastMessage, onError);
            PageMethods.GetSmsMessage("3", disasterCategory, onSuccessGetSms, onError);
        }

        function getDate()
        {
            var date = new Date();

            var month = date.getMonth() + 1; // 0부터 시작하므로 1더함 더함
            var day = date.getDate();

            return (month + "월 " + day + "일");
        }

        function onSuccessGetSms(result)
        {
            var smsTextBox = document.getElementById("sms");

            var smsElements = result.split("###");

            var sms = smsElements[0];
            var disasterType = smsElements[1]

            //replace date
            var date = getDate();

            sms = sms.replace("{date}", date);

            smsTextBox.value = sms;

            //parse sms message
            var equipZoneDropDown = document.getElementById("equipZoneDropDown");

            var equipZoneId = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].value;

            PageMethods.GetEquipZoneBroadcastName(equipZoneId, onSuccessGetEquipZoneBroadcastName, onError);

            PageMethods.GetSiteNameByEquipZoneId(equipZoneId, onSuccessGetSiteNameByEquipZoneId, onError);

            if (disasterType == "3") //leak
            {
                PageMethods.GetEquipZoneMaterialName(equipZoneId, onSuccessGetEquipZoneMaterialName, onError);
            }                                    
        }

        function onSuccessGetSiteNameByEquipZoneId(result)
        {
            var broadcastMessageTextbox = document.getElementById("broadcastMessage");

            var broadcastMessage = broadcastMessageTextbox.value;

            broadcastMessage = broadcastMessage.replace("{지사-location}", result);

            broadcastMessageTextbox.value = broadcastMessage;

            var smsTextbox = document.getElementById("sms");

            var sms = smsTextbox.value;

            sms = sms.replace("{지사-location}", result);

            smsTextbox.value = sms;

            var runTrainningText = document.getElementById("runTrainningText");

            var innerHtml = runTrainningText.innerHTML;

            innerHtml = innerHtml.replace("{지사-location}", result);

            runTrainningText.innerHTML = innerHtml;
        }        

        function onSuccessGetBroadcastMessage(result)
        {
            var broadcastMessageTextbox = document.getElementById("broadcastMessage");

            var broadcastMessageElements = result.split("###");

            var broadcastMessage = broadcastMessageElements[0];
            var useSiren = broadcastMessageElements[1];
            var repeatCount = broadcastMessageElements[2];
            var disasterType = broadcastMessageElements[3]
            
            //replace date
            var date = getDate();

            broadcastMessage = broadcastMessage.replace("{date}", date);

            var useSirenCheckBox = document.getElementById("radio_siren");

            if (useSiren == "1")
            {
                useSirenCheckBox.checked = true;
            }
            else
                useSirenCheckBox.checked = false;

            var repeatCountInt = parseInt(repeatCount);

            var repeatRadio = document.getElementsByName("alertRepeat");
            repeatRadio[repeatCountInt].checked = true;

            //parse broadcast message
            var equipZoneDropDown = document.getElementById("equipZoneDropDown");

            var equipZoneId = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].value;

            broadcastMessageTextbox.value = broadcastMessage;
            //방송 메시지를 해석해서 데이터를 메시지에 넣음

            PageMethods.GetEquipZoneBroadcastName(equipZoneId, onSuccessGetEquipZoneBroadcastName, onError);
            
            if(disasterType == "3") //leak
            {
                PageMethods.GetEquipZoneMaterialName(equipZoneId, onSuccessGetEquipZoneMaterialName, onError);                
            }       
        }

        function onSuccessGetEquipZoneMaterialName(result)
        {
            var broadcastMessageTextbox = document.getElementById("broadcastMessage");

            var broadcastMessage = broadcastMessageTextbox.value;

            broadcastMessage = broadcastMessage.replace("{PSMMaterial}", result);

            broadcastMessageTextbox.value = broadcastMessage;

            var smsTextbox = document.getElementById("sms");

            var sms = smsTextbox.value;

            sms = sms.replace("{PSMMaterial}", result);

            smsTextbox.value = sms;
        }

        function onSuccessGetEquipZoneBroadcastName(result)
        {
            var broadcastMessageTextbox = document.getElementById("broadcastMessage");

            var broadcastMessage = broadcastMessageTextbox.value;

            if (result.length > 0) {
                broadcastMessage = broadcastMessage.replace("{location}", result);
            }
            else {
                broadcastMessage = broadcastMessage.replace("{location}", "찾을수 없음");
            }

            broadcastMessageTextbox.value = broadcastMessage;

            var smsTextbox = document.getElementById("sms");

            var sms = smsTextbox.value;

            if (result.length > 0) {
                sms = sms.replace("{location}", result);
            }
            else {
                sms = sms.replace("{location}", "찾을수 없음");
            }

            smsTextbox.value = sms;

            var runTrainningText = document.getElementById("runTrainningText");

            var innerHtml = runTrainningText.innerHTML;

            innerHtml = innerHtml.replace("{location}", result);

            runTrainningText.innerHTML = innerHtml;
        }

        function ShowTrainningDialog()
        {
            var equipZoneDropDown = document.getElementById("equipZoneDropDown");

            var equipZoneId = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].value;

            PageMethods.GetEquipZoneBroadcastName(equipZoneId, onSuccessGetEquipZoneBroadcastName, onError);

            PageMethods.GetSiteNameByEquipZoneId(equipZoneId, onSuccessGetSiteNameByEquipZoneId, onError);

            var runTrainningText = document.getElementById("runTrainningText");

            runTrainningText.innerHTML = "<p><span class='text-red' >{지사-location} 사업소 {location} 센서</span>를 작동하여 훈련을 시작하시겠습니까?</p>";

            $('#popup_run_trainning').show();
        }

        function onSuccessGetBuildingList(result)
        {
            if (0 == result.length)
                return;

            //clear building dropdown

            var buildingDropDown = document.getElementById("buildingDropDown");

            if (null == buildingDropDown)
                return;

            var length = buildingDropDown.options.length;

            for (i = length - 1 ; i >= 0 ; i--) {
                buildingDropDown.remove(i);
            }

            var buildingElements = result.split("###");

            for (i = 0; i < buildingElements.length; i += 2) {
                var objOption = document.createElement("option");

                objOption.value = buildingElements[i];
                objOption.text = buildingElements[i + 1];

                buildingDropDown.add(objOption);
            }
        }

        function UpdateBuildingDropDown()
        {
            //건물 ID를 받는다.
            var buildingGroupDropDown = document.getElementById("buildingGroupDropDown");

            if (null == buildingGroupDropDown)
                return;

            var buildingGroupId = buildingGroupDropDown.options[buildingGroupDropDown.selectedIndex].value;

            PageMethods.GetBuildingList(buildingGroupId, onSuccessGetBuildingList,onError);            
        }

        function UpdateEquipZoneDropDown() {
            var buildingDropDown = document.getElementById("buildingDropDown");

            var buildingId = buildingDropDown.options[buildingDropDown.selectedIndex].value;

            var sensorTypeDropDown = document.getElementById("sensorTypeDropDown");

            var sensorType = sensorTypeDropDown.options[sensorTypeDropDown.selectedIndex].value;

            PageMethods.GetEquipZoneList(buildingId, sensorType, onSuccessGetEquipZoneGroupList, onError);
        }

        function onSuccessGetEquipZoneGroupList(result)
        {
            //clear building dropdown

            var equipZoneDropDown = document.getElementById("equipZoneDropDown");

            if (null == equipZoneDropDown)
                return;

            var length = equipZoneDropDown.options.length;

            for (i = length - 1 ; i >= 0 ; i--) {
                equipZoneDropDown.remove(i);
            }

            if (0 == result.length)
                return;

            var equipZoneElements = result.split("###");

            for (i = 0; i < equipZoneElements.length; i += 2) {
                var objOption = document.createElement("option");

                objOption.value = equipZoneElements[i];
                objOption.text = equipZoneElements[i + 1];

                equipZoneDropDown.add(objOption);
            }
        }

        function UpdateZoneMap()
        {
            var equipZoneDropDown = document.getElementById("equipZoneDropDown");

            var equipZoneId = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].value;
            var equipZoneName = equipZoneDropDown.options[equipZoneDropDown.selectedIndex].text;

            if (-1 == parseInt(equipZoneId) && "전체 보기" == equipZoneName)
            {
                $('#popup_sense').show();

                return;
            }

            PageMethods.GetZoneImagePath(equipZoneId, onSuccessGetZoneImagePath, onError);                      
        }

        function onSuccessGetZoneImagePath(result)
        {
            var imageName = result;

            var canvas = document.getElementById("myCanvas");

            var context = canvas.getContext("2d");

            var imageObj = new Image();

            imageObj.onload = function () {
                context.drawImage(imageObj, 0, 0);

                context.font = "10pt Calibri";
                context.fillText(imageName, 20, 20);
            };
            imageObj.src = "images/sample/training.png";
        }

        function UpdateSensorZoneByPopupWindow()
        {
            $('#popup_sense').remove();

            if (selectedNodeId.length > 0)
            {
                PageMethods.GetSensorZoneImagePath(selectedNodeId, onSucessGetSensorZoneImagePath, onError)
            }
        }

        function onSucessGetSensorZoneImagePath(result)
        {
            var canvas = document.getElementById("myCanvas");

            var context = canvas.getContext("2d");

            var imageObj = new Image();

            imageObj.onload = function () {
                context.drawImage(imageObj, 0, 0);

                context.font = "10pt Calibri";
                context.fillText(result, 20, 20);
            };
            imageObj.src = "images/sample/training.png";
        }

        

        function UpdateSensorZone()
        {
            //var floorDropDown = document.getElementById("floorDropDown");

            //var floorIndex = floorDropDown.options[floorDropDown.selectedIndex].value;            

            var buildingDropDown = document.getElementById("buildingDropDown");         

            var buildingId = buildingDropDown.options[buildingDropDown.selectedIndex].value;

            var sensorTypeDropDown = document.getElementById("sensorTypeDropDown");

            var sensorType = sensorTypeDropDown.options[sensorTypeDropDown.selectedIndex].value;

            PageMethods.GetSensorZoneList(buildingId, floorIndex, sensorType, onSucessGetSensorZoneList, onError);
        }

        function onSucessGetSensorZoneList(result) {

            //clear sensor zone drop down

            var sensorZoneDropDown = document.getElementById("sensorZoneDropDown");

            var length = sensorZoneDropDown.options.length;

            for (i = length - 1 ; i >= 0 ; i--) {
                sensorZoneDropDown.remove(i);
            }

            var sensorZoneList = result.split("@@");

            sensorZoneImageMap.clear();

            for(i=0;i<sensorZoneList.length;i++)
            {
                var sensorZoneElement = sensorZoneList[i].split("###");

                if (sensorZoneElement.length == 3)
                {
                    var objOption = document.createElement("option");
                    objOption.text = sensorZoneElement[1]; //description
                    objOption.value = sensorZoneElement[0]; //id

                    sensorZoneDropDown.add(objOption);

                    sensorZoneImageMap.put(sensorZoneElement[0], sensorZoneElement[2]);
                }
            }

            if (sensorZoneList.length > 0)
            {
                var objOptionLine = document.createElement("option");

                objOptionLine.text = "──────";
                objOptionLine.disabled = true;
                objOptionLine.value = "-9999";
                sensorZoneDropDown.add(objOptionLine);

                var objOption = document.createElement("option");
                objOption.text = "전체 보기";
                objOption.value = "-1";
                sensorZoneDropDown.add(objOption);
            }            
        }

        function onError(result) {
            alert('Cannot process your request at the moment, please try later.');
        }

        function UpdateBuildingGroupDropdown()
        {
            PageMethods.GetBuildingGroupList("3", onSuccessGetBuildingGroupList, onError);
        }

        function onSuccessGetBuildingGroupList(result)
        {
            if (0 == result.length)
                return;

            //clear buildingGroup dropdown

            var buildingGroupDropDown = document.getElementById("buildingGroupDropDown");

            var length = buildingGroupDropDown.options.length;

            for (i = length - 1 ; i >= 0 ; i--) {
                buildingGroupDropDown.remove(i);
            }

            var buildingGroupElements = result.split("###");            

            if (null == buildingGroupDropDown)
                return;

            for(i=0;i<buildingGroupElements.length;i+=2)
            {
                var objOption = document.createElement("option");
                
                objOption.value = buildingGroupElements[i];
                objOption.text = buildingGroupElements[i + 1];

                buildingGroupDropDown.add(objOption);
            }
        }

        //function UpdateFloor()
        //{
        //    var buildingDropDown  = document.getElementById("buildingDropDown");

        //    if(null == buildingDropDown)
        //        return;

        //    var buildingId = buildingDropDown.options[buildingDropDown.selectedIndex].value;

        //    if (null == buildingId || 0 == buildingId.length)
        //        return; 
 
        //    //clear floor dropdown
        //    var floorDropDown = document.getElementById("floorDropDown");

        //    var length = floorDropDown.options.length;            

        //    for(i = length - 1 ; i >= 0 ; i--)
        //    {
        //        floorDropDown.remove(i);
        //    }

        //    var result = map.get(buildingId);

        //    var minMax = result.split(',');

        //    var minFloor = parseInt(minMax[0]);
        //    var maxFloor = parseInt(minMax[1]);
            
            

        //    for(i=minFloor;i<maxFloor+1;i++)
        //    {
        //        var objOption = document.createElement("option");
        //        objOption.text = i.toString()+ "층";
        //        objOption.value = i;

        //        floorDropDown.add(objOption);
        //    }
        //}        
        
    </script>

    <script>
        //The magic code to add show/hide custom event triggers
        (function ($) {
            $.each(['show', 'hide'], function (i, ev) {
                var el = $.fn[ev];
                $.fn[ev] = function () {
                    this.trigger(ev);
                    return el.apply(this, arguments);
                };
            });
        })(jQuery);

        $(function () {
            // 6 create an instance when the DOM is ready
            console.log("create jstree");
            $('#jstree_demo_div').jstree({
                'core': {
                    "check_callback": true,
                    
                },
                "plugins": ["search"]
            });
            var to = false;
            $('#serachSensorZone').keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = $('#serachSensorZone').val();
                    $('#jstree_demo_div').jstree(true).search(v);
                }, 250);
            });

            // 7 bind to events triggered on the tree
            $('#jstree_demo_div').on("changed.jstree", function (e, data) {
                console.log(data.selected);

                selectedNodeId = data.selected[0];
                var selectedNode = $('#jstree_demo_div').jstree(true).get_node(data.selected[0]);

                $('#serachSensorZone').val(selectedNode.text);
            });
        });
        
  </script>
    <%@ Import NameSpace="System.Data.SqlClient" %>
<%@ Import NameSpace="TrainingSystem" %>
    <script runat="server">
    public static string hello = "'hello'";

    </script>
</html>
