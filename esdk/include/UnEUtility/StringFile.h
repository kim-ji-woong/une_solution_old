#pragma once

#include <iostream>
#include <string>

class StringFile
{
public:
	StringFile(std::wstring strData);
	StringFile(std::string strData);
	virtual ~StringFile(void);

public:
	void SetData(std::wstring strData);
	void SetDataA(std::string strData);
	std::string ReadLineA(bool& isSuccess);
	std::wstring ReadLine(bool& isSuccess);

protected:
	std::wstring m_wstrData;
	std::string m_strData;
	int m_nCurrentIndex;
};

