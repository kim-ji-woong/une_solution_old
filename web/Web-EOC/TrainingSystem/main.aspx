<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="TrainingSystem.main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <meta charset="utf-8"/>
    <meta content="IE=edge" http-equiv="X-UA-Compatible" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
    <title>한국 지역난방공사</title>
    <link rel="stylesheet" href="./css/kdhc.css" />
    <!--
    <script src="https://code.jquery.com/jquery-3.0.0.js"></script>
    <script src="https://code.jquery.com/jquery-migrate-3.0.0.js"></script>-->
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>  
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>  
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" type="text/css" /> 
    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
    <![endif]-->
    <script src="./js/kdhc.js"></script>
     <script>
         window.onload = function () {
             writeCurrentDateTime();
             loadBroadcast();
             loadSms();
             loadSchedule();
         };

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
    </script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->    
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
      <div class="datetime" id="currentDateTime"></div>
    </div> <!--header-->
    <div class="container">
        <div class="gnb">
            <div><a href="status.aspx">현황</a></div>
            <div><a href="training.aspx">훈련</a></div>
            <div class="active"><a href="main.aspx">관리</a></div>
        </div><!-- gnb -->

        <div class="content admin-container">
          <div class="title-underline"><h2 class="">광교지사</h2></div>
          <div class="tab-menu">
            <ul class="clear">
              <li ><a href="#admin_broadcast">방송관리 </a></li>
              <li class="active"><a href="#admin_scheduled">일정관리</a></li>
              <li><a href="#admin_sms">문자관리</a></li>
            </ul>
          </div>
           <div class="tab_menu_target " id="admin_broadcast">
              <div class="con">
                <div class="box">
                  <h3 class="title-icon">화재 훈련 안내 방송</h3>                  
                  <div class="form-input"><textarea name="" id="fireTrainningBroadcastText" style="display: block;width: 100%;height: 300px;">hello world</textarea></div>
                  <div class="description">
                    <p>상황에 따라 내용이 정해지는 것들은 아래의 <span class="text-purple">특수문자 버튼을</span> 클릭하여 확인하세요.</p>
                    <p>&lt;&lt; >>내의 메시지는 반복되지 않음</p>
                    <p><a href="#" class="btn-black">특수문자</a></p>
                  </div>
                </div><!-- box -->
                <div class="box">
                  <h3 class="title-icon">누출 훈련 안내 방송 </h3>
                  <div class="form-input"><textarea name="" id="leakTrainningBroadcastText" style="display: block;width: 100%;height: 300px;">hello world</textarea></div>

                  <div class="broadcast_option">
                    <h3 class="title-icon">방송 송출 옵션 </h3>
                    <div class="con_wrap">
                        <div class="underline">
                          <input type="radio" value="0" id="radio_1" name="alertRepeat" class="radio" checked="checked" >
                          <label for="radio_1" class="active">반복없음</label>
                          <input type="radio" value="1" id="radio_2" name="alertRepeat" class="radio"  >
                          <label for="radio_2" class="active">1회 반복</label>
                          <input type="radio" value="2" id="radio_3" name="alertRepeat" class="radio"  >
                          <label for="radio_3" class="active">2회 반복</label>
                        </div>
                        <div>
                          <input type="checkbox" name="useSiren" value="" class="checkbox" id="useSiren">
                          <label for="checkbox_1" class="active">방송시작시 사이렌 사용</label>
                        </div>
                    </div>
                  </div><!-- broadcast_option-->

                </div><!-- box -->
              </div><!-- con -->

              <div class="btn-list">
                <a href="#" class="btn-blue" id="saveBroadcastMessage" onclick="saveBroadcastMessage();">저장</a>
                <a href="#" class="btn-blue">취소</a>
              </div>
           </div>
           <div class="tab_menu_target active" id="admin_scheduled">
              <div class="con">
                  <h3 class="title-icon">등록된 일정 목록  </h3>
                  <div class="table">
                    <table >
                        <thead>
                            <tr>
                                <th>일시</th>
                                <th colspan="2">일정내용</th>
                            </tr>
                        </thead>
                        <tbody id="scheduleBody">
                            <tr>
                                <td >2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr class="bg-gray">
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr>
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr class="bg-gray">
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr class="active">
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr>
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr class="bg-gray">
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr>
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr class="bg-gray">
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                            <tr class="active">
                                <td>2017-01-11 </td>
                                <td class="bbs-title" >안전 교육</td>
                                <td class="btn-delete"><a href=""><img src="images/icon_delete.png" /></a></td>
                            </tr>
                        </tbody>
                    </table>
                  </<div> <!-- table -->

                  </div>
              </div><!-- con -->

              <div class="btn-list">
                <a href="#" class="btn-blue" onclick="saveNewSchedule();" >저장</a>
                <a href="#" class="btn-blue">취소</a>
              </div>
           </div>
           <div class="tab_menu_target " id="admin_sms">
              <div class="con">
                <div class="box">
                  <h3 class="title-icon">화재 훈련 안내 문자</h3>
                    <div class="form-input"><textarea name="" id="fireTrainningSmsText" style="display: block;width: 100%;height: 300px;">hello world</textarea></div>                  
                  <div class="description">
                    <p>상황에 따라 내용이 정해지는 것들은 아래의 <span class="text-purple">특수문자 버튼을</span> 클릭하여 확인하세요.</p>
                    <p><< >>내의 메시지는 반복되지 않음</p>
                    <p><a href="#" class="btn-black ">특수문자</a></p>
                  </div>
                </div><!-- box -->
                <div class="box">
                  <h3 class="title-icon">누출 훈련 안내 문자 </h3>
                  <div class="form-input"><textarea name="" id="leakTrainningSmsText" style="display: block;width: 100%;height: 300px;">hello world</textarea></div>                  

                </div><!-- box -->
              </div>
              <div class="btn-list">
                <a href="#" class="btn-blue" onclick="saveSms();" >저장</a>
                <a href="#" class="btn-blue">취소</a>
              </div>
           </div>
        </div>



    </div> <!-- wrap -->
