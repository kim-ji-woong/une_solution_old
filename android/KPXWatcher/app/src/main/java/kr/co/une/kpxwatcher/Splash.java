package kr.co.une.kpxwatcher;

import android.app.Activity;
import android.app.DownloadManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.os.Handler;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.widget.ImageView;
import android.Manifest;

import com.bumptech.glide.Glide;
import com.google.firebase.iid.FirebaseInstanceId;

import java.io.File;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.List;
import java.util.ArrayList;

public class Splash extends AppCompatActivity {
    public enum UserType
    {
        CERTIFICATED_USER(0), NO_ALARM_OFF(1), ALREADY_CERT_REQUEST_USER(2), NEED_CERT_CONFIRM_USER(3), NEED_CERTIFY(4), UNKNOWN(5);

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

    public enum PermissionMode { NONE, WAITING, ACCEPTED, DENIED };

    private final int REQUEST_PERMISSION_WRITE_STORAGE = 1;

    private PermissionMode m_permissionRequestResult = PermissionMode.ACCEPTED.NONE;

    private boolean m_pipeAccess = false;
    private List<Integer> m_tankAccessIDs = new ArrayList();
    private List<Integer> m_tankItems = new ArrayList();
    private List<Integer> m_pipeItems = new ArrayList();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_splash);

        if (CheckLatestVersion() == false)
            return;

