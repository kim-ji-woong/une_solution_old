#include "stdafx.h"
#include <Ogre.h>
#include <OgreFontManager.h>
#include <OgrePrerequisites.h>

#include "UMovableTextOverlay.h"

using namespace Ogre;



namespace UnE
{
	namespace Core
	{

		DisplayString StringToDisplayString(String str)
		{
			LPCSTR ansiStr = str.c_str();

			LPWSTR szUniStr;
			int nLen = MultiByteToWideChar(CP_ACP, 0, ansiStr, -1, NULL, NULL);
			nLen = nLen * sizeof(WCHAR);
			szUniStr = (LPWSTR)malloc(nLen + 1);

			memset(szUniStr, 0, nLen + 1);
			MultiByteToWideChar(CP_ACP, 0, ansiStr, -1, szUniStr, nLen);

			DisplayString result(szUniStr);

			free(szUniStr);

			return result;
		}


		UMovableTextOverlay::UMovableTextOverlay(int nID, Ogre::MovableObject* pMov, Camera* pCam, Vector3 position, String caption, UMovableTextOverlayAttributes* attrib)
			: m_nID(nID)
			, mpOv(NULL)
			, mpOvContainer(NULL)
			, mpOvText(NULL)
			, mAttrs(attrib)
			, mName("")
			, mCaption("")
			, mUpdateFrequency(0.01)
			, mNeedUpdate(TRUE)
			, mOnScreen(FALSE)
			, mEnabled(FALSE)
		{
			mpMov = pMov;
			// create an overlay that we can use for later
			mName = "_MovableTextOverlay_" + Ogre::StringConverter::toString(nID);
			mpOv = Ogre::OverlayManager::getSingleton().create(mName);
			mpOv->hide();
			//mpOvContainer = static_cast<Ogre::BorderPanelOverlayElement*>(Ogre::OverlayManager::getSingleton().getOverlayElement("TestScriptOverlay", false));

			mpOvContainer = static_cast<Ogre::BorderPanelOverlayElement*>(Ogre::OverlayManager::getSingleton().createOverlayElement(
				"BorderPanel", mName + "_OvC"));
			mpOvContainer->setDimensions(0.0, 0.0);
			//mpOvContainer->setMetricsMode(Ogre::GMM_PIXELS);
			

			mpOvContainer->setBorderMaterialName("BorderPaneBorder");

			//mpOvContainer->setMetricsMode(Ogre::GMM_RELATIVE);

			mpOv->add2D(mpOvContainer);

			mpOvText = static_cast<Ogre::TextAreaOverlayElement*>(Ogre::OverlayManager::getSingleton().createOverlayElement("TextArea", mName + "_OvTxt"));
			mpOvContainer->addChild(mpOvText);
			mpOvText->setMetricsMode(Ogre::GMM_RELATIVE);
			mpOvText->setDimensions(0.9, 0.9);
			mpOvText->setMetricsMode(Ogre::GMM_PIXELS);
			mpOvText->setPosition(10, 4);
			mpOvText->setColour(Ogre::ColourValue(0.0f, 0.0f, 1.0f));
			//mpOvText->setMaterialName("RedTransparent2");

			_updateOverlayAttrs();

			setCaption(caption);
		}

		UMovableTextOverlay::~UMovableTextOverlay()
		{
			// overlay cleanup -- Ogre would clean this up at app exit but if your app 
			// tends to create and delete these objects often it's a good idea to do it here.

			mpOv->hide();
			Ogre::OverlayManager *overlayManager = Ogre::OverlayManager::getSingletonPtr();
			mpOvContainer->removeChild(mName + "_OvTxt");
			mpOv->remove2D(mpOvContainer);
			overlayManager->destroyOverlayElement(mpOvText);
			overlayManager->destroyOverlayElement(mpOvContainer);
			overlayManager->destroy(mpOv);
		}

		void UMovableTextOverlay::setCaption(const Ogre::String & text)
		{
			Ogre::DisplayString caption = StringToDisplayString(text);
			if (caption != mCaption)
			{
				mCaption = caption;
				mpOvText->setCaption(mCaption);
				mNeedUpdate = true;
			}
		}

		void UMovableTextOverlay::SetPoistion(float x, float y, float z)
		{			
			mX = x;
			mY = y;
			mZ = z;
			mpMov = NULL;
		}

