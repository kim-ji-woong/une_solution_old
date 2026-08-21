package egovframework.weather.excel;

import java.util.List;
import java.util.Locale;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import net.sf.jxls.parser.Cell;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFCellStyle;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.hssf.util.CellRangeAddress;
import org.apache.poi.poifs.filesystem.POIFSFileSystem;
import org.springframework.core.io.Resource;
import org.springframework.core.io.support.LocalizedResourceHelper;
import org.springframework.web.servlet.support.RequestContextUtils;
import org.springframework.web.servlet.view.document.AbstractExcelView;

import egovframework.weather.service.RealTimeRainFall;

public class ExcelRealTimeRainFall extends AbstractExcelView {
	public ExcelRealTimeRainFall()
	{
		// 기존 양식을 이용한다.
		setUrl("/data/ExcelSample");
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
		ReferenceFile(model, wb);
		// 기존 양식이 없을 경우 새로운 Excel Sheet를 만든다.
		//NewFile(model, wb);
		
		String fileName = "실시간 강수.xls";
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

            // Copy style from old cell and apply to new cell
            //HSSFCellStyle newCellStyle = workbook.createCellStyle();
            //newCellStyle.cloneStyleFrom(oldCell.getCellStyle());
            //newCell.setCellStyle(newCellStyle);
            newCell.setCellStyle(oldCell.getCellStyle());

            // If there is a cell comment, copy
            /*if (oldCell.getCellComment() != null) {
                newCell.setCellComment(oldCell.getCellComment());
            }

            // If there is a cell hyperlink, copy
            if (oldCell.getHyperlink() != null) {
                newCell.setHyperlink(oldCell.getHyperlink());
            }

            // Set the cell data type
            newCell.setCellType(oldCell.getCellType());

            // Set the cell data value
            switch (oldCell.getCellType()) {
                case HSSFCell.CELL_TYPE_BLANK:
                    newCell.setCellValue(oldCell.getStringCellValue());
                    break;
                case HSSFCell.CELL_TYPE_BOOLEAN:
                    newCell.setCellValue(oldCell.getBooleanCellValue());
                    break;
                case HSSFCell.CELL_TYPE_ERROR:
                    newCell.setCellErrorValue(oldCell.getErrorCellValue());
                    break;
                case HSSFCell.CELL_TYPE_FORMULA:
                    newCell.setCellFormula(oldCell.getCellFormula());
                    break;
                case HSSFCell.CELL_TYPE_NUMERIC:
                    newCell.setCellValue(oldCell.getNumericCellValue());
                    break;
                case HSSFCell.CELL_TYPE_STRING:
                    newCell.setCellValue(oldCell.getRichStringCellValue());
                    break;
            }*/
        }

