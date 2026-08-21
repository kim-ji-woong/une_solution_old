<%@ page contentType="text/html; charset=UTF-8"%>
<%@ page import="java.io.*,java.util.*, javax.servlet.*" %>
<%@ page import="javax.servlet.http.*" %>
<%@ page import="org.apache.commons.fileupload.*" %>
<%@ page import="org.apache.commons.fileupload.disk.*" %>
<%@ page import="org.apache.commons.fileupload.servlet.*" %>
<%@ page import="org.apache.commons.io.output.*" %>

<%
  	File file ;
   	int maxFileSize = 500 * 1024 * 1024;
   	int maxMemSize = 500 * 1024 * 1024;
   	String filePath = "C://Update_Src";
   	String contentType = request.getContentType();
   	
   	if ((contentType.indexOf("multipart/form-data") >= 0)) {

      	DiskFileItemFactory factory = new DiskFileItemFactory();
      	factory.setSizeThreshold(maxMemSize);
      	File tempdir = new File("c:\\temp"); 
      	if (!tempdir.isDirectory()) { tempdir.mkdirs(); }
      	
      	factory.setRepository(new File("c:\\temp"));
      	ServletFileUpload upload = new ServletFileUpload(factory);
      	upload.setSizeMax( maxFileSize );
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
           	    		if(value.equals("admin")) {
           	    			isAdmin = true;
           	    		}            	    	
            	    }
            	    if(name.equals("PASS")) {
           	    		if(value.equals("une1234qwer")) {
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
            	FileItem fi = (FileItem)fileiterator.next();
            	if ( !fi.isFormField () )  {
                	String fieldName = fi.getFieldName();                	
                	String fileName = fi.getName();                	
                	uploadfilename= fileName;
                	
                	boolean isInMemory = fi.isInMemory();
                	long sizeInBytes = fi.getSize();
                	file = new File( filePath+"\\"+fileName) ;
                	fi.write( file ) ;      
                	
            	} 
         	}
         	out.println("Upload Finished : " + uploadfilename + "<br>");
         	out.println("</body>");
         	out.println("</html>");
      	}catch(Exception ex) {
        	System.out.println(ex);
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
