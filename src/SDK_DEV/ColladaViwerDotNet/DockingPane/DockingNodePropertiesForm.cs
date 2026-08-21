using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremePropertyGrid;

namespace UBMLViewer
{
	public partial class DockingNodePropertiesForm : Form
	{
		// Helper class to convert image formats between the native COM-based IPictureDisp 
		// and the managed System.Drawing.Image class
		internal class OCXImageConverter : AxHost
		{

			private OCXImageConverter() : base("") { }

			static public stdole.IPictureDisp ImageToCOMImage(Image image)
			{
				return (stdole.IPictureDisp)GetIPictureDispFromPicture(image);
			}

			static public Image COMImageToImage(stdole.IPictureDisp pictureDisp)
			{
				return GetPictureFromIPicture(pictureDisp);
			}

			static public Icon COMImageToIcon(stdole.IPictureDisp pictureDisp)
			{
				return System.Drawing.Icon.FromHandle(((Bitmap)GetPictureFromIPicture(pictureDisp)).GetHicon());
			}
		}

		private PropertyGridGlobalSettingsClass GridGlobalSettings = new PropertyGridGlobalSettingsClass();

		public AxXtremePropertyGrid.AxPropertyGrid MaterialPropertyGrid
		{
			get { return m_MaterialPropertyGrid; }
		}

		public AxXtremePropertyGrid.AxPropertyGrid ObjectPropertyGrid
		{
			get { return m_ObjectPropertyGrid; }
		}

		public AxXtremePropertyGrid.AxPropertyGrid NodePropertyGrid
		{
			get { return m_NodePropertyGrid; }
		}

		private TreeNode mCurrentNode = null;
		public System.Windows.Forms.TreeNode SelectedNode
		{
			get { return mCurrentNode; }
		}

		private Core.Scene mCurrentScene = null;
		public Core.Scene SelectedScene
		{
			get { return mCurrentScene; }
		}

		public DockingNodePropertiesForm()
		{
			InitializeComponent();            
			
			CreateSceneNodeProperties();

			CreateMaterialProperties();

			CreateObjectProperties();

			AddPythonFunction();
		}

		public void AddPythonFunction()
		{
			ScriptProxy proxy = ScriptProxy.Instance;
			proxy.UserObject.MoveNode = new Func<float, float, float, bool>(MoveNode);
		}

		private void CheckGrid(int nGrid)
		{
			NodePropertyGrid.Visible = false;
			MaterialPropertyGrid.Visible = false;
			ObjectPropertyGrid.Visible = false;

			switch(nGrid)
			{
				case 0:
					NodePropertyGrid.Visible = true;
					break;
				case 1:
					MaterialPropertyGrid.Visible = true;
					break;
				case 2:
					ObjectPropertyGrid.Visible = true;
					break;
			};
		}

		private void SetSceneValue(Core.Scene scene)
		{
			mItemName.Value = scene.AliasName;
			mItemVisible.Value = scene.Visible;
			mItemBoundingBox.Value = scene.ShowBound;
			Core.Position3D pos = mCurrentScene.GetPosition();
			string a = "" + pos.X + "," + pos.Y + "," + pos.Z;
			mItemPosition.Value = a;
		}

		private void SetDefaultSceneValue()
		{
			mItemName.Value = "";
			mItemVisible.Value = true;
			mItemBoundingBox.Value = false;
			mItemPosition.Value = "0,0,0";
		}

		public void SetSceneData(TreeNode node)
		{
			CheckGrid(0);
			if (node != null)
			{               
				Core.Scene scene = (Core.Scene)node.Tag;
				if (scene != null)
				{
					mCurrentNode = node;
					mCurrentScene = scene;
					if (mCategoryScene != null)
					{
						SetSceneValue(scene);
					}
					return;
				}               
			}

			mCurrentNode = null;
			mCurrentScene = null;
			if (mCategoryScene != null)
			{
				SetDefaultSceneValue();
			}            
		}

