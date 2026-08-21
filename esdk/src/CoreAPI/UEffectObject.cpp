#include "StdAfx.h"
#include "UEffectObject.h"
#include <Ogre.h>
#include <OgreRenderQueue.h>

#include "UDB.h"
#include "UObject.h"
#include "UBaseView.h"

using namespace Ogre;

namespace UnE
{
	namespace Core
	{
		static void MakeBoundingBox(ManualObject* pMo, const AxisAlignedBox& box, const Ogre::ColourValue& boxColor = Ogre::ColourValue(1,1,0), bool bNoDepth = true)
		{
			const Vector3* allcorners = box.getAllCorners();

			String mtrName;
			if(bNoDepth) mtrName = "NoShadowNoLightNoDepth";
			else mtrName = "NoShadowNoLight";
			pMo->begin(mtrName, RenderOperation::OT_LINE_LIST);

			for(int i = 0; i < 8; i++)
			{
				pMo->position(allcorners[i]);
				pMo->colour(boxColor);
			}

			pMo->position((allcorners[1] - allcorners[0]) * 0.2f + allcorners[0]);
			pMo->colour(boxColor);
			pMo->position((allcorners[3] - allcorners[0]) * 0.2f + allcorners[0]);
			pMo->colour(boxColor);
			pMo->position((allcorners[6] - allcorners[0]) * 0.2f + allcorners[0]);
			pMo->colour(boxColor);

			pMo->position((allcorners[0] - allcorners[1]) * 0.2f + allcorners[1]);
			pMo->colour(boxColor);
			pMo->position((allcorners[2] - allcorners[1]) * 0.2f + allcorners[1]);
			pMo->colour(boxColor);
			pMo->position((allcorners[5] - allcorners[1]) * 0.2f + allcorners[1]);
			pMo->colour(boxColor);

			pMo->position((allcorners[1] - allcorners[2]) * 0.2f + allcorners[2]);
			pMo->colour(boxColor);
			pMo->position((allcorners[3] - allcorners[2]) * 0.2f + allcorners[2]);
			pMo->colour(boxColor);
			pMo->position((allcorners[4] - allcorners[2]) * 0.2f + allcorners[2]);
			pMo->colour(boxColor);

			pMo->position((allcorners[0] - allcorners[3]) * 0.2f + allcorners[3]);
			pMo->colour(boxColor);
			pMo->position((allcorners[2] - allcorners[3]) * 0.2f + allcorners[3]);
			pMo->colour(boxColor);
			pMo->position((allcorners[7] - allcorners[3]) * 0.2f + allcorners[3]);
			pMo->colour(boxColor);

			pMo->position((allcorners[2] - allcorners[4]) * 0.2f + allcorners[4]);
			pMo->colour(boxColor);
			pMo->position((allcorners[5] - allcorners[4]) * 0.2f + allcorners[4]);
			pMo->colour(boxColor);
			pMo->position((allcorners[7] - allcorners[4]) * 0.2f + allcorners[4]);
			pMo->colour(boxColor);

			pMo->position((allcorners[1] - allcorners[5]) * 0.2f + allcorners[5]);
			pMo->colour(boxColor);
			pMo->position((allcorners[4] - allcorners[5]) * 0.2f + allcorners[5]);
			pMo->colour(boxColor);
			pMo->position((allcorners[6] - allcorners[5]) * 0.2f + allcorners[5]);
			pMo->colour(boxColor);

			pMo->position((allcorners[0] - allcorners[6]) * 0.2f + allcorners[6]);
			pMo->colour(boxColor);
			pMo->position((allcorners[5] - allcorners[6]) * 0.2f + allcorners[6]);
			pMo->colour(boxColor);
			pMo->position((allcorners[7] - allcorners[6]) * 0.2f + allcorners[6]);
			pMo->colour(boxColor);

			pMo->position((allcorners[3] - allcorners[7]) * 0.2f + allcorners[7]);
			pMo->colour(boxColor);
			pMo->position((allcorners[4] - allcorners[7]) * 0.2f + allcorners[7]);
			pMo->colour(boxColor);
			pMo->position((allcorners[6] - allcorners[7]) * 0.2f + allcorners[7]);
			pMo->colour(boxColor);

			for(int i = 0; i < 8; i++)
			{
				for(int j = 0; j < 3; j++)
				{
					pMo->index(i);
					pMo->index(8 + i*3 + j);
				}
			}

			pMo->end();
			pMo->setRenderQueueGroup(RENDER_QUEUE_SKIES_LATE - 1);
		}


