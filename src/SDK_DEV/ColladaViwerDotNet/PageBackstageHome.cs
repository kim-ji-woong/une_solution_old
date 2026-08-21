using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using Core;
using System.Diagnostics;

using XtremeDockingPane;
using System.IO;

namespace UBMLViewer
{
	public partial class PageBackstageHome : Form
	{

		private DockingPaneGlobalSettings DockingPaneGlobalSettings;
		
		private Core.Engine mEngine = new Core.Engine();

		private Core.SceneManager mSceneManager = null;
		public Core.SceneManager SceneManager
		{
			get { return mSceneManager; }
		}

		private ArrayList mViewList = new ArrayList();
		private BaseView mCurrent = null;
		public Core.BaseView View3D
		{
			get { return mCurrent; }
		}

		//private string szIconPath = "";
		//private string szMediaPath = "";

		private System.Windows.Forms.Label m_3DView;
	   
		private static PageBackstageHome m_home = null;
		public static PageBackstageHome Instance
		{
			get { return m_home; }
		}

		public AxXtremeDockingPane.AxDockingPane DockingPane
		{
			get { return m_axDockingPane; }
		}

		private DockingLogForm m_LogForm = null;
		public DockingLogForm DockingLogForm
		{
			get { return m_LogForm; }
		}
		private DockingModelTreeForm m_TreeForm = null;
		public DockingModelTreeForm DockingModelTreeForm
		{
			get { return m_TreeForm; }
		}
		private DockingNodePropertiesForm m_PropertiesForm = null;
		public DockingNodePropertiesForm DockingNodePropertiesForm
		{
			get { return m_PropertiesForm; }
		}

		private DockingCmdForm m_CmdForm = null;
		public UBMLViewer.DockingCmdForm DockingCmdForm
		{
			get { return m_CmdForm; }
		}

		public PageBackstageHome()
		{
			m_home = this;          

			InitializeComponent();

			CreateDockingPane();

			Create3DView();

			textureToolStripMenuItem.Checked = true;

			AddPythonFunction();
		}
			   

		private void AddPythonFunction()
		{
			ScriptProxy proxy = ScriptProxy.Instance;
			proxy.UserObject.ClearSelect = new Func<bool>(ClearSelect);
			proxy.UserObject.ViewTextured = new Func<bool>(ViewTextured);
			proxy.UserObject.ViewHidden = new Func<bool>(ViewHiddenLine);
			proxy.UserObject.ViewWire = new Func<bool>(ViewWireframe);
			proxy.UserObject.ViewShading = new Func<bool>(ViewShading);

			proxy.UserObject.ViewFront = new Func<bool>(ViewFront);
			proxy.UserObject.ViewRear = new Func<bool>(ViewRear);
			proxy.UserObject.ViewLeft = new Func<bool>(ViewLeft);
			proxy.UserObject.ViewRight = new Func<bool>(ViewRight);
			proxy.UserObject.ViewTop = new Func<bool>(ViewTop);
			proxy.UserObject.ViewHome = new Func<bool>(ViewHome);

			proxy.UserObject.OpenMesh = new Func<string, bool>(OpenFile);
			proxy.UserObject.ShowScriptWnd = new Func<bool>(ShowScriptPane);
		}

		private void PageBackstageHome_FormClosed(object sender, FormClosedEventArgs e)
		{
			m_3DView.Visible = false;
			mEngine.EngineDispose();
		}
		public static string EnginPath()
		{
			string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
			string szWorkPath = szMainPath;
			if (File.Exists(szWorkPath + "Core.dll"))
				return szWorkPath;

			szWorkPath = szMainPath + "common\\";
			if (File.Exists(szWorkPath + "Core.dll"))
				return szWorkPath;

			szWorkPath = szMainPath + "SOP\\";
			if (File.Exists(szWorkPath + "Core.dll"))
				return szWorkPath;

			return szMainPath;
		}
		private void Create3DView()
		{
			m_3DView = new BaseView();
			m_3DView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			m_3DView.Dock = DockStyle.Fill;
			m_3DView.BackColor = System.Drawing.Color.Transparent;
			m_3DView.Location = new System.Drawing.Point(0, 0);
			m_3DView.Size = new System.Drawing.Size(961, 579);
			m_ContentPane.Controls.Add(this.m_3DView);

			//m_3DView.MouseDown += OnMouseDown;
			//m_3DView.MouseUp += OnMouseUp;
			//m_3DView.MouseMove += OnMouseMove;
			//m_3DView.MouseClick += OnMouseClick;

			mEngine.Init(EnginPath(), "ColladaViewerDotNet");

			mViewList.Add(m_3DView);
			mCurrent = (BaseView)m_3DView;
            
			try
			{
				mCurrent.Popup = m_popupMenu;
				mCurrent.InitBaseView();
			}
			catch (System.Exception ex1)
			{
				Debug.WriteLine(ex1.StackTrace);
			}
			mCurrent.SetCheckPoistion(true);

			mSceneManager = new Core.SceneManager(mCurrent);

			//mCurrent.CreateCompass(0.0f);

            //mCurrent.AddCore();
		}