		private PropertyGridItem mCategoryScene = null;
		private PropertyGridItem mItemVisible = null;
		private PropertyGridItem mItemName = null;
		private PropertyGridItem mItemBoundingBox = null;
		private PropertyGridItem mItemPosition = null;
		
		private void CreateSceneNodeProperties()
		{
			NodePropertyGrid.VisualTheme = XTPPropertyGridVisualTheme.xtpGridThemeResource;
			NodePropertyGrid.ToolTipContext.Style = XTPToolTipStyle.xtpToolTipResource;

			NodePropertyGrid.VariableItemsHeight = true;
			
			mCategoryScene = NodePropertyGrid.AddCategory("Scene Node");
			mCategoryScene.Description = "Scene노드 상태를 설정합니다.";
			mCategoryScene.Id = ID.ID_SCENE_STATE;

			mItemName = mCategoryScene.AddChildItem(PropertyItemType.PropertyItemString, "Name", "");
			mItemName.Description = "Scene노드의 Pretty Name 입니다.";
			mItemName.ReadOnly = true;

			mItemVisible = mCategoryScene.AddChildItem(PropertyItemType.PropertyItemBool, "Visible", true);
			mItemVisible.Description = "Scene노드의 Visible 속성입니다.";

			PropertyGridItem ItemBoundingBox = mCategoryScene.AddChildItem(PropertyItemType.PropertyItemString, "BoundingBox", "");
			ItemBoundingBox.Description = "Scene노드 BoundingBox의 속성입니다.";

			mItemBoundingBox = ItemBoundingBox.AddChildItem(PropertyItemType.PropertyItemBool, "Visible", false);
			mItemBoundingBox.Description = "Scene노드 BoundingBox의 Visible 속성입니다.";


			mItemPosition = ItemBoundingBox.AddChildItem(PropertyItemType.PropertyItemString, "Position", "0,0,0");
			mItemPosition.Description = "Scene노드의 위치 입니다.";

			//PropertyGridItem Item = mItemPosition.AddChildItem(PropertyItemType.PropertyItemString, "dX", "0.0");
			//PropertyGridInplaceSpinButton SpinButton = (PropertyGridInplaceSpinButton)Item.AddSpinButton();
			//SpinButton.Min = -0xFFFF;
			//SpinButton.Max = 0xFFFF; 

			ItemBoundingBox.Expanded = true;

			mCategoryScene.Expanded = true;
			//PropertyGridItem Item = Category.AddChildItem(PropertyItemType.PropertyItemBool, "SaveOnClose", true);
			//PropertyGridItem ItemAppearance = Category.AddChildItem(PropertyItemType.PropertyItemString, "Appearance", "");
			//PropertyGridItem FontAppearance = ItemAppearance.AddChildItem(PropertyItemType.PropertyItemFont, "Window Font", this.Font.ToHfont().ToInt32());
			//FontAppearance.Tooltip = "형상을 설정합니다.";
			//FontAppearance.AddChildItem(PropertyItemType.PropertyItemBool, "Bold", this.Font.Bold);
			//FontAppearance.AddChildItem(PropertyItemType.PropertyItemBool, "Italic", this.Font.Italic);
			//FontAppearance.AddChildItem(PropertyItemType.PropertyItemBool, "Underline", this.Font.Underline);
			//FontAppearance.AddChildItem(PropertyItemType.PropertyItemBool, "Strikethrough", this.Font.Strikeout);
			//FontAppearance.AddChildItem(PropertyItemType.PropertyItemString, "Name", this.Font.Name);
			//FontAppearance.AddChildItem(PropertyItemType.PropertyItemNumber, "Weight", this.Font.Size);
			//PropertyGridItem FormAppearance = ItemAppearance.AddChildItem(PropertyItemType.PropertyItemString, "Form", "");
			//FormAppearance.AddChildItem(PropertyItemType.PropertyItemColor, "BackColor", this.BackColor.ToArgb());
			//FormAppearance.AddChildItem(PropertyItemType.PropertyItemColor, "ForeColor", this.ForeColor.ToArgb());

			//Category.AddChildItem(PropertyItemType.PropertyItemString, "WindowSize", "100; 100");
			//Category.Expanded = true;
			//Item.Selected = true;


			//Category = wndPropertyGrid.AddCategory("Global Settings");
			//Category.AddChildItem(PropertyItemType.PropertyItemString, "Greeting Text", "Welcome to your application!");
			//Category.AddChildItem(PropertyItemType.PropertyItemNumber, "ItemsInMRUList", 4);
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemNumber, "MaxRepeatRate", 10);
			//Item.Description = "The rate in milliseconds that the text will repeat.";
			//Item.Value = (int)Item.Value + 6;
			//Category.AddChildItem(PropertyItemType.PropertyItemColor, "ToolbarColor", 0xAA00FF);
			//Category.Expanded = true;

