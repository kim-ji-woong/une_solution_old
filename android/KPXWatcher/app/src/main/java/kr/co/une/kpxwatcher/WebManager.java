package kr.co.une.kpxwatcher;

import android.app.DownloadManager;
import android.content.Context;
import android.os.Build;
import android.util.Log;

import com.google.firebase.iid.FirebaseInstanceId;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Scanner;

/**
 * Created by 지웅 on 2017-06-21.
 */

public class WebManager extends Thread {
    public enum ResultType
    {
        UNKNOWN(-1),
        SUCCESS(0),
        ALREADY_REQUESTED(1),
        NOT_ENOUGH_PARAMETER(2),
        NOT_PERMITTED_CERT_CODE(3),
        EXPIRED_CERT_CODE(4),
        ALREADY_CERTIFIED_USER(5),
        NOT_CERTIFIED_USER(6),
        NEED_CERT_CODE_CONFIRM(7);


        private final int value;
        private ResultType(int value)
        {
            this.value = value;
        }

        public int toInt()
        {
            return value;
        }
    }

    public enum QueryType
    {
        NONE,
        CHECK_USER,             // 사용자 인증
        REQUEST_CERT_CODE,      // 인증코드 요청
        REQUEST_CERT_CONFIRM,   // 인증코드 확인
        REQUEST_CON_TANK,       // 연결된 탱크 리스트 요청
        REQUEST_LASTWORKHISTORY,// 현재 작업 내역 요청
        READ_NOTICE,            // 공지사항 읽기
        READ_PIPE,              // 배관정보 읽기
        READ_TANK,              // 탱크
        CLEAR_PIPE_ALARM,       // 배관 알람 해제
        CLEAR_TANK_ALARM,       // 탱크 알람 해제
        CLEAR_SULFURIC_ALARM,   // 황산 알람 해제
        READ_OPTION,            // 옵션 읽어오기
        IGNORE_ALARM,           // 알람 무시
        BEGIN_WORK,             // 작업 시작
        END_WORK,               // 작업 종료
        DEV_LOG                 // 디버깅용 로그
    }

    private QueryType mQueryType = QueryType.NONE;
    private Context mContext = null;
    private Map<String, String> mapParameters = new HashMap<String, String>();
    private ResultType mResult = ResultType.UNKNOWN;
    private List<String> mResultSet = null;

    public WebManager(Context context)
    {
        mContext = context;
    }

    public void setQueryType(QueryType type)
    {
        mQueryType = type;
    }

    public void setContext(Context context)
    {
        mContext = context;
    }

    public void setParameter(String key, String value)
    {
        mapParameters.put(key, value);
    }

    public ResultType getResult()
    {
        return mResult;
    }

    public List<String> getResultSet()
    {
        return mResultSet;
    }

    @Override
    public void run()
    {
        try {
            if (mQueryType == QueryType.CHECK_USER)
                CheckUser(mContext);
            else if (mQueryType == QueryType.REQUEST_CERT_CODE)
                RequestCertCode(mContext);
            else if (mQueryType == QueryType.REQUEST_CERT_CONFIRM)
                RequestCertConfirm(mContext);
            else if(mQueryType == QueryType.REQUEST_CON_TANK)
                RequestConnectedTankList(mContext);
            else if(mQueryType == QueryType.REQUEST_LASTWORKHISTORY)
                RequestLastWorkHistory(mContext);
            else if (mQueryType == QueryType.READ_NOTICE)
                ReadNotice(mContext);
            else if (mQueryType == QueryType.READ_PIPE)
                ReadPipe(mContext);
            else if (mQueryType == QueryType.READ_TANK)
                ReadTank(mContext);
            else if (mQueryType == QueryType.CLEAR_PIPE_ALARM)
                ClearPipeAlarm(mContext);
            else if (mQueryType == QueryType.CLEAR_TANK_ALARM)
                ClearTankAlarm(mContext);
            else if (mQueryType == QueryType.CLEAR_SULFURIC_ALARM)
                ClearSulfuricAlarm(mContext);
            else if (mQueryType == QueryType.READ_OPTION)
                ReadOption(mContext);
            else if (mQueryType == QueryType.IGNORE_ALARM)
                IgnoreAlarm(mContext);
            else if (mQueryType == QueryType.BEGIN_WORK)
                WorkCommand(mContext, true);
            else if (mQueryType == QueryType.END_WORK)
                WorkCommand(mContext, false);
            else if (mQueryType == QueryType.DEV_LOG)
                SendLog(mContext);
        }
        catch (Exception e)
        {
            String msg = e.getMessage();

            if (msg != null)
                Log.d("WebManager", msg);
        }
    }