		UEffectObject::UEffectObject(UBaseView* pView, Ogre::SceneNode* pNode, Ogre::Entity* pEntity)
			:UCoreComponent(COMTYPE_EFFECTOBJECT), UCoreObject()
			,mpSceneManager(NULL)
			,mpNode(pNode)
			,mpChildNode(NULL)
			,mpEntity(NULL)
			,mpEffectEntity(NULL)
			,mpEffectMtr(NULL)
			,mbCreated(false)
			,mpBoundingBox(NULL)
			,mbSelected(false)
			,mbVisible(true)
			,mbIsReleased(false)
			,mGlowAlpha(1)
			,mGlowColor(ColourValue::Red)
			,mGlowSize(4.0f)
			,mbVisibleGlow(false)
			,mOriginSize(1.0f)
			,mSizeEffectMaxSize(1.2f)
			,mSizeEffectMinSize(0.8f)
			,mbSizeEffect(false)
			,mbFirstFrame(false)
			,mRunTime(0)
			,mSizeEffectSpeed(5)
			,mVibrationSize(5)
			,mVibrationNegativeSize(5)
			,mVibrationSpeed(50)
			,mVibrationAxis(Vector3::UNIT_X)
			,mbVibration(false)
			,mbCaution(false)
			,mCautionRunTime(0)
			,mbNodeCreate(true)
			,mPlan(0)
			,mHighlihgtColor(ColourValue::Red)
			,mbHighlightColor(false)
			,mbAlphaMode(false)
			,mfAlpha(0.5f)
			,mbCeilingObject(false)
			,mp2DObject(NULL)
			,mbShow(true)
		{
			if( pView == NULL)
			{
				AfxMessageBox(_T("Not Initialized SceneManager!!"));
				throw Exception(Exception::ERR_INVALIDPARAMS, "Not Initialized SceneManager!!", "UEffectObject::UEffectObject");
			}

			m_pView = pView;
			char buf[512];
			sprintf(buf, "%d#SpaceVolumeEffect_%d", (int)(m_pView->GetHWnd()), this->mID);
			mObjName = std::string(buf);

			WndCtx * pCtx = GetWndContext(pView->GetHWnd());
			UObjectManager * pObjManager = UDB::GetObjectManger(m_pView->GetHWnd());
			pObjManager->AddUObject(this);		

			mpSceneManager = pCtx->sceneMgr;

			String name = pNode->getName();
			mID = pNode->getName();
			mName = mID;
			mGlowColor.a = mGlowAlpha;

			mpChildNode = mpNode->createChildSceneNode(mpNode->getName() + "_Child");

			Vector3 vDir = _GetWorldOrientation(*mpNode) * Vector3::UNIT_Z;
			vDir.y = 0;
			vDir.normalise();
			float angle = Ogre::Math::ACos((Vector3::UNIT_Z).dotProduct(vDir)).valueDegrees();
			if(vDir.x > 0)
			{
				angle *= -1;
			}
			mHeading = angle;

			mPosition = mpNode->getPosition();
			mOriginScale = mpNode->getScale();
			
			mpEntity = pEntity;
			mpNode->detachObject(mpEntity->getName());
			mOriginRenderQueGroup = mpEntity->getRenderQueueGroup();

			mpChildNode->attachObject(mpEntity);
			mTopPosition = mPosition + Vector3(0,mpEntity->getBoundingBox().getMaximum().y,0);

			unsigned int subNum = mpEntity->getNumSubEntities();
			for(unsigned int i = 0; i < subNum; i++)
			{
				SubEntity* pSubEt = mpEntity->getSubEntity(i);
				MaterialPtr pMtr = pSubEt->getMaterial();
				MaterialPtr pClonMtr = pMtr->clone(mpEntity->getName() + "_ClonMaterial" + StringConverter::toString(i));
				if(pClonMtr->getTechnique(0)->getPass(0)->getNumTextureUnitStates() > 0)
				{
					pClonMtr->getTechnique(0)->getPass(0)->getTextureUnitState(0)->setColourOperationEx(LBX_ADD_SIGNED);
				}
				mMtrs.push_back(pMtr);
				mClonMtrs.push_back(pClonMtr);
				if(mbCaution)
				{
					pSubEt->setMaterialName(pClonMtr->getName());
				}
				MaterialPtr pHighlightMtr = pMtr->clone(mpEntity->getName() + "_HighlightColorMaterial" + StringConverter::toString(i));
				if(pHighlightMtr->getTechnique(0)->getPass(0)->getNumTextureUnitStates() != 0)
				{
					pHighlightMtr->getTechnique(0)->getPass(0)->getTextureUnitState(0)->setColourOperationEx(LBX_MODULATE, LBS_MANUAL, LBS_TEXTURE, mHighlihgtColor);
				}
				else
				{
					pHighlightMtr->getTechnique(0)->getPass(0)->setDiffuse(mHighlihgtColor);
					pHighlightMtr->getTechnique(0)->getPass(0)->setAmbient(mHighlihgtColor);
				}
				if(mbHighlightColor)
					pSubEt->setMaterialName(pHighlightMtr->getName());
				mHighLightColorMtrs.push_back(pHighlightMtr);
				MaterialPtr pAlphaMtr = pMtr->clone(mpEntity->getName() + "_AlphaModeMaterial" + StringConverter::toString(i));
				pAlphaMtr->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
				pAlphaMtr->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
				ColourValue diffuse = pAlphaMtr->getTechnique(0)->getPass(0)->getDiffuse();
				diffuse.a = mfAlpha;
				pAlphaMtr->getTechnique(0)->getPass(0)->setDiffuse(diffuse);
				if(mbAlphaMode)
					pSubEt->setMaterialName(pAlphaMtr->getName());
				mAlphaMtrs.push_back(pAlphaMtr);
			}

			mpEffectEntity = mpEntity->clone(mpEntity->getName() + "_GlowEntity");
			bool bHasVP = Root::getSingleton().getRenderSystem()->getCapabilities()->hasCapability(Ogre::RSC_VERTEX_PROGRAM);
			bool bHasFP = Root::getSingleton().getRenderSystem()->getCapabilities()->hasCapability(Ogre::RSC_FRAGMENT_PROGRAM);
			if(MaterialManager::getSingleton().resourceExists("HighlightEffect/glow") && bHasVP && bHasFP)
			{
				mpEffectMtr = static_cast<MaterialPtr>(MaterialManager::getSingleton().getByName("HighlightEffect/glow"))->clone(mID + "_EffectMaterial");
				mpEffectEntity->setMaterialName(mpEffectMtr->getName());
				mpEffectMtr->getTechnique(0)->getPass(0)->getVertexProgramParameters()->setNamedConstant("size_value", mGlowSize);
				mpEffectMtr->getTechnique(0)->getPass(0)->getFragmentProgramParameters()->setNamedConstant("color_value", mGlowColor);
				mpEffectEntity->setRenderQueueGroup(RENDER_QUEUE_OUTLINE_GLOW_GLOWS);
				mpChildNode->attachObject(mpEffectEntity);
			}
			mpEffectEntity->setVisible(mbVisibleGlow);

			mpBoundingBox = mpSceneManager->createManualObject(mID + "_EffectObjectBoundingBox");
			MakeBoundingBox(mpBoundingBox, mpEntity->getBoundingBox());
			mpBoundingBox->setCastShadows(false);
			mpBoundingBox->setVisible(mbSelected);
			mpChildNode->attachObject(mpBoundingBox);

			mbCreated = true;
			Root::getSingleton().addFrameListener(this);		
		}

