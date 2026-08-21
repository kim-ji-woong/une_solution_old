<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ page import="javax.naming.*"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
<%@ page import="sun.misc.*"%>
<%@page import="org.slf4j.Logger"%>
<%@page import="org.slf4j.LoggerFactory"%>
<%@page import="kr.co.unes.aqm.dto.site.Site" %>
<%@page import="kr.co.unes.aqm.dto.area.*" %>
<%
    Logger logger = LoggerFactory.getLogger(getClass());
	logger.info("JSP Begin");
	
	Map<String, Object> model = (Map<String, Object>)request.getAttribute("model");
	List<AreaDepth1> useList = (List<AreaDepth1>)model.get("UseArea");
	List<Site> locationList = (List<Site>)model.get("LocationList");
	String activeName = (String)model.get("Active");
	if( activeName == null)
	{
		if(useList != null && useList.size() > 0 )
		{ 
			activeName = useList.get(0).getDetph();
		}	
		else
		{
			activeName= "";
		}
	}
%>
<!DOCTYPE html>
<html lang=en>
<head>
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>
    <%@include file="../inc/include.jsp"%>
    <script>
    function tabMove(item)
    {
    	$("#tabname").val(item.innerText);
    	$("#formTab").formMethod = "post";
    	$("#formTab").submit();    	
    }
    </script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
    <div id="wrap">
        <%@include file="../inc/logo.jsp"%>
        <div class="container">
            <%@include file="../inc/gnb.jsp"%>
            <!-- gnb -->
            <div class="content">
                <div class="breadcrumbs breadcrumbs_no_margin">
                   홈 > <strong>학교관리</strong>
                </div>
                <div class="well">
                    <div class="admin_sub_menu">
                      <h2 class="title_admin">지역별 등록학교</h2>
                    	<%@include file="../inc/admin_menu.jsp"%>
                    </div>
                    <div class="admin_info">
                        <form id="formTab" 
                        	action="${pageContext.request.contextPath}/Admin/manage"
                        	method="POST">
                        	
                            <input id="tabname" name="tabname" type="hidden" value=""/>
                            <ul class="tab_menu">
                            <% if(useList != null ){                             	
                           			for(int i = 0 ; i < useList.size(); i++)
	                            	{
	                            		AreaDepth1 area = useList.get(i);
	                            		String szName = area.getDetph();
	                            		
	                            		if( szName.equals(activeName) )
	                            		{
	                            %>
	                            			<li onclick="tabMove(this);return false;" class="active"><a href=""><%= szName %></a></li>                            			
	                            <%
	                            		}
	                            		else
	                            		{
	                         	%>                            			
	                            			<li onclick="tabMove(this);return false;"><a href=""><%= szName %></a></li>
	                            <%
	                            		}
	                            	}
                              	} else {%>
                             <% } %>
                            </ul>
                            <div class="box">
                                <ul class="list">
                                <% if(locationList != null ){                             	
                           			for(int i = 0 ; i < locationList.size(); i++)
	                            	{
                           				Site loc = locationList.get(i);
	                            	
                         		%>
                           			 	<li><a href="${pageContext.request.contextPath}/Admin/manage/detail/<%= loc.getID()%>"><%= loc.getName()%></a></li>
                        		<%
	                            	}
                                }
                           		%>                                    
                                </ul>
                            </div>
                            <div class="btn_submit">
                                <a href="${pageContext.request.contextPath}/Admin/manage/new" class="btn_add" ><img src="${pageContext.request.contextPath}/images/btn_info_add.png" /></a>
                                <!--   a href="" class="btn_add" onclick="return item_remove()"><img src="${pageContext.request.contextPath}/images/btn_info_remove.png" /></a-->
                            </div>
                        </form>
                    </div>
                </div><!-- well -->
                 <%@include file="../inc/footer.jsp"%>
                 <!-- footer -->
            </div><!-- content -->
        </div> <!-- container -->
    </div><!-- wrap -->

</body>
<script>
function item_remove(){
    var item_i = $('.admin_info .list li.active');

    if(item_i.length == 0){
        alert('삭제할 학교를 하나이상 선택해주세요');
        return false;
    }

    var message ='등록된 학교를 삭제하시겠습니까?';
    if(confirm(message)){

        return true;
    }
    return false;
}
$(document).on('click','.admin_info .list li', function(){
    $(this).toggleClass('active');
  });
  
$(document).on('click','.admin_info .tab_menu li', function(){
    $(this).toggleClass('active');
  });
</script>
</html>