		//public void OnMouseDown(object sender, MouseEventArgs e)
		//{
		//    ScriptProxy.Instance.Logger.AddInfo("MouseDown(" + e.X + "," + e.Y + ")");
		//}

		//public void OnMouseUp(object sender, MouseEventArgs e)
		//{
		//    ScriptProxy.Instance.Logger.AddInfo("MouseUp(" + e.X + "," + e.Y + ")");
		//}

		//public void OnMouseMove(object sender, MouseEventArgs e)
		//{     
		//}

		//public void OnMouseClick(object sender, MouseEventArgs e)
		//{
		//    ScriptProxy.Instance.Logger.AddInfo("MouseClik(" + e.X + "," + e.Y + ")");			
		//}

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
			if (mCurrent != null)
			{
				mCurrent.OnMouseWheel(e.X, e.Y, e.Delta);
			}
		}

		private Pane m_LogPane = null;
		private Pane m_TreePane = null;
		private Pane m_NodePane = null;
		private Pane m_CmdPane = null;
		private Pane m_ScriptPane = null;

		FormScriptEditor m_ScriptForm;

		private void CreateDockingPane()
		{
			m_LogForm = new DockingLogForm();
			m_TreeForm = new DockingModelTreeForm();
			m_PropertiesForm = new DockingNodePropertiesForm();
			m_CmdForm = new DockingCmdForm();

			m_ScriptForm = new FormScriptEditor();
			
			m_axDockingPane.Options.AlphaDockingContext = true;
			m_axDockingPane.Options.ShowDockingContextStickers = true;
			//m_axDockingPane.Options.ShowCaptionMaximizeButton = true;
						
			m_TreePane = m_axDockingPane.CreatePane(2, 240, 400, DockingDirection.DockRightOf, null);
			m_TreePane.Title = "모델";
			//m_TreePane.Options = PaneOptions.PaneNoCloseable;

			m_LogPane = m_axDockingPane.CreatePane(1, 500, 160, DockingDirection.DockBottomOf, null);
			m_LogPane.Title = "출력";
			//m_LogPane.Options = PaneOptions.PaneNoCloseable;

			m_CmdPane = m_axDockingPane.CreatePane(4, 500, 160, DockingDirection.DockLeftOf, m_LogPane);
			m_CmdPane.Title = "명령";

			m_CmdPane.AttachTo(m_LogPane);
			
			m_NodePane = m_axDockingPane.CreatePane(3, 240, 400, DockingDirection.DockBottomOf, m_TreePane);
			m_NodePane.Title = "속성";
			//m_NodePane.Options = PaneOptions.PaneNoCloseable;

			m_ScriptPane = m_axDockingPane.CreatePane(5, 373, 475, DockingDirection.DockLeftOf, null);
			m_ScriptPane.Title = "스크립트 실행";
			m_ScriptPane.Floating = true;
			m_ScriptPane.Hide();

			m_axDockingPane.Options.ThemedFloatingFrames = true;
			m_axDockingPane.Options.FloatingFrameCaption = "Panes";
		 
		}

		private void DockingPane_ResizeEvent(object sender, EventArgs e)
		{
			int left, top, right, bottom;
			DockingPane.GetClientRect(out left, out top, out right, out bottom);
			m_ContentPane.SetBounds(left, top, right - left, bottom - top);
		}

		private void DockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
		{
			if (e.item.Id == 1)
				e.item.Handle = m_LogForm.Handle.ToInt32();
			if (e.item.Id == 2)
				e.item.Handle = m_TreeForm.Handle.ToInt32();
			if (e.item.Id == 3)
				e.item.Handle = m_PropertiesForm.Handle.ToInt32();
			if (e.item.Id == 4)
				e.item.Handle = m_CmdForm.Handle.ToInt32();
			if (e.item.Id == 5)
				e.item.Handle = m_ScriptForm.Handle.ToInt32();
		}

		private void SetCheckMenuItem()
		{
			textureToolStripMenuItem.Checked = false;
			hiddenLineToolStripMenuItem.Checked = false;
			shadingToolStripMenuItem.Checked = false;
			wireFrameToolStripMenuItem.Checked = false;
		}

		private void selectNodeMenuItem_Click(object sender, EventArgs e)
		{
			string szName = mCurrent.OnSelectNode();
			m_TreeForm.ModelViewSelectNode(szName);
		}

