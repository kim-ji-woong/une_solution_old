<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
 
<%@page import="com.oreilly.servlet.MultipartRequest" %>
<%@page import="com.oreilly.servlet.multipart.DefaultFileRenamePolicy" %>
<%@page import="java.io.*" %>
<%@page import="java.util.*" %>

<%
  	
   	int maxFileSize = 500 * 1024 * 1024;
   	int maxMemSize = 500 * 1024 * 1024;
   	String filePath = "C:\\Update_Src";
	String tempPath = request.getRealPath("upload");
   	String encType = "utf-8";

	String uploadfilename = "";
	try{ 
		
		out.println("<html>");
		out.println("<body>");
		
		boolean isAdmin = false;
		boolean isPassCorrect = false;
		
		MultipartRequest multipartRequest = new MultipartRequest(request, tempPath, maxFileSize, encType, new DefaultFileRenamePolicy());
		
		String id = multipartRequest.getParameter("ID");
		String pass = multipartRequest.getParameter("PASS");
		
		if(id.equals("sa")) {				
			isAdmin = true;				     
			out.println("OK ID");
		}
		
		if(pass.equals("9449966Ab")) {
			isPassCorrect = true;
			out.println("OK PASS");
		}        
		
		if(!(isAdmin && isPassCorrect)) {
			out.println("You are not UNE Admin."+ "<br>");
			out.println("</body>");
			out.println("</html>");         	   	
			return;         	
		}         	
		out.println("1");
		
		Enumeration files = multipartRequest.getFileNames();
        
        while (files.hasMoreElements()) {
 
            
            String name = (String) files.nextElement();
 
            
            String filename = multipartRequest.getFilesystemName(name);
 
            
            String original = multipartRequest.getOriginalFileName(name);
 
            
            String type = multipartRequest.getContentType(name);
 
            
            File file = multipartRequest.getFile(name);
 
            out.println("파라미터 이름 : " + name + "<br/>");
            out.println("실제 파일 이름 : " + original + "<br/>");
            out.println("저장된 파일 이름 : " + filename + "<br/>");
            out.println("파일 타입 : " + type + "<br/>");
 
            if (file != null) {
                out.println("크기 : " + file.length());
                out.println("<br/>");
            }
        }
		out.println("2");
		out.println("Upload Finished");
		out.println("</body>");
		out.println("</html>");
	}catch(Exception ex) {
		out.println(ex.getMessage());			
	}
	
%>
</body>
</html>
