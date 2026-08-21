#pragma once

#include "CoreAPI.h"
#include "UBaseView.h"


#include <list>

namespace UnE
{
	namespace Core
	{		
		enum URenderer
		{
			eRS_OPENGL = 0,
			eRS_DIRECT9,
			eRS_DIRECT11,
			eRS_NULL,
		};

		class CORE_API UBaseDriver
		{
		public:

			static UBaseDriver s_cInstance;
			inline static UBaseDriver& Instance ()
			{
				return s_cInstance;
			}

			virtual ~UBaseDriver(void);

			bool InitDriver(std::string szEngineWorkDir, std::string szAppName);

			void SetDisplayMode( int nWidth, int nHeightm, int nPixelFormat );
			
			void SetClient(HINSTANCE& gInstance);

			void SetRegistry(HKEY& hRoot);

			void SetRenderer(URenderer nType);
			void CreateRenderSystem( URenderer nType = eRS_OPENGL);
			void ChangeRenderer(URenderer nType);

			void DisposeDriver();

			void ClearDriver();

			void NotityDispose();

			void Add(UnE::Core::UBaseView * pView);

			void Remove(UnE::Core::UBaseView * pView);
			
			bool IsInitDriver() const { return m_bInitDriver; }

			HINSTANCE& AppInstance() { return m_hInstance; }

			HKEY& GetRegistry() { return m_hRootKey; }

			//////////////////////////////////////////////////////////////////////////
			void SaveRendererType(URenderer eRendererType);
			
			int GetRendererType();

			//////////////////////////////////////////////////////////////////////////
			int GetPixFormat() const { return m_nPixFormat; }
			int GetWidth() const { return m_nScrWidth; }
			int GetHeight() const { return m_nScrHeight; }

			//////////////////////////////////////////////////////////////////////////
			

			void RenderAllView();


			std::string GetAppName() const { return m_szAppName; }
			std::string GetEngineWorkDir() const { return m_szEngineWorkDir; }


		protected:
			UBaseDriver();
			UBaseDriver(UBaseDriver& rhs){};
			void operator=(UBaseDriver & rhs){ };

			void OnChangeRenderer();	

			void GetWorkDir(std::string& strAppPath);			


		protected:

			URenderer m_nRenderer;

			HKEY m_hRootKey;

			HINSTANCE m_hInstance;
			
			bool m_bInitRegistry;
			bool m_bInitClient;
			bool m_bInitDriver;
			
			std::list< UBaseView * > m_ChildView;		

			int m_nPixFormat;
			int m_nScrWidth;
			int m_nScrHeight;
			
			std::string m_szEngineWorkDir;
			
			std::string m_szAppName;
			
		};

	}
}

