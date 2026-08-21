package kr.co.unes.aqm.view;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.file.Paths;
import java.util.LinkedHashMap;
import java.util.Map;

import javax.activation.MimetypesFileTypeMap;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import javax.ws.rs.Consumes;
import javax.ws.rs.DefaultValue;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.WebApplicationException;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.StreamingOutput;
import javax.ws.rs.core.Response.Status;

import org.glassfish.jersey.media.multipart.FormDataContentDisposition;
import org.glassfish.jersey.media.multipart.FormDataParam;
import org.glassfish.jersey.server.mvc.Viewable;
import org.slf4j.LoggerFactory;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import kr.co.unes.aqm.dto.site.NodeLinkImageMap;
import kr.co.unes.aqm.model.NodeImageMapDataAccessManager;
import kr.co.unes.aqm.servlet.AQMLoginManager;

@Path("/Map")
public class AdminMapMultipartJjspView {
	
	@Context
	private HttpServletRequest request;
	
	@Context
	private HttpServletResponse response;


	private final org.slf4j.Logger logger = LoggerFactory.getLogger(AdminMapMultipartJjspView.class);	
	private NodeImageMapDataAccessManager manager = new NodeImageMapDataAccessManager();
			
	private boolean checkLogin()
	{
		HttpSession session = request.getSession();
		AQMLoginManager manager = AQMLoginManager.getInstance();	
		return manager.checkLogin(session);
	}
	
	public Response rejectUpload()
	{		
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		Viewable view = new Viewable("/common/reject_upload.ftl", map);
		return Response.ok(view).build();
	}	
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/get/{uuid}")
	public Response getDownloadMapFile(@PathParam("uuid") String uuid)
	{
		if(uuid != null)
		{
			final NodeLinkImageMap file = manager.getImageMap(uuid);			
			if( file != null)
			{	
				String szHeader = "attachment; filename = " + file.getFileName();
				StreamingOutput fileStream =  new StreamingOutput() 
		        {
		            @Override
		            public void write(java.io.OutputStream output) throws IOException, WebApplicationException 
		            {
		                try
		                {		    
		                	byte[] data = file.getFileContent();
		                    output.write(data);
		                    output.flush();
		                } 
		                catch (Exception e) 
		                {
		                    throw new WebApplicationException("File Not Found !!");
		                }
		            }
		        };
		        return Response
		                .ok(fileStream, MediaType.APPLICATION_OCTET_STREAM)
		                .header("content-disposition", szHeader)
		                .build();	
			}			
		}
		return Response.status(Status.NOT_FOUND).build();
	}
	
	@POST
	@Consumes("multipart/form-data; charset=UTF-8")
	@Produces("application/json; charset=UTF-8")
	@Path("/new")
	public Response attachImageMapJson( 
			@FormDataParam("file") InputStream aUploadedInputStream,
	        @FormDataParam("file") FormDataContentDisposition fileMetaData) 
	{	
		if(!checkLogin())
			return rejectUpload();	
		
		return uploadMapFile(aUploadedInputStream, fileMetaData);
	}
	
	@SuppressWarnings("deprecation")
	private Response uploadMapFile(InputStream aUploadedInputStream, FormDataContentDisposition fileMetaData)
	{
		String szMimeType = "";		
		File uploadFile = null;
		
		String szResultJson = "{\"Upload\":{\"UploadInfo\":[{}]}, \"Result\":-1}";
		
		String szUploadPath = request.getRealPath("/");		
		szUploadPath = szUploadPath + "\\..\\upload";
		java.nio.file.Path path = Paths.get(szUploadPath);
		
		logger.debug("Local Path : " + path.toAbsolutePath());
		logger.debug("Upload Info");
	    try
	    {	 
	    	// Multipart에 포함된 FileName은  ISO-8859-1
			String szFileName = new String(fileMetaData.getFileName().getBytes("ISO-8859-1"), "UTF-8");
	    	String szUUID = java.util.UUID.randomUUID().toString().replaceAll("-","");	  
	    	 
	    	File f = new File(path.toAbsolutePath().toString());
	    	if( f.exists() == false)
	    	{
	    		f.mkdir();
	    	}  
	    	uploadFile = new File(f.getPath() + "\\"+ szUUID);	
	    	uploadFile.deleteOnExit();

	    	logger.debug("File Upload Location : " + uploadFile);			
	    	        	        
	    	int read = 0;
	        byte[] bytes = new byte[1024];	 
	        OutputStream out = new FileOutputStream(uploadFile);
	        while ((read = aUploadedInputStream.read(bytes)) != -1) 
	        {
	            out.write(bytes, 0, read);
	        }
	        out.flush();
	        out.close();
	        
	       
	        String ctxPath = request.getContextPath();
			String szFileURL = ctxPath + "/Map/get/"+szUUID;	             
	   			
			
			szMimeType = new MimetypesFileTypeMap().getContentType(uploadFile);
			
			byte[] fileByte = new byte[(int) uploadFile.length()];
			InputStream in = new FileInputStream(uploadFile);
			in.read(fileByte, 0, (int)uploadFile.length());			
		    in.close();
		    
		    NodeLinkImageMap file = new NodeLinkImageMap();
			file.setFileContent(fileByte);
			file.setFileName(szFileName);
			file.setMimeType(szMimeType);
			file.setLinkNode(false);
			file.setUuid(szUUID);
			file.setFileSize((int)uploadFile.length());
			file.setDescritpion("");
			file.setUrl(szFileURL);
			// 2 file 업로드 , 1이면 image업로드
			file.setUploadType(1);
			
			manager.saveUploadImage(file);
			
			Map<String, Object> map = new LinkedHashMap<String, Object>();
			map.put("attachurl", szFileURL);
			map.put("filemime", szMimeType);
			map.put("filename", szFileName);
			map.put("fileid", file.getId());
			
			long nFileSize = ( uploadFile.length() / 8);			
			map.put("filesize", "" + nFileSize);
		
			logger.debug("File Server URL : " + szFileURL);
			logger.debug("File Mime Type : " + szMimeType);
			logger.debug("File Size : " + nFileSize + "B");
			
			Gson gson = new Gson();
			Map<String, Object> result = new LinkedHashMap<String, Object>();
			result.put("UploadInfo", map);	
			JsonElement el = gson.toJsonTree(result);		
			JsonObject obj = new JsonObject ();
			obj.add("Upload", el);
			obj.addProperty("Result", 1);
			szResultJson = gson.toJson(obj);			
		} 
		catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
}
