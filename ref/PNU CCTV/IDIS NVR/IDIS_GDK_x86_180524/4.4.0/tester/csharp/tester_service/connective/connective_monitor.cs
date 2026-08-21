using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Threading;

using GDK;

namespace GDK_tester
{
    using G2HMONITOR = System.Int32;
    using G2HWND = System.IntPtr;
#if _WIN64
    using G2WPARAM = System.UInt64;
    using G2LPARAM = System.Int64;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int64;
#else
    using G2WPARAM = System.UInt32;
    using G2LPARAM = System.Int32;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int32;
#endif

    public partial class form_admin : g2monitor_listener
    {
        private g2monitor _monitor_adaptor;
        private G2GUIDSET _monitor_guids;
        private G2HMONITOR _monitor_handle;
        Dictionary<G2GUID, statusElementHealth.element> _prevElement;
        private EventWaitHandle _waitForSingle;

        public void init_connective_monitor()
        {
            _monitor_adaptor = g2monitor.get();
            _monitor_adaptor.set_listener(this);
            _monitor_adaptor.startup();
            _monitor_guids = new G2GUIDSET();
            _prevElement = new Dictionary<G2GUID, statusElementHealth.element>();
        }

        public void on_post_monitor_connected()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_monitor_connected(); });
                return;
            }

            LSV_DEVICE_HEALTH.Items.Clear();
            _prevElement.Clear();

            if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_DEVICE_HEALTH_MONITORING))
            {
                LSV_DEVICE_HEALTH.Enabled = true;
                request_health_info(ref _monitor_guids);
            }
            else
            {
                _monitor_adaptor.request_delete_device_status_and_health();
                LSV_DEVICE_HEALTH.Enabled = false;
            }
        }

        public void on_post_monitor_disconnected()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_monitor_disconnected(); });
                return;
            }

            LSV_DEVICE_HEALTH.Items.Clear();
            LSV_INSTANT_EVENT.Items.Clear();
        }

        #region g2monitor_listener

        public void on_g2monitor_connected(G2HMONITOR handle, ref G2GUID service)
        {
            _monitor_handle = handle;

            if (g2admin.get().is_connected() != true)
            {
                lock (_adaptor)
                {
                    _adaptor.set_disconnect_by_me(true);
                    _adaptor.disconnect();
                }
                return;
            }

            on_post_monitor_connected();
        }

        public void on_g2monitor_disconnected(G2HMONITOR handle, ref G2GUID service, G2DISCONNECT_REASON.TYPE reason)
        {
            _monitor_handle = -1;

            bool reconnect = true;
            switch (reason)
            {
                case G2DISCONNECT_REASON.TYPE.INVALID_VERSION:
                case G2DISCONNECT_REASON.TYPE.LOGIN_FAIL:
                case G2DISCONNECT_REASON.TYPE.NO_AUTHORITY:
                    reconnect = false;
                    break;
            }

            if (g2admin.get().is_connected())
            {
                if (reconnect)
                {
                    //do nothing..
                }
                else
                {
                    _adaptor.set_disconnect_by_me(true);
                }
            }
            if (_waitForSingle != null)
            {
               _waitForSingle.Set();
            }

            on_post_monitor_disconnected();
        }

        public void on_g2monitor_require_reconnect(G2HMONITOR handle, ref G2GUID service, ref bool reconnect)
        {
            reconnect = true;
        }

        public void on_g2monitor_receive_event(G2HMONITOR handle, ref G2EVENT_INFO ei)
        {
            append_instant_event(ei);
        }

        public void on_g2monitor_receive_service_log_load(G2HMONITOR handle, ref G2SYSTEM_LOG log)
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form.GUID.is_service_monitoring)
                    {
                        sub_form.add_system_log(log);
                        sub_form.S_ROW_ID = log._row_id;
                        break;
                    }
                }
            }
        }

        public void on_g2monitor_receive_service_log_load_end(G2HMONITOR handle)
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form.GUID.is_service_monitoring)
                    {
                        sub_form.str[0] = "Complete!";
                        sub_form.log_load_end_update_list();
                        break;
                    }
                }
            }
        }

        public void on_g2monitor_receive_service_log_load_fail(G2HMONITOR handle)
        {
            Debug.WriteLine("callback [on_g2monitor_receive_service_log_load_fail] is not implemented.");
        }

        public void on_g2monitor_receive_service_log_load_stop(G2HMONITOR handle)
        {
            Debug.WriteLine("callback [on_g2monitor_receive_service_log_load_stop] is not implemented.");
        }

        public void on_g2monitor_status_connected(G2HMONITOR handle, ref G2GUID service)
        {
            Debug.WriteLine("callback [on_g2monitor_status_connected] is not implemented.");
        }

        public void on_g2monitor_status_disconnected(G2HMONITOR handle, ref G2GUID service, G2DISCONNECT_REASON.TYPE reason)
        {
            Debug.WriteLine("callback [on_g2monitor_status_disconnected] is not implemented.");
        }

        public void on_g2monitor_status_receive_device_status(G2HMONITOR handle, G2MONITORING_DEVICE_STATUS[] status)
        {
            Debug.WriteLine("callback [on_g2monitor_status_receive_device_status] is not implemented.");
        }

        public void on_g2monitor_status_receive_device_health(G2HMONITOR handle, G2MONITORING_DEVICE_HEALTH[] health)
        {
            update_health_info(health);
        }

        public void on_g2monitor_status_receive_event(G2HMONITOR handle, ref G2EVENT_INFO ei)
        {
            Debug.WriteLine("callback [on_g2monitor_status_receive_event] is not implemented.");
        }

        public void on_g2monitor_receive_debug_log_load(int handle, ref G2DEBUG_LOG log)
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form.GUID.is_service_monitoring)
                    {
                        sub_form.add_debug_log(log);
                        sub_form.D_ROW_ID = log._row_id;
                        break;
                    }
                }
            }
        }

        public void on_g2monitor_receive_debug_log_load_end(int handle)
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form.GUID.is_service_monitoring)
                    {
                        sub_form.str[1] = "Complete!";
                        sub_form.log_load_end_update_list();
                        break;
                    }
                }
            }
        }

        public void on_g2monitor_receive_debug_log_load_fail(int handle)
        {
            Debug.WriteLine("callback [on_g2monitor_receive_debug_log_load_fail] is not implemented.");
        }

        public void on_g2monitor_receive_debug_log_load_stop(int handle)
        {
            Debug.WriteLine("callback [on_g2monitor_receive_debug_log_load_stop] is not implemented.");
        }

        #endregion
    }
}
