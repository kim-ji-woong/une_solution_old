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


#ifndef C4Array_h
#define C4Array_h


//# \component	Utility Library
//# \prefix		Utilities/


#include "C4Memory.h"


namespace C4
{
	//# \class	Array	A container class that holds an array of objects.
	//
	//# The $Array$ class represents a dynamically resizable array of objects
	//# for which any entry can be accessed in constant time.
	//
	//# \def	template <typename type> class Array
	//
	//# \tparam		type	The type of the class that can be stored in the array.
	//
	//# \ctor	explicit Array(int32 count = 0, int32 expand = 4);
	//
	//# \param	count	The number of objects for which space is initially reserved in the array's storage.
	//# \param	expand	The minimum amount of space, as a number of objects, by which the array storage grows when adding a new object requires space to be reallocated.
	//
	//# \desc
	//# The $Array$ class represents a homogeneous array of objects whose type is given by the
	//# $type$ template parameter. Upon construction, the initial size of the array is zero, but
	//# space is reserved for the number of objects given by the $count$ parameter. The array is
	//# stored contiguously in memory, allowing constant-time random access to its elements.
	//# 
	//# As elements are added to the array (using the $@Array::AddElement@$ function), the storage
	//# size is automatically increased to a size somewhat larger than that needed to store the new
	//# element. The cost of adding an element is thus amortized linear time.
	//#
	//# An $Array$ object can be implicitly converted to a pointer to its first element. This allows the
	//# use of the $[]$ operator to access individual elements of the array.
	
	
	//# \function	Array::GetElementCount		Returns the current size of an array.
	//
	//# \proto	int32 GetElementCount(void) const;
	//
	//# \desc
	//# The $GetElementCount$ function returns the number of objects currently stored in an array.
	//# When an array is constructed, its initial element count is zero.
	//
	//# \also	$@Array::SetElementCount@$
	//# \also	$@Array::AddElement@$
	//# \also	$@Array::InsertElement@$
	//# \also	$@Array::RemoveElement@$
	
	
	//# \function	Array::SetElementCount		Sets the current size of an array.
	//
	//# \proto	void SetElementCount(int32 count, const type *init = nullptr);
	//
	//# \param	count	The new size of the array.
	//# \param	init	A pointer to an object that is used to construct new objects in the array.
	//#					If this is $nullptr$, then new objects are default-constructed. Otherwise, new
	//#					objects are copy-constructed using a reference to the object to which $init$ points.
	//
	//# \desc
	//# The $SetElementCount$ function sets the number of objects currently stored in an array.
	//# If $count$ is greater than the current size of the array, then space is allocated for
	//# $count$ objects and each new object is constructed using the value of the $init$ parameter.
	//# If $count$ is less than the current size of the array, then the logical size of the array
	//# is reduced, and each object beyond the new size of the array is destroyed in reverse order.
	//
	//# \also	$@Array::GetElementCount@$
	//# \also	$@Array::AddElement@$
	//# \also	$@Array::InsertElement@$
	//# \also	$@Array::RemoveElement@$
	
	
	//# \function	Array::AddElement		Adds an object to the end of an array.
	//
	//# \proto	void AddElement(const type& element);
	//
	//# \param	element		The new element to add to the array.
	//
	//# \desc
	//# The $AddElement$ function increases the size of an array by one and copy-constructs the
	//# new element using the object referenced by the $element$ parameter.
	//
	//# \also	$@Array::InsertElement@$
	//# \also	$@Array::RemoveElement@$
	//# \also	$@Array::GetElementCount@$
	//# \also	$@Array::SetElementCount@$
	
	
	//# \function	Array::InsertElement	Inserts an object into an array.
	//
	//# \proto	void InsertElement(int32 index, const type& element);
	//
	//# \param	index		The location at which the object is to be inserted.
	//# \param	element		The new element to insert into the array.
	//
	//# \desc
	//# The $InsertElement$ function increases the size of an array by one, moves all of the existing 
	//# elements at location $index$ or greater up by one, and copy-constructs the new element into
	//# the array using the object referenced by the $element$ parameter. When the existing elements 
	//# are moved, they are copy-constructed to their new locations, and the old objects are destroyed. 
	//#  
	//# If the $index$ parameter is greater than or equal to the current size of the array, then
	//# the array is enlarged to the size $index&nbsp;+&nbsp;1$, and the elements between the old size and 
	//# new size are default-constructed.
	//
	//# \also	$@Array::RemoveElement@$
	//# \also	$@Array::AddElement@$ 
	//# \also	$@Array::GetElementCount@$
	//# \also	$@Array::SetElementCount@$
	
	 
	//# \function	Array::RemoveElement	Removes an object from an array.
	//
	//# \proto	void RemoveElement(int32 index);
	//
	//# \param	index	The location at which to remove an object.
	//
	//# \desc
	//# The $RemoveElement$ function decreases the size of an array by one, destroys the object at location
	//# $index$, and moves all of the existing elements at location $index&nbsp;+&nbsp;1$ or greater down by one.
	//# When the existing elements are moved, they are copy-constructed to their new locations, and the old
	//# objects are destroyed.
	//# 
	//# If the $index$ parameter is greater than or equal to the current size of the array, then
	//# calling the $RemoveElement$ function has no effect.
	//
	//# \also	$@Array::InsertElement@$
	//# \also	$@Array::AddElement@$
	//# \also	$@Array::GetElementCount@$
	//# \also	$@Array::SetElementCount@$
	
	
	//# \function	Array::Purge		Removes all objects from an array.
	//
	//# \proto	void Purge(void);
	//
	//# \desc
	//# The $Purge$ function destroys all objects in an array (in reverse order) and sets the size of
	//# the array to zero.
	//
	//# \also	$@Array::RemoveElement@$
	//# \also	$@Array::SetElementCount@$
	
	
	//# \function	Array::FindElement		Finds a specific element in an array.
	//
	//# \proto	int32 FindElement(const type& element) const;
	//
	//# \param	element		The value of the element to find.
	//
	//# \desc
	//# The $FindElement$ function searches an array for the first element matching the value passed into the
	//# $element$ parameter based on the $==$ operator. If a match is found, its index is returned.
	//# If no match is found, then the return value is &minus;1. The running time of this function is
	//# <i>O</i>(<i>n</i>), where <i>n</i> is the number of elements in the array.
	
	
	template <typename type, int32 minSize = 0> class Array
	{
		private:
			
			int32		logicalSize;
			int32		physicalSize;
			
			type		*arrayPointer;
			char		arrayStorage[minSize * sizeof(type)];
			
			void SetPhysicalSize(int32 size);
		
		public:
			
			explicit Array();
			Array(const Array& array);
			~Array();
			
			operator type *(void)
			{
				return (arrayPointer);
			}
			
			operator const type *(void) const
			{
				return (arrayPointer);
			}
			
			int32 GetElementCount(void) const
			{
				return (logicalSize);
			}
			
			void Purge(void);
			
			void SetElementCount(int32 count, const type *init = nullptr);
			void AddElement(const type& element);
			type *AddElement(void);
			
			void InsertElement(int32 index, const type& element);
			void RemoveElement(int32 index);
			
			int32 FindElement(const type& element) const;
	};
	
	
	template <typename type, int32 minSize> Array<type, minSize>::Array()
	{
		logicalSize = 0;
		physicalSize = minSize;
		arrayPointer = reinterpret_cast<type *>(arrayStorage);
	}
	