			//Category = wndPropertyGrid.AddCategory("Version");
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "AppVerion", "1.0");

			//Item.ReadOnly = true;

			//Category.AddChildItem(PropertyItemType.PropertyItemString, "Language", "Russian");
			//Category.Expanded = true;

			//Item.Tag = 500;
			//Item.Value = "2.0.0";

			////Dynamic Options
			//Category = wndPropertyGrid.AddCategory("Dynamic Options");
			//PropertyGridItemBool ItemBool;
			//ItemBool = (PropertyGridItemBool)Category.AddChildItem(PropertyItemType.PropertyItemBool, "Advanced", false);
			//ItemBool.Id = 501;
			//ItemBool.CheckBoxStyle = true;

			//ItemBool = (PropertyGridItemBool)Category.AddChildItem(PropertyItemType.PropertyItemBool, "Option 1", false);
			//ItemBool.Hidden = true;
			//ItemBool.CheckBoxStyle = true;

			//ItemBool = (PropertyGridItemBool)Category.AddChildItem(PropertyItemType.PropertyItemBool, "Option 2", false);
			//ItemBool.Hidden = true;
			//ItemBool.CheckBoxStyle = true;

			//ItemBool = (PropertyGridItemBool)Category.AddChildItem(PropertyItemType.PropertyItemBool, "Option 3", false);
			//ItemBool.Hidden = true;
			//ItemBool.CheckBoxStyle = true;

			//ItemBool = (PropertyGridItemBool)Category.AddChildItem(PropertyItemType.PropertyItemBool, "Option 4", false);
			//ItemBool.Hidden = true;
			//ItemBool.ReadOnly = true;
			//ItemBool.CheckBoxStyle = true;
			//ItemBool.Value = true;

			////Masked items
			//Category = wndPropertyGrid.AddCategory("Masked");

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Phone", "Phone No: (800) 555-1212");
			//Item.SetMask("Phone No: (000) 000-0000", "Phone No: (___) ___-____", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "SSN", "SSN: 123-45-6789");
			//Item.SetMask("SSN: 000-00-0000", "SSN: ___-__-____", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "IP Address", "IP Address: 192.168.100.100");
			//Item.SetMask("IP Address: 000.000.000.000", "IP Address: ___.___.___.___", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Currency", "Currency: $1,250.29");
			//Item.SetMask("Currency: $0,000.00", "Currency: $_,___.__", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Hex", "Hex: 0x0012FCE4");
			//Item.SetMask("Hex: 0xHHHHHHHH", "Hex: 0x________", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Time", "Time: 14:25");
			//Item.SetMask("Time: 00:00", "Time: __:__", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Upper Case", "Upper Case: ABCDEFG");
			//Item.SetMask("Upper Case: >>>>>>>", "Upper Case: _______", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Lower Case", "Lower Case: abcdefg");
			//Item.SetMask("Lower Case: <<<<<<<", "Lower Case: _______", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Alpha", "Alpha: AbCdEfG");
			//Item.SetMask("Alpha: LLLLLLL", "Alpha: _______", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Alpha-Numeric", "Alpha-Numeric: AbC1234");
			//Item.SetMask("Alpha-Numeric: AAAAAAA", "Alpha-Numeric: _______", null);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Password", "Text");
			//Item.PasswordMask = true;


