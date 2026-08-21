package kr.co.unes.aqm.view;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.nio.file.DirectoryStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.Map;

import javax.activation.MimetypesFileTypeMap;
import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;
import javax.ws.rs.Consumes;
import javax.ws.rs.DefaultValue;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.QueryParam;
import javax.ws.rs.WebApplicationException;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;
import javax.ws.rs.core.StreamingOutput;

import org.glassfish.jersey.media.multipart.FormDataContentDisposition;
import org.glassfish.jersey.media.multipart.FormDataParam;
import org.glassfish.jersey.server.mvc.Viewable;

import org.slf4j.LoggerFactory;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import kr.co.unes.aqm.dto.SensorValue;
import kr.co.unes.aqm.dto.post.AttachFile;
import kr.co.unes.aqm.dto.site.NodeLinkImageMap;
import kr.co.unes.aqm.dto.site.NodeLocation;
import kr.co.unes.aqm.dto.site.Site;
import kr.co.unes.aqm.model.AttachedFileDataAccessManager;
import kr.co.unes.aqm.model.NodeImageMapDataAccessManager;
import kr.co.unes.aqm.model.NodeLocationDataAccessManager;
import kr.co.unes.aqm.model.SensorDataAccessManager;
import kr.co.unes.aqm.model.SiteDataAccessManager;
import kr.co.unes.aqm.servlet.AQMLoginManager;

import org.apache.poi.*;
import org.apache.poi.common.*;
import org.apache.poi.hssf.usermodel.HSSFClientAnchor;
import org.apache.poi.hssf.usermodel.HSSFPatriarch;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.DateUtil;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.xssf.usermodel.XSSFCell;
import org.apache.poi.xssf.usermodel.XSSFClientAnchor;
import org.apache.poi.xssf.usermodel.XSSFDrawing;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;

@Path("/File")
public class MultiPartFtlView {

	@Context
	private HttpServletRequest request;
	

	private AttachedFileDataAccessManager manager = new AttachedFileDataAccessManager();
	