		private void selectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string szName = mCurrent.OnSelect();
		}

		public bool ClearSelect()
		{
			mCurrent.ClearSelect();
			SceneManager.ShowBoundingBoxAll(false);
			m_TreeForm.ClearSelect();
			return true;
		}
		private void clearSelectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearSelect();
		}

		public bool ViewTextured()
		{
			SetCheckMenuItem();
			textureToolStripMenuItem.Checked = true;
			mCurrent.OnViewTextured();
			mCurrent.RedrawScene();
			return true;
		}
		private void textureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ViewTextured();
		}

		public bool ViewHiddenLine()
		{
			SetCheckMenuItem();
			hiddenLineToolStripMenuItem.Checked = true;
			mCurrent.OnViewHiddenline();
			mCurrent.RedrawScene();
			return true;
		}

		private void hiddenLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ViewHiddenLine();
		}

		public bool ViewShading()
		{
			SetCheckMenuItem();
			shadingToolStripMenuItem.Checked = true;
			mCurrent.OnViewPolygon();
			mCurrent.RedrawScene();
			return true;
		}
		private void shadingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ViewShading();
		}

		public bool ViewWireframe()
		{
			SetCheckMenuItem();
			wireFrameToolStripMenuItem.Checked = true;
			mCurrent.OnViewWireframe();
			mCurrent.RedrawScene();
			return true;
		}
		private void wireFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ViewWireframe();
		}		

		public void OnChangeTheme(int nID)
		{
			switch(nID)
			{
				case ID.ID_OPTIONS_STYLEBLACK:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2007.dll", "Office2007Black.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLEBLUE:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile("", "");
						DockingPane.VisualTheme = (VisualTheme)DockingPaneGlobalSettings.ColorManager.SystemTheme;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLEAQUA:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2007.dll", "Office2007Aqua.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLESILVER:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2007.dll", "Office2007Silver.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFCIE2010BLUE:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2010.dll", "Office2010Blue.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFICE2010SILVER:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2010.dll", "Office2010Silver.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFCIE2010BLACK:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();                        
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2010.dll", "Office2010Black.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;                       
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				case ID.ID_OPTIONS_STYLESCENIC:
					{
						DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
						DockingPaneGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Windows7.dll", "Windows7Blue.ini");
						DockingPane.VisualTheme = VisualTheme.ThemeResource;
						DockingPane.RecalcLayout();
						DockingPane.RedrawPanes();
					}
					break;
				default:
					break;
			};

			if (m_PropertiesForm != null)
			{
				m_PropertiesForm.OnChangeTheme(nID);
			}
		}

		public bool IsShowScriptPane()
		{
			if (m_ScriptPane != null && m_ScriptPane.Hidden == false && m_ScriptPane.Closed == false)
			{              
				return true;
			}
			return false;
		}

		public bool ShowScriptPane()
		{
			if (m_ScriptPane != null)
			{
				if (m_ScriptPane.Hidden == true || m_ScriptPane.Closed == true)
				{
					if( m_ScriptPane.Hidden == true && m_ScriptPane.Closed == false)
						m_ScriptPane.Floating = true;
					m_ScriptPane.Hidden = false;
					m_ScriptPane.Closed = false;
					RedrawScene();
					return true;
				}
				else
				{
					m_ScriptPane.Hidden = true;
					m_ScriptPane.Closed = true;
					RedrawScene();
				}
			}
			
			return false;
		}

		public bool OpenFile(string szFileName)
		{            
			if( mCurrent != null )
			{
				mCurrent.OpenMesh(szFileName, false);
				mCurrent.CreateCompass(0.0f);
				mCurrent.OnViewHome();
				
				mCurrent.UpdateWindow();

				DockingModelTreeForm.UpdateModelTree(SceneManager);
 
				return true;
			}             
			return false;
		}

		public bool ViewRear()
		{
			if (mCurrent != null)
			{
				mCurrent.OnViewRear();
				return true;
			}
			return false;           
		}

		public bool ViewFront()
		{
			if (mCurrent != null)
			{
				mCurrent.OnViewFront();
				return true;
			}
			return false;  
		}

		public bool ViewLeft()
		{
			if (mCurrent != null)
			{
				mCurrent.OnViewLeft();
				return true;
			}
			return false;
		}

		public bool ViewRight()
		{
			if (mCurrent != null)
			{
				mCurrent.OnViewRight();
				return true;
			}
			return false;
		}

		public bool ViewTop()
		{
			if (mCurrent != null)
			{
				mCurrent.OnViewTop();
				return true;
			}
			return false;
		}

		public bool ViewHome()
		{
			if (mCurrent != null)
			{
				mCurrent.OnViewHome();
				return true;
			}
			return false;
		}

		public bool RedrawScene()
		{
			if (mCurrent != null)
			{
				mCurrent.RedrawScene();
				return true;
			}
			return false;
		}
	}
}
