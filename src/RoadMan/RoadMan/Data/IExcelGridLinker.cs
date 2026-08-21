using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public interface IExcelGridLinker
    {
        // 복사할 대상 위치에 있는 셀들
        DataGridViewSelectedCellCollection GetPastePositionCells();
        void PasteCells(DataGridViewSelectedCellCollection cells);
    }

    public interface IExcelGridManager
    {
        IExcelGridLinker ExcelGridLinker
        {
            get;
            set;
        }
    }
}
