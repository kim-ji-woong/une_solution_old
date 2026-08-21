using System.Diagnostics;

using GDK;

namespace GDK_tester
{
    public partial class form_admin : g2user_listener
    {
        private g2user _user_adaptor;
        private bool _after_login = false;

        public void init_connective_user()
        {
            _user_adaptor = g2user.get();
            _user_adaptor.set_listener(this);
        }

        public void on_post_receive_authority_modify()
        {
            for (int i = 0; i < _sub_forms.Count; ++i)
            {
                if (_sub_forms[i] is form_live)
                {
                    form_live live = (form_live)_sub_forms[i];
                    live.on_screen_changed_pane(live.CUR_PAN);
                    live.check_alarm_auth();
                }
                else if (_sub_forms[i] is form_play)
                {
                    form_play play = (form_play)_sub_forms[i];
                    play.check_clip_auth();
                }
                else if (_sub_forms[i] is form_backup)
                {
                    form_backup backup = (form_backup)_sub_forms[i];
                    backup.check_clip_auth();
                }
            }
            if (_after_login)
            {
                on_post_monitor_connected();
                check_admin_features_auth();
            }
        }

        #region g2user_listener ¸â¹ö

        public void on_g2user_connected()
        {
            Debug.WriteLine("callback [on_g2user_connected] is not implemented.");
        }

        public void on_g2user_disconnect_entered(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            Debug.WriteLine("callback [on_g2user_disconnect_entered] is not implemented.");
        }

        public void on_g2user_login_cancelled()
        {
            Debug.WriteLine("callback [on_g2user_login_cancelled] is not implemented.");
        }

        public void on_g2user_login()
        {
            Debug.WriteLine("callback [on_g2user_login] is not implemented.");
            _after_login = true;
        }

        public void on_g2user_logout(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            Debug.WriteLine("callback [on_g2user_logout] is not implemented.");
        }

        public void on_g2user_login_failed(ref G2NETWORK_INFO ni, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            Debug.WriteLine("callback [on_g2user_login_failed] is not implemented.");
        }

        public void on_g2user_login_failed_from_dvrns(ref G2NETWORK_INFO ni, G2FEN_RESULT.TYPE reason)
        {
            Debug.WriteLine("callback [on_g2user_login_failed_from_dvrns] is not implemented.");
        }

        public void on_g2user_set_authority()
        {
            on_post_receive_authority_modify();
        }

        public void on_g2user_modify()
        {
            Debug.WriteLine("callback [on_g2user_modify] is not implemented.");
        }

        #endregion
    }
}