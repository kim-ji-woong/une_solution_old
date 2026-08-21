package g1Weather.excel;

import g1Weather.service.RealTimeRainFall;
import g1Weather.service.ReportData;

import g1Weather.service.SearchSnow;
import g1Weather.webService.SpecialNews;

import java.io.InputStream;
import java.net.URL;
import java.util.Calendar;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFClientAnchor;
import org.apache.poi.hssf.usermodel.HSSFPatriarch;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.web.servlet.view.document.AbstractExcelView;

import java.text.DecimalFormat;

public class ExcelMainPage extends AbstractExcelView {
	private String reportTime = "";
	private String period = "";
	
	public ExcelMainPage()
	{
		// 기존 양식을 이용한다.
		setUrl("/data/메인화면보고서");
	}
	
	/**
	 * 엑셀파일을 설정하고 생성한다.
	 * @param model
	 * @param wb
	 * @param request
	 * @param response
	 * @throws Exception
	 */
	@Override
	protected void buildExcelDocument(Map<String, Object> model,
			HSSFWorkbook wb, HttpServletRequest request,
			HttpServletResponse response) throws Exception 
	{
		String param = getParams(model);
		period = param;
		String fileTag = "메인화면보고서.xls";
		
		// 기존 양식이 존재할 경우 첫번째 Sheet를 불러온다.
		String cityName = ReferenceFile(model, wb);
		// 기존 양식이 없을 경우 새로운 Excel Sheet를 만든다.
		//NewFile(model, wb);
		
		String fileName = cityName.length() == 0 ? fileTag : cityName + "_" + fileTag;
		fileName = new String(fileName.getBytes("euc-kr"), "8859_1");
		
		response.setHeader("Content-Disposition", "attachment; fileName=\"" + fileName + "\";");
		response.setHeader("Content-Transfer-Encoding", "binary"); 
	}
	
	private String getParams(Map<String, Object> model)
	{
		String param = "";
		String excelParam = (String)model.get("excelParam");
		
		if (excelParam != null)
		{
			String[] arrParams = excelParam.split(";");
			
			if (arrParams.length >= 2)
			{
				String beginDate = getMonthNDay(arrParams[0]);
				String endDate = getMonthNDay(arrParams[1]);
				
				if (beginDate.length() > 0 && endDate.length() > 0)
					param = beginDate + "~" + endDate;
			}
			
			if (arrParams.length >= 3)
			{
				reportTime = arrParams[2];
			}
		}
		
		return param;
	}
	
	private String getMonthNDay(String date)
	{
		if (date.length() == 10)
		{
			return date.substring(5);
		}
		else if (date.length() == 8)
		{
			String month = date.substring(4, 6);
			String day = date.substring(6, 8);
			return month + "-" + day;
		}
		
		return "";
	}
	
	private void copyRow(HSSFWorkbook workbook, HSSFSheet worksheet, int sourceRowNum, int destinationRowNum)
	{
        // Get the source / new row
        HSSFRow newRow = worksheet.getRow(destinationRowNum);
        HSSFRow sourceRow = worksheet.getRow(sourceRowNum);

        // If the row exist in destination, push down all rows by 1 else create a new row
        if (newRow != null) {
            worksheet.shiftRows(destinationRowNum, worksheet.getLastRowNum(), 1);
        } else {
            newRow = worksheet.createRow(destinationRowNum);
        }
        
        // Loop through source columns to add to new row
        for (int i = 0; i < sourceRow.getLastCellNum(); i++) {
            // Grab a copy of the old/new cell
            HSSFCell oldCell = sourceRow.getCell(i);
            HSSFCell newCell = newRow.createCell(i);

            // If the old cell is null jump to next cell
            if (oldCell == null) {
                newCell = null;
                continue;
            }

            newCell.setCellStyle(oldCell.getCellStyle());
        }
    }
	
	// 엑셀양식에 존재하는 테이블 개수를 실제 데이터 개수에 맞춘다.
	private void ChangeTableSize(HSSFWorkbook wb, HSSFSheet sheet, int nDataCount, int nFirstRowIndex)
	{
		int nExcelRowCount = 10;
		
		if (nDataCount == 0)
			return;
		else if (nDataCount == nExcelRowCount)
			return;
		else if (nDataCount < nExcelRowCount)
		{
			for (int i=nDataCount;i<nExcelRowCount;i++)
			{
				sheet.removeRow(sheet.getRow(nFirstRowIndex + i - nDataCount));
			}
			
			sheet.shiftRows(nExcelRowCount - nDataCount + 1, nExcelRowCount, nDataCount - nExcelRowCount);
		}
		else if (nDataCount > nExcelRowCount)
		{
			for (int i=nExcelRowCount;i<nDataCount;i++)
			{
				copyRow(wb, sheet, nFirstRowIndex, nFirstRowIndex + 1);
			}
		}
	}
	
