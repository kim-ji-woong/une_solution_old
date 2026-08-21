package com.une.manual;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileWriter;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.io.StringReader;
import java.util.Base64;
import java.util.Vector;
import java.util.Base64.Encoder;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathFactory;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;

public class SemiAutoImporter {
	
	private DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();	
	
	public SemiAutoImporter (String inputFileName, String outputFileName) {
		factory.setNamespaceAware(false);
		JSONArray jsonArray = new JSONArray();
		try {
			
			File file = new File(inputFileName);
			
			InputStream inputStream= new FileInputStream(file);
			Reader reader = new InputStreamReader(inputStream,"UTF-8");			    	      
			
			BufferedReader br = new BufferedReader(reader);
		    String line;
		    Vector<StringBuilder> datalist = new Vector<StringBuilder>();
		    StringBuilder stringbuilder = new StringBuilder();		   

		    StringBuilder tempString = new StringBuilder();
		    int count = 0;
		    while ((line = br.readLine()) != null) {
		    	
		    	
		    	if(line.contains("<TABLE") && (count > 0)) {
		    		stringbuilder.append(tempString.toString());		//이전 텍스트 set을 전부 append.
		    		tempString = new StringBuilder();		    		
		    	}
		    		    		
	    		String replacedStr = line.replaceAll("IMG src=\".\\\\(\\S*)\\.(.{3})\"", "IMG src=\"img\\\\$1.$2\"");		//이미지 경로 바꾸기
	    		replacedStr = replacedStr.replaceAll("&nbsp;","");			//&nbsp; 제거		
	    		replacedStr = replacedStr.replaceAll("CLASS=(\\s?)(HStyle[0-9]{1,2})","CLASS=$1\"$2\"");					//CLASS=HStyle16  --> CLASS="HStyle16"
	    		replacedStr = replacedStr.replaceAll("<BR>","");		    												//<BR> 제거
	    			    	
		    	
	    		String regex = "<SPAN ([^<]*)조치목록\\s?</SPAN>";
				Pattern pattern = Pattern.compile(regex);
				Matcher match = pattern.matcher(replacedStr);
				
				if(match.find()) {
					System.out.println("match : " + replacedStr);
//					isFoundPinPoint = true;
					if(count> 0)
						datalist.add(stringbuilder);
					stringbuilder = new StringBuilder();
					count++;
				}

				tempString.append(replacedStr);

		    }
		    
		    stringbuilder.append(tempString);		//이전 텍스트 set을 전부 append.
		    datalist.add(stringbuilder);			//맨 마지막 끝까지 읽혀진 부분 add.
    		
    		
		    System.out.println("dataset total count: " + datalist.size());
			
			BufferedWriter out = new BufferedWriter(new FileWriter(outputFileName));
//			BufferedWriter out = new BufferedWriter(new FileWriter("disease.json"));		//각 매뉴얼을 json파일로 만든 후 붙여야 한다
			
			
			/*** 임무 목록에서 필요한 데이터 추출 ***/
		    for(int i =0; i < datalist.size(); i++) {
		    	
		    	StringBuilder builder = datalist.get(i);
		    	String text = builder.toString();
		    	int firstindex = text.indexOf("</TABLE>");			//조치 목록 table 뜯어내기.	
		    	System.out.println(firstindex);
		    	System.out.println(text.length());
		    	String tableText = text.substring(0, firstindex+8);
		    	System.out.println(tableText);
				
				InputSource is = new InputSource(tableText);
				Document document = factory.newDocumentBuilder().parse(new InputSource(new StringReader(tableText)));
				
				XPath xpath = XPathFactory.newInstance().newXPath();
				
				// NodeList 가져오기 : row 아래에 있는 모든 col1을 선택
				NodeList cols = (NodeList)xpath.evaluate("//SPAN", document, XPathConstants.NODESET);			//XPath를 통해 노드의 SPAN부분만 추출.
				
				System.out.println("cols length : " + cols.getLength());
				
				JSONObject eachobject = new JSONObject();
				
				String processName = cols.item(0).getTextContent();		//징후감지, 초기대응, 비상대응, 수습복구....
				
				String actionTemp = cols.item(4).getTextContent();		//item(3) : 조치목록 , item(4) : 조치목록 컬럼 내용.
				String actionName = actionTemp.split("]")[1].trim();	
				
				System.out.println("action name : " + actionName);				
				
				String detailTemp = cols.item(6).getTextContent();		// item(5) : 조치내용 , item(6) : 조치내용 컬럼 내용.
				String regex = "\\d+-\\d+";
				Pattern pattern = Pattern.compile(regex);
				Matcher match = pattern.matcher(detailTemp);
				System.out.println("detailTemp : " + detailTemp);				
				
				String detailName = detailTemp.split("]")[1].trim();
				System.out.println("detail name : " + detailName);
				
				match.find();
				String code = match.group();
				String[] codes = code.split("-");				
				System.out.println("code : " + code);
				
				int actionCode = Integer.parseInt(codes[0]);
				int detailCode = Integer.parseInt(codes[1]);
				
				System.out.println("detailCode : " + detailCode);
				
				if(processName.indexOf("징후") >= 0 && processName.indexOf("감지") > 0) {
					
					eachobject.put("process_code", 1);
					
				} else if(processName.indexOf("초기") >= 0 && processName.indexOf("대응") > 0) {
					
					eachobject.put("process_code", 2);
					
				} else if(processName.indexOf("비상") >= 0 && processName.indexOf("대응") > 0) {
					
					eachobject.put("process_code", 3);
					
				} else if(processName.indexOf("수습") >= 0 && processName.indexOf("복구") > 0) {
					
					eachobject.put("process_code", 4);
					
				} else {
					
					eachobject.put("process_code", -1);
				}
				
				eachobject.put("process", processName);
				eachobject.put("action_code", actionCode);
				eachobject.put("action_name", actionName);
				eachobject.put("detail_code", detailCode);
				eachobject.put("detail_name", detailName);
				
				JSONArray orgArray = new JSONArray();
				
				for( int idx = 0; idx < cols.getLength(); idx++ ){		    				
					
					System.out.println("TextContent : " + cols.item(idx).getTextContent());
					
					String strtemp = cols.item(idx).getTextContent();
					String regex2 = "주관\\t?\\s?:";
					Pattern pattern2 = Pattern.compile(regex2);
					Matcher match2 = pattern2.matcher(strtemp);
					
					if(match2.find()) {
						//pattern is matched
						String departtemp = strtemp.split(":")[1].trim();		//주관 : 소방서==== 소방서 추출
						if(departtemp.contains(",")) { 
							// 주관 : ooo, ooo로 두개 이상인 경우
							String[] tempdeparts = departtemp.split(",");
							for(int index = 0;index < tempdeparts.length;index++) {
								orgArray.add(tempdeparts[index]);
							}
						} else {
							orgArray.add(departtemp);
						}
						System.out.println("주관 : " + departtemp);
					}
					
					
//					out.write(s); out.newLine();
					
				}	
				
				eachobject.put("org_list", orgArray);								
				
				byte[] targetBytes = text.getBytes("UTF-8");
			    
		        Encoder encoder = Base64.getEncoder();	        
		        
		        byte[] encodedBytes = encoder.encode(targetBytes);
		        
		        System.out.println(new String(targetBytes));
		        eachobject.put("htmldata", new String(encodedBytes));
		        
		        jsonArray.add(eachobject);
		        
				
		    }
		  
	        
	
		    out.write(jsonArray.toJSONString());

		    out.close();
		    br.close();
		    

		} catch(Exception ex) {
			ex.printStackTrace();
		}
	}
	
}
