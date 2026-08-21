#include "stdafx.h"

#include <limits>
#include <vector>
#include <string>

//////////////////////////////////////////////////////////////////////////
// Ogre headers
#include <OgreCommon.h>
#include <OgreException.h>
#include <OgreConfigFile.h>
#include <OgreRoot.h>
#include <OgreCamera.h>
#include <OgreViewport.h>
#include <OgreSceneManager.h>
#include <OgreRenderWindow.h>
#include <OgreEntity.h>
#include <OgreWindowEventUtilities.h>
#include <OgreLogManager.h>
#include <OgreRenderSystem.h>
#include <OgreResourceBackgroundQueue.h>
#include <OgreManualObject.h>
#include <OgreStaticGeometry.h>
#include <OgreMeshManager.h>

#include <OgreSubMesh.h>
#include <OgreRay.h>
#include <OgreMaterialManager.h>


//////////////////////////////////////////////////////////////////////////
// Core API
#include "UCamera.h"
#include "UMathAPI.h"
#include "UMath.h"
#include "UMatrix3.h"
#include "UAxisAlignedBox.h"
#include "USphere.h"
#include "URay.h"
#include "UPlaneBoundedVolume.h"
#include "UFrustum.h"



using namespace UnE::Math;

namespace UnE
{
	namespace Core
	{
		std::string Camera::msMovableType = "Camera";
		//-----------------------------------------------------------------------
		Camera::Camera( const std::string& name)
			: Frustum(name),
			mOrientation(Quaternion::IDENTITY),
			mPosition(Vector3::ZERO),
			//mSceneDetail(PM_SOLID),
			mAutoTrackTarget(0),
			mAutoTrackOffset(Vector3::ZERO),
			mSceneLodFactor(1.0f),
			mSceneLodFactorInv(1.0f),
			mWindowSet(false),
			//mLastViewport(0),
			mAutoAspectRatio(false),
			//mCullFrustum(0),
			mUseRenderingDistance(true)
			//mLodCamera(0)
			//mUseMinPixelSize(false),
			//mPixelDisplayRatio(0)
		{

			// Reasonable defaults to camera params
			mFOVy = Radian(UMath::PI/4.0f);
			mNearDist = 100.0f;
			mFarDist = 100000.0f;
			mAspect = 1.33333333333333f;
			mProjType = PT_PERSPECTIVE;
						
			mYawFixed = true;
			mYawFixedAxis = Vector3::UNIT_Y;		

			// Init matrices
			mViewMatrix = Matrix4::ZERO;
			mProjMatrixRS = Matrix4::ZERO;

			//mParentNode = 0;

			// no reflection
			mReflect = false;

			//mVisible = false;
		}

		//-----------------------------------------------------------------------
		Camera::~Camera()
		{			
		}		
		//-----------------------------------------------------------------------
		void Camera::setPosition(Real x, Real y, Real z)
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			mPosition.x = x;
			mPosition.y = y;
			mPosition.z = z;
			pCam->setPosition(x, y, z);
		}