		void UMovableTextOverlay::_computeTextWidth()
		{
			const Font *pFont = mAttrs->getFont();
			mTextWidth = 0;

			for (Ogre::DisplayString::iterator i = mCaption.begin(); i < mCaption.end(); i++)
			{
				if (*i == 0x0020)
					mTextWidth += pFont->getGlyphAspectRatio(0x0030);
				else
				{
					mTextWidth += pFont->getGlyphAspectRatio(*i);
				}			
			}
			mTextWidth *= mAttrs->getCharacterHeight();
		}

		void UMovableTextOverlay::_getMinMaxEdgesOfTopAABBIn2D(Ogre::Real& MinX, Ogre::Real& MinY, Ogre::Real& MaxX, Ogre::Real& MaxY)
		{
			const Ogre::Camera* mpCam = mAttrs->getCamera();

			MinX = 0;
			MinY = 0;
			MaxX = 0;
			MaxY = 0;

			Ogre::Real X[4];// the 2D dots of the AABB in screencoordinates
			Ogre::Real Y[4];

			if (mpMov == NULL)
				return;
			//if (!mpMov->isInScene())
			//	return;

			const Ogre::AxisAlignedBox &AABB = mpMov->getWorldBoundingBox(true);// the AABB of the target
			const Ogre::Vector3 CornersOfTopAABB[4] = { AABB.getCorner(AxisAlignedBox::FAR_LEFT_TOP),
				AABB.getCorner(AxisAlignedBox::FAR_RIGHT_TOP),
				AABB.getCorner(AxisAlignedBox::NEAR_LEFT_TOP),
				AABB.getCorner(AxisAlignedBox::NEAR_RIGHT_TOP) };

			Ogre::Vector3 CameraPlainNormal = mpCam->getDerivedOrientation().zAxis();//The normal vector of the plaine.this points directly infront of the cam

			Ogre::Plane CameraPlain = Plane(CameraPlainNormal, mpCam->getDerivedPosition());//the plaine that devides the space bevor and behin the cam

			for (int i = 0; i < 4; i++)
			{
				X[i] = 0;
				Y[i] = 0;

				_getScreenCoordinates(CornersOfTopAABB[i], X[i], Y[i]);// transfor into 2d dots


				if (CameraPlain.getSide(CornersOfTopAABB[i]) == Plane::NEGATIVE_SIDE)
				{

					if (i == 0)// accept the first set of values, no matter how bad it might be.
					{
						MinX = X[i];
						MinY = Y[i];
						MaxX = X[i];
						MaxY = Y[i];
					}
					else// now compare if you get "better" values
					{
						if (MinX > X[i])// get the x minimum
						{
							MinX = X[i];
						}
						if (MinY > Y[i])// get the y minimum
						{
							MinY = Y[i];
						}
						if (MaxX < X[i])// get the x maximum
						{
							MaxX = X[i];
						}
						if (MaxY < Y[i])// get the y maximum
						{
							MaxY = Y[i];
						}
					}
				}
				else
				{
					MinX = 0;
					MinY = 0;
					MaxX = 0;
					MaxY = 0;
					break;
				}
			}
		}

		void UMovableTextOverlay::_getScreenCoordinates(const Ogre::Vector3& position, Ogre::Real& x, Ogre::Real& y)
		{
			const Ogre::Camera* mpCam = mAttrs->getCamera();
			Vector3 hcsPosition = mpCam->getProjectionMatrix() * (mpCam->getViewMatrix() * position);

			x = 1.0f - ((hcsPosition.x * 0.5f) + 0.5f);// 0 <= x <= 1 // left := 0,right := 1
			y = ((hcsPosition.y * 0.5f) + 0.5f);// 0 <= y <= 1 // bottom := 0,top := 1
		}

		void UMovableTextOverlay::enable(bool enable)
		{
			if (mEnabled == enable)
				return;

			mEnabled = enable;
			if (mEnabled)
				mpOv->show();
			else
				mpOv->hide();
		}

