package kr.co.unes.aqm.lbs.map.data;

import android.util.Log;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.StringTokenizer;

import org.apache.http.HttpResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import kr.co.unes.aqm.lbs.map.task.SensorCheckAsync;
import kr.co.unes.aqm.lbs.map.task.SensorValueAsync;

/**
 * Created by skkim on 2016-08-29.
 */
public class AirSensorManager {

    private static AirSensorManager mInstance = null;
    public static AirSensorManager getInstance() {
        if(mInstance == null)
            mInstance = new AirSensorManager();
        return mInstance;
    }

    private ArrayList<AirSensorData> arList = new ArrayList<AirSensorData>();

    private AirSensorManager()
    {
        UpdateData();
    }

    public void UpdateData()
    {
        String args = loadSensorData();
        Log.d("WebResult", args);
        try {
            JSONObject object = new JSONObject(args);

            //object.getJSONObject();
            JSONObject obj1 = object.getJSONObject("SensorList");
            JSONArray Array = obj1.getJSONArray("Sensors");
            for (int i = 0; i < Array.length(); i++) {

                JSONArray insideObject = Array.getJSONArray(i);

                String szID = ""+(i + 1);
                String szName = insideObject.getString(0);
                String szPos = insideObject.getString(1);

                try
                {
                    StringTokenizer tokenizer = new StringTokenizer(szPos, ",", false);
                    String szLat = tokenizer.nextToken();
                    String szLon = tokenizer.nextToken();

                    AirSensorData data = new AirSensorData(szID,szName, "지역평균");
                    data.setLatitude(szLat);
                    data.setLongitutde(szLon);
                    data.setValue("0");

                    arList.add(data);
                }
                catch (Exception exx)
                {
                }

            } // for

        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public ArrayList<AirSensorData> getSensorList()
    {
        return arList;
    }

    private String loadSensorData()
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Marker/Average/json";
            HttpResponse response = new SensorCheckAsync().execute(testURL, "ID=1").get();


            BufferedReader bufreader = new BufferedReader(
                    new InputStreamReader(response.getEntity().getContent(),
                            "utf-8"));

            String line = null;
            while ((line = bufreader.readLine()) != null) {
                result += line;
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();

        }
        return result;
    }

    public float ReedSensorValue(String nodeId, String seosnrCode)
    {
        String args = getSensorValue(nodeId, seosnrCode);
        Log.d("WebResult", args);
        try {
            JSONObject object = new JSONObject(args);

            //object.getJSONObject();
            JSONObject obj1 = object.getJSONObject("Sensor");
            JSONObject obj2= obj1.getJSONObject("value");

            float value = (float)obj2.getDouble("SensorValue");
            return value;


        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
        return 0.0f;
    }

    private String getSensorValue(String nodeId, String sensorCode)
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Sensor/value/"+sensorCode + "/"+ nodeId;
            Log.d("CallURL", testURL);
            HttpResponse response = new SensorValueAsync().execute(testURL).get();


            BufferedReader bufreader = new BufferedReader(
                    new InputStreamReader(response.getEntity().getContent(),
                            "utf-8"));

            String line = null;
            while ((line = bufreader.readLine()) != null) {
                result += line;
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();

        }
        return result;
    }
}

