package kr.co.une.kpxwatcher;

import android.content.Intent;
import android.graphics.Typeface;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;

public class CertConfirmActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_cert_confirm);

        int textPos = 55;
        int screenHeight = (int)(MainActivity.getScreenHeight(this) + 0.1f);

        if (screenHeight == 1280)
            textPos = 35;
        else if (screenHeight == 2560)
            textPos = 85;

        setImage((ImageView) findViewById(R.id.titleLogo), R.drawable.title_logo);
        setImage((ImageView) findViewById(R.id.btnCertCode), R.drawable.cert_input);
        setImage((ImageView) findViewById(R.id.btnConfirm), R.drawable.cert_confirm_normal);
        //setImageSizeNLocation((ImageView) findViewById(R.id.titleLogo), R.drawable.title_logo, 657, 297);
        setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.descriptionLayout), null, (TextView)findViewById(R.id.textDescription), 55, 20, 0, null);
        //setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.certCodeLayout), (ImageButton)findViewById(R.id.btnCertCode), (TextView)findViewById(R.id.textCertCode), 0, textPos, 40, R.drawable.cert_input);
        //setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.confirmLayout), (ImageButton)findViewById(R.id.btnConfirm), null, 0, 45, 80, R.drawable.cert_confirm_normal);
    }

    private void setImage(ImageView view, int nImageID)
    {
        Glide.with(this).load(nImageID).into(view);
    }

    private void setLayoutSizeNLocation(RelativeLayout layout, ImageButton btn, TextView text, int paddingTop, int textPos, int layoutPos, Object image)
    {
        if (layout == null)
            return;

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        // 표준모델
        int stLeftPadding = 274;//, stRightPadding = 300;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth);
        int right = left;
        int top = paddingTop;
        int bottom = 0;

        layout.setPadding(left, top, right, bottom);
        layout.setY(layout.getY() - layoutPos);

        if (text != null)
        {
            float y = text.getY();
            text.setY(y - textPos);
            text.setTypeface(null, Typeface.BOLD);
        }

        if (btn != null)
            Glide.with(this).load(image).into(btn);
    }

    private void setImageButtonSizeNLocation(ImageButton btn, int paddingTop, int nImageID, int imgWidth, int imgHeight)
    {
        if (btn == null)
            return;

        Glide.with(this).load(nImageID).into(btn);

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);
        //int imgWidth = btn.getDrawable().getIntrinsicWidth();
        //int imgHeight = btn.getDrawable().getIntrinsicHeight();

        // 표준모델
        int stLeftPadding = 300, stRightPadding = 300;
        int stScreenWidth = 1440, stScreenHeight = 2560;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth);
        int right = left;
        int top = paddingTop;
        int bottom = 0;

        btn.setPadding(left, top, right, bottom);
    }

    private void setImageSizeNLocation(ImageView view, int nImageID, int imgWidth, int imgHeight)
    {
        if (view == null)
            return;

        Glide.with(this).load(nImageID).into(view);

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        // 표준모델
        int stTopPadding = 497, stLeftPadding = 211, stRightPadding = 211;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();

        float scale = 1.0f;//getResources().getDisplayMetrics().density;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth / scale);
        int right = (int)((fScreenWidth - stRightPadding * fScreenWidth / stScreenWidth) / scale);
        int top = (int)(stTopPadding * fScreenHeight / stScreenHeight / scale);
        int height = (right - left) * imgHeight / imgWidth;
        int bottom = top + height;

        bottom = 0;
        right = left;

        view.setPadding(left, top, right, bottom);
    }

    public void btnConfirmClick(View v)
    {
        String strCertCode = getTextString(R.id.textCertCode).trim();

        if (strCertCode == null || strCertCode.length() == 0)
        {
            MainActivity.showAlert("전달받은 인증코드를 입력하세요.", "오류", this);
            return;
        }

        WebManager mgr = new WebManager(getApplicationContext());

        mgr.setQueryType(WebManager.QueryType.REQUEST_CERT_CONFIRM);

        mgr.setParameter("DeviceID", MainActivity.getDeviceID());
        mgr.setParameter("SerialNumber", WebManager.getDeviceSerialNumber());
        mgr.setParameter("CertCode", strCertCode);

        mgr.start();

        int nTimeOut = 3000, delay = 500, sum = 0;

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

        if (mgr.getResult() == WebManager.ResultType.SUCCESS || mgr.getResult() == WebManager.ResultType.ALREADY_CERTIFIED_USER)
        {
            Handler hd = new Handler();
            ConfirmHandler handler = new ConfirmHandler();
            hd.post(handler);
        }
        else if (mgr.getResult() == WebManager.ResultType.NOT_PERMITTED_CERT_CODE)
        {
            MainActivity.showAlert("승인되지 않은 인증코드입니다.", "오류", this);
        }
        else if (mgr.getResult() == WebManager.ResultType.EXPIRED_CERT_CODE)
        {
            MainActivity.showAlert("인증코드의 유효기간이 지났습니다.\r\n다시 인증요청을 진행해 주세요.", "오류", this);
        }
    }

    private String getTextString(int textID)
    {
        EditText text = (EditText)findViewById(textID);
        return text.getText().toString();
    }

    public void btnInputClick(View view)
    {
        EditText text = (EditText)findViewById(R.id.textCertCode);
        InputMethodManager inputMethodManager = (InputMethodManager)getSystemService(INPUT_METHOD_SERVICE);
        inputMethodManager.showSoftInput(text, InputMethodManager.SHOW_IMPLICIT);
    }

    private class ConfirmHandler implements Runnable{

        public void run() {
            startActivity(new Intent(getApplication(), MenuActivity.class)); // 로딩이 끝난후 이동할 Activity
            CertConfirmActivity.this.finish(); // 로딩페이지 Activity Stack에서 제거
        }
    }
}
