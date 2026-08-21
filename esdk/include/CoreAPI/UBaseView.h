#ifndef VisEngine_h__
#define VisEngine_h__

#pragma once

#include "CoreAPI.h"
#include <list>

#include "UVector3.h"
#include "UCamera.h"


namespace UnE
{
	namespace Core
	{
		class UDB;
		class UBaseModel;
		class MouseOperator;
		class UBaseOperator;

		enum UPolygonMode
		{
			ePM_TEXTURED = 0,
			ePM_SHADED,
			ePM_WIREFRAME,
			ePM_VERTEX,
			ePM_HIDDENLINE,

		};

		struct RenderableContext
		{
			bool selected;
			bool ignoreViewDetail;
			RenderableContext() : selected(false), ignoreViewDetail(false) {}
			friend std::ostream& operator<<(std::ostream& o, const RenderableContext& c)
			{
				o << "RenderableContext {selected: " << c.selected << "}";
				return o;
			}
		};
		
		class CORE_API UBaseView : public UCoreObject
		{
			friend class UDB;
			friend class UBaseModel;
		private:
			HWND					m_hWnd;

			char					m_szEngineName[MAX_PATH+1];

			int						m_nScrWidth;
			int						m_nScrHeight;
			int						m_nPixFormat;
	
			UPolygonMode			m_curMode;
			
			bool					m_bInitInstance;
			bool					m_bInitDisplay;
			bool					m_bChangingRenderSystem;
			bool					m_bRender;
			HKEY					m_hKey;
			
			std::list<UBaseOperator*>   m_OperatorList;
			std::vector<UnE::Core::Camera*> mCameraList;

			float					m_fFontHeight;
			float					m_fIconWidth;
			float					m_fIconHeight;

			float					m_rTextColor;
			float					m_gTextColor;
			float					m_bTextColor;

			   
			bool                    m_bEnableGradient;
			float                   m_rBackUpper;
			float                   m_gBackUpper;
			float                   m_bBackUpper;

			float                   m_rBackBottom;
			float                   m_gBackBottom;
			float                   m_bBackBottom;

			float					m_fTextLODDist;

			bool					m_bLODText;


			UBaseModel * m_pModel;
		


		protected:					
			UBaseView(UBaseView& rhs){};
			void operator=(UBaseView & rhs){ };

		public:
			UBaseView(HWND hWnd);	
			virtual ~UBaseView();			

			static HRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);

		public:

			bool IsInitWindow();			

			HKEY& GetRegistry();

			void AddOperator(UBaseOperator* pOperator);
			void RemoveOperator(UBaseOperator* pOperator);	
		
			//////////////////////////////////////////////////////////////////////////
			// INIT PROCESS			
			bool CreateRenderWindow(int nWidth, int nHeight, std::string title , std::string camName);
			
			inline HWND GetHWnd() const { return m_hWnd; }

			bool CreateSubWindow(HWND hParent, int nWidth, int nHeight, std::string camName );
			
			bool ChangeDisplay( int scrWidth, int scrHeight);

			void ChangeDisplaySize( int nWidth, int nHeightm );

			int  GetViewportWidth();
			int	 GetViewportHeight();

			//////////////////////////////////////////////////////////////////////////
			// Render Scenen
			bool RenderScene();
			
			bool RenderOneFrame();

			bool RenderAllOneFrame();			
		
			//////////////////////////////////////////////////////////////////////////
			// Refresh Window
			bool RefreshWindow();

			//////////////////////////////////////////////////////////////////////////
			// Dispose window
			bool DisposeWindow();

			//////////////////////////////////////////////////////////////////////////
			// Dispose engine
			bool Dispose();

			//////////////////////////////////////////////////////////////////////////
			/*void CreateBox(std::string szName);
			void RemoveCube(std::string szCubeName);
			void SaveCube(std::string szCubeName);
			*/

			//////////////////////////////////////////////////////////////////////////
			// Camera operation
			float					GetCameraPitch();
			//float					GetCameraYaw();

			UnE::Math::Vector3		GetCameraRight();
			UnE::Math::Vector3		GetCameraPosition();
			UnE::Math::Quaternion	GetCameraOrientaion();
			UnE::Math::Vector3		GetCameraDirection();

			void SetCameraPitch(float fPitch);
			void SetCameraYaw(const float fYaw);
			void SetCameraPosition(UnE::Math::Vector3& vCamPos);
			void SetCameraOrientation(UnE::Math::Quaternion& vCamOrient);
			void SetCameraDirection(UnE::Math::Vector3& vCamDir);
			
			void MoveCameraRelative(UnE::Math::Vector3& vCamPos);
			//////////////////////////////////////////////////////////////////////////
			// Camera operation
			UnE::Core::Camera*		CreateCamera();
			
			void CreateCircle();

			void ChangeViewMode(UPolygonMode mode);

			UnE::Core::UPolygonMode GetViewMode() { return m_curMode; }

			void ShowOctree(BOOL bShow);			

			bool SaveScreenShot(std::string path);

