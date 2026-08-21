<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ page import="java.util.*" %>
<%@ page import="java.io.*" %>
<%@ page import="java.net.URLDecoder"%>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>download</title>
</head>
<body>
<%
 try{
  
  String file_path = "D://SOP/";
  String file_name = "namdong_inside_21.cmo";
  File file = new File(file_path + file_name);
  
  //page의 설정을 바꾸기 위해서 response를 다 날려버림 !
  response.reset();
  
  //setContentType는 MIME 타입을 지정. 캐릭터의 인코딩을 지정할 수도 있다.
  //octer-stream으로 지정시, 형식을 지정하지않겠다는 것 ! ex)msword를 첨부한다고치면 application/msword;
  response.setContentType("application/octer-stream");
  
  //브라우저 파일확장자를 포함하여 모든 확장자의 파일들에 대해 다운로드시 무조건 파일다운로드 대화상자가 뜨도록 하는 헤더속성 !
  response.setHeader("Content-Disposition", "attachment;filename="+file_name+"");
  
  //바이너리 데이터를 아스키 텍스트 형식으로 변환하기 위한 방법
  response.setHeader("Content-Treansper-Encoding", "binary");
  
  response.setContentLength((int)file.length());
  
  //cache에서 해당 페이지 읽기방지 ! 로딩시마다 새로고침한것 !
  response.setHeader("Pargma","no-cache");
  //cache 막기!
  response.setHeader("Expires","-1");
  
  byte[] data = new byte[1024 * 1024];
  BufferedInputStream fis = new BufferedInputStream(new FileInputStream(file));
  BufferedOutputStream fos = new BufferedOutputStream(response.getOutputStream());
  
  int count = 0;
  while((count = fis.read(data))!= -1){
   fos.write(data);
  }
  
  if(fis !=null) fis.close();
  if(fos != null) fos.close();
  
 }catch(Exception e){
  System.out.println("download error" + e);
 }

//﻿jsp에 내장객체로 out 사용 따라서 outputStream이 가능하다 !
out.clear();
out = pageContext.pushBody();

//<h4><a href="D://Installer/output/namdong_inside_01.cmo">1. File : inside</a></h4>
//<h4><a href="D://Installer/output/namdong_outside_01.cmo">2. File : outside</a></h4>

%>
</body>
</html> 