		UEffectObject::UEffectObject(UBaseView* pView, const Ogre::String& id, const Ogre::String& meshFile, const Ogre::Vector3& vPos, const float& heading, const Ogre::String& name)
			:UCoreComponent(COMTYPE_EFFECTOBJECT)
			,mpSceneManager(NULL)
			,mpNode(NULL)
			,mpChildNode(NULL)
			,mpEntity(NULL)
			,mpEffectEntity(NULL)
			,mpEffectMtr(NULL)
			,mbCreated(false)
			,mID(id)
			,mMeshFile(meshFile)
			,mpBoundingBox(NULL)
			,mbSelected(false)
			,mbVisible(true)
			,mPosition(vPos)
			,mHeading(heading)
			,mbIsReleased(false)
			,mGlowAlpha(1)
			,mGlowColor(ColourValue::Red)
			,mGlowSize(4.0f)
			,mbVisibleGlow(false)
			,mOriginSize(1.0f)
			,mSizeEffectMaxSize(1.2f)
			,mSizeEffectMinSize(0.8f)
			,mbSizeEffect(false)
			,mbFirstFrame(false)
			,mRunTime(0)
			,mSizeEffectSpeed(5)
			,mVibrationSize(5)
			,mVibrationNegativeSize(5)
			,mVibrationSpeed(50)
			,mVibrationAxis(Vector3::UNIT_X)
			,mbVibration(false)
			,mbCaution(false)
			,mCautionRunTime(0)
			,mbNodeCreate(false)
			,mPlan(0)
			,mHighlihgtColor(ColourValue::Red)
			,mbHighlightColor(false)
			,mbAlphaMode(false)
			,mfAlpha(0.5f)
			,mbCeilingObject(false)
			,mOriginScale(1.0f, 1.0f, 1.0f)
			,mbShow(true)
		{
			if( pView == NULL)
			{
				AfxMessageBox(_T("Not Initialized SceneManager!!"));
				throw Exception(Exception::ERR_INVALIDPARAMS, "Not Initialized SceneManager!!", "UEffectObject::UEffectObject");
			}
			
			m_pView = pView;
			char buf[512];
			sprintf(buf, "%d#SpaceVolumeEffect_%d", (int)(m_pView->GetHWnd()), this->mID);
			mObjName = std::string(buf);

			WndCtx * pCtx = GetWndContext(pView->GetHWnd());
			UObjectManager * pObjManager = UDB::GetObjectManger(m_pView->GetHWnd());
			pObjManager->AddUObject(this);		

			mpSceneManager = pCtx->sceneMgr;

			if(name != "")
			{
				mName = name;
			}
			else
			{
				mName = mID;
			}
		
			mGlowColor.a = mGlowAlpha;
			_Create();
			mbCreated = true;
			Root::getSingleton().addFrameListener(this);
		}