</div><!-- outline -->
      <form class="search-form clear" runat="server">  
          <asp:ScriptManager ID="ScriptManager2" runat="server" EnablePageMethods="true"/>     
      </form>

<!-- -->
<%@ Import NameSpace="MySql.Data.MySqlClient" %>
<script runat="server">
    
  int recordCount= 0; 
  void Page_Load( Object o, EventArgs e) {
     MySqlConnection dbConnection;
     dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
     dbConnection.Open();
     MySqlCommand cmd;
     cmd = new MySqlCommand("select count(*) as cnt from ActionStep", dbConnection);
     MySqlDataReader dr;
     dr=cmd.ExecuteReader();
     if(dr.Read()) {
         string result = dr[0].ToString();
       recordCount=int.Parse(result);
     }
     else {
       recordCount=0;
     }
     dr.Close();
     dbConnection.Close();
     DataBind();
  }  
</script>
      <script>
          function saveSms()
          {
              var fireBroadcastMessage = document.getElementById("fireTrainningSmsText").value;
              PageMethods.UpdateSmsTemplate(fireBroadcastMessage, "2", "3", onSucessUpdateFireSmsTemplate, onError); //화재

              var leakBroadcastMessage = document.getElementById("leakTrainningSmsText").value;
              PageMethods.UpdateSmsTemplate(leakBroadcastMessage, "3", "3", onSucessUpdateLeakSmsTemplate, onError); //화재
          }

          function onSucessUpdateFireSmsTemplate(result) {

          }

          function onSucessUpdateLeakSmsTemplate(result) {
              alert("문자 내용을 저장하였습니다.");
          }

          function saveBroadcastMessage()
          {
              var repeatCount = $(':radio[name="alertRepeat"]:checked').val();
              var useSiren = "";

              if ($('#useSiren').is(":checked")) {
                  useSiren = "TRUE";
              }
              else
                  useSiren = "FALSE";

              var fireBroadcastMessage = document.getElementById("fireTrainningBroadcastText").value;
              PageMethods.UpdateBroadcastTemplate(fireBroadcastMessage, useSiren, repeatCount, "2", "3", onSucessUpdateFireBroadcastTemplate, onError); //화재

              var leakBroadcastMessage = document.getElementById("leakTrainningBroadcastText").value;
              PageMethods.UpdateBroadcastTemplate(leakBroadcastMessage, useSiren, repeatCount, "3", "3", onSucessUpdateLeakBroadcastTemplate, onError); //화재
          }

          function onSucessUpdateFireBroadcastTemplate(result)
          {
              
          }

          function onSucessUpdateLeakBroadcastTemplate(result)
          {
              alert("방송 내용을 저장하였습니다.");
          }

          function loadSms() {
              PageMethods.GetSmsTemplate("3", "2", onSuccessGetFireSmsTemplate, onError); //화재
              PageMethods.GetSmsTemplate("3", "3", onSuccessGetLeakSmsTemplate, onError); //유출
          }

          function onSuccessGetFireSmsTemplate(result) {
              var fireTrainningSmsText = document.getElementById("fireTrainningSmsText");
              fireTrainningSmsText.value = result;
          }

          function onSuccessGetLeakSmsTemplate(result) {
              var leakTrainningSmsText = document.getElementById("leakTrainningSmsText");
              leakTrainningSmsText.value = result;
          }

          function loadBroadcast()
          {
              PageMethods.GetBroadcastTemplate("3", "2", onSucessGetFireBroadcastTemplate, onError); //화재
              PageMethods.GetBroadcastTemplate("3", "3", onSucessGetLeakBroadcastTemplate, onError); //유출
          }          

          function onSucessGetFireBroadcastTemplate(result)
          {
              var broadcastElements = result.split("###");

              if(broadcastElements.length == 3)
              {
                  var text = broadcastElements[0];
                  var useSiren = broadcastElements[1];
                  var repeadCount = broadcastElements[2];
              }

              var fireTrainningBroadcastText = document.getElementById("fireTrainningBroadcastText");
              fireTrainningBroadcastText.value = text;
          }

          function onSucessGetLeakBroadcastTemplate(result) {
              var broadcastElements = result.split("###");

              if (broadcastElements.length == 3) {
                  var text = broadcastElements[0];
                  var useSiren = broadcastElements[1];
                  var repeadCount = broadcastElements[2];
              }

              var leakTrainningBroadcastText = document.getElementById("leakTrainningBroadcastText");
              leakTrainningBroadcastText.value = text;
          }

          function deleteSchedule(scheduleId)
          {
              if (confirm('일정을 삭제하시겠습니까?')) {
                  // delete it!
                  PageMethods.DeleteTrainningSchedule(scheduleId, onSuccessDeleteTrainningSchedule,onError);

                  
              } else {
                  // Do nothing!
              }
          }

          function onSuccessDeleteTrainningSchedule(result)
          {
              //refresh schedule list
              loadSchedule();
          }

          function loadSchedule()
          {
              PageMethods.GetTrainningScheduleList(onSuccessGetTrainningScheduleList,onError);
          }

          function onSuccessGetTrainningScheduleList(result)
          {
              var scheduleBody = document.getElementById("scheduleBody");

              scheduleBody.innerHTML = result;

              console.log(result);
          }

          function addScheduleDate(clickedTd)
          {

          }

          function showDatePicker()
          {
              $('#datepicker1').datepicker({ dateFormat: 'yy-mm-dd' });
              $('#datepicker1').datepicker('show');
          }

          function saveNewSchedule()
          {
              var scheduleData = document.getElementById("datepicker1");

              var date = scheduleData.value;

              var text = document.getElementById("newScheduleText").value;

              if (confirm('일정을 저장하시겠습니까?'))
              {
                  if (date.length == 0 || text.length == 0)
                  {
                      alert('데이터를 입력해주세요.')
                  }
                  else
                  {
                      PageMethods.SaveNewSchedule("3", date, text, onSuccessSaveNewSchedule, onError);
                  }                    
              }              
          }

          function onSuccessSaveNewSchedule(result)
          {
              //refresh schedule list
              loadSchedule();
          }
  
          function onError(result) {
              alert('Cannot process your request at the moment, please try later.');
          }
      </script>
</body>
</html>