	template <typename type, int32 minSize> Array<type, minSize>::Array(const Array& array)
	{
		logicalSize = array.logicalSize;
		physicalSize = array.physicalSize;
		
		if (logicalSize > minSize) arrayPointer = reinterpret_cast<type *>(new char[sizeof(type) * physicalSize]);
		else arrayPointer = reinterpret_cast<type *>(arrayStorage);
		
		for (machine a = 0; a < logicalSize; a++) new(&arrayPointer[a]) type(array.arrayPointer[a]);
	}
	
	template <typename type, int32 minSize> Array<type, minSize>::~Array()
	{
		type *pointer = arrayPointer + logicalSize;
		for (machine a = logicalSize - 1; a >= 0; a--) (--pointer)->~type();
		
		char *ptr = reinterpret_cast<char *>(arrayPointer);
		if (ptr != arrayStorage) delete[] ptr;
	}
	
	template <typename type, int32 minSize> void Array<type, minSize>::Purge(void)
	{
		type *pointer = arrayPointer + logicalSize;
		for (machine a = logicalSize - 1; a >= 0; a--) (--pointer)->~type();
		
		char *ptr = reinterpret_cast<char *>(arrayPointer);
		if (ptr != arrayStorage) delete[] ptr;
		
		logicalSize = 0;
		physicalSize = minSize;
		arrayPointer = reinterpret_cast<type *>(arrayStorage);
	}
	
