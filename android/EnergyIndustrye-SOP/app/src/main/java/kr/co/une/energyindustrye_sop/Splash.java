package kr.co.une.energyindustrye_sop;

import android.app.Activity;
import android.content.DialogInterface;
import android.content.Intent;
import android.media.Image;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AlertDialog;
import android.widget.ImageView;
import com.bumptech.glide.Glide;
import com.google.firebase.iid.FirebaseInstanceId;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by 지웅 on 2017-05-18.
 */

public class Splash extends Activity {
    public enum UserType
    {
        REGISTERED_USER(0), ALARM_OCCURED(1), UNKNOWN(2);

        private final int value;
        private UserType(int value)
        {
            this.value = value;
        }

        public int toInt()
        {
            return value;
        }
    };

    private List<String> m_resultSet = null;

    private static String m_strWebServerURL = "";

    public static String getWebServerURL()
    {
        return m_strWebServerURL;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);

        ImageView imgMain = (ImageView)findViewById(R.id.imgSplashLogo);
        Glide.with(this).load(R.drawable.logo).into(imgMain);

        m_strWebServerURL = readBeginURL();

        if (m_strWebServerURL.length() == 0)
        {
            MainActivity.showAlert("인터넷에 접속할 수 없습니다.", "오류", getApplicationContext());
        }
        else {
            UserType userType = CheckUser();

            Handler hd = new Handler();
            splashhandler handler = new splashhandler();
            handler.setUserType(userType);
            handler.setResultSet(m_resultSet);
            hd.post(handler);

            //Handler hd = new Handler();
            //hd.postDelayed(new splashhandler() , 1000); // 1초 후에 hd Handler 실행
        }
    }

    private String readBeginURL()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.REQUEST_URL);
        mgr.start();

        int nTimeOut = 5000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN)
        {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            }
            catch (Exception e)
            {
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS) {
            List<String> results = mgr.getResultSet();

            if (results != null && results.size() > 0) {
                return results.get(0);
            }
        }

        return "";
    }

    private UserType CheckUser()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.CHECK_USER);
        mgr.start();

        int nTimeOut = 5000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN)
        {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            }
            catch (Exception e)
            {
            }
        }

        UserType type = UserType.UNKNOWN;

        if (mgr.getResult() == WebManager.ResultType.SUCCESS) {
            List<String> results = mgr.getResultSet();

            if (results != null && results.size() >= 3) {
                m_resultSet = results;
                type = UserType.ALARM_OCCURED;
            }
            else {
                m_resultSet = new ArrayList();
                m_resultSet.add(mgr.getResultSet().get(0));
                type = UserType.REGISTERED_USER;
            }
        }

        return type;
    }

    private class splashhandler implements Runnable{
        private UserType userType = UserType.UNKNOWN;
        private List<String> resultSet = null;

        public void setUserType(UserType userType)
        {
            this.userType = userType;
        }

        public void setResultSet(List<String> results)
        {
            resultSet = results;
        }

        public void run() {
            if (userType == UserType.REGISTERED_USER) {
                //String strParam = resultSet.get(0);

                Intent intent = new Intent(getApplication(), MainActivity.class); // 로딩이 끝난후 이동할 Activity
                intent.putExtra("Alarm", "0");

                startActivity(intent); // 로딩이 끝난후 이동할 Activity
            }
            else if (userType == UserType.ALARM_OCCURED)
            {
                String strTitle = resultSet.get(1);
                String strMessage = resultSet.get(2);

                Intent intent = new Intent(getApplication(), MainActivity.class); // 로딩이 끝난후 이동할 Activity
                intent.putExtra("Alarm", "1");
                intent.putExtra("Title", strTitle);
                intent.putExtra("Message", strMessage);

                startActivity(intent);
            }
            else
                startActivity(new Intent(getApplication(), InputPhoneNumberActivity.class)); // 로딩이 끝난후 이동할 Activity

            Splash.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }
    }
}
