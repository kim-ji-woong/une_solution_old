<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="status_sub.aspx.cs" Inherits="TrainingSystem.sustus_sub" %>

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
        </div>
        <div class="container">
            <div class="gnb">
                <div class="active"><a href="status.aspx">현황</a></div>
                <div><a href="training.aspx">훈련</a></div>
                <div><a href="main.aspx">관리</a></div>
            </div><!-- gnb -->

            <div class="content status-container">
                  <div class="area_detail">
                      <h3 class="area-title">광교지사</h3>
                      <div class="thumbnail">
                        <img src="images/sample/status_detail.png" />
                      </div>

                      <div class="area_bbs">
                        <h3 class="sub-title">훈련 및 대응이력</h3>
                        <div >
                            <div class="tab-menu">
                              <ul class="clear">
                                <li class="active"><a href="#recent_traning">훈련실시</a></li>
                                <li><a href="#recent_disaster">재난발생</a></li>
                              </ul>
                            </div><!-- tab-menu-->
                           <div class="tab_menu_target active" id="recent_traning">
                              <ul class="list-dot">
                                  <% writeRecentTrainingList(); %>
                                  <!-- 
                                      <li><a href="" class="btn_popup" >[2017-1-2]  광교  <span class="small">- ㅇㅇ재난발생</span>
                                   <span class="file"><img src="images/icon_file.png"></span></a></li>
                                  -->
                              </ul>
                           </div>
                           <div class="tab_menu_target" id="recent_disaster">
                              <ul class="list-dot">
                                  <%writeRecentLeakList(); %>
                                  <!--
                                  <li><a href="" class="btn_popup" >[2017-1-2]  광교  <span class="small">- ㅇㅇ화재훈련</span>
                                   <span class="file"><img src="images/icon_file.png"></span></a></li>                                  
                                  -->
                              </ul>
                           </div>
                        </div><!-- recent_bbs-->
                      </div><!-- area_bbs-->
                  </div><!-- area_detail-->

                  <div class="area_report">
                      <div class="box" id="leakReport">
                          <h3 class="sub-title">누출탐지 분석</h3>
                          <div class="report_summary clear">
                              <div class="report_graph "  id="report_graph" data-percent="<% Response.Write((1.0 - getLeakPercent(2017,7,3)).ToString()); %>" ><!-- data-percent="60"  이부분에  1을 기준으로하는 소수점을 입력해주세요. -->
                                <div class="graph_value">
                                  <p class="txt"><% Response.Write(((int)totalLeakCount).ToString()); %></p>
                                  <p><span class="label">누출발생</span> <span class="text-red"><% Response.Write(((int)totalLeakNormalCount).ToString()); %></span></p>
                                  <p><span class="label">오작동</span>  <span class="text-green"><% Response.Write(((int)leakMalfunctionCount).ToString()); %></span></p>
                                </div>
                              </div><!-- report_graph-->

                              <div class="report_summary_info">
                                  <div class="box-red">
                                      <p>누출발생</p>
                                      <p class="important"><% Response.Write(((int)totalLeakNormalCount  ).ToString()); %>건 <span class="small">(<%Response.Write(Math.Round(totalLeakNormalPercent,1).ToString()); %>%)</span></p>
                                  </div>
                                  <div class="box-green">
                                      <p>오작동</p>
                                      <p class="important"><% Response.Write(((int)leakMalfunctionCount).ToString()); %>건<span class="small">(<%Response.Write(Math.Round(leakMalfunctionPercent,1).ToString()); %>%)</span></p>
                                  </div>
                              </div><!-- report_graph-->
                          </div><!-- report_summary -->
                      </div><!-- box -->

                      <div class="box">
                          <div class="report_detail">
                              <ul>
                                  <% writeLeakDetailList(2017, 7,3); %>
                              </ul>
                          </div><!-- report_detail -->
                      </div><!-- box -->

                  </div><!-- area_detail-->


            </div>
        </div> <!-- wrap -->
    </div><!-- outline -->
    </div>
<script src="js/progressbar.min.js"></script>

