using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using GDK;

namespace GDK_tester
{
    public partial class form_admin : g2admin_listener
    {
        private g2admin _adaptor;
        private int _handle;
        private List<G2GUID> _connectable_services;
        private List<TreeNode> _checked_camera_node_list;
        private List<DisableDeviceNode> _disable_devices;
        private List<Form> _sub_forms;
        private G2NETWORK_INFO _ni;
        private object _lockObject;
        private System.Windows.Forms.Timer _recording_status_timer;
        private bool _finalize;
        private string _xml_path = "./tester.xml";
        private int _failover_test_num;
        

        private void on_post_admin_connected()
        {
            BTN_DISCONNECT.Enabled = true;
        }

        private void on_post_admin_disconnected(string reason)
        {
            BTN_CONNECT_FORM.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
            BTN_GET_GUID.Enabled = false;
            TRV_DEVICE_LIST.Nodes[0].Nodes.Clear();
            TRV_DEVICE_LIST.Nodes[1].Nodes.Clear();
            TRV_DEVICE_LIST.Enabled = false;
            DGV_Record_Device_Status.Enabled = false;
            DGV_Record_Device_Status.Rows.Clear();
            BTN_LIVE.Enabled = false;
            BTN_PLAY.Enabled = false;
            BTN_BACKUP.Enabled = false;
            LB_INFO.Text = "Disconnect : " + reason;
            CB_GET_TYPE.Items.Clear();
            CB_GET_TYPE.Enabled = false;
            CB_ADMIN.Items.Clear();
            CB_ADMIN.Enabled = false;
            BTN_ADMIN_LOG.Enabled = false;
            LB_SERVICES.Items.Clear();
            BTN_SERVICE_LOG.Enabled = false;
            _checked_camera_node_list.Clear();
            _connectable_services.Clear();
            _disable_devices.Clear();
            monitor_unset_guids();
        }

        private void on_post_login_completed()
        {
            CB_GET_TYPE.Enabled = true;
            CB_GET_TYPE.Items.Add("IP Address");
            CB_GET_TYPE.Items.Add("FEN Name");
            CB_GET_TYPE.Items.Add("MAC Address");
            DGV_Record_Device_Status.Enabled = true;
            _recording_status_timer.Interval = 100;
            _recording_status_timer.Tick += new EventHandler(recording_status_timer_tick);
            _recording_status_timer.Start();
            LB_INFO.Text = "Connect success";

            G2GUIDLIST guids = new G2GUIDLIST();
            _adaptor.get_root_guid(guids);
            monitor_set_guids(ref guids);

            check_admin_features_auth();

            for (int i = 0; i < _connectable_services.Count; ++i)
            {
                G2GUID service = _connectable_services[i];

                if (service.is_service_monitoring)
                {
                    connect_monitor(service);
                }

                addServiceListBox(service);
            }

            G2GUIDLIST service_list = new G2GUIDLIST();
            _adaptor.get_service_guid(service_list);
            foreach (G2GUID service in service_list)
            {
                if (service.is_service_administration)
                {
                    admin_failover_net_info_save(service);
                    break;
                }
            }
        }

