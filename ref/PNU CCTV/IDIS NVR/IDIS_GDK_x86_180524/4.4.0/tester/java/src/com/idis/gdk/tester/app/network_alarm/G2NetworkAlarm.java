package com.idis.gdk.tester.app.network_alarm;

import com.idis.gdk.G2Main;
import com.idis.gdk.G2Watch;
import com.idis.gdk.define.G2Event;
import com.idis.gdk.define.G2NetworkInfo;
import com.idis.gdk.define.G2Time;
import com.idis.gdk.define.live.G2LiveNetworkAlarmInfo;

public class G2NetworkAlarm {
    public static void main(String[] args) {
        String address = "";
        short port = 0;
        int seq = 0;
        int id = 0;
        int event = 0;
        boolean controlOn = true;
        long timeout = 100;
        int retry = 5;
        int level = 0;
        String data = "";

        for (String var : args) {
            var = var.trim();
            if (var.startsWith("/address=")) {
                int pos = var.indexOf("=");
                String s = var.substring(pos + 1);
                String[] a = s.split(":");
                if (a.length == 1) {
                    address = a[0];
                } else {
                    address = a[0];
                    port = (short) Integer.parseInt(a[1]);
                }
            } else if (var.startsWith("/seq=")) {
                int pos = var.indexOf("=");
                seq = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/id=")) {
                int pos = var.indexOf("=");
                id = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/event=")) {
                int pos = var.indexOf("=");
                event = Integer.parseInt(var.substring(pos + 1));
            } else if (var.compareToIgnoreCase("/off") == 0) {
                controlOn = false;
            } else if (var.startsWith("/timeout=")) {
                int pos = var.indexOf("=");
                timeout = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/retry=")) {
                int pos = var.indexOf("=");
                retry = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/level=")) {
                int pos = var.indexOf("=");
                level = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/data=")) {
                int pos = var.indexOf("=");
                data = var.substring(pos + 1);
            }
        }

        String path = G2NetworkAlarm.class.getProtectionDomain().getCodeSource().getLocation().getPath();
        path += "../../../bin";

        G2Main.get().appInitialize(G2Main.G2LANGUAGE.ENGLISH, path);

        G2Watch adaptor = new G2Watch();
        G2WatchDelegator delegator = new G2WatchDelegator(adaptor);
        adaptor.startup(2);
        adaptor.setListener(delegator);

        G2NetworkInfo ni = new G2NetworkInfo();
        ni._address = address;
        ni.setPort(G2NetworkInfo.PORT_TYPE.WATCH, port);

        G2LiveNetworkAlarmInfo nai = new G2LiveNetworkAlarmInfo();
        nai._version[0] = 1;
        nai._version[1] = 0;
        nai._seq_number = seq;
        nai._id = id;
        nai._on = controlOn;
        nai._data = data;
        nai._level = (level == 1) ? G2Event.LEVEL.EMERGENCY :
                     (level == 2) ? G2Event.LEVEL.ERROR : G2Event.LEVEL.NORMAL;
        nai._event = event;
        nai._time = G2Time.current();
        nai._msec = System.currentTimeMillis();

        delegator.options().alarm = nai;
        delegator.options().ni = ni;
        delegator.options().timeout = timeout * 1000;
        delegator.options().retry = retry;

        do {
            if (delegator.operation()) {
                break;
            }
        } while (--retry > 0 && delegator.isContinue());

        delegator.print();
        adaptor.cleanup();

        G2Main.get().appFinalize();
    }
}
