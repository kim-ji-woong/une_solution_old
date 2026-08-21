<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.io.*,java.util.*, javax.servlet.*" %>
<%@ page import="javax.servlet.http.*" %>
<%@ page import="org.apache.commons.fileupload.*" %>
<%@ page import="org.apache.commons.fileupload.disk.*" %>
<%@ page import="org.apache.commons.fileupload.servlet.*" %>
<%@ page import="org.apache.commons.fileupload.util.*" %>
<%@ page import="org.apache.commons.io.*" %>
<%@ page import="org.apache.commons.io.output.*" %>
<%@ page import="org.json.simple.JSONObject" %>

<%


	//Result [Code : 결과]
	//  { code : int, message : "" }	
	//
	// 100 : 파일 업로드 성공 message : {
	//		filename : "",     (string)  --- 업로드된 파일 이름.
	//      size : 121231 (int)
	// }
	// 110 : multipart/form-data 형식이 아닙니다. 폼 데이터 형식으로 전송해야 합니다.
	// 120 : ID 또는 PASS워드가 안 맞음
	// 130 : File이 upload 되지 않음		
	// 140 : File이 클라이언트로부터 전달되지 않음.
	//  -1 : Unknown Error;
	
  	File file ;
   	
   	int maxMemSize = 500 * 1024 * 1024;
	int maxFileSize = 500 * 1024 * 1024;
	
   	String filePath = "C:\\Update_Src";
   	String contentType = request.getContentType();
   	String resultFileName = "";
	long fileSize = 0;
	String errorMessage = "";
	int customResultCode = -1;
	boolean isIncludedFile = false;
   	if ((contentType.indexOf("multipart/form-data") >= 0)) {

		DiskFileItemFactory factory = new DiskFileItemFactory();
      	ServletFileUpload upload = new ServletFileUpload(factory);
		upload.setSizeMax( maxFileSize );
      	File tempdir = new File("C:\\temp"); 
      	if (!tempdir.isDirectory()) { tempdir.mkdirs(); }
      	
      	String uploadfilename = "";
      	try{ 
         	List<FileItem> fileItems = upload.parseRequest(request);
         	Iterator<FileItem> itrerator = fileItems.iterator();         	
         	boolean isAdmin = false;
         	boolean isPassCorrect = false;
         	while (itrerator.hasNext () ) 
         	{
            	FileItem item = (FileItem)itrerator.next();
            	if ( item.isFormField () )  {
            		String name = item.getFieldName();
            	    String value = item.getString();
            	    if(name.equals("ID")) {
           	    		if(value.equals("sa")) {
           	    			isAdmin = true;
           	    		}            	    	
            	    }
            	    if(name.equals("PASS")) {
           	    		if(value.equals("9449966Ab")) {
           	    			isPassCorrect = true;
           	    		}            	    	
            	    }                	
            	} 
         	}
         	
         	if(!(isAdmin && isPassCorrect)) {
         		customResultCode = 120;      	   	
         		throw new Exception("Access Denied ! : id, pass 확인 해주세요.");						        	
         	}   

			Iterator<FileItem> fileiterator = fileItems.iterator();
			
         	while ( fileiterator.hasNext () ) 
         	{
            	FileItem item = (FileItem)fileiterator.next();
            	if ( !item.isFormField () )  {
					isIncludedFile = true;
                	String fieldName = item.getFieldName();                	
                	String fileName = item.getName();  
					
					fileName = fileName.substring(fileName.lastIndexOf(File.separator) + 1);				
					uploadfilename = fileName;			
                	
                	boolean isInMemory = item.isInMemory();
                	long sizeInBytes = item.getSize();
                	
					file = new File( filePath, fileName);
					 
					InputStream instream = item.getInputStream();
					FileOutputStream fout = new FileOutputStream(file);
					
					int read = 0;
					byte[] buf = new byte[1024];
					
					while((read=instream.read(buf,0,buf.length))!=-1){
						fout.write(buf, 0, read);
					}					
					
					instream.close();
					fout.close();
					
					if(file.isFile() && file.exists()) {
						//out.println("Upload Finished : " + uploadfilename);
						customResultCode = 100;
						resultFileName = uploadfilename;
						fileSize =  file.length();
						break;			//File 한개에 대해서만 처리.
					} else {
						//out.println("Upload failed : " + uploadfilename + "can't upload");
						customResultCode = 130;
						throw new Exception("Upload Process is Failed: 파일 이름 [" + uploadfilename+ "] 업로드에 실패한 이유는 해당 디렉토리의 접근 권한이나 경로에 문제가 발생했을 가능성이 있습니다. ");						
					}                      	
            	} 
         	}
			
      	} catch(Exception ex) {
			
			errorMessage = ex.getMessage();
			
     	} finally {
			
			JSONObject resultJson = new JSONObject();
			resultJson.put("code", new Integer(customResultCode));			
			if(customResultCode == 100) {
				JSONObject messageJson = new JSONObject();
				messageJson.put("filename", resultFileName);	
				messageJson.put("size", new Long(fileSize));	
				resultJson.put("message", messageJson);
			}
			//else if ((customResultCode == -1) && (!isIncludedFile)) {
			//	resultJson.put("code", new Integer(140));			//전달된 파일이 없음.			
			//}
			else {
				resultJson.put("message", errorMessage);	
			}
			out.print(resultJson.toJSONString());
			out.flush();
		}
		
	} else {
		
		customResultCode = 110;
		JSONObject resultJson = new JSONObject();
		resultJson.put("code", new Integer(customResultCode));			
		resultJson.put("message", "It's not type of [multipart/form-data]");	
		out.print(resultJson);
		out.flush();
		
   	}
%>

