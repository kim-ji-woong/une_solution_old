package kr.co.une.energyindustrye_sop;

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
        ALREADY_REGISTERED(1),
        NOT_ENOUGH_PARAMETER(2),
        NOT_REGISERED_DEVICE(3),
        NO_LINKED_SOP(4),
        NO_WEB_SERVER_URL(5);

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
        CHECK_USER,
        REGIST_USER,
        REQUEST_SOP_DATA,
        REQUEST_URL,
        REQUEST_DISASTER_EQUIPMENT_LIST,
        SOP_COMMAND
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
            else if (mQueryType == QueryType.REGIST_USER)
                RegistUser(mContext);
            else if (mQueryType == QueryType.REQUEST_SOP_DATA)
                RequestSOPData(mContext);
            else if (mQueryType == QueryType.REQUEST_URL)
                RequestURL(mContext);
            else if (mQueryType == QueryType.REQUEST_DISASTER_EQUIPMENT_LIST)
                RequestDisasterEquipmentList(mContext);
            else if (mQueryType == QueryType.SOP_COMMAND)
                SendSOPCommand(mContext);
        }
        catch (Exception e)
        {
            String msg = e.getMessage();

            if (msg != null)
                Log.d("WebManager", msg);
        }
    }

    private boolean RequestURL(Context context) throws IOException
    {
        URL url = new URL(mContext.getString(R.string.begin_url));
        return ExecuteQuery2("", url);
    }

    private boolean RequestDisasterEquipmentList(Context context) throws IOException
    {
        /*String strBeginURL = mContext.getString(R.string.begin_url);
        String strIP = getIP(strBeginURL);

        if (strIP.equals(""))
            return false;

        String strHalfURL = getURLExceptJSP(strBeginURL);

        if (strHalfURL.equals(""))
            return false;

        String strJSPFile = mContext.getString(R.string.disaster_equipment_request_file);
        String strURL = strHalfURL + strJSPFile;*/

        String strURL = Splash.getWebServerURL() + "/" + mContext.getString(R.string.disaster_equipment_request_file);
        URL url = new URL(strURL);
        //return ExecuteQuery2("Host=" + strIP, url);
        return ExecuteQuery2("Host=127.0.0.1", url);
    }

    private String getURLExceptJSP(String url)
    {
        int nIndex = url.lastIndexOf('/');

        if (nIndex < 0)
            return "";

        return url.substring(0, nIndex + 1);
    }

    private String getIP(String url)
    {
        int nIndex1 = url.indexOf("//");

        if (nIndex1 >= 0)
            nIndex1 += 2;
        else
            nIndex1 = 0;

        int nIndex2 = url.indexOf(':', nIndex1);

        if (nIndex2 < 0)
        {
            nIndex2 = url.indexOf('/', nIndex1);
        }

        if (nIndex2 < nIndex1)
            return "";

        String strIP = url.substring(nIndex1, nIndex2);
        return strIP;
    }

    private boolean SendSOPCommand(Context context) throws  IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "SOPCommand.jsp");
    }

    private boolean RequestSOPData(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "RequestSOPData.jsp");
    }

    private boolean RegistUser(Context context) throws IOException
    {
        String param = "";

        for (Map.Entry<String, String> pair : mapParameters.entrySet())
        {
            if (param.length() == 0)
                param = pair.getKey() + "=" + pair.getValue();
            else
                param += "&" + pair.getKey() + "=" + pair.getValue();
        }

        return ExecuteQuery(param, "RegistUser.jsp");
    }

    // 등록된 사용자인지 여부 확인
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

        String param = "DeviceID=" + deviceID + "&SerialNumber=" + serialNumber;
        return ExecuteQuery(param, "CheckUser.jsp");
    }

    private boolean ExecuteQuery(String param, String siteURL) throws IOException
    {
        URL url = new URL(Splash.getWebServerURL() + "/" + siteURL);
        //URL url = new URL(mContext.getString(R.string.web_url) + "/" + siteURL);
        return ExecuteQuery2(param, url);
    }

    private boolean ExecuteQuery2(String param, URL url) throws IOException
    {
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
