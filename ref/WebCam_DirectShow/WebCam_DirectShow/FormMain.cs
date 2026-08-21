using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;

namespace WebCam_DirectShow
{
    public partial class FormMain : Form
    {
        public enum PlayState { Stopped, Paused, Running, Init };

        private const int WM_GRAPHNOTIFY = 0x8001;

        private IVideoWindow m_videoWindow = null;
        private IMediaControl m_mediaControl = null;
        private IMediaEventEx m_mediaEventEx = null;
        private IGraphBuilder m_graphBuilder = null;
        private ICaptureGraphBuilder2 m_captureGraphBuilder = null;

        private DsROTEntry m_rot = null;
        private PlayState m_currentState = PlayState.Stopped;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CaptureVideo();
        }

        private void CaptureVideo()
        {
            int hr = 0;
            IBaseFilter sourceFilter = null;

            try
            {
                GetInterfaces();

                // Specifies filter graph "graphbuilder" for the capture graph builder "captureGraphBuilder" to use.
                hr = m_captureGraphBuilder.SetFiltergraph(m_graphBuilder);
                System.Diagnostics.Debug.WriteLine("Attach the filter graph to the capture graph : " + DsError.GetErrorText(hr));
                DsError.ThrowExceptionForHR(hr);

                sourceFilter = FindCaptureDevice();

                hr = m_graphBuilder.AddFilter(sourceFilter, "Video Capture");
                System.Diagnostics.Debug.WriteLine("Add capture filter to our graph : " + DsError.GetErrorText(hr));
                DsError.ThrowExceptionForHR(hr);

                hr = m_captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, sourceFilter, null, null);
                System.Diagnostics.Debug.WriteLine("Render the preview pin on the video capture filter : " + DsError.GetErrorText(hr));
                DsError.ThrowExceptionForHR(hr);
               
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sourceFilter);

                SetupVideoWindow();

                m_rot = new DsROTEntry(m_graphBuilder);

                hr = m_mediaControl.Run();
                System.Diagnostics.Debug.WriteLine("Start previewing video data : " + DsError.GetErrorText(hr));
                DsError.ThrowExceptionForHR(hr);
               