			////Items Type Samples
			//Category = wndPropertyGrid.AddCategory("Items");
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "StringItem", "String");
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemNumber, "NumberItem", 10);
			//Item.Tag = 200;
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemMultilineString, "MultilineString", "1\r\n2");
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemColor, "ColorItem", 0xFF00FF);
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemBool, "BoolItem", true);
			//PropertyGridItemFont ItemFont = (PropertyGridItemFont)Category.AddChildItem(PropertyItemType.PropertyItemFont, "FontItem", this.Font.ToHfont().ToInt32());
			//ItemFont.Color = 0xFF0000;

			//PropertyGridItemDate DateItem;
			//DateItem = (PropertyGridItemDate)Category.AddChildItem(PropertyItemType.PropertyItemDate, "DateItem", "01/31/2009");

			////Sets date format to MM/DD/YYYY
			//DateItem.Format = "%m/%d/%Y";

			////Returns the currently set date formated with the mask MM/DD/YYYY
			//System.Diagnostics.Debug.WriteLine("Currently set date (MM/DD/YYYY): " + DateItem.Value);

			////Returns the currently set date without a format mask
			//System.Diagnostics.Debug.WriteLine("Unformated date (MMDDYYYY): " + DateItem.MaskedText);

			////Changes Day to the 9th
			//DateItem.Day = 9;

			////Changes Month to February
			//DateItem.Month = 2;

			////Changes the Year to 2010
			//DateItem.Year = 2010;

			////Returns the date using the Month, Day, and Year properties
			//System.Diagnostics.Debug.WriteLine("Current Date: " + DateItem.Month + "/" + DateItem.Day + "/" + DateItem.Year);

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemPicture, "PictureItem", "");
			//stdole.IPictureDisp Picture;            

			//PropertyGridItem ItemDirectory = Category.AddChildItem(PropertyItemType.PropertyItemString, "Directory", @"C:\");
			//ItemDirectory.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
			//ItemDirectory.Id = 10;

			//PropertyGridItem ItemEnum;

			////Adds a PropertyItemEnum item with a caption of "Enum" and an initial value of 2
			////This will cause the constraint with a value of 2 to be selected
			//ItemEnum = Category.AddChildItem(PropertyItemType.PropertyItemEnum, "Enum", 2);

			////Adds some constraints along with a Data value
			//ItemEnum.Constraints.Add("Windows 98", 1);
			//ItemEnum.Constraints.Add("Windows 2000", 2);
			//ItemEnum.Constraints.Add("Windows XP", 3);

			////Returns the Data value of the constraint selected
			//System.Diagnostics.Debug.WriteLine("Enum Item Value = " + ItemEnum.Value);

			////Returns the String of the constraint selected
			//System.Diagnostics.Debug.WriteLine("Enum Item String = " + ItemEnum.MaskedText);

			//PropertyGridItem ItemFlags;

			////Adds an items of type EnumFlags with a default value of 1 + 8 = 9 = [Windows 98; Windows 95]
			//ItemFlags = Category.AddChildItem(PropertyItemType.PropertyItemEnumFlags, "Flags", 1 + 8);

			////Adds a constraint that will set all items TRUE or FALSE, note the value of the constraint is the sum
			////of all the other constraints.  This item will also be updated when the values of the other constraints
			////have changed.
			//ItemFlags.Constraints.Add("All Windows", 1 + 2 + 4 + 8 + 16 + 32);

			////Adds some constraint that can have a value of TRUE or FALSE
			//ItemFlags.Constraints.Add("Windows 98", 1);
			//ItemFlags.Constraints.Add("Windows 2000", 2);
			//ItemFlags.Constraints.Add("Windows XP", 4);
			//ItemFlags.Constraints.Add("Windows 95", 8);
			//ItemFlags.Constraints.Add("Windows NT", 16);
			//ItemFlags.Constraints.Add("Windows 2003", 32);

			////This changes the value of the flag item to 21 and the string
			////caption to [Windows 98; Windows XP; Windows NT]
			//ItemFlags.Value = 21;

			////Returns the sum of all true constraints
			//System.Diagnostics.Debug.WriteLine("Flags Item Value = " + ItemFlags.Value);

			////Returns a string containing the text of all true constraints
			//System.Diagnostics.Debug.WriteLine("Flags Item String = " + ItemFlags.MaskedText);

			//PropertyGridItemOption ItemOption;
			//ItemOption = (PropertyGridItemOption)Category.AddChildItem(PropertyItemType.PropertyItemOption, "Options", 1);
			//ItemOption.Constraints.Add("Option 1", 1);
			//ItemOption.Constraints.Add("Option 2", 2);

			//ItemOption = (PropertyGridItemOption)Category.AddChildItem(PropertyItemType.PropertyItemOption, "Options", 1 + 2);
			//ItemOption.CheckBoxStyle = true;
			//ItemOption.Constraints.Add("Option 1", 1);
			//ItemOption.Constraints.Add("Option 2", 2);
			////ItemOption.Constraints.Add "Option 3", 4
			////ItemOption.Constraints.Add "Option 4", 8

			////Metrics Samples
			//wndPropertyGrid.EnableMarkup = true;
			//Category = wndPropertyGrid.AddCategory("<TextBlock VerticalAlignment='Center'>Metrics <Run Foreground='Red'>(New)</Run></TextBlock>");

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Value Color", "Red");
			//Item.ValueMetrics.BackColor = (uint)HexToDecimal("EBEBEB");
			//Item.ValueMetrics.ForeColor = (uint)HexToDecimal("0000FF");

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Caption Color", "Blue");
			//Item.CaptionMetrics.BackColor = (uint)HexToDecimal("EBEBEB");
			//Item.CaptionMetrics.ForeColor = (uint)HexToDecimal("FF0000");

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemEnum, "Images", 0);
			//Item.Constraints.Add("Green", 0);
			//Item.Constraints.Add("Red", 1);
			//Item.Constraints.Add("Yellow", 2);
			//Item.Constraints.Add("Blue", 3);

			//Item.Constraints[1].IconIndex = 0;
			//Item.Constraints[2].IconIndex = 1;
			//Item.Constraints[3].IconIndex = 2;
			//Item.Constraints[4].IconIndex = 3;

			//Item.CaptionMetrics.IconIndex = 4;
			//Item.ValueMetrics.IconIndex = 0;

			//wndPropertyGrid.Icons.MaskColor = (uint)HexToDecimal("FF00FF");
			
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "", "");
			//Item.Caption = "<TextBlock VerticalAlignment='Center'><Underline Foreground='Red'>Markup</Underline> Item</TextBlock>";

			////Inplace Buttons Type Samples
			//Category = wndPropertyGrid.AddCategory("Inplace Buttons");
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemBool, "Combo Button", true);
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Expand Button", "");
			//Item.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "2 Buttons", "");
			//Item.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton | PropertyItemFlags.ItemHasComboButton;
			//Item.Constraints.Add("Windows 98", 1);
			//Item.Constraints.Add("Windows 2000", 2);
			//PropertyGridInplaceButton Button;
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Text Button", "");
			//Button = Item.AddInplaceButton(1);
			//Button.Caption = "Find";
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Image Button", "");
			//Button = Item.AddInplaceButton(1);
			//Button.IconIndex = 10;
			//Button.Tooltip = "Click to set filter";

			//wndPropertyGrid.Icons.MaskColor = (uint)HexToDecimal("C8D0D4");            
			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Menu Button", "");
			//Item.Flags = PropertyItemFlags.ItemHasComboButton | PropertyItemFlags.ItemHasEdit;


			//Item = Category.AddChildItem(PropertyItemType.PropertyItemNumber, "Slider Control", 10);

			//PropertyGridInplaceSliderControl SliderCtrl;
			//SliderCtrl = (PropertyGridInplaceSliderControl)Item.AddSliderControl();
			//SliderCtrl.Max = 200;

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemNumber, "Spin Button", 10);

			//PropertyGridInplaceSpinButton SpinButton;
			//SpinButton = (PropertyGridInplaceSpinButton)Item.AddSpinButton();
			//SpinButton.Max = 50;

			//Item = Category.AddChildItem(PropertyItemType.PropertyItemString, "Hyperlink Button", "");
			//Item.Flags = 0;
			//Button = Item.InplaceButtons.Add(1);
			//Button.Hyperlink = true;
			//Button.ShowAlways = true;
			//Button.Alignment = 0;
			//Button.Caption = "Click Me";
			//Button.Width = 0; // Auto

			////FindItem
			//Item = wndPropertyGrid.FindItem("Version");

			////Constraints Samples
			//Item = wndPropertyGrid.FindItem("Language");
			//Item.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasComboButton;

			//Item.Constraints.Add("Neutral", 0);
			//Item.Constraints.Add("Arabic", 0);
			//Item.Constraints.Add("German", 0);
			//Item.Constraints.Add("Chinese(Taiwan)", 0);
			//Item.Constraints.Add("English (United States)", 0);
			//Item.Constraints.Add("France", 0);
			//Item.Constraints.Add("Russian", 0);

			NodePropertyGrid.ToolTipContext.Style = XtremePropertyGrid.XTPToolTipStyle.xtpToolTipResource;

		}

		private void CreateMaterialProperties()
		{
			MaterialPropertyGrid.VisualTheme = XTPPropertyGridVisualTheme.xtpGridThemeResource;
			MaterialPropertyGrid.ToolTipContext.Style = XTPToolTipStyle.xtpToolTipResource;
		}

		private void CreateObjectProperties()
		{
			ObjectPropertyGrid.VisualTheme = XTPPropertyGridVisualTheme.xtpGridThemeResource;
			ObjectPropertyGrid.ToolTipContext.Style = XTPToolTipStyle.xtpToolTipResource;
		}
		
		private void SetTheme(string stylePath, string iniPath, XTPPropertyGridVisualTheme type)
		{
			GridGlobalSettings = new PropertyGridGlobalSettingsClass();
			GridGlobalSettings.ResourceImages.LoadFromFile(stylePath, iniPath);

			NodePropertyGrid.ToolTipContext.Style = XTPToolTipStyle.xtpToolTipResource;
			NodePropertyGrid.VisualTheme = type;
			NodePropertyGrid.Update();

			MaterialPropertyGrid.ToolTipContext.Style = XTPToolTipStyle.xtpToolTipResource;
			MaterialPropertyGrid.VisualTheme = type;
			MaterialPropertyGrid.Update();

			ObjectPropertyGrid.ToolTipContext.Style = XTPToolTipStyle.xtpToolTipResource;
			ObjectPropertyGrid.VisualTheme = type;
			ObjectPropertyGrid.Update();
		}

		public void OnChangeTheme(int nID)
		{
			switch (nID)
			{
				case ID.ID_OPTIONS_STYLEBLACK:
					{
						string stylePath = FormMain.StylesPath() + "Office2007.dll";
						string iniPath = "Office2007Black.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type);                  
					}
					break;
				case ID.ID_OPTIONS_STYLEBLUE:
					{
						string stylePath = "";
						string iniPath = "";
						XTPPropertyGridVisualTheme type = (XTPPropertyGridVisualTheme)GridGlobalSettings.ColorManager.SystemTheme;
						SetTheme(stylePath, iniPath, type);
					   
					}
					break;
				case ID.ID_OPTIONS_STYLEAQUA:
					{
						string stylePath = FormMain.StylesPath() + "Office2007.dll";
						string iniPath = "Office2007Aqua.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type);
					}
					break;
				case ID.ID_OPTIONS_STYLESILVER:
					{
						string stylePath = FormMain.StylesPath() + "Office2007.dll";
						string iniPath = "Office2007Silver.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type);                        
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFCIE2010BLUE:
					{
						string stylePath = FormMain.StylesPath() + "Office2010.dll";
						string iniPath = "Office2010Blue.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type);  
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFICE2010SILVER:
					{
						string stylePath = FormMain.StylesPath() + "Office2010.dll";
						string iniPath = "Office2010Silver.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type);  
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFCIE2010BLACK:
					{
						string stylePath = FormMain.StylesPath() + "Office2010.dll";
						string iniPath = "Office2010Black.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type);  
					}
					break;
				case ID.ID_OPTIONS_STYLESCENIC:
					{
						string stylePath = FormMain.StylesPath() + "Windows7.dll";
						string iniPath = "Windows7Blue.ini";
						XTPPropertyGridVisualTheme type = XTPPropertyGridVisualTheme.xtpGridThemeResource;
						SetTheme(stylePath, iniPath, type); 
					}
					break;
				default:
					break;
			};
		}

		private void NodePropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
		{
			if (e.item == mItemVisible)
			{
				bool bShow = (bool)e.item.Value;
				if (mCurrentNode != null)
				{
					mCurrentNode.Checked = bShow;
					mCurrentScene.Visible = bShow;
					ScriptProxy.Instance.Call("Update3DView()");
				}                    
			}
			else if (e.item == mItemBoundingBox)
			{
				bool bShow = (bool)e.item.Value;
				if (mCurrentScene != null)
				{
					mCurrentScene.ShowBound = bShow;
					ScriptProxy.Instance.Call("Update3DView()");
				} 
			}
			else if (e.item == mItemPosition)
			{
				string szValue = (string)e.item.Value;
				float x, y, z;
				if (CheckVectorString(szValue,out x,out y,out z) == false)
				{
					if (mCurrentScene != null)
					{
						Core.Position3D pos = mCurrentScene.GetPosition();
						string a = "" + pos.X + "," + pos.Y + "," + pos.Z;
						mItemPosition.Value = a;
					}
				}
				else
				{
					ScriptProxy.Instance.Call("MoveNode(" + x + "," + y + "," + z + ")");					
				}
			}			
		}
		
		private bool CheckVectorString(string szVec, out float x, out float y, out float z )
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;

			string [] v = szVec.Split(',');
			if( v.Length != 3)
				return false;

			if( String.IsNullOrWhiteSpace(v[0]))
				return false;
			if( String.IsNullOrWhiteSpace(v[1]))
				return false;
			if( String.IsNullOrWhiteSpace(v[2]))
				return false;
			
			if (float.TryParse(v[0], out x) == false)
				return false;
			if (float.TryParse(v[1], out y) == false)
				return false;
			if (float.TryParse(v[2], out z) == false)
				return false;

			return true;
		}

		public bool MoveNode(float x, float y, float z)
		{
			if (mCurrentScene != null)
			{
				mCurrentScene.SetPosition(new Core.Position3D(x, y, z));
				FormMain.Update3DView();
				return true;
			}
			return false;
		}		

		private void NodePropertyGrid_AfterEdit(object sender, AxXtremePropertyGrid._DPropertyGridEvents_AfterEditEvent e)
		{
			if (e.item == mItemPosition)
			{               
				string szValue = (string)e.item.Value;
				float x, y, z;
				if (CheckVectorString(szValue, out x, out y, out z) == false)
				{
					if( mCurrentScene != null)
					{
						Core.Position3D pos = mCurrentScene.GetPosition();
						string a = "" + pos.X + "," + pos.Y + "," + pos.Z;
						mItemPosition.Value = a;
					}                    
				}
			}
		}

		private void NodePropertyGrid_MouseDownEvent(object sender, AxXtremePropertyGrid._DPropertyGridEvents_MouseDownEvent e)
		{		   
		}

		private void NodePropertyGrid_CancelEdit(object sender, AxXtremePropertyGrid._DPropertyGridEvents_CancelEditEvent e)
		{
		}		
	}
}