		UEffectObject::~UEffectObject(void)
		{
			Root::getSingleton().removeFrameListener(this);
			_Destroy();
		}

		ComponentPickResert UEffectObject::Picking(Ray pickRay)
		{
			ComponentPickResert Result;
			std::pair<bool, float> res;
			res = UCoreUtil::UtilPickEntityEx(pickRay, mpEntity);
			Result.bHit = res.first;
			if(res.first)
			{
				Result.cso = CSO_BODY;
				Result.dist = res.second;
			}

			return Result;
		}

		void UEffectObject::SelectComponent(ComponentSubObject cso)
		{
			if(cso == CSO_BODY || cso == CSO_ALL)
			{
				mpBoundingBox->setVisible(true);
				mbSelected = true;
			}
			else
			{
				mpBoundingBox->setVisible(false);
				mbSelected = false;
			}
		}

		void UEffectObject::DeSelect()
		{
			SelectComponent(CSO_NOTHING);
		}

		void UEffectObject::Move(float dist, MoveDirection direction, Ogre::Camera* pCurrViewCam)
		{
			Ogre::Camera* pCam = NULL;
			if(pCurrViewCam)
			{
				pCam = pCurrViewCam;
			}
			else
			{
				WndCtx * pCtx = GetWndContext(m_pView->GetHWnd());
				pCam = pCtx->camera;
			}
			Vector3 vMainCamDir = pCam->getDirection();
			vMainCamDir.y = 0;
			vMainCamDir.normalise();
			Vector3 vMainCamRight = pCam->getRight();
			vMainCamRight.y = 0;
			vMainCamRight.normalise();
			Vector3 vDir = Vector3::ZERO;

			switch(direction)
			{
			case MOVEDIR_LEFT_RIGHT:
				vDir = vMainCamRight * dist;
				break;
			case MOVEDIR_FORWARD_BACKWARD:
				vDir = vMainCamDir * dist;
				break;
			case MOVEDIR_UP_DOWN:
				vDir.y = dist;
				break;
			}

			Move(vDir);
		}

		void UEffectObject::Move(const Vector3& vMove)
		{
			mpNode->translate(vMove, Node::TS_WORLD);
		}

		void UEffectObject::ShowObject(bool bShow)
		{
			mbShow = bShow;
			SetVisible(mbVisible);
		}

		void UEffectObject::SetVisible(bool bVisible, bool bNoChangeValue)
		{
			if(!bNoChangeValue)
				mbVisible = bVisible;
			mpEntity->setVisible(bVisible && mbShow);
			if(mpEffectEntity)
			{
				if(mbVisibleGlow && bVisible)
				{
					mpEffectEntity->setVisible(true && mbShow);
				}
				else if(!bVisible)
				{
					mpEffectEntity->setVisible(false);
				}
			}
			if(mbSelected && bVisible)
			{
				mpBoundingBox->setVisible(true && mbShow);
			}
			else if(!bVisible)
			{
				mpBoundingBox->setVisible(false);
			}
		}

		void UEffectObject::Release()
		{
			if(mbNodeCreate) return;
			if(mbIsReleased) return;
			mPosition = GetPosition();
			_Destroy();
			mbIsReleased = true;
		}

		void UEffectObject::Restore()
		{
			if(mbNodeCreate) return;
			if(!mbIsReleased) return;
			_Create();
			mbIsReleased = false;
		}

		void UEffectObject::RestoreVisible()
		{
			if(mpEffectEntity)
				mpEffectEntity->setVisible(mbVisibleGlow && mpEntity->getVisible());
			if(mpBoundingBox)
				mpBoundingBox->setVisible(mbSelected && mpEntity->getVisible());
		}

