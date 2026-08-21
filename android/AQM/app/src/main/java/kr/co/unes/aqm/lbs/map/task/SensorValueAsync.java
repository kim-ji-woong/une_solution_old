package kr.co.unes.aqm.lbs.map.task;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URL;
import java.io.IOException;
import java.util.List;
import java.util.ArrayList;

import android.os.AsyncTask;
import android.os.Bundle;

import org.apache.http.HttpResponse;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.HttpConnectionParams;

/**
 * Created by skkim on 2016-08-31.
 */
public class SensorValueAsync extends AsyncTask<String, Void, HttpResponse>
{
    @Override
    protected void onPreExecute() {
        super.onPreExecute();
    }

    @Override
    protected HttpResponse doInBackground(String... param)
    {
        String httpHost = param[0];

        List postParam = new ArrayList();
        UrlEncodedFormEntity entity = null;

        HttpResponse response = null;

        DefaultHttpClient client = new DefaultHttpClient();
        HttpConnectionParams.setConnectionTimeout(client.getParams(), 10000);
        HttpGet httpPost = new HttpGet(httpHost);



        try
        {
            response = client.execute(httpPost);
        }
        catch(org.apache.http.client.ClientProtocolException e)
        {
            e.printStackTrace();
            client.getConnectionManager().shutdown();    // 연결 지연 종료
        }

        catch(IOException e)
        {
            e.printStackTrace();
            client.getConnectionManager().shutdown();    // 연결 지연 종료
        }
        return response;
    }

    @Override
    protected void onPostExecute(HttpResponse result) {

        super.onPostExecute(result);

    }

}
