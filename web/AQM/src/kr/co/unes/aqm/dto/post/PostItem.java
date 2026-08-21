package kr.co.unes.aqm.dto.post;

import java.util.Date;

public class PostItem {

	private String title;
	private String writer;
	private int postType;
	private String content;
	private String convertContent;
	private Date timeStamp;
	private int readCount;
	private boolean hasFile;
	private int id;
	
	
	public String getTitle() {
		return title;
	}
	public void setTitle(String title) {
		this.title = title;
	}
	public String getWriter() {
		return writer;
	}
	public void setWriter(String writer) {
		this.writer = writer;
	}
	public int getPostType() {
		return postType;
	}
	public void setPostType(int postType) {
		this.postType = postType;
	}
	public String getContent() {
		return content;
	}
	public void setContent(String content) {
		this.content = content;
		//setConvertContent(content.replace("\n", "<BR/>"));
		setConvertContent(content);
	}
	
	public Date getTimeStamp() {
		return timeStamp;
	}
	public void setTimeStamp(Date timeStamp) {
		this.timeStamp = timeStamp;
	}
	public int getReadCount() {
		return readCount;
	}
	public void setReadCount(int readCount) {
		this.readCount = readCount;
	}
	public int getId() {
		return id;
	}
	public void setId(int id) {
		this.id = id;
	}
	public String getConvertContent() {
		return convertContent;
	}
	public void setConvertContent(String convertContent) {
		this.convertContent = convertContent;
	}
	public boolean isHasFile() {
		return hasFile;
	}
	public void setHasFile(boolean hasFile) {
		this.hasFile = hasFile;
	}	
}