		void UEffectObject::SetHeading(float heading)
		{
			if(heading > 360)
			{
				heading = heading - (int(heading/360) * 360);
			}
			else if(heading < 0)
			{
				heading = 360 + (heading - (int(heading/360) * 360));
			}
			mHeading = heading;
			Quaternion quat(Radian(Degree(heading)), -Vector3::UNIT_Y); // heading+180을 하면 기준은 Z축 하지 않으면 -Z축
			mpNode->setOrientation(quat);
		}

		void UEffectObject::SetHeading(Vector3 vDir)
		{
			vDir.y = 0;
			vDir.normalise();
			float angle = Ogre::Math::ACos((Vector3::UNIT_Z).dotProduct(vDir)).valueDegrees();
			if(vDir.x > 0)
			{
				angle *= -1;
			}
			SetHeading(angle);
		}

		void UEffectObject::Rotate(float deltaAngle)
		{
			mHeading += deltaAngle;
			SetHeading(mHeading);
		}

		void UEffectObject::SetPosition(const Vector3& pos)
		{
			mpNode->setPosition(pos);
		}

		Vector3 UEffectObject::GetPosition()
		{
			float size = mpSceneManager->getRootSceneNode()->getScale().x;
			//return mpNode->getWorldPosition() / size;
			return _GetWorldPosition(*mpNode) / size;
		}

		void UEffectObject::SetScale(float xScale, float yScale, float zScale)
		{
			Vector3 newScale = mOriginScale * Vector3(xScale, yScale, zScale);
			mpNode->setScale(newScale);
		}

		void UEffectObject::SetVisibleGlow(bool bVisible)
		{
			mbVisibleGlow = bVisible;
			if(mpEffectEntity)
			{
				if(mbVisible && mbVisibleGlow)
				{
					if(mbAlphaMode)
						SetAlphaMode(false);
					if(mpEntity->getVisible())
						mpEffectEntity->setVisible(true);
					mpEntity->setRenderQueueGroup(RENDER_QUEUE_OUTLINE_GLOW_OBJECTS);
					mpSceneManager->getRenderQueue()->getQueueGroup(RENDER_QUEUE_OUTLINE_GLOW_OBJECTS)->setShadowsEnabled(false);
					//mpSceneManager->getRenderQueue()->getQueueGroup(RENDER_QUEUE_OUTLINE_GLOW_GLOWS)->setShadowsEnabled(false);
				}
				else if(!mbVisibleGlow)
				{
					mpEffectEntity->setVisible(false);
					mpEntity->setRenderQueueGroup(mOriginRenderQueGroup);
				}
			}
		}

		void UEffectObject::SetGlowColor(ColourValue color)
		{
			mGlowColor = color;
			mGlowColor.a = mGlowAlpha;
			if(!mpEffectMtr.isNull())
			{
				mpEffectMtr->getTechnique(0)->getPass(0)->getFragmentProgramParameters()->setNamedConstant("color_value", mGlowColor);
			}
		}

		void UEffectObject::SetGlowAlpha(float alpha)
		{
			mGlowAlpha = alpha;
			mGlowColor.a = mGlowAlpha;
			if(!mpEffectMtr.isNull())
			{
				mpEffectMtr->getTechnique(0)->getPass(0)->getFragmentProgramParameters()->setNamedConstant("color_value", mGlowColor);
			}
		}

		void UEffectObject::SetGlowSize(float size)
		{
			mGlowSize = size;
			if(!mpEffectMtr.isNull())
			{
				mpEffectMtr->getTechnique(0)->getPass(0)->getVertexProgramParameters()->setNamedConstant("size_value", mGlowSize);
			}
		}

		void UEffectObject::OnSizeEffect(bool bOn)
		{
			if(mbSizeEffect != bOn)
			{
				mbSizeEffect = bOn;
				if(bOn)
				{
					mbFirstFrame = true;
					mRunTime = 0;
				}
				else
				{
					mpChildNode->setScale(Vector3(mOriginSize, mOriginSize, mOriginSize));
				}
			}
		}

		void UEffectObject::OnVibration(bool bOn)
		{
			if(mbVibration != bOn)
			{
				mbVibration = bOn;
				if(bOn)
				{
					mbFirstFrame = true;
					mRunTime = 0;
				}
				else
				{
					mpChildNode->setPosition(Vector3::ZERO);
				}
			}
		}

		void UEffectObject::SetHighlightColor(Ogre::ColourValue color)
		{
			if(mHighlihgtColor != color)
			{
				mHighlihgtColor = color;
				for(size_t i = 0; i < mHighLightColorMtrs.size(); i++)
				{
					MaterialPtr pHighlightMtr = mHighLightColorMtrs.at(i);
					if(pHighlightMtr->getTechnique(0)->getPass(0)->getNumTextureUnitStates() != 0)
					{
						pHighlightMtr->getTechnique(0)->getPass(0)->getTextureUnitState(0)->setColourOperationEx(LBX_MODULATE, LBS_MANUAL, LBS_TEXTURE, mHighlihgtColor);
					}
					else
					{
						pHighlightMtr->getTechnique(0)->getPass(0)->setDiffuse(mHighlihgtColor);
						pHighlightMtr->getTechnique(0)->getPass(0)->setAmbient(mHighlihgtColor);
					}
				}
			}
		}

