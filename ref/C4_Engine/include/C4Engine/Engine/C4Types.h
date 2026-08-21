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


#ifndef C4Types_h
#define C4Types_h


//# \component	Utility Library
//# \prefix		Utilities/


#include "C4Constants.h"
#include "C4Random.h"
#include "C4Spatial.h"
#include "C4Shared.h"
#include "C4Array.h"
#include "C4Tree.h"
#include "C4Graph.h"
#include "C4Map.h"
#include "C4Hash.h"
#include "C4Link.h"
#include "C4Rect.h"
#include "C4String.h"
#include "C4Complex.h"
#include "C4Bivector4D.h"
#include "C4Quaternion.h"
#include "C4Completable.h"
#include "C4Observable.h"


namespace C4
{
	typedef unsigned_int32	Type;
	typedef Type			EventType;
	
	
	enum
	{
		#if C4PLAYSTATION3
		
			kKeyCodeReturn		= 10,
		
		#else
		
			kKeyCodeReturn		= 13,
		
		#endif
		
		kKeyCodeEscape			= 27,
		kKeyCodeLeftArrow		= 28,
		kKeyCodeRightArrow		= 29,
		kKeyCodeUpArrow			= 30,
		kKeyCodeDownArrow		= 31,
		kKeyCodePageUp			= 11,
		kKeyCodePageDown		= 12,
		kKeyCodeHome			= 1,
		kKeyCodeEnd				= 4,
		kKeyCodeBackspace		= 8,
		kKeyCodeDelete			= 127,
		kKeyCodeTab				= 9
	};
	
	
	//# \enum	MouseEventFlags
	
	enum
	{
		kMouseDoubleClick		= 1 << 0		//## The mouse down event is the second click in a double-click.
	};
	
	
	//# \enum	KeyboardModifiers
	
	enum
	{
		kModifierKeyShift		= 1 << 0,		//## The shift key was held down.
		kModifierKeyOption		= 1 << 1,
		kModifierKeyCommand		= 1 << 2,
		kModifierKeyConsole		= 1 << 16
	};
	
	
	enum
	{
		kEventNone							= 0,
		kEventMouseDown						= 'MSDN',
		kEventMouseUp						= 'MSUP',
		kEventRightMouseDown				= 'RMDN',
		kEventRightMouseUp					= 'RMUP',
		kEventMiddleMouseDown				= 'MMDN',
		kEventMiddleMouseUp					= 'MMUP',
		kEventMouseMoved					= 'MSMV',
		kEventMouseWheel					= 'MSWH',
		kEventMultiaxisMouseTranslation		= 'MATR',
		kEventMultiaxisMouseRotation		= 'MART',
		kEventMultiaxisMouseButtonState		= 'MABS',
		kEventKeyDown						= 'KYDN',
		kEventKeyUp							= 'KYUP',
		kEventKeyCommand					= 'KYCM'
	};
	
	
	//# \struct	MouseEventData		Contains information about a mouse event.
	//
	//# The $MouseEventData$ structure contains information about a mouse event. 
	//
	//# \def	struct MouseEventData 
	// 
	//# \data	MouseEventData 
	//
	//# \desc 
	//# The $MouseEventData$ structure contains the event type, mouse position, and special flags
	//# for a mouse event. The $eventFlags$ field can be a combination (through logical OR) of the
	//# following values.
	// 
	//# \table	MouseEventFlags
	//
	//# \also	$@KeyboardEventData@$
	 
	
	//# \member		MouseEventData
	
	struct MouseEventData
	{
		EventType		eventType;			//## The type of mouse event.
		unsigned_int32	eventFlags;			//## The event flags for the event.
		Point3D			mousePosition;		//## The mouse position associated with the event.
	};
	
	
	//# \struct	KeyboardEventData		Contains information about a keyboard event.
	//
	//# The $KeyboardEventData$ structure contains information about a keyboard event.
	//
	//# \def	struct KeyboardEventData
	//
	//# \data	KeyboardEventData
	//
	//# \desc
	//# The $KeyboardEventData$ structure contains the type of event, the Unicode character, and
	//# information about modifiers keys for a keyboard event. The $modifierKeys$ field can be a
	//# combination (through logical OR) of the following values.
	//
	//# \table	KeyboardModifiers
	//
	//# \also	$@MouseEventData@$
	
	
	//# \member		KeyboardEventData
	
