#pragma once

#include <string>
#include <vector>
#include "CoreAPI.h"
#include "UObject.h"
#include "UAnimation.h"


namespace UnE
{
	namespace Core
	{

		

		class Vertices;

		class CORE_API UEntity : public UObject
		{
		protected:
			UAnimationStateSet * mAnimationState;
			void *			  _mAnimationStateInternal;


			void	ClearAnimationState();
		public:
			UEntity(void);
			virtual ~UEntity(void);


			void			  InitAmimationState();	
			void			  UpdateAnimationState();
			UAnimationState * GetAnimationState(std::string& szName);

			UAnimationStateSet* GetAllAnimationState();

			void     SetEnableAnimation(bool bEnable);
		};

		


		class CORE_API Texture
		{
		public:
			Texture();

		public:
			int m_nTextureID;
			std::wstring m_strImagePath;
		};




		class CORE_API Material
		{
		public:
			Material();

		public:
			int GetElementTag() const;

		public:
			int m_nMaterialID;
			std::wstring m_strMaterialName;
			float m_arrDiffuseColor[4];	// rgba
			float m_arrAmbientColor[4];
			float m_arrSpecularColor[4];
			float m_arrEmissiveColor[4];
			int m_nShininess;
			Texture* m_pTexture;

			bool m_useSpecular;
			bool m_useEmissive;
			bool m_useTexture;
		};
		

		class CORE_API Face
		{
		public:
			Face();

		public:
			int v1, v2, v3;
			float v1u, v1v, v2u, v2v, v3u, v3v;
			bool m_useSmoothShading;
			bool m_useCulling;

			float m_fTextureScaleX, m_fTextureScaleY;
			float m_fOffsetX, m_fOffsetY;
		};

		class CORE_API Mesh
		{
		public:
			Mesh();

		public:
			int m_nMeshID;
			Vertices* m_p3DVertices;
			Vertices* m_p2DVertices;
			std::vector<Face> m_vec3DFace;
			std::vector<Face> m_vec2DFace;
		};

		class CORE_API Layer
		{
		public:
			enum LayerType {UnknownLayer = 0, FloorLayer, ObjectLayer};

		public:
			Layer();

		public:
			int m_nLayerID;
			LayerType m_layerType;
			std::string m_strLayerName;
			std::string m_strDescription;
			Material* m_pMaterial;
			Layer* m_pParentLayer;
			std::vector<Layer*> m_vecChildLayer;
		};
		
	}
}