	private final org.slf4j.Logger logger = LoggerFactory.getLogger(AdminPostFtlView.class);
	
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
	@Path("/new")
	public Response attachFileView()
	{
		if(!checkLogin())
			return rejectUpload();
				
		String ftlDir = "/admin/file_upload.ftl";
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		Viewable view = new Viewable(ftlDir, map);		
		return Response.ok(view).build();
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/get/{uuid}")
	public Response getDownloadFile(@PathParam("uuid") String szUUID)
	{
		if(szUUID != null)
		{
			final AttachFile file = manager.getAttachedFile(szUUID);
			String szHeader = "attachment; filename = " + file.getFileName();
			if( file != null)
			{
				
				StreamingOutput fileStream =  new StreamingOutput() 
		        {
		            @Override
		            public void write(java.io.OutputStream output) throws IOException, WebApplicationException 
		            {
		                try
		                {		    
		                	//String szUploadPath = request.getRealPath("/");		
		    				//szUploadPath = szUploadPath + "\\..\\upload\\" + file.getUuid();		    				
		                	//java.nio.file.Path uploadPath = Paths.get(szUploadPath);		                	
		                    //byte[] data = Files.readAllBytes(uploadPath);
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
	
	//private XSSFWorkbook createExcelReport(int nNodeID)
	//{
		
	//}
	
	@SuppressWarnings({ "unused", "deprecation" })
	private void AddImage(String url, XSSFWorkbook wb, XSSFSheet sheet) throws Exception
	{
		try
		{
			InputStream inputStream = new URL(url).openStream();			
			byte[] bytes = org.apache.poi.util.IOUtils.toByteArray(inputStream);
			int pictureIdx = wb.addPicture(bytes, org.apache.poi.hssf.usermodel.HSSFWorkbook.PICTURE_TYPE_PNG);
			inputStream.close();
			
	        XSSFDrawing patriarch = sheet.createDrawingPatriarch();
	        XSSFClientAnchor anchor = new XSSFClientAnchor(0,0,0,255,(short)1,12,(short)13,23); // 이미지 크기조절은 여기서..
	        anchor.setAnchorType( 2 );
	        patriarch.createPicture(anchor, pictureIdx); // 삽입 할 이미지
		}
		catch (Exception e)
		{
		}
	}
	
	public Date getYesterday ( Date today ) {
		 if ( today == null ) throw new IllegalStateException ( "today is null" );
		 Date yesterday = new Date ( );
		 yesterday.setTime ( today.getTime ( ) - ( (long) 1000 * 60 * 60 * 24 ) );
		 return yesterday; 
	}
	
	public Date getNextday ( Date today ) {
		 if ( today == null ) throw new IllegalStateException ( "today is null" );
		 Date yesterday = new Date ( );
		 yesterday.setTime ( today.getTime ( ) + ( (long) 1000 * 60 * 60 * 24 ) );
		 return yesterday; 
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/report/excel/{netNode}")
	public Response getDownloadExcel(
			@DefaultValue("-1") @PathParam("netNode") int nNode,
			@QueryParam("From") String szFormDate,
			@QueryParam("To") String szToDate) throws Exception
	{
		if(nNode > 0)
		{
			ServletContext ctx = request.getServletContext();
			String szFilePath = ctx.getRealPath("/WEB-INF/report.xlsx");
			final File file = new File(szFilePath);
			
			logger.debug("Local Path : " + file.getAbsolutePath());
			
	        XSSFWorkbook wb = null;
	        
			try 
			{
				//엑셀 파일 오픈
				wb = new XSSFWorkbook(new FileInputStream(file));
			} catch (FileNotFoundException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			}
						
			XSSFSheet sheet = wb.getSheetAt(0);	

			NodeLocationDataAccessManager nlm = new NodeLocationDataAccessManager();
			NodeLocation nl = nlm.getNodeLocationByNetNodeID(nNode);
			try 
			{
				String ctxPath = ctx.getContextPath();
				
				if( nl != null)
				{	
					NodeImageMapDataAccessManager mapMgr = new NodeImageMapDataAccessManager();
					NodeLinkImageMap map = mapMgr.getImageMap(nl.getMapImage());
					
					String uri = request.getScheme() + "://" +
				             request.getServerName() + 
				             ("http".equals(request.getScheme()) && request.getServerPort() == 80 || "https".equals(request.getScheme()) && request.getServerPort() == 443 ? "" : ":" + request.getServerPort() );
					
					logger.debug("Context Path : " + uri + ctxPath);
					AddImage(uri + map.getUrl(), wb, sheet);
				}
				
			} catch (Exception e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			}
					
			XSSFRow rowLoc1 = sheet.getRow(4);
			XSSFCell cellLoc1 = rowLoc1.getCell(10);	
			// 11행, 2열  장소 표시

			if( nl != null)
			{				
				Site site = new SiteDataAccessManager().getLocation(nl.getLocationID());
				if( site != null)
				{
					String locName = site.getName() + " - " + nl.getName();						
					cellLoc1.setCellValue(locName);
				}				
			}	
			
			
			// 5행 , 10열  발행날자
			XSSFRow rowDate = sheet.getRow(5);
			XSSFCell cellDate = rowDate.getCell(10);
			Calendar cal = Calendar.getInstance();
			Date createTime = cal.getTime();
			SimpleDateFormat dayTime = new SimpleDateFormat("yyyy-MM-dd");		
			SimpleDateFormat dayTime2 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");	
			cellDate.setCellValue(dayTime.format(createTime));
						
			Date dtFrom = null, dtTo = null;
			try {
				dtFrom = dayTime.parse(szFormDate);
				dtTo = dayTime.parse(szToDate);
			} catch (ParseException e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			}
			
			// 8행 , 0열 기간 표시
			XSSFRow rowRange = sheet.getRow(8);
			XSSFCell cellRange = rowRange.getCell(0);			
			cellRange.setCellValue(szFormDate + " ~ " + szToDate);
			
			SensorDataAccessManager sm = new SensorDataAccessManager();
			
			// 27 - 33 행 	 - 측정결과표	
			Date dt1 = dtFrom;
			for( int i = 27 ; i <= 33 ; i++)
			{				
				String time1 = dayTime2.format(dt1);
				Date dt = getNextday(dt1);
				String time2 = dayTime2.format(dt);
				
				XSSFRow row = sheet.getRow(i);
				
				// 1열 날짜
				XSSFCell cellDay = row.getCell(0);				
				String szDay = (dt1.getMonth() +1) + "월 " + dt1.getDate() + "일";
				cellDay.setCellValue(szDay);
				
				// 2열 - 미세먼지,
				XSSFCell cell = row.getCell(2);
				SensorValue sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 23040, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}
								
				// 3열 = 이산화탄소
				cell = row.getCell(3);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 21248, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				// 4열 = 폼알데히드
				cell = row.getCell(4);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 36864, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				// 5열 = 초미세먼지
				cell = row.getCell(5);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 23296, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				// 6열 = 극초미세먼지
				cell = row.getCell(6);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 23808, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				// 7열 = 총유기화합물
				cell = row.getCell(7);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 22784, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				// 8열 = 산소
				cell = row.getCell(8);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 21760, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				// 9열 = 라돈
				cell = row.getCell(9);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 37120, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				//10열 = 온도
				cell = row.getCell(10);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 8192, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
				//11열 = 습도
				cell = row.getCell(11);
				sv = null;
				try {
					sv = sm.getMaxSensorValue(nNode, 8448, time1, time2);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				if( sv != null)
				{
					cell.setCellValue(sv.getSensorValue());
				}
				else
				{
					cell.setCellValue("");
				}	
//				//12열 = 진드기
//				cell = row.getCell(12);
//				sv = null;
//				try {
//					sv = sm.getMaxSensorValue(nNode, 38400, time1, time2);
//				} catch (Exception e) {
//					// TODO Auto-generated catch block
//					e.printStackTrace();
//				}
//				if( sv != null)
//				{
//					cell.setCellValue(sv.getSensorValue());
//				}
//				else
//				{
//					cell.setCellValue("");
//				}	
//				//13열= 유기화합물
//				cell = row.getCell(13);
//				sv = null;
//				try {
//					sv = sm.getMaxSensorValue(nNode, 22784, time1, time2);
//				} catch (Exception e) {
//					// TODO Auto-generated catch block
//					e.printStackTrace();
//				}
//				if( sv != null)
//				{
//					cell.setCellValue(sv.getSensorValue());
//				}
//				else
//				{
//					cell.setCellValue("");
//				}
//				
				dt1 = dt;
			}		
			
			
			String szFilePath2 = ctx.getRealPath("/WEB-INF/temp.xlsx");
			final File file2 = new File(szFilePath2);
			FileOutputStream fileOut;
			try {
				fileOut = new FileOutputStream(file2);
				wb.write(fileOut);
				fileOut.close();  
			} catch( IOException exx){
				return Response.status(Status.NOT_FOUND).build();
			}
			
			
			String fileName = "실내공기질상세보고서.xlsx";
			fileName = new String(fileName.getBytes("euc-kr"), "8859_1");
		
			//final AttachFile file = manager.getAttachedFile(szUUID);
			String szHeader = "attachment; fileName=\"" + fileName + "\";";
			if( file != null)
			{
				
				StreamingOutput fileStream =  new StreamingOutput() 
		        {
		            @Override
		            public void write(java.io.OutputStream output) throws IOException, WebApplicationException 
		            {
		                try
		                {		    
		                
		                	InputStream is = new FileInputStream(file2);
		                    long length = file2.length();		               
		                    if (length > Integer.MAX_VALUE) {
		                        // File is too large
		                    }
		                    // Create the byte array to hold the data
		                    byte[] bytes = new byte[(int)length];
		                    // Read in the bytes
		                    int offset = 0;
		                    int numRead = 0;
		                    while (offset < bytes.length
		                           && (numRead=is.read(bytes, offset, bytes.length-offset)) >= 0) {
		                        offset += numRead;
		                    }
		                    // Ensure all the bytes have been read in
		                    if (offset < bytes.length) {
		                        throw new WebApplicationException("Could not completely read file "+file2.getName());
		                    }
		                    is.close();
		                    
		                	byte[] data = bytes;
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
	@Produces("application/json; charset=UTF-8")
	@Path("/delete")
	public Response deleteAttach(
			
			)
	{
		if(!checkLogin())
			return rejectUpload();
		
		String szResultJson = "{\"Delete\":{\"DeleteInfo\":[{}]}, \"Result\":-1}";
		try
		{
			
		}	
		catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();	
	}
	

	@POST
	@Consumes("multipart/form-data; charset=UTF-8")
	@Produces("application/json; charset=UTF-8")
	@Path("/new")
	public Response attachFileJson( 
			@FormDataParam("upload_file") InputStream aUploadedInputStream,
	        @FormDataParam("upload_file") FormDataContentDisposition fileMetaData)
	{	
		if(!checkLogin())
			return rejectUpload();	
		
		return uploadFile(2,aUploadedInputStream, fileMetaData);	
	}
	
	@POST
	@Consumes("multipart/form-data; charset=UTF-8")
	@Produces("application/json; charset=UTF-8")
	@Path("/image/new")
	public Response attachImageJson( 
			@FormDataParam("upload_file") InputStream aUploadedInputStream,
	        @FormDataParam("upload_file") FormDataContentDisposition fileMetaData) 
	{	
		if(!checkLogin())
			return rejectUpload();	
		
		return uploadFile(1,aUploadedInputStream, fileMetaData);
	}
	
	@SuppressWarnings("deprecation")
	private Response uploadFile(int nUploadType, InputStream aUploadedInputStream, FormDataContentDisposition fileMetaData)
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
			String szFileURL = ctxPath + "/File/get/"+szUUID;	             
	   			
			
			szMimeType = new MimetypesFileTypeMap().getContentType(uploadFile);
			
			byte[] fileByte = new byte[(int) uploadFile.length()];
			InputStream in = new FileInputStream(uploadFile);
			in.read(fileByte, 0, (int)uploadFile.length());			
		    in.close();
		    
			AttachFile file = new AttachFile();
			file.setFileContent(fileByte);
			file.setFileName(szFileName);
			file.setMimeType(szMimeType);
			file.setLinkPost(false);
			file.setUuid(szUUID);
			file.setFileSize((int)uploadFile.length());
			file.setDescritpion("");
			file.setUrl(szFileURL);
			// 2 file 업로드 , 1이면 image업로드
			file.setUploadType(nUploadType);
			
			manager.saveUploadFile(file);
			
			Map<String, Object> map = new LinkedHashMap<String, Object>();
			map.put("attachurl", szFileURL);
			map.put("filemime", szMimeType);
			map.put("filename", szFileName);
			map.put("fileid", szUUID);
			
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
	