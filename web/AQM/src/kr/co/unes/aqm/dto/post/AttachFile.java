package kr.co.unes.aqm.dto.post;

import java.io.File;

public class AttachFile {		
	private int id;
	private String fileName;
	private String url;
	private String uuid;	
	private String mimeType;
	private int fileSize;
	private boolean linkPost;	
	private byte[] fileContent;	
	private String descritpion;	
	private int uploadType;
	
	public int getUploadType() {
		return uploadType;
	}
	public void setUploadType(int uploadType) {
		this.uploadType = uploadType;
	}
	public String getUuid() {
		return uuid;
	}
	public void setUuid(String uuid) {
		this.uuid = uuid;
	}
	public String getDescritpion() {
		return descritpion;
	}
	public void setDescritpion(String descritpion) {
		this.descritpion = descritpion;
	}
	public byte[] getFileContent() {
		return fileContent;
	}
	public void setFileContent(byte[] fileContent) {
		this.fileContent = fileContent;
	}
	public boolean isLinkPost() {
		return linkPost;
	}
	public void setLinkPost(boolean linkPost) {
		this.linkPost = linkPost;
	}
	public int getFileSize() {
		return fileSize;
	}
	public void setFileSize(int fileSize) {
		this.fileSize = fileSize;
	}
	public String getMimeType() {
		return mimeType;
	}
	public void setMimeType(String mimeType) {
		this.mimeType = mimeType;
	}
	public String getUrl() {
		return url;
	}
	public void setUrl(String url) {
		this.url = url;
	}
	public String getFileName() {
		return fileName;
	}
	public void setFileName(String fileName) {
		this.fileName = fileName;
	}
	public int getId() {
		return id;
	}
	public void setId(int id) {
		this.id = id;
	}
}
