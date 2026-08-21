#ifndef __UNE_CORE_CAMERA_H_INCLUDED__
#define __UNE_CORE_CAMERA_H_INCLUDED__

#pragma once

#include "CoreAPI.h"

#include "UMathAPI.h"
#include "UMatrix4.h"
#include "UVector3.h"
#include "UPlane.h"
#include "UQuaternion.h"
#include "UPlaneBoundedVolume.h"
#include "UFrustum.h"

namespace UnE
{
	namespace Core
	{
		class SceneNode 
		{

		};

		class UCoreObject;
		class CORE_API Camera : public Frustum
		{
		public:
			/** Listener interface so you can be notified of Camera events. 
			*/
			class CORE_API Listener 
			{
			public:
				Listener() {}
				virtual ~Listener() {}

				/// Called prior to the scene being rendered with this camera
				virtual void cameraPreRenderScene(Camera* cam)
							{ (void)cam; }

				/// Called after the scene has been rendered with this camera
				virtual void cameraPostRenderScene(Camera* cam)
							{ (void)cam; }

				/// Called when the camera is being destroyed
				virtual void cameraDestroyed(Camera* cam)
							{ (void)cam; }
			};
			typedef std::vector<Listener*> ListenerList;
		protected:	

			SceneNode* mParentNode;

			/// Camera orientation, quaternion style
			UnE::Math::Quaternion mOrientation;

			/// Camera position - default (0,0,0)
			UnE::Math::Vector3 mPosition;

			/// Derived orientation/position of the camera, including reflection
			mutable UnE::Math::Quaternion mDerivedOrientation;
			mutable UnE::Math::Vector3 mDerivedPosition;

			/// Real world orientation/position of the camera
			mutable UnE::Math::Quaternion mRealOrientation;
			mutable UnE::Math::Vector3 mRealPosition;

			/// Whether to yaw around a fixed axis.
			bool mYawFixed;
			/// Fixed axis to yaw around
			UnE::Math::Vector3 mYawFixedAxis;

			/// Stored number of visible faces in the last render
			unsigned int mVisFacesLastRender;

			/// Stored number of visible faces in the last render
			unsigned int mVisBatchesLastRender;

			/// Shared class-level name for Movable type
			static std::string msMovableType;

			/// SceneNode which this Camera will automatically track
			UObject* mAutoTrackTarget;
			/// Tracking offset for fine tuning
			UnE::Math::Vector3 mAutoTrackOffset;

			// Scene LOD factor used to adjust overall LOD
			Real mSceneLodFactor;
			/// Inverted scene LOD factor, can be used by Renderables to adjust their LOD
			Real mSceneLodFactorInv;


			/** Viewing window. 
			@remarks
			Generalize camera class for the case, when viewing frustum doesn't cover all viewport.
			*/
			Real mWLeft, mWTop, mWRight, mWBottom;
			/// Is viewing window used.
			bool mWindowSet;
			/// Windowed viewport clip planes 
			mutable std::vector<UnE::Math::Plane> mWindowClipPlanes;
			// Was viewing window changed.
			mutable bool mRecalcWindow;
		
			/** Whether aspect ratio will automatically be recalculated 
				when a viewport changes its size
			*/
			bool mAutoAspectRatio;
			/// Custom culling frustum
			Frustum *mCullFrustum;
		
			/// Whether or not the rendering distance of objects should take effect for this camera
			bool mUseRenderingDistance;
			
			/** Helper function for forwardIntersect that intersects rays with canonical plane */
			virtual std::vector<UnE::Math::Vector4> getRayForwardIntersect(
				const UnE::Math::Vector3& anchor, 
				const UnE::Math::Vector3 *dir, 
				Real planeOffset) const;

			ListenerList mListeners;

			//////////////////////////////////////////////////////////////////////////
			// internel vector/quat operation 
			void getPositionInternel();
			void getDerivedPositionInternel();
			void getRealPositionInternel();

			void getOrientationInternel();
			void getDerivedOrientationInternel();
			void getRealOrientationInternel();
			//////////////////////////////////////////////////////////////////////////

		public:
			/** Standard constructor.
			*/
			Camera( const std::string& name );

			/** Standard destructor.
			*/
			virtual ~Camera();

			/// Add a listener to this camera
			virtual void addListener(Listener* l);
			/// Remove a listener to this camera
			virtual void removeListener(Listener* l);

			//void setPolygonMode(PolygonMode sd);