	template <typename type, int32 minSize> void Array<type, minSize>::SetPhysicalSize(int32 size)
	{
		physicalSize = Max(Max(size, 4), physicalSize + Max((physicalSize / 2 + 3) & ~3, minSize));
		type *newPointer = reinterpret_cast<type *>(new char[sizeof(type) * physicalSize]);
		
		type *pointer = arrayPointer;
		for (machine a = 0; a < logicalSize; a++)
		{
			new(&newPointer[a]) type(*pointer);
			pointer->~type();
			pointer++;
		}
		
		char *ptr = reinterpret_cast<char *>(arrayPointer);
		if (ptr != arrayStorage) delete[] ptr;
		
		arrayPointer = newPointer;
	}
	
	template <typename type, int32 minSize> void Array<type, minSize>::SetElementCount(int32 count, const type *init)
	{
		if (count > physicalSize) SetPhysicalSize(count);
		
		if (count > logicalSize)
		{
			type *pointer = arrayPointer + (logicalSize - 1);
			if (init) for (machine a = logicalSize; a < count; a++) new(++pointer) type(*init);
			else for (machine a = logicalSize; a < count; a++) new(++pointer) type;
		}
		else if (count < logicalSize)
		{
			type *pointer = arrayPointer + logicalSize;
			for (machine a = logicalSize - 1; a >= count; a--) (--pointer)->~type();
		}
		
		logicalSize = count;
	}
	
	template <typename type, int32 minSize> void Array<type, minSize>::AddElement(const type& element)
	{
		if (logicalSize >= physicalSize) SetPhysicalSize(logicalSize + 1);
		
		type *pointer = arrayPointer + logicalSize;
		new(pointer) type(element);
		
		logicalSize++;
	}
	
	template <typename type, int32 minSize> type *Array<type, minSize>::AddElement(void)
	{
		if (logicalSize >= physicalSize) SetPhysicalSize(logicalSize + 1);
		
		type *pointer = arrayPointer + logicalSize;
		new(pointer) type;
		
		logicalSize++;
		return (pointer);
	}
	
	template <typename type, int32 minSize> void Array<type, minSize>::InsertElement(int32 index, const type& element)
	{
		if (index >= logicalSize)
		{
			int32 count = index + 1;
			if (count > physicalSize) SetPhysicalSize(count);
			
			type *pointer = &arrayPointer[logicalSize - 1];
			for (machine a = logicalSize; a < index; a++) new(++pointer) type;
			
			new (++pointer) type(element);
			logicalSize = count;
		}
		else
		{
			int32 count = logicalSize + 1;
			if (count > physicalSize) SetPhysicalSize(count);
			
			type *pointer = &arrayPointer[logicalSize];
			for (machine a = logicalSize; a > index; a--)
			{
				new(pointer) type(pointer[-1]);
				(--pointer)->~type();
			}
			
			new (&arrayPointer[index]) type(element);
			logicalSize = count;
		}
	}
	
	template <typename type, int32 minSize> void Array<type, minSize>::RemoveElement(int32 index)
	{
		if (index < logicalSize)
		{
			type *pointer = &arrayPointer[index];
			pointer->~type();
			
			for (machine a = index + 1; a < logicalSize; a++)
			{
				new(pointer) type(pointer[1]);
				(++pointer)->~type();
			}
			
			logicalSize--;
		}
	}
	
	template <typename type, int32 minSize> int32 Array<type, minSize>::FindElement(const type& element) const
	{
		for (machine a = 0; a < logicalSize; a++) if (arrayPointer[a] == element) return (a);
		return (-1);
	}
	
	
	template <typename type> class Array<type, 0>
	{
		private:
			
			int32		logicalSize;
			int32		physicalSize;
			int32		expandSize;
			
			type		*arrayPointer;
			
			void SetPhysicalSize(int32 size);
		
		public:
			
			explicit Array(int32 count = 0, int32 expand = 4);
			Array(const Array& array);
			~Array();
			
			operator type *(void)
			{
				return (arrayPointer);
			}
			
			operator const type *(void) const
			{
				return (arrayPointer);
			}
			
			int32 GetElementCount(void) const
			{
				return (logicalSize);
			}
			
			void Purge(void);
			
			void SetElementCount(int32 count, const type *init = nullptr);
			void AddElement(const type& element);
			type *AddElement(void);
			
			void InsertElement(int32 index, const type& element);
			void RemoveElement(int32 index);
			
			int32 FindElement(const type& element) const;
	};
	
	
	template <typename type> Array<type, 0>::Array(int32 count, int32 expand)
	{
		logicalSize = 0;
		physicalSize = count;
		expandSize = Max(expand, 4);
		
		arrayPointer = (count > 0) ? reinterpret_cast<type *>(new char[sizeof(type) * count]) : nullptr;
	}
	
