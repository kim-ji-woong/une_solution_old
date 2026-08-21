package g1Weather.excel;

import g1Weather.service.WaterLevelSumData;

import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.web.servlet.view.document.AbstractExcelView;

public class ExcelWaterLevelSumData extends AbstractExcelView {
	public ExcelWaterLevelSumData()
	{
		// 기존 양식을 이용한다.
		setUrl("/data/수위현황");
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
		
		String fileName = cityName.length() == 0 ? "수위현황.xls" : cityName + "_수위현황.xls";
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
	
	// 기존에 존재하는 양식을 사용한다.
	// Return 값 : 도시이름
	private String ReferenceFile(Map<String, Object> model, HSSFWorkbook wb)
	{
		HSSFCell cell = null;
		String cityName = "";
		
		// 첫번째 Sheet를 불러온다.
		HSSFSheet sheet = wb.getSheetAt(0);
		List<WaterLevelSumData> levelList = (List<WaterLevelSumData>)model.get("totalWaterLevelSumData");
		
		int nRowIndex = 1;
		int nLevelCount = levelList.size();
		
		ChangeTableSize(wb, sheet, nLevelCount, nRowIndex);
		
		if (nLevelCount > 0)
			cityName = levelList.get(0).getCityName();
		
		for (int i=0;i<nLevelCount;i++)
		{
			WaterLevelSumData level = levelList.get(i);
			int col = 0;
			
			setText(cell = getCell(sheet, nRowIndex, col++), Integer.toString(i + 1));		
			setText(getCell(sheet, nRowIndex, col++), level.getLocationName());
			setText(getCell(sheet, nRowIndex, col++), level.getTimeStamp());
			setText(getCell(sheet, nRowIndex, col++), level.getStandardWaterLevel());
			setText(getCell(sheet, nRowIndex, col++), level.getWaterLevel());
			setText(getCell(sheet, nRowIndex++, col++), level.getDescription());
		}
		
		return cityName;
	}
}
