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


#ifndef C4Construction_h
#define C4Construction_h


//# \component	System Utilities
//# \prefix		System/


#include "C4Packing.h"


namespace C4
{
	//# \class	 Constructor		Encapsulates a constructor function for application-defined subclasses.
	//
	//# The $Constructor$ class template encapsulates a constructor function for application-defined subclasses.
	//
	//# \def	template <class type, typename param = Unpacker&> class Constructor :
	//# \def2	public ListElement<Constructor<type, param>>
	//
	//# \tparam		type	The type of the base class for the application-defined class.
	//# \tparam		param	The type of the data passed to the constructor function.
	//
	//# \ctor	Constructor(ConstructProc *proc);
	//
	//# \param		proc	A pointer to the constructor function.
	//
	//# \desc
	//# The $Constructor$ class template encapsulates a constructor function that is called when the engine
	//# encounters an object that is a subclass of the template parameter, but is of an application-defined type.
	//# The $ConstructProc$ type is defined as follows.
	//
	//# \code	typedef type *ConstructProc(param data, unsigned_int32 flags);
	//
	//# An application should define a function having this signature, create a $Constructor$ instance to
	//# encapsulate it, and then install the constructor object by calling the $@Constructable::InstallConstructor@$
	//# function.
	//# 
	//# When the engine calls the constructor function specified by the $proc$ parameter, it passes in the custom
	//# data type specified by the $param$ template parameter, which is normally a reference to an $@ResourceMgr/Unpacker@$
	//# object. The application should examine this data (normally by calling the $@ResourceMgr/Unpacker::GetType@$
	//# function) and return a newly constructed class corresponding to that type if it is recognized. If the type is
	//# not recognized, then the function should return $nullptr$ to indicate that the remaining installed constructor
	//# objects are to be given the opportunity to construct the subclass.
	//
	//# \base	Utilities/ListElement<Constructor<type, param>>		The $Constructable$ class holds a list of $Constructor$ instances.
	//
	//# \also	$@Constructable@$
	
	
	template <class type, typename param = Unpacker&> class Constructor : public ListElement<Constructor<type, param> >
	{
		public:
			
			typedef type *ConstructProc(param, unsigned_int32);
		
		private:
			
			ConstructProc	*constructProc;
		
		public:
			
			Constructor(ConstructProc *proc)
			{
				constructProc = proc;
			}
			
			type *Construct(param data, unsigned_int32 flags = 0)
			{
				return ((*constructProc)(data, flags));
			}
	};
	
	
	//# \class	 Constructable		The base class for class types that can be extended by application-defined subclasses.
	//
	//# The $Constructable$ class template is the base class for class types that can be extended by
	//# application-defined subclasses.
	//
	//# \def	template <class type, typename param = Unpacker&> class Constructable
	//
	//# \tparam		type	The type of the base class of a hierarchy of extensible classes.
	//# \tparam		param	The type of the data passed to the constructor functions.
	//
	//# \ctor	Constructable()
	//
	//# \desc
	//# The $Constructable$ class template appears as a base class for types of objects that can have custom subclasses
	//# defined by an application. So that the engine can construct these custom subclasses when necessary
	//# (e.g., when loading a world), an application installs a special constructor object by calling the
	//# $@Constructable::InstallConstructor@$ function. This object has a member function that is responsible
	//# for returning a pointer to a newly constructed instance of the subclass of a given type.
	//
	//# \also	$@Constructor@$
	//# \also	$@Registrable@$
	
	
	//# \function	Constructable::InstallConstructor		Installs a constructor object for application-defined subclasses of the template parameter.
	//
	//# \proto	static void InstallConstructor(Constructor<type, param> *constructor);
	//
	//# \param	constructor		A pointer to the $@Constructor@$ object corresponding to the template parameter. 
	//
	//# \desc 
	//# The $InstallConstructor$ function installs a constructor object whose responsibility it is to create 
	//# new instances of application-defined subclasses of the template parameter. The constructor object 
	//# encapsulates a function that is called by the engine when it encounters a custom type.
	//#  
	//# Once the constructor object is installed, it must continue to exist in order to function. A constructor
	//# object is uninstalled by simply deleting it or by calling the $@Constructable::RemoveConstructor@$ function.
	//
	//# \also	$@Constructable::RemoveConstructor@$ 
	//# \also	$@Constructor@$
	
	
	//# \function	Constructable::RemoveConstructor		Removes a constructor object for application-defined subclasses of the template parameter. 
	//
	//# \proto	static void RemoveConstructor(Constructor<type, param> *constructor);
	//
	//# \param	constructor		A pointer to the $@Constructor@$ object corresponding to the template parameter.
	//
	//# \desc
	//# The $RemoveConstructor$ function removes a constructor object. The constructor object specified by the $constructor$
	//# parameter must have previously been installed by calling the $@Constructable::InstallConstructor@$ function.
	//# 
	//# A constructor object can also be removed by simply deleting it.
	//
	//# \also	$@Constructable::InstallConstructor@$
	//# \also	$@Constructor@$
	
	
	template <class type, typename param = Unpacker&> class Constructable
	{
		private:
			
			static List<Constructor<type, param> >	constructorList;
		
		public:
			
			static void InstallConstructor(Constructor<type, param> *constructor)
			{
				constructorList.Append(constructor);
			}
			
			static void RemoveConstructor(Constructor<type, param> *constructor)
			{
				constructorList.Remove(constructor);
			}
			
			static type *Construct(param data, unsigned_int32 flags = 0);
	};
	
	
	#ifdef C4ENGINEMODULE
	
