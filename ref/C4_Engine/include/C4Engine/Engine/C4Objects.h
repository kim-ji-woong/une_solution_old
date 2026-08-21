//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4Objects_h
#define C4Objects_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Construction.h"
#include "C4Configurable.h"


namespace C4
{
	//# \tree	Object
	//
	//# \node	GraphicsMgr/MaterialObject
	//
	//# \node	GraphicsMgr/CameraObject
	//# \sub
	//#		\node	GraphicsMgr/OrthoCameraObject
	//#		\node	GraphicsMgr/FrustumCameraObject
	//#		\sub
	//#			\node	GraphicsMgr/RemoteCameraObject
	//#		\end
	//# \end
	//
	//# \node	GraphicsMgr/LightObject
	//# \sub
	//#		\node	GraphicsMgr/InfiniteLightObject
	//#		\sub
	//#			\node	GraphicsMgr/DepthLightObject
	//#			\node	GraphicsMgr/LandscapeLightObject
	//#		\end
	//#		\node	GraphicsMgr/PointLightObject
	//#		\sub
	//#			\node	GraphicsMgr/ShadowLightObject
	//#			\sub
	//#				\node	GraphicsMgr/CubeLightObject
	//#				\node	GraphicsMgr/SpotLightObject
	//#			\end
	//#		\end
	//# \end
	//
	//# \node	SpaceObject
	//# \sub
	//#		\node	FogSpaceObject
	//#		\node	ShadowSpaceObject
	//#		\node	AmbientSpaceObject
	//#		\node	AcousticsSpaceObject
	//#		\node	OcclusionSpaceObject
	//#		\node	PaintSpaceObject
	//# \end
	//
	//# \node	SourceObject
	//# \sub
	//#		\node	OmniSourceObject
	//#		\sub
	//#			\node	DirectedSourceObject
	//#		\end
	//# \end
	//
	//# \node	GeometryObject
	//# \sub
	//#		\node	MeshGeometryObject
	//#		\node	PrimitiveGeometryObject
	//#		\sub
	//#			\node	PlateGeometryObject
	//#			\node	DiskGeometryObject
	//#			\node	HoleGeometryObject
	//#			\node	AnnulusGeometryObject
	//#			\node	BoxGeometryObject
	//#			\node	PyramidGeometryObject
	//#			\node	CylinderGeometryObject
	//#			\node	ConeGeometryObject
	//#			\node	SphereGeometryObject
	//#			\node	DomeGeometryObject
	//#			\node	TorusGeometryObject
	//#			\node	PathPrimitiveGeometryObject
	//#			\sub
	//#				\node	TubeGeometryObject
	//#				\node	ExtrusionGeometryObject
	//#				\node	RevolutionGeometryObject
	//#			\end
	//#			\node	PhysicsMgr/ClothGeometryObject
	//#			\node	PhysicsMgr/WaterGeometryObject
	//#		\end
	//# \end
	//
	//# \node	TriggerObject
	//# \sub
	//#		\node	BoxTriggerObject
	//#		\node	CylinderTriggerObject
	//#		\node	SphereTriggerObject
	//# \end
	//
	//# \node	EffectMgr/EffectObject
	//# \sub
	//#		\node	EffectMgr/FireEffectObject
	//#		\node	EffectMgr/BeamEffectObject
	//#		\node	EffectMgr/FlareEffectObject
	//#		\node	EffectMgr/PanelEffectObject 
	//# \end
	// 
	//# \node	EffectMgr/EmitterObject 
	//# \sub 
	//#		\node	EffectMgr/BoxEmitterObject
	//#		\node	EffectMgr/CylinderEmitterObject 
	//#		\node	EffectMgr/SphereEmitterObject
	//# \end
	//
	//# \node	PortalObject 
	//# \sub
	//#		\node	DirectPortalObject
	//#		\node	RemotePortalObject
	//#		\node	OcclusionPortalObject 
	//# \end
	//
	//# \node	ZoneObject
	//# \sub
	//#		\node	InfiniteZoneObject
	//#		\node	BoxZoneObject
	//#		\node	CylinderZoneObject
	//#		\node	DomeZoneObject
	//#		\node	PolygonZoneObject
	//# \end
	//
	//# \node	PhysicsMgr/ShapeObject
	//# \sub
	//#		\node	PhysicsMgr/BoxShapeObject
	//#		\node	PhysicsMgr/PyramidShapeObject
	//#		\node	PhysicsMgr/CylinderShapeObject
	//#		\node	PhysicsMgr/ConeShapeObject
	//#		\node	PhysicsMgr/SphereShapeObject
	//#		\node	PhysicsMgr/DomeShapeObject
	//#		\node	PhysicsMgr/CapsuleShapeObject
	//#		\node	PhysicsMgr/TruncatedPyramidShapeObject
	//#		\node	PhysicsMgr/TruncatedConeShapeObject
	//#		\node	PhysicsMgr/TruncatedDomeShapeObject
	//# \end
	//# \node	PhysicsMgr/JointObject
	//# \sub
	//#		\node	PhysicsMgr/SphericalJointObject
	//#		\node	PhysicsMgr/UniversalJointObject
	//#		\node	PhysicsMgr/DiscalJointObject
	//#		\node	PhysicsMgr/RevoluteJointObject
	//#		\node	PhysicsMgr/CylindricalJointObject
	//#		\node	PhysicsMgr/PrismaticJointObject
	//# \end
	//# \node	PhysicsMgr/FieldObject
	//# \sub
	//#		\node	PhysicsMgr/BoxFieldObject
	//#		\node	PhysicsMgr/CylinderFieldObject
	//#		\node	PhysicsMgr/SphereFieldObject
	//# \end
	
	
	typedef Type	ObjectType;
	
	
	enum
	{
		kObjectCamera		= 'CAMR',
		kObjectLight		= 'LITE',
		kObjectSpace		= 'SPAC'
	};
	
	
	enum
	{
		kMaxObjectSizeCount		= 6
	};
	
	
	//# \class	Object		The base class for all objects that can be referenced by a node.
	//
	//# Every object that can be referenced by a scene graph node is a subclass of the $Object$ class.
	//
	//# \def	class Object : public Shared, public Packable, public Configurable, public Constructable<Object>, public ListElement<Object>
	//
	//# \ctor	Object(ObjectType type);
	//
	//# The constructor has protected access. The $Object$ class can only exist as a base class for another class.
	//
	//# \param	type	The type of the object.
	//
	//# \desc
	//# The $Object$ class is the base class for shared objects that are referenced by scene graph nodes.
	//# Every instance of the $@Node@$ class can reference a primary object, simply called the node object.
	//# Some types of nodes can also referenced additional objects. For instance, the $@Geometry@$ class can reference
	//# a $@GraphicsMgr/MaterialObject@$ object in addition to its primary object (which is of type $@GeometryObject@$).
	//
	//# A single object can be referenced by multiple nodes. This allows for data instancing at multiple places
	//# within a world. Since the $Object$ class is shared, it must be released using the $@Utilities/Shared::Release@$ function.
	//
	//# \base	Utilities/Shared				Objects can be referenced by multiple nodes.
	//# \base	ResourceMgr/Packable			Objects can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable		Objects can define configurable parameters that are exposed
	//#											as user interface widgets in the World Editor.
	//# \base	System/Constructable<Object>	New object subclasses may be defined by an application, and a constructor
	//#											function can be installed using the $Constructable$ class.
	//# \base	Utilities/ListElement<Object>	Used internally by the World Manager.
	//
	//# \also	$@Node@$
	
	
	//# \function	Object::GetObjectType		Returns the type of an object.
	//
	//# \proto	ObjectType GetObjectType(void) const;
	//
	//# \desc
	//# The $GetObjectType$ function returns the type of an object. The following table lists the built-in types
	//# that can be returned. Additional types may be defined by the application.
	//
	//# \value	kObjectCamera		Camera object
	//# \value	kObjectLight		Light object
	//# \value	kObjectSpace		Space object
	//# \value	kObjectSource		Sound source object
	//# \value	kObjectGeometry		Geometry object
	//# \value	kObjectTrigger		Trigger object
	//# \value	kObjectEffect		Effect object
	//# \value	kObjectEmitter		Emitter object
	//# \value	kObjectPortal		Portal object
	//# \value	kObjectZone			Zone object
	//# \value	kObjectMaterial		Material object
	//# \value	kObjectShape		Physics shape object
	//# \value	kObjectJoint		Physics joint object
	
	
	//# \function	Object::GetModifiedFlag		Returns the modified flag for an object.
	//
	//# \proto	bool GetModifiedFlag(void) const;
	//
	//# \desc
	//# The $GetModifiedFlag$ function returns the flag indicating whether an object has been modified. This flag
	//# is used when saving a world using the $@WorldMgr::SaveDeltaWorld@$ function.
	//
	//# \also	$@Object::SetModifiedFlag@$
	
	
	//# \function	Object::SetModifiedFlag		Sets the modified flag for an object.
	//
	//# \proto	void SetModifiedFlag(void);
	//
	//# \desc
	//# The $SetModifiedFlag$ function sets the flag indicating whether an object has been modified. This flag
	//# is used when saving a world using the $@WorldMgr::SaveDeltaWorld@$ function. The $SetModifiedFlag$ function
	//# should be called whenever a property of an object has been altered and the world to which it belongs might
	//# later be saved as a difference, or delta, from the original world resource. Once the modified flag has
	//# been set, it cannot be cleared.
	//
	//# \also	$@Object::SetModifiedFlag@$
	
	
	class C4_API Object : public Shared, public Packable, public Configurable, public Constructable<Object>, public ListElement<Object>
	{
		friend class Node;
		
		private:
			
			ObjectType		objectType;
			int32			objectIndex;
			bool			modifiedFlag;
			
			virtual Object *Replicate(void) const;
		
		protected:
			
			Object(ObjectType type);
			virtual ~Object();
		
		public:
			
			ObjectType GetObjectType(void) const
			{
				return (objectType);
			}
			
			int32 GetObjectIndex(void) const
			{
				return (objectIndex);
			}
			
			void SetObjectIndex(int32 index)
			{
				objectIndex = index;
			}
			
			bool GetModifiedFlag(void) const
			{
				return (modifiedFlag);
			}
			
			void SetModifiedFlag(void)
			{
				modifiedFlag = true;
			}
			
			Object *Clone(void) const
			{
				return (Replicate());
			}
			
			static Object *Construct(Unpacker& data, unsigned_int32 unpackFlags = 0);
			
			void PackType(Packer& data) const;
			
			virtual int32 GetObjectSize(float *size) const;
			virtual void SetObjectSize(const float *size);
	};
}


#endif

// ZYURVUR
