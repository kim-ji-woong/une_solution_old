package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.support.v4.app.FragmentActivity;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.widget.TextView;

public class PopupWindowActivity extends FragmentActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.activity_popup_window);

        setContents();
    }

    private void setContents()
    {
        Intent intent = getIntent();
        String message = intent.getStringExtra("Message");

        TextView textView = (TextView)findViewById(R.id.popupText);
        textView.setText(message);
    }

    public void onConfirm(View v)
    {
        this.finish();
    }
}
