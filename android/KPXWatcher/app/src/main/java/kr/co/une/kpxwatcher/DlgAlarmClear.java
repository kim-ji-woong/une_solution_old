package kr.co.une.kpxwatcher;

import android.app.Dialog;
import android.content.Context;
import android.os.Bundle;
import android.support.annotation.IdRes;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.text.TextUtils;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioGroup;
import android.widget.Toast;

import org.w3c.dom.Text;

import kr.co.une.kpxwatcher.Data.Pipe;
import kr.co.une.kpxwatcher.Data.Tank;
import kr.co.une.kpxwatcher.R;

/**
 * Created by cocoff22 on 2017-11-06.
 */

public class DlgAlarmClear extends Dialog {

    private Tank m_Tank = null;
    private Pipe m_Pipe = null;
    private int m_nOccurType = 0;
    private boolean m_bResult = false;

    public DlgAlarmClear(@NonNull Context context) {
        super(context);
    }

    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //getWindow().requestFeature(Window.FEATURE_CUSTOM_TITLE);
        // 모달로 적용
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCH_MODAL, WindowManager.LayoutParams.FLAG_NOT_TOUCH_MODAL);

        getWindow().setTitle("알람 해제");

        setContentView(R.layout.dlg_alarmclear);

        // 버튼 이벤트 등록
        Button btnOk = (Button)findViewById(R.id.btn_yes);
        btnOk.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnBtnClick(v);
            }
        });
        Button btnCancel = (Button)findViewById(R.id.btn_no);
        btnCancel.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnBtnClick(v);
            }
        });

        // radio event 등록
        RadioGroup group = (RadioGroup)findViewById(R.id.radioGroup);
        group.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(RadioGroup group, @IdRes int checkedId) {
                OnRadioChanged(group, checkedId);
            }
        });
    }

    private void OnBtnClick(final View v)
    {
        if(v.getId() == R.id.btn_yes) {
            EditText edit = (EditText)findViewById(R.id.edit_alarmComment);
            if(TextUtils.isEmpty(edit.getText().toString()))
            {
                Toast.makeText(getContext(), "알람 해결 내용을 입력해 주세요", Toast.LENGTH_SHORT).show();
                return;
            }
            m_bResult = true;
        }
        else
            m_bResult = false;

        dismiss();
    }

    private void OnRadioChanged(final RadioGroup group, final int id)
    {
        if(id == R.id.radio1)
            m_nOccurType = 0;
        else if(id == R.id.radio2)
            m_nOccurType = 1;
        else if(id == R.id.radio3)
            m_nOccurType = 2;
        else
            m_nOccurType = 3;
    }

    public void SetTank(Tank tank)
    {
        m_Tank = tank;
    }

    public Tank GetTank()
    {
        return m_Tank;
    }

    public void SetPipe(Pipe pipe)
    {
        m_Pipe = pipe;
    }

    public Pipe GetPipe()
    {
        return m_Pipe;
    }

    public int GetOccurrenceType()
    {
        return m_nOccurType;
    }

    public String GetComment()
    {
        EditText edit = (EditText)findViewById(R.id.edit_alarmComment);
        return edit.getText().toString();
    }

    public boolean Result()
    {
        return m_bResult;
    }
}
