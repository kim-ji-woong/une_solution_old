using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPReadServer
{
    // SAP 연결 인터페이스
    public class SAPConnectorInterface
    {
        private RfcDestination rfcDestination;

        // SAP 연결 확인
        public bool TestConnection(string destinationName)
        {
            bool result = false;

            try
            {
                rfcDestination = RfcDestinationManager.GetDestination(destinationName);
                if (rfcDestination != null)
                {
                    rfcDestination.Ping();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Instance.Write("[ERROR] TestConnection Ping() is fail : " + ex.Message);
            }

            return result;
        }

        public DataTable ConvertToDotNetTable(IRfcTable RFCTable)
        {
            DataTable dtTable = new DataTable();

            for (int item = 0; item < RFCTable.ElementCount; item++)
            {
                RfcElementMetadata metadata = RFCTable.GetElementMetadata(item);
                dtTable.Columns.Add(metadata.Name);
            }

            foreach (IRfcStructure row in RFCTable)
            {
                DataRow dr = dtTable.NewRow();

                for (int item = 0; item < RFCTable.ElementCount; item++)
                {
                    RfcElementMetadata metadata = RFCTable.GetElementMetadata(item);

                    // 컬럼 별로 데이터 타입 구별 
                    //if (metadata.DataType == RfcDataType.BCD && metadata.Name == "ABC")
                    //{
                    //    dr[item] = row.GetInt(metadata.Name);
                    //}
                    //else
                    //{
                    //    dr[item] = row.GetString(metadata.Name);
                    //}
                    dr[item] = row.GetString(metadata.Name);
                }

                dtTable.Rows.Add(dr);
            }

            return dtTable;
        }

        // SAP 테이블 데이터 받아오기
        public DataTable RetrieveCustomers(string destinationName, string strFunctionName, string strTableName, string strParamName, string strParamValue)
        {
            //DataSet dsCustomers = new DataSet();
            DataTable dtTable = null;

            try
            {
                if (rfcDestination == null)
                {
                    rfcDestination = RfcDestinationManager.GetDestination(destinationName);
                }

                RfcRepository rfcRepository = rfcDestination.Repository;

                IRfcFunction rfcFunction = rfcRepository.CreateFunction(strFunctionName);
                rfcFunction.SetValue(strParamName, strParamValue);
                rfcFunction.Invoke(rfcDestination);

                //dsCustomers.Tables.Add(ConvertToDotNetTable(rfcFunction.GetTable(strTableName)));
                dtTable = ConvertToDotNetTable(rfcFunction.GetTable(strTableName));
            }
            catch (Exception ex)
            {
                //throw new Exception("RetrieveCustomers Error: " + ex.Message);
                Logger.Instance.Write("[ERROR] RetrieveCustomers is fail : " + ex.Message);
            }

            return dtTable;
        }
    }

    public class SAPDestinationConfig : IDestinationConfiguration
    {
        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;

        public bool ChangeEventsSupported()
        {
            //throw new NotImplementedException();
            return false;
        }

        public RfcConfigParameters GetParameters(string destinationName)
        {
            RfcConfigParameters parms = new RfcConfigParameters();
            parms.Add(RfcConfigParameters.Name, destinationName);
            parms.Add(RfcConfigParameters.AppServerHost, ConfigurationManager.AppSettings["SAP_APPSERVERHOST"]);
            parms.Add(RfcConfigParameters.SystemNumber, ConfigurationManager.AppSettings["SAP_SYSTEMNUM"]);
            //parms.Add(RfcConfigParameters.SystemID, ConfigurationManager.AppSettings["SAP_CLIENT"]);
            parms.Add(RfcConfigParameters.User, ConfigurationManager.AppSettings["SAP_USERNAME"]);
            parms.Add(RfcConfigParameters.Password, ConfigurationManager.AppSettings["SAP_PASSWORD"]);
            parms.Add(RfcConfigParameters.Client, ConfigurationManager.AppSettings["SAP_CLIENT"]);
            parms.Add(RfcConfigParameters.Language, ConfigurationManager.AppSettings["SAP_LANGUAGE"]);
            //parms.Add(RfcConfigParameters.PoolSize, ConfigurationManager.AppSettings["SAP_POLLSIZE"]);

            return parms;
        }
    }
}