        // If there are are any merged regions in the source row, copy to new row
        /*for (int i = 0; i < worksheet.getNumMergedRegions(); i++) {
            CellRangeAddress cellRangeAddress = worksheet.getMergedRegion(i);
            if (cellRangeAddress.getFirstRow() == sourceRow.getRowNum()) {
                CellRangeAddress newCellRangeAddress = new CellRangeAddress(newRow.getRowNum(),
                        (newRow.getRowNum() +
                                (cellRangeAddress.getLastRow() - cellRangeAddress.getFirstRow()
                                        )),
                        cellRangeAddress.getFirstColumn(),
                        cellRangeAddress.getLastColumn());
                worksheet.addMergedRegion(newCellRangeAddress);
            }
        }*/
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
				sheet.removeRow(sheet.getRow(nFirstRowIndex));
			}
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
	private void ReferenceFile(Map<String, Object> model, HSSFWorkbook wb)
	{
		HSSFCell cell = null;
		
		// 첫번째 Sheet를 불러온다.
		HSSFSheet sheet = wb.getSheetAt(0);
		List<RealTimeRainFall> rainList = (List<RealTimeRainFall>)model.get("totalRRF");
		
		int nRowIndex = 1;
		int nRainCount = rainList.size();
		
		ChangeTableSize(wb, sheet, nRainCount, nRowIndex);
		
		for (int i=0;i<nRainCount;i++)
		{
			RealTimeRainFall rain = rainList.get(i);
			
			setText(cell = getCell(sheet, nRowIndex, 0), Integer.toString(i + 1));		
			setText(getCell(sheet, nRowIndex, 1), rain.getCityName());
			setText(getCell(sheet, nRowIndex, 2), rain.getLocationNumber());
			setText(getCell(sheet, nRowIndex, 3), rain.getLocationName());
			setText(getCell(sheet, nRowIndex, 4), rain.getTimeStamp());
			setText(getCell(sheet, nRowIndex, 5), rain.getRaining());
			setText(getCell(sheet, nRowIndex, 6), rain.getRain15M());
			setText(getCell(sheet, nRowIndex, 7), rain.getRain60M());
			setText(getCell(sheet, nRowIndex, 8), rain.getRainToday());
			setText(getCell(sheet, nRowIndex, 9), rain.getRainYesterday());
			setText(getCell(sheet, nRowIndex, 10), rain.getTemperature());
			setText(getCell(sheet, nRowIndex, 11), rain.getWindDirection1M());
			setText(getCell(sheet, nRowIndex, 12), rain.getWindSpeed1M());
			setText(getCell(sheet, nRowIndex, 13), rain.getHumidity());
			setText(getCell(sheet, nRowIndex++, 14), rain.getDescription());
		}
	}
	
	// 기존 양식이 없을 경우 새로운 Excel Sheet를 만든다.
	private void NewFile(Map<String, Object> model, HSSFWorkbook wb)
	{
		HSSFCell cell = null;
		
		// 기존 양식이 존재할 경우 첫번째 Sheet를 불러온다.
		HSSFSheet sheet = wb.getSheetAt(0);
		// 기존 양식이 없을 경우 새로운 Excel Sheet를 만든다.
		//HSSFSheet sheet = wb.createSheet("실시간 강수");
		
		sheet.setDefaultColumnWidth((short) 12);
 
		// put text in first cell
		cell = getCell(sheet, 0, 0);
		setText(cell, "실시간 강수");
		
		String[] headers = new String[] { "", "시군", "지점번호", "지점명", "관측시각", "강수", "15M", "60M", "금일", "전일", "기온", "풍향1M", "풍속1M", "습도", "비고" };
		List<RealTimeRainFall> rainList = (List<RealTimeRainFall>)model.get("totalRRF");
		
		int nRowIndex = 2, nColumnIndex = 0;
		
		for (String header : headers)
		{
			setText(getCell(sheet, nRowIndex, nColumnIndex++), header);
		}
		
		int nRainCount = rainList.size();
		
		for (int i=0;i<nRainCount;i++)
		{
			RealTimeRainFall rain = rainList.get(i);
			
			setText(cell = getCell(sheet, ++nRowIndex, 0), Integer.toString(i + 1));		
			setText(getCell(sheet, nRowIndex, 1), rain.getCityName());
			setText(getCell(sheet, nRowIndex, 2), rain.getLocationNumber());
			setText(getCell(sheet, nRowIndex, 3), rain.getLocationName());
			setText(getCell(sheet, nRowIndex, 4), rain.getTimeStamp());
			setText(getCell(sheet, nRowIndex, 5), rain.getRaining());
			setText(getCell(sheet, nRowIndex, 6), rain.getRain15M());
			setText(getCell(sheet, nRowIndex, 7), rain.getRain60M());
			setText(getCell(sheet, nRowIndex, 8), rain.getRainToday());
			setText(getCell(sheet, nRowIndex, 9), rain.getRainYesterday());
			setText(getCell(sheet, nRowIndex, 10), rain.getTemperature());
			setText(getCell(sheet, nRowIndex, 11), rain.getWindDirection1M());
			setText(getCell(sheet, nRowIndex, 12), rain.getWindSpeed1M());
			setText(getCell(sheet, nRowIndex, 13), rain.getHumidity());
			setText(getCell(sheet, nRowIndex, 14), rain.getDescription());
		}
	}
}
