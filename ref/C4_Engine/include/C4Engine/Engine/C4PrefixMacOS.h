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


#include <memory>

#define __CF_USE_FRAMEWORK_INCLUDES__
#define GL_GLEXT_LEGACY

#include <AudioToolbox/AUGraph.h>
#include <AudioUnit/AudioUnit.h>
#include <AGL/agl.h>
#include <IOKit/IOCFPlugIn.h>
#include <IOKit/hid/IOHIDLib.h>
#include <IOKit/hid/IOHIDUsageTables.h>
#include <QuickTime/QuickTime.h>
#include <libkern/OSAtomic.h>
#include <sys/syslimits.h>
#include <sys/stat.h>
#include <sys/sysctl.h>
#include <sys/ioctl.h>
#include <mach/mach_time.h>
#include <pthread.h>
#include <dirent.h>
#include <unistd.h>
#include <netinet/in.h>
#include <netdb.h>
#include <net/if.h>
#include <ifaddrs.h>
#include <dlfcn.h>


#undef CompareText
#undef InstallWindowEventHandler


#include <new>

// ZYURVUR