    private boolean SendLog(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQueryForDebugging(param, "Logger.jsp");
    }

    private boolean WorkCommand(Context context, boolean beginWork) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        if (param.contains("PipeID") == false)
            return false;

        /*if (beginWork)
            param += "&IgnoreType=0";
        else
            param += "&IgnoreType=1";*/

        if (param.contains("DeviceID") == false)
        {
            String deviceID = FirebaseInstanceId.getInstance().getToken();
            param += "&DeviceID=" + deviceID;
        }

        return ExecuteQuery(param, "BeginOrEndWork.jsp");
    }

    private boolean IgnoreAlarm(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "AlarmIgnore.jsp");
    }

    private boolean ReadOption(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "RequestOptions.jsp");
    }

    private boolean ClearTankAlarm(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "TankAlarmClear.jsp");
    }

    private boolean ClearSulfuricAlarm(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "TankLeakAlarmClear.jsp");
    }

    private boolean ClearPipeAlarm(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "AlarmClear.jsp");
    }

    private boolean ReadTank(Context context) throws IOException
    {
        return ExecuteQuery("", "RequestTankStatus.jsp");
    }

    private boolean ReadPipe(Context context) throws IOException
    {
        return ExecuteQuery("", "RequestPipeStatus.jsp");
    }

    private boolean ReadNotice(Context context) throws IOException
    {
        return ExecuteQuery("", "Notice.jsp");
    }

    private boolean RequestCertConfirm(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "RequestConfirmCertCode.jsp");
    }

    private boolean RequestConnectedTankList(Context context) throws IOException
    {
        return ExecuteQuery("", "RequestConnectedTankList.jsp");
    }

    private boolean RequestLastWorkHistory(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "RequestLastWorkHistory.jsp");
    }

    private boolean RequestCertCode(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "RequestCertCode.jsp");
    }

    // 승인된 사용자인지 여부 확인
    private boolean CheckUser(Context context) throws IOException
    {
        String serialNumber = getDeviceSerialNumber();

        if (serialNumber == null || serialNumber.length() == 0) {
            mResult = ResultType.NOT_ENOUGH_PARAMETER;
            return false;
        }

        //String param = "SerialNumber=" + serialNumber;
        String deviceID = FirebaseInstanceId.getInstance().getToken();

        if (deviceID == null || deviceID.length() == 0) {
            // App을 처음 설치하면 deviceID가 없다.
            deviceID = "temp";
            /*mResult = ResultType.NOT_ENOUGH_PARAMETER;
            return false;*/
        }

        //deviceID = "e8KeFPiaovE:APA91bFXGLC2DTrf9tRALuwOKgJOvLOnRtzZfayI9MfLEyV1RefEJ1_J7pEmeTCe5-rzDajtjNDo12cqj1ZagXDWC043PjX9HWFZAQ8S1Auxxh9iIYA9-Nt6tLD-0teLqkNbZLV4HzYD";
        //serialNumber = "ce031713d098b20402";
        String param = "DeviceID=" + deviceID + "&SerialNumber=" + serialNumber;
        return ExecuteQuery(param, "CheckUser.jsp");
    }

    private boolean ExecuteQueryForDebugging(String param, String siteURL) throws IOException
    {
        URL url = new URL(mContext.getString(R.string.debugging_url) + "/" + siteURL);
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();

        conn.setRequestMethod("POST");

        conn.setDoOutput(true);
        conn.setUseCaches(false);
        conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");

        conn.setConnectTimeout(3000);
        conn.setReadTimeout(3000);

        conn.connect();

        //param = URLEncoding(param);

        DataOutputStream out = null;
        OutputStream oStream = conn.getOutputStream();

        try {
            out = new DataOutputStream(oStream);
            out.write(param.getBytes("UTF-8"));
            //out.writeBytes(param);
            out.flush();
        }
        finally {
            if (out != null)
                out.close();
        }

        if (out == null) {
            conn.disconnect();
            return false;
        }

        InputStream input = conn.getInputStream();
        Scanner scan = new Scanner(input);

        boolean isBegin = false;

        while (scan.hasNext())
        {
            String str = scan.nextLine();

            if (str == null)
                continue;

            str = str.trim();

            if (str.length() == 0)
                continue;

            if (isBegin == false)
            {
                if (str.equals("Begin Data"))
                    isBegin = true;
            }
            else
            {
                if (str.equals("End Data"))
                    break;

                int nIndex = str.indexOf(':');

                if (nIndex < 0)
                {
                    mResultSet = null;
                    break;
                }

                String strValueType = str.substring(0, nIndex);
                String strValue = str.substring(nIndex + 2, str.length() - 1);

                if (strValueType.equals("ErrorCode"))
                {
                    try
                    {
                        int errorCode = Integer.parseInt(strValue);
                        mResult = toResultType(errorCode);
                        mResultSet = null;
                        break;
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    if (mResultSet == null)
                        mResultSet = new ArrayList();

                    mResultSet.add(strValue);
                }
            }
        }

        scan.close();
        conn.disconnect();

        if (mResult == ResultType.UNKNOWN && mResultSet != null)
        {
            mResult = ResultType.SUCCESS;
            return true;
        }

        return false;
    }

    private boolean ExecuteQuery(String param, String siteURL) throws IOException
    {
        URL url = new URL(mContext.getString(R.string.web_url) + "/" + siteURL);
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();

        conn.setRequestMethod("POST");

        conn.setDoOutput(true);
        conn.setUseCaches(false);
        conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");

        conn.setConnectTimeout(3000);
        conn.setReadTimeout(3000);

        conn.connect();

        //param = URLEncoding(param);

        DataOutputStream out = null;
        OutputStream oStream = conn.getOutputStream();

        try {
            out = new DataOutputStream(oStream);
            out.write(param.getBytes("UTF-8"));
            //out.writeBytes(param);
            out.flush();
        }
        finally {
            if (out != null)
                out.close();
        }

        if (out == null) {
            conn.disconnect();
            return false;
        }

        InputStream input = conn.getInputStream();
        Scanner scan = new Scanner(input);

        boolean isBegin = false;

        while (scan.hasNext())
        {
            String str = scan.nextLine();

            if (str == null)
                continue;

            str = str.trim();

            if (str.length() == 0)
                continue;

            if (isBegin == false)
            {
                if (str.equals("Begin Data"))
                    isBegin = true;
            }
            else
            {
                if (str.equals("End Data"))
                    break;

                int nIndex = str.indexOf(':');

                if (nIndex < 0)
                {
                    mResultSet = null;
                    break;
                }

                String strValueType = str.substring(0, nIndex);
                String strValue = str.substring(nIndex + 2, str.length() - 1);

                if (strValueType.equals("ErrorCode"))
                {
                    try
                    {
                        int errorCode = Integer.parseInt(strValue);
                        mResult = toResultType(errorCode);
                        mResultSet = null;
                        break;
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    if (mResultSet == null)
                        mResultSet = new ArrayList();

                    mResultSet.add(strValue);
                }
            }
        }

        scan.close();
        conn.disconnect();

        if (mResult == ResultType.UNKNOWN && mResultSet != null)
        {
            mResult = ResultType.SUCCESS;
            return true;
        }

        return false;
    }

    private ResultType toResultType(int nType)
    {
        for (ResultType type : ResultType.values())
        {
            if (type.toInt() == nType)
                return type;
        }

        return ResultType.UNKNOWN;
    }

    public static String getDeviceSerialNumber()
    {
        try
        {
            return (String)Build.class.getField("SERIAL").get(null);
        }
        catch (Exception ignored)
        {
        }

        return "";
    }
}
