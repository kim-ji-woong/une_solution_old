/*************************************************************************/
/*	File :	GeLog.h	 													 */
/*																		 */	
/*	Author : Guillaume CAURANT 											 */	
/*	Last Modification : 20/04/2005										 */	
/*																		 */	
/*	Interface for Log class.											 */
/*************************************************************************/

#if !defined(AFX_GELOG_H__D1544EF6_49C7_43B0_AC71_C0F9542F3A3D__INCLUDED_)
#define AFX_GELOG_H__D1544EF6_49C7_43B0_AC71_C0F9542F3A3D__INCLUDED_

#include "CKAll.h"

class GeLog  
{
public:
	GeLog(){;}
	virtual ~GeLog(){;}

	/************************************************
	Summary: Initialize the log
	Return Value: true if success, false otherwise
	***********************************************/
	virtual CKBOOL InitLog() = 0;

	/************************************************
	Summary: Close the log
	Return Value: true if success, false otherwise
	***********************************************/
	virtual CKBOOL CloseLog() = 0;

	/************************************************************************
	Summary: Output an empty line
	************************************************************************/
	virtual void OutputNewLine() = 0;

	/************************************************************************
	Summary: Output a separator (a written line)
	************************************************************************/
	virtual void OutputSeparator() = 0;

	/************************************************************************
	Summary: Output a string message
	Argument: + iMsg - message to log
	************************************************************************/
	virtual void OutputMessage(char* iMsg) = 0;

	/************************************************************************
	Summary: Output a formated string message like printf
	Argument: + format - formated string
	************************************************************************/
	virtual void Format(char *format, ...)=0;

	/************************************************************************
	Summary: Output a formated string message like vprintf
	Argument: + format - formated string
	          + va_list -list of variables
	************************************************************************/
	virtual void VFormat(char *format, va_list v)=0;

};

#endif // !defined(AFX_GELOG_H__D1544EF6_49C7_43B0_AC71_C0F9542F3A3D__INCLUDED_)
