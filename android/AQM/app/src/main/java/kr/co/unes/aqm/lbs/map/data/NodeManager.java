package kr.co.unes.aqm.lbs.map.data;

import android.util.Log;

import org.apache.http.HttpResponse;
import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.StringTokenizer;

import kr.co.unes.aqm.lbs.map.task.NodeCheckAsync;
import kr.co.unes.aqm.lbs.map.task.SiteNodeAsync;

/**
 * Created by skkim on 2016-12-26.
 */

public class NodeManager {

    private static NodeManager mInstance = null;
    public static NodeManager getInstance() {
        if(mInstance == null)
            mInstance = new NodeManager();
        return mInstance;
    }


    private ArrayList<NodeData> arList = new ArrayList<NodeData>();
    private String testURL = "http://unes.iptime.org:8112/AQM/Search/nodeList/json";

    private NodeManager()
    {

    }

    public ArrayList<NodeData> getNodeList()
    {
        return arList;
    }


    public void searchData(String szName)
    {
        arList.removeAll(arList);
        String args = loadSensorData(szName);
        parseSensorData(args);
    }


    private void parseSensorData(String args)
    {
        Log.d("WebResult", args);
        try {
            JSONObject object = new JSONObject(args);

            JSONObject obj1 = object.getJSONObject("SensorList");
            JSONArray Array = obj1.getJSONArray("Sensors");
            for (int i = 0; i < Array.length(); i++) {

                JSONObject insideObject = Array.getJSONObject(i);
                String szID = insideObject.getString("ID");
                String szName = insideObject.getString("locationName");
                String szTel = insideObject.getString("phone");
                String szAddress = insideObject.getString("mainAddress");
                try
                {
                    String szLat = insideObject.getString("locationX");
                    String szLon = insideObject.getString("locationY");

                    NodeData data = new NodeData(szID, szName, szAddress);
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

    public void searchData(String szDepth1, String szDepth2, String szDepth3, String szDepth4)
    {
        arList.removeAll(arList);
        String args = loadSensorData(szDepth1, szDepth2, szDepth3, szDepth4);
        parseSensorData(args);
    }

    private String loadSensorData(String szSearchName)
    {
        String result = "";
        try
        {
            String szParam = "SearchType=1";
            if( szSearchName != null && szSearchName.compareTo("") != 0) {
                szParam = szParam + "&Name=" + szSearchName;
            }
            HttpResponse response = new NodeCheckAsync().execute(testURL, szParam).get();
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

    private String loadSensorData(String szDepth1, String szDepth2, String szDepth3, String szDepth4)
    {
        String result = "";
        try
        {
            String szParam = "SearchType=2";
            if( szDepth1 != null && szDepth1.compareTo("") != 0) {
                szParam = szParam + "&depth1=" + szDepth1;
                if (szDepth2 != null && szDepth2.compareTo("") != 0) {
                    szParam = szParam + "&depth2=" + szDepth2;
                    if (szDepth3 != null && szDepth3.compareTo("") != 0) {
                        szParam = szParam + "&depth3=" + szDepth3;
                        if (szDepth4 != null && szDepth4.compareTo("") != 0)
                            szParam = szParam + "&depth4=" + szDepth4;
                    }
                }
            }

            HttpResponse response = new NodeCheckAsync().execute(testURL, szParam).get();
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

    public List<String> ReadSiteNode(String siteID)
    {
        String args = getNodeList(siteID);
        Log.d("WebResult", args);
        List<String> result = new ArrayList<String>();
        try {
            JSONObject object = new JSONObject(args);

            //object.getJSONObject();
            JSONObject obj1 = object.getJSONObject("Site");
            JSONArray Array = obj1.getJSONArray("values");

            for (int i = 0; i < Array.length(); i++) {

                JSONObject insideObject = Array.getJSONObject(i);
                String nodeID = insideObject.getString("nodeID");
                result.add(nodeID);
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
        return result;
    }

    private String getNodeList(String siteID)
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Site/nodes/"+siteID;
            Log.d("CallURL", testURL);
            HttpResponse response = new SiteNodeAsync().execute(testURL).get();


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

