<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="status.aspx.cs" Inherits="TrainingSystem.status" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>한국 지역난방공사</title>
    <link rel="stylesheet" href="./css/kdhc.css">
    <script src="https://code.jquery.com/jquery-3.0.0.js"></script>
    <script src="https://code.jquery.com/jquery-migrate-3.0.0.js"></script>
    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
    <![endif]-->
    <script src="./js/kdhc.js"></script>
    <script>
        window.onload = function () {
            init();
            showSchedule();
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
            <div class="range active">
            <h2 class="dropdown">전체</h2>
            <ul>
              <li><a href="">전체</a></li>
              <li><a href="">지역</a></li>
            </ul>
          </div>
          <div class="datetime" id="currentDateTime"></div>
        </div>
        <div class="container">
            <div class="gnb">
                <div class="active"><a href="status.aspx">현황</a></div>
                <div><a href="training.aspx">훈련</a></div>
                <div><a href="main.aspx">관리</a></div>
            </div><!-- gnb -->

            <div class="content status-container">
                <div class="areas">
                  <a href="status_sub.aspx" class="item">
                    <div class="name">고양</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item active">
                    <div class="name">광교</div>
                    <div class="icon">정상</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">광주전남</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">김해</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">대구</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">분당</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">삼송</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">강남</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">중앙</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">세종</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">수원</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">양산</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">용인</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">청주</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">파주</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">판교</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">화성</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>

                  <a href="status_sub.aspx" class="item">
                    <div class="name">동탄</div>
                    <div class="icon status-block ">통신단절</div>
                  </a>
                </div><!-- areas -->

                <div class="recent_bbs">
                    <div class="tab-menu">
                      <ul class="clear">
                        <li class="active"><a href="#recent_traning">최근 훈련 이력 </a></li>
                        <li><a href="#recent_disaster">최근 재난 이력 </a></li>
                        <li><a href="#calendar">주요일정</a></li>
                      </ul>
                    </div>
                     <div class="tab_menu_target active" id="recent_traning">
                        <ul class="list-dot">
                            <!--
                            <li><a href="" class="btn_popup" target="_blank">[2017-1-2]  분당 <span class="small">- 제어실 화재</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="#popup" class="btn_popup">[2016-12-1] 광교 <span class="small">- 염산가스누출</span></a></li>--> <!-- a href="" 영역의 값의 ID를 보여준다 -->
                            <!--
                            <li><a href="">[2016-11-2] 수원 <span class="small">- 암모니아수 누출</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2017-1-2]  분당 <span class="small">- 제어실 화재</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-12-1] 광교 <span class="small">- 염산가스누출</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-11-2] 수원 <span class="small">- 암모니아수 누출</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            -->
                            <% showRecentTrainingHistory(); %>
                        </ul>
                     </div>
                     <div class="tab_menu_target" id="recent_disaster">
                        <ul class="list-dot">
                            <!--
                            <li><a href="">[2017-1-2]  최근 재난 이력 <span class="small">- 제어실 화재</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-12-1] 최근 재난 이력 <span class="small">- 염산가스누출</span> <span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-11-2] 최근 재난 이력 <span class="small">- 암모니아수 누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2017-1-2]  최근 재난 이력 <span class="small">- 제어실 화재</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-12-1] 최근 재난 이력 <span class="small">- 염산가스누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-11-2] 최근 재난 이력 <span class="small">- 암모니아수 누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            -->
                            <% showRecentSensorHistory(); %>
                        </ul>
                     </div>
                     <div class="tab_menu_target" id="calendar">
                        <ul class="list-dot" id="calendarList">
                            <!--
                            <li><a href="">[2017-1-2]  주요일정 <span class="small">- 제어실 화재</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-12-1] 주요일정 <span class="small">- 염산가스누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-11-2] 주요일정 <span class="small">- 암모니아수 누출</span><<span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2017-1-2]  주요일정 <span class="small">- 제어실 화재</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-12-1] 주요일정 <span class="small">- 염산가스누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            <li><a href="">[2016-11-2] 주요일정 <span class="small">- 암모니아수 누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                            -->                            
                        </ul>
                     </div>

                </div><!-- recent_bbs -->
            </div>


  <div class="popup" id="popupDisaster">
    <a href="#" class="btn_popup_close" onclick="closeDisasterPopup();"><img src="./images/btn_popup_close.png" width="13" height="12" alt="닫기" /></a>
    <div class="message ">
        <h2 id="siteName">광교지사</h2>
        <div class="con">
          <h3>[ <span class="text-red" id="disasterType">"재난종류"</span> 상황 발생]</h3>
          <p id="disasterTime">- 2017-01-23   14:23:24</p>          
        </div>
    </div> <!-- message -->
  </div> <!-- popup -->

