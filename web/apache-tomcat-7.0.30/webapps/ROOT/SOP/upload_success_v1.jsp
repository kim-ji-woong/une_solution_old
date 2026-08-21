<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.io.*,java.util.*, javax.servlet.*" %>
<%@ page import="javax.servlet.http.*" %>
<%@ page import="org.apache.commons.fileupload.*" %>
<%@ page import="org.apache.commons.fileupload.disk.*" %>
<%@ page import="org.apache.commons.fileupload.servlet.*" %>
<%@ page import="org.apache.commons.fileupload.util.*" %>
<%@ page import="org.apache.commons.io.*" %>
<%@ page import="org.apache.commons.io.output.*" %>
<%
  	File file ;
   	int maxFileSize = 500 * 1024 * 1024;
   	int maxMemSize = 500 * 1024 * 1024;
   	String filePath = "C:\\Update_Src";
   	String contentType = request.getContentType();
   	
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
         	out.println("<html>");
         	out.println("<body>");
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
         		out.println("You are not UNE Admin."+ "<br>");
         	   	out.println("</body>");
         	    out.println("</html>");         	   	
         		return;         	
         	}   

			Iterator<FileItem> fileiterator = fileItems.iterator();
         	while ( fileiterator.hasNext () ) 
         	{
            	FileItem item = (FileItem)fileiterator.next();
            	if ( !item.isFormField () )  {
                	String fieldName = item.getFieldName();                	
                	String fileName = item.getName();  
					
					fileName = fileName.substring(fileName.lastIndexOf(File.separator) + 1);				
					uploadfilename = fileName;			
                	
                	boolean isInMemory = item.isInMemory();
                	long sizeInBytes = item.getSize();
                	file = new File( filePath, fileName);
					item.write( file ) ; 
					
					if(file.isFile() && file.exists()) {
						out.println("Upload Finished : " + uploadfilename);
						break;
					} else {
						out.println("Upload failed : " + uploadfilename + "can't upload");
						throw new Exception("can't upload");						
					}                      	
            	} 
         	}
         	
         	
         	out.println("</body>");
         	out.println("</html>");
			
      	}catch(Exception ex) {
        	out.println(ex.getMessage());	
					
     	}
	}else{
		out.println("<html>");
   	  	out.println("<body>");
      	out.println("<p>No file uploaded</p>"); 
      	out.println("</body>");
      	out.println("</html>");
   	}
%>
</body>
</html>
