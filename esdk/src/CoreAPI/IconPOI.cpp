#include "StdAfx.h"
#include "IconPOI.h"
#include <Ogre.h>


using namespace Ogre;


namespace UnE
{
	namespace Core
	{
		UIconPOI::UIconPOI(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::Vector3 position, float width, float height)
			:mpSceneMgr(pSceneMgr)
			,mpCamera(pCam)
			,m3DPosition(position)
			,mWidth(width)
			,mHeight(height)
			,mColor(1,1,1)
			,mHilightColor(1,0,0)
			,mBlinkColorFirst(1,0,0)
			,mBlinkColorSecond(1,1,1)
			,mbVisible(true)
			,mHilightType(0)
			,mHilightTime(0)
			,mBlinkTime(1)
			,mLODDist(10000)
			,mAmount(100)
		{			
			mPickWidth = width;
			mPickHeight = height;
			mbEnabled = true;
		}

		UIconPOI::~UIconPOI(void)
		{

		}

		void UIconPOI::Set3DPosition(Vector3 vPos)
		{
			if(m3DPosition != vPos)
			{
				m3DPosition = vPos;
				_Update();
			}
		}

		void UIconPOI::SetIconSize(float width, float height)
		{
			if(mWidth != width || mHeight != height)
			{
				mWidth = width;
				mHeight = height;
			}
		}

		Ogre::Vector3 UIconPOI::Get3DPosition()
		{
			return m3DPosition;
		}
		Ogre::Vector3 UIconPOI::Get2DPosition(bool bUpdate)
		{
			if(bUpdate)
				_Update();
			return m2DPosition;
		}

		Ogre::Vector2 UIconPOI::GetScreenPosition(bool bUpdate)
		{
			if(bUpdate)
				_Update();
			Vector2 res;
			res.x = ((m2DPosition.x / 2) + 0.5f) * mpCamera->getViewport()->getActualWidth();
			res.y = (1 - ((m2DPosition.y / 2) + 0.5f)) * mpCamera->getViewport()->getActualHeight();
			return res;
		}

		void UIconPOI::SetBlinkColor(Ogre::ColourValue colorFirst, Ogre::ColourValue colorSecond)
		{
			mBlinkColorFirst = colorFirst;
			mBlinkColorSecond = colorSecond;
		}

		Ogre::ColourValue UIconPOI::GetColor(float time)
		{
			if(mHilightType == 1)
			{
				return mHilightColor;
			}
			else if(mHilightType == 2)
			{
				mHilightTime += time;
				float sinvalue = (Math::Sin(mHilightTime * 5 * (1.0f / mBlinkTime)) + 1) * 0.5f;
				Ogre::ColourValue diff2 = mBlinkColorFirst - mBlinkColorSecond;
				return mBlinkColorSecond + (diff2 * sinvalue);
			}
			if(mHilightType == 3)
			{
				return mDisableColor;
			}
			return mColor;
		}

		bool UIconPOI::IsDrawed()
		{
			if(mbVisible)
			{
				_Update();
				Ogre::Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
				if(m2DPosition.x < -1 || m2DPosition.x > 1 || m2DPosition.y < -1 || m2DPosition.y > 1 || m2DPosition.z < -1 || m2DPosition.z > 1
					|| ((mpCamera->getRealPosition() / rootScale) - m3DPosition).length() > mLODDist)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		IconPOIGeoMetryData UIconPOI::GetGeometryData(bool bUpdate)
		{
			if(bUpdate)
				_Update();
			IconPOIGeoMetryData data;
			Ogre::Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
			if(m2DPosition.x < -1 || m2DPosition.x > 1 || m2DPosition.y < -1 || m2DPosition.y > 1 || m2DPosition.z < -1 || m2DPosition.z > 1
				|| ((mpCamera->getRealPosition() / rootScale) - m3DPosition).length() > mLODDist)
			{
				data.bDraw = false;
				return data;
			}

			int width = mpCamera->getViewport()->getActualWidth();
			int height = mpCamera->getViewport()->getActualHeight();
			float halfwidth = mWidth / width;
			float halfHeight = mHeight / height;
			//float left = m2DPosition.x;
			float left = m2DPosition.x - halfwidth;
			float top = m2DPosition.y + halfHeight*2;
			float bottom = m2DPosition.y;// - halfHeight;
			float right = left + (halfwidth * 2) * mAmount * 0.01f;

			data.coners[0] = Ogre::Vector3(left, top, m2DPosition.z);
			data.coners[1] = Ogre::Vector3(right, top, m2DPosition.z);
			data.coners[2] = Ogre::Vector3(right, bottom, m2DPosition.z);
			data.coners[3] = Ogre::Vector3(left, bottom, m2DPosition.z);

			float coordLeft = 1.0f - mAmount * 0.01f;
			data.texCoords[0] = Ogre::Vector2(coordLeft,0);
			data.texCoords[1] = Ogre::Vector2(1,0);
			data.texCoords[2] = Ogre::Vector2(1,1);
			data.texCoords[3] = Ogre::Vector2(coordLeft,1);

			data.indexs[0] = 0;
			data.indexs[1] = 2;
			data.indexs[2] = 1;
			data.indexs[3] = 0;
			data.indexs[4] = 3;
			data.indexs[5] = 2;

			return data;
		}

		std::pair<bool, float> UIconPOI::Pick(float x, float y)
		{
			Ogre::Vector2 scrPos = GetScreenPosition();

			std::pair<bool, float> res;
			res.first = false;

			Ogre::Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
			bool bDraw = true;
			if(m2DPosition.x < -1 || m2DPosition.x > 1 || m2DPosition.y < -1 || m2DPosition.y > 1 || m2DPosition.z < -1 || m2DPosition.z > 1
				|| ((mpCamera->getRealPosition() / rootScale) - m3DPosition).length() > mLODDist)
			{
				bDraw = false;
			}

			if(!mbVisible || !bDraw) return res;

			Ogre::Rectangle rt;
			//rt.left = scrPos.x; 
			rt.left = scrPos.x - mPickWidth * 0.5f;	
			rt.top = scrPos.y -mPickHeight;//- mHeight * 0.5f;
			rt.right = rt.left + mPickWidth;
			rt.bottom = scrPos.y;

			res.first = rt.inside(x,y);
			res.second = m2DPosition.z;

			return res;
		}

		void UIconPOI::_Update()
		{
			Ogre::Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
			m2DPosition = mpCamera->getProjectionMatrix() * (mpCamera->getViewMatrix() * (m3DPosition * rootScale));
		}

		void UIconPOI::SetPickSize( float pWidth, float pHeight )
		{
			mPickWidth = pWidth;
			mPickHeight = pHeight;
		}

	}
}
