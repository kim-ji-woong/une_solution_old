package kr.co.unes.aqm.lbs.map.fragment;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.support.v7.widget.GridLayoutManager;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import kr.co.unes.aqm.lbs.map.MainActivity;
import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.NodeData;
import kr.co.unes.aqm.lbs.map.data.NodeManager;
import kr.co.unes.aqm.lbs.map.view.NodeItemRecyclerViewAdapter;

/**
 * A simple {@link Fragment} subclass.
 * Activities that contain this fragment must implement the
 * {@link OnFragmentInteractionListener} interface
 * to handle interaction events.
 * Use the {@link AddressSearchResultFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class AddressSearchResultFragment extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    public AddressSearchResultFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment AddressSearchResultFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static AddressSearchResultFragment newInstance(String param1, String param2) {
        AddressSearchResultFragment fragment = new AddressSearchResultFragment();
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

    private TextView mTitleText;
    private RecyclerView mRecyclerView;
    private NodeItemRecyclerViewAdapter mAdpater;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_address_search_result, container, false);

        mTitleText = (TextView)view.findViewById(R.id.txt_address_search_result);

        mRecyclerView = (RecyclerView)view.findViewById(R.id.search_result_list);

        if(mRecyclerView != null)
        {
            Context context = view.getContext();
            if (mColumnItems.size() <= 1) {
                mRecyclerView.setLayoutManager(new LinearLayoutManager(context));
            } else {
                mRecyclerView.setLayoutManager(new GridLayoutManager(context, mColumnItems.size()));
            }
            MainActivity main = (MainActivity)getActivity();

            mAdpater = new NodeItemRecyclerViewAdapter(mColumnItems, main);
            mRecyclerView.setAdapter(mAdpater);
            //recyclerView.addItemDecoration(new SensorRecyclerViewDeco(getActivity(), LinearLayoutManager.VERTICAL));
        }

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

    private List<NodeData> mColumnItems = new ArrayList<NodeData>();

    public void setData(Uri uri, List<NodeData> arList)
    {
        if( mListener == null)
            return;

        String szType = uri.getQueryParameter("SearchType");
        if( szType != null && szType.compareTo("Address") == 0) {
            String szDepth1 = uri.getQueryParameter("depth1");
            String szDepth2 = uri.getQueryParameter("depth2");
            String szDepth3 = uri.getQueryParameter("depth3");
            String szDepth4 = uri.getQueryParameter("depth4");


            String szTitle = "";
            if (szDepth1 != null && szDepth1.compareTo("") != 0)
            {

                szTitle = szTitle + szDepth1;
            }
            if (szDepth2 != null && szDepth2.compareTo("") != 0)
            {
                szTitle = szTitle + " "  + szDepth2;
            }
            if (szDepth3 != null && szDepth3.compareTo("") != 0)
            {
                szTitle = szTitle + " "  + szDepth3;
            }

            if (szDepth4 != null && szDepth4.compareTo("") != 0)
            {
                szTitle = szTitle + " "  + szDepth4;
            }



            mTitleText.setText(szTitle);

        }
        // SetData
        MainActivity main = (MainActivity)getActivity();

        mColumnItems = NodeManager.getInstance().getNodeList();
        if(mAdpater != null)
        {
            mAdpater.removeAllItems();
            for(int i = 0; i < mColumnItems.size(); i++)
            {
                NodeData item = (NodeData)mColumnItems.get(i);
                mAdpater.add(i, item);
            }
        }
    }


    public interface OnListFragmentInteractionListener {

        void onListFragmentInteraction(NodeData item);
    }

}
