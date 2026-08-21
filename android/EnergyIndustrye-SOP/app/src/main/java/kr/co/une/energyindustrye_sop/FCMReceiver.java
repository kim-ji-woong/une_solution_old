package kr.co.une.energyindustrye_sop;

/**
 * Created by dev on 2017-07-22.
 */

public interface FCMReceiver {
    void onNotify(String strTitle, String strBoday);
}
