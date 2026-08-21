package kr.co.unes.aqm.view;

import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.HttpHeaders;
import javax.ws.rs.core.MultivaluedMap;
import javax.ws.rs.core.Response;

import org.glassfish.jersey.server.mvc.Viewable;

import org.slf4j.LoggerFactory;

import kr.co.unes.aqm.dto.post.AttachFile;
import kr.co.unes.aqm.dto.post.PostItem;
import kr.co.unes.aqm.model.AttachedFileDataAccessManager;
import kr.co.unes.aqm.model.PostDataAccessManager;
import kr.co.unes.aqm.servlet.AQMLoginManager;

@Path("/Admin")
public class AdminPostFtlView {

	@Context
	private HttpServletRequest request;
	
	@Context
	private HttpServletResponse response;

	private final org.slf4j.Logger logger = LoggerFactory.getLogger(AdminPostFtlView.class);
	private PostDataAccessManager manager = new PostDataAccessManager();
	private AttachedFileDataAccessManager fileManager = new AttachedFileDataAccessManager();
	
	private boolean checkLogin()
	{
		HttpSession session = request.getSession();
		AQMLoginManager manager = AQMLoginManager.getInstance();	
		return manager.checkLogin(session);
	}
	
	public Response requestLogin()
	{		
		return requestLogin(true);
	}
	
	public Response requestLogin(boolean bSavedReferer)
	{
		if( bSavedReferer == true)
		{
			String referrer = request.getHeader("Referer");
		    request.getSession().setAttribute("prevPage", referrer);
		}
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		Viewable view = new Viewable("/admin/admin_login.ftl", map);
		return Response.ok(view).build();
	}
	
	public Response readAllPost()
	{
		//ArrayList<PostItem> posts = (ArrayList<PostItem>) manager.getAllPost();
		Map<String, Object> map = new LinkedHashMap<String, Object>();	
		
		//makePageNavigationInfo(map, posts, 10);
			
		Viewable view = new Viewable("/admin/admin_bbs_list.ftl", map);
		return Response.ok(view).build();
	}
	
