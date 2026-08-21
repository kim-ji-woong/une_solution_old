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


#define GL_GLEXT_LEGACY
#define XK_MISCELLANY

#include <sys/stat.h>
#include <sys/sysctl.h>
#include <sys/ioctl.h>
#include <sys/time.h>
#include <time.h>
#include <fcntl.h>
#include <pthread.h>
#include <dirent.h>
#include <unistd.h>
#include <netinet/in.h>
#include <netdb.h>
#include <net/if.h>
#include <ifaddrs.h>
#include <dlfcn.h>
#include <cpuid.h>
#include <signal.h>
#include <errno.h>
#include <xmmintrin.h>
#include <GL/glx.h>
#include <X11/extensions/Xrandr.h>
#include <X11/keysymdef.h>
#include <alsa/asoundlib.h>
#include <math.h>
#include <new>

#undef Complex

// ZYURVUR