	struct KeyboardEventData
	{
		EventType		eventType;			//## The type of keyboard event.
		unsigned_int32	keyCode;			//## The Unicode value for the key associated with the event.
		unsigned_int32	modifierKeys;		//## The modifier keys associated with the event.
	};
	
	
	template <class type, int32 len, typename param1> class ClassArray1
	{
		private:
			
			char	align_address(16) storage[len * sizeof(type)];
		
		public:
			
			ClassArray1(param1 p1)
			{
				for (machine a = 0; a < len; a++) new(&(*this)[a]) type(p1);
			}
			
			~ClassArray1()
			{
				for (machine a = len - 1; a >= 0; a--) (*this)[a].~type();
			}
			
			operator type *(void)
			{
				return (reinterpret_cast<type *>(storage));
			}
			
			operator const type *(void) const
			{
				return (reinterpret_cast<const type *>(storage));
			}
	};
	
	
	template <class type, int32 len, typename param1, typename param2> class ClassArray2
	{
		private:
			
			char	align_address(16) storage[len * sizeof(type)];
		
		public:
			
			ClassArray2(param1 p1, param2 p2)
			{
				for (machine a = 0; a < len; a++) new(&(*this)[a]) type(p1, p2);
			}
			
			~ClassArray2()
			{
				for (machine a = len - 1; a >= 0; a--) (*this)[a].~type();
			}
			
			operator type *(void)
			{
				return (reinterpret_cast<type *>(storage));
			}
			
			operator const type *(void) const
			{
				return (reinterpret_cast<const type *>(storage));
			}
	};
	
	
	//# \class	Range	Encapsulates a range of values.
	//
	//# The $Range$ class template encapsulates a range of values.
	//
	//# \def	template <typename type> struct Range
	//
	//# \tparam		type	The type of value used to represent the beginning and end of a range.
	//
	//# \data	Range
	//
	//# \ctor	Range();
	//# \ctor	Range(const type& x, const type& y);
	//
	//# \param	x	The beginning of the range.
	//# \param	y	The end of the range.
	//
	//# \desc
	//# The $Range$ class template encapsulates a range of values of the type given by the
	//# $type$ class template.
	//# 
	//# The default constructor leaves the beginning and end values of the range undefined.
	//# If the values $x$ and $y$ are supplied, then they are assigned to the beginning and
	//# end of the range, respectively.
	//
	//# \operator	type& operator [](machine index);
	//#				Returns a reference to the minimum value if $index$ is 0, and returns a reference to the maximum value if $index$ is 1.
	//#				The $index$ parameter must be 0 or 1.
	//
	//# \operator	const type& operator [](machine index) const;
	//#				Returns a constant reference to the minimum value if $index$ is 0, and returns a constant reference to the maximum value if $index$ is 1.
	//#				The $index$ parameter must be 0 or 1.
	//
	//# \operator	bool operator ==(const Range& range);
	//#				Returns a boolean value indicating the equality of two ranges.
	//
	//# \operator	bool operator !=(const Range& range);
	//#				Returns a boolean value indicating the inequality of two ranges.
	
	
	//# \function	Range::Set		Sets the beginning and end of a range.
	//
	//# \proto	Range& Set(const type& x, const type& y);
	//
	//# \param	x	The new beginning of the range.
	//# \param	y	The new end of the range.
	//
	//# \desc
	//# The $Set$ function sets the beginning and end of a range to the values given by the
	//# $x$ and $y$ parameters, respectively.
	
	
	//# \member		Range
	
