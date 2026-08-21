<%@ page language="java" contentType="text/html; charset=utf-8" pageEncoding="utf-8"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="ui" uri="http://egovframework.gov/ctl/ui"%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/functions" prefix="fn" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" >
<meta http-equiv="content-language" content="ko">
<title>기상 종합조회 시스템 - 로그인</title>
<link href="<c:url value='/'/>css/login.css" rel="stylesheet" type="text/css" >
<!--<link href="<c:url value='/'/>css/common.css" rel="stylesheet" type="text/css" >
<link href="<c:url value='/'/>css/login.css" rel="stylesheet" type="text/css" >-->
<script type="text/javascript">
<!--
function actionLogin() {

    if (document.loginForm.id.value =="") {
        alert("아이디를 입력하세요");
        return false;
    } else if (document.loginForm.password.value =="") {
        alert("비밀번호를 입력하세요");
        return false;
    } else {
        document.loginForm.action="<c:url value='/actionLogin.do'/>";
        //document.loginForm.j_username.value = document.loginForm.userSe.value + document.loginForm.username.value;
        //document.loginForm.action="<c:url value='/j_spring_security_check'/>";
        document.loginForm.submit();
    }
}

function setCookie (name, value, expires) {
    document.cookie = name + "=" + escape (value) + "; path=/; expires=" + expires.toGMTString();
}

function getCookie(Name) {
    var search = Name + "=";
    if (document.cookie.length > 0) { // 쿠키가 설정되어 있다면
        offset = document.cookie.indexOf(search);
        if (offset != -1) { // 쿠키가 존재하면
            offset += search.length;
            // set index of beginning of value
            end = document.cookie.indexOf(";", offset);
            // 쿠키 값의 마지막 위치 인덱스 번호 설정
            if (end == -1)
                end = document.cookie.length;
            return unescape(document.cookie.substring(offset, end));
        }
    }
    return "";
}

function saveid(form) {
    var expdate = new Date();
    // 기본적으로 30일동안 기억하게 함. 일수를 조절하려면 * 30에서 숫자를 조절하면 됨
    if (form.checkId.checked)
        expdate.setTime(expdate.getTime() + 1000 * 3600 * 24 * 30); // 30일
    else
        expdate.setTime(expdate.getTime() - 1); // 쿠키 삭제조건
    setCookie("saveid", form.id.value, expdate);
}

function getid(form) {
    form.checkId.checked = ((form.id.value = getCookie("saveid")) != "");
}

function fnInit() {
	//alert('g1Login fnInit');
    var message = document.loginForm.message.value;
    if (message != "") {
        alert(message);
        document.loginForm.message.value = "";
    }
    getid(document.loginForm);
    
    // go back으로 이 Page에 못들어오게 한다.
    window.history.forward();
}
//-->
</script>
</head>
<body  onload="fnInit();">
<!-- 전체 레이어 시작 -->
<div id="wrap">
    <!-- header 시작 -->
    <div id="header_mainsize"><c:import url="/EgovPageLink.do?link=main/inc/EgovIncHeader" /></div>
    <div id="topnavi"><c:import url="/EgovPageLink.do?link=main/inc/EgovIncTopnav" /></div>
    <!-- //header 끝 -->
    <!-- container 시작 -->
    <div id="container">
        <!-- //좌측메뉴 끝 -->
            <!-- content 시작 --> 
            <div id="content">
                <!-- 타이틀 이미지 -->            
                <div id="content_img_div"><img  alt="로그인 하세요." src="<c:url value='/'/>images/login/img_subtitle_login.gif" width="776" height="230" /></div>       
                    <div class="user_login">
                            <form:form name="loginForm" method="post" action="#LINK">
                            <div class="user_login_ultop">
                                <ul>
                                    <li>
                                        <label for="id"><img alt="login" src="<c:url value='/'/>images/login/img_idtext.gif" /></label>
                                        <input type="text" class="input_style" title="아이디를 입력하세요." id="id" name="id" maxlength="16"/>
                                    </li>
                                    <li>
                                        <label for="password"><img alt="password" src="<c:url value='/'/>images/login/img_pwtext.gif" /></label>
                                        <input type="password" class="input_style" maxlength="25" title="비밀번호를 입력하세요." id="password" name="password" 
                                               onkeydown="javascript:if (event.keyCode == 13) { actionLogin(); }"/>
                                    </li>
                                    <li>
                                        <input type="checkbox" name="checkId" onclick="javascript:saveid(this.form);" id="checkId" /><label for="checkId">ID저장</label>
                                    </li>
                                </ul>
                                <input type="image" alt="로그인 버튼" class="btn_style" onclick="javascript:actionLogin()" src="<c:url value='/'/>images/login/btn_login.gif"  />
                            </div>
                            <input type="hidden" name="message" value="${message}" />
				            <input type="hidden" name="userID"  value="${userID}"/>
				            <input type="hidden" name="userGrade"  value="${userGrade}"/>
				            <!-- <input type="hidden" name="j_username" />-->
                            </form:form>
                            <div class="text_area">
                                <ul>
                                    <li>비밀번호는 6~12자의 영문 대/소문자, 숫자, 특수문자를 혼합해서 사용하실 수 있습니다.</li>
                                    <li>쉬운 비밀번호나 자주 쓰는 사이트의 비밀번호가 같을 경우, 도용되기 쉬우므로 주기적으로 변경하셔서 사용하는 것이 좋습니다.</li>
                                </ul>
                            </div>
                    </div>
            </div>                      
            <!-- //content 끝 -->    
    </div>  
    <!-- //container 끝 -->
    <!-- footer 시작 -->
    <div id="footer"><c:import url="/EgovPageLink.do?link=main/inc/EgovIncFooter" /></div>
    <!-- //footer 끝 -->
</div>
<!-- //전체 레이어 끝 -->
</body>
</html>