		void UMovableTextOverlay::update(Real timeSincelastFrame)
		{
			static Real timeTillUpdate = 0;

			timeTillUpdate -= timeSincelastFrame;
			if (timeTillUpdate > 0)
				return;
			timeTillUpdate = mUpdateFrequency;

			float f = (mTextWidth + 20);
			float f2 = (mAttrs->getCharacterHeight() + 7);

			Real relTextWidth = f / Ogre::OverlayManager::getSingleton().getViewportWidth();
			Real relTextHeight = f2 / Ogre::OverlayManager::getSingleton().getViewportHeight();

			if (mpMov != NULL)
			{
				Ogre::Real min_x, max_x, min_y, max_y;
				_getMinMaxEdgesOfTopAABBIn2D(min_x, min_y, max_x, max_y);

				if ((min_x>0.0) && (max_x<1.0) && (min_y>0.0) && (max_y<1.0))
					mOnScreen = true;
				else
					mOnScreen = false;

				if (mNeedUpdate)
				{
					_computeTextWidth();
					mNeedUpdate = false;
				}

				mpOvContainer->setPosition(1 - (min_x + max_x + relTextWidth) / 2, 1 - max_y);
				mpOvContainer->setDimensions(relTextWidth, relTextHeight);
			    
			}
			else
			{

				if (mNeedUpdate)
				{
					_computeTextWidth();
					mNeedUpdate = false;
				}
				Vector3 vec(mX, mY, mZ);
				float x = 0; float y = 0;

				
				_getScreenCoordinates(vec, x, y);
				mpOvContainer->setPosition(1 - x - relTextWidth / 2.0f, 1 - y - relTextHeight);
				mpOvContainer->setDimensions(relTextWidth, relTextHeight);

				float ux = relTextWidth / f;
				float vy = relTextHeight / f2;
				mpOvContainer->setBorderSize(ux, ux, vy, vy);
			}	
		}

		void UMovableTextOverlay::_updateOverlayAttrs()
		{
			const String &newMatName = mAttrs->getMaterialName();
			const String &oldMatName = mpOvContainer->getMaterialName();
			if (oldMatName != newMatName)
			{
				if (oldMatName.length())
					mpOvContainer->getMaterial()->unload();

				if (newMatName.length())
					mpOvContainer->setMaterialName(newMatName);

			}

			mpOvText->setColour(mAttrs->getColor());

			mpOvText->setParameter("font_name", mAttrs->getFontName());
			mpOvText->setParameter("char_height", Ogre::StringConverter::toString(mAttrs->getCharacterHeight()));
			mpOvText->setParameter("horz_align", "left");
			mpOvText->setParameter("vert_align", "top");
		}


		UMovableTextOverlayAttributes::UMovableTextOverlayAttributes(const Ogre::String & name, const Ogre::Camera *cam,
			const Ogre::String & fontName, int charHeight, const Ogre::ColourValue & color, const Ogre::String & materialName)
			: mpCam(cam)
			, mpFont(NULL)
			, mName(name)
			, mFontName("")
			, mMaterialName("")
			, mCharHeight(charHeight)
			, mColor(ColourValue::ZERO)
		{
			if (fontName.length() == 0)
				Ogre::Exception(Ogre::Exception::ERR_INVALIDPARAMS, "Invalid font name", "MovableTextOverlayAttributes::MovableTextOverlayAttributes");

			setFontName(fontName);
			setMaterialName(materialName);
			setColor(color);
		}

		UMovableTextOverlayAttributes::~UMovableTextOverlayAttributes()
		{
			setFontName("");
			setMaterialName("");
		}

		void UMovableTextOverlayAttributes::setFontName(const Ogre::String & fontName)
		{
			if (mFontName != fontName || !mpFont)
			{
				if (mpFont)
				{
					mpFont->unload();
					mpFont = NULL;
				}

				mFontName = fontName;
				if (mFontName.length())
				{
					mpFont = dynamic_cast<Ogre::Font*>(Ogre::FontManager::getSingleton().getByName(mFontName).getPointer());
					if (!mpFont)
						Ogre::Exception(Ogre::Exception::ERR_ITEM_NOT_FOUND, "Could not find font " + fontName, "MovableTextOverlay::setFontName");
					mpFont->load();
				}
			}
		}

		void UMovableTextOverlayAttributes::setMaterialName(const Ogre::String & materialName)
		{
			if (mMaterialName != materialName)
			{
				if (mMaterialName.length())
					Ogre::MaterialManager::getSingletonPtr()->getByName(mMaterialName).getPointer()->unload();

				mMaterialName = materialName;
				if (mMaterialName.length())
				{
					Ogre::Material *mpMaterial = dynamic_cast<Ogre::Material*>(Ogre::MaterialManager::getSingletonPtr()->getByName(materialName).getPointer());
					if (!mpMaterial)
						Ogre::Exception(Ogre::Exception::ERR_ITEM_NOT_FOUND, "Could not find font " + materialName, "MovableTextOverlay::setMaterialName");
					mpMaterial->load();
				}
			}
		}

		void UMovableTextOverlayAttributes::setColor(const Ogre::ColourValue & color)
		{
			mColor = color;
		}

		void UMovableTextOverlayAttributes::setCharacterHeight(unsigned int height)
		{
			mCharHeight = height;
		}
	}
}