	template <typename type> struct Range
	{
		type	min;		//## The beginning of the range.
		type	max;		//## The end of the range.
		
		Range() {}
		
		Range(const Range& range)
		{
			min = range.min;
			max = range.max;
		}
		
		Range(const type& x, const type& y)
		{
			min = x;
			max = y;
		}
		
		type& operator [](machine index)
		{
			return ((&min)[index]);
		}
		
		const type& operator [](machine index) const
		{
			return ((&min)[index]);
		}
		
		Range& operator =(const Range& range)
		{
			min = range.min;
			max = range.max;
			return (*this);
		}
		
		Range& Set(const type& x, const type& y)
		{
			min = x;
			max = y;
			return (*this);
		}
		
		bool operator ==(const Range& range)
		{
			return ((min == range.min) && (max == range.max));
		}
		
		bool operator !=(const Range& range)
		{
			return ((min != range.min) || (max != range.max));
		}
	};
	
	
	//# \class	Transformable	Encapsulates an object-to-world transform and its inverse.
	//
	//# The $Transformable$ class encapsulates an object-to-world transform and its inverse.
	//
	//# \def	class Transformable
	//
	//# \ctor	Transformable();
	//
	//# \desc
	//# The $Transformable$ class encapsulates a transform from object space to world space
	//# and maintains the corresponding inverse transform from world space to object space.
	//# 
	//# The constructor leaves the transform undefined.
	
	
	//# \function	Transformable::SetIdentityTransform		Sets the transform to the identity.
	//
	//# \proto	void SetIdentityTransform(void);
	//
	//# \desc
	//# The $SetIdentityTransform$ function sets both the object-to-world transform and its
	//# inverse to the identity transform.
	//
	//# \also	$@Transformable::SetWorldTransform@$
	//# \also	$@Transformable::SetWorldPosition@$
	
	
	//# \function	Transformable::GetWorldTransform		Returns the object-to-world transform.
	//
	//# \proto	const Transform4D& GetWorldTransform(void) const;
	//
	//# \desc
	//# The $GetWorldTransform$ returns the $@Math/Transform4D@$ object corresponding to the
	//# transform from object space to world space.
	//
	//# \also	$@Transformable::GetInverseWorldTransform@$
	//# \also	$@Transformable::SetWorldTransform@$
	//# \also	$@Transformable::GetWorldPosition@$
	//# \also	$@Transformable::SetWorldPosition@$
	
	
	//# \function	Transformable::GetInverseWorldTransform		Returns the world-to-object transform.
	//
	//# \proto	const Transform4D& GetInverseWorldTransform(void) const;
	//
	//# \desc
	//# The $GetInverseWorldTransform$ returns the $@Math/Transform4D@$ object corresponding to the
	//# transform from world space to object space. The inverse is not calculated at the time
	//# this function is called, but when the transform is set, so the performance of this function
	//# is high.
	//
	//# \also	$@Transformable::GetWorldTransform@$
	//# \also	$@Transformable::SetWorldTransform@$
	//# \also	$@Transformable::GetWorldPosition@$
	//# \also	$@Transformable::SetWorldPosition@$
	
	
	//# \function	Transformable::SetWorldTransform		Sets the object-to-world transform and its inverse.
	//
	//# \proto	void SetWorldTransform(const Transform4D& transform);
	//# \proto	void SetWorldTransform(const Matrix3D& m, const Point3D& p);
	//# \proto	void SetWorldTransform(const Vector3D& c1, const Vector3D& c2, const Vector3D& c3, const Point3D& c4);
	//# \proto	void SetWorldTransform(float n00, float n01, float n02, float n03, float n10, float n11, float n12, float n13,
	//# \proto2	float n20, float n21, float n22, float n23);
	//
	//# \param	transform	The new transform.
	//# \param	m			The upper-left 3&nbsp;&times;&nbsp;3 portion of the 4D transform.
	//# \param	p			The fourth column of the 4D transform, representing the world position.
	//# \param	c1			The first column of the 4D transform.
	//# \param	c2			The second column of the 4D transform.
	//# \param	c3			The third column of the 4D transform.
	//# \param	c4			The fourth column of the 4D transform.
	//# \param	nij			The entry residing in row <i>i</i> and column <i>j</i> of the 4D transform.
	//
	//# \desc
	//# The $SetWorldTransform$ function sets the object-to-world transform. It also calculates
	//# and stores the inverse representing the transform from world space to object space.
	//
	//# \warning
	//# The $SetWorldTransform$ function is normally called only by internal components of the engine
	//# during routine update procedures. This function should not be called by external code to directly
	//# set the world transform of an object.
	//
	//# \also	$@Transformable::GetWorldTransform@$
	//# \also	$@Transformable::GetWorldPosition@$
	//# \also	$@Transformable::SetWorldPosition@$
	//# \also	$@Transformable::SetIdentityTransform@$
	
	
	//# \function	Transformable::GetWorldPosition		Returns the world-space position.
	//
	//# \proto	const Point3D& GetWorldPosition(void) const;
	//
	//# \desc
	//# The $GetWorldPosition$ function returns the world-space position represented by a
	//# transform. Calling $GetWorldPosition$ is equivalent to the following.
	//
	//# \code	GetWorldTransform().GetTranslation();
	//
	//# \also	$@Transformable::SetWorldPosition@$
	//# \also	$@Transformable::GetWorldTransform@$
	//# \also	$@Transformable::SetWorldTransform@$
	
	
	//# \function	Transformable::SetWorldPosition		Sets the world-space position.
	//
	//# \proto	void SetWorldPosition(const Point3D& position);
	//
	//# \param	position	The new world-space position.
	//
	//# \desc
	//# The $SetWorldPosition$ function sets the world-space position to that given by the
	//# $position$ parameter and recalculates the inverse transform from world space to object space.
	//# The upper-left 3&nbsp;&times;&nbsp;3 portion of the 4D transform is not affected.
	//
	//# \warning
	//# The $SetWorldPosition$ function is normally called only by internal components of the engine
	//# during routine update procedures. This function should not be called by external code to directly
	//# set the world position of an object.
	//
	//# \also	$@Transformable::GetWorldPosition@$
	//# \also	$@Transformable::GetWorldTransform@$
	//# \also	$@Transformable::SetWorldTransform@$
	
	
	class C4_API Transformable
	{
		private:
			