        private void admin_failover_net_info_save(G2GUID admin_failover_service)
        {
            G2SERVICE info;
            _adaptor.get_service_info(ref admin_failover_service, out info);

            try
            {
                if (System.IO.File.Exists(_xml_path))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(_xml_path);

                    XmlNode administrations = doc.SelectSingleNode("Administrations");
                    XmlNode old = administrations.SelectSingleNode("Admin[@Address='" + _ni._address.ToString() + "']");
                    if (old != null) administrations.RemoveChild(old);

                    XmlElement admin = doc.CreateElement("Admin");
                    XmlAttribute attr = doc.CreateAttribute("Address");
                    attr.Value = _ni._address.ToString();
                    admin.Attributes.Append(attr);

                    XmlElement failover = doc.CreateElement("Failover");
                    attr = doc.CreateAttribute("G2GUID");
                    attr.Value = info._guid.ToString();
                    failover.Attributes.Append(attr);

                    for (int i = 0; i < info._ni._count; ++i)
                    {
                        XmlElement netInfo = doc.CreateElement("NetInfo");
                        XmlAttribute num = doc.CreateAttribute("Number");
                        num.Value = i.ToString();
                        netInfo.Attributes.Append(num);

                        XmlElement inf = doc.CreateElement("Port");
                        inf.InnerXml = info._ni._element[i]._port[info._ni._element[i]._type].ToString();
                        netInfo.AppendChild(inf);

                        inf = doc.CreateElement("Address");
                        inf.InnerXml = info._ni._element[i]._address._string;
                        netInfo.AppendChild(inf);

                        failover.AppendChild(netInfo);
                    }

                    admin.AppendChild(failover);
                    doc.DocumentElement.AppendChild(admin);

                    doc.Save(_xml_path);
                }
                else
                {
                    XmlDocument newDoc = new XmlDocument();
                    newDoc.AppendChild(newDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
                    newDoc.LoadXml("<Administrations></Administrations>");

                    XmlElement admin = newDoc.CreateElement("Admin");
                    XmlAttribute attr = newDoc.CreateAttribute("Address");
                    attr.Value = _ni._address.ToString();
                    admin.Attributes.Append(attr);

                    XmlElement failover = newDoc.CreateElement("Failover");
                    attr = newDoc.CreateAttribute("G2GUID");
                    attr.Value = info._guid.ToString();
                    failover.Attributes.Append(attr);

                    for (int i = 0; i < info._ni._count; ++i)
                    {
                        XmlElement netInfo = newDoc.CreateElement("NetInfo");
                        XmlAttribute num = newDoc.CreateAttribute("Number");
                        num.Value = i.ToString();
                        netInfo.Attributes.Append(num);

                        XmlElement inf = newDoc.CreateElement("Port");
                        inf.InnerXml = info._ni._element[i]._port[info._ni._element[i]._type].ToString();
                        netInfo.AppendChild(inf);

                        inf = newDoc.CreateElement("Address");
                        inf.InnerXml = info._ni._element[i]._address._string;
                        netInfo.AppendChild(inf);

                        failover.AppendChild(netInfo);
                    }
                    
                    admin.AppendChild(failover);
                    newDoc.DocumentElement.AppendChild(admin);

                    using (XmlTextWriter writer = new XmlTextWriter(_xml_path, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        newDoc.Save(writer);
                    }
                }
            }
            catch (System.Exception) { }
        }

        private void addServiceListBox(G2GUID service)
        {
            G2SERVICE service_info;
            bool res = _adaptor.get_service_info(ref service, out service_info);
            if (res)
            {
                ListBoxItem item = new ListBoxItem(service_info._name._string, service_info._guid);
                LB_SERVICES.Items.Add(item);
            }
        }

        public class ListBoxItem
        {
            private string name;
            public G2GUID guid;

            public ListBoxItem(string name, G2GUID guid)
            {
                this.name = name;
                this.guid = guid;
            }

            public override string ToString()
            {
                return name;
            }
        }

        private void record_device_status_request()
        {
            G2GUIDSET record_camera_guids = new G2GUIDSET();
            for (int i = 0; i < _connectable_services.Count; ++i)
            {
                if (_connectable_services[i].is_service_recording || _connectable_services[i].is_service_recording_failover)
                {
                    G2GUID recording_service_guid = _connectable_services[i];   // can have more than one recording service
                    G2GUIDLIST guid_list = new G2GUIDLIST();
                    _adaptor.get_camera_guid_from_recording_service(ref recording_service_guid, guid_list);
                    record_camera_guids.AddRange(guid_list);
                }
            }

            G2GUIDSET record_device_guids = new G2GUIDSET();
            for (int i = 0; i < record_camera_guids.Count; ++i)
            {
                G2GUID guid = record_camera_guids[i];
                G2GUID root = _adaptor.get_root_guid_from_leaf_guid(ref guid);
                if (!record_device_guids.contains(root))
                {
                    record_device_guids.Add(root);
                }
            }

            _adaptor.request_recording_device_status(record_device_guids, true);       // it is registration to watching recording device status
        }

        private void on_post_receive_recording_device_status(G2GUID root, int connection, int disconnect_reason, G2RECORDING_CAMERA_STATUS[] status)
        {
            Monitor.Enter(_lockObject);
            G2DEVICE_ROOT root_info;
            _adaptor.get_root_device_info(ref root, out root_info);
            string str_root_name = root_info._name._string;
            string str_connection;
            if (connection == 3)
            {
                str_connection = "OK";
            }
            else
            {
                str_connection = g2foundation.get_string_disconnect_reason((G2DISCONNECT_REASON.TYPE)disconnect_reason);
            }

            for (int i = 0; i < status.Length; ++i)
            {
                string[] row = new string[6];
                row[0] = str_root_name;
                G2DEVICE_LEAF camera_info;
                _adaptor.get_leaf_device_info(ref status[i]._camera, out camera_info);
                row[1] = camera_info._name._string;
                row[2] = str_connection;
                if (status[i]._on_recording && status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.RECORD_SUCCESS)
                {
                    row[3] = "OK";
                }
                else
                {
                    if (status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.DEACTIVATED)
                    {
                        row[3] = "Deactivated";
                    }
                    else if (status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.DISCONNECTED)
                    {
                        row[3] = "Disconnected";
                    }
                    else if (status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.NO_SCHEDULE)
                    {
                        row[3] = "No schedule";
                    }
                    else if (status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.RECORD_FAIL)
                    {
                        row[3] = "Record fail";
                    }
                    else if (status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.STORAGE_FULL)
                    {
                        row[3] = "Storage full";
                    }
                    else if (status[i]._rec_fail_reason == G2RECORDING_CAMERA_STATUS.REC_FAIL_REASON.VIDEO_LOSS)
                    {
                        row[3] = "Video loss";
                    }
                    else
                    {
                        row[3] = "--";
                    }
                }
                row[4] = status[i]._on_recording_audio ? "OK" : "--";
                if (status[i]._rec_type == G2RECORDING_CAMERA_STATUS.REC_TYPE.EVENT)
                {
                    row[5] = "event";
                }
                else if (status[i]._rec_type == G2RECORDING_CAMERA_STATUS.REC_TYPE.IDLE)
                {
                    row[5] = "idle";
                }
                else if (status[i]._rec_type == G2RECORDING_CAMERA_STATUS.REC_TYPE.INSTANT)
                {
                    row[5] = "instant";
                }
                else if (status[i]._rec_type == G2RECORDING_CAMERA_STATUS.REC_TYPE.TIME_LAPSE)
                {
                    row[5] = "time lapse";
                }
                else
                {
                    row[5] = "--";
                }

                bool contain = false;
                for (int j = 0; j < DGV_Record_Device_Status.Rows.Count; ++j)
                {
                    if ((G2GUID)DGV_Record_Device_Status.Rows[j].Tag == status[i]._camera)
                    {
                        contain = true;
                        if (row[0] == "" && row[1] == "")
                        {
                            DGV_Record_Device_Status.Rows.RemoveAt(j);
                        }
                        else
                        {
                            DGV_Record_Device_Status.Rows.RemoveAt(j);
                            DGV_Record_Device_Status.Rows.Insert(j, row);
                            DGV_Record_Device_Status.Rows[j].Tag = status[i]._camera;
                        }
                    }
                }
                if (!contain)
                {
                    DGV_Record_Device_Status.Rows.Add(row);
                    DGV_Record_Device_Status.Rows[DGV_Record_Device_Status.Rows.Count - 1].Tag = status[i]._camera;
                }

                // record dot, instant rec ////////////////////////////////////////////////////////////
                for (int r = 0; r < _sub_forms.Count; ++r)
                {
                    if (_sub_forms[r] is form_live)
                    {
                        ((form_live)_sub_forms[r]).set_is_rec(status[i]._camera, status[i]._on_recording);
                        if (((form_live)_sub_forms[r]).is_current_panel(status[i]._camera))
                        {
                            ((form_live)_sub_forms[r]).feature_control_instant_rec(true, status[i]._camera);
                        }
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////

            }

            //_adaptor.request_recording_device_status(ref root, false);      // it is exclusion to watching recording device status
            
            Monitor.Exit(_lockObject);
        }

        private void on_post_receive_device_group_list(G2DEVICE_GROUP[] bunch)
        {
            TRV_DEVICE_LIST.Enabled = true;

            TreeNodeCollection trnc = TRV_DEVICE_LIST.Nodes;
            int group_index = trnc.IndexOfKey("DeviceGroup");   // = 1
            trnc[group_index].SelectedImageIndex = 1;

            for (int i = 0; i < bunch.Length; ++i)
            {
                TreeNode[] find_res = trnc[group_index].Nodes.Find(bunch[i]._guid.ToString(), true);
                if (find_res.Length == 0)
                {
                    add_group_node(bunch[i]);
                }
            }

            TRV_DEVICE_LIST.Sort();
        }

        private void add_group_node(G2DEVICE_GROUP group_info)
        {
            TreeNodeCollection trnc = TRV_DEVICE_LIST.Nodes;
            int group_index = trnc.IndexOfKey("DeviceGroup");   // = 1

            if (group_info._guid_parent.valid)
            {
                TreeNode[] find_res = trnc[group_index].Nodes.Find(group_info._guid_parent.ToString(), true);
                if (find_res.Length == 0)
                {
                    G2GUID guid_parent = group_info._guid_parent;
                    G2DEVICE_GROUP parent_group_info;
                    _adaptor.get_device_group_info(ref guid_parent, out parent_group_info);
                    add_group_node(parent_group_info);

                    TreeNode[] find_parent_res = trnc[group_index].Nodes.Find(guid_parent.ToString(), true);
                    find_parent_res[0].Nodes.Add(group_info._guid.ToString(), group_info._name._string);
                    int index_leaf = find_parent_res[0].Nodes.IndexOfKey(group_info._guid.ToString());
                    find_parent_res[0].Nodes[index_leaf].Tag = group_info._guid;
                    find_parent_res[0].Nodes[index_leaf].ImageIndex = 2;
                    find_parent_res[0].Nodes[index_leaf].SelectedImageIndex = 2;
                }
                else
                {
                    find_res[0].Nodes.Add(group_info._guid.ToString(), group_info._name._string);
                    int index_leaf = find_res[0].Nodes.IndexOfKey(group_info._guid.ToString());
                    find_res[0].Nodes[index_leaf].Tag = group_info._guid;
                    find_res[0].Nodes[index_leaf].ImageIndex = 2;
                    find_res[0].Nodes[index_leaf].SelectedImageIndex = 2;
                }
            }
            else
            {
                trnc[group_index].Nodes.Add(group_info._guid.ToString(), group_info._name._string);
                int index = trnc[group_index].Nodes.IndexOfKey(group_info._guid.ToString());
                trnc[group_index].Nodes[index].Tag = group_info._guid;
                trnc[group_index].Nodes[index].ImageIndex = 2;
                trnc[group_index].Nodes[index].SelectedImageIndex = 2;
            }
        }

        private void on_post_receive_device_list(G2GUID[] bunch)
        {
            List<TreeNode> remove_items = new List<TreeNode>();

            for (int i = 0; i < bunch.Length; ++i)
            {
                G2GUID guid = bunch[i];
                if (guid.is_device_parent)
                {
                    G2DEVICE_ROOT root_info;
                    if (guid.is_device_root)
                    {
                        _adaptor.get_root_device_info(ref guid, out root_info);
                    }
                    else
                    {
                        _adaptor.get_parent_device_info(ref guid, out root_info);
                    }
                    if (!TRV_DEVICE_LIST.Nodes[0].Nodes.ContainsKey(guid.ToString()))        // Nodes[0] = All Device
                    {
                        TRV_DEVICE_LIST.Nodes[0].Nodes.Add(guid.ToString(), root_info._name._string);
                        int index = TRV_DEVICE_LIST.Nodes[0].Nodes.IndexOfKey(guid.ToString());
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index].Tag = guid;
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index].ImageIndex = 3;
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index].SelectedImageIndex = 3;

                        if (root_info._enable == false)
                        {
                            TreeNode it = TRV_DEVICE_LIST.Nodes[0].Nodes[index];
                            if (remove_items.Contains(it) == false)
                            {
                                remove_items.Add(it);
                            }
                        }
                    }
                }
                else if (guid.is_device_leaf && (guid.is_device_camera || guid.is_device_hybrid_camera))    // only camera
                {
                    G2DEVICE_LEAF leaf_info;
                    _adaptor.get_leaf_device_info(ref guid, out leaf_info);
                    G2DEVICE_ROOT root_info;
                    _adaptor.get_parent_device_info(ref guid, out root_info);
                    if (!TRV_DEVICE_LIST.Nodes[0].Nodes.ContainsKey(root_info._guid.ToString()))
                    {
                        TRV_DEVICE_LIST.Nodes[0].Nodes.Add(root_info._guid.ToString(), root_info._name._string);
                        int index = TRV_DEVICE_LIST.Nodes[0].Nodes.IndexOfKey(root_info._guid.ToString());
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index].Tag = root_info._guid;
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index].ImageIndex = 3;
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index].SelectedImageIndex = 3;

                        if (root_info._enable == false)
                        {
                            TreeNode it = TRV_DEVICE_LIST.Nodes[0].Nodes[index];
                            if (remove_items.Contains(it) == false)
                            {
                                remove_items.Add(it);
                            }
                        }
                    }

