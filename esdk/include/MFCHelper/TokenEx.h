#if !defined(AFX_TOKENEX_H__7CEAEBEE_D15D_11D2_9CA9_444553540000__INCLUDED_)
#define AFX_TOKENEX_H__7CEAEBEE_D15D_11D2_9CA9_444553540000__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/// \brief Gpln에서 쓰이는 문자열을 파싱하는 클래스

// esdk/incude/mfchelper/tokenex

class CTokenEx  
{
public:
	CTokenEx();
	~CTokenEx();

   	/*
     @FUNCTION: Splits a Path into 4 parts (Directory, Drive, Filename, Extention).
          NOTE: Supports UNC path names.
       @PARAM1: Whether the Supplied Path (PARAM2) contains a directory name only
                or a file name (Reason: some directories will end with "xxx.xxx"
                which is like a file name).
       @PARAM2: Path to Split.
       @PARAM3: (Referenced) Directory.
       @PARAM4: (Referenced) Drive.
       @PARAM5: (Referenced) Filename.
       @PARAM6: (Referenced) Extention.
	*/
	void SplitPath(BOOL UsingDirsOnly, CString Path, CString& Drive, CString& Dir, CString& FName, CString& Ext);

	/*
     @FUNCTION: Splits a CString into an CStringArray according the Deliminator.
	      NOTE: Supports UNC path names.
       @PARAM1: Source string to be Split.
       @PARAM2: Deliminator.
       @PARAM3: (Referenced) CStringArray to Add to.
       @PARAM4: Add empty strings for deliminators found 
	            (e.g.: test,test1,test2,,test3,test4).
				                       /\
									   ||
				this is added as ''=====

    */
	void Split(CString Source, CString Deliminator, CStringArray& AddIt, BOOL bAddEmpty);
  
	/*
     @FUNCTION: Joins a CStringArray to create a CString according the Deliminator.
       @PARAM1: Deliminator.
       @PARAM2: (Referenced) CStringArray to Add to.
    */
	CString Join(CString Deliminator, CStringArray& AddIt);

	/*
     @FUNCTION: Returns the first string found (according to the token given). If the
	            string does not contain a token, then it returns the entire string. 
	      NOTE: This uses a REFERENCE as the first param, so everytime it finds
	            a token, the string (& deliminator) are removed from the referenced
	            string.  SEE CODE EXAMPLE!
       @PARAM1: (Referenced) csRefered.
       @PARAM2: Deliminator.
       @PARAM3: Add empty strings for deliminators found 
	            (e.g.: test,test1,test2,,test3,test4).
				                       /\
									   ||
				this is returned as ''==

    */
	CString GetString(CString &csRefered, CString Deliminator, BOOL bAddEmpty);

protected:

};

#endif // !defined(AFX_TOKENEX_H__7CEAEBEE_D15D_11D2_9CA9_444553540000__INCLUDED_)
