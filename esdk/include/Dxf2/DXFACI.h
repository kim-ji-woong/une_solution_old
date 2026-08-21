#pragma once
// AutoCAD Color Index

namespace PenWorld
{
	class ACI
	{
	public:
		ACI(int nACI, int nRed, int nGreen, int nBlue);
		~ACI(void);

	public:
		static bool ACIToRGB(int nACI, int* pRed, int* pGreen, int* pBlue);
		static bool RGBToACI(int nRed, int nGreen, int nBlue, int* pACI);

	public:
		int m_nACI;
		int m_nRed;
		int m_nGreen;
		int m_nBlue;
	};
}