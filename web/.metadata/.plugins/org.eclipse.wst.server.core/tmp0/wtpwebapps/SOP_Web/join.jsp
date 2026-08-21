<%@ page language="java" contentType="text/html; charset=EUC-KR"
    pageEncoding="EUC-KR"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">    
<html>
<head>
<title>회원 등록</title>

<style type="text/css">
	table {border-collapse:collapse;}
	th {border-left:1px solid #999;border-bottom:1px solid #999;border-top:1px solid #999;background:#EAEAEA;padding:5px;font-size:9pt;width:200px;}
	td {border:1px solid #999; text-align:left;font-size:9pt;width:400px;}
	.inputBox {border:1px solid #999;font-size:9pt;background:#F2F2F2;}
	.tdRight {text-align:right;}
	input, select {margin:3px;}
	p {margin:0px;padding:0px;}
</style>

<script>
function check(formMembership)
{
	var UserName = formMembership.UserName.value;
	var UserID = formMembership.UserID.value;
	var UserPW = formMembership.UserPW.value;
	var MemberID = formMembership.MemberID.value;
	var MemberTeamID = formMembership.MemberTeamID.value;

	var forms = document.getElementById("formMembership");

	if ((forms.UserName.value == "")||(forms.UserName.value.length <= 1))
	{
		alert("이름을 입력하세요.");
		forms.UserName.focus();
        return false;
	}
	if(UserID.length == 0)
	{
		alert("아이디를 입력하세요.");
		forms.UserID.focus();
		return false;
	}
	if(UserPW.length == 0)
	{
		alert("비밀번호를 입력하세요.");
		forms.UserPW.focus();
		return false;
	} 
	if(MemberID.value=="")
	{
		alert("사원번호를 입력하세요.");
		forms.MemberID.focus();
        return false;
 	}
	if(MemberTeamID.length == 0)
	{
		alert("부서를 입력하세요.");
		forms.MemberTeamID.focus();
		return false;
	}


	return true;
}

function fncCheckInfo()
{			
	var UserName = formMembership.UserName.value;
	var UserID = formMembership.UserID.value;
	var UserPW = formMembership.UserPW.value;
	var MemberID = formMembership.MemberID.value;
	var MemberTeamID = formMembership.MemberTeamID.value;

	var forms = document.getElementById("formMembership");

	if ((forms.UserName.value == "")||(forms.UserName.value.length <= 1))
	{
		alert("이름을 입력하세요.");
		forms.name.focus();
        return false;
	}
	if(UserID.length == 0)
	{
		alert("아이디를 입력하세요.");
		forms.UserID.focus();
		return false;
	}
	if(UserPW.length == 0)
	{
		alert("비밀번호를 입력하세요.");
		forms.UserPW.focus();
		return false;
	} 
	if(forms.MemberID.value=="")
	{
		alert("사원번호를 입력하세요.");
		forms.MemberID.focus();
        return false;
 	}
	if(MemberTeamID.length == 0)
	{
		alert("부서를 입력하세요.");
		forms.MemberTeamID.focus();
		return false;
	}
	
	//document.formMembership.action="joinCheck.jsp";
	document.formMembership.action="test.jsp";
	document.formMembership.method="post";
	document.formMembership.submit();
}

function fncJoin()
{
	document.formMembership.action="joinSuccess.jsp";
	document.formMembership.method="post";
	document.formMembership.submit();
}

</script>
</head>

<body>
<h2 align="center">&nbsp;</h2>
<h2 align="center">&nbsp;</h2>
<h2 align="center">회원 등록 </h2>
<form name="formMembership" id = "formMembership" action="./joinCheck.jsp" method="post" onSubmit="return check(this.form)">
  <table width="489" align="center">
    <tr>
      <th scope="row">이름</th>
      <td><input type="text" name="UserName" class="inputBox" id="UserName"></td>
    </tr>
    <tr>
      <th scope="row">아이디</th>
      <td ><input type="text" name="UserID" class="inputBox" id="UserID"></td>
    </tr>
    <tr>
      <th scope="row">비밀번호</th>
      <td><input type="password" name="UserPW" class="inputBox" id="UserPW"></td>
    </tr>
    <tr>
      <th scope="row">사원번호</th>
      <td><input type="text" name="MemberID" class="inputBox" id="MemberID"></td>
    </tr>
    <tr>
      <th scope="row">부서</th>
      <td><input type="text" name="MemberTeamID" class="inputBox" id="MemberTeamID"></td>
    </tr>
    <tr>
      <td colspan="2" class="tdRight"> 
      	<input name="check" type="image" id="check" src="images/btn_check.gif" onClick="javascript:fncCheckInfo()">        
      	<input name="join" type="image" id="join" src="images/btn_join.gif" onClick="javascript:fncJoin()">
      </td>
    </tr>
  </table>
</form>
</body>
</html>