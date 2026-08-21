using System;
using NipaSOP.Model.Sop;

namespace NipaSOP.IDAL
{
    public interface ICreate
    {
        string GetErrorMessage();

        StartInfo CreateStartInfo(DateTime dtTimeStamp, string strAccessMode, string strAccessToken, string strServiceType, int nFacilityID, bool randomID = false);
        LocationLinkedSOP CreateLocationLinkedSOP(int nFacilityID, int nFacilityTypeID, int nDisasterCategoryID, int nSubDisasterCategoryID, string strDisasterName);
        Facility CreateFacility(int id, string strFacilityName, string strSiteName, string strDisplayName, int nSiteID);
    }
}
