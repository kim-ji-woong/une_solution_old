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
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;

/**
 * Created by skkim on 2016-08-31.
 */
public class NodeCheckAsync extends AsyncTask<String, Void, HttpResponse>
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
        HttpPost httpPost = new HttpPost(httpHost);


        if( param.length > 1)
        {
            String[] pm 	= param[1].split("&");
            for(int i = 0; i < pm.length; i++)
            {
                String[] newPm = pm[i].split("=");
                postParam.add(new BasicNameValuePair(newPm[0], newPm[1]));
            }
        }

        try
        {
            entity = new UrlEncodedFormEntity(postParam, "UTF-8");

        } catch (java.io.UnsupportedEncodingException e) {
            e.printStackTrace();
        }

        httpPost.setEntity(entity);

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
