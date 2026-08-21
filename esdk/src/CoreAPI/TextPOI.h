#pragma once

#include "CoreAPI.h"


namespace UnE
{
	namespace Core
	{
		struct CORE_API TextPOIGeoMetryData{
			Ogre::Vector3		coners[4];
			Ogre::Vector2		texCoords[4];
			unsigned short		indexs[6];
		};

		class CORE_API UTextPOI
		{
		public:
			enum HorizontalAlignment    {H_LEFT, H_CENTER};
			enum VerticalAlignment      {V_BELOW, V_ABOVE, V_CENTER};

		public:
			UTextPOI(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::Font*	pFont, Ogre::Vector3 position, float charHeight, Ogre::ColourValue textColor, Ogre::String caption = "");
			~UTextPOI(void);

			void Set3DPosition(Ogre::Vector3 vPos);
			Ogre::Vector3 Get3DPosition();
			Ogre::Vector3 Get2DPosition(bool bUpdate = true);
			Ogre::Vector2 GetScreenPosition(bool bUpdate = true);
			void SetColor(Ogre::ColourValue color);
			Ogre::ColourValue GetColor(){return mColor;}
			void SetVisible(bool bVisible){mbVisible = bVisible;}
			bool GetVisible(){return mbVisible;}
			bool IsDraw(){return mbDraw;}
			size_t GetCaptionSize(){return mCaptionSize;}
			void SetCaption(Ogre::String caption);
			void SetCharHeight(float charHeight);
			float GetCharHeight(){return mCharHeight;}
			void SetTextAlignment(const HorizontalAlignment& horizontalAlignment, const VerticalAlignment& verticalAlignment);
			void SetVerticalAdjust(float verticalAdjust);
			float GetVerticlaAdjust(){return mVerticalAdjust;}
			void SetSpaceWidth(float spaceWidth);
			float GetSpaceWidth(){return mSpaceWidth;}
			void SetSpaceBetweenLetters(float sbl);

			void _Update2DPosition();
			void _UpdateGeometry();
			TextPOIGeoMetryData* GetPOIGeometryData(){return mpGeometryData;}
			
			Ogre::String GetCaption(){return mCaptionOri;}
			void SetLODDist(float lodDist){mLODDist = lodDist;}
			void ToggleLODDist(bool bToggle) { mToggleLod = bToggle;}
			bool IsToggleLOD() { return mToggleLod ;}
			int GetID() const { return mID; }
			void SetID(int val) { mID = val; }
		protected:
			Ogre::SceneManager* mpSceneMgr;
			Ogre::Camera*		mpCamera;
			Ogre::Font*			mpFont;
			Ogre::Vector3		m3DPosition;
			Ogre::Vector3		m2DPosition;
			Ogre::DisplayString	mCaption;
			Ogre::String		mCaptionOri;
			size_t				mCaptionSize;
			HorizontalAlignment	mHorizontalAlignment;
			VerticalAlignment	mVerticalAlignment;
			Ogre::ColourValue	mColor;
			float				mCharHeight;
			float				mSpaceWidth;
			float				mSBL; // ÀÚ°£
			float				mVerticalAdjust;
			bool				mbVisible;
			bool				mbDraw;
			bool				mbNeedUpdate;

			TextPOIGeoMetryData*	mpGeometryData;
			float mLODDist;
			bool mToggleLod;
			int mID;
		};
	}
}
