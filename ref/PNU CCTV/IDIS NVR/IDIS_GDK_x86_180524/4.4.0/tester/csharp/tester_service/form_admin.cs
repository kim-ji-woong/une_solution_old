using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using GDK;

namespace GDK_tester
{
    public partial class form_admin : Form
    {
        public class DisableDeviceNode
        {
            public TreeNode deviceNode;
            public TreeNode parentNode;

            public DisableDeviceNode(TreeNode node)
            {
                deviceNode = node;
                parentNode = node.Parent;
            }
        }

        public form_admin()
        {
            InitializeComponent();

            _adaptor = g2admin.get();
            _adaptor.startup();
            _adaptor.set_listener(this);
            _handle = -1;
            _connectable_services = new List<G2GUID>();
            _checked_camera_node_list = new List<TreeNode>();
            _lockObject = new object();
            _recording_status_timer = new System.Windows.Forms.Timer();
            _sub_forms = new List<Form>();
            _disable_devices = new List<DisableDeviceNode>();
            _failover_test_num = 0;
            init_connective_monitor();
            init_connective_user();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            BTN_CONNECT_FORM.Focus();
            ImageList treeViewImageList = new ImageList();
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.alldevice);
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.device_group);
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.group);
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.device);
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.camera);
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.audio);
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.alarm);            
            treeViewImageList.Images.Add(GDK_tester.Properties.Resources.text);
            TRV_DEVICE_LIST.ImageList = treeViewImageList;
            TRV_DEVICE_LIST.SelectedNode = null;
        }

        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            _finalize = true;

            monitor_cleanup(true);

            for (int i = 0, j = _sub_forms.Count; i < j; ++i)
            {
                _sub_forms[0].Close();
            }

            while (_sub_forms.Count > 0)
            {
                Thread.Sleep(10);
            }

            if (_adaptor.is_disconnectable())
            {
                _adaptor.disconnect();

                while (_handle >= 0)
                {
                    Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
        }

        private void on_btn_disconnect(object sender, EventArgs e)
        {
            monitor_cleanup(false);

            for (int i = 0, j = _sub_forms.Count; i < j; ++i)
            {
                _sub_forms[0].Close();
            }

            while (_sub_forms.Count > 0)
            {
                Thread.Sleep(10);
            }

            if (_handle >= 0)
            {
                _adaptor.disconnect();
            }
        }

        private void on_btn_connect_form(object sender, EventArgs e)
        {
            form_connect_admin form = new form_connect_admin();
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            BTN_CONNECT_FORM.Enabled = false;

            _ni = form.ni;

            if (_ni._address_type == G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
            {
                if (form.FEN_query)
                {
                    if (g2fen.get().is_startup() != true)
                    {
                        g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_service.dat");
                    }

                    if (g2fen.get().is_startup())
                    {
                        LB_INFO.Text = "FEN preparing...";
                        this.Enabled = false;
                        g2fen.get().set_gateway(_ni._extra_server_address, _ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                        g2fen.get().join_service((uint)TimeSpan.FromMinutes(2).TotalMilliseconds, true);
                        this.Enabled = true;
                    }

                    if (g2fen.get().is_activate() != true)
                    {
                        g2main.dvrns_setup(_ni._extra_server_address, _ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                    }
                }

                if (_finalize) return;
            }

            lock (_adaptor)
            {
                LB_INFO.Text = "Connecting...";

                G2CONNECT_RES res;
                bool connect_res = _adaptor.connect(ref _ni, out res);
            }
        }

        private void test_admin_failover_connect()
        {
            BTN_CONNECT_FORM.Enabled = false;

            try
            {
                if (System.IO.File.Exists(_xml_path))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(_xml_path);
                    XmlNode admin = doc.SelectSingleNode("/Administrations/Admin[@Address='" + _ni._address.ToString() + "']");
                    if (admin != null)
                    {
                        G2NETWORK_INFO failover = _ni;
                        XmlNode net_info = admin.SelectSingleNode("Failover/NetInfo[@Number='" + _failover_test_num++.ToString() + "']");
                        failover._address = net_info["Address"].InnerXml;
                        failover.set_port(G2NETWORK_INFO.PORT_TYPE.SERVICE_PORT, ushort.Parse(net_info["Port"].InnerXml));

                        LB_INFO.Text = "Connecting to failover...";

                        G2CONNECT_RES res;
                        G2CONNECT_OPTIONS option = new G2CONNECT_OPTIONS();
                        option._connection_timeout = 1000 * 5;
                        _adaptor.connect_failover(ref _ni, ref failover, ref option, out res);
                    }
                    else
                    {
                        BTN_CONNECT_FORM.Enabled = true;
                    }
                }
                else
                {
                    BTN_CONNECT_FORM.Enabled = true;
                }
            }
            catch (System.Exception)
            {
                BTN_CONNECT_FORM.Enabled = true;
                _failover_test_num = 0;
            }
        }

        public byte[] string_to_mac(string address)
        {
            string[] bytes = address.Split('-');
            byte[] res = new byte[6];

            try
            {
                for (int i = 0; i < 6; ++i)
                {
                    int hex = Convert.ToInt32(bytes[i], 16);
                    res[i] = (byte)hex;
                }
            }
            catch (Exception)
            {
                
            }

            return res;
        }

        private void on_btn_get_guid(object sender, EventArgs e)
        {
            G2GUID guid = G2GUID.INVALID_GUID;
            switch (CB_GET_TYPE.SelectedIndex)
            {
                case 0:
                    guid = _adaptor.get_parent_guid_from_address(TB_IP_ADDRESS_GET_GUID.Text);
                    break;
                case 1:
                    guid = _adaptor.get_parent_guid_from_dvrns_name(TB_IP_ADDRESS_GET_GUID.Text);
                    break;
                case 2:
                    G2MAC_ADDRESS mac;
                    mac._mac = string_to_mac(TB_IP_ADDRESS_GET_GUID.Text);
                    guid = _adaptor.g2_admin_get_parent_guid_from_mac(ref mac);
                    break;
            }
           
            if (guid.valid)
            {
                TB_GETTED_GUID.Enabled = true;
                TB_GETTED_GUID.Text = guid.ToString();
                G2_PRODUCT_INFO pi;
                _adaptor.get_device_product_info(ref guid, out pi);
                TB_GETTED_MODEL.Enabled = true;
                TB_GETTED_MODEL.Text = pi.name._string;
                G2DEVICE_ROOT root;
                _adaptor.get_root_device_info(ref guid, out root);
                TB_GETTED_NAME.Enabled = true;
                TB_GETTED_NAME.Text = root._name._string;
                TB_GETTED_MAC.Enabled = true;
                TB_GETTED_MAC.Text = root._ni._mac.ToString();
                TB_GETTED_IP.Enabled = true;
                TB_GETTED_IP.Text = root._ni._address;
            }
            else
            {
                TB_GETTED_GUID.Enabled = false;
                TB_GETTED_GUID.Text = "it's not managed device by service";                
                TB_GETTED_MODEL.Enabled = false;
                TB_GETTED_MODEL.Text = "";
                TB_GETTED_NAME.Enabled = false;
                TB_GETTED_NAME.Text = "";   
                TB_GETTED_MAC.Enabled = false;
                TB_GETTED_MAC.Text = "";
                TB_GETTED_IP.Enabled = false;
                TB_GETTED_IP.Text = "";
            }
        }

        private void on_btn_live(object sender, EventArgs e)
        {
            Dictionary<G2GUID, List<G2DEVICE_LEAF>> service_camera_dic = new Dictionary<G2GUID, List<G2DEVICE_LEAF>>();

            List<G2GUID> root_guids = new List<G2GUID>();
            List<G2DEVICE_ROOT> root_infos = new List<G2DEVICE_ROOT>();

            for (int i = 0; i < _checked_camera_node_list.Count; ++i)
            {
                G2GUID leaf_guid = (G2GUID)_checked_camera_node_list[i].Tag;
                G2DEVICE_LEAF leaf_info;
                _adaptor.get_leaf_device_info(ref leaf_guid, out leaf_info);
                G2GUID streaming_service_guid = _adaptor.get_streaming_service_guid_from_device(ref leaf_guid);
                if (service_camera_dic.ContainsKey(streaming_service_guid))
                {
                    service_camera_dic[streaming_service_guid].Add(leaf_info);
                }
                else
                {
                    service_camera_dic[streaming_service_guid] = new List<G2DEVICE_LEAF>();
                    service_camera_dic[streaming_service_guid].Add(leaf_info);
                }

                if (root_guids.Contains(leaf_info._guid_root) == false)
                {
                    root_guids.Add(leaf_info._guid_root);
                    G2DEVICE_ROOT root_info;
                    _adaptor.get_root_device_info(ref leaf_guid, out root_info);
                    root_infos.Add(root_info);
                }
            }

            List<G2DEVICE_LEAF> alarm_outs = new List<G2DEVICE_LEAF>();
            for (int j = 0; j < root_guids.Count; ++j)
            {
                G2GUID root = root_guids[j];
                G2GUIDLIST leafs = new G2GUIDLIST();
                _adaptor.get_leaf_guid_from_root_guid(ref root, leafs);
                for (int k = 0; k < leafs.Count; ++k)
                {
                    if (leafs[k].is_device_alarm_out)
                    {
                        G2GUID alarmout = leafs[k];
                        G2DEVICE_LEAF leaf_info;
                        _adaptor.get_leaf_device_info(ref alarmout, out leaf_info);
                        alarm_outs.Add(leaf_info);
                    }
                }
            }

            form_live live_form = new form_live(service_camera_dic, alarm_outs, root_infos, _adaptor, _user_adaptor);
            live_form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.live_form_closed);
            live_form.Show();
            _sub_forms.Add(live_form);
        }

        public void live_form_closed(object sender, EventArgs e)
        {
            _sub_forms.Remove((form_live)sender);
        }

        private void on_btn_play(object sender, EventArgs e)
        {
            List<G2GUID> cameras = new List<G2GUID>();
            List<G2DEVICE_LEAF> camera_infos = new List<G2DEVICE_LEAF>();
            List<G2DEVICE_ROOT> root_infos = new List<G2DEVICE_ROOT>();
            List<G2DEVICE_LEAF> textin_infos = new List<G2DEVICE_LEAF>();
            
            for (int i = 0; i < _checked_camera_node_list.Count; ++i)
            {
                G2GUID camera = (G2GUID)_checked_camera_node_list[i].Tag;
                cameras.Add(camera);
                G2DEVICE_LEAF camera_info;
                _adaptor.get_leaf_device_info(ref camera, out camera_info);
                camera_infos.Add(camera_info);
                G2DEVICE_ROOT root_info;
                _adaptor.get_root_device_info(ref camera, out root_info);
                root_infos.Add(root_info);
                G2GUID_8 textin;
                _adaptor.get_ref_text_in_network_from_camera(ref camera, out textin);
                for (int j = 0; j < 8; ++j)
                {
                    if (textin._guid[j].valid)
                    {
                        G2DEVICE_LEAF textin_info = new G2DEVICE_LEAF();
                        _adaptor.get_leaf_device_info(ref textin._guid[j], out textin_info);
                        if (textin_info._enable)
                        {
                            textin_info._number = (uint)i;    // "i" is cameras index
                            textin_infos.Add(textin_info);
                            break;                            // 1 camera, 1 text-in
                        }
                    }
                }                
            }

            G2GUID service = new G2GUID();
            G2GUID cam = (G2GUID)_checked_camera_node_list[0].Tag;
            service = _adaptor.get_recording_service_guid_from_device(ref cam);

            form_play play_form = new form_play(service, cameras, camera_infos, root_infos, textin_infos, _user_adaptor);
            play_form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.play_form_closed);
            play_form.set_func(on_rec_failover_start, check_exist_rec_failover);
            play_form.Show();
            _sub_forms.Add(play_form);

            foreach (G2GUID g in _connectable_services)
            {
                if (g.is_service_recording_failover)
                {
                    play_form.Tag = g;
                    break;
                }
            }
        }

        public void play_form_closed(object sender, EventArgs e)
        {
            _sub_forms.Remove((form_play)sender);
        }

        public void on_rec_failover_start(G2GUID failover_service, form_play.play_info info)
        {
            form_play rec_failover_form = new form_play(failover_service, info.camera_list,
                info.camera_infos, info.root_infos, info.textin_infos, _user_adaptor);
            rec_failover_form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.play_form_closed);
            rec_failover_form.Text = "GDK Service R/e/c/o/r/d/i/n/g/ f/a/i/l/o/v/e/r";
            rec_failover_form.failover_btn_invisible();
            rec_failover_form.Show();            
            _sub_forms.Add(rec_failover_form);
        }

        public bool check_exist_rec_failover()
        {
            bool exist = false;
            foreach (G2GUID service in _connectable_services)
            {
                if (service.is_service_recording_failover)
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        private void on_btn_backup(object sender, EventArgs e)
        {
            List<G2GUID> cameras = new List<G2GUID>();
            List<G2DEVICE_LEAF> camera_infos = new List<G2DEVICE_LEAF>();
            List<G2DEVICE_ROOT> root_infos = new List<G2DEVICE_ROOT>();
            List<G2DEVICE_LEAF> textin_infos = new List<G2DEVICE_LEAF>();

            for (int i = 0; i < _checked_camera_node_list.Count; ++i)
            {
                G2GUID camera = (G2GUID)_checked_camera_node_list[i].Tag;
                cameras.Add(camera);
                G2DEVICE_LEAF camera_info;
                _adaptor.get_leaf_device_info(ref camera, out camera_info);
                camera_infos.Add(camera_info);
                G2DEVICE_ROOT root_info;
                _adaptor.get_root_device_info(ref camera, out root_info);
                root_infos.Add(root_info);
                G2GUID_8 textin;
                _adaptor.get_ref_text_in_network_from_camera(ref camera, out textin);
                for (int j = 0; j < 8; ++j)
                {
                    if (textin._guid[j].valid)
                    {
                        G2DEVICE_LEAF textin_info = new G2DEVICE_LEAF();
                        _adaptor.get_leaf_device_info(ref textin._guid[j], out textin_info);
                        if (textin_info._enable)
                        {
                            textin_info._number = (uint)i;    // "i" is cameras index
                            textin_infos.Add(textin_info);
                            break;                            // 1 camera, 1 text-in
                        }
                    }
                }
            }

            G2GUID cam = (G2GUID)_checked_camera_node_list[0].Tag;
            G2GUIDLIST backup_service = new G2GUIDLIST();
            _adaptor.get_backup_service_guid_from_device(ref cam, backup_service);

            G2GUID site = get_only_one_site_registered_in_backup();

            form_play.play_info backup_info = new form_play.play_info(cameras, camera_infos, root_infos, textin_infos);
            form_backup backup_form = new form_backup(backup_service[0], backup_info, _user_adaptor, site);
            backup_form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.backup_form_closed);
            backup_form.Show();
            _sub_forms.Add(backup_form);
        }

        public void backup_form_closed(object sender, EventArgs e)
        {
            _sub_forms.Remove((form_backup)sender);
        }

        private void on_check_device_list_item(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                BTN_LIVE.Enabled = false;
                BTN_PLAY.Enabled = false;
                BTN_BACKUP.Enabled = false;

                checkbox_status_update_with_relations(e.Node);

                checking_possible_sevices();

                LB_INFO.Text = "Checked Camera count : " + _checked_camera_node_list.Count.ToString();

                if (LB_SERVICES.SelectedItem != null)
                {
                    G2GUID selected_guid = ((ListBoxItem)LB_SERVICES.SelectedItem).guid;
                    if (selected_guid.is_service_backup)
                    {
                        selected_connectable_service(LB_SERVICES, new EventArgs());
                    }
                }
            }
        }

        private void on_device_list_click(object sender, EventArgs e)
        {
            if (e as MouseEventArgs != null)
            {
                if (((MouseEventArgs)e).Button == MouseButtons.Right)
                {
                    try
                    {
                        G2GUID guid = (G2GUID)TRV_DEVICE_LIST.SelectedNode.Tag;
                        if (guid.is_device_leaf)
                        {
                            G2DEVICE_LEAF linfo = new G2DEVICE_LEAF();
                            g2admin.get().get_leaf_device_info(ref guid, out linfo);
                            string ss = "GUID : " + linfo._guid + "\n";
                            ss += "Name : " + linfo._name + "\n";
                            ss += "Number : " + linfo._number + "\n";
                            MessageBox.Show(ss);
                        }
                        else if (guid.is_device_root)
                        {
                            G2DEVICE_ROOT rinfo = new G2DEVICE_ROOT();
                            g2admin.get().get_root_device_info(ref guid, out rinfo);
                            string ss = "GUID : " + rinfo._guid + "\n";
                            ss += "Name : " + rinfo._name + "\n";
                            MessageBox.Show(ss);
                        }
                    }
                    catch
                    { }
                }
            }
        }

        private void checking_possible_sevices()
        {
            bool live = true;
            bool play = true;
            bool backup = true;

            List<G2GUID> recordings = new List<G2GUID>();

            int camera_count = _checked_camera_node_list.Count;
            for (int i = 0; i < camera_count; ++i)
            {
                G2GUID camera_guid = (G2GUID)_checked_camera_node_list[i].Tag;
                G2GUID streaming_service_guid = _adaptor.get_streaming_service_guid_from_device(ref camera_guid);
                if (!_connectable_services.Contains(streaming_service_guid) ||
                    !_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_LIVE))
                {
                    live = false;
                }
                G2GUID record_service_guid = _adaptor.get_recording_service_guid_from_device(ref camera_guid);
                if (!_connectable_services.Contains(record_service_guid) ||
                    !_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_SEARCH))
                {
                    play = false;
                }
                else
                {
                    if (!recordings.Contains(record_service_guid)) recordings.Add(record_service_guid);
                }
            }

            if (play)
            {
                if (recordings.Count > 1) play = false;
            }

            G2GUID site = get_only_one_site_registered_in_backup();
            if (!(site.valid && _user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_SEARCH))) backup = false;

            if (camera_count > 0)
            {
                BTN_LIVE.Enabled = live;
                BTN_PLAY.Enabled = play;
                BTN_BACKUP.Enabled = backup;
            }
        }

        private void checkbox_status_update_with_relations(TreeNode selected_node)
        {
            lock (TRV_DEVICE_LIST)
            {
                List<TreeNode> all_nodes = get_leafs(selected_node);

                int count = all_nodes.Count;
                if (selected_node.Checked)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        all_nodes[i].Checked = true;
                        if (((G2GUID)all_nodes[i].Tag).is_device_camera || ((G2GUID)all_nodes[i].Tag).is_device_hybrid_camera)
                        {
                            if (!_checked_camera_node_list.Contains(all_nodes[i])) _checked_camera_node_list.Add(all_nodes[i]);
                        }
                    }
                    if (selected_node.Tag != null && (((G2GUID)selected_node.Tag).is_device_camera || ((G2GUID)selected_node.Tag).is_device_hybrid_camera))
                    {
                        if (!_checked_camera_node_list.Contains(selected_node)) _checked_camera_node_list.Add(selected_node);
                    }
                }
                else
                {
                    if (selected_node.Tag != null) all_nodes.AddRange(get_parents(selected_node));
                    count = all_nodes.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        all_nodes[i].Checked = false;
                        if (((G2GUID)all_nodes[i].Tag).is_device_camera || ((G2GUID)all_nodes[i].Tag).is_device_hybrid_camera)
                        {
                            _checked_camera_node_list.Remove(all_nodes[i]);
                        }
                    }
                    if (selected_node.Tag != null && (((G2GUID)selected_node.Tag).is_device_camera || ((G2GUID)selected_node.Tag).is_device_hybrid_camera))
                    {
                        _checked_camera_node_list.Remove(selected_node);
                    }
                }
            }
        }

        private List<TreeNode> get_parents(TreeNode leaf_node)
        {
            List<TreeNode> parents = new List<TreeNode>();

            if (leaf_node.Parent.Name != "AllDevice" && leaf_node.Parent.Name != "DeviceGroup")
            {
                parents.Add(leaf_node.Parent);
                parents.AddRange(get_parents(leaf_node.Parent));
            }

            return parents;
        }

        private List<TreeNode> get_leafs(TreeNode parent_node)
        {
            List<TreeNode> leafs = new List<TreeNode>();

            int leafs_count = parent_node.GetNodeCount(false);
            for (int i = 0; i < leafs_count; ++i)
            {
                G2GUID leaf_guid = (G2GUID)parent_node.Nodes[i].Tag;
                if (leaf_guid.is_device_group || leaf_guid.is_device_parent || leaf_guid.is_device_root)
                {
                    leafs.Add(parent_node.Nodes[i]);
                    leafs.AddRange(get_leafs(parent_node.Nodes[i]));
                }
                else
                {
                    leafs.Add(parent_node.Nodes[i]);
                }
            }

            return leafs;
        }

        private void selected_get_guid_type(object sender, EventArgs e)
        {
            if (BTN_GET_GUID.Enabled == false)
            {
                BTN_GET_GUID.Enabled = true;
            }
        }

        private void selected_connectable_service(object sender, EventArgs e)
        {
            bool enable = true;
            LB_TODO_LOG.Text = "";

            if (((ListBox)sender).SelectedItem == null) return;

            G2GUID selected_guid = ((ListBoxItem)((ListBox)sender).SelectedItem).guid;

            if (selected_guid.is_service_monitoring||
                selected_guid.is_service_recording ||
                selected_guid.is_service_streaming ||
                selected_guid.is_service_backup ||
                selected_guid.is_service_recording_failover)
            {
                for (int i = 0; i < _sub_forms.Count; ++i)
                {
                    form_log sub_form = _sub_forms[i] as form_log;
                    if (sub_form != null)
                    {
                        if (sub_form.GUID == selected_guid)
                        {
                            enable = false;
                            LB_TODO_LOG.Text = STRING.get(STRING.NIS_ALREADY_SHOW_LOG);
                            break;
                        }
                    }
                }
            }            
            else
            {
                enable = false;
            }

            BTN_SERVICE_LOG.Enabled = enable;
        }

        private G2GUID get_only_one_site_registered_in_backup()
        {
            G2GUID site = new G2GUID();

            for (int i = 0; i < _checked_camera_node_list.Count; ++i)
            {
                G2GUID camera_guid = (G2GUID)_checked_camera_node_list[i].Tag;
                 List<G2SERVICE_ITEM> sites = new List<G2SERVICE_ITEM>();
                _adaptor.get_backup_service_site_guid_from_device(ref camera_guid, false, sites);
                if (sites.Count != 1 || (site.valid && site != sites[0]._site) || !_connectable_services.Contains(sites[0]._service))
                {
                    site = new G2GUID();
                    break;
                }
                site = sites[0]._site;
            }
            return site;
        }

        private void on_btn_admin_log(object sender, EventArgs e)
        {
            BTN_ADMIN_LOG.Enabled = false;

            G2GUID guid = G2GUID.INVALID_GUID;
            form_log log_form = new form_log(ref guid, true);
            log_form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.admin_log_closed);
            log_form.Show();
            _sub_forms.Add(log_form);
        }

        public void admin_log_closed(object sender, EventArgs e)
        {
            if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_LOG_VIEW))
            {
                BTN_ADMIN_LOG.Enabled = true;
            }

            _sub_forms.Remove((form_log)sender);
        }

        private void on_btn_service_log(object sender, EventArgs e)
        {
            G2GUID selected_guid = ((ListBoxItem)(LB_SERVICES.SelectedItem)).guid;
            form_log log_form = new form_log(ref selected_guid);
            log_form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.service_log_closed);
            log_form.Show();
            _sub_forms.Add(log_form);
            ((Button)sender).Enabled = false;
            LB_TODO_LOG.Text = STRING.get(STRING.NIS_ALREADY_SHOW_LOG);
        }

        public void service_log_closed(object sender, EventArgs e)
        {
            G2GUID sub_form_guid = ((form_log)sender).GUID;
            G2GUID selected_guid = ((ListBoxItem)(LB_SERVICES.SelectedItem)).guid;
            if (sub_form_guid == selected_guid &&
                _user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_LOG_VIEW))
            {
                BTN_SERVICE_LOG.Enabled = true;
                LB_TODO_LOG.Text = "";
            }

            _sub_forms.Remove((form_log)sender);
        }

        public void check_admin_features_auth()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { check_admin_features_auth(); });
                return;
            }

            if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_LOG_VIEW))
            {
                BTN_ADMIN_LOG.Enabled = true;
                selected_connectable_service(LB_SERVICES, null);
            }
            else
            {
                BTN_ADMIN_LOG.Enabled = false;
                BTN_SERVICE_LOG.Enabled = false;
            }

            if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_SHOW_DEVICE_AND_GROUP))
            {
                TRV_DEVICE_LIST.Nodes[0].Expand();
                TRV_DEVICE_LIST.Nodes[1].Expand();
                TRV_DEVICE_LIST.Enabled = true;
            }
            else
            {
                TRV_DEVICE_LIST.Nodes[0].Collapse();
                TRV_DEVICE_LIST.Nodes[1].Collapse();
                TRV_DEVICE_LIST.Enabled = false;
            }
        }

        private void on_btn_violet(object sender, EventArgs e)
        {
            form_violet form = new form_violet();
            form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.violet_form_closed);
            form.Show();
            _sub_forms.Add(form);
        }

        public void violet_form_closed(object sender, EventArgs e)
        {
            form_violet form = sender as form_violet;
            if (form != null)
            {
                form.cleanup();
            }
            _sub_forms.Remove((form_violet)sender);
        }
    }
}