<script>
    $(document).ready(function () {


        var bar = new ProgressBar.Circle(report_graph, {
            strokeWidth: 6,
            easing: 'easeInOut',
            duration: 1400,
            color: '#0bd1a5',
            trailColor: '#f66252',
            trailWidth: 6,
            svgStyle: null
        });
        var txt = parseFloat($('#report_graph').attr('data-percent'));
        bar.animate(txt);  // Number from 0.0 to 1.0
    });

    $('#fireReport').hide();

    setInterval(writeCurrentDateTime, 1000);

    function writeCurrentDateTime()
    {
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
</script>
</body>
<%@ Import NameSpace="MySql.Data.MySqlClient" %>
<%@ Import NameSpace="TrainingSystem" %>
<script runat="server">

    //한달 현황정보를 가져온다.
    //누출 탐지 총 개수
    float totalLeakCount = 0;
    float leakMalfunctionCount = 0;
    float totalLeakNormalCount = 0;
    float leakMalfunctionPercent = 0.0f;
    float totalLeakNormalPercent = 0.0f;
    
    //화재 정보
    float totalFireCount = 0;
    float fireMalfunctionCount = 0;
    float totalFireNormalCount = 0;
    float fireMalfunctionPercent = 0.0f;
    float totalFireNormalPercent = 0.0f;
    
    
    //실제 누출 발생 비율
    private float getLeakPercent(int year,int month,int siteId)
    {
        totalLeakCount = getLeakCount(year, month, siteId);
        leakMalfunctionCount = getLeakMalfunctionCount(year, month,siteId);
        totalLeakNormalCount = totalLeakCount - leakMalfunctionCount;

        float percent = totalLeakNormalCount / totalLeakCount;

        leakMalfunctionPercent = leakMalfunctionCount / totalLeakCount * 100.0f;
        totalLeakNormalPercent = totalLeakNormalCount / totalLeakCount * 100.0f;

        return percent;
    }

    //실제 화재 발생 비율
    private float getFirePercent(int year, int month, int siteId)
    {
        totalFireCount = getfireCount(year, month, siteId);
        fireMalfunctionCount = getFireMalfunctionCount(year, month, siteId);
        totalFireNormalCount = totalFireCount - fireMalfunctionCount;

        float percent = totalFireNormalCount / totalFireCount;

        fireMalfunctionPercent = fireMalfunctionCount / totalFireCount * 100.0f;
        totalFireNormalPercent = totalFireNormalCount / totalFireCount * 100.0f;

        return percent;
    }

    private int getfireCount(int year, int month, int siteId)
    {
        int recordCount = 0;

        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;

        string messageSqlString = @"SELECT  srh.ID,pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
from SensorReactionHistory as srh
INNER JOIN SensorZoneHistory as szh on szh.id = srh.SensorHistoryID
INNER JOIN SensorZone as sz on szh.SensorID = sz.ID
INNER JOIN EquipmentZone as ez on ez.ID = sz.EquipZoneID
INNER JOIN PSMSensor as ps on ps.EquipZoneID = ez.ID
INNER JOIN PSMMaterial as pm on pm.ID = ps.MaterialType
INNER JOIN Site as st on ez.SiteID = st.ID
WHERE (srh.ReactionType =60 and st.ID = " + siteId.ToString() + @" AND MONTH(srh.Time) = " + month.ToString() + @" and YEAR(srh.Time) = " + year.ToString() + @")
GROUP BY srh.ID, pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
ORDER BY srh.ID DESC";



        //        string sqlString = @"SELECT DISTINCT COUNT(a.ID) as cnt
        //FROM  sop_3.SensorReactionHistory a
        //WHERE (a.ReactionType =60 and MONTH(a.Time) = " + month.ToString() + " and YEAR(a.Time) = " + year.ToString() + ")";

        cmd = new MySqlCommand(messageSqlString, dbConnection);
        MySqlDataReader dr;
        dr = cmd.ExecuteReader();

        int counter = 0;

        while (dr.Read())
        {
            //recordCount = int.Parse(dr["cnt"].ToString());
            counter++;
        }

        recordCount = counter;


        dr.Close();
        dbConnection.Close();
        DataBind();

        return recordCount;
    }
    
    private int getLeakCount(int year,int month,int siteId)
    {
        int recordCount = 0;

        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;

        string messageSqlString = @"SELECT  srh.ID,pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
from SensorReactionHistory as srh
INNER JOIN SensorZoneHistory as szh on szh.id = srh.SensorHistoryID
INNER JOIN SensorZone as sz on szh.SensorID = sz.ID
INNER JOIN EquipmentZone as ez on ez.ID = sz.EquipZoneID
INNER JOIN PSMSensor as ps on ps.EquipZoneID = ez.ID
INNER JOIN PSMMaterial as pm on pm.ID = ps.MaterialType
INNER JOIN Site as st on ez.SiteID = st.ID
WHERE (srh.ReactionType =60 and st.ID = " + siteId.ToString() + @" AND MONTH(srh.Time) = " + month.ToString() + @" and YEAR(srh.Time) = " + year.ToString() + @")
GROUP BY srh.ID, pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
ORDER BY srh.ID DESC";
        
        

//        string sqlString = @"SELECT DISTINCT COUNT(a.ID) as cnt
//FROM  sop_3.SensorReactionHistory a
//WHERE (a.ReactionType =60 and MONTH(a.Time) = " + month.ToString() + " and YEAR(a.Time) = " + year.ToString() + ")";

        cmd = new MySqlCommand(messageSqlString, dbConnection);
        MySqlDataReader dr;
        dr = cmd.ExecuteReader();
        
        int counter = 0;
        
        while (dr.Read())
        {
            //recordCount = int.Parse(dr["cnt"].ToString());
            counter++;
        }

        recordCount = counter;
        

        dr.Close();
        dbConnection.Close();
        DataBind();

        return recordCount;
    }

    private int getFireMalfunctionCount(int year, int month, int siteId)
    {
        int recordCount = 0;

        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;

        string sqlString = @"SELECT COUNT(*) as cnt
FROM  sop_3.SensorReactionHistory a, sop_3.SensorReactionHistory b
WHERE (a.ReactionType =60 and MONTH(a.Time) = " + month.ToString() + " and YEAR(a.Time) = " + year.ToString() + ") and (a.SensorHistoryID = b.SensorHistoryID and b.ReactionType = 21) ";

        cmd = new MySqlCommand(sqlString, dbConnection);
        MySqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            recordCount = int.Parse(dr["cnt"].ToString());
        }

        dr.Close();
        dbConnection.Close();
        DataBind();

        return recordCount;
    }  
    
    //누출 탐지중 오류로 취소된 갯수
    private int getLeakMalfunctionCount(int year,int month,int siteId)
    {
        int recordCount = 0;
        
        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;
        
        string sqlString = @"SELECT COUNT(*) as cnt
FROM  sop_3.SensorReactionHistory a, sop_3.SensorReactionHistory b
WHERE (a.ReactionType =0 and MONTH(a.Time) = " + month.ToString() + " and YEAR(a.Time) = " + year.ToString() +") and (a.SensorHistoryID = b.SensorHistoryID and b.ReactionType = 21) ";

        cmd = new MySqlCommand(sqlString, dbConnection);
        MySqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            recordCount = int.Parse(dr["cnt"].ToString());
        }
        
        dr.Close();
        dbConnection.Close();
        DataBind();

        return recordCount;
    }

    private void writeFireDetailList(int year, int month, int siteId)
    {
        string messageSqlString = @"SELECT  srh.ID,pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
from SensorReactionHistory as srh
INNER JOIN SensorZoneHistory as szh on szh.id = srh.SensorHistoryID
INNER JOIN SensorZone as sz on szh.SensorID = sz.ID
INNER JOIN EquipmentZone as ez on ez.ID = sz.EquipZoneID
INNER JOIN PSMSensor as ps on ps.EquipZoneID = ez.ID
INNER JOIN PSMMaterial as pm on pm.ID = ps.MaterialType
INNER JOIN Site as st on ez.SiteID = st.ID
WHERE (srh.ReactionType =0 and st.ID = " + siteId.ToString() + @" AND MONTH(srh.Time) = " + month.ToString() + @" and YEAR(srh.Time) = " + year.ToString() + @")
GROUP BY srh.ID, pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
ORDER BY srh.ID DESC";

        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;
        cmd = new MySqlCommand(messageSqlString, dbConnection);

        MySqlDataReader dr;
        dr = cmd.ExecuteReader();


        Dictionary<String, int> leakTypeMap = new Dictionary<string, int>();

        string[] splitter = { "@@@" };

        while (dr.Read())
        {
            string materialName = (string)dr["MaterialName"];

            string zoneName = (string)dr["BroadcastName"];

            string siteName = (string)dr["ShortName"];

            string zoneNameLeakCombo = zoneName + splitter[0] + materialName + splitter[0] + siteName;

            if (!leakTypeMap.ContainsKey(zoneNameLeakCombo))
            {
                leakTypeMap.Add(zoneNameLeakCombo, 1);
            }
            else
            {
                leakTypeMap[zoneNameLeakCombo] += 1;
            }
        }

        int maxCount = 0;

        foreach (KeyValuePair<string, int> pair in leakTypeMap)
        {
            maxCount = Math.Max(maxCount, pair.Value);
        }

        foreach (KeyValuePair<string, int> pair in leakTypeMap)
        {
            string[] splittedString = pair.Key.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            if (3 == splittedString.Length) //SiteName,ZoneName,LeakType
            {
                string zoneName = splittedString[0];
                string leakType = splittedString[1];
                string siteName = splittedString[2];

                int width = pair.Value * 100 / maxCount;

                string listString = @"<li>
<span class='label'>" + siteName + @"/" + zoneName + @"/" + leakType + @"</span>
            <span class='bar'><small style='width: " + width.ToString() + @"%'></small></span>
            <span class='txt'>" + pair.Value.ToString() + @"</span>
</li>";
                Response.Write(listString);
            }
        }

        //<li>
        //                            <span class="label">건물/구역/물질</span>
        //                            <span class="bar"><small style="width: 100%"></small></span>
        //                            <span class="txt">10</span>
        //                          </li>
    }    
   
    //누출 건물/구역/물질 리스트
    private void writeLeakDetailList(int year,int month,int siteId)
    {
        string messageSqlString = @"SELECT  srh.ID,pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
from SensorReactionHistory as srh
INNER JOIN SensorZoneHistory as szh on szh.id = srh.SensorHistoryID
INNER JOIN SensorZone as sz on szh.SensorID = sz.ID
INNER JOIN EquipmentZone as ez on ez.ID = sz.EquipZoneID
INNER JOIN PSMSensor as ps on ps.EquipZoneID = ez.ID
INNER JOIN PSMMaterial as pm on pm.ID = ps.MaterialType
INNER JOIN Site as st on ez.SiteID = st.ID
WHERE (srh.ReactionType =60 and st.ID = " + siteId.ToString() + @" AND MONTH(srh.Time) = " + month.ToString() + @" and YEAR(srh.Time) = " + year.ToString() + @")
GROUP BY srh.ID, pm.MaterialName,ez.BroadcastName,st.SiteName, st.ShortName
ORDER BY srh.ID DESC";

        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;
        cmd = new MySqlCommand(messageSqlString, dbConnection);
        
        MySqlDataReader dr;
        dr = cmd.ExecuteReader();


        Dictionary<String, int> leakTypeMap = new Dictionary<string, int>();

        string[] splitter = { "@@@" };
        
        while (dr.Read())
        {
            string materialName = (string)dr["MaterialName"];

            string zoneName = (string)dr["BroadcastName"];

            string siteName = (string)dr["ShortName"];

            string zoneNameLeakCombo = zoneName + splitter[0] + materialName + splitter[0] + siteName;

            if (!leakTypeMap.ContainsKey(zoneNameLeakCombo))
            {
                leakTypeMap.Add(zoneNameLeakCombo, 1);
            }
            else
            {
                leakTypeMap[zoneNameLeakCombo] += 1;
            }
        }

        int maxCount = 0;
        
        foreach (KeyValuePair<string, int> pair in leakTypeMap)
        {
            maxCount = Math.Max(maxCount, pair.Value);
        }
        
        foreach(KeyValuePair<string,int> pair in leakTypeMap)
        {
            string[] splittedString = pair.Key.Split(splitter,  StringSplitOptions.RemoveEmptyEntries);          
            
            if(3 == splittedString.Length) //SiteName,ZoneName,LeakType
            {
                string zoneName = splittedString[0];
                string leakType = splittedString[1];
                string siteName = splittedString[2];                
                
                int width = pair.Value * 100 / maxCount;
                
                string listString = @"<li>
<span class='label'>" + siteName + @"/" + zoneName + @"/" + leakType + @"</span>
            <span class='bar'><small style='width: " + width.ToString() + @"%'></small></span>
            <span class='txt'>" + pair.Value.ToString() + @"</span>
</li>";
                Response.Write(listString);
            }            
        }        
        
        //<li>
        //                            <span class="label">건물/구역/물질</span>
        //                            <span class="bar"><small style="width: 100%"></small></span>
        //                            <span class="txt">10</span>
        //                          </li>
    }
    
    private void writeRecentLeakList()
    {
        Dictionary<int, DisasterStep> disasterMap = new Dictionary<int, DisasterStep>();

        string queryString = @"SELECT  *  FROM SensorReactionHistory 
ORDER BY Time DESC limit 100";
        
        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;
        cmd = new MySqlCommand(queryString, dbConnection);

        MySqlDataReader dr;
        dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            int sensorHistoryId = (int)dr["SensorHistoryID"];
            TrainingSystem.DisasterStep.ReactionType reactionType = (TrainingSystem.DisasterStep.ReactionType)(int)dr["ReactionType"];
            string message = (string)dr["Message"];
            
            if(disasterMap.ContainsKey(sensorHistoryId))
            {
                disasterMap[sensorHistoryId].addReaction(reactionType,message);
            }
            else
            {
                DisasterStep ds = new DisasterStep();

                ds.Id = sensorHistoryId;
                ds.Time = ((DateTime)dr["Time"]).ToShortDateString();
                ds.addReaction(reactionType,message);
                
                disasterMap.Add(sensorHistoryId,ds);
            }
        }     
        
        foreach(KeyValuePair<int,DisasterStep> ds in disasterMap)
        {
            Response.Write(@"<li><a href='' class='btn_popup' >[" + ds.Value.Time + @"]  광교  <span class='small'>-" + ds.Value.getStatusMessage() + @" </span>
                                   <span class='file'><img src='images/icon_file.png'></span></a></li>");            
        }

        dr.Close();
        dbConnection.Close(); 
    }
    
    private void writeRecentTrainingList()
    {
        string queryString = @"SELECT acsh.BeginTime, dis.DisasterName
  FROM  sop_3.ActionStepHistory as acsh
  INNER JOIN  sop_3.ActionStep as acs on acs.ID = acsh.ActionStepID
  INNER JOIN  sop_3.Disaster as dis on dis.ID = acs.DisasterID
  where acsh.RealMode = 0  
  ORDER BY acsh.BeginTime DESC
    limit 5";

        MySqlConnection dbConnection;
        dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
        dbConnection.Open();
        MySqlCommand cmd;
        cmd = new MySqlCommand(queryString, dbConnection);

        MySqlDataReader dr;
        dr = cmd.ExecuteReader();

        
        while (dr.Read())
        {
            Response.Write(@"<li><a href='' class='btn_popup' >[" + ((DateTime)dr["BeginTime"]).ToShortDateString() + @"]  광교  <span class='small'>-"
            + dr["DisasterName"].ToString() + @"</span><span class='file'><img src='images/icon_file.png'></span></a></li>");
        }

        dr.Close();
        dbConnection.Close();

    }
    //화재 건물/구역 리스트
   
     
</script>
</html>
