package kr.co.une.energyindustrye_sop.photoview;

/**
 * Created by dev on 2017-05-27.
 */

public interface PhotoViewOwner {
    public static int SWIPE_MIN_DISTANCE = 120;
    public static int SWIPE_MAX_OFF_PATH = 250;
    public static int SWIPE_THRESHOLD_VELOCITY = 200;

    void leftSwipe();
    void rightSwipe();
}
