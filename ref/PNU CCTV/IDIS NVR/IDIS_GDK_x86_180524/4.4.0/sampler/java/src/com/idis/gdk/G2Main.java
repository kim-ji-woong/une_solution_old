package com.idis.gdk;

import java.io.File;

import com.idis.gdk.util.G2Enum;
import com.sun.jna.Native;
import com.sun.jna.Platform;
import com.sun.jna.win32.StdCallLibrary;

public class G2Main {
    public interface G2Library extends StdCallLibrary {
        void j2_main_app_initialize(int language);
        void j2_main_app_finalize();
        void j2_main_set_language(int language);
        int j2_main_get_language();
        void j2_main_dvrns_setup(String address, int port);
    }

    public enum G2LANGUAGE implements G2Enum.Converter<Integer, G2LANGUAGE> {
        UNKNOWN(-1),
        ENGLISH(0),    				// English (U.S.)
        GERMAN(1),                  // German (Standard)
        FRENCH(2),                  // French (Standard)
        ITALIAN(3),                 // Italian (Standard)
        SPANISH(4),                 // Spanish (Spain, International Sort)
        KOREAN(5),                  // Korean
        JAPANESE(6),                // Japanese
        CHINESE_TRADITIONAL(7),     // Chinese (Taiwan)
        CHINESE_SIMPLIFIED(8),      // Chinese (RPC)
        DANISH(9),                  // Danish
        DUTCH(10),                  // Dutch (Netherlands)
        POLISH(11),                 // Polish
        SWEDISH(12),                // Swedish
        PORTUGUESE(13),             // Portuguese
        CZECH(14),                  // Czech
        RUSSIAN(15),                // Russian
        HUNGARIAN(16),              // Hungarian
        UNITEDKINGDOM(17),          // English (U.K.)
        FINNISH(18),                // Finnish (Finland)
        TURKISH(19),                // Turkish (Turkey)
        CROATIAN(20),               // Croatian
        COUNT(21);
        
        static G2Enum.ReverseMap<Integer, G2LANGUAGE> map = new G2Enum.ReverseMap<Integer, G2LANGUAGE>(G2LANGUAGE.class, G2LANGUAGE.ENGLISH);
        static public G2LANGUAGE from(int id) { return map.get(id); }
        private final int value;
        private G2LANGUAGE(int value) { this.value = value; }
        public Integer to() { return this.value; }
        public G2LANGUAGE get(Integer id) { return map.get(id); }
    }

    public boolean appInitialize(G2LANGUAGE id, String pathBin) {
        if (pathBin.isEmpty() != true) {
            File f = new File(pathBin);
            try {
                pathBin = f.getCanonicalPath();
            }
            catch (Exception e) {}
        
            if (pathBin.charAt(pathBin.length() - 1) != File.separatorChar) {
                pathBin += File.separator;
            }

            _path_bin = pathBin;
            _path_dll = pathBin + _module_name + ".dll";

            try {
                System.load(_path_dll);
            } catch (SecurityException e) {
            } catch (UnsatisfiedLinkError e) {
            } catch (NullPointerException e) {
            }
        }
        return appInitialize(id);
    }

    public boolean appInitialize(G2LANGUAGE id) {
        _lib = (G2Library) Native.loadLibrary(_module_name, G2Library.class);
        _lib.j2_main_app_initialize(id.to());
        G2Foundation.get().startup();
        G2GUID.G2GUIDLib.get().startup();
        return (_lib != null);
    }

    public void appFinalize() {
        G2GUID.G2GUIDLib.get().cleanup();
        G2Foundation.get().cleanup();
        _lib.j2_main_app_finalize();
        _lib = null;
    }

    public void setLanguage(G2LANGUAGE id) {
        lib().j2_main_set_language(id.to());
    }

    public G2LANGUAGE getLanguage() {
        return G2LANGUAGE.from(lib().j2_main_get_language());
    }

    public void dvrnsSetup(String address, int port) {
        lib().j2_main_dvrns_setup(address, port);
    }

    public G2Library lib() {
        return _lib;
    }

    public String moduleName() {
        return _module_name;
    }
    public String pathBin() {
        return _path_bin;
    }
    public String pathDLL() {
        return _path_dll;
    }

    protected G2Main() {}
    private String _module_name = Platform.is64Bit() ? "G2ClientGDKx64" : "G2ClientGDK";
    private String _path_bin = new String();
    private String _path_dll = new String();
	private G2Library _lib;
	private static G2Main s_singleton = new G2Main();
	public static G2Main get() {
        return s_singleton;
    }
}