		template <class type, typename param> List<Constructor<type, param> > Constructable<type, param>::constructorList;
		
		
		template <class type, typename param> type *Constructable<type, param>::Construct(param data, unsigned_int32 flags)
		{
			Constructor<type, param> *constructor = constructorList.First();
			while (constructor)
			{
				type *constructable = constructor->Construct(data, flags);
				if (constructable) return (constructable);
				
				constructor = constructor->Next();
			}
			
			return (nullptr);
		}
	
	#endif
	
	
	//# \class	 Registration		The base class for registration objects.
	//
	//# The $Registration$ class template is the base class for registration objects
	//
	//# \def	template <class classType, class regType> class Registration :
	//# \def2	public MapElement<Registration<classType, regType>>
	//
	//# \tparam		classType	The type of the base class of a hierarchy of extensible classes.
	//# \tparam		regType		The type of the $Registration$ subclass used to register classes derived from $classType$.
	//
	//# \ctor	Registration(Type type);
	//
	//# \param	type	The type of the subclass being registered.
	//
	//# \desc
	//# The $Registration$ class is the base class used by more specific types of registration objects. Registration objects
	//# are used to identify custom types in the engine.
	//
	//# \base	Utilities/MapElement<Registration<classType, regType>>	Registration objects are stored in a map container by the engine.
	//
	//# \also	$@Registration@$
	
	
	//# \function	Registration::GetRegistrableType		Returns the type of the registered class.
	//
	//# \proto	Type GetRegistrableType(void) const;
	//
	//# \desc
	//# The $GetRegistrableType$ function returns the unique type identifier for the custom class type that
	//# the registration object represents.
	//
	//# \also	$@Registrable::FindRegistration@$
	
	
	template <class classType, class regType> class Registration : public MapElement<Registration<classType, regType> >
	{
		private:
			
			Type	registrableType;
		
		protected:
			
			Registration(Type type);
		
		public:
			
			typedef Type KeyType;
			
			virtual ~Registration();
			
			KeyType GetKey(void) const
			{
				return (registrableType);
			}
			
			Type GetRegistrableType(void) const
			{
				return (registrableType);
			}
			
			regType *Previous(void) const
			{
				return (static_cast<regType *>(MapElement<Registration<classType, regType> >::Previous()));
			}
			
			regType *Next(void) const
			{
				return (static_cast<regType *>(MapElement<Registration<classType, regType> >::Next()));
			}
			
			virtual classType *Construct(void) const = 0;
	};
	
	
	//# \class	 Registrable		The base class for class types that can have custom subclasses that are registered with the engine.
	//
	//# The $Registrable$ class template is the base class for class types that can have custom subclasses
	//# that are registered with the engine.
	//
	//# \def	template <class classType, class regType> class Registrable : public Constructable<classType>
	//
	//# \tparam		classType	The type of the base class of a hierarchy of extensible classes.
	//# \tparam		regType		The type of the $@Registration@$ subclass used to register classes derived from $classType$.
	//
	//# \ctor	Registrable()
	//
	//# \desc
	//# The $Registrable$ class is the base class for class types that can have custom subclasses that
	//# are registered with the engine.
	//
	//# \base	Constructable<classType>	Registrable objects are a special type of constructable object.
	//
	//# \also	$@Registration@$
	
	
	//# \function	Registrable::GetFirstRegistration		Returns the first registration.
	//
	//# \proto	static regType *GetFirstRegistration(void);
	//
	//# \desc
	//# The $GetFirstRegistration$ function returns a pointer to the registration object corresponding
	//# to the first registration for subclasses of the $classType$ template parameter. The entire list of
	//# registrations can be iterated by calling the $@Utilities/MapElement::Next@$ function on the returned
	//# object and continuing until $nullptr$ is returned.
	//
	//# \also	$@Registrable::FindRegistration@$
	//# \also	$@Registration@$
	
	
	//# \function	Registrable::FindRegistration		Returns a specific registration.
	//
	//# \proto	static regType *FindRegistration(Type type);
	//
	//# \param	type	The type of the subclass.
	//
	//# \desc
	//# The $FindRegistration$ function returns a pointer to the registration object corresponding
	//# to the subclass type specified by the $type$ parameter. If no such registration exists,
	//# then the return value is $nullptr$.
	//
	//# \also	$@Registrable::GetFirstRegistration@$
	//# \also	$@Registration@$
	
	
	template <class classType, class regType> class Registrable : public Constructable<classType>
	{
		friend class Registration<classType, regType>;
		
		private:
			
			static Map<Registration<classType, regType> >		registrationMap;
		
		public:
			
			static regType *GetFirstRegistration(void)
			{
				return (static_cast<regType *>(registrationMap.First()));
			}
			
			static regType *FindRegistration(Type type)
			{
				return (static_cast<regType *>(registrationMap.Find(type)));
			}
			
			static int32 GetRegistrationCount(void)
			{
				return (registrationMap.GetElementCount());
			}
			
			static classType *Construct(Unpacker& data, unsigned_int32 unpackFlags = 0);
	};
	
	
	#ifdef C4ENGINEMODULE
	
		template <class classType, class regType> Map<Registration<classType, regType> > Registrable<classType, regType>::registrationMap;
		
		
		template <class classType, class regType> Registration<classType, regType>::Registration(Type type)
		{
			registrableType = type;
			Registrable<classType, regType>::registrationMap.Insert(this);
		}
		
		template <class classType, class regType> Registration<classType, regType>::~Registration()
		{
		}
		
		template <class classType, class regType> classType *Registrable<classType, regType>::Construct(Unpacker& data, unsigned_int32 unpackFlags)
		{
			Type type = data.GetType();
			regType *registration = FindRegistration(type);
			if (registration) return (registration->Construct());
			
			return (Constructable<classType>::Construct(data, unpackFlags));
		}
	
	#endif
}


#endif

// ZYURVUR
