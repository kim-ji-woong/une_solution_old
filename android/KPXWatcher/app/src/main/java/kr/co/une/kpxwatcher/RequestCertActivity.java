package kr.co.une.kpxwatcher;

import android.app.Activity;
import android.content.Context;
import android.graphics.Typeface;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Layout;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.KeyEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;

public class RequestCertActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_request_cert);

        int textPos1 = 25, textPos2 = 55;
        int screenHeight = (int)(MainActivity.getScreenHeight(this) + 0.1f);

        if (screenHeight == 1280)
        {
            textPos1 = 7;
            textPos2 = 35;
        }
        else if (screenHeight == 2560)
        {
            textPos1 = 60;
            textPos2 = 85;
        }

        setImage((ImageView) findViewById(R.id.titleLogo), R.drawable.title_logo);
        setImage((ImageView) findViewById(R.id.btnTeamName), R.drawable.cert_input);
        setImage((ImageView) findViewById(R.id.btnUserName), R.drawable.cert_input);
        setImage((ImageView) findViewById(R.id.btnPhoneNumber), R.drawable.cert_input);
        setImage((ImageView) findViewById(R.id.btnConfirm), R.drawable.cert_confirm_normal);
        /*setImageSizeNLocation((ImageView) findViewById(R.id.titleLogo), R.drawable.title_logo, 657, 297);
        setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.teamNameLayout), (ImageButton)findViewById(R.id.btnTeamName), (TextView)findViewById(R.id.textTeamName), 55, textPos1, 0, R.drawable.cert_input);
        setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.userNameLayout), (ImageButton)findViewById(R.id.btnUserName), (TextView)findViewById(R.id.textUserName), 0, textPos2, 40, R.drawable.cert_input);
        setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.phoneNumberLayout), (ImageButton)findViewById(R.id.btnPhoneNumber), (TextView)findViewById(R.id.textPhoneNumber), 0, textPos2, 80, R.drawable.cert_input);
        setLayoutSizeNLocation((RelativeLayout)findViewById(R.id.confirmLayout), (ImageButton)findViewById(R.id.btnConfirm), null, 0, 45, 80, R.drawable.cert_confirm_normal);*/

        Object finishEditing = new EditText.OnEditorActionListener()
        {
            @Override
            public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
                if (actionId == EditorInfo.IME_ACTION_SEARCH ||
                        actionId == EditorInfo.IME_ACTION_DONE ||
                        event.getAction() == KeyEvent.ACTION_DOWN &&
                                event.getKeyCode() == KeyEvent.KEYCODE_ENTER) {
                    if (!event.isShiftPressed()) {
                        // the user is done typing.

                        return true; // consume.
                    }
                }
                return false; // pass on to other listeners.
            }
        };

        /*((EditText) findViewById(R.id.textTeamName)).setOnEditorActionListener(new EditListener());
        ((EditText) findViewById(R.id.textUserName)).setOnEditorActionListener(new EditListener());
        ((EditText) findViewById(R.id.textPhoneNumber)).setOnEditorActionListener(new EditListener());*/
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
        String strTeamName = getTextString(R.id.textTeamName).trim();

        if (strTeamName == null || strTeamName.length() == 0 || strTeamName.equals("부서"))
        {
            ((EditText)findViewById(R.id.textTeamName)).setText("부서");
            MainActivity.showAlert("부서명을 입력하세요.", "오류", this);
            return;
        }

        String strUserName = getTextString(R.id.textUserName).trim();

        if (strUserName == null || strUserName.length() == 0 || strUserName.equals("이름"))
        {
            ((EditText)findViewById(R.id.textUserName)).setText("이름");
            MainActivity.showAlert("이름을 입력하세요.", "오류", this);
            return;
        }

        String strPhoneNumber = getTextString(R.id.textPhoneNumber).trim();

        if (strPhoneNumber == null || strPhoneNumber.length() == 0)
        {
            ((EditText)findViewById(R.id.textPhoneNumber)).setText("이름");
            MainActivity.showAlert("전화번호를 입력하세요.", "오류", this);
            return;
        }

        strPhoneNumber = isValidPhoneNumber(strPhoneNumber);

        if (strPhoneNumber == null)
        {
            MainActivity.showAlert("전화번호가 형식에 맞지 않습니다.", "오류", this);
            return;
        }

        WebManager mgr = new WebManager(getApplicationContext());

        mgr.setQueryType(WebManager.QueryType.REQUEST_CERT_CODE);

        mgr.setParameter("DeviceID", MainActivity.getDeviceID());
        mgr.setParameter("SerialNumber", WebManager.getDeviceSerialNumber());
        mgr.setParameter("TeamName", strTeamName);
        mgr.setParameter("UserName", strUserName);
        mgr.setParameter("PhoneNumber", strPhoneNumber);

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

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
        {
            MainActivity.showAlert("인증 요청을 하였습니다.\r\n승인처리 메시지를 받은후 다시 프로그램을 실행해 주세요.", "알림", this);
        }
        else
        {
            if (mgr.getResult() == WebManager.ResultType.ALREADY_REQUESTED)
                MainActivity.showAlert("이미 인증 요청되었습니다.", "알림", this);
            else
                MainActivity.showAlert("인증 요청이 실패하였습니다.", "알림", this);
        }
    }

    private String isValidPhoneNumber(String strPhoneNumber)
    {
        strPhoneNumber = strPhoneNumber.replace(" ", "");
        strPhoneNumber = strPhoneNumber.replace("-", "");

        int len = strPhoneNumber.length();

        if (len < 10 || len > 11)
            return null;

        String strHeader = strPhoneNumber.substring(0, 3);

        if (strHeader.equals("010") == false && strHeader.equals("011") && strHeader.equals("017")
                && strHeader.equals("018") == false && strHeader.equals("019"))
            return null;

        String strBody = strPhoneNumber.substring(3);

        for (int i=0;i<strBody.length();i++)
        {
            char ch = strBody.charAt(i);

            if (ch < '0' && ch > '9')
                return null;
        }

        return strPhoneNumber;
    }

    private String getTextString(int textID)
    {
        EditText text = (EditText)findViewById(textID);
        return text.getText().toString();
    }

    public void btnInputClick(View view)
    {
        EditText text = null;

        if (view.getId() == R.id.btnTeamName)
            text = (EditText) findViewById(R.id.textTeamName);
        else if (view.getId() == R.id.btnUserName)
            text = (EditText)findViewById(R.id.textUserName);
        else if (view.getId() == R.id.btnPhoneNumber)
            text = (EditText)findViewById(R.id.textPhoneNumber);

        if (text == null)
            return;

        InputMethodManager inputMethodManager = (InputMethodManager)getSystemService(INPUT_METHOD_SERVICE);
        inputMethodManager.showSoftInput(text, InputMethodManager.SHOW_IMPLICIT);
    }

    class EditListener implements TextView.OnEditorActionListener
    {
        public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
            if (actionId == EditorInfo.IME_ACTION_SEARCH ||
                    actionId == EditorInfo.IME_ACTION_DONE ||
                    event.getAction() == KeyEvent.ACTION_DOWN &&
                            event.getKeyCode() == KeyEvent.KEYCODE_ENTER) {
                if (!event.isShiftPressed()) {

                    return true; // consume.
                }
            }
            return false; // pass on to other listeners.
        }
    }
}
