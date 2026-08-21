/*************************************************************************/
/*	File : GeFileLog.h													 */
/*																		 */	
/*	Author : Guillaume CAURANT 											 */	
/*	Last Modification : 20/04/2005										 */	
/*																		 */	
/*	Interface for the GeFileLog class.									 */
/*************************************************************************/

#if !defined(AFX_GEFILELOG_H__FADF2164_B700_4186_8894_AC3F5C9A4B40__INCLUDED_)
#define AFX_GEFILELOG_H__FADF2164_B700_4186_8894_AC3F5C9A4B40__INCLUDED_

#include "GeLog.h"
#include "CKAll.h"

// Log in a file
class GeFileLog : public GeLog
{
public:
	GeFileLog();
	GeFileLog(XString iFileName);
	virtual ~GeFileLog();


	virtual CKBOOL InitLog();
	virtual CKBOOL CloseLog();
	virtual void OutputNewLine();
	virtual void OutputSeparator();
	virtual void OutputMessage(char* iMsg);
	virtual void Format(char *format, ...);
	virtual void VFormat(char *format, va_list v);
private:
	FILE*	m_LogFile;
	XString	m_FileName;
};

#endif // !defined(AFX_GEFILELOG_H__FADF2164_B700_4186_8894_AC3F5C9A4B40__INCLUDED_)
