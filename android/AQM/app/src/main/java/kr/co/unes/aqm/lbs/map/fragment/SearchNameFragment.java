package kr.co.unes.aqm.lbs.map.fragment;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;

import java.util.List;

import kr.co.unes.aqm.lbs.map.MainActivity;
import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.NodeData;
import kr.co.unes.aqm.lbs.map.data.NodeManager;

/**
 * A simple {@link Fragment} subclass.
 * Activities that contain this fragment must implement the
 * {@link OnFragmentInteractionListener} interface
 * to handle interaction events.
 * Use the {@link SearchNameFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class SearchNameFragment extends Fragment implements  View.OnClickListener  {
    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    public SearchNameFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment SearchNameFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchNameFragment newInstance(String param1, String param2) {
        SearchNameFragment fragment = new SearchNameFragment();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            mParam1 = getArguments().getString(ARG_PARAM1);
            mParam2 = getArguments().getString(ARG_PARAM2);
        }
    }

    private View m_searchView;
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        m_searchView = inflater.inflate(R.layout.fragment_search_name, container, false);

        MainActivity activity = (MainActivity)getActivity();
        ImageButton btn1 = (ImageButton)m_searchView.findViewById(R.id.btn_name_search);
        btn1.setOnClickListener(this);

        return m_searchView;
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.btn_name_search:
                MainActivity mainActivity = (MainActivity)getActivity();
                if( mainActivity != null)
                {
                    EditText edit = (EditText)m_searchView.findViewById(R.id.editText);
                    java.lang.String text = edit.getText().toString();
                    NodeManager.getInstance().searchData(text);
                    List<NodeData> arList = NodeManager.getInstance().getNodeList();
                    if( arList != null && arList.size() > 0)
                    {
                        for(int i = 0 ; i < arList.size(); i++)
                        {
                            String szValue = arList.get(i).toString();
                            Log.d("SearchResult : ", szValue);
                        }
                    }
                }
                break;
        }
    }

    // TODO: Rename method, update argument and hook method into UI event
    public void onButtonPressed(Uri uri) {
        if (mListener != null) {
            mListener.onFragmentInteraction(uri);
        }
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
        if (context instanceof OnFragmentInteractionListener) {
            mListener = (OnFragmentInteractionListener) context;
        } else {
            throw new RuntimeException(context.toString()
                    + " must implement OnFragmentInteractionListener");
        }
    }

    @Override
    public void onDetach() {
        super.onDetach();
        mListener = null;
    }

}
