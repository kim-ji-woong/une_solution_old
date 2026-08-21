package kr.co.une.energyindustrye_sop;

import android.content.DialogInterface;
import android.content.Intent;
import android.os.Handler;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;

import com.google.firebase.iid.FirebaseInstanceId;

public class InputPhoneNumberActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_input_phone_number);
    }

    public void btnInputClick(View view)
    {
        EditText text = (EditText)findViewById(R.id.textPhoneNumber);
        InputMethodManager inputMethodManager = (InputMethodManager)getSystemService(INPUT_METHOD_SERVICE);
        inputMethodManager.showSoftInput(text, InputMethodManager.SHOW_IMPLICIT);
    }

    private String getTextString(int textID)
    {
        EditText text = (EditText)findViewById(textID);
        return text.getText().toString();
    }

    public void btnApplyClick(View view)
    {
        String strPhoneNumber = getTextString(R.id.textPhoneNumber).trim();

        if (strPhoneNumber == null || strPhoneNumber.length() == 0)
        {
            MainActivity.showAlert("전화번호를 입력하세요.\r\n사용자 등록을 위하여 필요합니다.", "오류", this);
            return;
        }

        String deviceID = FirebaseInstanceId.getInstance().getToken();

        if (deviceID == null || deviceID.length() == 0) {
            // App을 처음 설치하면 deviceID가 없다.
            deviceID = "temp";
            /*mResult = ResultType.NOT_ENOUGH_PARAMETER;
            return false;*/
        }

        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.REGIST_USER);
        mgr.setParameter("PhoneNumber",  strPhoneNumber);
        mgr.setParameter("DeviceID", deviceID);
        mgr.setParameter("SerialNumber", WebManager.getDeviceSerialNumber());
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

        Splash.UserType type = Splash.UserType.UNKNOWN;

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
        {
            if (deviceID.equals("temp"))
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);

                builder.setMessage("App이 정상적으로 등록되었습니다.\r\n확인 버튼을 누른후 재시작하여 주세요.");
                builder.setTitle("확인");

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
                ConfirmHandler handler = new ConfirmHandler();
                hd.post(handler);
            }
        }
    }

    private class ConfirmHandler implements Runnable{

        public void run() {
            startActivity(new Intent(getApplication(), MainActivity.class)); // 로딩이 끝난후 이동할 Activity
            InputPhoneNumberActivity.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }
    }
}
