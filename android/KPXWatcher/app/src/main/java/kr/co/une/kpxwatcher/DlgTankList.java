package kr.co.une.kpxwatcher;

import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.annotation.StyleRes;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.LinearLayout;
import android.widget.ListView;

import java.util.ArrayList;
import java.util.List;

import kr.co.une.kpxwatcher.Data.Tank;

/**
 * Created by cocoff22 on 2017-09-07.
 */

public class DlgTankList extends Dialog {

    private LinearLayout m_Layout = null;
    private ListView m_listView;
    private int[] m_arrTanks = null;
    private Tank m_Tank = null;

    private Dialog.OnDismissListener ListenerDismiss;

    public DlgTankList(@NonNull Context context) {
        super(context);
    }

    public DlgTankList(@NonNull Context context, @StyleRes int themeResId) {
        super(context, themeResId);
    }

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // 배경 Activity가 흐려지는(어두워지는) 효과 없앰
        getWindow().clearFlags(WindowManager.LayoutParams.FLAG_DIM_BEHIND);
        // 모달로 적용
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCH_MODAL, WindowManager.LayoutParams.FLAG_NOT_TOUCH_MODAL);
        // Title 없앰
        getWindow().requestFeature(Window.FEATURE_NO_TITLE);
        setCancelable(true);

        setContentView(R.layout.dlg_tank_item);

        m_Layout = (LinearLayout)findViewById(R.id.TankListDlg);

        Init_TankList(getContext());

        AddListener();
    }

    private void AddListener()
    {
        m_listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {

                int pos = m_listView.getCheckedItemPosition();   //리스트뷰의 포지션을 가져옴.
                //Object vo = (Object)adapterView.getAdapter().getItem(i);  //리스트뷰의 포지션 내용을 가져옴.
                m_Tank = MenuActivity.Instance().getTank(m_arrTanks[pos]);
                dismiss();
            }
        });
    }

    public int GetWidth()
    {
        if(m_Layout == null)
            return 400;

        return m_Layout.getWidth();
    }

    public Tank GetTank()
    {
        return m_Tank;
    }
    public void ReleaseTank()
    {
        m_Tank = null;
    }

    private void Init_TankList(Context context)
    {
        m_listView = (ListView)findViewById(R.id.tank_list);
        WebManager mgr = new WebManager(context);
        mgr.setQueryType(WebManager.QueryType.REQUEST_CON_TANK);
        mgr.start();

        int nTimeOut = 3000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN) {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            } catch (Exception e) {
                return;
            }
        }

        ArrayList names = new ArrayList();
        if (mgr.getResult() == WebManager.ResultType.SUCCESS) {
            List<String> results = mgr.getResultSet();
            int nResultCount = results.size();

            m_arrTanks = new int[nResultCount/2];
            for (int i = 0; i < nResultCount; i += 2) {
                try {
                    int id = Integer.parseInt(results.get(i).trim());
                    String strName = results.get(i + 1).trim();

                    names.add("TK - " + strName);
                    m_arrTanks[i/2] = id;
                } catch (Exception e) {
                    continue;
                }
            }
        }

        m_listView.setAdapter(new ArrayAdapter<String>(context, R.layout.tank_items, names));
    }
}