                    int index2 = TRV_DEVICE_LIST.Nodes[0].Nodes.IndexOfKey(root_info._guid.ToString());
                    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes.Add(guid.ToString(), leaf_info._name._string);
                    int index3 = TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes.IndexOfKey(guid.ToString());
                    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].Tag = guid;
                    if (guid.is_device_camera || guid.is_device_hybrid_camera)
                    {
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].ImageIndex = 4;
                        TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].SelectedImageIndex = 4;

                        if (leaf_info._enable == false)
                        {
                            TreeNode it = TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3];
                            if (remove_items.Contains(it) == false)
                            {
                                remove_items.Add(it);
                            }
                        }
                    }
                    //          other devices
                    //else if (guid.is_device_audio_in || guid.is_device_audio_out)
                    //{
                    //    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].ImageIndex = 5;
                    //    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].SelectedImageIndex = 5;
                    //}
                    //else if (guid.is_device_alarm_in || guid.is_device_alarm_in_network || guid.is_device_alarm_out)
                    //{
                    //    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].ImageIndex = 6;
                    //    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].SelectedImageIndex = 6;
                    //}
                    //else if (guid.is_device_text_in || guid.is_device_text_in_network)
                    //{
                    //    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].ImageIndex = 7;
                    //    TRV_DEVICE_LIST.Nodes[0].Nodes[index2].Nodes[index3].SelectedImageIndex = 7;
                    //}
                }
            }

            foreach (TreeNode nod in remove_items)
            {
                _disable_devices.Add(new DisableDeviceNode(nod));
                TRV_DEVICE_LIST.Nodes.Remove(nod);
            }
        }

        private void on_post_receive_device_to_group_map(G2GROUP_MEMBER[] bunch)
        {
            List<TreeNode> remove_items = new List<TreeNode>();

            for (int i = 0; i < bunch.Length; ++i)
            {
                G2GUID bunch_group = bunch[i]._group;
                G2GUID bunch_member = bunch[i]._member;

                TreeNode[] find_group_res = TRV_DEVICE_LIST.Nodes[1].Nodes.Find(bunch_group.ToString(), true);
                TreeNodeCollection group_nodes = find_group_res[0].Nodes;
                
                if (bunch_member.is_device_parent)
                {
                    if (group_nodes.ContainsKey(bunch_member.ToString()) == false)
                    {
                        G2DEVICE_ROOT root_info;
                        if (bunch_member.is_device_root)
                        {
                            _adaptor.get_root_device_info(ref bunch_member, out root_info);
                        }
                        else
                        {
                            _adaptor.get_parent_device_info(ref bunch_member, out root_info);
                        }
                        
                        group_nodes.Add(bunch_member.ToString(), root_info._name._string);
                        int index = group_nodes.IndexOfKey(bunch_member.ToString());
                        group_nodes[index].Tag = bunch_member;
                        group_nodes[index].ImageIndex = 3;
                        group_nodes[index].SelectedImageIndex = 3;

                        if (root_info._enable == false)
                        {
                            TreeNode it = group_nodes[index];
                            if (remove_items.Contains(it) == false)
                            {
                                remove_items.Add(it);
                            }
                        }
                    }
                }
                else if (bunch_member.is_device_leaf && (bunch_member.is_device_camera || bunch_member.is_device_hybrid_camera))    // only camera
                {
                    G2DEVICE_LEAF leaf_info;
                    _adaptor.get_leaf_device_info(ref bunch_member, out leaf_info);
                    G2DEVICE_ROOT root_info;
                    _adaptor.get_parent_device_info(ref bunch_member, out root_info);
                    int index_group;
                    if (group_nodes.ContainsKey(root_info._guid.ToString()) == false)
                    {
                        group_nodes.Add(root_info._guid.ToString(), root_info._name._string);
                        index_group = group_nodes.IndexOfKey(root_info._guid.ToString());
                        group_nodes[index_group].Tag = root_info._guid;
                        group_nodes[index_group].ImageIndex = 3;
                        group_nodes[index_group].SelectedImageIndex = 3;

                        if (root_info._enable == false)
                        {
                            TreeNode it = group_nodes[index_group];
                            if (remove_items.Contains(it) == false)
                            {
                                remove_items.Add(it);
                            }
                        }
                    }
                    else
                    {
                        index_group = group_nodes.IndexOfKey(root_info._guid.ToString());
                    }
                    
                    group_nodes[index_group].Nodes.Add(bunch_member.ToString(), leaf_info._name._string);
                    int index_leaf = group_nodes[index_group].Nodes.IndexOfKey(bunch_member.ToString());
                    group_nodes[index_group].Nodes[index_leaf].Tag = bunch_member;
                    if (bunch_member.is_device_camera || bunch_member.is_device_hybrid_camera)
                    {
                        group_nodes[index_group].Nodes[index_leaf].ImageIndex = 4;
                        group_nodes[index_group].Nodes[index_leaf].SelectedImageIndex = 4;

                        if (leaf_info._enable == false)
                        {
                            TreeNode it = group_nodes[index_group].Nodes[index_leaf];
                            if (remove_items.Contains(it) == false)
                            {
                                remove_items.Add(it);
                            }
                        }
                    }
                    //      only camera 
                    //else if (bunch_member.is_device_audio_in || bunch_member.is_device_audio_out)
                    //{
                    //    group_nodes[index_group].Nodes[index_leaf].ImageIndex = 5;
                    //    group_nodes[index_group].Nodes[index_leaf].SelectedImageIndex = 5;
                    //}
                    //else if (bunch_member.is_device_alarm_in || bunch_member.is_device_alarm_in_network || bunch_member.is_device_alarm_out)
                    //{
                    //    group_nodes[index_group].Nodes[index_leaf].ImageIndex = 6;
                    //    group_nodes[index_group].Nodes[index_leaf].SelectedImageIndex = 6;
                    //}
                    //else if (bunch_member.is_device_text_in || bunch_member.is_device_text_in_network)
                    //{
                    //    group_nodes[index_group].Nodes[index_leaf].ImageIndex = 7;
                    //    group_nodes[index_group].Nodes[index_leaf].SelectedImageIndex = 7;
                    //}
                }
            }

            foreach (TreeNode nod in remove_items)
            {
                _disable_devices.Add(new DisableDeviceNode(nod));
                TRV_DEVICE_LIST.Nodes.Remove(nod);
            }

            TRV_DEVICE_LIST.Sort();
        }

        private void on_post_receive_device_list_remove_to_group(G2GROUP_MEMBER[] bunch)
        {
            for (int i = 0; i < bunch.Length; ++i)
            {
                int group_node_index = TRV_DEVICE_LIST.Nodes[1].Nodes.IndexOfKey(bunch[i]._group.ToString());
                TreeNode[] nodes = TRV_DEVICE_LIST.Nodes[1].Nodes[group_node_index].Nodes.Find(bunch[i]._member.ToString(), true);
                for (int j = 0; j < nodes.Length; ++j)
                {
                    TRV_DEVICE_LIST.Nodes[1].Nodes[group_node_index].Nodes.Remove(nodes[j]);
                }
            }
        }

        private void on_post_receive_device_remove_list(G2GUID[] bunch)
        {
            for (int i = 0; i < bunch.Length; ++i)
            {
                TreeNode[] nodes = TRV_DEVICE_LIST.Nodes.Find(bunch[i].ToString(), true);
                for (int j = 0; j < nodes.Length; ++j)
                {
                    TRV_DEVICE_LIST.Nodes.Remove(nodes[j]);
                }
            }
        }

        private void on_post_receive_device_group_remove(G2GUID group_guid)
        {
            TreeNode[] find_res = TRV_DEVICE_LIST.Nodes.Find(group_guid.ToString(), true);
            for (int i = 0; i < find_res.Length; ++i)
            {
                TRV_DEVICE_LIST.Nodes.Remove(find_res[i]);
            }
        }

        private void on_post_receive_device_group_modify(G2DEVICE_GROUP group)
        {
            TreeNode[] find_res = TRV_DEVICE_LIST.Nodes.Find(group._guid.ToString(), true);
            TreeNode group_node = find_res[0];
            if (group_node.Text != group._name._string)
            {
                group_node.Text = group._name._string;
            }
            if ((G2GUID)group_node.Parent.Tag != group._guid_parent)
            {
                TRV_DEVICE_LIST.Nodes.Remove(group_node);
                if (group._guid_parent.valid)
                {
                    TreeNode[] find_parent_res = TRV_DEVICE_LIST.Nodes.Find(group._guid_parent.ToString(), true);
                    TreeNode parent_group_node = find_parent_res[0];
                    parent_group_node.Nodes.Add(group_node);
                }
                else
                {
                    TRV_DEVICE_LIST.Nodes[1].Nodes.Add(group_node);
                }
            }
        }

        private void on_post_receive_device_modify(G2GUID guid)
        {
            TreeNode[] find_res = TRV_DEVICE_LIST.Nodes.Find(guid.ToString(), true);
            string name;
            bool disable = false;
            if (guid.is_device_parent || guid.is_device_root)
            {
                G2DEVICE_ROOT root_info;
                _adaptor.get_root_device_info(ref guid, out root_info);
                name = root_info._name._string;
                disable = root_info._enable ? false : true;
            }
            else
            {
                G2DEVICE_LEAF leaf_info;
                _adaptor.get_leaf_device_info(ref guid, out leaf_info);
                name = leaf_info._name._string;
                disable = leaf_info._enable ? false : true;
            }

            if (find_res.Length > 0)
            {
                for (int i = 0; i < find_res.Length; ++i)
                {
                    if (disable)
                    {
                        _disable_devices.Add(new DisableDeviceNode(find_res[i]));
                        TRV_DEVICE_LIST.Nodes.Remove(find_res[i]);
                    }
                    else
                    {
                        find_res[i].Text = name;
                    }
                }
            }
            else
            {
                if (disable == false)
                {
                    List<DisableDeviceNode> remove_items = new List<DisableDeviceNode>();
                    for (int i = 0; i < _disable_devices.Count; ++i)
                    {
                        G2GUID disable_guid = (G2GUID)_disable_devices[i].deviceNode.Tag;
                        if (disable_guid == guid)
                        {
                            _disable_devices[i].parentNode.Nodes.Add(_disable_devices[i].deviceNode);

                            remove_items.Add(_disable_devices[i]);
                        }
                    }

                    foreach (DisableDeviceNode item in remove_items)
                    {
                        _disable_devices.Remove(item);
                    }
                }
            }
        }

        private void recording_status_timer_tick(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Interval = 30 * 1000;
            record_device_status_request();
        }

        #region g2admin_listener 멤버

        public void on_g2admin_connected(int handle)
        {
            _handle = handle;
            _failover_test_num = 0;

            if (_finalize == false)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_admin_connected(); });
            }
        }

        public void on_g2admin_disconnected(int handle, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            _handle = -1;
            if (_finalize == false)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_admin_disconnected(reason.ToString()); });
            }

            if ((reason == G2DISCONNECT_REASON.TYPE.NO_SERVER || reason == G2DISCONNECT_REASON.TYPE.CONN_TIMEOUT
                ) && reason_user == G2SERVICE_LOGIN_FAIL_REASON.TYPE.NO_ERROR)
            {
                this.BeginInvoke((MethodInvoker)delegate() { test_admin_failover_connect(); });
            }
            else
            {
                _failover_test_num = 0;
            }
        }

        public void on_g2admin_failover_connected(int handle) 
        {
            Debug.WriteLine("callback [on_g2admin_failover_connected] is not implemented.");
        }

        public void on_g2admin_failover_failed(int handle, bool canceled) 
        {
            Debug.WriteLine("callback [on_g2admin_failover_failed] is not implemented.");
        }

        public void on_g2admin_failover_prepare(int handle, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user) 
        {
            Debug.WriteLine("callback [on_g2admin_failover_prepare] is not implemented.");
        }

        public void on_g2admin_login_completed(int handle)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_login_completed(); });
        }

        public void on_g2admin_login_failed_from_dvrns(int handle, ref G2NETWORK_INFO ni, G2FEN_RESULT.TYPE error) 
        {
            _handle = -1;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_admin_disconnected(error.ToString()); });
        }

        public void on_g2admin_notify_connectable_service(int handle, ref G2GUID service, ref G2NETWORK_INFO ni)
        {
            if (!_connectable_services.Contains(service))
            {
                _connectable_services.Add(service);

                if (_user_adaptor.is_login())
                {
                    G2GUID service2 = (G2GUID)service.Clone();
                    this.BeginInvoke((MethodInvoker)delegate() { addServiceListBox(service2); });
                }
            }
        }

        public void on_g2admin_receive_device_append(int handle, G2GROUP_MEMBER[] bunch)     // when device appended at no group / at group too
        {
            List<G2GUID> list = new List<G2GUID>();
            for (int i = 0; i < bunch.Length; ++i)
            {
                list.Add(bunch[i]._member);
            }
            
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_list(list.ToArray()); });
        }

        public void on_g2admin_receive_device_append_to_group(int handle, ref G2GUID device, ref G2GUID group) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_device_append_to_group] is not implemented.");
        }

        public void on_g2admin_receive_device_group_append(int handle, ref G2DEVICE_GROUP group)        // when group appended
        {
            List<G2DEVICE_GROUP> list = new List<G2DEVICE_GROUP>();
            list.Add(group);
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_group_list(list.ToArray()); });
        }    

        public void on_g2admin_receive_device_group_list(int handle, G2DEVICE_GROUP[] bunch) 
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_group_list(bunch); });
        }

        public void on_g2admin_receive_device_group_modify(int handle, ref G2DEVICE_GROUP group)        // group name modified and group hierarchy
        {
            G2DEVICE_GROUP modified_group = group;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_group_modify(modified_group); });
        }

        public void on_g2admin_receive_device_group_remove(int handle, ref G2GUID group)                // when group removed
        {
            G2GUID group_guid = group;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_group_remove(group_guid); });
        }            

        public void on_g2admin_receive_device_list(int handle, G2GUID[] bunch)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_list(bunch); });
        }

        public void on_g2admin_receive_device_list_append_to_group(int handle, G2GROUP_MEMBER[] bunch) // when device appended at group
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_to_group_map(bunch); });
        }

        public void on_g2admin_receive_device_list_remove_to_group(int handle, G2GROUP_MEMBER[] bunch)  // when remove device only at group
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_list_remove_to_group(bunch); });
        }

        public void on_g2admin_receive_device_modify(int handle, ref G2GUID device, bool reconnect)     // device name and enable modified without sync  at only local and with sync
        {
            G2GUID guid = device;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_modify(guid); });
        }     

        public void on_g2admin_receive_device_remove(int handle, ref G2GUID device) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_device_remove] is not implemented.");
        }

        public void on_g2admin_receive_device_remove_list(int handle, G2GUID[] bunch)                       // when remove device
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_remove_list(bunch); });
        }

        public void on_g2admin_receive_device_remove_to_group(int handle, ref G2GUID device, ref G2GUID group) { }

        public void on_g2admin_receive_device_to_group_map(int handle, G2GROUP_MEMBER[] bunch) 
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_device_to_group_map(bunch); });
        }

        public void on_g2admin_receive_empty_device(int handle)      // when no device at group
        {
            Debug.WriteLine("callback [on_g2admin_receive_empty_device] is not implemented.");
        }

        public void on_g2admin_receive_layout_append(int handle, ref G2GUID layout) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_layout_append] is not implemented.");
        }

        public void on_g2admin_receive_layout_list(int handle, G2GUID[] bunch) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_layout_list] is not implemented.");
        }

        public void on_g2admin_receive_layout_modify(int handle, ref G2GUID layout) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_layout_modify] is not implemented.");
        }

        public void on_g2admin_receive_layout_remove(int handle, ref G2GUID layout) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_layout_remove] is not implemented.");
        }

        public void on_g2admin_receive_recording_device_status(int handle, ref G2GUID service, ref G2RECORDING_DEVICE_STATUS status) 
        {
            int size_structure = Marshal.SizeOf(typeof(G2RECORDING_CAMERA_STATUS));
            G2RECORDING_CAMERA_STATUS[] camera_status = new G2RECORDING_CAMERA_STATUS[status._status._len];
            for (int i = 0; i < status._status._len; ++i)
            {
                IntPtr p = new IntPtr((status._status._params.ToInt64() + i * size_structure));
                camera_status[i] = (G2RECORDING_CAMERA_STATUS)Marshal.PtrToStructure(p, typeof(G2RECORDING_CAMERA_STATUS));
            }
            int connection = status._connection;
            int disconnect_reason = status._disconnect_reason;
            G2GUID root = status._root;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_recording_device_status(root, connection, disconnect_reason, camera_status); });
        }

        public void on_g2admin_receive_sequence_append(int handle, ref G2GUID sequence)
        {
            Debug.WriteLine("callback [on_g2admin_receive_sequence_append] is not implemented.");
        }

        public void on_g2admin_receive_sequence_list(int handle, G2GUID[] bunch) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_sequence_list] is not implemented.");
        }

        public void on_g2admin_receive_sequence_modify(int handle, ref G2GUID sequence) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_sequence_modify] is not implemented.");
        }

        public void on_g2admin_receive_sequence_remove(int handle, ref G2GUID sequence) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_sequence_remove] is not implemented.");
        }

        public void on_g2admin_receive_service_log_load(int handle, ref G2SYSTEM_LOG log) 
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form._is_admin)
                    {
                        sub_form.add_system_log(log);
                        sub_form.S_ROW_ID = log._row_id;
                        break;
                    }
                }
            }
        }

        public void on_g2admin_receive_service_log_load_end(int handle) 
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form._is_admin)
                    {
                        sub_form.str[0] = "Complete!";
                        sub_form.log_load_end_update_list();
                        break;
                    }
                }
            }
        }

        public void on_g2admin_receive_service_log_load_fail(int handle) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_service_log_load_fail] is not implemented.");
        }

        public void on_g2admin_receive_service_log_load_stop(int handle) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_service_log_load_stop] is not implemented.");
        }

        public void on_g2admin_receive_debug_log_load(int handle, ref G2DEBUG_LOG log)
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form._is_admin)
                    {
                        sub_form.add_debug_log(log);
                        sub_form.D_ROW_ID = log._row_id;
                        break;
                    }
                }
            }
        }

        public void on_g2admin_receive_debug_log_load_end(int handle)
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                form_log sub_form = _sub_forms[i] as form_log;
                if (sub_form != null)
                {
                    if (sub_form._is_admin)
                    {
                        sub_form.str[1] = "Complete!";
                        sub_form.log_load_end_update_list();
                        break;
                    }
                }
            }
        }

        public void on_g2admin_receive_debug_log_load_fail(int handle) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_debug_log_load_fail] is not implemented.");
        }

        public void on_g2admin_receive_debug_log_load_stop(int handle) 
        {
            Debug.WriteLine("callback [on_g2admin_receive_debug_log_load_fail] is not implemented.");
        }

        public void on_g2admin_modify_device_enable(int handle, ref G2GUID device, bool enable)
        {
            Debug.WriteLine("callback [on_g2admin_receive_debug_log_load_fail] is not implemented.");
        }
        public void on_g2admin_receive_test_failover_end(int handle, bool timeout)
        {
            Debug.WriteLine("callback [on_g2admin_receive_test_failover_end] is not implemented.");
        }

        #endregion
    }
}
