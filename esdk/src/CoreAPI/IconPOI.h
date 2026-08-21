#pragma once

#include "CoreAPI.h"



namespace UnE
{
	namespace Core
	{
		struct __declspec(dllexport) IconPOIGeoMetryData{
			bool				bDraw;
			Ogre::Vector3		coners[4];
			Ogre::Vector2		texCoords[4];
			unsigned short		indexs[6];
			IconPOIGeoMetryData(){bDraw = true;}
		};

		class CORE_API UIconPOI
		{
		public:
			UIconPOI(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::Vector3 position, float width, float height);
			~UIconPOI(void);

			void Set3DPosition(Ogre::Vector3 vPos);
			void SetIconSize(float width, float height);
			void SetPickSize(float pWidth, float pHeight);

			Ogre::Vector3 Get3DPosition();
			Ogre::Vector3 Get2DPosition(bool bUpdate = true);
			
			Ogre::Vector2 GetScreenPosition(bool bUpdate = true);
			
			void SetHilightColor(Ogre::ColourValue color){mHilightColor = color;}
			void SetBlinkColor(Ogre::ColourValue colorFirst, Ogre::ColourValue colorSecond);
			void SetDisableColor(Ogre::ColourValue val) { mDisableColor = val; }
			Ogre::ColourValue GetColor(float time);
			
			void SetVisible(bool bVisible){mbVisible = bVisible;}
			bool GetVisible(){return mbVisible;}
			bool IsDrawed();
			void SetBlinkTime(float blinkTime){mBlinkTime = blinkTime;}

			IconPOIGeoMetryData GetGeometryData(bool bUpdate = true);

			std::pair<bool, float> Pick(float x, float y);
			void SetHilightMode(long nMode){if(mHilightType != nMode){mHilightType = nMode; mHilightTime = 0;}}
			long GetHilightMode() const { return mHilightType; }
			void SetLODDist(float lodDist){mLODDist = lodDist;}

			void SetAmount(float amount){mAmount = amount; if(mAmount < 0.1f) mAmount = 0.1f; if(mAmount > 100) mAmount = 100;}
			float GetAmount(){return mAmount;}

			void _Update();		
			int GetID() const { return mID; }
			void SetID(int val) { mID = val; }

			bool IsEnabled() const { return mbEnabled; }
			void Enabled(bool bEnabled) { mbEnabled = bEnabled;}
		protected:
			Ogre::SceneManager* mpSceneMgr;
			Ogre::Camera* mpCamera;
			Ogre::Vector3 m3DPosition;
			Ogre::Vector3 m2DPosition;
			float mWidth;
			float mHeight;
			Ogre::ColourValue mColor;
			bool mbVisible;
			float mPickWidth;
			float mPickHeight;
			Ogre::ColourValue mDisableColor;
			
			bool mbEnabled;
			
			Ogre::ColourValue mHilightColor;
			Ogre::ColourValue mBlinkColorFirst;
			Ogre::ColourValue mBlinkColorSecond;
			long mHilightType; // 0 = NONE, 1 = 색상 변경, 2 = 깜박임, 3 = disable
			
			
			float mHilightTime;
			float mBlinkTime;
			float mLODDist;

			float mAmount;

			int mID;
			
		};
	}
}
	