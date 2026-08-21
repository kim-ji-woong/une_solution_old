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

import kr.co.unes.aqm.lbs.map.task.AddressCheckAsync;

/**
 * Created by skkim on 2016-12-23.
 */

public class AddressManager {


    private static AddressManager mInstance = null;
    public static AddressManager getInstance() {
        if(mInstance == null)
            mInstance = new AddressManager();
        return mInstance;
    }

    private ArrayList<String> arList = new ArrayList<String>();
    private ArrayList<String> arListDepth2 = new ArrayList<String>();
    private ArrayList<String> arListDepth3 = new ArrayList<String>();
    private ArrayList<String> arListDepth4 = new ArrayList<String>();


    private AddressManager()
    {
        updateData();
    }

    public void updateData() {
        String args = loadAddressDepth1();
        Log.d("WebResult", args);
        if (!args.contains("HTTP Status 404"))
        {
            try {
                JSONObject object = new JSONObject(args);
                JSONObject obj1 = object.getJSONObject("Area");
                JSONArray Array = obj1.getJSONArray("depth1s");
                for (int i = 0; i < Array.length(); i++) {
                    JSONObject insideObject = Array.getJSONObject(i);
                    String szPos = insideObject.getString("depth1");
                    arList.add(szPos);
                } // for
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }
    }

    public List<String> getAreaDepth1s()
    {
        return arList;
    }

    public List<String> getAreaDepth2s(String szAreaDepth1)
    {

        arListDepth2.clear();

        String args = loadAddressDepth2(szAreaDepth1);
        Log.d("WebResult", args);
        if(args.contains("HTTP Status 404"))
            return arListDepth2;
        try {
            JSONObject object = new JSONObject(args);
            JSONObject obj1 = object.getJSONObject("Area");
            JSONArray Array = obj1.getJSONArray("depth2s");
            for (int i = 0; i < Array.length(); i++) {
                JSONObject insideObject = Array.getJSONObject(i);
                String szPos = insideObject.getString("depth2");
                if(szPos != null && szPos.compareTo("") != 0)
                    arListDepth2.add(szPos);
            } // for
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
        return arListDepth2;
    }

    public List<String> getAreaDepth3s(String szAreaDepth1, String szAreaDepth2)
    {

        arListDepth3.clear();

        String args = loadAddressDepth3(szAreaDepth1, szAreaDepth2);
        Log.d("WebResult", args);
        if(args.contains("HTTP Status 404"))
            return arListDepth3;
        try {
            JSONObject object = new JSONObject(args);
            JSONObject obj1 = object.getJSONObject("Area");
            JSONArray Array = obj1.getJSONArray("depth3s");
            for (int i = 0; i < Array.length(); i++) {
                JSONObject insideObject = Array.getJSONObject(i);
                String szPos = insideObject.getString("depth3");
                if(szPos != null && szPos.compareTo("") != 0)
                    arListDepth3.add(szPos);
            } // for
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
        return arListDepth3;
    }


    public List<String> getAreaDepth4s(String szAreaDepth1, String szAreaDepth2,  String szAreaDepth3)
    {

        arListDepth4.clear();

        String args = loadAddressDepth4(szAreaDepth1, szAreaDepth2, szAreaDepth3);
        Log.d("WebResult", args);
        if(args.contains("HTTP Status 404"))
            return arListDepth4;
        try {
            JSONObject object = new JSONObject(args);
            JSONObject obj1 = object.getJSONObject("Area");
            JSONArray Array = obj1.getJSONArray("depth4s");
            for (int i = 0; i < Array.length(); i++) {
                JSONObject insideObject = Array.getJSONObject(i);
                String szPos = insideObject.getString("depth4");
                if(szPos != null && szPos.compareTo("") != 0)
                    arListDepth4.add(szPos);
            } // for
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
        return arListDepth4;
    }

    private String loadAddressDepth4(String szDepth1, String szDepth2, String szDepth3)
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Area/depth4s/" + szDepth1+ "/" + szDepth2 + "/" + szDepth3;
            HttpResponse response = new AddressCheckAsync().execute(testURL).get();

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

    private String loadAddressDepth3(String szDepth1, String szDepth2)
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Area/depth3s/" + szDepth1+ "/" + szDepth2;
            HttpResponse response = new AddressCheckAsync().execute(testURL).get();

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

    private String loadAddressDepth2(String szDepth1)
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Area/depth2s/" + szDepth1;
            HttpResponse response = new AddressCheckAsync().execute(testURL).get();

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

    private String loadAddressDepth1()
    {
        String result = "";
        try
        {
            String testURL = "http://unes.iptime.org:8112/AQM/Area/depth1s";
            HttpResponse response = new AddressCheckAsync().execute(testURL).get();

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
