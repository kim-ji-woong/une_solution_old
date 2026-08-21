using Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.IDAL
{
    public interface ICreate
    {
        CurrentWorkPermit CreateCurrentWorkPermit(int nGeneralCnt, int nFireCnt, int nHighCnt, int nElecCnt, int nClosenessCnt, int nCraneCnt, int nDiggCnt, int nRadiCnt, int nTotalCnt, string strPlantPrcsID, DateTime dtUpdate);
    }
}
