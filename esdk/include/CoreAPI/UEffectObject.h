#pragma once

#include "CoreAPI.h"
#include "UObject.h"
#include "UCoreUtil.h"

namespace UnE
{
	namespace Core
	{

		class UBaseView;
		class UCoreObject;


		class CORE_API UEffectObject : public UCoreComponent, UCoreObject
		{
		public:
			UEffectObject(UBaseView * pView, Ogre::SceneNode* pNode, Ogre::Entity* pEntity);		
			UEffectObject(UBaseView * pView, const Ogre::String& id, const Ogre::String& meshFile, const Ogre::Vector3& vPos, const float& heading, const Ogre::String& name = "");
			~UEffectObject(void);

			virtual ComponentPickResert Picking(Ogre::Ray pickRay);
			virtual void SelectComponent(ComponentSubObject cso);
			virtual void DeSelect();
			virtual Ogre::SceneNode* GetBodyNode(){return mpNode;}
			virtual void Move(float dist, MoveDirection direction, Ogre::Camera* pCurrViewCam = NULL);
			virtual void Move(const Ogre::Vector3& vMove);
			virtual void SetVisible(bool bVisible, bool bNoChangeValue = false);
			virtual bool GetVisible(){return mbVisible;}
			virtual void Release();
			virtual void Restore();
			void RestoreVisible();

			bool IsCreated(){return mbCreated;}
			Ogre::String GetID(){return mID;}
			void SetName(Ogre::String name){mName = name;}
			Ogre::String GetName(){return mName;}

			void SetHeading(float heading);
			void SetHeading(Ogre::Vector3 vDir);
			void Rotate(float deltaAngle);
			void SetPosition(const Ogre::Vector3& pos);
			Ogre::Vector3 GetPosition();
			Ogre::Vector3 GetTopPosition(){return mTopPosition;}
			void SetScale(float xScale, float yScale, float zScale);

			void SetVisibleGlow(bool bVisible);
			void SetGlowColor(Ogre::ColourValue color);
			void SetGlowAlpha(float alpha);
			void SetGlowSize(float size);

			void OnSizeEffect(bool bOn);
			void SetSizeEffectMaxSize(float maxSize){mSizeEffectMaxSize = maxSize;}
			void SetSizeEffectMinSize(float minSize){mSizeEffectMinSize = minSize;}
			void SetSizeEffectSpeed(float speed){mSizeEffectSpeed = speed;}

			void OnVibration(bool bOn);
			void SetVibrationSize(float size){mVibrationSize = size;} // 축방향
			void SetVibrationNegativeSize(float negativeSize){mVibrationNegativeSize = negativeSize;} // 축 반대방향
			void SetVibrationSpeed(float speed){mVibrationSpeed = speed;}
			void SetVibrationAxis(Ogre::Vector3 axis){mVibrationAxis = axis;}

			Ogre::AxisAlignedBox GetBoundingBox();
			Ogre::Entity* GetEntity();

			void SetPlan(int plan){mPlan = plan;}
			int GetPlan(){return mPlan;}

			// **** Caution, ColorHighlight, AlphaMode 는 동시에 작동할 수 없음 **** //
			void Caution(bool bOn);
			bool IsCaution(){return mbCaution;}

			void SetHighlightColor(Ogre::ColourValue color);
			void ColorHighlight(bool bOn);
			Ogre::ColourValue GetHighlightColor(){return mHighlihgtColor;}
			bool IsColorHighlight(){return mbHighlightColor;}

			void SetAlphaMode(bool bAlpha);
			void SetAlphaValue(float fAlpha);
			bool GetAlphaMode(){return mbAlphaMode;}
			float GetAlphaValue(){return mfAlpha;}
		
			void SetCeilingObject(bool bCeilingObject){mbCeilingObject = bCeilingObject;}
			bool GetCeilingObject(){return mbCeilingObject;}

			void SetPlanVisible(int planIdx);

			void PrevMove();
			void PostMove();
			void Set2DObject(void* p2DObject){mp2DObject = p2DObject;}
			void* Get2DObject(){return mp2DObject;}

			void ShowObject(bool bShow);

		protected:
			void _Create();
			void _Destroy();
			virtual bool frameStarted(const Ogre::FrameEvent& evt);
			virtual bool frameEnded(const Ogre::FrameEvent& evt);
		
		protected:
			UnE::Core::UBaseView * m_pView;
			Ogre::String mID;
			Ogre::String mName;
			Ogre::String mMeshFile;
			Ogre::SceneManager* mpSceneManager;
			Ogre::SceneNode*    mpNode;
			Ogre::SceneNode*    mpChildNode;
			Ogre::Entity*       mpEntity;
			Ogre::Entity*		mpEffectEntity;
			Ogre::ManualObject* mpBoundingBox;
			Ogre::MaterialPtr   mpEffectMtr;
			std::vector<Ogre::MaterialPtr> mMtrs;
			std::vector<Ogre::MaterialPtr> mClonMtrs;
			std::vector<Ogre::MaterialPtr> mHighLightColorMtrs;
			std::vector<Ogre::MaterialPtr> mAlphaMtrs;

			bool mbCreated;
			bool mbSelected;
			bool mbVisible;
			bool mbIsReleased;
			bool mbFirstFrame;

			float mHeading;
			Ogre::Vector3 mPosition;
			Ogre::Vector3 mTopPosition;
			Ogre::Vector3 mOriginScale;
			float mRunTime;

			float mGlowAlpha;
			Ogre::ColourValue mGlowColor;
			float mGlowSize;
			bool mbVisibleGlow;

			float mOriginSize;
			float mSizeEffectMaxSize;
			float mSizeEffectMinSize;
			float mSizeEffectSpeed;
			bool  mbSizeEffect;

			float mVibrationSize;
			float mVibrationNegativeSize;
			float mVibrationSpeed;
			Ogre::Vector3 mVibrationAxis;
			bool mbVibration;

			bool mbCaution;
			float mCautionRunTime;

			bool mbNodeCreate;
			int mPlan;

			Ogre::ColourValue mHighlihgtColor;
			bool mbHighlightColor;

			bool mbAlphaMode;
			float mfAlpha;
			unsigned char mOriginRenderQueGroup;

			bool mbCeilingObject;

			Ogre::Vector3 mPrevPos;
			void* mp2DObject;

			bool mbShow;
		};
	}
}//namespace