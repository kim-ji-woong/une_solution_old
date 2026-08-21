package kr.co.une.energyindustrye_sop.FCM;

import android.util.Log;

import com.firebase.jobdispatcher.JobParameters;

/**
 * Created by 지웅 on 2017-07-18.
 */

public class JobService extends com.firebase.jobdispatcher.JobService {
    private static final String TAG = "e-SOPJobService";

    @Override
    public boolean onStartJob(JobParameters jobParameters) {
        Log.d(TAG, "Performing long running task in scheduled job");
        // TODO(developer): add long running task here.
        return false;
    }

    @Override
    public boolean onStopJob(JobParameters jobParameters) {
        return false;
    }
}