</div> <!-- wrap -->
</div><!-- outline -->
<form  runat="server">     
    <asp:ScriptManager ID="ScriptManager2" runat="server" EnablePageMethods="true"/>      
</form>
        <script>
            var popupDistasterId = "";

            function init()
            {
                writeCurrentDateTime();
                $('#popupDisaster').hide();
                setInterval(checkDisaster, 3000)
            }

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
            }

            function closeDisasterPopup()
            {
                //DB에 Detection을 했다는 표시를 한다.
                //이후로는 같은 재난으로 팝업이 뜨지 않는다.
                PageMethods.UpdateActionHistoryConfirm(popupDistasterId);

                $('#popupDisaster').hide();
            }

            function checkDisaster()
            {
                PageMethods.CheckDisaster(onSuccessCheckDisaster, onError);
            }

            function onSuccessCheckDisaster(result)
            {
                if ($('#popupDisaster').css('display') == 'none') {
                    //hidden

                    if (result.length > 0) {
                        var disasterElements = result.split("###");

                        if (4 == disasterElements.length)
                        {
                            popupDistasterId = disasterElements[3];

                            var disasterTime = document.getElementById("disasterTime");

                            disasterTime.innerHTML = disasterElements[0];

                            var disasterType = document.getElementById("disasterType");

                            disasterType.innerHTML = disasterElements[1];

                            var siteName = document.getElementById("siteName");

                            siteName.innerHTML = disasterElements[2];

                            $('#popupDisaster').show();
                        }                        
                    }
                }
                else
                {
                    //do nothing
                }                
            }

            function showSchedule()
            {
                PageMethods.GetScheduleList(onSuccessGetScheduleList,onError);
            }

            function onSuccessGetScheduleList(result)
            {
                var calendarList = document.getElementById("calendarList");

                calendarList.innerHTML = result;
            }

            function onError(result) {
                alert('Cannot process your request at the moment, please try later.');
            }
        </script>