			UnE::Math::AxisAlignedBox GetCurrentAABB() const { return mAABB; }
			void SetFixView();
			void SetFitView();
			void SetFrontView();
			void SetTopView();

			void SetLeftView();
			void SetRightView();
			void SetHomeView(float zoomFactor = 0.5f);
			void SetRearView();
			

			void SetTextPOIColor(float fred, float fgreen, float fbule);
			void SetIconPOISize(float nWidth, float nHeight);


			int AddIconPOI(std::string szIconPath);
			int AddIconPOI(std::string szIconPath, float x, float y, float z, float fwidth, float fHeight, bool bVisible = true);
			int AddIconPOI(std::string szIconPath, float x, float y, float z, bool bVisible = true);
			void RemovePOI(int hIcon);
			void RemovePOI();
			void RemovePOI(float x, float y, float z);

			void EnablePOI(int hIcon, bool bEnable);

			void SetPickSize(int hIcon, int width, int height);
			
			void ShowIconPOI(int hIcon, bool bShow);

			void UpdateIcon(int hIcon, std::string szIconPath);
			
			void SelectIconPOI(int hIcon, bool bSelect);
			bool IsIconPOISelected(int hIcon);

			void ClearSelectedPIO();

			bool MoveIconPOI(int hIcon, float x, float y, float z);
			float GetPOIDistance(int hIcon);
			
			int AddTextPOI(std::string szText);
			int AddTextPOI(std::string szText, float x, float y, float z, bool bVisible = true);
			bool MoveTextPOI( int hText, float x, float y, float z );
			void RemoveTextPOI( int nID );

			int AddTextPOI2(std::string szText, float x, float y, float z, bool bVisible = true);
			bool MoveTextPOI2(int hText, float x, float y, float z);
			void RemoveTextPOI2(int nID);
			void ShowTextPOI2(int hText, bool bShow);

			UnE::Core::MouseOperator * GetMouseOperator();

			int ShowObjectName(UObject* pObj, std::string szName);
			int ShowZoneName( float x, float y, float z, std::string szAlias );
			void ShowTextPOI(int hText, bool bShow);


			void ClearViewData();


			float GetFontHeight() const { return m_fFontHeight; }
			void SetFontHeight(float val) { m_fFontHeight = val; }

			bool GetTextLOD() { return m_bLODText;  }
			void SetTextLOD(bool val) { m_bLODText = val; }

			void SetTextLODDist(float val) { m_fTextLODDist = val; }
			void SetTextPOILOD(int nID, bool bToogle, float dist);
			//// temp
			void DrawTempLine(std::vector< UnE::Math::Vector3 >& vecPoints);


			//bool AddFireExtinguisher(int id, float x, float y, float z, std::string szName);
			
			void OnChangeRenderer();

			void StopRendering();
			void ResumeRendering();

			void LoadDefultResource();
			
			
			void EnableGraient(bool bEnabled);

			void SetBackUpperColor(float r, float g, float b);
			void SetBackBottomColor(float r, float g, float b);

			void CreateScenePane(std::string szName, int type, float x, float y, float z, bool bVisible );
			//void CreateSceneNode(std::string szName, int type, float x, float y, float z, bool bVisible);
			void CreateSceneNode( std::string szName, float nWidth, float nLength, float nHeight, float x, float y, float z, bool bVisible );

			void RemoveSceneNode(std::string szSceneName);

			std::string CloneSceneNode(std::string& srcName, float tx, float ty, float tz, bool bVisible = true);
			std::string CloneSceneNode( std::string& srcName, std::string& parentName, float tx, float ty, float tz, bool bVisible = true);

			bool AddComponent(int nCompType, float tx, float ty, float tz);
			bool CreateTree();

			void CreateCompass(float fAzumith);
			//void CreateSubCompass(float fAzumith);
			void CreatePath();
			void ShowPath(int nType);
			void HideAllPath();


			int CheckScenePosition(std::string szName, int type, float value);


			//////////////////////////////////////////////////////////////////////////////////////////////
			void CreateFloor(float x, float y, float z);
			void SetFloorTopColor(float r, float g, float b);
			void SetFloorBottomColor(float r, float g, float b);
			void SetFloorEnableGradient(bool bEnable);

			// 170928 KYJ
			// 오브젝트의 color material 변경
			void SetOriginMaterial();
			void SetTempMaterial(bool earthquake);
			// 지진활동(좌우 흔들림)
			void SetEarthquakeMotion();

		protected:
			bool ReCreateRenderWindow(int nWidth, int nHeight, std::string title , std::string camName);
			
			void CreateBackgroundPane();

			void ResetOperator();	

			UBaseModel * GetBaseModel() { return m_pModel; }
			
			UnE::Math::AxisAlignedBox mAABB;	
			
			void * mpCompass;
			void * mpCompassSub;

			//////////////////////////////////////////////////////////////////////////////////////////////
			bool m_bEnableFloorGradient;
			float m_rFloorTop;
			float m_gFloorTop;
			float m_bFloorTop;
			float m_rFloorBtm;
			float m_gFloorBtm;
			float m_bFloorBtm;
				
		};
	}
}


#endif // VisEngine_h__

