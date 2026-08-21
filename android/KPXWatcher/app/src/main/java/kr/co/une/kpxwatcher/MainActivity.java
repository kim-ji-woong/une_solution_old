package kr.co.une.kpxwatcher;

        import android.app.Activity;
        import android.content.Context;
        import android.content.Intent;
        import android.content.res.Resources;
        import android.media.Ringtone;
        import android.media.RingtoneManager;
        import android.net.Uri;
        import android.os.Vibrator;
        import android.support.v7.app.AlertDialog;
        import android.support.v7.app.AppCompatActivity;
        import android.os.Bundle;
        import android.util.DisplayMetrics;
        import android.util.Log;
        import android.view.Display;
        import android.widget.ImageView;
        import android.widget.TextView;

        import com.google.firebase.iid.FirebaseInstanceId;

        import java.util.Set;

public class MainActivity extends AppCompatActivity {
    public enum  ActivityType { NONE, Menu, PipeMonitor, TankMonitor };

    private static MainActivity m_instance = null;

    private static int m_nStandardScreenWidth = 1080;
    private static int m_nStandardScreenHeight = 1920;

    public static MainActivity Instance()
    {
        return m_instance;
    }

    public static int getStandardScreenWidth()
    {
        return m_nStandardScreenWidth;
    }

    public static int getStandardScreenHeight()
    {
        return m_nStandardScreenHeight;
    }

    private static ActivityType m_currentActivity = ActivityType.NONE;

    public static ActivityType getCurrentActivity()
    {
        return m_currentActivity;
    }

    public static void setCurrentActivity(ActivityType type)
    {
        m_currentActivity = type;
        Log.d("ActivityType", "Current Activity : " + type.toString());
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        m_instance = this;

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_main);

        //String refreshedToken = FirebaseInstanceId.getInstance().getToken();
        //Log.d("MainActivity", "Token: " + refreshedToken);

        //setImageSizeNLocation((ImageView) findViewById(R.id.main_background), R.drawable.logo, 810, 481);
    }

    public void onNotification(String message) {
        //TextView text = (TextView)findViewById(R.id.mainText);
        //text.setText(message);

        // 알람 소리와 진동을 동시에 사용
        PlayNotificationSound();
        Vibrate();

        final String msg = message;

        new Thread(new Runnable() {
            public void run() {
                final TextView text = (TextView)findViewById(R.id.mainText);

                text.post(new Runnable() {
                    public void run() {
                        text.setText(msg);
                    }
                });
            }
        }).start();
    }

    private void Vibrate()
    {
        Vibrator vibrator = (Vibrator)getSystemService(Context.VIBRATOR_SERVICE);
        vibrator.vibrate(2000);
    }

    private void PlayNotificationSound()
    {
        Uri uri = RingtoneManager.getActualDefaultRingtoneUri(

                getApplicationContext(), RingtoneManager.TYPE_NOTIFICATION);

        Ringtone ringtone = RingtoneManager

                .getRingtone(getApplicationContext(), uri);

        ringtone.play();
    }

    public static float getScreenWidth(Activity activity)
    {
        Display display = activity.getWindowManager().getDefaultDisplay();
        DisplayMetrics outMetrics = new DisplayMetrics();
        display.getMetrics(outMetrics);

        float pxWidth = outMetrics.widthPixels;
        return pxWidth;
    }

    public static float getScreenHeight(Activity activity)
    {
        Display display = activity.getWindowManager().getDefaultDisplay();
        DisplayMetrics outMetrics = new DisplayMetrics();
        display.getMetrics(outMetrics);

        float pxHeight = outMetrics.heightPixels;
        return pxHeight;
    }

    // 화면 제일 위 시계 및 밧데리 표시 영역
    public static int getStatusBarHeight(Resources res) {
        int result = 0;
        int resourceId = res.getIdentifier("status_bar_height", "dimen", "android");
        if (resourceId > 0) {
            result = res.getDimensionPixelSize(resourceId);
        }
        return result;
    }

    public static void showAlert(final String message, final String caption, final Context context)
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(context);

        builder.setMessage(message);
        builder.setTitle(caption);
        builder.setCancelable(false);
        builder.setPositiveButton("확인", null);
        builder.show();
    }

    public static String getDeviceID()
    {
        return FirebaseInstanceId.getInstance().getToken();
    }
}
