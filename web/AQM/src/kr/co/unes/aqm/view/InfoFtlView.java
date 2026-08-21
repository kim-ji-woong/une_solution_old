package kr.co.unes.aqm.view;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.Response;

import org.glassfish.jersey.server.mvc.Viewable;
import org.slf4j.LoggerFactory;

import kr.co.unes.aqm.dto.post.PostItem;
import kr.co.unes.aqm.model.PostDataAccessManager;

@Path("/Info")
public class InfoFtlView {
	
	@Context
	private HttpServletRequest request;
	
	@Context
	private HttpServletResponse response;

	private final org.slf4j.Logger logger = LoggerFactory.getLogger(AdminPostFtlView.class);
	private PostDataAccessManager manager = new PostDataAccessManager();

	
	public Response readAllPost()
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();	
		Viewable view = new Viewable("/bbs_list.ftl", map);
		return Response.ok(view).build();
	}
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/")
    public Response viewPosts() 
	{			
		return readAllPost();
    }
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/postlist")
	public Response viewTablePost()
	{
		ArrayList<PostItem> posts = (ArrayList<PostItem>) manager.getAllPost();
		Map<String, Object> map = new LinkedHashMap<String, Object>();	
		map.put("Target", "Info");
		makePageNavigationInfo(map, posts, 10);
			
		Viewable view = new Viewable("/common/bbs_table_list.ftl", map);
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
	@Path("/post/detail/{id}")
    public Response viewDetailItem(@PathParam("id") int nID) 
	{
        PostItem item = manager.getPost(nID);
		if( item != null)
		{
			Map<String, Object> map = new LinkedHashMap<String, Object>();
			map.put("Post", item);
			Viewable view = new Viewable("/bbs_detail.ftl", map);
			return Response.ok(view).build();
		}
		return readAllPost();
    }
}
