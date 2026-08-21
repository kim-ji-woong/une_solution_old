package com.idis.gdk.tester.app.elevator_status;

import com.idis.gdk.G2Main;
import com.idis.gdk.G2Watch;
import com.idis.gdk.define.G2CodecInfo;
import com.idis.gdk.define.G2NetworkInfo;
import com.idis.gdk.define.live.G2LiveElevatorStatusInfo;

public class G2ElevatorStatus {
    public static void main(String[] args) {
        String address = "";
        short port = 0;
        int seq = 0;
        short id = 0;
        float floor = 0.0f;
        byte door = G2CodecInfo.ElevatorStatus.DOOR_STATUS.UNKNOWN.to();
        byte direction = G2CodecInfo.ElevatorStatus.DIRECTION.UNKNOWN.to();
        byte mode = G2CodecInfo.ElevatorStatus.MODE.UNKNOWN.to(); 
        long timeout = 100;
        int retry = 5;
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
                id = Short.parseShort(var.substring(pos + 1));
            } else if (var.startsWith("/floor=")) {
                int pos = var.indexOf("=");
                floor = Float.parseFloat(var.substring(pos + 1));
            } else if (var.startsWith("/door=")) {
                int pos = var.indexOf("=");
                door = Byte.parseByte(var.substring(pos + 1));
            } else if (var.startsWith("/direction=")) {
                int pos = var.indexOf("=");
                direction = Byte.parseByte(var.substring(pos + 1));
            } else if (var.startsWith("/mode=")) {
                int pos = var.indexOf("=");
                mode = Byte.parseByte(var.substring(pos + 1));
            } else if (var.startsWith("/timeout=")) {
                int pos = var.indexOf("=");
                timeout = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/retry=")) {
                int pos = var.indexOf("=");
                retry = Integer.parseInt(var.substring(pos + 1));
            } else if (var.startsWith("/data=")) {
                int pos = var.indexOf("=");
                data = var.substring(pos + 1);
            }
        }

        String path = G2ElevatorStatus.class.getProtectionDomain().getCodeSource().getLocation().getPath();
        path += "../../../bin";

        G2Main.get().appInitialize(G2Main.G2LANGUAGE.ENGLISH, path);

        G2Watch adaptor = new G2Watch();
        G2WatchDelegator delegator = new G2WatchDelegator(adaptor);
        adaptor.startup(2);
        adaptor.setListener(delegator);

        G2NetworkInfo ni = new G2NetworkInfo();
        ni._address = address;
        ni.setPort(G2NetworkInfo.PORT_TYPE.WATCH, port);
        
        G2LiveElevatorStatusInfo esi = new G2LiveElevatorStatusInfo();
        esi._seq_number = seq;
        esi._status._id = id;
        esi._status._floor = floor;
        esi._status._door_status = G2CodecInfo.ElevatorStatus.DOOR_STATUS.from(door);
        esi._status._direction = G2CodecInfo.ElevatorStatus.DIRECTION.from(direction);
        esi._status._mode = G2CodecInfo.ElevatorStatus.MODE.from(mode);
        esi._status._additional = data;
        
        delegator.options().status = esi;
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