		void UEffectObject::ColorHighlight(bool bOn)
		{
			if(bOn != mbHighlightColor)
			{
				mbHighlightColor = bOn;
				if(bOn && mbCaution)
					Caution(false);
				if(bOn && mbAlphaMode)
					SetAlphaMode(false);
				int count = 0;
				if(bOn)
				{
					unsigned int numSub = mpEntity->getNumSubEntities();
					for(unsigned int j = 0; j < numSub; j++)
					{
						SubEntity* pSubEt = mpEntity->getSubEntity(j);
						pSubEt->setMaterialName(mHighLightColorMtrs.at(count++)->getName());
					}
				}
				else
				{
					unsigned int numSub = mpEntity->getNumSubEntities();
					for(unsigned int j = 0; j < numSub; j++)
					{
						SubEntity* pSubEt = mpEntity->getSubEntity(j);
						pSubEt->setMaterialName(mMtrs.at(count++)->getName());
					}
				}
			}
		}

		void UEffectObject::SetAlphaValue(float fAlpha)
		{
			if(mfAlpha != fAlpha)
			{
				mfAlpha = fAlpha;
				for(size_t i = 0; i < mAlphaMtrs.size(); i++)
				{
					MaterialPtr pAlphaMtr = mAlphaMtrs.at(i);
					ColourValue diffuse = pAlphaMtr->getTechnique(0)->getPass(0)->getDiffuse();
					diffuse.a = mfAlpha;
					pAlphaMtr->getTechnique(0)->getPass(0)->setDiffuse(diffuse);
				}
			}
		}

		void UEffectObject::SetAlphaMode(bool bAlpha)
		{
			if(mbAlphaMode != bAlpha)
			{
				mbAlphaMode = bAlpha;
				if(bAlpha && mbCaution)
					Caution(false);
				if(bAlpha && mbHighlightColor)
					ColorHighlight(false);
				if(bAlpha && mbVisibleGlow)
					SetVisibleGlow(false);
				int count = 0;
				if(mbAlphaMode)
				{
					unsigned int numSub = mpEntity->getNumSubEntities();
					for(unsigned int j = 0; j < numSub; j++)
					{
						SubEntity* pSubEt = mpEntity->getSubEntity(j);
						pSubEt->setMaterialName(mAlphaMtrs.at(count++)->getName());
					}
					mpEntity->setRenderQueueGroup(RENDER_QUEUE_9);
				}
				else
				{
					unsigned int numSub = mpEntity->getNumSubEntities();
					for(unsigned int j = 0; j < numSub; j++)
					{
						SubEntity* pSubEt = mpEntity->getSubEntity(j);
						pSubEt->setMaterialName(mMtrs.at(count++)->getName());
					}
					mpEntity->setRenderQueueGroup(mOriginRenderQueGroup);
				}
			}
		}

		void UEffectObject::Caution(bool bOn)
		{
			if(bOn != mbCaution)
			{
				mbCaution = bOn;
				if(bOn && mbHighlightColor)
					ColorHighlight(false);
				if(bOn && mbAlphaMode)
					SetAlphaMode(false);
				int count = 0;
				if(bOn)
				{
					unsigned int numSub = mpEntity->getNumSubEntities();
					for(unsigned int j = 0; j < numSub; j++)
					{
						SubEntity* pSubEt = mpEntity->getSubEntity(j);
						pSubEt->setMaterialName(mClonMtrs.at(count++)->getName());
					}
					mCautionRunTime = 0;
					mbFirstFrame = true;
				}
				else
				{
					unsigned int numSub = mpEntity->getNumSubEntities();
					for(unsigned int j = 0; j < numSub; j++)
					{
						SubEntity* pSubEt = mpEntity->getSubEntity(j);
						pSubEt->setMaterialName(mMtrs.at(count++)->getName());
					}
				}
			}
		}

		void UEffectObject::SetPlanVisible(int planIdx)
		{
			if(planIdx == -1)
			{
				SetVisible(mbVisible, true);
			}
			else
			{
				if(mPlan == planIdx)
				{
					SetVisible(mbVisible, true);
				}
				else
				{
					SetVisible(false, true);
				}
			}
		}