		//-----------------------------------------------------------------------
		void Camera::setPosition(const Vector3& vec)
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			mPosition = vec;
			pCam->setPosition(mPosition.x, mPosition.y, mPosition.z);
		}

		//-----------------------------------------------------------------------
		const Vector3& Camera::getPosition(void) const
		{
			Camera * pCam =(Camera*) this;
			pCam->getPositionInternel();
			return mPosition;
		}

		void Camera::getPositionInternel()
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Vector3 vPos = pCam->getPosition();
			mPosition.x = vPos.x;
			mPosition.y = vPos.y;
			mPosition.z = vPos.z;
		}

		//-----------------------------------------------------------------------
		void Camera::move(const Vector3& vec)
		{
			mPosition = mPosition + vec;
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->move(Ogre::Vector3(mPosition.x, mPosition.y, mPosition.z));				
		}

		//-----------------------------------------------------------------------
		void Camera::moveRelative(const Vector3& vec)
		{
			// Transform the axes of the relative vector by camera's local axes
			mOrientation = getOrientation();
			Vector3 trans = mOrientation * vec;
			move(trans);
		}

		//-----------------------------------------------------------------------
		void Camera::setDirection(Real x, Real y, Real z)
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->setDirection(Ogre::Vector3(x,y,z));
		}

		//-----------------------------------------------------------------------
		void Camera::setDirection(const Vector3& vec)
		{
			setDirection(vec.x, vec.y, vec.z);
		}


		//-----------------------------------------------------------------------
		Vector3 Camera::getDirection(void) const
		{			
			// Direction points down -Z by default
			return getOrientation() * -Vector3::UNIT_Z;
		}

		//-----------------------------------------------------------------------
		Vector3 Camera::getUp(void) const
		{
			return getOrientation() * Vector3::UNIT_Y;
		}

		//-----------------------------------------------------------------------
		Vector3 Camera::getRight(void) const
		{
			return getOrientation() * Vector3::UNIT_X;
		}

		//-----------------------------------------------------------------------
		void Camera::lookAt(const Vector3& targetPoint)
		{			
			lookAt(targetPoint.x, targetPoint.y, targetPoint.z);
		}

		//-----------------------------------------------------------------------
		void Camera::lookAt( Real x, Real y, Real z )
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->lookAt(Ogre::Vector3(x, y, z));			
		}

		//-----------------------------------------------------------------------
		void Camera::roll(const Radian& angle)
		{
			// Rotate around local Z axis
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->roll(Ogre::Radian(angle.valueRadians()));		
		}

		//-----------------------------------------------------------------------
		void Camera::yaw(const Radian& angle)
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->yaw(Ogre::Radian(angle.valueRadians()));		
		}

		//-----------------------------------------------------------------------
		void Camera::pitch(const Radian& angle)
		{
			// Rotate around local X axis
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->pitch(Ogre::Radian(angle.valueRadians()));
		}

		//-----------------------------------------------------------------------
		void Camera::rotate(const Vector3& axis, const Radian& angle)
		{	
			Quaternion q;
			q.FromAngleAxis(angle,axis);
			rotate(q);
		}

		//-----------------------------------------------------------------------
		void Camera::rotate(const Quaternion& q)
		{
			// Note the order of the mult, i.e. q comes after

			// Normalise the quat to avoid cumulative problems with precision
			Ogre::Quaternion qx( q.w, q.x, q.y, q.z);
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->rotate(qx);
		}
		

	
		//---------------------------------------------------------------------
		void Camera::addListener(Listener* l)
		{
			if (std::find(mListeners.begin(), mListeners.end(), l) == mListeners.end())
				mListeners.push_back(l);
		}
		//---------------------------------------------------------------------
		void Camera::removeListener(Listener* l)
		{
			ListenerList::iterator i = std::find(mListeners.begin(), mListeners.end(), l);
			if (i != mListeners.end())
				mListeners.erase(i);
		}
		
		//-----------------------------------------------------------------------
		void Camera::setFixedYawAxis(bool useFixed, const Vector3& fixedAxis)
		{
			mYawFixed = useFixed;
			mYawFixedAxis = fixedAxis;
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			pCam->setFixedYawAxis(useFixed, Ogre::Vector3(fixedAxis.x, fixedAxis.y, fixedAxis.z));
		}

		//-----------------------------------------------------------------------
		const Quaternion& Camera::getOrientation(void) const
		{
			Camera * pCam = (Camera*)this;
			pCam->getOrientationInternel();
			return mOrientation;
		}
		
		//-----------------------------------------------------------------------
		void Camera::getOrientationInternel()
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Quaternion q = pCam->getOrientation();
			mOrientation.x = q.x;
			mOrientation.y = q.y;
			mOrientation.z = q.z;
			mOrientation.w = q.w;
		}

		//-----------------------------------------------------------------------
		void Camera::setOrientation(const Quaternion& q)
		{
			mOrientation = q;
			mOrientation.normalise();			
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Quaternion qx(mOrientation.x, mOrientation.y,mOrientation.z,mOrientation.w);
			pCam->setOrientation(qx);
		}

		//-----------------------------------------------------------------------
		const Quaternion& Camera::getDerivedOrientation(void) const
		{		
			Camera * pCam = (Camera*)this;
			pCam->getDerivedOrientationInternel();
			return mDerivedOrientation;
		}
		
	    //-----------------------------------------------------------------------
		void Core::Camera::getDerivedOrientationInternel()
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Quaternion q = pCam->getDerivedOrientation();
			mDerivedOrientation.x = q.x;
			mDerivedOrientation.y = q.y;
			mDerivedOrientation.z = q.z;
			mDerivedOrientation.w = q.w;
		}


		//-----------------------------------------------------------------------
		const Vector3& Camera::getDerivedPosition(void) const
		{
			Camera * pCam = (Camera*)this;
			pCam->getDerivedPositionInternel();
			return mDerivedPosition;
		}
		
		//-----------------------------------------------------------------------
		void Core::Camera::getDerivedPositionInternel()
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Vector3 q = pCam->getDerivedPosition();
			mDerivedPosition.x = q.x;
			mDerivedPosition.y = q.y;
			mDerivedPosition.z = q.z;

		}

		//-----------------------------------------------------------------------
		Vector3 Camera::getDerivedDirection(void) const
		{
			// Direction points down -Z
			mDerivedOrientation = getDerivedOrientation();
			return mDerivedOrientation * Vector3::NEGATIVE_UNIT_Z;
		}
		//-----------------------------------------------------------------------
		Vector3 Camera::getDerivedUp(void) const
		{
			mDerivedOrientation = getDerivedOrientation();
			return mDerivedOrientation * Vector3::UNIT_Y;
		}
		//-----------------------------------------------------------------------
		Vector3 Camera::getDerivedRight(void) const
		{
			mDerivedOrientation = getDerivedOrientation();
			return mDerivedOrientation * Vector3::UNIT_X;
		}
		//-----------------------------------------------------------------------
		const Quaternion& Camera::getRealOrientation(void) const
		{
			Camera * pCam = (Camera*)this;
			pCam->getRealOrientationInternel();
			return mRealOrientation;
		}
		//-----------------------------------------------------------------------
		void Core::Camera::getRealOrientationInternel()
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Quaternion q = pCam->getRealOrientation();
			mRealOrientation.x = q.x;
			mRealOrientation.y = q.y;
			mRealOrientation.z = q.z;
			mRealOrientation.w = q.w;
		}

		//-----------------------------------------------------------------------
		const Vector3& Camera::getRealPosition(void) const
		{
			Camera * pCam = (Camera*)this;
			pCam->getRealPositionInternel();
			return mRealPosition;
		}

		//-----------------------------------------------------------------------
		void Camera::getRealPositionInternel()
		{
			Ogre::Camera * pCam = (Ogre::Camera *)pInternal;
			Ogre::Vector3 q = pCam->getRealPosition();
			mRealPosition.x = q.x;
			mRealPosition.y = q.y;
			mRealPosition.z = q.z;
		}
		//-----------------------------------------------------------------------
		Vector3 Camera::getRealDirection(void) const
		{
			// Direction points down -Z
			updateView();
			return mRealOrientation * Vector3::NEGATIVE_UNIT_Z;
		}
		//-----------------------------------------------------------------------
		Vector3 Camera::getRealUp(void) const
		{
			mRealOrientation = getRealOrientation();
			return mRealOrientation * Vector3::UNIT_Y;
		}
		//-----------------------------------------------------------------------
		Vector3 Camera::getRealRight(void) const
		{
			mRealOrientation = getRealOrientation();
			return mRealOrientation * Vector3::UNIT_X;
		}
		//-----------------------------------------------------------------------
		void Camera::getWorldTransforms(Matrix4* mat) const 
		{
			updateView();

			Vector3 scale(1.0, 1.0, 1.0);
			//if (mParentNode)
			//	scale = mParentNode->_getDerivedScale();

			mat->makeTransform(
				mDerivedPosition,
				scale,
				mDerivedOrientation);
		}
		//-----------------------------------------------------------------------
		const std::string& Camera::getMovableType(void) const
		{
			return msMovableType;
		}
		//-----------------------------------------------------------------------
		/*void Camera::setAutoTracking(bool enabled, SceneNode* const target,  const Vector3& offset)
		{
			if (enabled)
			{
				assert (target != 0 && "target cannot be a null pointer if tracking is enabled");
				mAutoTrackTarget = target;
				mAutoTrackOffset = offset;
			}
			else
			{
				mAutoTrackTarget = 0;
			}
		}*/
		//-----------------------------------------------------------------------
		void Camera::_autoTrack(void)
		{
			// NB assumes that all scene nodes have been updated
			if (mAutoTrackTarget)
			{
				//lookAt(mAutoTrackTarget->_getDerivedPosition() + mAutoTrackOffset);
			}
		}
		//-----------------------------------------------------------------------
		void Camera::setLodBias(Real factor)
		{
			assert(factor > 0.0f && "Bias factor must be > 0!");
			mSceneLodFactor = factor;
			mSceneLodFactorInv = 1.0f / factor;
		}
		//-----------------------------------------------------------------------
		Real Camera::getLodBias(void) const
		{
			return mSceneLodFactor;
		}
		//-----------------------------------------------------------------------
		Real Camera::_getLodBiasInverse(void) const
		{
			return mSceneLodFactorInv;
		}
		//-----------------------------------------------------------------------
		/*void Camera::setLodCamera(const Camera* lodCam)
		{
			if (lodCam == this)
				mLodCamera = 0;
			else
				mLodCamera = lodCam;
		}*/
		//---------------------------------------------------------------------
		/*const Camera* Camera::getLodCamera() const
		{
			return mLodCamera? mLodCamera : this;
		}*/
		//-----------------------------------------------------------------------
		Ray Camera::getCameraToViewportRay(Real screenX, Real screenY) const
		{
			Ray ret;
			getCameraToViewportRay(screenX, screenY, &ret);
			return ret;
		}
		//---------------------------------------------------------------------
		void Camera::getCameraToViewportRay(Real screenX, Real screenY, Ray* outRay) const
		{
			Matrix4 inverseVP = (getProjectionMatrix() * getViewMatrix(true)).inverse();

#if OGRE_NO_VIEWPORT_ORIENTATIONMODE == 0
			// We need to convert screen point to our oriented viewport (temp solution)
			Real tX = screenX; Real a = getOrientationMode() * UMath::HALF_PI;
			screenX = UMath::Cos(a) * (tX-0.5f) + UMath::Sin(a) * (screenY-0.5f) + 0.5f;
			screenY = UMath::Sin(a) * (tX-0.5f) + UMath::Cos(a) * (screenY-0.5f) + 0.5f;
			if ((int)getOrientationMode()&1) screenY = 1.f - screenY;
#endif

			Real nx = (2.0f * screenX) - 1.0f;
			Real ny = 1.0f - (2.0f * screenY);
			Vector3 nearPoint(nx, ny, -1.f);
			// Use midPoint rather than far point to avoid issues with infinite projection
			Vector3 midPoint (nx, ny,  0.0f);

			// Get ray origin and ray target on near plane in world space
			Vector3 rayOrigin, rayTarget;

			rayOrigin = inverseVP * nearPoint;
			rayTarget = inverseVP * midPoint;

			Vector3 rayDirection = rayTarget - rayOrigin;
			rayDirection.normalise();

			outRay->setOrigin(rayOrigin);
			outRay->setDirection(rayDirection);
		} 
		//---------------------------------------------------------------------
		PlaneBoundedVolume Camera::getCameraToViewportBoxVolume(Real screenLeft, 
			Real screenTop, Real screenRight, Real screenBottom, bool includeFarPlane)
		{
			PlaneBoundedVolume vol;
			getCameraToViewportBoxVolume(screenLeft, screenTop, screenRight, screenBottom, 
				&vol, includeFarPlane);
			return vol;

		}
		//---------------------------------------------------------------------()
		void Camera::getCameraToViewportBoxVolume(Real screenLeft, 
			Real screenTop, Real screenRight, Real screenBottom, 
			PlaneBoundedVolume* outVolume, bool includeFarPlane)
		{
			outVolume->planes.clear();

			if (mProjType == PT_PERSPECTIVE)
			{

				// Use the corner rays to generate planes
				Ray ul = getCameraToViewportRay(screenLeft, screenTop);
				Ray ur = getCameraToViewportRay(screenRight, screenTop);
				Ray bl = getCameraToViewportRay(screenLeft, screenBottom);
				Ray br = getCameraToViewportRay(screenRight, screenBottom);


				Vector3 normal;
				// top plane
				normal = ul.getDirection().crossProduct(ur.getDirection());
				normal.normalise();
				outVolume->planes.push_back(
					Plane(normal, getDerivedPosition()));

				// right plane
				normal = ur.getDirection().crossProduct(br.getDirection());
				normal.normalise();
				outVolume->planes.push_back(
					Plane(normal, getDerivedPosition()));

				// bottom plane
				normal = br.getDirection().crossProduct(bl.getDirection());
				normal.normalise();
				outVolume->planes.push_back(
					Plane(normal, getDerivedPosition()));

				// left plane
				normal = bl.getDirection().crossProduct(ul.getDirection());
				normal.normalise();
				outVolume->planes.push_back(
					Plane(normal, getDerivedPosition()));

			}
			else
			{
				// ortho planes are parallel to frustum planes

				Ray ul = getCameraToViewportRay(screenLeft, screenTop);
				Ray br = getCameraToViewportRay(screenRight, screenBottom);

				updateFrustumPlanes();
				outVolume->planes.push_back(
					Plane(mFrustumPlanes[FRUSTUM_PLANE_TOP].normal, ul.getOrigin()));
				outVolume->planes.push_back(
					Plane(mFrustumPlanes[FRUSTUM_PLANE_RIGHT].normal, br.getOrigin()));
				outVolume->planes.push_back(
					Plane(mFrustumPlanes[FRUSTUM_PLANE_BOTTOM].normal, br.getOrigin()));
				outVolume->planes.push_back(
					Plane(mFrustumPlanes[FRUSTUM_PLANE_LEFT].normal, ul.getOrigin()));


			}

			// near & far plane applicable to both projection types
			outVolume->planes.push_back(getFrustumPlane(FRUSTUM_PLANE_NEAR));
			if (includeFarPlane)
				outVolume->planes.push_back(getFrustumPlane(FRUSTUM_PLANE_FAR));
		}
		// -------------------------------------------------------------------
		void Camera::setWindow (Real Left, Real Top, Real Right, Real Bottom)
		{
			mWLeft = Left;
			mWTop = Top;
			mWRight = Right;
			mWBottom = Bottom;

			mWindowSet = true;
			mRecalcWindow = true;
		}
		// -------------------------------------------------------------------
		void Camera::resetWindow ()
		{
			mWindowSet = false;
		}
		// -------------------------------------------------------------------
		void Camera::setWindowImpl() const
		{
			if (!mWindowSet || !mRecalcWindow)
				return;

			// Calculate general projection parameters
			Real vpLeft, vpRight, vpBottom, vpTop;
			calcProjectionParameters(vpLeft, vpRight, vpBottom, vpTop);

			Real vpWidth = vpRight - vpLeft;
			Real vpHeight = vpTop - vpBottom;

			Real wvpLeft   = vpLeft + mWLeft * vpWidth;
			Real wvpRight  = vpLeft + mWRight * vpWidth;
			Real wvpTop    = vpTop - mWTop * vpHeight;
			Real wvpBottom = vpTop - mWBottom * vpHeight;

			Vector3 vp_ul (wvpLeft, wvpTop, -mNearDist);
			Vector3 vp_ur (wvpRight, wvpTop, -mNearDist);
			Vector3 vp_bl (wvpLeft, wvpBottom, -mNearDist);
			Vector3 vp_br (wvpRight, wvpBottom, -mNearDist);

			Matrix4 inv = mViewMatrix.inverseAffine();

			Vector3 vw_ul = inv.transformAffine(vp_ul);
			Vector3 vw_ur = inv.transformAffine(vp_ur);
			Vector3 vw_bl = inv.transformAffine(vp_bl);
			Vector3 vw_br = inv.transformAffine(vp_br);

			mWindowClipPlanes.clear();
			if (mProjType == PT_PERSPECTIVE)
			{
				Vector3 position = getPositionForViewUpdate();
				mWindowClipPlanes.push_back(Plane(position, vw_bl, vw_ul));
				mWindowClipPlanes.push_back(Plane(position, vw_ul, vw_ur));
				mWindowClipPlanes.push_back(Plane(position, vw_ur, vw_br));
				mWindowClipPlanes.push_back(Plane(position, vw_br, vw_bl));
			}
			else
			{
				Vector3 x_axis(inv[0][0], inv[0][1], inv[0][2]);
				Vector3 y_axis(inv[1][0], inv[1][1], inv[1][2]);
				x_axis.normalise();
				y_axis.normalise();
				mWindowClipPlanes.push_back(Plane( x_axis, vw_bl));
				mWindowClipPlanes.push_back(Plane(-x_axis, vw_ur));
				mWindowClipPlanes.push_back(Plane( y_axis, vw_bl));
				mWindowClipPlanes.push_back(Plane(-y_axis, vw_ur));
			}

			mRecalcWindow = false;

		}
		// -------------------------------------------------------------------
		const std::vector<Plane>& Camera::getWindowPlanes(void) const
		{
			updateView();
			setWindowImpl();
			return mWindowClipPlanes;
		}
		// -------------------------------------------------------------------
		Real Camera::getBoundingRadius(void) const
		{
			// return a little bigger than the near distance
			// just to keep things just outside
			return mNearDist * 1.5f;

		}
		//-----------------------------------------------------------------------
		const Vector3& Camera::getPositionForViewUpdate(void) const
		{
			// Note no update, because we're calling this from the update!
			return mRealPosition;
		}
		//-----------------------------------------------------------------------
		const Quaternion& Camera::getOrientationForViewUpdate(void) const
		{
			return mRealOrientation;
		}
		//-----------------------------------------------------------------------
		bool Camera::getAutoAspectRatio(void) const
		{
			return mAutoAspectRatio;
		}
		//-----------------------------------------------------------------------
		void Camera::setAutoAspectRatio(bool autoratio)
		{
			mAutoAspectRatio = autoratio;
		}
		//-----------------------------------------------------------------------
		bool Camera::isVisible(const AxisAlignedBox& bound, FrustumPlane* culledBy) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->isVisible(bound, culledBy);
			}
			else
			{
				return Frustum::isVisible(bound, culledBy);
			}
		}
		//-----------------------------------------------------------------------
		bool Camera::isVisible(const Sphere& bound, FrustumPlane* culledBy) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->isVisible(bound, culledBy);
			}
			else
			{
				return Frustum::isVisible(bound, culledBy);
			}
		}
		//-----------------------------------------------------------------------
		bool Camera::isVisible(const Vector3& vert, FrustumPlane* culledBy) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->isVisible(vert, culledBy);
			}
			else
			{
				return Frustum::isVisible(vert, culledBy);
			}
		}
		//-----------------------------------------------------------------------
		const Vector3* Camera::getWorldSpaceCorners(void) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->getWorldSpaceCorners();
			}
			else
			{
				return Frustum::getWorldSpaceCorners();
			}
		}
		//-----------------------------------------------------------------------
		const Plane& Camera::getFrustumPlane( unsigned short plane ) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->getFrustumPlane(plane);
			}
			else
			{
				return Frustum::getFrustumPlane(plane);
			}
		}
		//-----------------------------------------------------------------------
		bool Camera::projectSphere(const Sphere& sphere, 
			Real* left, Real* top, Real* right, Real* bottom) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->projectSphere(sphere, left, top, right, bottom);
			}
			else
			{
				return Frustum::projectSphere(sphere, left, top, right, bottom);
			}
		}
		//-----------------------------------------------------------------------
		Real Camera::getNearClipDistance(void) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->getNearClipDistance();
			}
			else
			{
				return Frustum::getNearClipDistance();
			}
		}
		//-----------------------------------------------------------------------
		Real Camera::getFarClipDistance(void) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->getFarClipDistance();
			}
			else
			{
				return Frustum::getFarClipDistance();
			}
		}
		//-----------------------------------------------------------------------
		const Matrix4& Camera::getViewMatrix(void) const
		{
			if (mCullFrustum)
			{
				return mCullFrustum->getViewMatrix();
			}
			else
			{
				return Frustum::getViewMatrix();
			}
		}
		//-----------------------------------------------------------------------
		const Matrix4& Camera::getViewMatrix(bool ownFrustumOnly) const
		{
			if (ownFrustumOnly)
			{
				return Frustum::getViewMatrix();
			}
			else
			{
				return getViewMatrix();
			}
		}
		//-----------------------------------------------------------------------
		//_______________________________________________________
		//|														|
		//|	getRayForwardIntersect								|
		//|	-----------------------------						|
		//|	get the intersections of frustum rays with a plane	|
		//| of interest.  The plane is assumed to have constant	|
		//| z.  If this is not the case, rays					|
		//| should be rotated beforehand to work in a			|
		//| coordinate system in which this is true.			|
		//|_____________________________________________________|
		//
		std::vector<Vector4> Camera::getRayForwardIntersect(const Vector3& anchor, const Vector3 *dir, Real planeOffset) const
		{
			std::vector<Vector4> res;

			if(!dir)
				return res;

			int infpt[4] = {0, 0, 0, 0}; // 0=finite, 1=infinite, 2=straddles infinity
			Vector3 vec[4];

			// find how much the anchor point must be displaced in the plane's
			// constant variable
			Real delta = planeOffset - anchor.z;

			// now set the intersection point and note whether it is a 
			// point at infinity or straddles infinity
			unsigned int i;
			for (i=0; i<4; i++)
			{
				Real test = dir[i].z * delta;
				if (test == 0.0) {
					vec[i] = dir[i];
					infpt[i] = 1;
				}
				else {
					Real lambda = delta / dir[i].z;
					vec[i] = anchor + (lambda * dir[i]);
					if(test < 0.0)
						infpt[i] = 2;
				}
			}

			for (i=0; i<4; i++)
			{
				// store the finite intersection points
				if (infpt[i] == 0)
					res.push_back(Vector4(vec[i].x, vec[i].y, vec[i].z, 1.0));
				else
				{
					// handle the infinite points of intersection;
					// cases split up into the possible frustum planes 
					// pieces which may contain a finite intersection point
					int nextind = (i+1) % 4;
					int prevind = (i+3) % 4;
					if ((infpt[prevind] == 0) || (infpt[nextind] == 0))
					{
						if (infpt[i] == 1)
							res.push_back(Vector4(vec[i].x, vec[i].y, vec[i].z, 0.0));
						else
						{
							// handle the intersection points that straddle infinity (back-project)
							if(infpt[prevind] == 0) 
							{
								Vector3 temp = vec[prevind] - vec[i];
								res.push_back(Vector4(temp.x, temp.y, temp.z, 0.0));
							}
							if(infpt[nextind] == 0)
							{
								Vector3 temp = vec[nextind] - vec[i];
								res.push_back(Vector4(temp.x, temp.y, temp.z, 0.0));
							}
						}
					} // end if we need to add an intersection point to the list
				} // end if infinite point needs to be considered
			} // end loop over frustun corners

			// we end up with either 0, 3, 4, or 5 intersection points

			return res;
		}

		//_______________________________________________________
		//|														|
		//|	forwardIntersect									|
		//|	-----------------------------						|
		//|	Forward intersect the camera's frustum rays with	|
		//| a specified plane of interest.						|
		//| Note that if the frustum rays shoot out and would	|
		//| back project onto the plane, this means the forward	|
		//| intersection of the frustum would occur at the		|
		//| line at infinity.									|
		//|_____________________________________________________|
		//
		void Camera::forwardIntersect(const Plane& worldPlane, std::vector<Vector4>* intersect3d) const
		{
			if(!intersect3d)
				return;

			Vector3 trCorner = getWorldSpaceCorners()[0];
			Vector3 tlCorner = getWorldSpaceCorners()[1];
			Vector3 blCorner = getWorldSpaceCorners()[2];
			Vector3 brCorner = getWorldSpaceCorners()[3];

			// need some sort of rotation that will bring the plane normal to the z axis
			Plane pval = worldPlane;
			if(pval.normal.z < 0.0)
			{
				pval.normal *= -1.0;
				pval.d *= -1.0;
			}
			Quaternion invPlaneRot = pval.normal.getRotationTo(Vector3::UNIT_Z);

			// get rotated light
			Vector3 lPos = invPlaneRot * getDerivedPosition();
			Vector3 vec[4];
			vec[0] = invPlaneRot * trCorner - lPos;
			vec[1] = invPlaneRot * tlCorner - lPos; 
			vec[2] = invPlaneRot * blCorner - lPos; 
			vec[3] = invPlaneRot * brCorner - lPos; 

			// compute intersection points on plane
			std::vector<Vector4> iPnt = getRayForwardIntersect(lPos, vec, -pval.d);


			// return wanted data
			if(intersect3d) 
			{
				Quaternion planeRot = invPlaneRot.Inverse();
				(*intersect3d).clear();
				for(unsigned int i=0; i<iPnt.size(); i++)
				{
					Vector3 intersection = planeRot * Vector3(iPnt[i].x, iPnt[i].y, iPnt[i].z);
					(*intersect3d).push_back(Vector4(intersection.x, intersection.y, intersection.z, iPnt[i].w));
				}
			}
		}
		//-----------------------------------------------------------------------
		void Camera::synchroniseBaseSettingsWith(const Camera* cam)
		{
			setPosition(cam->getPosition());
			setProjectionType(cam->getProjectionType());
			setOrientation(cam->getOrientation());
			setAspectRatio(cam->getAspectRatio());
			setNearClipDistance(cam->getNearClipDistance());
			setFarClipDistance(cam->getFarClipDistance());
			setUseRenderingDistance(cam->getUseRenderingDistance());
			setFOVy(cam->getFOVy());
			setFocalLength(cam->getFocalLength());

			// Don't do these, they're not base settings and can cause referencing issues
			//this->setLodCamera(cam->getLodCamera());
			//this->setCullingFrustum(cam->getCullingFrustum());

		}



	} // namespace core
} // namespace Ogre
