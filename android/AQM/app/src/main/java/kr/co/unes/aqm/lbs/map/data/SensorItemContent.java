package kr.co.unes.aqm.lbs.map.data;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import kr.co.unes.aqm.lbs.map.data.AirSensorData;
import kr.co.unes.aqm.lbs.map.data.AirSensorManager;

/**
 * Helper class for providing sample content for user interfaces created by
 * Android template wizards.
 * <p/>
 * TODO: Replace all uses of this class before publishing your app.
 */
public class SensorItemContent {

    public List<AirSensorData> ITEMS = new ArrayList<AirSensorData>();

    public Map<String, AirSensorData> ITEM_MAP = new HashMap<String, AirSensorData>();


    public SensorItemContent() {

        ArrayList<AirSensorData> arList = AirSensorManager.getInstance().getSensorList();
        // Add some sample items.
        for (int i = 0; i < arList.size(); i++) {
            AirSensorData data = (AirSensorData)arList.get(i);
            addItem(data);
        }
    }

    private void addItem(AirSensorData item) {
        ITEMS.add(item);
        ITEM_MAP.put(item.id, item);
    }

    private String makeDetails(int position) {
        StringBuilder builder = new StringBuilder();
        builder.append("Details about Item: ").append(position);
        for (int i = 0; i < position; i++) {
            builder.append("\nMore details information here.");
        }
        return builder.toString();
    }

}
