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


#include "C4CameraObjects.h"
#include "C4LightObjects.h"
#include "C4SpaceObjects.h"


using namespace C4;


namespace C4
{
	template class Constructable<Object>;
}


Object::Object(ObjectType type)
{
	objectType = type;
	objectIndex = -1;
	modifiedFlag = false;
}

Object::~Object()
{
}

Object *Object::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kObjectCamera:
			
			return (CameraObject::Construct(++data, unpackFlags));
		
		case kObjectLight:
			
			return (LightObject::Construct(++data, unpackFlags));
		
		case kObjectSpace:
			
			return (SpaceObject::Construct(++data, unpackFlags));
	}
	
	return (Constructable<Object>::Construct(data, unpackFlags));
}

void Object::PackType(Packer& data) const
{
	data << objectType;
}

Object *Object::Replicate(void) const
{
	return (nullptr);
}

int32 Object::GetObjectSize(float *size) const
{
	return (0);
}

void Object::SetObjectSize(const float *size)
{
}

// ZYURVUR
