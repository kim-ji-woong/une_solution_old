#include "StdAfx.h"

#include <Ogre.h>
#include <OgreFontManager.h>
#include <OgreOverlayElement.h>
#include "TextPOI.h"


#define UNICODE_NEL 0x0085 // next line
#define UNICODE_CR 0x000D
#define UNICODE_LF 0x000A
#define UNICODE_SPACE 0x0020
#define UNICODE_ZERO 0x0030
using namespace Ogre;

#ifndef OGRE_DELETE_ARRAY
#define OGRE_DELETE_ARRAY(p) { if(p) { delete[] (p);   (p)=NULL; } }
#endif


 DisplayString StringToDisplayString(String str)
{
	LPCSTR ansiStr = str.c_str();
	
	LPWSTR szUniStr;  
	int nLen = MultiByteToWideChar(CP_ACP, 0, ansiStr, -1, NULL, NULL);
	nLen = nLen * sizeof(WCHAR);  
	szUniStr = (LPWSTR)malloc(nLen+1);
	
	memset(szUniStr,0,nLen+1);  
	MultiByteToWideChar(CP_ACP, 0, ansiStr, -1, szUniStr, nLen);     
	
	DisplayString result(szUniStr);
	
	free(szUniStr);
	
	return result;
}


namespace UnE
{
	namespace Core
	{
		UTextPOI::UTextPOI(SceneManager* pSceneMgr, Camera* pCam, Ogre::Font*	pFont, Vector3 position, float charHeight, ColourValue textColor, String caption)
			:mpSceneMgr(pSceneMgr)
			,mpCamera(pCam)
			,mpFont(pFont)
			,m3DPosition(position)
			,mCharHeight(charHeight)
			,mSpaceWidth(0)
			,mVerticalAdjust(0)
			,mColor(textColor)
			,mbNeedUpdate(true)
			,mHorizontalAlignment(H_CENTER)
			,mVerticalAlignment(V_CENTER)
			,mbVisible(true)
			,mbDraw(true)
			,mpGeometryData(NULL)
			,mSBL(1)
			,mCaptionOri(caption)
			,mLODDist(10000)
		{
			mCaption = StringToDisplayString(caption);

			mCaptionSize = mCaption.size();
			mpGeometryData = new TextPOIGeoMetryData[mCaptionSize];
			mToggleLod = false;
		}

		UTextPOI::~UTextPOI(void)
		{
			OGRE_DELETE_ARRAY(mpGeometryData);
		}

		void UTextPOI::Set3DPosition(Vector3 vPos)
		{
			if(m3DPosition != vPos)
			{
				m3DPosition = vPos;
				_Update2DPosition();
			}
		}

		Vector3 UTextPOI::Get3DPosition()
		{
			return m3DPosition;
		}

		void UTextPOI::SetColor(ColourValue color)
		{
			mColor = color;
		}

		Vector3 UTextPOI::Get2DPosition(bool bUpdate)
		{
			if(bUpdate)
				_Update2DPosition();
			return m2DPosition;
		}

		Vector2 UTextPOI::GetScreenPosition(bool bUpdate)
		{
			if(bUpdate)
				_Update2DPosition();
			Vector2 res;
			res.x = ((m2DPosition.x / 2) + 0.5f) * mpCamera->getViewport()->getActualWidth();
			res.y = (1 - ((m2DPosition.y / 2) + 0.5f)) * mpCamera->getViewport()->getActualHeight();
			return res;
		}

		void UTextPOI::SetCaption(String caption)
		{
			DisplayString tmp = StringToDisplayString(caption);
			if(mCaption != tmp)
			{
				OGRE_DELETE_ARRAY(mpGeometryData);
				mCaption = tmp;
				mCaptionSize = mCaption.size();
				mpGeometryData = new TextPOIGeoMetryData[mCaptionSize];
				mbNeedUpdate = true;
			}
		}

		void UTextPOI::SetCharHeight(float charHeight)
		{
			if(mCharHeight != charHeight)
			{
				mCharHeight = charHeight;
				mbNeedUpdate = true;
			}
		}

		void UTextPOI::SetTextAlignment(const HorizontalAlignment& horizontalAlignment, const VerticalAlignment& verticalAlignment)
		{
			if(mHorizontalAlignment != horizontalAlignment)
			{
				mHorizontalAlignment = horizontalAlignment;
				mbNeedUpdate = true;
			}
			if(mVerticalAlignment != verticalAlignment)
			{
				mVerticalAlignment = verticalAlignment;
				mbNeedUpdate = true;
			}
		}

		void UTextPOI::SetVerticalAdjust(float verticalAdjust)
		{
			if(mVerticalAdjust != verticalAdjust)
			{
				mVerticalAdjust = verticalAdjust;
				mbNeedUpdate = true;
			}
		}

		void UTextPOI::SetSpaceWidth(float spaceWidth)
		{
			if(mSpaceWidth != spaceWidth)
			{
				mSpaceWidth = spaceWidth;
				mbNeedUpdate = true;
			}
		}

		void UTextPOI::SetSpaceBetweenLetters(float sbl)
		{
			if(mSBL != sbl)
			{
				mSBL = sbl;
				mbNeedUpdate = true;
			}
		}