	template <typename type> Array<type, 0>::Array(const Array& array)
	{
		logicalSize = array.logicalSize;
		physicalSize = array.physicalSize;
		expandSize = array.expandSize;
		
		if (physicalSize > 0)
		{
			arrayPointer = reinterpret_cast<type *>(new char[sizeof(type) * physicalSize]);
			for (machine a = 0; a < logicalSize; a++) new(&arrayPointer[a]) type(array.arrayPointer[a]);
		}
		else
		{
			arrayPointer = nullptr;
		}
	}
	
	template <typename type> Array<type, 0>::~Array()
	{
		type *pointer = arrayPointer + logicalSize;
		for (machine a = logicalSize - 1; a >= 0; a--) (--pointer)->~type();
		delete[] reinterpret_cast<char *>(arrayPointer);
	}
	
	template <typename type> void Array<type, 0>::Purge(void)
	{
		type *pointer = arrayPointer + logicalSize;
		for (machine a = logicalSize - 1; a >= 0; a--) (--pointer)->~type();
		delete[] reinterpret_cast<char *>(arrayPointer);
		
		logicalSize = 0;
		physicalSize = 0;
		arrayPointer = nullptr;
	}
	
	template <typename type> void Array<type, 0>::SetPhysicalSize(int32 size)
	{
		physicalSize = Max(Max(size, 4), physicalSize + Max((physicalSize / 2 + 3) & ~3, expandSize));
		type *newPointer = reinterpret_cast<type *>(new char[sizeof(type) * physicalSize]);
		
		type *pointer = arrayPointer;
		if (pointer)
		{
			for (machine a = 0; a < logicalSize; a++)
			{
				new(&newPointer[a]) type(*pointer);
				pointer->~type();
				pointer++;
			}
			
			delete[] reinterpret_cast<char *>(arrayPointer);
		}
		
		arrayPointer = newPointer;
	}
	
	template <typename type> void Array<type, 0>::SetElementCount(int32 count, const type *init)
	{
		if (count > physicalSize) SetPhysicalSize(count);
		
		if (count > logicalSize)
		{
			type *pointer = arrayPointer + (logicalSize - 1);
			if (init) for (machine a = logicalSize; a < count; a++) new(++pointer) type(*init);
			else for (machine a = logicalSize; a < count; a++) new(++pointer) type;
		}
		else if (count < logicalSize)
		{
			type *pointer = arrayPointer + logicalSize;
			for (machine a = logicalSize - 1; a >= count; a--) (--pointer)->~type();
		}
		
		logicalSize = count;
	}
	
	template <typename type> void Array<type, 0>::AddElement(const type& element)
	{
		if (logicalSize >= physicalSize) SetPhysicalSize(logicalSize + 1);
		
		type *pointer = arrayPointer + logicalSize;
		new(pointer) type(element);
		
		logicalSize++;
	}
	
	template <typename type> type *Array<type, 0>::AddElement(void)
	{
		if (logicalSize >= physicalSize) SetPhysicalSize(logicalSize + 1);
		
		type *pointer = arrayPointer + logicalSize;
		new(pointer) type;
		
		logicalSize++;
		return (pointer);
	}
	
	template <typename type> void Array<type, 0>::InsertElement(int32 index, const type& element)
	{
		if (index >= logicalSize)
		{
			int32 count = index + 1;
			if (count > physicalSize) SetPhysicalSize(count);
			
			type *pointer = &arrayPointer[logicalSize - 1];
			for (machine a = logicalSize; a < index; a++) new(++pointer) type;
			
			new (++pointer) type(element);
			logicalSize = count;
		}
		else
		{
			int32 count = logicalSize + 1;
			if (count > physicalSize) SetPhysicalSize(count);
			
			type *pointer = &arrayPointer[logicalSize];
			for (machine a = logicalSize; a > index; a--)
			{
				new(pointer) type(pointer[-1]);
				(--pointer)->~type();
			}
			
			new (&arrayPointer[index]) type(element);
			logicalSize = count;
		}
	}
	
	template <typename type> void Array<type, 0>::RemoveElement(int32 index)
	{
		if (index < logicalSize)
		{
			type *pointer = &arrayPointer[index];
			pointer->~type();
			
			for (machine a = index + 1; a < logicalSize; a++)
			{
				new(pointer) type(pointer[1]);
				(++pointer)->~type();
			}
			
			logicalSize--;
		}
	}
	
	template <typename type> int32 Array<type, 0>::FindElement(const type& element) const
	{
		for (machine a = 0; a < logicalSize; a++) if (arrayPointer[a] == element) return (a);
		return (-1);
	}
}


#endif

// ZYURVUR
