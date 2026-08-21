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


#ifndef C4Configurable_h
#define C4Configurable_h


//# \component	Interface Manager
//# \prefix		InterfaceMgr/


#include "C4Types.h"


namespace C4
{
	class Setting;
	
	
	//# \class	Configurable	The base class for user-configurable objects.
	//
	//# Objects that need to display a user configuration interface are subclasses of the $Configurable$ class.
	//
	//# \def	class Configurable
	//
	//# \ctor	Configurable();
	//
	//# \desc
	//# The $Configurable$ class is a base class used by all user-configurable objects. It contains virtual functions that are
	//# used for getting and setting properties that can be changed by the user in the World Editor.
	//
	//# \also	$@Setting@$
	
	
	//# \function	Configurable::GetSettingCount		Called to retrieve the number of settings that a configurable object exposes.
	//
	//# \proto	virtual int32 GetSettingCount(void) const;
	//
	//# \desc
	//# The $GetSettingCount$ function is called to retrieve the number of settings that a configurable object exposes.
	//# The value returned by an override implementation should be the number of separate settings than can be returned by
	//# the $@Configurable::GetSetting@$ function.
	//
	//# \also	$@Configurable::GetSetting@$
	//# \also	$@Configurable::SetSetting@$
	
	
	//# \function	Configurable::GetSetting		Called to retrieve a particular setting for a configurable object.
	//
	//# \proto	virtual Setting *GetSetting(int32 index) const;
	//
	//# \param	index	The index of the setting to retrieve from the configurable object.
	//
	//# \desc
	//# The $GetSetting$ function is called to retrieve a particular setting for a configurable object. An override implementation
	//# of this function should allocate a new $@Setting@$ object corresponding the the setting index given by the $index$
	//# parameter and return a pointer to it. This function is called <i>n</i> times when building a configuration interface,
	//# where <i>n</i> is the number of settings returned by the $@Configurable::GetSettingCount@$ function. The value of $index$
	//# ranges from 0 to <i>n</i>&nbsp;&minus;&nbsp;1.
	//#
	//# The $GetSetting$ function is allowed to return $nullptr$ in the case that no setting should be presented to the user
	//# for a particular index.
	//
	//# \also	$@Configurable::GetSettingCount@$
	//# \also	$@Configurable::SetSetting@$
	//# \also	$@Setting@$
	
	
	//# \function	Configurable::SetSetting		Called to update a particular setting for a configurable object.
	//
	//# \proto	virtual void SetSetting(const Setting *setting);
	//
	//# \param	setting		A pointer to the setting whose value should be updated in the configurable object.
	//
	//# \desc
	//# The $SetSetting$ function is called to update a particular setting for a configurable object. An override implementation
	//# of this function should examine the setting identifier, retrieved with the $@Setting::GetSettingIdentifier@$ function,
	//# and update the object setting to which it corresponds to the value stored in the $@Setting@$ object. This function is
	//# called once for each setting returned by the $@Configurable::GetSetting@$ function when committing a configuration interface.
	//
	//# \also	$@Configurable::GetSettingCount@$
	//# \also	$@Configurable::GetSetting@$
	//# \also	$@Setting@$
	
	
	//# \function	Configurable::BeginSettings		Called when the settings for a configurable object are about to be updated.
	//
	//# \proto	virtual void *BeginSettings(void);
	//
	//# \desc
	//# The $BeginSettings$ function is called right before the settings for a configurable object are going to be updated
	//# through calls to the $@Configurable::SetSetting@$ function. After all of the settings have been updated, the
	//# $@Configurable::EndSettings@$ function is called. Implementing this pair of functions allows an object to take
	//# specific actions when it is being reconfigured by the user in the World Editor.
	//#
	//# The pointer returned by the $BeginSettings$ function is passed to the $cookie$ parameter of the $EndSettings$ function.
	//# This pointer is for use by the implementation of overriding class and is not used by the engine. For example,
	//# an implementation may allocate a data structure to hold some previous settings, return a pointer to that structure
	//# from the $BeginSettings$ function, and then use the data is holds in the $EndSettings$ function to determine if
	//# specific settings have changed that require some additional action to be taken. (The $EndSettings$ function should
	//# then deallocate the data structure, if necessary.)
	//
	//# \also	$@Configurable::EndSettings@$
	//# \also	$@Configurable::SetSetting@$
	 
	
	//# \function	Configurable::EndSettings		Called when the settings for a configurable object have finished being updated. 
	// 
	//# \proto	virtual void EndSettings(void *cookie); 
	//
	//# \param	cookie	The pointer previously returned by the $@Configurable::BeginSettings@$ function. 
	//
	//# \desc
	//# The $EndSettings$ function is called right after the settings for a configurable object are updated through calls
	//# to the $@Configurable::SetSetting@$ function. Before any of the settings are updated, the $@Configurable::BeginSettings@$ 
	//# function is called. Implementing this pair of functions allows an object to take specific actions when it is being
	//# reconfigured by the user in the World Editor.
	//#
	//# The $cookie$ parameter holds the pointer that the implementation previously returned from the $BeginSettings$ function. 
	//# If the implementation allocated memory in the $BeginSettings$ function and returned a pointer to it, then that memory
	//# should be released before the $EndSettings$ function returns.
	//
	//# \also	$@Configurable::BeginSettings@$
	//# \also	$@Configurable::SetSetting@$
	
	
	class C4_API Configurable
	{
		public:
			
			virtual int32 GetSettingCount(void) const;
			virtual Setting *GetSetting(int32 index) const;
			virtual void SetSetting(const Setting *setting);
			
			virtual int32 GetCategoryCount(void) const;
			virtual Type GetCategoryType(int32 index, const char **title) const;
			virtual int32 GetCategorySettingCount(Type category) const;
			virtual Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			virtual void SetCategorySetting(Type category, const Setting *setting);
			
			virtual void *BeginSettings(void);
			virtual void EndSettings(void *cookie);
	};
}


#endif

// ZYURVUR
