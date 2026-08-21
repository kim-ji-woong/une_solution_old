package g1Weather.common;

public class MenuItem {
	private boolean firstMenu = false;
	private boolean selected = false;
	private String linkedPage = "";
	private String linkedPageOrigin = "";
	private String clickedEvent = "";
	private String name = "";
	private boolean visible = true;
	
	public MenuItem()
	{
	}
	
	public MenuItem(String name, String linkedPage, String clickedEvent)
	{
		this.name = name;
		this.linkedPage = linkedPage;
		this.linkedPageOrigin = linkedPage;
		this.clickedEvent = clickedEvent;
	}
	
	public boolean getFirstMenu()
	{
		return firstMenu;
	}
	
	public void setFirstMenu(boolean isFirst)
	{
		firstMenu = isFirst;
	}
	
	public boolean getSelected()
	{
		return selected;
	}
	
	public void setSelected(boolean selected)
	{
		this.selected = selected;
	}
	
	public String getLinkedPage()
	{
		return linkedPage;
	}
	
	public void setLinkedPage(String page)
	{
		linkedPage = page;
	}
	
	public String getLinkedPageOrigin()
	{
		return linkedPageOrigin;
	}
	
	public String getClickedEvent()
	{
		return clickedEvent;
	}
	
	public void setClickedEvent(String clickedEvent)
	{
		this.clickedEvent = clickedEvent;
	}
	
	public String getName()
	{
		return name;
	}
	
	public void setName(String name)
	{
		this.name = name;
	}
	
	public boolean getVisible()
	{
		return visible;
	}
	
	public void setVisible(boolean isVisible)
	{
		this.visible = isVisible;
	}
}