			Transform4D		worldTransform;
			Transform4D		inverseWorldTransform;
		
		public:
			
			Transformable() {}
			
			void SetIdentityTransform(void)
			{
				worldTransform.SetIdentity();
				inverseWorldTransform.SetIdentity();
			}
			
			const Transform4D& GetWorldTransform(void) const
			{
				return (worldTransform);
			}
			
			const Point3D& GetWorldPosition(void) const
			{
				return (worldTransform.GetTranslation());
			}
			
			const Transform4D& GetInverseWorldTransform(void) const
			{
				return (inverseWorldTransform);
			}
			
			void SetWorldTransform(const Transform4D& transform)
			{
				worldTransform = transform;
				inverseWorldTransform = Inverse(worldTransform);
			}
			
			void SetWorldTransform(const Matrix3D& m, const Point3D& p)
			{
				worldTransform.Set(m, p);
				inverseWorldTransform = Inverse(worldTransform);
			}
			
			void SetWorldTransform(const Vector3D& c1, const Vector3D& c2, const Vector3D& c3, const Point3D& c4)
			{
				worldTransform.Set(c1, c2, c3, c4);
				inverseWorldTransform = Inverse(worldTransform);
			}
			
			void SetWorldTransform(float n00, float n01, float n02, float n03, float n10, float n11, float n12, float n13, float n20, float n21, float n22, float n23)
			{
				worldTransform.Set(n00, n01, n02, n03, n10, n11, n12, n13, n20, n21, n22, n23);
				inverseWorldTransform = Inverse(worldTransform);
			}
			
			void SetWorldPosition(const Point3D& position)
			{
				worldTransform.SetTranslation(position);
				inverseWorldTransform = Inverse(worldTransform);
			}
	};
	
	
	template <class type> class Reference : public ListElement<Reference<type> >
	{
		private:
			
			type	*referenceTarget;
		
		public:
			
			explicit Reference(type *target)
			{
				referenceTarget = target;
			}
			
			~Reference()
			{
			}
			
			type *GetTarget(void) const
			{
				return (referenceTarget);
			}
	};
	
	
	class C4_API Buffer
	{
		private:
			
			char	*buffer;
		
		public:
			
			explicit Buffer(unsigned_int32 size)
			{
				buffer = new char[size];
			}
			
			~Buffer()
			{
				delete[] buffer;
			}
			
			operator void *(void) const
			{
				return (buffer);
			}
			
			void *operator *(void) const
			{
				return (buffer);
			}
	};
	
	
	inline void Reverse(int16 *addr)
	{
		#if C4POWERPC
		
			*reinterpret_cast<unsigned_int16 *>(addr) = __lhbrx(addr);
		
		#else
		
			unsigned_int32 x = *reinterpret_cast<unsigned_int16 *>(addr);
			*reinterpret_cast<unsigned_int16 *>(addr) = (unsigned_int16) ((x << 8) | (x >> 8));
		
		#endif
	}
	