	// 엑셀양식에 존재하는 테이블 개수를 실제 데이터 개수에 맞춘다.
	private void ChangeTableSize2(HSSFWorkbook wb, HSSFSheet sheet, int nDataCount, int nFirstRowIndex, int nExcelRowCount)
	{
		if (nDataCount <= 1)
			nDataCount = 1;
		
		if (nDataCount == 0)
			return;
		else if (nDataCount == nExcelRowCount)
			return;
		else if (nDataCount < nExcelRowCount)
		{
			for (int i=nFirstRowIndex + nDataCount;i<nFirstRowIndex + nExcelRowCount;i++)
			{
				sheet.removeRow(sheet.getRow(i));
				//sheet.removeRow(sheet.getRow(nFirstRowIndex + i - nDataCount));
			}
			
			sheet.shiftRows(nFirstRowIndex + nExcelRowCount, nFirstRowIndex + nExcelRowCount + 2, nDataCount - nExcelRowCount);
			//sheet.shiftRows(nExcelRowCount - nDataCount + nFirstRowIndex, nExcelRowCount + nFirstRowIndex + 2, nDataCount - nExcelRowCount);
			//sheet.shiftRows(nExcelRowCount - nDataCount + nFirstRowIndex, nExcelRowCount + nFirstRowIndex, nDataCount - nExcelRowCount);
		}
		else if (nDataCount > nExcelRowCount)
		{
			for (int i=nExcelRowCount;i<nDataCount;i++)
			{
				copyRow(wb, sheet, nFirstRowIndex, nFirstRowIndex + 1);
			}
		}
	}
	
	// 기존에 존재하는 양식을 사용한다.
	// Return 값 : 도시이름
	private String ReferenceFile(Map<String, Object> model, HSSFWorkbook wb)
	{
		HSSFCell cell = null;
		String cityName = (String)model.get("cityName");
		
		// 첫번째 Sheet를 불러온다.
		HSSFSheet sheet = wb.getSheetAt(0);
		
		String radarImageURL = (String)model.get("radarImageURL");
		List<SpecialNews> specialNewsList = (List<SpecialNews>)model.get("newsList");
		List<RealTimeRainFall> rainList = (List<RealTimeRainFall>)model.get("totalRRF");
		
		/// 보고서 시간
		//setText(getCell(sheet, 2, 1), "[ " + reportTime + " ]");
		//////////////////////// 
		 
		int nRowIndex = 6, nExcelRowCount = 11;
		int nRainCount = rainList.size();

		ChangeTableSize(wb, sheet, nRainCount, nRowIndex);
		 
		for (int i=0;i<nRainCount;i++)
		{
			RealTimeRainFall rain = rainList.get(i);
			int col = 5;
			
			setText(cell = getCell(sheet, nRowIndex, col++), Integer.toString(i + 1)); 
			setText(getCell(sheet, nRowIndex, col++), rain.getLocationName());
			setText(getCell(sheet, nRowIndex, col++), rain.getTimeStamp()); 
			setText(getCell(sheet, nRowIndex, col++), rain.getRain15M());
			setText(getCell(sheet, nRowIndex, col++), rain.getRain60M());
			setText(getCell(sheet, nRowIndex, col++), rain.getRainToday());
			setText(getCell(sheet, nRowIndex, col++), rain.getRainYesterday()); 
			setText(getCell(sheet, nRowIndex++, col++), rain.getDescription());
		}
		 		
		nRowIndex = 20;

		if (nRainCount > nExcelRowCount)
			nRowIndex += nRainCount - nExcelRowCount; 
		
		//특보 리스트
		int nNewsCount = specialNewsList.size();
		nExcelRowCount = 100;
		
		ChangeTableSize2(wb, sheet, nNewsCount, nRowIndex, nExcelRowCount);
		
		for (int i=0;i<nNewsCount;i++)
		{
			SpecialNews news = specialNewsList.get(i);
			
			String item = "[" + news.getTime() + "] [" + news.getNewsType() + "] [" + news.getCommandString() + "] [" + news.getAreaName() + "]"; 
			setText(getCell(sheet, nRowIndex++, 1), item);
		}
		
		if (nNewsCount < 1)
			nRowIndex += 1 - nNewsCount;
		
		setText(getCell(sheet, nRowIndex + 2, 1), cityName + " 재난안전대책본부");
		
		try
		{
			AddImage(radarImageURL, wb, sheet);
		}
		catch (Exception e)
		{
		}
				
		return cityName;
	}
	
	private void AddImage(String url, HSSFWorkbook wb, HSSFSheet sheet) throws Exception
	{
		try
		{
			InputStream inputStream = new URL(url).openStream();
			
			//Get the contents of an InputStream as a byte[].
			byte[] bytes = org.apache.poi.util.IOUtils.toByteArray(inputStream);
			//Adds a picture to the workbook
			int pictureIdx = wb.addPicture(bytes, org.apache.poi.hssf.usermodel.HSSFWorkbook.PICTURE_TYPE_PNG);
			//close the input stream
			inputStream.close();
			
			 // Create the drawing patriarch.  This is the top level container for
	        // all shapes. This will clear out any existing shapes for that sheet.
	        HSSFPatriarch patriarch = sheet.createDrawingPatriarch();

	        HSSFClientAnchor anchor = new HSSFClientAnchor(0,0,0,255,(short)1,5,(short)4,14); // 이미지 크기조절은 여기서..
	        anchor.setAnchorType( 2 );
	        patriarch.createPicture(anchor, pictureIdx); // 삽입 할 이미지
		}
		catch (Exception e)
		{
		}
	} 
}
