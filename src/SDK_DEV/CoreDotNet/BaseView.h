// CoreDotNet.h

#pragma once

using namespace System;

namespace Core
{
	ref class LayerManager;

	 enum Component
	{
		None = 0, TreeA , TreeB 
	};

	

	public ref class Engine
	{
	protected:
		int hInstance;
	public:
		void Init(System::String^ szWorkPath, System::String^ szAppName);
		void EngineDispose();
	};

	public ref class BaseView : public System::Windows::Forms::ToolStripContentPanel
	{

	protected:
		bool bOrbit;
		bool bPan;
		bool bCheckPoistion;
		int mHWND;
		int m_nViewMode;

		System::Windows::Forms::ToolStripContentPanel^ mTarget;
		System::Windows::Forms::ContextMenuStrip^ mPopup;

		int mMode;
		bool bMain;

		System::Drawing::Color ^ m_ColorBackBottom;
		System::Drawing::Color ^ m_ColorBackUpper;
		bool m_bEnableGradient;
	public:
				
		property System::Drawing::Color ^ BackBottomColor
		{
			System::Drawing::Color ^ get() { return m_ColorBackBottom; }
			void set(System::Drawing::Color ^ val) { m_ColorBackBottom = val; }
		}
		
		property System::Drawing::Color ^ BackUpperColor
		{
			System::Drawing::Color ^ get() { return m_ColorBackUpper; }
			void set(System::Drawing::Color ^ val) { m_ColorBackUpper = val; }
		}	

		property bool EnableGradient
		{
			bool get() { return m_bEnableGradient; }
			void set(bool bVal) { m_bEnableGradient = bVal;}
		}

		
		
	protected:

		UnE::View::Content::ILayerManager^ m_LayerManager;
		bool m_bComponentMode;
		Core::Component mCompType;

	public:
		BaseView();

		property UnE::View::Content::ILayerManager^ LayerManager
		{
			UnE::View::Content::ILayerManager^ get() { return m_LayerManager; }
			void set(UnE::View::Content::ILayerManager^ val) { m_LayerManager = val; }
		}

		property int WindowHandle
		{
			int get() { return mHWND; }
		}

		property bool ComponentMode
		{
			bool get() { return m_bComponentMode; }
		}

		property int ViewMode
		{
			int get() { return m_nViewMode; }
		}
	
		property System::Windows::Forms::ContextMenuStrip^ Popup
		{
			System::Windows::Forms::ContextMenuStrip^ get() { return mPopup; }
			void set(System::Windows::Forms::ContextMenuStrip^ val) { mPopup = val; }
		}		

		bool SaveScreen(System::String^ path);	

		void SetComponentMode(bool bSet, int compType)
		{
			m_bComponentMode = bSet;
			mCompType = (Core::Component)compType;
		}

		void SetMode(int nMode);

		bool InitBaseView();

		void UpdateWindow();


		void OnMouseWheel( long x, long y, int delta );

		void OpenMesh(System::String^ strPath, bool bDAE);
		void OpenMesh(System::String^ strPath);
		//////////////////////////////////////////////////////////////////////////
		void OnViewFront(bool bUpdateWindow);
		void OnViewFront();

		void OnViewTop(bool bUpdateWindow);
		void OnViewTop();

		void OnViewLeft(bool bUpdateWindow);
		void OnViewLeft();

		void OnViewRight(bool bUpdateWindow);
		void OnViewRight();

		void OnViewFit(bool bUpdateWindow);
		void OnViewFit();

		void OnViewHome(bool bUpdateWindow);
		void OnViewHome();
		
		void OnViewRear(bool bUpdateWindow);
		void OnViewRear();

		virtual void OnViewFix();
		//////////////////////////////////////////////////////////////////////////
		void CreateSceneNodes();


		void CreateFloor(float fwidth, float fheight, float felevation, System::Drawing::Color^ tcolor, System::Drawing::Color^ bcolor, bool bEnableGradient);
		
		void AddCore(float tx, float ty);
		void AddBeams(float tx, float ty);

		System::Drawing::Point^ Get2DPoint(Position3D^ pos);
		
		Core::Position3D^ Get3DPoint(System::Drawing::Point^ pt);

		Core::Position3D^ AddPOI(System::String^ szPath);

		System::String^ OnPickName();
		System::String^ OnSelect();
		void ClearSelect();
		
		Core::Position3D^ OnPosition();

		int AddPOI(System::String^ szPath, float x, float y, float z);

		int AddGroupName(System::String^ groupName, float x, float y, float z );
		
		int AddZoneName(System::String^ groupName, float x, float y, float z );


		int AddAliasName(System::String^ orName, System::String^ alias);
		
		void ShowShelterPath(int nType);
		void HideAllShelter();

		void ShowNames(int nID, bool bShow);
		
		void RemovePOI();
		void RemovePOI(float x, float y, float z);
		void RemovePOI(int nID);

		void RemoveTextPOI(int nID);

		void EnablePOI(int nID, bool bEnable);
		
		bool MovePOI( int nID, float x, float y, float z);
		void ShowIconPOI(int nID, bool bShow);

		void ClearAllData();

		void ChangeViewSize(int width, int height);
	
		void SetCheckPoistion(bool bCheck);

		void AddPointToLine(float x, float y, float z);
		void UpdateLine();

		void RedrawScene();


		void CreateCompass(float fAzumith);
		
		void AddComponent(int x, int y, int compType);

		void ZoomObject(System::String^ szName);
		void ZoomObjectAnimation(System::String^ szName);
		void ZoomTarget(Position3D^ pos, float dist);
		void ZoomTargetAnimation(Position3D^ pos, float dist);
#ifdef SAFE_KOREA_YH_2017
		void CameraMovingAnimationViewAll(Position3D^ targetCampos, Position3D^ targetCamDir, Quaternion3D^ targetCamQuart);
#endif
		void CameraMovingAnimation(Position3D^ targetCampos, Position3D^ targetCamDir, Quaternion3D^ targetCamQuart);

		//void AddFire(int id, float x, float y, float z,System::String^ szName );

		void MouseMoveTo( int x, int y);
		

		void OnSavePt(System::Windows::Forms::MouseEventArgs^ e);

	
		void OnPaint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e);
		void OnSize(System::Object^ sender, System::EventArgs^ e);
		void OnMouseDown(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseUp(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseMove(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseClick(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
	protected:
		virtual void OnPaintBackground(System::Windows::Forms::PaintEventArgs^ pevent) override;
		virtual void Refresh() new;
		virtual void OnResize(System::EventArgs^ e) override;
		virtual void OnPaint(System::Windows::Forms::PaintEventArgs^ e) override;

		virtual void PerformLayout(System::Windows::Forms::Control^ affectedControl, System::String^ affectedProperty);
	private:		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		System::ComponentModel::Container ^components;
		System::Drawing::Brush^ m_Brush;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		void InitializeComponent(void)
		{
			
		}
		
#pragma endregion

	public:
		void OnViewOctree(bool m_bShowOctree);
		void OnViewWireframe();
		void OnViewHiddenline();
		void OnViewPolygon();
		void OnViewTextured();
		System::String^ OnSelectNode();


		int OnSelectPOI(int x, int y);
		void SelectPOI(int nIcon, bool bShow);

		void ClearAllSelectedPOI();

		void SetPickSize(int nIcon, int width, int height);
		bool IsPOISelected(int nIcon);
		int UpdateIcon( int nID, System::String^ szNewPath );
		
		void SetIconPOISize(float width, float height);
		
		void SetTextColor(float red, float green, float blue);
		void SetTextHeight(float fFontHeight);
		void SetTextLODDist(float fDistance);

		void SetTextLOD(bool bLOD);
		void SetTextPOILOD(int nID, bool bToogle, float dist);

		bool IsInCamera(float x, float y, float z);
		float GetPOIDistance(int nPoi);


		Core::Position3D^ GetCameraPosition();
		void SetCameraPosition(Position3D^ pos);

		Core::Position3D^ GetCameraDirection();
		void SetCameraDirection(Position3D^ pos);

		Core::Quaternion3D^ GetCameraOrientaion();
		void SetCameraOrientaion(Core::Quaternion3D^ orient);
		

		int CheckScenePosition(System::String^ szName, int type, float value);

	/*	void SetBackgroudGradient(bool bEnabled);
		void SetBackgroundUpperColor(float red, float green, float blue);
		void SetBackgroundBottomColor(float red, float green, float blue);*/

		// 170928 KYJ
		void EarthquakeMotion(bool earthquake);
	};

	

}

extern int hMainWnd;
