#ifndef __MovableTextOverlay_H__
#define __MovableTextOverlay_H__

#include "CoreAPI.h"
#include "Ogre.h"
#include "OgreFont.h"
#include "OgreFontManager.h"
#include "OgreBorderPanelOverlayElement.h"
#include "OgreTextAreaOverlayElement.h"

using namespace Ogre;

namespace UnE
{
	namespace Core
	{
		class CORE_API UMovableTextOverlayAttributes
		{
		public:
			UMovableTextOverlayAttributes(const Ogre::String & name, const Ogre::Camera *cam,
				const Ogre::String & fontName = "AritaSB", int charHeight = 16, const Ogre::ColourValue & color = Ogre::ColourValue::White, const Ogre::String & materialName = "");

			~UMovableTextOverlayAttributes();

			void setFontName(const Ogre::String & fontName);
			void setMaterialName(const Ogre::String & materialName);
			void setColor(const Ogre::ColourValue & color);
			void setCharacterHeight(unsigned int height);

			const Ogre::String& getName() const { return mName; }
			const Ogre::Camera* getCamera() const { return mpCam; }
			const Ogre::Font* getFont() const { return mpFont; }
			const Ogre::String& getFontName() const { return mFontName; }
			const Ogre::String& getMaterialName() const { return mMaterialName; }
			const Ogre::ColourValue& getColor() const { return mColor; }
			const Ogre::Real getCharacterHeight() const { return mCharHeight; }

			const Ogre::String mName;
			const Ogre::Camera *mpCam;

			Ogre::Font* mpFont;
			Ogre::String mFontName;
			Ogre::String mMaterialName;
			Ogre::ColourValue mColor;
			Ogre::Real mCharHeight;
		};

		class CORE_API UMovableTextOverlay {
		public:
			UMovableTextOverlay(int nID, Ogre::MovableObject* pMov, Camera* pCam, Vector3 position, String caption, UMovableTextOverlayAttributes* attrib);
			virtual ~UMovableTextOverlay();

			void setCaption(const Ogre::String & caption);
			void setUpdateFrequency(Ogre::Real updateFrequency) { mUpdateFrequency = updateFrequency; }
			void setAttributes(UMovableTextOverlayAttributes *attrs)
			{
				mAttrs = attrs;
				_updateOverlayAttrs();
			}

			const Ogre::String&	getName() const { return mName; }
			const Ogre::String&	getCaption() const { return mCaption; }
			const Ogre::Real getUpdateFrequency() const { return mUpdateFrequency; }
			const bool isOnScreen() const { return mOnScreen; }
			const bool isEnabled() const { return mEnabled; }
			const UMovableTextOverlayAttributes* getAttributes() const { return mAttrs; }

			void enable(bool enable);
			void update(Ogre::Real timeSincelastFrame);

			// Needed for RectLayoutManager.
			int getPixelsTop() { return Ogre::OverlayManager::getSingleton().getViewportHeight() * (mpOvContainer->getTop()); }
			int getPixelsBottom() { return Ogre::OverlayManager::getSingleton().getViewportHeight() * (mpOvContainer->getTop() + mpOvContainer->getHeight()); }
			int getPixelsLeft() { return Ogre::OverlayManager::getSingleton().getViewportWidth() * mpOvContainer->getLeft(); }
			int getPixelsRight() { return Ogre::OverlayManager::getSingleton().getViewportWidth() * (mpOvContainer->getLeft() + mpOvContainer->getWidth()); }

			void setPixelsTop(int px) { mpOvContainer->setTop((Ogre::Real)px / Ogre::OverlayManager::getSingleton().getViewportHeight()); }
			// end

			int GetID() { return m_nID;  }
			void SetID(int val) { m_nID = val; }
			void SetPoistion(float x, float y, float z);
		protected:
			void _computeTextWidth();
			void _updateOverlayAttrs();
			void _getMinMaxEdgesOfTopAABBIn2D(Ogre::Real& MinX, Ogre::Real& MinY, Ogre::Real& MaxX, Ogre::Real& MaxY);
			void _getScreenCoordinates(const Ogre::Vector3& position, Ogre::Real& x, Ogre::Real& y);

			Ogre::String mName;

			Ogre::Overlay* mpOv;
			Ogre::BorderPanelOverlayElement * mpOvContainer;
			Ogre::BorderPanelOverlayElement * mpOvContainer2;
			Ogre::TextAreaOverlayElement* mpOvText;

			// true if mpOvContainer is visible, false otherwise
			bool mEnabled;

			// true if mTextWidth needs to be recalculated
			bool mNeedUpdate;

			float mX, mY, mZ;
			//
			Ogre::MovableObject* mpMov;

			// Text width in pixels
			Ogre::Real mTextWidth;

			// the Text
			Ogre::DisplayString mCaption;

			// true if the upper vertices projections of the -MovableObject are on screen
			bool mOnScreen;

			// the update frequency in seconds
			// mpOvContainer coordinates get updated each mUpdateFrequency seconds.
			Ogre::Real mUpdateFrequency;

			// the Font/Material/Color text attributes
			UMovableTextOverlayAttributes *mAttrs;

			int m_nID;
		};
	}
}

#endif /* __MovableTextOverlay_H__ */