        init();
    }

    private void init()
    {
        UserType userType = CheckUser();

        setImage((ImageView)findViewById(R.id.imgSplashLogo), R.drawable.main_background);

        if (userType == UserType.ALREADY_CERT_REQUEST_USER)
        {
            MainActivity.showAlert("아직 인증요청에 대한 승인이 이루어지지 않았습니다.", "알림", this);

            /*try {
                Thread.sleep(5000);
            }
            catch (Exception e)
            {
            }

            finish();*/
        }
        else if (userType == UserType.UNKNOWN)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.setMessage("서버에 접속할 수 없습니다.\r\n관리자에게 문의해 주세요.");
            builder.setTitle("접속오류");

            builder.setPositiveButton("확인", new DialogInterface.OnClickListener()
            {
                public void onClick(DialogInterface dialog, int which)
                {
                    finish();
                }
            });

            builder.show();
        }
        else {
            Handler hd = new Handler();
            splashhandler handler = new splashhandler();
            handler.setUserType(userType);
            handler.setPipeAccess(m_pipeAccess);
            handler.setTankAccess(m_tankAccessIDs);
            handler.setTankItems(m_tankItems);
            handler.setPipeItems(m_pipeItems);
            //handler.setActivity(this);
            hd.post(handler);
            //hd.postDelayed(new splashhandler() , 3000);
        }
    }

    // Return 값 : true(버전 변경을 하지 않는다.)
    //             false(버전 변경을 위한 사용자 입력을 기다린다.)
    private boolean CheckLatestVersion()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.READ_OPTION);
        mgr.setParameter("PropertyName", "PTMSVersion");
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
            String currentVersion = this.getResources().getString(R.string.version);

            List<String> results = mgr.getResultSet();

            if (results != null && results.size() > 0)
            {
                String latestVersion = results.get(0);
                int result = currentVersion.compareTo(latestVersion);

                if (result < 0) {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);

                    builder.setMessage("최신 버전이 존재합니다.\r\n업데이트 하시겠습니까?");
                    builder.setTitle("확인");

                    builder.setNegativeButton("네", new DialogInterface.OnClickListener()
                    {
                        public void onClick(DialogInterface dialog, int which)
                        {
                            if(grantExternalStoragePermission())
                            {
                                Delete_apk();
                            }
                        }
                    });

                    builder.setPositiveButton("아니오", new DialogInterface.OnClickListener()
                    {
                        public void onClick(DialogInterface dialog, int which)
                        {
                            init();
                        }
                    });

                    builder.show();

                    return false;
                }
            }
        }

        return true;
    }

    private boolean grantExternalStoragePermission() {
        if (Build.VERSION.SDK_INT >= 23) {
            if (checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED) {
                return true;
            }else{
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 1);

                return false;
            }
        }
        else{
            return true;
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        //if (Build.VERSION.SDK_INT >= 23) {
            //if(grantResults[0]== PackageManager.PERMISSION_GRANTED){
                Delete_apk();
            //}
        //}
    }

    private void Delete_apk()
    {
        String PATH = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS) + "/" + getString(R.string.apk_name);
        //File path = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS);

        //File file = new File(path.getPath(), getString(R.string.apk_name));
        File file = new File(PATH);
        if (file.exists()) {
            if (!file.delete()) {
                file.deleteOnExit();
            }
        }

        updateApp();
    }

    private void updateApp()
    {
        String url = this.getString(R.string.install_url);

        Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
        startActivity(intent);
        finish();
        /*try
        {
            String PATH = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS) + "/KPXWatcher.apk";
            final Uri uri = Uri.parse("file://" + PATH);

            File file = new File(PATH);

            if (file.exists())
                file.delete();

            String apkUrl = this.getString(R.string.apk_url);

            //set downloadmanager
            DownloadManager.Request request = new DownloadManager.Request(Uri.parse(apkUrl));
            request.setTitle(this.getString(R.string.app_name));

            //set destination
            request.setDestinationUri(uri);

            // get download service and enqueue file
            final DownloadManager manager = (DownloadManager) getSystemService(Context.DOWNLOAD_SERVICE);
            final long downloadId = manager.enqueue(request);

            //set BroadcastReceiver to install app when .apk is downloaded
            BroadcastReceiver onComplete = new BroadcastReceiver() {
                public void onReceive(Context ctxt, Intent intent) {
                    Intent install = new Intent(Intent.ACTION_VIEW);
                    install.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
                    install.setDataAndType(uri,
                            manager.getMimeTypeForDownloadedFile(downloadId));
                    startActivity(install);

                    unregisterReceiver(this);
                    finish();
                }
            };

            //register receiver for when .apk download is compete
            registerReceiver(onComplete, new IntentFilter(DownloadManager.ACTION_DOWNLOAD_COMPLETE));
        } catch (Exception e) {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.setMessage("설치파일을 다운로드 받는데 실패하였습니다.");
            builder.setTitle("오류");

            builder.setPositiveButton("확인", new DialogInterface.OnClickListener()
            {
                public void onClick(DialogInterface dialog, int which)
                {
                    finish();
                }
            });
        }*/
    }

    /*public  boolean haveStoragePermission() {
        if (Build.VERSION.SDK_INT >= 23) {
            if (checkSelfPermission(android.Manifest.permission.WRITE_EXTERNAL_STORAGE)
                    == PackageManager.PERMISSION_GRANTED) {
                Log.e("Permission Success","You have permission");
                return true;
            } else {

                Log.e("Permission error","You have asked for permission");
                ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.WRITE_EXTERNAL_STORAGE}, REQUEST_PERMISSION_WRITE_STORAGE);

                int permission = checkSelfPermission(android.Manifest.permission.WRITE_EXTERNAL_STORAGE);
                m_permissionRequestResult = PermissionMode.WAITING;

                return false;
            }
        }
        else { //you dont need to worry about these stuff below api level 23
            Log.e("Permission error","You already have the permission");
            return true;
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String permissions[], int[] grantResults) {
        switch (requestCode) {
            case REQUEST_PERMISSION_WRITE_STORAGE:

                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    // 권한 허가
                    // 해당 권한을 사용해서 작업을 진행할 수 있습니다
                    m_permissionRequestResult = PermissionMode.ACCEPTED;
                    updateApp();
                } else {
                    // 권한 거부
                    // 사용자가 해당권한을 거부했을때 해주어야 할 동작을 수행합니다
                    m_permissionRequestResult = PermissionMode.DENIED;
                    init();
                }
                return;
        }
    }*/

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

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
            type = setUserType(mgr);
        else if (mgr.getResult() == WebManager.ResultType.ALREADY_REQUESTED)
            type = UserType.ALREADY_CERT_REQUEST_USER;
        else if (mgr.getResult() == WebManager.ResultType.NEED_CERT_CODE_CONFIRM)
            type = UserType.NEED_CERT_CONFIRM_USER;
        else if (mgr.getResult() == WebManager.ResultType.NOT_CERTIFIED_USER)
            type = UserType.NEED_CERTIFY;

        return type;
    }

    private UserType setUserType(WebManager mgr)
    {
        List<String> results = mgr.getResultSet();
        UserType userType = UserType.UNKNOWN;

        if (results == null || results.size() == 0)
            return userType;

        int nResultCount = results.size();

        try {
            int nType = Integer.parseInt(results.get(0));

            for (UserType type : UserType.values())
            {
                if (type.toInt() == nType) {
                    userType = type;
                    break;
                }
            }

            if (nResultCount >= 2)
            {
                int access = Integer.parseInt(results.get(1));

                if (access == 1)
                    m_pipeAccess = true;
            }

            if (nResultCount >= 3)
            {
                getIDs(results.get(2), m_pipeItems);
            }

            if (nResultCount >= 4)
            {
                getIDs(results.get(3), m_tankAccessIDs);
            }

            if (nResultCount >= 5)
            {
                getIDs(results.get(4), m_tankItems);
            }

            /*MenuActivity.setPipeAccess(pipeAccess);
            MenuActivity.setTankAccess(tankAccessIDs);
            MenuActivity.setTankItems(tankItems);*/
        }
        catch (Exception e)
        {
        }

        return userType;
    }

    public static void getIDs(String str, List<Integer> ids)
    {
        int id;
        String[] tokens = str.split(",");

        for (String strToken : tokens)
        {
            try
            {
                id = Integer.parseInt(strToken.trim());
                ids.add(id);
            }
            catch (Exception e)
            {
            }
        }
    }

    private void setImage(ImageView view, int nImageID)
    {
        if (view == null)
            return;

        Glide.with(this).load(nImageID).into(view);
    }

    public static UserType getUserType(String strUserType)
    {
        try
        {
            int nUserType = Integer.parseInt(strUserType);

            for (UserType type : UserType.values())
            {
                if (type.toInt() == nUserType)
                    return type;
            }
        }
        catch (Exception e)
        {
        }

        return UserType.UNKNOWN;
    }

    private class splashhandler implements Runnable{

        private UserType userType = UserType.UNKNOWN;
        private boolean pipeAccess = false;
        private List<Integer> tankAccessIDs = null;
        private List<Integer> tankItems = null;
        private List<Integer> pipeItems = null;
        //private Activity callActivity = null;

        public void setUserType(UserType userType)
        {
            this.userType = userType;
        }

        public void setPipeAccess(boolean pipeAccess)
        {
            this.pipeAccess = pipeAccess;
        }

        public void setTankAccess(List<Integer> tankAccessIDs)
        {
            this.tankAccessIDs = tankAccessIDs;
        }

        public void setTankItems(List<Integer> tankItems)
        {
            this.tankItems = tankItems;
        }
        public void setPipeItems(List<Integer> pipeItems)
        {
            this.pipeItems = pipeItems;
        }

        /*public void setActivity(Activity activity)
        {
            callActivity = activity;
        }*/

        public void run() {
            if (userType == UserType.CERTIFICATED_USER || userType == UserType.NO_ALARM_OFF)
                runMenu(userType);
            /*else if (userType == UserType.ALREADY_CERT_REQUEST_USER) {
                //MainActivity.showAlert("아직 인증요청에 대한 승인이 이루어지지 않았습니다.", "알림", callActivity);
                runMain();
            }*/
            else if (userType == UserType.NEED_CERT_CONFIRM_USER)
                runCertConfirm();
            else
                runCert();
        }

        public void runMain() {
            startActivity(new Intent(getApplication(), MainActivity.class)); // 로딩이 끝난후 이동할 Activity
            Splash.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }

        public void runMenu(UserType type) {
            Intent intent = new Intent(getApplication(), MenuActivity.class);
            intent.putExtra("UserType", Integer.toString(type.toInt()));
            intent.putExtra("PipeAccess", m_pipeAccess ? "1" : "0");
            intent.putExtra("TankAccess", getIDs(tankAccessIDs));
            intent.putExtra("TankItems", getIDs(tankItems));
            intent.putExtra("PipeItems", getIDs(pipeItems));

            startActivity(intent); // 로딩이 끝난후 이동할 Activity

            Splash.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }

        private String getIDs(List<Integer> idList)
        {
            String ids = "";

            for (int nID : idList)
            {
                if (ids.length() == 0)
                    ids = Integer.toString(nID);
                else
                    ids += ", " + Integer.toString(nID);
            }

            return ids;
        }

        public void runCert() {
            startActivity(new Intent(getApplication(), RequestCertActivity.class)); // 로딩이 끝난후 이동할 Activity
            Splash.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }

        public void runCertConfirm()
        {
            startActivity(new Intent(getApplication(), CertConfirmActivity.class)); // 로딩이 끝난후 이동할 Activity
            Splash.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }
    }
}
