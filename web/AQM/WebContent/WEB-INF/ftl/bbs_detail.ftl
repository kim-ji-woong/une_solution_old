<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=en>

<head>
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>
    <#include "inc/include.ftl">
</head>

<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
    <div id="wrap">
        <#include "inc/logo.ftl">
        <div class="container">
            <#include "inc/gnb.ftl">
            <!-- gnb -->
            <div class="content">
                <div class="breadcrumbs breadcrumbs_no_margin">
                   홈 > <strong>안내사항</strong>
                </div>
                <div class="well">
                	<h2 class="title_bbs">안내사항</h2>
                    <div class="admin_sub_menu">
                      <#if Post ??>	    
                      <h2 class="title_admin">${Post.title}</h2>
                      <#else>
                      <h2 class="title_admin"></h2>
                      </#if>
                    </div>
                    <div class="bbs_detail" >
                        <table>
                          <thead>
                              <tr>
                                  <th width="73px" class="num">작성일</th>
                                  <#if Post ??>	    
			                      <td >${Post.timeStamp?date?iso_utc}   ${Post.timeStamp?time}</td>
			                      <#else>
			                      <td ></td>
			                      </#if>                                  
                                  <th width="53px" >조회</th>
                                  <td width="173px" >${Post.readCount}</td>
                              </tr>
                          </thead>
                          <tbody>
                              <tr>
                              	<#if Post ??>	    
			                      <td colspan="6" class="description">${Post.convertContent}</td>
			                      <#else>
			                      <td colspan="6" class="description"></td>
			                      </#if>
                              </tr>
                          </tbody>
                      </table>
                      <div class="btn_submit">                      	
                        <a href="${Context.contextPath}/Info"><img src="${Context.contextPath}/images/icon_bbs_list.png" alt="목록" width="66" height="35" /></a>
                      </div>
                    </div>
                </div><!-- well -->
                <#include "inc/footer.ftl">
                <!-- footer -->
            </div><!-- content -->
        </div> <!-- container -->
    </div><!-- wrap -->
</body>
</html>