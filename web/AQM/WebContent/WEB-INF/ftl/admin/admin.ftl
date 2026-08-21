<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=en>

<head>
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>
    <#include "../inc/include.ftl">
</head>

<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
    <div id="wrap">
        <#include "../inc/logo.ftl">
        <div class="container">
            <#include "../inc/gnb.ftl">
            <div class="content">
                <div class="breadcrumbs breadcrumbs_no_margin">
                   홈 > <strong>관리자</strong> 
                </div>
                <div class="well">
                    <div class="admin_menu admin_main">
                        <ul>
                            <li class="information"><a href="${Context.contextPath}/Admin/manage">학교등록관리</a></li>
                            <li class="notice"><a href="${Context.contextPath}/Admin/posts">안내사항관리</a></li>
                            <li class="logout"><a href="${Context.contextPath}/Admin/logout">로그아웃</a></li>
                        </ul>
                    </div>
                </div>
                <#include "../inc/footer.ftl">
            </div><!-- content -->
        </div> <!-- container -->
    </div><!-- wrap -->

</body>

</html>
