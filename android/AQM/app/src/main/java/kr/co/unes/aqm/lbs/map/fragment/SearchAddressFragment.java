package kr.co.unes.aqm.lbs.map.fragment;

import android.os.Parcel;
import android.util.Log;
import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.view.inputmethod.InputMethodManager;

import org.w3c.dom.Node;

import java.util.ArrayList;
import java.util.List;

import kr.co.unes.aqm.lbs.map.MainActivity;
import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.AddressManager;
import kr.co.unes.aqm.lbs.map.data.NodeData;
import kr.co.unes.aqm.lbs.map.data.NodeManager;


/**
 * A simple {@link Fragment} subclass.
 * Activities that contain this fragment must implement the
 * {@link OnFragmentInteractionListener} interface
 * to handle interaction events.
 * Use the {@link SearchAddressFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class SearchAddressFragment extends Fragment implements View.OnClickListener, AdapterView.OnItemSelectedListener
{
    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    public SearchAddressFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment SearchAddressFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchAddressFragment newInstance(String param1, String param2) {
        SearchAddressFragment fragment = new SearchAddressFragment();
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

    private  List<String> arDepth1s;
    private  List<String> arDepth2s;
    private  List<String> arDepth3s;
    private  List<String> arDepth4s;

    private Spinner spinDepth1;
    private Spinner spinDepth2;
    private Spinner spinDepth3;
    private Spinner spinDepth4;

    private String szDepth1;
    private String szDepth2;
    private String szDepth3;
    private String szDepth4;

    private String szNotSelected = "--- 선택하세요 ---";
    private boolean bInitProcess = true;


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_search_address, container, false);

        bInitProcess = true;
        spinDepth1 = (Spinner)view.findViewById(R.id.spn_address_depth1s);


        arDepth1s = new ArrayList<>(AddressManager.getInstance().getAreaDepth1s());
        arDepth1s.add(0,szNotSelected);
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_spinner_item, arDepth1s);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinDepth1.setAdapter(adapter);
        spinDepth1.setOnItemSelectedListener(this);


        spinDepth2 = (Spinner)view.findViewById(R.id.spn_address_depth2s);
        spinDepth2.setOnItemSelectedListener(this);

        spinDepth3 = (Spinner)view.findViewById(R.id.spn_address_depth3s);
        spinDepth3.setOnItemSelectedListener(this);

        spinDepth4 = (Spinner)view.findViewById(R.id.spn_address_depth4s);
        spinDepth4.setOnItemSelectedListener(this);

        Button btn = (Button)view.findViewById(R.id.btn_address_search);
        btn.setOnClickListener(this);

        bInitProcess = false;
        return view;
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

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.btn_address_search:
                MainActivity mainActivity = (MainActivity)getActivity();
                if( mainActivity != null)
                {
                    NodeManager.getInstance().searchData(szDepth1,szDepth2, szDepth3, szDepth4);
                    List<NodeData> arList = NodeManager.getInstance().getNodeList();
                    if( arList != null && arList.size() > 0)
                    {
                        if( mListener != null)
                        {
                            Uri.Builder builder = new Uri.Builder();
                            builder.appendQueryParameter("SearchType", "Address");
                            builder.appendQueryParameter("depth1", szDepth1);
                            builder.appendQueryParameter("depth2", szDepth2);
                            builder.appendQueryParameter("depth3", szDepth3);
                            builder.appendQueryParameter("depth4", szDepth4);
                            builder.encodedFragment("SearchAddressFragment");
                            builder.fragment("AddressSearchResultFragment");

                            Uri uri = builder.build();
                            mListener.onFragmentInteraction(uri);
                        }
                    }
                }
                break;
        }
    }

    @Override
    public void onItemSelected(AdapterView<?> parent, View view, int position, long id)
    {
        if(bInitProcess == true)
            return;

        if( position < 0)
            return;

        MainActivity mainActivity = (MainActivity)getActivity();
        if( mainActivity == null)
        {
            return;
        }
        switch (parent.getId()) {
            case R.id.spn_address_depth1s:
                String szSelectedItem = arDepth1s.get(position);
                Log.d("AddressDepth1", szSelectedItem);
                if(szSelectedItem.compareTo(szNotSelected) != 0)
                {
                    szDepth1 = szSelectedItem;
                    szDepth2 = szDepth3 = szDepth4 = "";
                    arDepth2s = AddressManager.getInstance().getAreaDepth2s(szDepth1);
                    if(arDepth2s != null && arDepth2s.size() > 0)
                    {
                        spinDepth2.setEnabled(true);

                        arDepth2s.add(0,szNotSelected);
                        ArrayAdapter<String> adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_spinner_item, arDepth2s);
                        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                        spinDepth2.setAdapter(adapter);
                    }
                }
                else
                {
                    szDepth1 = "";
                    szDepth2 = "";
                    szDepth3 = "";
                    szDepth4 = "";

                    spinDepth2.setSelection(0);
                    spinDepth2.setAdapter(null);
                    spinDepth2.setEnabled(false);

                    spinDepth3.setSelection(0);
                    spinDepth3.setAdapter(null);
                    spinDepth3.setEnabled(false);

                    spinDepth4.setSelection(0);
                    spinDepth4.setAdapter(null);
                    spinDepth4.setEnabled(false);
                }
                break;
            case R.id.spn_address_depth2s:
                String szSelectedItem2 = arDepth2s.get(position);
                Log.d("AddressDepth2", szSelectedItem2);
                if(szSelectedItem2.compareTo(szNotSelected) != 0)
                {
                    szDepth2 = szSelectedItem2;
                    szDepth3 = szDepth4 = "";
                    arDepth3s = AddressManager.getInstance().getAreaDepth3s(szDepth1, szDepth2);
                    if(arDepth3s != null && arDepth3s.size() > 0)
                    {
                        spinDepth3.setEnabled(true);

                        arDepth3s.add(0,szNotSelected);
                        ArrayAdapter<String> adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_spinner_item, arDepth3s);
                        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                        spinDepth3.setAdapter(adapter);
                    }
                }
                else
                {
                    szDepth2 = "";
                    szDepth3 = "";
                    szDepth4 = "";
                    spinDepth3.setSelection(0);
                    spinDepth3.setAdapter(null);
                    spinDepth3.setEnabled(false);

                    spinDepth4.setSelection(0);
                    spinDepth4.setAdapter(null);
                    spinDepth4.setEnabled(false);
                }
                break;

            case R.id.spn_address_depth3s:
                String szSelectedItem3 = arDepth3s.get(position);
                Log.d("AddressDepth3", szSelectedItem3);
                if(szSelectedItem3.compareTo(szNotSelected) != 0)
                {
                    szDepth3 = szSelectedItem3;
                    szDepth4 = "";
                    arDepth4s = AddressManager.getInstance().getAreaDepth4s(szDepth1, szDepth2,szDepth3);
                    if(arDepth4s != null && arDepth4s.size() > 0)
                    {
                        spinDepth4.setEnabled(true);

                        arDepth4s.add(0,szNotSelected);
                        ArrayAdapter<String> adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_spinner_item, arDepth4s);
                        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                        spinDepth4.setAdapter(adapter);
                    }
                }
                else
                {
                    szDepth3 = "";
                    szDepth4 = "";

                    spinDepth4.setSelection(0);
                    spinDepth4.setAdapter(null);
                    spinDepth4.setEnabled(false);
                }
                break;

            case R.id.spn_address_depth4s:
                String szSelectedItem4 = arDepth4s.get(position);
                Log.d("AddressDepth4", szSelectedItem4);
                if(szSelectedItem4.compareTo(szNotSelected) != 0)
                {
                    szDepth4 = szSelectedItem4;
                }
                else
                {
                    szDepth4 = "";
                }
                break;
        }
    }

    @Override
    public void onNothingSelected(AdapterView<?> parent)
    {

    }
}
