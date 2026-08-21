using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_admin
    {
        public struct statusElementHealth
        {
            public enum HEALTH_STATUS
            {
                STATUS_NORMAL = -1,
                STATUS_HEALTHY = 0,
                STATUS_PROBLEM,
                STATUS_UNREACHABLE,
            };
            public struct element
            {
                public string _status;
                public string _problem;
                public string _version;
                public string _site;
                public string _address;
                public string _mac;
                public string _cameras;
                public string _record;
                public string _recordCheck;
                public string _recordPeriod;
                public string _group;
                public string _alarm_in;
                public string _fan;

            };
            public struct bad_status_t
            {
                public bool _camera;
                public bool _camera_record;
                public bool _alarm_in;
                public bool _fan;
                public bool _record;
            };
        
        };

        #region FEATURE_CONNECT
        public bool connect_monitor(G2GUID monitor_service)
        {
            bool res = false;
            G2CONNECT_RES connection_res;
            lock (_adaptor)
            {
                _monitor_adaptor.connect(ref monitor_service, out connection_res);
            }
            if (connection_res._channel > -1)
            {
                res = true;
            }
            if (res)
            {
                _monitor_adaptor.set_activate(true);
            }

            return res;
        }
        #endregion

        public void monitor_set_guids(ref G2GUIDLIST guids)
        {
            _monitor_guids.AddRange(guids);
        }

        public void monitor_unset_guids()
        {
            _monitor_guids.Clear();
        }

        public void monitor_cleanup(bool final)
        {
            if (_adaptor.is_disconnectable())
            {
                lock (_monitor_adaptor)
                {
                    if (_monitor_adaptor.is_disconnectable())
                    {
                        _waitForSingle = new EventWaitHandle(false, EventResetMode.AutoReset);
                        _monitor_adaptor.disconnect();
                        if (final)
                        {
                            _waitForSingle.WaitOne();
                            _monitor_adaptor.cleanup();
                        }
                    }
                }
            }
        }
        
        #region FEATURE_REQ_HEALTH_INFO
        public bool request_health_info(ref G2GUIDSET devices)
        {
            bool res = false;
            lock (_adaptor) 
            {
                if (_adaptor.is_connected())
                {
                    res = _monitor_adaptor.set_device_list_interest_for_health(_monitor_handle, devices);
                    if (res)
                    {
                        res = _monitor_adaptor.request_update_device_list_interest_for_health(_monitor_handle);
                    }
                }
            }
            return res;
        }
        #endregion

        #region HEALTH_INFO_PARSE_HELPER
        private statusElementHealth.HEALTH_STATUS for_status(ref G2MONITORING_DEVICE_HEALTH health, ref statusElementHealth.bad_status_t bad_status)
        {
            statusElementHealth.HEALTH_STATUS status = statusElementHealth.HEALTH_STATUS.STATUS_HEALTHY;
             bad_status._camera = false;
             bad_status._camera_record = false;
             bad_status._alarm_in = false;
             bad_status._fan = false;
             bad_status._record = false;
           
            if (!(health._connection.Equals((int)G2MONITORING_DEVICE_STATUS.SESSION.CONNECTED)))
            {
                switch ((G2DISCONNECT_REASON.TYPE)health._reason_disconnect)
                {
                    case G2DISCONNECT_REASON.TYPE.UNKNOWN:
                    case G2DISCONNECT_REASON.TYPE.FULL_CHANNEL:
                    case G2DISCONNECT_REASON.TYPE.INVALID_VERSION:
                    case G2DISCONNECT_REASON.TYPE.LOGIN_FAIL:
                        status = statusElementHealth.HEALTH_STATUS.STATUS_PROBLEM;
                        break;
                    default:
                        status = statusElementHealth.HEALTH_STATUS.STATUS_UNREACHABLE;
                        break;
                }
            }
            else if(health._authority != true)
            {
                status = statusElementHealth.HEALTH_STATUS.STATUS_PROBLEM;
            }
            else
            {
                bool bad_sensor = find_match_byte_in_array((byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref health._health._sensors);
                bool bad_camera = find_match_byte_in_array((byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref health._health._cameras);
                bool bad_camera_record = find_match_byte_in_array((byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref health._health._cameras_record);
                bool bad_fans = find_match_byte_in_array((byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref health._health._fans);
                bool bad_record = (health._health._record == (long)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL);

                if(bad_sensor ||
                   bad_camera ||
                   bad_camera_record ||
                   bad_record ||
                   bad_fans)
                {
                    status = statusElementHealth.HEALTH_STATUS.STATUS_PROBLEM;
                }

                bad_status._camera = bad_camera;
                bad_status._camera_record = bad_camera_record;
                bad_status._alarm_in = bad_sensor;
                bad_status._fan = bad_fans;
                bad_status._record = bad_record;
            }
           
            return status;
        }

        public bool find_match_byte_in_array(byte compare_target, ref byte[] container)
        {
            bool res = false;
            foreach (byte element in container)
            {
                if (element == compare_target)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        public bool for_string(ref byte[] element, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM compare_target, ref string str)
        {
            str = "";
            if (element.Length == 0) return false;
            if (element.Length == 1 && element.GetValue(0).Equals((byte)G2MONITORING_DEVICE_STATUS.NOT_SUPPORT))
            {
                str = " - - - - - ";
                return true;
            }
            return string_abbereviate_channel_set(ref element, ref str, compare_target);
        }

        public bool string_abbereviate_channel_set(ref byte[] element, ref string str, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM compare_target)
        {
            if (element.Length == 0) return false;
            int count = 0;
            for (int i = 0; i < element.Length; i++)
            {
                if (element[i].Equals((byte)compare_target))
                {
                    if (count == 0)
                    {
                        str += ((str.Length == 0) ? (string)("" + i + 1) : (string)("," + i + 1));
                    }
                    ++count;
                }
                else 
                {
                    if (count >= 2)
                    {
                        str += (string)("~" + i);
                    }
                    count = 0;
                }
            }
            if (count >= 2)
            {
                str += (string)("~" + element.Length);
            }

            bool result = (str.Length != 0);
            return result;
        }

        public void parse_health_info_array(ref G2MONITORING_DEVICE_HEALTH health, out statusElementHealth.element element)
        {

            statusElementHealth.HEALTH_STATUS status;
            statusElementHealth.bad_status_t bad_status = new statusElementHealth.bad_status_t();

            element = new statusElementHealth.element();
            element._status = "";
            element._problem = "";
            element._version = "";
            element._site = "";
            element._address = "";
            element._mac = "";
            element._cameras = "";
            element._record = "";
            element._recordCheck = "";
            element._recordPeriod = "";
            element._group = "";
            element._alarm_in = "";
            element._fan = "";

            G2DEVICE_ROOT root;
            g2admin.get().get_root_device_info(ref health._site, out root);
            element._site = root._name;
            element._address = root._ni._address;
            element._mac = health._mac.ToString();
            G2DEVICE_GROUP groups;
            g2admin.get().get_device_group_info(ref health._site, out groups);
            element._group = groups._name;

            status = for_status(ref health, ref bad_status);
            if (status == statusElementHealth.HEALTH_STATUS.STATUS_UNREACHABLE)
            {
                element._problem = g2foundation.get_string_disconnect_reason((G2DISCONNECT_REASON.TYPE)health._reason_disconnect);
            }
            else if (status == statusElementHealth.HEALTH_STATUS.STATUS_PROBLEM)
            {
                if (bad_status._record)
                {
                    element._problem += "Record Bad";
                }
                if (bad_status._camera)
                {
                    if (element._problem.Length != 0)
                    {
                        element._problem += ",";
                    }
                    element._problem += "Video Loss";
                }
                if (bad_status._alarm_in)
                {
                    if (element._problem.Length != 0)
                    {
                        element._problem += ",";
                    }
                    element._problem += "AlarmIn Bad";
                }
                if (bad_status._fan)
                {
                    if (element._problem.Length != 0)
                    {
                        element._problem += ",";
                    }
                    element._problem += "Fan Error On";
                }
                if (element._problem.Length == 0)
                {
                    element._problem = g2foundation.get_string_disconnect_reason((G2DISCONNECT_REASON.TYPE)health._reason_disconnect);
                }

            }
            if (status == statusElementHealth.HEALTH_STATUS.STATUS_PROBLEM || status == statusElementHealth.HEALTH_STATUS.STATUS_HEALTHY)
            {
                element._address = root._ni._address_resolved;
            }

            if (health._authority.Equals(false))
            {
                element._status = "No Authority";
            }

            element._version = health._version;
            element._status = (status == statusElementHealth.HEALTH_STATUS.STATUS_HEALTHY) ? "Healthy" :
                         (status == statusElementHealth.HEALTH_STATUS.STATUS_PROBLEM) ? "Problem" :
                         (status == statusElementHealth.HEALTH_STATUS.STATUS_UNREACHABLE) ? "Unreachable" :
                                                                        "Unknown";

            bool proceed = false;
            proceed = status != statusElementHealth.HEALTH_STATUS.STATUS_UNREACHABLE &&
                      health._authority;

            if (proceed)
            {
                string str = "";
                int count = 0;
                foreach (byte value in health._health._sensors)
                {
                    if (value != (byte)G2MONITORING_DEVICE_STATUS.NOT_VALUE) count++;
                }
                if (count == 0)
                {
                    element._alarm_in = "";
                }

                if (health._health._sensors[0].Equals((byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NOT_USE))
                {
                    element._alarm_in = "Not Use";
                }
                else if (for_string(ref health._health._sensors, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref str))
                {
                    element._alarm_in = str;
                }
                else if (for_string(ref health._health._sensors, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, ref str))
                {
                    element._alarm_in = str;
                }
                else
                {
                    element._alarm_in = "Not Use";
                }

                count = 0;
                foreach (byte value in health._health._fans)
                {
                    if (value != (byte)G2MONITORING_DEVICE_STATUS.NOT_VALUE) count++;
                }
                if (count == 0)
                {
                    element._fan = "";
                }
                else if (for_string(ref health._health._fans, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref str))
                {
                    element._fan = str;
                }
                else if (for_string(ref health._health._fans, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, ref str))
                {
                    element._fan = str;
                }
                else
                {
                    element._fan = "Not Use";
                }
                count = 0;
                foreach (byte value in health._health._cameras)
                {
                    if (value != (byte)G2MONITORING_DEVICE_STATUS.NOT_VALUE) count++;
                }
                if (count == 0)
                {
                    element._cameras = "";
                }
                else if (for_string(ref health._health._cameras, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, ref str))
                {
                    element._cameras = str;
                }
                else if (for_string(ref health._health._cameras, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, ref str))
                {
                    element._cameras = str;
                }
                else
                {
                    element._cameras = "Not Use";
                }
                
                if (health._health._record.Equals((long)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL))
                {
                    element._recordCheck = "BAD";
                }
                else if (health._health._record.Equals((long)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL))
                {
                    element._recordCheck = "GOOD";
                }
                else if (health._health._record.Equals((long)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NOT_SUPPORT))
                {
                    element._recordCheck = " - - - - - ";
                }
                else
                {
                    element._recordCheck = "Off";
                }
               
                if (health._record._begin.is_same_date(health._record._end)||
                    health._record._begin.valid != true||
                    health._record._end.valid != true)
                {
                    element._recordPeriod = " - - - - - ";
                }
                else
                {
                    element._recordPeriod = health._record._begin.to_string_date_time() + " ~ " + health._record._end.to_string_date_time();
                }
                
                element._record = (health._record._on._record == (byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NOT_SUPPORT) ? "- - - " :
                    ((health._record._mode & (uint)G2MONITORING_DEVICE_STATUS.RECORD_MODE.RECORDING) == (uint)G2MONITORING_DEVICE_STATUS.RECORD_MODE.RECORDING) ? "On" : "Off";
            }
    
        }

        public bool isUpdated(statusElementHealth.element prevElement, ref statusElementHealth.element element)
        {
            return ((prevElement._status != element._status) ||
                   (prevElement._problem != element._problem) ||
                   (prevElement._group != element._group) ||
                   (prevElement._site != element._site) ||
                   (prevElement._address != element._address) ||
                   (prevElement._mac != element._mac) ||
                   (prevElement._version != element._version) ||
                   (prevElement._cameras != element._cameras) ||
                   (prevElement._alarm_in != element._alarm_in) ||
                   (prevElement._fan != element._fan) ||
                   (prevElement._record != element._record) ||
                   (prevElement._recordCheck != element._recordCheck) ||
                   (prevElement._recordPeriod != element._recordPeriod));
        }

        #endregion

        public void append_instant_event(G2EVENT_INFO info)
        {
            imp_append_instant_event(info, true);
        }

        public void append_instant_event(G2EVENT_INFO info, bool monitoring)
        {
            imp_append_instant_event(info, monitoring);
        }

        public void imp_append_instant_event(G2EVENT_INFO info, bool monitoring)
        {
            if (monitoring != true)
            {
                if (_monitor_adaptor.is_monitoring())
                {
                    return;
                }
            }
            append_event(info);

        }

        private void append_event(G2EVENT_INFO info)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { append_event(info); });
                return;
            }
            string evt = info.string_event_type();
            G2DEVICE_ROOT root;
            g2admin.get().get_root_device_info(ref info._source, out root);
            string label = info._event_ras._label._string;
            string device = root._name;
            string ti = info._spot._time.ToString();

            LSV_INSTANT_EVENT.BeginUpdate();
            {
                ListViewItem lvi = new ListViewItem(evt);
                lvi.SubItems.Add(label);
                lvi.SubItems.Add(device);
                lvi.SubItems.Add(ti);
                LSV_INSTANT_EVENT.Items.Insert(0, lvi);
            }
            LSV_INSTANT_EVENT.EndUpdate();
        }

        private void update_health_info(G2MONITORING_DEVICE_HEALTH[] health)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { update_health_info(health); });
                return;
            }
            bool change = false;
            statusElementHealth.element element;
            Dictionary<G2GUID, statusElementHealth.element> changedList = new Dictionary<G2GUID, statusElementHealth.element>();
            for (int i = 0; i < health.Length; i++)
            {
                parse_health_info_array(ref health[i], out element);
                if (_prevElement.ContainsKey(health[i]._site) != true)
                {
                    changedList.Add(health[i]._site, element);
                    change = true;
                }
                else
                {
                    G2GUID updated = G2GUID.INVALID_GUID;
                    if (isUpdated(_prevElement[health[i]._site], ref element))
                    {
                        changedList.Add(health[i]._site, element);
                        change = true;
                    }
                }
            }

            if (change == true)
            {
                LSV_DEVICE_HEALTH.BeginUpdate();
                foreach (KeyValuePair<G2GUID, statusElementHealth.element> kvp in changedList)
                {
                    string[] arr = new string[13];
                    arr[0] = kvp.Value._status;
                    arr[1] = kvp.Value._problem;
                    arr[2] = kvp.Value._group;
                    arr[3] = kvp.Value._site;
                    arr[4] = kvp.Value._address;
                    arr[5] = kvp.Value._mac;
                    arr[6] = kvp.Value._version;
                    arr[7] = kvp.Value._cameras;
                    arr[8] = kvp.Value._alarm_in;
                    arr[9] = kvp.Value._fan;
                    arr[10] = kvp.Value._record;
                    arr[11] = kvp.Value._recordCheck;
                    arr[12] = kvp.Value._recordPeriod;
                    ListViewItem lvi = new ListViewItem(arr);
                    lvi.Tag = kvp.Key;
                    int idx = -1;
                    foreach (ListViewItem lv in LSV_DEVICE_HEALTH.Items)
                    {
                        if ((G2GUID)lv.Tag == kvp.Key)
                        {
                            idx = lv.Index;
                            break;
                        }
                    }
                    if (idx < 0)
                    {
                        LSV_DEVICE_HEALTH.Items.Add(lvi);
                    }
                    else
                    {
                        LSV_DEVICE_HEALTH.Items[idx] = lvi;
                    }

                    if (_prevElement.ContainsKey(kvp.Key))
                    {
                        _prevElement[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        _prevElement.Add(kvp.Key, kvp.Value);
                    }

                }
                LSV_DEVICE_HEALTH.EndUpdate();
            }
        }
    }
}