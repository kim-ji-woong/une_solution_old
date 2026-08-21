package g1Weather.excel;

import g1Weather.service.SearchRain;
import g1Weather.service.SnowSumData;
import g1Weather.service.SearchRainOption.SearchRainToday;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.web.servlet.view.document.AbstractExcelView;

public class ExcelSearchRainToday extends AbstractExcelView {
	public ExcelSearchRainToday()
	{
		// 기존 양식을 이용한다.
		setUrl("/data/일일조회");
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
		// 기존 양식이 존재할 경우 첫번째 Sheet를 불러온다.
		String cityName = ReferenceFile(model, wb);
		// 기존 양식이 없을 경우 새로운 Excel Sheet를 만든다.
		//NewFile(model, wb);
		
		String fileName = cityName.length() == 0 ? "강우조회(금일).xls" : cityName + "_강우조회(금일).xls";
		fileName = new String(fileName.getBytes("euc-kr"), "8859_1");
		
		response.setHeader("Content-Disposition", "attachment; fileName=\"" + fileName + "\";");
		response.setHeader("Content-Transfer-Encoding", "binary"); 
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
		int nExcelRowCount = 11;
		
		if (nDataCount <= 1)
			return;
		else if (nDataCount == nExcelRowCount)
			return;
		else if (nDataCount < nExcelRowCount)
		{
			for (int i=nFirstRowIndex + nDataCount;i<nFirstRowIndex + nExcelRowCount;i++)
			{
				sheet.removeRow(sheet.getRow(i));
			}
			
			sheet.shiftRows(nFirstRowIndex + nExcelRowCount, nFirstRowIndex + nExcelRowCount + 2, nDataCount - nExcelRowCount);
		}
		else if (nDataCount > nExcelRowCount)
		{
			for (int i=nExcelRowCount;i<nDataCount;i++)
			{
				copyRow(wb, sheet, nFirstRowIndex + 2, nFirstRowIndex + 3);
			}
		}
		/*else if (nDataCount < nExcelRowCount)
		{
			for (int i=nDataCount;i<nExcelRowCount;i++)
			{
				sheet.removeRow(sheet.getRow(nFirstRowIndex + i - nDataCount));
			}
			
			sheet.shiftRows(nExcelRowCount - nDataCount + 2, nExcelRowCount, nDataCount - nExcelRowCount);
		}
		else if (nDataCount > nExcelRowCount)
		{
			for (int i=nExcelRowCount;i<nDataCount;i++)
			{
				copyRow(wb, sheet, nFirstRowIndex, nFirstRowIndex + 1);
			}
		}*/
	}
	
	// 기존에 존재하는 양식을 사용한다.
	// Return 값 : 도시이름
	private String ReferenceFile(Map<String, Object> model, HSSFWorkbook wb)
	{
		HSSFCell cell = null;
		String cityName = (String)model.get("cityName");
		
		// 첫번째 Sheet를 불러온다.
		HSSFSheet sheet = wb.getSheetAt(0);
		List<SearchRainToday> rainList = (List<SearchRainToday>)model.get("totalSearchRain");
		
		int nRowIndex = 6;
		int nRainCount = rainList.size();
		
		ChangeTableSize(wb, sheet, nRainCount, nRowIndex - 1);
		
		for (int i=0;i<nRainCount;i++)
		{
			SearchRainToday rain = rainList.get(i);
			int col = 0;
			
			//setText(cell = getCell(sheet, nRowIndex, col++), Integer.toString(i + 1));		
			setText(getCell(sheet, nRowIndex, col++), rain.getLocationName());
			
			List<SearchRain.RainData> items = rain.getItemValues();
			
			for (SearchRain.RainData data : items)
			{
				setText(getCell(sheet, nRowIndex, col++), data.getValue());
			}
			
			nRowIndex++;
		}
		
		DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
		Date date = new Date();
		String today = dateFormat.format(date);
		
		setText(getCell(sheet, 1, 0), "강우조회결과");
		setText(getCell(sheet, 2, 0), "검색조건 : 금일");
		setText(getCell(sheet, 3, 0), "검색기간 : " + today);
		
		return cityName;
	}
}
