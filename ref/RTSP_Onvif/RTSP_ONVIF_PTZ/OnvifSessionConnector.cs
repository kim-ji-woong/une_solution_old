using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using odm.core;
using onvif.services;
using utils;
using System.Diagnostics;
using System.Collections;

using Microsoft.FSharp.Core;


namespace RTSP_ONVIF
{
    class OnvifSessionConnector
    {

        private INvtSession session = null;
        private Profile targetProfile = null;
        private PTZPreset targetPreset = null;
        
        private System.Net.NetworkCredential account = null;

        private string ipAddress = null;
        private string username = null;
        private string password = null;

        private float panSpeed = 0.1f;
        private float TiltSpeed = 0.1f;
        private float zoomSpeed = 0.1f;



        public OnvifSessionConnector(string ipAddress, string username, string password)
        {
            this.ipAddress = ipAddress;
            this.username = username;
            this.password = password;
        }

        public int MakeSession()
        {
            try
            {
                if (string.IsNullOrEmpty(username) || username.ToLower() == "null")
                    return RTSP_ONVIF_RESPONSE.USER_NAME_EMPTY;
                if (string.IsNullOrEmpty(password) || password.ToLower() == "null")
                    return RTSP_ONVIF_RESPONSE.USER_PASS_EMPTY;
                if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "null")
                    return RTSP_ONVIF_RESPONSE.IPADDRESS_EMPTY;

                account = new System.Net.NetworkCredential(username, password);

                string httpAddress = "http://" + ipAddress + "/onvif/device_service";
                NvtSessionFactory factory = new NvtSessionFactory(account);

                session = factory.CreateSession(new Uri(httpAddress));

                Profile[] profileTokens = session.GetProfiles().RunSynchronously();

                if (profileTokens.Length > 0)
                    targetProfile = profileTokens[0];
                else
                {
                    Debug.WriteLine("해당하는 profile이 존재하지 않습니다. IPADDRESS : " + ipAddress);
                    return RTSP_ONVIF_RESPONSE.PROFILE_NOT_FOUND;
                }

                return RTSP_ONVIF_RESPONSE.MAKE_SESSION_SUCCCESS;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
            return RTSP_ONVIF_RESPONSE.UNKNOWN;
        }

        public int GoTargetPreset(string presetname)
        {
            try
            {
                if (targetProfile == null)
                {
                    return RTSP_ONVIF_RESPONSE.PROFILE_NOT_FOUND;
                }

                PTZPreset[] presets = session.GetPresets(targetProfile.token).RunSynchronously();
                targetPreset = null;

                foreach (PTZPreset ptzPreset in presets)
                {
                    Debug.Write(ptzPreset.name + "\t");
                    Debug.WriteLine(ptzPreset.ptzPosition.ToString());
                    Debug.WriteLine(ptzPreset.ToHtmlText());
                    Debug.WriteLine(ptzPreset.token);
                    if (ptzPreset.name.Equals(presetname)) targetPreset = ptzPreset;

                }
                if (targetPreset == null || !targetPreset.name.Equals(presetname)) return RTSP_ONVIF_RESPONSE.PRESET_NOT_FOUND;

                session.GotoPreset(targetProfile.token, targetPreset.token, new PTZSpeed()).RunSynchronously();

                return RTSP_ONVIF_RESPONSE.SET_TARGET_SUCCESS;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
            return RTSP_ONVIF_RESPONSE.UNKNOWN;
        }
        
        public int RelativeMove(int direction)
        {
            try
            {
                PTZVector ptzVector = new PTZVector();
                ptzVector.panTilt = new Vector2D();
                ptzVector.zoom = new Vector1D();
                
                switch (direction)
                {
                    case PTZDirectionCode.UPLEFT:
                        ptzVector.panTilt.x = -panSpeed;
                        ptzVector.panTilt.y = -panSpeed;
                        break;

                    case PTZDirectionCode.UP:
                        ptzVector.panTilt.y = -panSpeed;
                        break;

                    case PTZDirectionCode.UPRIGHT:
                        ptzVector.panTilt.x = panSpeed;
                        ptzVector.panTilt.y = -panSpeed;
                        break;

                    case PTZDirectionCode.LEFT:
                        ptzVector.panTilt.x = -panSpeed;
                        break;

                    case PTZDirectionCode.RIGHT:
                        ptzVector.panTilt.x = panSpeed;;
                        break;

                    case PTZDirectionCode.DOWNLEFT:
                        ptzVector.panTilt.x = -panSpeed;;
                        ptzVector.panTilt.y = panSpeed;;
                        break;

                    case PTZDirectionCode.DOWN:
                        ptzVector.panTilt.y = panSpeed;;
                        break;

                    case PTZDirectionCode.DOWNRIGHT:
                        ptzVector.panTilt.x = panSpeed;;
                        ptzVector.panTilt.y = panSpeed;;
                        break;

                    case PTZDirectionCode.ZOOMIN:
                        ptzVector.zoom.x = zoomSpeed;
                        break;
                    case PTZDirectionCode.ZOOMOUT:
                        ptzVector.zoom.x = -zoomSpeed;
                        break;
                }   
                
                Unit unit = session.RelativeMove(targetProfile.token, ptzVector, new PTZSpeed()).RunSynchronously();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return 0;

        }
             
        public bool GoPreset()
        {
            if (session == null) return false;
            if (targetProfile == null) return false;

            try
            {
                session.GotoPreset(targetProfile.token, targetPreset.token, new PTZSpeed()).RunSynchronously();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return false;
            }

            return true;
        }

        public ArrayList GetPresetList()
        {
            try
            {
                var response = session.GetDeviceInformation().RunSynchronously();
                PTZConfiguration[] ptzConfigurations = session.GetConfigurations().RunSynchronously();
                Profile[] profileTokens = session.GetProfiles().RunSynchronously();

                if (profileTokens.Length > 0)
                    targetProfile = profileTokens[0];
                else
                    return null;

                if (targetProfile == null)
                {
                    Debug.WriteLine("해당하는 profile이 존재하지 않습니다. IPADDRESS : " + ipAddress);
                    return null;
                }

                PTZPreset[] presets = session.GetPresets(targetProfile.token).RunSynchronously();
                ArrayList resultList = new ArrayList();
                foreach (PTZPreset ptzPreset in presets)
                {
                    Debug.Write(ptzPreset.name + "\t");
                    Debug.WriteLine(ptzPreset.ptzPosition.ToString());
                    Debug.WriteLine(ptzPreset.ToHtmlText());
                    Debug.WriteLine(ptzPreset.token);
                    resultList.Add(ptzPreset.name);
                }
                if (resultList.Count > 0)
                    return resultList;
                return null;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
            return null;
        }

    }
}