	inline void Reverse(unsigned_int16 *addr)
	{
		#if C4POWERPC
		
			*addr = __lhbrx(addr);
		
		#else
		
			unsigned_int32 x = *addr;
			*addr = (unsigned_int16) ((x << 8) | (x >> 8));
		
		#endif
	}
	
	inline void Reverse(int32 *addr)
	{
		#if C4POWERPC
		
			*addr = __lwbrx(addr);
		
		#else
		
			unsigned_int32 x = *reinterpret_cast<unsigned_int32 *>(addr);
			*addr = (int32) ((x << 24) | ((x << 8) & 0x00FF0000) | ((x >> 8) & 0x0000FF00) | (x >> 24));
		
		#endif
	}
	
	inline void Reverse(unsigned_int32 *addr)
	{
		#if C4POWERPC
		
			*addr = __lwbrx(addr);
		
		#else
		
			unsigned_int32 x = *addr;
			*addr = (x << 24) | ((x << 8) & 0x00FF0000) | ((x >> 8) & 0x0000FF00) | (x >> 24);
		
		#endif
	}
	
	inline void Reverse(float *addr)
	{
		#if C4POWERPC
		
			*reinterpret_cast<unsigned_int32 *>(addr) = __lwbrx(addr);
		
		#else
		
			unsigned_int32 x = *reinterpret_cast<unsigned_int32 *>(addr);
			*reinterpret_cast<unsigned_int32 *>(addr) = (x << 24) | ((x << 8) & 0x00FF0000) | ((x >> 8) & 0x0000FF00) | (x >> 24);
		
		#endif
	}
	
	inline void Reverse(int64 *addr)
	{
		#if C4POWERPC
		
			*addr = __ldbrx(addr);
		
		#else
		
			unsigned_int32 *a = reinterpret_cast<unsigned_int32 *>(addr);
			unsigned_int32 x = a[0];
			unsigned_int32 y = a[1];
			a[1] = (int32) ((x << 24) | ((x << 8) & 0x00FF0000) | ((x >> 8) & 0x0000FF00) | (x >> 24));
			a[0] = (int32) ((y << 24) | ((y << 8) & 0x00FF0000) | ((y >> 8) & 0x0000FF00) | (y >> 24));
		
		#endif
	}
	
	inline void Reverse(unsigned_int64 *addr)
	{
		#if C4POWERPC
		
			*addr = __ldbrx(addr);
		
		#else
		
			unsigned_int32 *a = reinterpret_cast<unsigned_int32 *>(addr);
			unsigned_int32 x = a[0];
			unsigned_int32 y = a[1];
			a[1] = (int32) ((x << 24) | ((x << 8) & 0x00FF0000) | ((x >> 8) & 0x0000FF00) | (x >> 24));
			a[0] = (int32) ((y << 24) | ((y << 8) & 0x00FF0000) | ((y >> 8) & 0x0000FF00) | (y >> 24));
		
		#endif
	}
	
	
	template <typename type> inline void Reverse(Range<type> *range)
	{
		Reverse(&range->min);
		Reverse(&range->max);
	}
	
	
	inline void Reverse(char *)
	{
	}
	
	inline void Reverse(unsigned_int8 *)
	{
	}
	
	inline void Reverse(signed char *)
	{
	}
	
	template <int32 len> inline void Reverse(String<len> *)
	{
	}
	
	
	C4API void Reverse(Vector2D *v);
	C4API void Reverse(Vector3D *v);
	C4API void Reverse(Vector4D *v);
	C4API void Reverse(Antivector4D *v);
	C4API void Reverse(Matrix3D *m);
	C4API void Reverse(Matrix4D *m);
	C4API void Reverse(Quaternion *q);
	C4API void Reverse(ColorRGB *c);
	C4API void Reverse(ColorRGBA *c);
	C4API void Reverse(Fixed2D *v);
	C4API void Reverse(Fixed3D *v);
	C4API void Reverse(Integer2D *v);
	C4API void Reverse(Integer3D *v);
	C4API void Reverse(Rect *r);
}


#endif

// ZYURVUR