                m_currentState = PlayState.Running;
                System.Diagnostics.Debug.WriteLine("The currentstate : " + m_currentState.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unrecoverable error has occurred.With error : " + ex.ToString());
            }
        }

        public void SetupVideoWindow()
        {
            int hr = 0;
            // set the video window to be a child of the main window
            // putowner : Sets the owning parent window for the video playback window. 
            hr = m_videoWindow.put_Owner(this.Handle);
            DsError.ThrowExceptionForHR(hr);

            hr = m_videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
            DsError.ThrowExceptionForHR(hr);

            //Use helper function to position video window in client rect of main application window
            ResizeVideoWindow();

            // Make the video window visible, now that it is properly positioned
            // put_visible : This method changes the visibility of the video window. 
            hr = m_videoWindow.put_Visible(OABool.True);
            DsError.ThrowExceptionForHR(hr);
        }

        public void ResizeVideoWindow()
        {
            // Resize the video preview window to match owner window size
            // left , top , width , height
            if (!(m_videoWindow == null))
            {
                //if the videopreview is not nothing
                m_videoWindow.SetWindowPosition(0, 0, this.Width, this.ClientSize.Height);
            }
        }

        public IBaseFilter FindCaptureDevice()
        {
            System.Diagnostics.Debug.WriteLine("Start the Sub FindCaptureDevice");

            int hr = 0;

            System.Runtime.InteropServices.UCOMIEnumMoniker classEnum = null;
            System.Runtime.InteropServices.UCOMIMoniker[] moniker = new System.Runtime.InteropServices.UCOMIMoniker[1];
            object source = null;
            ICreateDevEnum devEnum = (ICreateDevEnum)(new CreateDevEnum());
            
            hr = devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, out classEnum, CDef.None);
            System.Diagnostics.Debug.WriteLine("Create an enumerator for the video capture devices : " + DsError.GetErrorText(hr));
            DsError.ThrowExceptionForHR(hr);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(devEnum);

            if (classEnum == null)
            {
                throw new ApplicationException("No video capture device was detected.\r\n\r\n" +
                               "This sample requires a video capture device, such as a USB WebCam,\r\n" +
                               "to be installed and working properly.  The sample will now close.");
            }

            int fetched;

            if (classEnum.Next(moniker.Length, moniker, out fetched) == 0)
            {
                Guid iid = typeof(IBaseFilter).GUID;
                moniker[0].BindToObject(null, null, ref iid, out source);
            }
            else
            {
                throw new ApplicationException("Unable to access video capture device!");
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(moniker[0]);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(classEnum);
            return (IBaseFilter)(source);
        }

        private void GetInterfaces()
        {
            int hr = 0;

            m_graphBuilder = (IGraphBuilder)(new FilterGraph());
            m_captureGraphBuilder = (ICaptureGraphBuilder2)(new CaptureGraphBuilder2());
            m_mediaControl = (IMediaControl)m_graphBuilder;
            m_videoWindow = (IVideoWindow)m_graphBuilder;
            m_mediaEventEx = (IMediaEventEx)m_graphBuilder;

            // This method designates a window as the recipient of messages generated by or sent to the current DirectShow object
            hr = m_mediaEventEx.SetNotifyWindow(this.Handle, WM_GRAPHNOTIFY, IntPtr.Zero);
            // ThrowExceptionForHR is a wrapper for Marshal.ThrowExceptionForHR, but additionally provides descriptions for any DirectShow specific error messages.If the hr value is not a fatal error, no exception will be thrown:
            DsError.ThrowExceptionForHR(hr);
            System.Diagnostics.Debug.WriteLine("I started Sub Get interfaces , the result is : " + DsError.GetErrorText(hr));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_GRAPHNOTIFY)
            {
                HandleGraphEvent();
            }

            if (m_videoWindow != null)
            {
                m_videoWindow.NotifyOwnerMessage(m.HWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
            }

            base.WndProc(ref m);
        }

        public void HandleGraphEvent()
        {
            int hr = 0;
            EventCode evCode;
            int evParam1, evParam2;

            if (m_mediaEventEx == null)
                return;
            
            while (m_mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                // Free event parameters to prevent memory leaks associated with
                // event parameter data.  While this application is not interested
                // in the received events, applications should always process them.
                hr = m_mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
                DsError.ThrowExceptionForHR(hr);

                // Insert event processing code here, if desired
            }
        }

        public void CloseInterfaces()
        {
            //stop previewing data
            if (m_mediaControl != null)
                m_mediaControl.StopWhenReady();

            m_currentState = PlayState.Stopped;

            //stop recieving events
            if (m_mediaEventEx != null)
                m_mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);

            // Relinquish ownership (IMPORTANT!) of the video window.
            // Failing to call put_Owner can lead to assert failures within
            // the video renderer, as it still assumes that it has a valid
            // parent window.
            if (m_videoWindow != null)
            {
                m_videoWindow.put_Visible(OABool.False);
                m_videoWindow.put_Owner(IntPtr.Zero);
            }

             // Remove filter graph from the running object table
            if (m_rot != null)
            {
                m_rot.Dispose();
                m_rot = null;
            }

            // Release DirectShow interfaces
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_mediaControl); m_mediaControl = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_mediaEventEx); m_mediaEventEx = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_videoWindow); m_videoWindow = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_graphBuilder); m_graphBuilder = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_captureGraphBuilder); m_captureGraphBuilder = null;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                ChangePreviewState(false);

            if (this.WindowState == FormWindowState.Normal)
                ChangePreviewState(true);

            ResizeVideoWindow();
        }

        private void ChangePreviewState(bool showVideo)
        {
            int hr = 0;
            
            // If the media control interface isn't ready, don't call it
            if (m_mediaControl == null)
            {
                System.Diagnostics.Debug.WriteLine("MediaControl is nothing");
                return;
            }

            if (showVideo)
            {
                if (m_currentState != PlayState.Running)
                {
                    System.Diagnostics.Debug.WriteLine("Start previewing video data");
                    hr = m_mediaControl.Run();
                    m_currentState = PlayState.Running;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Stop previewing video data");
                hr = m_mediaControl.StopWhenReady();
                m_currentState = PlayState.Stopped;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(";alkjsf");
        }
    }
}
