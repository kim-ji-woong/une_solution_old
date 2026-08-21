#ifndef _AUDIO_IO_H_
#define _AUDIO_IO_H_

#include <stdio.h>
#include <windows.h>
#include <mmsystem.h>
#include <memory.h>
#include <math.h>


typedef enum {
	AUD_OK,
	AUDERR_NOTINIT,
	AUDERR_FORMAT,
	AUDERR_OPEN,
	AUDERR_CLOSE,
	AUDERR_PREPAREHDR,
	AUDERR_ADDBUFFER,
	AUDERR_NOBUFFER,
	AUDERR_WRITE,
	AUDERR_MEM,
	AUDERR_START,
	AUDERR_RESTART,
	AUDERR_RESET,
	AUDERR_PAUSE,
	AUDERR_WAITING,
	AUDERR_SETVOLUME,
	AUDERR_GETVOLUME
} AUDERR_TYPE;


int InitAudioOut();
int UninitAudioOut();
bool SwitchControl(int stop_startl);
void AlrimiSwitchProtect();


#endif