	private int makePageNavigationInfo(Map<String, Object> map, List<PostItem> locationList, int nPagePerItem)
	{
		int nItemCount = locationList.size();
		int nExtraCount = nPagePerItem - (nItemCount % nPagePerItem);
		int nPage = nItemCount / nPagePerItem;
		
		if(nItemCount % nPagePerItem > 0)
			nPage = nPage + 1;
		if(nExtraCount == 10)
			nExtraCount = 0;
		map.put("PageCount", nPage);
		map.put("ExtraCount",  nExtraCount);
		
		LinkedHashMap<String, Object> mapPage = new LinkedHashMap<String,Object>();
		for( int j = 0 ; j < nPage; j++)
		{
			List<PostItem> itemList = new ArrayList<PostItem>();
			for(int i = 0 ; i < nPagePerItem; i++)
			{
				int index = j * nPagePerItem + i;
				if( index < nItemCount)
				{
					PostItem item = locationList.get(index);
					itemList.add(item);
				}
				else
				{
					break;
				}				
			}
			mapPage.put("Page" + (j+1), itemList);
		}
		if(mapPage.size() > 0)
			map.put("PageList", mapPage);
		return nPage;
	}
	
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/")
	public Response adminView()
	{
		if(!checkLogin())
			return requestLogin(false);
				
		String ftlDir = "/admin/admin.ftl";
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		Viewable view = new Viewable(ftlDir, map);		
		return Response.ok(view).build();
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/logout")
	public Response adminViewLogout()
	{
		AQMLoginManager.getInstance().logoutAdmin(request.getSession());			
		return requestLogin(false);		
	}
		
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/")
    public Response adminView(@DefaultValue("-1") @FormParam("number") int nPass)
	{		
		HttpSession session = request.getSession();
		AQMLoginManager manager = AQMLoginManager.getInstance();		
		if(manager.adminLogin(session, nPass))
		{
			
		    String redirectUrl = (String) request.getSession().getAttribute("prevPage");		    
		    if (redirectUrl != null) {
                session.removeAttribute("prevPage");
                try {
                	request.getSession().removeAttribute("prevPage");
					response.sendRedirect(redirectUrl);
				} catch (IOException e) {					
					e.printStackTrace();
				}                
            } 
			Map<String, Object> map = new LinkedHashMap<String, Object>();
	        Viewable view = new Viewable("/admin/admin.ftl",map );        
	        return Response.ok(view).build();
		}
		else
		{
			Map<String, Object> map = new LinkedHashMap<String, Object>();
	        Viewable view = new Viewable("/admin/admin_login.ftl",map );        
	        return Response.ok(view).build();
		}
    }
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/posts")
	public Response adminManagePost()
	{
		if(!checkLogin())
			return requestLogin();
		
		return readAllPost();
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/postlist")
	public Response adminViewTablePost()
	{
		ArrayList<PostItem> posts = (ArrayList<PostItem>) manager.getAllPost();
		Map<String, Object> map = new LinkedHashMap<String, Object>();	
		map.put("Target", "Admin");
		makePageNavigationInfo(map, posts, 10);
			
		Viewable view = new Viewable("/common/bbs_table_list.ftl", map);
		return Response.ok(view).build();
	}
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/searchlist")
	public Response adminSearchTablePost(
			@FormParam("type") int type,
			@FormParam("text") String text)
	{
		ArrayList<PostItem> posts = (ArrayList<PostItem>) manager.searchPost(type, text);
		Map<String, Object> map = new LinkedHashMap<String, Object>();	
		map.put("Target", "Admin");
		makePageNavigationInfo(map, posts, 10);
			
		Viewable view = new Viewable("/common//bbs_table_list.ftl", map);
		return Response.ok(view).build();
		
	}

	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/post/detail/{id}")
	public Response adminViewPost(@PathParam("id") int nID)
	{		
		if(!checkLogin())
			return requestLogin();
		
		PostItem item = manager.getPost(nID);
		if( item != null)
		{
			Map<String, Object> map = new LinkedHashMap<String, Object>();
			map.put("Post", item);
			Viewable view = new Viewable("/admin/admin_bbs_detail.ftl", map);
			return Response.ok(view).build();
		}

		return readAllPost();
	}	

	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/post/new")
	public Response adminPostWrite(@Context HttpHeaders headers,
			MultivaluedMap<String, String> formFields,
			@FormParam("title") String title,
			@FormParam("content") String text,
			@FormParam("attach_file") List<String> attachFiles,
			@FormParam("postType") int postType)
	{		
		if(!checkLogin())
			return requestLogin();		
		
		if(text != null)
		{
			PostItem item = new PostItem();
			item.setTitle(title);
			item.setContent(text);
			item.setWriter("관리자");
			item.setPostType(postType);
			if(attachFiles == null || attachFiles.size() == 0)
			{
				item.setHasFile(false);
			}
			else
			{
				item.setHasFile(true);
			}
			int nResult = manager.writePost(item);
			if(nResult > 0)
			{
				
				logger.debug("PostID : " + item.getId());
				
				if( attachFiles!= null && attachFiles.size() > 0)
				{
					ArrayList<String> arList = new ArrayList<String>();
					for(int i = 0 ; i < attachFiles.size(); i++)
					{
						String [] paths = attachFiles.get(i).split("/");
						if( paths!= null && paths.length > 0)
						{
							int idx = paths.length - 1;
							String uuid = paths[idx];
							logger.debug("Link File : " + uuid);
							arList.add(uuid);							
						}
					}
					fileManager.setAttachFileInPost(item.getId(), arList);
				}
				Map<String, Object> map = new LinkedHashMap<String, Object>();
				map.put("Post", item);
				Viewable view = new Viewable("/admin/admin_bbs_detail.ftl", map);
				return Response.ok(view).build();
			}			
		}
		return readAllPost();
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/post/new")
	public Response adminPostWrite()
	{		
		if(!checkLogin())
			return requestLogin();
		
		Map<String, Object> map = new LinkedHashMap<String, Object>();		
		Viewable view = new Viewable("/admin/admin_bbs_write.ftl", map);
		return Response.ok(view).build();
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/post/modify/{id}")
	public Response adminModifyPost(@PathParam("id") int nID)			
	{		
		if(!checkLogin())
			return requestLogin();
		
		PostItem item = manager.getPost(nID);
		if( item != null)
		{
			List<AttachFile> files = fileManager.getAttachedFiles(nID);
			
			for(AttachFile file : files)
			{
				logger.debug("AttachFile : " + file.getUrl());
			}
			
			Map<String, Object> map = new LinkedHashMap<String, Object>();
			map.put("Post", item);
			map.put("Files", files);
			Viewable view = new Viewable("/admin/admin_bbs_modify.ftl", map);
			return Response.ok(view).build();
		}

		return readAllPost();
	}
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/post/modify/{id}")
	public Response adminPostModify(
			@PathParam("id") int nID,
			@FormParam("title") String title,
			@FormParam("content") String text,
			@FormParam("attach_file") List<String> attachFiles,
			@FormParam("postType") int postType)
	{		
		if(!checkLogin())
			return requestLogin();
		
		if(text != null)
		{			
			PostItem item = new PostItem();
		
			item.setId(nID);
			item.setTitle(title);
			item.setContent(text);
			item.setPostType(postType);
			item.setWriter("관리자");
			if(attachFiles == null || attachFiles.size() == 0)
			{
				item.setHasFile(false);
			}
			else
			{
				item.setHasFile(true);
			}
			
			manager.modifyPost(item);
			
			if( attachFiles!= null && attachFiles.size() > 0)
			{
				ArrayList<String> arList = new ArrayList<String>();
				for(int i = 0 ; i < attachFiles.size(); i++)
				{
					String [] paths = attachFiles.get(i).split("/");
					if( paths!= null && paths.length > 0)
					{
						int idx = paths.length - 1;
						String uuid = paths[idx];
						logger.debug("Link File : " + uuid);
						arList.add(uuid);							
					}
				}
				fileManager.modifyAttachFileInPost(item.getId(), arList);
			}	
			
			item = manager.getPost(nID);
			
			
			Map<String, Object> map = new LinkedHashMap<String, Object>();
			map.put("Post", item);
			Viewable view = new Viewable("/admin/admin_bbs_detail.ftl", map);
			return Response.ok(view).build();
		}		
		
		return readAllPost();
	}
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/post/delete/{id}")
	public Response adminPostDelete(@DefaultValue("0") @PathParam("id") int nID)			
	{		
		
		if(!checkLogin())
			return requestLogin();
		
		if(nID > 0)
		{
			fileManager.deleteAttachedFile(nID);
			manager.deletePost(nID);			
		}
		return readAllPost();
	}
}