		void UEffectObject::_Create()
		{
			mpNode = mpSceneManager->getRootSceneNode()->createChildSceneNode(mID + "_EffectObjectNode", mPosition);
			mpChildNode = mpNode->createChildSceneNode(mpNode->getName() + "_Child");
			SetHeading(mHeading);

			mpEntity = mpSceneManager->createEntity(mID + "_EffectObjectEntity", mMeshFile);
			//mpEntity->setNormaliseNormals(true);
			_SetNormaliseNormals(*mpEntity,true);
			//mpEntity->setRenderQueueGroup(RENDER_QUEUE_OUTLINE_GLOW_OBJECTS);
			mpChildNode->attachObject(mpEntity);
			mTopPosition = mPosition + Vector3(0,mpEntity->getBoundingBox().getMaximum().y,0);

			unsigned int subNum = mpEntity->getNumSubEntities();
			for(unsigned int i = 0; i < subNum; i++)
			{
				SubEntity* pSubEt = mpEntity->getSubEntity(i);
				MaterialPtr pMtr = pSubEt->getMaterial();
				MaterialPtr pClonMtr = pMtr->clone(mpEntity->getName() + "_ClonMaterial" + StringConverter::toString(i));
				if(pClonMtr->getTechnique(0)->getPass(0)->getNumTextureUnitStates() > 0)
				{
					pClonMtr->getTechnique(0)->getPass(0)->getTextureUnitState(0)->setColourOperationEx(LBX_ADD_SIGNED);
				}
				mMtrs.push_back(pMtr);
				mClonMtrs.push_back(pClonMtr);
				if(mbCaution)
				{
					pSubEt->setMaterialName(pClonMtr->getName());
				}
				MaterialPtr pHighlightMtr = pMtr->clone(mpEntity->getName() + "_HighlightColorMaterial" + StringConverter::toString(i));
				if(pHighlightMtr->getTechnique(0)->getPass(0)->getNumTextureUnitStates() != 0)
				{
					pHighlightMtr->getTechnique(0)->getPass(0)->getTextureUnitState(0)->setColourOperationEx(LBX_MODULATE, LBS_MANUAL, LBS_TEXTURE, mHighlihgtColor);
				}
				else
				{
					pHighlightMtr->getTechnique(0)->getPass(0)->setDiffuse(mHighlihgtColor);
					pHighlightMtr->getTechnique(0)->getPass(0)->setAmbient(mHighlihgtColor);
				}
				if(mbHighlightColor)
					pSubEt->setMaterialName(pHighlightMtr->getName());
				mHighLightColorMtrs.push_back(pHighlightMtr);
				MaterialPtr pAlphaMtr = pMtr->clone(mpEntity->getName() + "_AlphaModeMaterial" + StringConverter::toString(i));
				pAlphaMtr->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
				pAlphaMtr->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
				ColourValue diffuse = pAlphaMtr->getTechnique(0)->getPass(0)->getDiffuse();
				diffuse.a = mfAlpha;
				pAlphaMtr->getTechnique(0)->getPass(0)->setDiffuse(diffuse);
				if(mbAlphaMode)
					pSubEt->setMaterialName(pAlphaMtr->getName());
				mAlphaMtrs.push_back(pAlphaMtr);
			}

			mpEffectEntity = mpEntity->clone(mpEntity->getName() + "_GlowEntity");
			if(MaterialManager::getSingleton().resourceExists("HighlightEffect/glow"))
			{
				mpEffectMtr = static_cast<MaterialPtr>(MaterialManager::getSingleton().getByName("HighlightEffect/glow"))->clone(mID + "_EffectMaterial");
				mpEffectEntity->setMaterialName(mpEffectMtr->getName());
				mpEffectMtr->getTechnique(0)->getPass(0)->getVertexProgramParameters()->setNamedConstant("size_value", mGlowSize);
				mpEffectMtr->getTechnique(0)->getPass(0)->getFragmentProgramParameters()->setNamedConstant("color_value", mGlowColor);
				mpEffectMtr->setReceiveShadows(false);
				mpEffectEntity->setRenderQueueGroup(RENDER_QUEUE_OUTLINE_GLOW_GLOWS);
				mpChildNode->attachObject(mpEffectEntity);
			}
			mpEffectEntity->setVisible(mbVisibleGlow);
			mpEffectEntity->setCastShadows(false);

			mpBoundingBox = mpSceneManager->createManualObject(mID + "_EffectObjectBoundingBox");
			MakeBoundingBox(mpBoundingBox, mpEntity->getBoundingBox());
			mpBoundingBox->setCastShadows(false);
			mpBoundingBox->setVisible(mbSelected);
			mpChildNode->attachObject(mpBoundingBox);
		}