			/** Retrieves the level of detail that the camera will render.
			*/
			//PolygonMode getPolygonMode(void) const;

			/** Sets the camera's position.
			*/
			void setPosition(Real x, Real y, Real z);

			/** Sets the camera's position.
			*/
			void setPosition(const UnE::Math::Vector3& vec);

			/** Retrieves the camera's position.
			*/
			const UnE::Math::Vector3& getPosition(void) const;

			/** Moves the camera's position by the vector offset provided along world axes.
			*/
			void move(const UnE::Math::Vector3& vec);

			/** Moves the camera's position by the vector offset provided along it's own axes (relative to orientation).
			*/
			void moveRelative(const UnE::Math::Vector3& vec);

			/** Sets the camera's direction vector.
				@remarks
					Note that the 'up' vector for the camera will automatically be recalculated based on the
					current 'up' vector (i.e. the roll will remain the same).
			*/
			void setDirection(Real x, Real y, Real z);

			/** Sets the camera's direction vector.
			*/
			void setDirection(const UnE::Math::Vector3& vec);

			/* Gets the camera's direction.
			*/
			UnE::Math::Vector3 getDirection(void) const;

			/** Gets the camera's up vector.
			*/
			UnE::Math::Vector3 getUp(void) const;

			/** Gets the camera's right vector.
			*/
			UnE::Math::Vector3 getRight(void) const;

			/** Points the camera at a location in worldspace.
				@remarks
					This is a helper method to automatically generate the
					direction vector for the camera, based on it's current position
					and the supplied look-at point.
				@param
					targetPoint A vector specifying the look at point.
			*/
			void lookAt( const UnE::Math::Vector3& targetPoint );
			/** Points the camera at a location in worldspace.
				@remarks
					This is a helper method to automatically generate the
					direction vector for the camera, based on it's current position
					and the supplied look-at point.
				@param
					x
				@param
					y
				@param
					z Co-ordinates of the point to look at.
			*/
			void lookAt(Real x, Real y, Real z);

			/** Rolls the camera anticlockwise, around its local z axis.
			*/
			void roll(const UnE::Math::Radian& angle);

			/** Rotates the camera anticlockwise around it's local y axis.
			*/
			void yaw(const UnE::Math::Radian& angle);

			/** Pitches the camera up/down anticlockwise around it's local z axis.
			*/
			void pitch(const UnE::Math::Radian& angle);

			/** Rotate the camera around an arbitrary axis.
			*/
			void rotate(const UnE::Math::Vector3& axis, const UnE::Math::Radian& angle);

			/** Rotate the camera around an arbitrary axis using a Quaternion.
			*/
			void rotate(const UnE::Math::Quaternion& q);

			/** Tells the camera whether to yaw around it's own local Y axis or a 
				fixed axis of choice.
				@remarks
					This method allows you to change the yaw behaviour of the camera
					- by default, the camera yaws around a fixed Y axis. This is 
					often what you want - for example if you're making a first-person 
					shooter, you really don't want the yaw axis to reflect the local 
					camera Y, because this would mean a different yaw axis if the 
					player is looking upwards rather than when they are looking
					straight ahead. You can change this behaviour by calling this 
					method, which you will want to do if you are making a completely
					free camera like the kind used in a flight simulator. 
				@param
					useFixed If true, the axis passed in the second parameter will 
					always be the yaw axis no matter what the camera orientation. 
					If false, the camera yaws around the local Y.
				@param
					fixedAxis The axis to use if the first parameter is true.
			*/
			void setFixedYawAxis( bool useFixed, const UnE::Math::Vector3& fixedAxis = UnE::Math::Vector3::UNIT_Y );


			/** Returns the camera's current orientation.
			*/
			const UnE::Math::Quaternion& getOrientation(void) const;

			/** Sets the camera's orientation.
			*/
			void setOrientation(const UnE::Math::Quaternion& q);

		
			/** Gets the derived orientation of the camera, including any
				rotation inherited from a node attachment and reflection matrix. */
			const UnE::Math::Quaternion& getDerivedOrientation(void) const;
			/** Gets the derived position of the camera, including any
				translation inherited from a node attachment and reflection matrix. */
			const UnE::Math::Vector3& getDerivedPosition(void) const;
			/** Gets the derived direction vector of the camera, including any
				rotation inherited from a node attachment and reflection matrix. */
			UnE::Math::Vector3 getDerivedDirection(void) const;
			/** Gets the derived up vector of the camera, including any
				rotation inherited from a node attachment and reflection matrix. */
			UnE::Math::Vector3 getDerivedUp(void) const;
			/** Gets the derived right vector of the camera, including any
				rotation inherited from a node attachment and reflection matrix. */
			UnE::Math::Vector3 getDerivedRight(void) const;