<%@ Import NameSpace="MySql.Data.MySqlClient" %>    
        <script runat="server">
        protected void showSchedule()  
        {
                MySqlConnection dbConnection;
                dbConnection = new MySqlConnection("server=127.0.0.1;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");

                dbConnection.Open();

                string MySqlCommand = @"SELECT ID
      ,Schedule
      ,TimeStamp
      ,SiteID
  FROM  sop_3.publicschedule
  WHERE SiteID = 3 
limit 5";

                MySqlCommand cmdRecentSchedule = new MySqlCommand(MySqlCommand, dbConnection);
            
                MySqlDataReader dr;
                dr = cmdRecentSchedule.ExecuteReader();
            
                while (dr.Read())
                {
                    Response.Write("<li><a href=''>[" + ((DateTime)dr["CreationTime"]).ToShortDateString() + "] " + dr["SiteName"].ToString() + "<span class='small'>- " + dr["Schedule"].ToString() + "</span><span class='file'><img src='images/icon_file.png'></span></a></li>");
                }

                dr.Close();
                dbConnection.Close();
                //DataBind();         
            }

        protected void showRecentTrainingHistory()
        {
            MySqlConnection dbConnection;
            dbConnection = new MySqlConnection("server=127.0.0.1;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");

            dbConnection.Open();
            MySqlCommand cmdRecentDisaster;

            string sqlCommandForTraining = @"Select ash.ID, ash.ActionStepID, Site.SiteName, ash.RealMode, ash.BeginTime, ash.EndTime, ash.CancelTime, ash.DetectTime, dis.DisasterName, ash.SelectedComponentID, ash.SelectedComponentType, ash.StartOption, ash.Description, ash.DisasterOption from ActionStepHistory as ash
INNER JOIN ActionStep as step on step.ID = ash.ActionStepID 
INNER JOIN Disaster as dis on step.DisasterID = dis.ID
INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID
INNER JOIN DisasterCategory as dc on dc.ID = sdc.DisasterID AND dc.SiteID = 3
INNER JOIN Site on Site.ID = dc.SiteID
where ash.RealMode = 0
ORDER BY ash.DetectTime DESC";

            cmdRecentDisaster = new MySqlCommand(sqlCommandForTraining, dbConnection);
            
            MySqlDataReader dr;
            dr = cmdRecentDisaster.ExecuteReader();

            while (dr.Read())
            {
                string status = "";

                //Response.Write("<br><a href=''>[" + ((DateTime)dr["DetectTime"]).ToShortDateString() + "]" + " " + status + " " + dr["DisasterName"].ToString());
                //<li><a href="">[2016-11-2] 최근 재난 이력 <span class="small">- 암모니아수 누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                Response.Write("<li><a href=''>[" + ((DateTime)dr["BeginTime"]).ToShortDateString() + "] " + dr["SiteName"].ToString() + 
                    status  + "<span class='small'>- " + dr["DisasterName"].ToString() + "</span><span class='file'><img src='images/icon_file.png'></span></a></li>");
            }

            dr.Close();
            dbConnection.Close();
            DataBind();
        }

        protected void showRecentSensorHistory()
        {
            

            string sqlCommandForSensorHistory = @"Select ash.ID, ash.ActionStepID, Site.SiteName, ash.RealMode, ash.BeginTime, ash.EndTime, ash.CancelTime, ash.DetectTime, dis.DisasterName, ash.SelectedComponentID, ash.SelectedComponentType, ash.StartOption, ash.Description, ash.DisasterOption from ActionStepHistory as ash
INNER JOIN ActionStep as step on step.ID = ash.ActionStepID 
INNER JOIN Disaster as dis on step.DisasterID = dis.ID
INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID
INNER JOIN DisasterCategory as dc on dc.ID = sdc.DisasterID AND dc.SiteID = 3
INNER JOIN Site on Site.ID = dc.SiteID
where ash.RealMode = 1
ORDER BY ash.DetectTime DESC";

            MySqlConnection dbConnection;
            dbConnection = new MySqlConnection("server=127.0.0.1;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");

            dbConnection.Open();
            MySqlCommand cmdRecentSensorHistory;
            
            cmdRecentSensorHistory = new MySqlCommand(sqlCommandForSensorHistory, dbConnection);

            MySqlDataReader dr;
            dr = cmdRecentSensorHistory.ExecuteReader();

            while (dr.Read())
            {
                string status = "";

                //Response.Write("<br><a href=''>[" + ((DateTime)dr["DetectTime"]).ToShortDateString() + "]" + " " + status + " " + dr["DisasterName"].ToString());
                //<li><a href="">[2016-11-2] 최근 재난 이력 <span class="small">- 암모니아수 누출</span><span class="file"><img src="images/icon_file.png"></span></a></li>
                Response.Write("<li><a href=''>[" + ((DateTime)dr["DetectTime"]).ToShortDateString() + "] " + dr["SiteName"].ToString() +
                    status + "<span class='small'>- " + dr["DisasterName"].ToString() + "</span><span class='file'><img src='images/icon_file.png'></span></a></li>");
            }

            dr.Close();
            dbConnection.Close();
            DataBind();
        }
        
</script>      
</body>
</html>
