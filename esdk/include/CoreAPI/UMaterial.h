#pragma once

namespace UnE
{
	namespace Core
	{


		class UMaterial
		{
			bool m_bLightEnabled;

			bool m_bUseTexture;

			bool m_bUse;

			//UMaterial*
			UMaterial*		m_pHighlightMaterial;
			UMaterial*		m_pWireframeMaterial;
			UMaterial*		m_pHiddenLineMaterial;

		};


		class UMaterialGroup 
		{

		};

		class UTexture
		{

		};

		class UImage
		{

		};

		class UBaseDB;
		class UMaterialManager
		{
			friend class UBaseDB;

		protected:
			bool m_bInit;
			bool m_bResLoadComplete;
			bool DeleteMaterialInternal();


	
		protected:
			UMaterialManager();			
			UMaterialManager(UMaterialManager& rhs){};
			void operator=(UMaterialManager & rhs){ };

		public:
			virtual ~UMaterialManager();
			static UMaterialManager s_cInstance;
			inline static UMaterialManager& Instance ()
			{
				return s_cInstance;
			}

			bool	RemoveMaterial(std::string szMatName);	

			bool	CreateMaterial(std::string szMatName);
			bool	CreateMaterial(std::string szMatName, UImage * pImage);
			
			UMaterial*	GetMaterial(std::string szMatName);

			UImage* GetTextureImage(std::string szMatName);


			bool	Clear();
			
			void LoadDefultResource();

		};

	}
}