			/** Gets the real world orientation of the camera, including any
				rotation inherited from a node attachment */
			const UnE::Math::Quaternion& getRealOrientation(void) const;
			/** Gets the real world position of the camera, including any
				translation inherited from a node attachment. */
			const UnE::Math::Vector3& getRealPosition(void) const;
			/** Gets the real world direction vector of the camera, including any
				rotation inherited from a node attachment. */
			UnE::Math::Vector3 getRealDirection(void) const;
			/** Gets the real world up vector of the camera, including any
				rotation inherited from a node attachment. */
			UnE::Math::Vector3 getRealUp(void) const;
			/** Gets the real world right vector of the camera, including any
				rotation inherited from a node attachment. */
			UnE::Math::Vector3 getRealRight(void) const;

			/** Overridden from Frustum/Renderable */
			void getWorldTransforms(UnE::Math::Matrix4* mat) const;

			/** Overridden from MovableObject */
			const std::string& getMovableType(void) const;

			/** Enables / disables automatic tracking of a SceneNode.
			@remarks
				If you enable auto-tracking, this Camera will automatically rotate to
				look at the target SceneNode every frame, no matter how 
				it or SceneNode move. This is handy if you want a Camera to be focused on a
				single object or group of objects. Note that by default the Camera looks at the 
				origin of the SceneNode, if you want to tweak this, e.g. if the object which is
				attached to this target node is quite big and you want to point the camera at
				a specific point on it, provide a vector in the 'offset' parameter and the 
				camera's target point will be adjusted.
			@param enabled If true, the Camera will track the SceneNode supplied as the next 
				parameter (cannot be null). If false the camera will cease tracking and will
				remain in it's current orientation.
			@param target Pointer to the SceneNode which this Camera will track. Make sure you don't
				delete this SceneNode before turning off tracking (e.g. SceneManager::clearScene will
				delete it so be careful of this). Can be null if and only if the enabled param is false.
			@param offset If supplied, the camera targets this point in local space of the target node
				instead of the origin of the target node. Good for fine tuning the look at point.
			*/
			void setAutoTracking(bool enabled, UObject* const target = 0, 
				const UnE::Math::Vector3& offset = UnE::Math::Vector3::ZERO);


			/** Sets the level-of-detail factor for this Camera.
			@remarks
				This method can be used to influence the overall level of detail of the scenes 
				rendered using this camera. Various elements of the scene have level-of-detail
				reductions to improve rendering speed at distance; this method allows you 
				to hint to those elements that you would like to adjust the level of detail that
				they would normally use (up or down). 
			@par
				The most common use for this method is to reduce the overall level of detail used
				for a secondary camera used for sub viewports like rear-view mirrors etc.
				Note that scene elements are at liberty to ignore this setting if they choose,
				this is merely a hint.
			@param factor The factor to apply to the usual level of detail calculation. Higher
				values increase the detail, so 2.0 doubles the normal detail and 0.5 halves it.
			*/
			void setLodBias(Real factor = 1.0);

			/** Returns the level-of-detail bias factor currently applied to this camera. 
			@remarks
				See Camera::setLodBias for more details.
			*/
			Real getLodBias(void) const;

			/** Get a pointer to the camera which should be used to determine 
				LOD settings. 
			@remarks
				Sometimes you don't want the LOD of a render to be based on the camera
				that's doing the rendering, you want it to be based on a different
				camera. A good example is when rendering shadow maps, since they will 
				be viewed from the perspective of another camera. Therefore this method
				lets you associate a different camera instance to use to determine the LOD.
			@par
				To revert the camera to determining LOD based on itself, call this method with 
				a pointer to itself. 
			*/
			//virtual void setLodCamera(const Camera* lodCam);

			/** Get a pointer to the camera which should be used to determine 
				LOD settings. 
			@remarks
				If setLodCamera hasn't been called with a different camera, this
				method will return 'this'. 
			*/
			//virtual const Camera* getLodCamera() const;


