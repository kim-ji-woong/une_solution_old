#include "StdAfx.h"
#include "UEntity.h"

#include "UBMLFactory.h"

namespace UnE
{
	namespace Core
	{
		UEntity::UEntity(void)
			: UObject()
		{
			mTypeName = "UEntity";
			mObjName = "";
			mAnimationState = NULL;
			_mAnimationStateInternal = NULL;
		}


		UEntity::~UEntity(void)
		{

		}

		UAnimationState * UEntity::GetAnimationState(std::string& szName)
		{
			if( mAnimationState != NULL)
			{
				return mAnimationState->getAnimationState(szName);
			}
			return NULL;
		}

		void UEntity::InitAmimationState()
		{
			if( mAnimationState == NULL)
			{
				mAnimationState = new UAnimationStateSet(); 
				if( pInternal != NULL)
				{
					Ogre::Entity * entity = (Ogre::Entity*)pInternal;

					mAnimationState->SetInternal((void*)entity->getAllAnimationStates());
					
					Ogre::AnimationStateSet * pSet = entity->getAllAnimationStates();
					if( pSet == NULL)
					{
						return;
					}
					Ogre::AnimationStateIterator iter = pSet->getAnimationStateIterator();
					for (Ogre::AnimationStateMap::iterator i = iter.begin(); i != iter.end(); ++i)
					{
						Ogre::AnimationState * pState = i->second;
					
						std::string szName = pState->getAnimationName().c_str();						
						UAnimationState * pAni = mAnimationState->createAnimationState(szName, 
							pState->getTimePosition(), 
							pState->getLength(), 
							pState->getWeight(), 
							pState->getEnabled()
						);
						if( pAni != NULL)
						{
							pAni->SetInternal((void*)pState);
						}
					}
				}
			}
		}

		UnE::Core::UAnimationStateSet* UEntity::GetAllAnimationState()
		{
			return mAnimationState;
		}

		void UEntity::UpdateAnimationState()
		{

		}

		void UEntity::SetEnableAnimation( bool bEnable )
		{

		}

		void UEntity::ClearAnimationState()
		{
			if( mAnimationState != NULL)
			{

				delete mAnimationState;
				mAnimationState = NULL;
			}
		}

		//////////////////////////////////////////////////////////////////////////

		Texture::Texture()
		{
			m_nTextureID = -1;
			m_strImagePath = L"";
		}

		Material::Material()
		{
			m_nMaterialID = -1;
			m_strMaterialName = L"";

			for (int i=0;i<4;i++)
			{
				m_arrDiffuseColor[i] = 1.0f;
				m_arrAmbientColor[i] = 1.0f;
				m_arrSpecularColor[i] = 0.0f;
				m_arrEmissiveColor[i] = 0.0f;
			}

			m_nShininess = 0;
			m_pTexture = 0;

			m_useSpecular = m_useEmissive = m_useTexture = false;
		}

		int Material::GetElementTag() const
		{
			if (m_useSpecular)
			{
				if (m_useEmissive)
				{
					if (m_useTexture)
					{
						return UBMLFactory::MATERIAL60_TAG;
					}
					else
					{
						return UBMLFactory::MATERIAL20_TAG;
					}
				}
				else
				{
					if (m_useTexture)
					{
						return UBMLFactory::MATERIAL50_TAG;
					}
					else
					{
						return UBMLFactory::MATERIAL10_TAG;
					}
				}
			}
			else
			{
				if (m_useEmissive)
				{
					if (m_useTexture)
					{
						return UBMLFactory::MATERIAL70_TAG;
					}
					else
					{
						return UBMLFactory::MATERIAL30_TAG;
					}
				}
				else
				{
					if (m_useTexture)
					{
						return UBMLFactory::MATERIAL40_TAG;
					}
					else
					{
						return UBMLFactory::MATERIAL0_TAG;
					}
				}
			}

			return 0;
		}

		Face::Face()
		{
			m_useSmoothShading = true;
			m_useCulling = true;

			m_fTextureScaleX = m_fTextureScaleY = 1.0f;
			m_fOffsetX = m_fOffsetY = 0.0f;
		}

		Mesh::Mesh()
		{
			m_nMeshID = -1;
			m_p3DVertices = 0;
			m_p2DVertices = 0;
		}

		Layer::Layer()
		{
			m_nLayerID = -1;
			m_layerType = UnknownLayer;
			m_strLayerName = "";
			m_strDescription = "";
			m_pMaterial = 0;
			m_pParentLayer = 0;
		}

		//UObject::UObject()
		//{
		//	m_nObjectID = -1;
		//	m_objType = UnknownObject;
		//	m_pLayer = 0;
		//	m_pOwnMaterial = 0;
		//	m_strObjectName = "";
		//}

		

	} // Namespace Core
} // Namespace UnE