		void UTextPOI::_Update2DPosition()
		{
			Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
			m2DPosition = mpCamera->getProjectionMatrix() * (mpCamera->getViewMatrix() * (m3DPosition * rootScale));
			float dist = ((mpCamera->getRealPosition() / rootScale) - m3DPosition).length();
			bool bCheckLOD = dist > mLODDist;
			if( mToggleLod == true)
			{
				bCheckLOD = dist < mLODDist;
			}
			if(m2DPosition.x < -1 || m2DPosition.x > 1 || m2DPosition.y < -1 || m2DPosition.y > 1 || m2DPosition.z < -1 || m2DPosition.z > 1
				|| bCheckLOD)
			{
				mbDraw = false;
			}
			else mbDraw = true;
		}

		void UTextPOI::_UpdateGeometry()
		{
			if(mbNeedUpdate)
			{
				mCaptionSize = mCaption.size();
				int width = mpCamera->getViewport()->getActualWidth();
				int height = mpCamera->getViewport()->getActualHeight();
				float capHeight = (mCharHeight * 2) / height;
				float verAdj = (mVerticalAdjust * 2) / height;

				float spaceWidth = mSpaceWidth;
				if (spaceWidth == 0)
					spaceWidth = mpFont->getGlyphAspectRatio(UNICODE_ZERO) * mCharHeight;
				spaceWidth = (spaceWidth * 2) / width;

				float verticalOffset = 0;
				switch (mVerticalAlignment)
				{
				case UTextPOI::V_ABOVE:
					verticalOffset = capHeight;
					break;
				case UTextPOI::V_CENTER:
					verticalOffset = 0.5f*capHeight;
					break;
				case UTextPOI::V_BELOW:
					verticalOffset = 0;
					break;
				}

				int baseIdx = 0;
				float len = 0;
				bool newLine = true;
				float top = verticalOffset;
				float left = 0;

				DisplayString::iterator i, iend;
				iend = mCaption.end();
				for (i = mCaption.begin(); i != iend; ++i)
				{
					Ogre::Font::CodePoint character = OGRE_DEREF_DISPLAYSTRING_ITERATOR(i);
					if (character == UNICODE_LF)
					{
						top += verticalOffset;
					}
				}

				int count = 0;
				for(i = mCaption.begin(); i != iend; ++i)
				{
					if (newLine)
					{
						len = 0.0f;
						for(DisplayString::iterator j = i; j != iend; j++)
						{
							Ogre::Font::CodePoint character = OGRE_DEREF_DISPLAYSTRING_ITERATOR(j);
							if (character == UNICODE_CR	|| character == UNICODE_NEL || character == UNICODE_LF) 
							{
								break;
							}
							else if(character == UNICODE_SPACE)
							{
								len += mSpaceWidth + mSBL;
							}
							else
							{
								len += mpFont->getGlyphAspectRatio(character) * mCharHeight + mSBL;
							}
						}
						len = (len * 2) / width;
						newLine = false;
					}

					Ogre::Font::CodePoint character = OGRE_DEREF_DISPLAYSTRING_ITERATOR(i);
					if (character == UNICODE_LF)
					{
						left = 0;
						top -= capHeight;
						newLine = true;
						continue;
					}

					if (character == UNICODE_SPACE)
					{
						left += spaceWidth;
						mCaptionSize--;
						continue;
					}

					float asp = mpFont->getGlyphAspectRatio(character);
					float capWidth = mCharHeight * asp;
					capWidth = (capWidth * 2) / width;

					Ogre::Font::UVRect utmp;
					utmp = mpFont->getGlyphTexCoords(character);
					mpGeometryData[count].texCoords[0].x = utmp.left;  mpGeometryData[count].texCoords[0].y = utmp.top;
					mpGeometryData[count].texCoords[1].x = utmp.right; mpGeometryData[count].texCoords[1].y = utmp.top;
					mpGeometryData[count].texCoords[2].x = utmp.right; mpGeometryData[count].texCoords[2].y = utmp.bottom;
					mpGeometryData[count].texCoords[3].x = utmp.left;  mpGeometryData[count].texCoords[3].y = utmp.bottom;

					// top left
					if(mHorizontalAlignment == UTextPOI::H_LEFT)
					{
						mpGeometryData[count].coners[0].x = left;
					}
					else
					{
						mpGeometryData[count].coners[0].x = left - (len / 2);
					}
					mpGeometryData[count].coners[0].y = top + verAdj;
					mpGeometryData[count].coners[0].z = 0;

					// bottom left
					mpGeometryData[count].coners[3].x = mpGeometryData[count].coners[0].x;
					mpGeometryData[count].coners[3].y = mpGeometryData[count].coners[0].y - capHeight;
					mpGeometryData[count].coners[3].z = 0;

					left += capWidth;

					// top right
					if(mHorizontalAlignment == UTextPOI::H_LEFT)
					{
						mpGeometryData[count].coners[1].x = left;
					}
					else
					{
						mpGeometryData[count].coners[1].x = left - (len / 2);
					}
					mpGeometryData[count].coners[1].y = mpGeometryData[count].coners[0].y;
					mpGeometryData[count].coners[1].z = 0;

					// bottom right
					mpGeometryData[count].coners[2].x = mpGeometryData[count].coners[1].x;
					mpGeometryData[count].coners[2].y = mpGeometryData[count].coners[3].y;
					mpGeometryData[count].coners[2].z = 0;

					mpGeometryData[count].indexs[0] = 0;
					mpGeometryData[count].indexs[1] = 2;
					mpGeometryData[count].indexs[2] = 1;
					mpGeometryData[count].indexs[3] = 0;
					mpGeometryData[count].indexs[4] = 3;
					mpGeometryData[count].indexs[5] = 2;

					left += (mSBL * 2) / width;

					count++;
				}
				mbNeedUpdate = false;
			}
		}
	}
}