			/** Gets a world space ray as cast from the camera through a viewport position.
			@param screenx, screeny The x and y position at which the ray should intersect the viewport, 
				in normalised screen coordinates [0,1]
			*/
			UnE::Math::Ray getCameraToViewportRay(Real screenx, Real screeny) const;
			/** Gets a world space ray as cast from the camera through a viewport position.
			@param screenx, screeny The x and y position at which the ray should intersect the viewport, 
				in normalised screen coordinates [0,1]
			@param outRay Ray instance to populate with result
			*/
			void getCameraToViewportRay(Real screenx, Real screeny, UnE::Math::Ray* outRay) const;

			/** Gets a world-space list of planes enclosing a volume based on a viewport
				rectangle. 
			@remarks
				Can be useful for populating a PlaneBoundedVolumeListSceneQuery, e.g. 
				for a rubber-band selection. 
			@param screenLeft, screenTop, screenRight, screenBottom The bounds of the
				on-screen rectangle, expressed in normalised screen coordinates [0,1]
			@param includeFarPlane If true, the volume is truncated by the camera far plane, 
				by default it is left open-ended
			*/
			UnE::Math::PlaneBoundedVolume getCameraToViewportBoxVolume(Real screenLeft, 
				Real screenTop, Real screenRight, Real screenBottom, bool includeFarPlane = false);

			/** Gets a world-space list of planes enclosing a volume based on a viewport
				rectangle. 
			@remarks
				Can be useful for populating a PlaneBoundedVolumeListSceneQuery, e.g. 
				for a rubber-band selection. 
			@param screenLeft, screenTop, screenRight, screenBottom The bounds of the
				on-screen rectangle, expressed in normalised screen coordinates [0,1]
			@param outVolume The plane list to populate with the result
			@param includeFarPlane If true, the volume is truncated by the camera far plane, 
				by default it is left open-ended
			*/
			void getCameraToViewportBoxVolume(Real screenLeft, 
				Real screenTop, Real screenRight, Real screenBottom, 
				UnE::Math::PlaneBoundedVolume* outVolume, bool includeFarPlane = false);

			/** Internal method for OGRE to use for LOD calculations. */
			Real _getLodBiasInverse(void) const;


			/** Internal method used by OGRE to update auto-tracking cameras. */
			void _autoTrack(void);


			/** Sets the viewing window inside of viewport.
			@remarks
			This method can be used to set a subset of the viewport as the rendering
			target. 
			@param Left Relative to Viewport - 0 corresponds to left edge, 1 - to right edge (default - 0).
			@param Top Relative to Viewport - 0 corresponds to top edge, 1 - to bottom edge (default - 0).
			@param Right Relative to Viewport - 0 corresponds to left edge, 1 - to right edge (default - 1).
			@param Bottom Relative to Viewport - 0 corresponds to top edge, 1 - to bottom edge (default - 1).
			*/
			virtual void setWindow (Real Left, Real Top, Real Right, Real Bottom);
			/// Cancel view window.
			virtual void resetWindow (void);
			/// Returns if a viewport window is being used
			virtual bool isWindowSet(void) const { return mWindowSet; }
			/// Gets the window clip planes, only applicable if isWindowSet == true
			const std::vector<UnE::Math::Plane>& getWindowPlanes(void) const;

			/** Overridden from MovableObject */
			Real getBoundingRadius(void) const;
			/** Get the auto tracking target for this camera, if any. */
			UObject* getAutoTrackTarget(void) const { return mAutoTrackTarget; }
			/** Get the auto tracking offset for this camera, if it is auto tracking. */
			const UnE::Math::Vector3& getAutoTrackOffset(void) const { return mAutoTrackOffset; }
		

			/** If set to true a viewport that owns this frustum will be able to 
				recalculate the aspect ratio whenever the frustum is resized.
			@remarks
				You should set this to true only if the frustum / camera is used by 
				one viewport at the same time. Otherwise the aspect ratio for other 
				viewports may be wrong.
			*/    
			void setAutoAspectRatio(bool autoratio);

			/** Retrieves if AutoAspectRatio is currently set or not
			*/
			bool getAutoAspectRatio(void) const;

			/** Tells the camera to use a separate Frustum instance to perform culling.
			@remarks
				By calling this method, you can tell the camera to perform culling
				against a different frustum to it's own. This is mostly useful for
				debug cameras that allow you to show the culling behaviour of another
				camera, or a manual frustum instance. 
			@param frustum Pointer to a frustum to use; this can either be a manual
				Frustum instance (which you can attach to scene nodes like any other
				MovableObject), or another camera. If you pass 0 to this method it
				reverts the camera to normal behaviour.
			*/
			//void setCullingFrustum(Frustum* frustum) { mCullFrustum = frustum; }
			/** Returns the custom culling frustum in use. */
			//Frustum* getCullingFrustum(void) const { return mCullFrustum; }

