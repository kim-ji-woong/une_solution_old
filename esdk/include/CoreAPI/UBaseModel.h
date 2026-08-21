#ifndef UModel_h__
#define UModel_h__

#pragma once

#include "CoreAPI.h"
#include "UAxisAlignedBox.h"

namespace UnE 
{
	namespace Core
	{
		enum UFileInputResult
		{
			eIF_RESULT_OK				= 1,  
			eIF_RESULT_VERSION_MISMATCH	= 2, 
			eIF_RESULT_FAIL				= 3, 
			eIF_RESULT_NOT_HANDLED		= 4, 
			eIF_RESULT_BAD_FILENAME		= 5
		};

		enum UFileOutputResult
		{
			eOF_RESULT_OK				= 1,  
			eOF_RESULT_VERSION_MISMATCH	= 2, 
			eOF_RESULT_FAIL				= 3, 
			eOF_RESULT_NOT_HANDLED		= 4, 
			eOF_RESULT_BAD_FILENAME		= 5

		};

		class UDB;
		class UCoreObject;
		class UBaseView;
		class UModelInfo;
		class UEventManager;
		class USceneNodeManager;
		class UShellVertexData;
		class UAssimpLoader;
		
		class CORE_API UModel
		{
		public:
			UModel(void);
			virtual ~UModel(void);
		};
			

		class CORE_API UBaseModel
		{

		public:

			UBaseModel(HWND hWnd);

			virtual ~UBaseModel();

			virtual void Init();

			virtual void Flush();

			bool GetFileLoadComplete() { return m_bFileLoadComplete; };

			void SetFileLoadComplete(bool value) { m_bFileLoadComplete = value; };

			bool GetFirstFitComplete() { return m_bFirstFitComplete; };

			void SetFirstFitComplete(bool value) { m_bFirstFitComplete = value; };

			virtual UFileInputResult ReadScene(const std::string& szFileName);

			virtual UFileInputResult ReadDAE(const std::string& szFileName);


			virtual UFileOutputResult Write(const char * FileName);

			UModelInfo * GetHModelInfo();

			virtual std::string& GetModelName();

			virtual bool ComputeData(int data_cycles = 30);


			virtual UShellVertexData * GetShellVertexData(void);


			virtual int GetShellVertexDataCount(void);

			virtual int GetDataCycles(void);

			UEventManager* GetEventManager();

			void Update(bool forceUpdate = false);

			void SetBaseView(UBaseView * pView);

			UBaseView * GetBaseView() const { return m_pView; }

			USceneNodeManager * GetSecneManager() { return m_pSecneManager; }

			void CreateSceneManager();
			

		protected:

			UBaseView * m_pView;

			static unsigned long m_ModelCount;		/*!< Integer denoting the number of created model objects */

			std::string mModelName;		

			std::string mFilePath;
										
			USceneNodeManager * m_pSecneManager;
			
			UnE::Math::AxisAlignedBox     mBoundBox;
			
			UModelInfo * m_pModelInfo;

			bool m_bFileLoadComplete; 

			bool m_bFirstFitComplete; 
			
			UShellVertexData * m_pShellVertexData; 		
			int			m_ShellVertexDataCount;	
			int			m_DataCycles;		

			UAssimpLoader * m_pAssimpLoader;

			UEventManager * m_pEventManager;

			HWND m_hWnd;
		};
		
		extern CORE_API UBaseModel* GetActiveDB();
		extern CORE_API void SetActiveDB(UBaseModel* db);
	}
}

#endif // UModel_h__
