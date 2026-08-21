using System.Collections.Generic;

namespace SensorMaker.BLL
{
    using Models.Response;
    using Excel.Reader;
    using Excel.Writer;
    using Models.Data.Sensor;

    public class ExcelManager
    {
        public static ResponseOpenSensorFile OpenFireSensorFile(string strPath)
        {
            return OpenSensorFile<FireSensor>(DataMode.Fire, strPath);
        }

        public static ResponseOpenSensorFile OpenPSMSensorFile(string strPath)
        {
            return OpenSensorFile<PSMSensor>(DataMode.PSM, strPath);
        }

        public static ResponseOpenSensorFile OpenEtcSensorFile(string strPath)
        {
            return OpenSensorFile<EtcSensor>(DataMode.Etc, strPath);
        }

        public static ResponseOpenSensorFile OpenCCTVFile(string strPath)
        {
            return OpenSensorFile<CCTVSensor>(DataMode.CCTV, strPath);
        }

        private static ResponseOpenSensorFile OpenSensorFile<SensorType>(DataMode mode, string strPath)
        {
            string strErrorMessage;
            ExcelReader reader = ExcelReader.MakeInstance(mode, strPath);

            ResponseOpenSensorFile response = new ResponseOpenSensorFile(false, "");

            if (reader != null)
            {
                if (reader.Run(out strErrorMessage))
                {
                    if (reader.Result != null)
                    {
                        response.Success = true;

                        if (typeof(SensorType) == typeof(FireSensor))
                            response.FireSensors = (List<FireSensor>)reader.Result;
                        else if (typeof(SensorType) == typeof(PSMSensor))
                            response.PsmSensors = (List<PSMSensor>)reader.Result;
                        else if (typeof(SensorType) == typeof(EtcSensor))
                            response.EtcSensors = (List<EtcSensor>)reader.Result;
                        else if (typeof(SensorType) == typeof(CCTVSensor))
                            response.Cctvs = (List<CCTVSensor>)reader.Result;
                        else
                        {
                            response.Success = false;
                            response.Message = "알수없는 타입의 데이터 파일입니다.";
                        }
                    }
                    else
                        response.Message = "데이터가 존재하지 않습니다.";
                }
                else
                    response.Message = strErrorMessage;
            }
            else
                response.Message = "알수없는 타입의 데이터 파일입니다.";

            return response;
        }

        public static ResponseExcelInfo DownloadSensorFile(object sensorList)
        {
            ExcelWriter writer = ExcelWriter.MakeInstance(sensorList);

            if (writer == null)
                return new ResponseExcelInfo(false, "ExcelWriter 생성 실패");

            string strErrorMessage;
            byte[] bytes = writer.Run(out strErrorMessage);

            if (bytes == null)
                return new ResponseExcelInfo(false, strErrorMessage);

            ResponseExcelInfo response = new ResponseExcelInfo(true, "");
            response.Bytes = bytes;
            return response;
        }
    }
}