			/** Forward projects frustum rays to find forward intersection with plane.
				@remarks
				Forward projection may lead to intersections at infinity.
			*/
			virtual void forwardIntersect(const UnE::Math::Plane& worldPlane, std::vector<UnE::Math::Vector4>* intersect3d) const;

			/// @copydoc Frustum::isVisible
			bool isVisible(const UnE::Math::AxisAlignedBox& bound, FrustumPlane* culledBy = 0) const;
			/// @copydoc Frustum::isVisible
			bool isVisible(const UnE::Math::Sphere& bound, FrustumPlane* culledBy = 0) const;
			/// @copydoc Frustum::isVisible
			bool isVisible(const UnE::Math::Vector3& vert, FrustumPlane* culledBy = 0) const;
			/// @copydoc Frustum::getWorldSpaceCorners
			const UnE::Math::Vector3* getWorldSpaceCorners(void) const;
			/// @copydoc Frustum::getFrustumPlane
			const UnE::Math::Plane& getFrustumPlane( unsigned short plane ) const;
			/// @copydoc Frustum::projectSphere
			bool projectSphere(const UnE::Math::Sphere& sphere, 
				Real* left, Real* top, Real* right, Real* bottom) const;
			/// @copydoc Frustum::getNearClipDistance
			Real getNearClipDistance(void) const;
			/// @copydoc Frustum::getFarClipDistance
			Real getFarClipDistance(void) const;
			/// @copydoc Frustum::getViewMatrix
			const UnE::Math::Matrix4& getViewMatrix(void) const;
			/** Specialised version of getViewMatrix allowing caller to differentiate
				whether the custom culling frustum should be allowed or not. 
			@remarks
				The default behaviour of the standard getViewMatrix is to delegate to 
				the alternate culling frustum, if it is set. This is expected when 
				performing CPU calculations, but the final rendering must be performed
				using the real view matrix in order to display the correct debug view.
			*/
			const UnE::Math::Matrix4& getViewMatrix(bool ownFrustumOnly) const;
			/** Set whether this camera should use the 'rendering distance' on
				objects to exclude distant objects from the final image. The
				default behaviour is to use it.
			@param use True to use the rendering distance, false not to.
			*/
			virtual void setUseRenderingDistance(bool use) { mUseRenderingDistance = use; }
			/** Get whether this camera should use the 'rendering distance' on
				objects to exclude distant objects from the final image.
			*/
			virtual bool getUseRenderingDistance(void) const { return mUseRenderingDistance; }

			/** Synchronise core camera settings with another. 
			@remarks
				Copies the position, orientation, clip distances, projection type, 
				FOV, focal length and aspect ratio from another camera. Other settings like query flags, 
				reflection etc are preserved.
			*/
			virtual void synchroniseBaseSettingsWith(const Camera* cam);

			/** Get the derived position of this frustum. */
			const UnE::Math::Vector3& getPositionForViewUpdate(void) const;
			/** Get the derived orientation of this frustum. */
			const UnE::Math::Quaternion& getOrientationForViewUpdate(void) const;
		
			void setWindowImpl() const;


			/** @brief 
					Sets whether to use min display size calculations 
				When active objects who's size on the screen is less then a given number will not
				be rendered.
			*/
			//void setUseMinPixelSize(bool enable) { mUseMinPixelSize = enable; }
			/** Returns whether to use min display size calculations 
				@see Camera::setUseMinDisplaySize
			*/
			//bool getUseMinPixelSize() const { return mUseMinPixelSize; }

			/** Returns an estimated ratio between a pixel and the display area it represents.
				For orthographic cameras this function returns the amount of meters covered by
				a single pixel along the vertical axis. For perspective cameras the value
				returned is the amount of meters covered by a single pixel per meter distance 
				from the camera.
			@note
				This parameter is calculated just before the camera is rendered
			@note
				This parameter is used in min display size calculations.
			*/
			//Real getPixelDisplayRatio() const { return mPixelDisplayRatio; }
		
		}; 
		/** @} */
	} // namespace Core
} // namespace UnE

#endif