		void UEffectObject::_Destroy()
		{
			if(mpNode)
			{
				mpNode->removeAllChildren();
				if(!mbNodeCreate)
				{
					mpSceneManager->destroySceneNode(mpNode->getName());
				}
				mpChildNode->detachAllObjects();
				mpSceneManager->destroySceneNode(mpChildNode->getName());
				if(mpEntity)
				{
					mpSceneManager->destroyEntity(mpEntity->getName());
				}
				if(mpEffectEntity)
				{
					mpSceneManager->destroyEntity(mpEffectEntity->getName());
				}
				if(mpBoundingBox)
				{
					mpSceneManager->destroyManualObject(mpBoundingBox->getName());
				}
			}
			if(!mpEffectMtr.isNull())
			{
				MaterialManager::getSingleton().remove(mpEffectMtr->getHandle());
				mpEffectMtr.setNull();
			}

			for(size_t i = 0; i < mClonMtrs.size(); i++)
			{
				MaterialManager::getSingleton().remove(mClonMtrs.at(i)->getHandle());
			}
			mClonMtrs.clear();
			for(size_t i = 0; i < mHighLightColorMtrs.size(); i++)
			{
				MaterialManager::getSingleton().remove(mHighLightColorMtrs.at(i)->getHandle());
			}
			mHighLightColorMtrs.clear();
			for(size_t i = 0; i < mAlphaMtrs.size(); i++)
			{
				MaterialManager::getSingleton().remove(mAlphaMtrs.at(i)->getHandle());
			}
			mAlphaMtrs.clear();
			mMtrs.clear();
		}

		bool UEffectObject::frameStarted(const Ogre::FrameEvent& evt)
		{
			if(mbSizeEffect || mbVibration || mbCaution)
			{
				if(mbFirstFrame)
				{
					mbFirstFrame = false;
				}
				else
				{
					mRunTime += evt.timeSinceLastFrame;
					if(mbSizeEffect)
					{
						float sinvalue = (Ogre::Math::Sin(mRunTime * mSizeEffectSpeed) + 1) * 0.5f;
						float scaleValue = (mSizeEffectMinSize + (mSizeEffectMaxSize - mSizeEffectMinSize) * sinvalue) * mOriginSize;
						mpChildNode->setScale(Vector3(scaleValue, scaleValue, scaleValue));
					}
					if(mbVibration)
					{
						float sinvalue = (Ogre::Math::Sin(mRunTime * mVibrationSpeed) + 1) * 0.5f;
						Vector3 tmpV = mVibrationAxis * ((mVibrationSize + mVibrationNegativeSize) * sinvalue - mVibrationNegativeSize);
						mpChildNode->setPosition(tmpV);
					}
					if(mbCaution)
					{
						mCautionRunTime += evt.timeSinceLastFrame;
						float sinvalue = (Ogre::Math::Sin(mCautionRunTime * 5) + 1) * 0.5f;
						for(size_t i = 0; i < mClonMtrs.size(); i++)
						{
							ColourValue diff = mMtrs.at(i)->getTechnique(0)->getPass(0)->getDiffuse();
							ColourValue ambi = mMtrs.at(i)->getTechnique(0)->getPass(0)->getAmbient();
							ColourValue diff2 = ColourValue::Red - diff;
							ColourValue ambi2 = ColourValue::Black - ambi;
							mClonMtrs.at(i)->getTechnique(0)->getPass(0)->setSelfIllumination(ColourValue(sinvalue, 0, 0));
							mClonMtrs.at(i)->getTechnique(0)->getPass(0)->setDiffuse(diff + (diff2 * sinvalue));
							if(mClonMtrs.at(i)->getTechnique(0)->getPass(0)->getNumTextureUnitStates() > 0)
							{
								mClonMtrs.at(i)->getTechnique(0)->getPass(0)->setAmbient(ColourValue::Black);
								mClonMtrs.at(i)->getTechnique(0)->getPass(0)->setSpecular(ColourValue::Black);
							}
							else
							{
								mClonMtrs.at(i)->getTechnique(0)->getPass(0)->setAmbient(ambi + (ambi2 * sinvalue));
							}
						}
					}
				}
			}
			return true;
		}

		bool UEffectObject::frameEnded(const Ogre::FrameEvent& evt)
		{
			return true;
		}

		AxisAlignedBox UEffectObject::GetBoundingBox()
		{
			return mpEntity->getBoundingBox();
		}

		Entity* UEffectObject::GetEntity()
		{
			return mpEntity;
		}

		void UEffectObject::PrevMove()
		{
			mPrevPos = GetPosition();
		}

		void UEffectObject::PostMove()
		{
			Vector3 vCurrPos = GetPosition();
			// 여기서 mPrevPos를 사용해서 command 객체 생성
		}
	}
}//namespace
