#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

EntityManager::EntityManager(void)
{
	m_pEntity = 0;
	MakeHatchPattern();
}

EntityManager::~EntityManager(void)
{
	Clear();
	m_bDeleted = true;
}

void EntityManager::Clear()
{
	if (m_bDeleted) return;
	if (m_pOwner)
	{
		if (m_pOwner->GetEntityManager() != this) return;
	}
	std::list<Entity*>::const_iterator p = m_list.begin();

	int nCount = 0;
	while (p != m_list.end())
	{
		nCount++;
		Entity* pEnt = *p;
		p++;

		if (pEnt->GetHandle() >= 0)	delete pEnt;
	}

	m_list.clear();
}

void EntityManager::AddEntity(Entity* pEnt)
{
	m_list.push_back(pEnt);
}

void EntityManager::Write(Utility::FileManager* pMgr)
{
	wchar_t strDefault[32];
	swprintf_s(strDefault,L"0\r\nSECTION\r\n2\r\nENTITIES\r\n");
	pMgr->Write(strDefault,0,FILE_CURRENT);

	std::list<Entity*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Entity* pEnt = *p;
		p++;
		pEnt->Write(pMgr);
	}

	AddLine(pMgr,0,L"ENDSEC");
}

// pID : Entity 정보를 담고 있는 링크드 리스트 노드의 포인터
Entity* EntityManager::GetEntity(void*& pID)
{
	//static std::list<Entity*>::iterator p;
	std::list<Entity*>::iterator& p = m_entIter;

	if (pID == 0) p = m_list.begin();
	else
	{
		p = *(std::list<Entity*>::iterator*)pID;
	}

	if (p != m_list.end())
	{
		Entity* pEntity = *p;
		p++;
		pID = &p;

		if (pEntity->GetHandle() < 0) return 0;
		return pEntity;
	}

	return 0;
}

// nHandle을 소유한 Entity를 찾아낸다.
Entity* EntityManager::GetEntity(int nHandle)
{
	std::list<Entity*>::iterator pIter = m_list.begin();
	std::list<Entity*>::iterator pEnd = m_list.end();

	while (pIter != pEnd)
	{
		Entity* pEntity = *pIter;
		if (pEntity->GetHandle() == nHandle) return pEntity;
		++pIter;
	}

	return 0;
}

void EntityManager::ReadDatai(int nCode, int nData)
{
	if (m_pEntity) m_pEntity->ReadDatai(nCode,nData);
}

void EntityManager::ReadDatad(int nCode, double dData)
{
	if (m_pEntity) m_pEntity->ReadDatad(nCode,dData);
}

void EntityManager::ReadDatas(int nCode, wchar_t* strData)
{
	if (nCode == 0)
	{
		if (!wcscmp(strData,L"ARC")) m_pEntity = new DXF::ENTITIES::Arc;
		else if (!wcscmp(strData,L"CIRCLE"))	m_pEntity = new Circle;
		else if (!wcscmp(strData,L"ELLIPSE")) m_pEntity = new Ellipse;
		else if (!wcscmp(strData,L"HATCH")) m_pEntity = new Hatch;
		else if (!wcscmp(strData,L"LINE")) m_pEntity = new Line;
		else if (!wcscmp(strData, L"POINT")) m_pEntity = new Point;
		else if (!wcscmp(strData,L"LWPOLYLINE"))	m_pEntity = new PolyLine(true);
		else if (!wcscmp(strData,L"POLYLINE"))	m_pEntity = new PolyLine(false);
		else if (!wcscmp(strData,L"TEXT")) m_pEntity = new Text;
		else if (!wcscmp(strData,L"MTEXT")) m_pEntity = new MText;
		else if (!wcscmp(strData,L"INSERT")) m_pEntity = new Insert;
		else if (!wcscmp(strData,L"VERTEX"))
		{
			if (m_pEntity && !wcscmp(m_pEntity->GetEntityType(),L"POLYLINE"))
			{
				PolyLine* pPolyLine = (PolyLine*)m_pEntity;
				pPolyLine->ReadVertex(true);
				return;
			}
			else
			{
				m_pEntity = 0;
				return;
			}
		}
		else if (!wcscmp(strData,L"ENDSEC"))
		{
			m_pEntity = 0;
			return;
		}
		else
		{
			//if (m_pEntity) m_pEntity->ReadDatas(nCode,strData);
			m_pEntity = 0;
			return;
		}

		m_pEntity->SetManager(this);
		m_list.push_back(m_pEntity);
	}
	else
	{
		if (m_pEntity) m_pEntity->ReadDatas(nCode,strData);
	}
}

void EntityManager::MakeHatchPattern()
{
	m_nHatchPatternSize = 21;
	
	m_arrHatchPatternGroup[0].m_strPatternName = L"AR-CONC";
	Hatch::HatchPattern pattern;

	pattern.SetData(50.0, 0.0, 0.0, 45.546167249001, -3.984771384725771);
	pattern.m_listDashLength.push_back(4.7625);
	pattern.m_listDashLength.push_back(-52.3875);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(355.0, 0.0, 0.0, -8.810706493916747, 47.76416072054499);
	pattern.m_listDashLength.push_back(3.81);
	pattern.m_listDashLength.push_back(-41.91);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(100.451, 3.7955, -0.3320625, 36.73562541648981, 43.77917195507296);
	pattern.m_listDashLength.push_back(4.0475);
	pattern.m_listDashLength.push_back(-44.5225);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(46.1842, 0.0, 12.7, 67.76976022303248, -10.51058198318179);
	pattern.m_listDashLength.push_back(7.14375);
	pattern.m_listDashLength.push_back(-78.58125);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(96.6356, 5.647475, 11.824125, 59.35105861655928, 61.8563889725123);
	pattern.m_listDashLength.push_back(6.07125);
	pattern.m_listDashLength.push_back(-66.78375);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(351.184, 0.0, 12.7, 59.35115145348348, 61.85627946514942);
	pattern.m_listDashLength.push_back(5.715);
	pattern.m_listDashLength.push_back(-62.865);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(21.0, 6.35, 9.525, 37.90371995884582, -25.56637962919062);
	pattern.m_listDashLength.push_back(4.7625);
	pattern.m_listDashLength.push_back(-52.3875);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(326.0, 6.35, 9.525, 15.45050708691177, 46.04699153057981);
	pattern.m_listDashLength.push_back(3.81);
	pattern.m_listDashLength.push_back(-41.91);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(71.4514, 9.508625, 7.394475, 53.35412269253112, 20.48071442901436);
	pattern.m_listDashLength.push_back(4.0475);
	pattern.m_listDashLength.push_back(-44.5225);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(37.5, 0.0, 0.0, 0.7721508126479542, 21.13875971828828);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-41.402);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-42.545);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-42.06875);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(7.5, 0.0, 0.0, 16.70491562757644, 25.04514370452988);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-24.257);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-40.4495);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-16.03375);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(327.5, -14.1605, 0.0, 33.89764879171785, -1.432185998168364);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-15.875);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-49.53);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-65.7225);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(317.5, -20.5105, 0.0, 37.03229534650451, 6.356622758333253);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-20.6375);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-32.893);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-46.6725);
	m_arrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[1].m_strPatternName = L"AR-HBONE";

	pattern.SetData(45.0, 0.0, 0.0, 0.0, 35.92102448427662);
	pattern.m_listDashLength.push_back(76.2);
	pattern.m_listDashLength.push_back(-25.4);
	m_arrHatchPatternGroup[1].m_listPattern.push_back(pattern);

	pattern.SetData(135.0, 17.9605, 17.9605, 0.0, 35.92102448427662);
	pattern.m_listDashLength.push_back(76.2);
	pattern.m_listDashLength.push_back(-25.4);
	m_arrHatchPatternGroup[1].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[2].m_strPatternName = L"AR-SAND";

	pattern.SetData(37.5, 0.0, 0.0, -0.4000078239960115, 12.23533093323356);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-9.652);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-10.795);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-10.31875);
	m_arrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pattern.SetData(7.5, 0.0, 0.0, 11.23808207845006, 17.92062751420886);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-5.207);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-8.6995);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-3.33375);
	m_arrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pattern.SetData(327.5, -7.8105, 0.0, 19.77480925603645, 0.0359296169636423);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-3.175);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-11.43);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-14.9225);
	m_arrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pattern.SetData(317.5, -7.8105, 0.0, 19.08891131180743, 5.573233081430405);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-1.5875);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-7.493);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-8.5725);
	m_arrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[3].m_strPatternName = L"CLAY";

	pattern.SetData(0.0, 0.0, 0.0, 0.0, 9.525);
	m_arrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pattern.SetData(0.0, 0.0, 1.5875, 0.0, 9.525);
	m_arrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pattern.SetData(0.0, 0.0, 3.175, 0.0, 9.525);
	m_arrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pattern.SetData(0.0, 0.0, 6.35, 0.0, 9.525);
	pattern.m_listDashLength.push_back(9.525);
	pattern.m_listDashLength.push_back(-6.35);
	m_arrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[4].m_strPatternName = L"BRICK";

	pattern.SetData(0.0, 0.0, 0.0, 0.0, 25.4);
	m_arrHatchPatternGroup[4].m_listPattern.push_back(pattern);

	pattern.SetData(90.0, 0.0, 0.0, -50.8, 0.0);
	pattern.m_listDashLength.push_back(25.4);
	pattern.m_listDashLength.push_back(-25.4);
	m_arrHatchPatternGroup[4].m_listPattern.push_back(pattern);

	pattern.SetData(90.0, 25.4, 0.0, -50.8, 0.0);
	pattern.m_listDashLength.push_back(-25.4);
	pattern.m_listDashLength.push_back(25.4);
	m_arrHatchPatternGroup[4].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[5].m_strPatternName = L"GRAVEL";

	pattern.SetData(228.0128, 73.152, 101.6, -812.8006906358404, -914.4011235805031);
	pattern.m_listDashLength.push_back(13.66884);
	pattern.m_listDashLength.push_back(-1353.22);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(184.9697, 64.008, 91.44, 1219.201972247165, 101.5992830154983);
	pattern.m_listDashLength.push_back(23.4562);
	pattern.m_listDashLength.push_back(-2322.16);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(132.5104, 40.64, 89.408, 1015.999260733267, -1117.601091698416);
	pattern.m_listDashLength.push_back(16.53928);
	pattern.m_listDashLength.push_back(-1637.388);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(267.2737, 1.016, 64.008, 101.5995588331269, 2031.999346134657);
	pattern.m_listDashLength.push_back(21.3602);
	pattern.m_listDashLength.push_back(-2114.656);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(292.8337, 0.0, 42.672, -508.0014422200761, 1219.200693202044);
	pattern.m_listDashLength.push_back(20.94536);
	pattern.m_listDashLength.push_back(-2073.592);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(357.2737, 8.128, 23.368, -2031.999346134657, 101.599558833127);
	pattern.m_listDashLength.push_back(21.3602);
	pattern.m_listDashLength.push_back(-2114.656);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(37.6942, 29.464, 22.352, -1320.800834449597, -1015.999094279395);
	pattern.m_listDashLength.push_back(28.24776);
	pattern.m_listDashLength.push_back(-2796.524);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(72.2553, 51.816, 39.624, 711.2014226186878, 2235.200648241456);
	pattern.m_listDashLength.push_back(26.6688);
	pattern.m_listDashLength.push_back(-2640.212);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(121.4296, 59.944, 65.024, -812.8002094105791, 1320.798446398907);
	pattern.m_listDashLength.push_back(21.43252);
	pattern.m_listDashLength.push_back(-2121.82);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(175.2364, 48.768, 83.312, 1117.599122122178, -101.5990465088462);
	pattern.m_listDashLength.push_back(24.46852);
	pattern.m_listDashLength.push_back(-1198.956);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(222.3974, 24.384, 85.344, -1219.200036689588, -1117.598546025479);
	pattern.m_listDashLength.push_back(31.64312);
	pattern.m_listDashLength.push_back(-3132.672);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(138.8141, 101.6, 62.992, -711.1995469689894, 609.5990622849601);
	pattern.m_listDashLength.push_back(10.8002);
	pattern.m_listDashLength.push_back(-1069.224);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(171.4692, 93.472, 70.104, 1320.797983823656, -203.200512001828);
	pattern.m_listDashLength.push_back(20.54728);
	pattern.m_listDashLength.push_back(-2034.184);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(225.0, 73.152, 73.152, 0.0, -101.5999307480079);
	pattern.m_listDashLength.push_back(14.36836);
	pattern.m_listDashLength.push_back(-129.3156);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(203.1986, 66.04, 85.344, 508.0017130236332, 203.2008909722506);
	pattern.m_listDashLength.push_back(7.73764);
	pattern.m_listDashLength.push_back(-766.024);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(291.8014, 58.928, 82.296, -101.6000712757803, 304.8001963492746);
	pattern.m_listDashLength.push_back(10.94264);
	pattern.m_listDashLength.push_back(-536.192);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(30.9638, 62.992, 72.136, 304.799678192329, 203.2001583464221);
	pattern.m_listDashLength.push_back(17.7728);
	pattern.m_listDashLength.push_back(-574.652);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(161.5651, 78.232, 81.28, 203.2000927272393, -101.599848898603);
	pattern.m_listDashLength.push_back(12.85148);
	pattern.m_listDashLength.push_back(-308.436);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(16.3895, 0.0, 82.296, 1016.000560558774, 304.7993659987122);
	pattern.m_listDashLength.push_back(18.00352);
	pattern.m_listDashLength.push_back(-1782.352);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(70.3462, 17.272, 87.376, -406.3988675470603, -1117.5984195418);
	pattern.m_listDashLength.push_back(15.10396);
	pattern.m_listDashLength.push_back(-1495.288);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(293.1986, 78.232, 101.6, -203.2008909722506, 508.0017130236333);
	pattern.m_listDashLength.push_back(15.4752);
	pattern.m_listDashLength.push_back(-758.288);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(343.6105, 84.32799999999998, 87.376, -1016.000560558774, 304.7993659987124);
	pattern.m_listDashLength.push_back(18.00352);
	pattern.m_listDashLength.push_back(-1782.352);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(339.444, 0.0, 19.304, -508.0006305702363, 203.1997534420055);
	pattern.m_listDashLength.push_back(17.3614);
	pattern.m_listDashLength.push_back(-850.708);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(294.7751, 16.256, 13.208, -507.9988161450735, 1117.599481046267);
	pattern.m_listDashLength.push_back(14.54688);
	pattern.m_listDashLength.push_back(-1440.144);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(66.8014, 79.248, 0.0, 203.2008909722507, 508.0017130236332);
	pattern.m_listDashLength.push_back(15.4752);
	pattern.m_listDashLength.push_back(-758.288);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(17.354, 85.344, 14.224, -1320.800071274398, -406.3993870242253);
	pattern.m_listDashLength.push_back(17.03132);
	pattern.m_listDashLength.push_back(-1686.096);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(69.444, 29.464, 0.0, -203.1997534420058, -508.0006305702363);
	pattern.m_listDashLength.push_back(8.68072);
	pattern.m_listDashLength.push_back(-859.3919999999998);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(101.3099, 73.152, 0.0, -101.5994985154939, 406.3986984361048);
	pattern.m_listDashLength.push_back(5.1806);
	pattern.m_listDashLength.push_back(-512.88);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(165.9638, 72.136, 5.08, 304.8002141878869, -101.5998241649456);
	pattern.m_listDashLength.push_back(20.94536);
	pattern.m_listDashLength.push_back(-397.962);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(186.009, 51.816, 10.16, 1015.998501777118, 101.5997627419143);
	pattern.m_listDashLength.push_back(19.41068);
	pattern.m_listDashLength.push_back(-1921.656);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(303.6901, 62.992, 62.992, -101.6000348522893, 203.1998039692736);
	pattern.m_listDashLength.push_back(14.65296);
	pattern.m_listDashLength.push_back(-351.6712);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(353.1572, 71.12, 50.8, 1727.200714265737, -203.2009100661936);
	pattern.m_listDashLength.push_back(25.58228);
	pattern.m_listDashLength.push_back(-2532.64);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(60.9454, 96.52, 47.752, -406.3990546331636, -711.198304433365);
	pattern.m_listDashLength.push_back(10.46032);
	pattern.m_listDashLength.push_back(-1035.576);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(90.0, 101.6, 56.896, -101.6, 101.6);
	pattern.m_listDashLength.push_back(6.096);
	pattern.m_listDashLength.push_back(-95.504);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(120.2564, 49.784, 13.208, 406.3991386734444, -711.1996412991024);
	pattern.m_listDashLength.push_back(14.11468);
	pattern.m_listDashLength.push_back(-1397.356);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(48.0128, 42.672, 25.4, 812.8006906358402, 914.4011235805031);
	pattern.m_listDashLength.push_back(27.33772);
	pattern.m_listDashLength.push_back(-1339.552);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(0.0, 60.96, 45.72, 101.6, 101.6);
	pattern.m_listDashLength.push_back(26.416);
	pattern.m_listDashLength.push_back(-75.184);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(325.3048, 87.376, 45.72, -1015.998239638147, 711.2000676193905);
	pattern.m_listDashLength.push_back(16.0644);
	pattern.m_listDashLength.push_back(-1590.372);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(254.0546, 100.584, 36.576, -101.6002591880589, -406.4009681281451);
	pattern.m_listDashLength.push_back(14.79316);
	pattern.m_listDashLength.push_back(-724.868);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(207.646, 96.52, 22.352, -1930.399321960909, -1016.000638988936);
	pattern.m_listDashLength.push_back(24.0858);
	pattern.m_listDashLength.push_back(-2384.5);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(175.4261, 75.184, 11.176, -1320.8003041899, 101.5995084487784);
	pattern.m_listDashLength.push_back(25.48116);
	pattern.m_listDashLength.push_back(-2522.632);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(175.4261, 75.184, 11.176, -1320.8003041899, 101.5995084487784);
	pattern.m_listDashLength.push_back(25.48116);
	pattern.m_listDashLength.push_back(-2522.632);
	m_arrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[6].m_strPatternName = L"HONEY";

	pattern.SetData(0.0, 0.0, 0.0, 19.05, 10.99852);
	pattern.m_listDashLength.push_back(12.7);
	pattern.m_listDashLength.push_back(-25.4);
	m_arrHatchPatternGroup[6].m_listPattern.push_back(pattern);

	pattern.SetData(120.0, 0.0, 0.0, -19.04999772403122, 10.99852394209356);
	pattern.m_listDashLength.push_back(12.7);
	pattern.m_listDashLength.push_back(-25.4);
	m_arrHatchPatternGroup[6].m_listPattern.push_back(pattern);

	pattern.SetData(60.0, 0.0, 0.0, 0.0000022759687788, 21.99704394209355);
	pattern.m_listDashLength.push_back(-25.4);
	pattern.m_listDashLength.push_back(12.7);
	m_arrHatchPatternGroup[6].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[7].m_strPatternName = L"JIS_LC_20";

	pattern.SetData(45.0, 0.0, 0.0, -56.56854249492379, 56.5685424949238);
	m_arrHatchPatternGroup[7].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 1.6, 0.0, -56.56854249492379, 56.5685424949238);
	m_arrHatchPatternGroup[7].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[8].m_strPatternName = L"STEEL";

	pattern.SetData(45.0, 0.0, 0.0, -8.980256121069151, 8.980256121069153);
	m_arrHatchPatternGroup[8].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 0.0, 6.35, -8.980256121069151, 8.980256121069153);
	m_arrHatchPatternGroup[8].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[9].m_strPatternName = L"ANSI31";

	pattern.SetData(45.0, 0.0, 0.0, -8.980256121069151, 8.980256121069153);
	m_arrHatchPatternGroup[9].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[10].m_strPatternName = L"ANSI32";

	pattern.SetData(45.0, 0.0, 0.0, -26.94076836320747, 26.94076836320747);
	m_arrHatchPatternGroup[10].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 13.47039, 0.0, -26.94076836320747, 26.94076836320747);
	m_arrHatchPatternGroup[10].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[11].m_strPatternName = L"ANSI33";

	pattern.SetData(45.0, 0.0, 0.0, -17.960512242138293, 17.960512242138293);
	m_arrHatchPatternGroup[11].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 13.47039, 0.0, -17.960512242138293, 17.960512242138293);
	pattern.m_listDashLength.push_back(9.525);
	pattern.m_listDashLength.push_back(-4.7625);
	m_arrHatchPatternGroup[11].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[12].m_strPatternName = L"ANSI34";

	pattern.SetData(45.0, 0.0, 0.0, -53.88153672641492, 53.88153672641492);
	m_arrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 13.47039, 0.0, -53.88153672641492, 53.88153672641492);
	m_arrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 26.94078, 0.0, -53.88153672641492, 53.88153672641492);
	m_arrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 40.4112, 0.0, -53.88153672641492, 53.88153672641492);
	m_arrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[13].m_strPatternName = L"ANSI35";

	pattern.SetData(45.0, 0.0, 0.0, -17.960512242138293, 17.960512242138293);
	m_arrHatchPatternGroup[13].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 13.47039, 0.0, -17.960512242138293, 17.960512242138293);
	pattern.m_listDashLength.push_back(23.8125);
	pattern.m_listDashLength.push_back(-4.7625);
	pattern.m_listDashLength.push_back(-4.7625);
	pattern.m_listDashLength.push_back(0.6246936655556858);
	m_arrHatchPatternGroup[13].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[14].m_strPatternName = L"ANSI36";

	pattern.SetData(45.0, 0.0, 0.0, 6.7351920908018693, 24.695704332940173);
	pattern.m_listDashLength.push_back(23.8125);
	pattern.m_listDashLength.push_back(-4.7625);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-4.7625);
	m_arrHatchPatternGroup[14].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[15].m_strPatternName = L"ANSI37";

	pattern.SetData(45.0, 0.0, 0.0, -8.9802561210691507, 8.9802561210691507);
	m_arrHatchPatternGroup[15].m_listPattern.push_back(pattern);

	pattern.SetData(135.0, 0.0, 0.0, -8.9802561210691507, -8.9802561210691507);
	m_arrHatchPatternGroup[15].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[16].m_strPatternName = L"ANSI38";

	pattern.SetData(45.0, 0.0, 0.0, -8.9802561210691507, 8.9802561210691507);
	m_arrHatchPatternGroup[16].m_listPattern.push_back(pattern);

	pattern.SetData(135.0, 0.0, 0.0, -26.94076836320747, 8.9802561210691547);
	pattern.m_listDashLength.push_back(23.8125);
	pattern.m_listDashLength.push_back(-14.2875);
	m_arrHatchPatternGroup[16].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[17].m_strPatternName = L"JIS_RC_10";

	pattern.SetData(45.0, 0.0, 0.0, -28.284271247461893, 28.284271247461893);
	m_arrHatchPatternGroup[17].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 2.175, 0.0, -28.284271247461893, 28.284271247461893);
	m_arrHatchPatternGroup[17].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 4.35, 0.0, -28.284271247461893, 28.284271247461893);
	m_arrHatchPatternGroup[17].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[18].m_strPatternName = L"JIS_RC_15";

	pattern.SetData(45.0, 0.0, 0.0, -42.42640687119284, 42.42640687119284);
	m_arrHatchPatternGroup[18].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 2.175, 0.0, -42.42640687119284, 42.42640687119284);
	m_arrHatchPatternGroup[18].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 4.35, 0.0, -42.42640687119284, 42.42640687119284);
	m_arrHatchPatternGroup[18].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[19].m_strPatternName = L"JIS_RC_18";

	pattern.SetData(45.0, 0.0, 0.0, -50.911688245431413, 50.911688245431413);
	m_arrHatchPatternGroup[19].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 3.0, 0.0, -50.911688245431413, 50.911688245431413);
	m_arrHatchPatternGroup[19].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 6.0, 0.0, -50.911688245431413, 50.911688245431413);
	m_arrHatchPatternGroup[19].m_listPattern.push_back(pattern);

	m_arrHatchPatternGroup[20].m_strPatternName = L"JIS_RC_30";

	pattern.SetData(45.0, 0.0, 0.0, -84.85281374238568, 84.85281374238568);
	m_arrHatchPatternGroup[20].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 3.0, 0.0, -84.85281374238568, 84.85281374238568);
	m_arrHatchPatternGroup[20].m_listPattern.push_back(pattern);

	pattern.SetData(45.0, 6.0, 0.0, -84.85281374238568, 84.85281374238568);
	m_arrHatchPatternGroup[20].m_listPattern.push_back(pattern);
}

Hatch::HatchPatternGroup* EntityManager::HatchPatternGroups()
{
	return m_arrHatchPatternGroup;
}

MText* EntityManager::FindFirstMText(MText* pText)
{
	int nHandle = pText->GetHandle();
	std::map<int, MText*>::iterator iter = m_mapTextOwner.find(nHandle);

	if (iter == m_mapTextOwner.end())
		return 0;

	m_tempMTextList.push_back(pText);
	return iter->second;
}

// MText 임시 객체들을 삭제한다.
void EntityManager::RemoveTempMText(BLOCKS::BlockManager* pBlockManager)
{
	pBlockManager->RemoveTempMText(m_tempMTextList);

	for (std::list<MText*>::iterator iter = m_tempMTextList.begin(); iter != m_tempMTextList.end(); iter++)
	{
		m_list.remove(*iter);
		delete *iter;
	}

	m_tempMTextList.clear();
}

void EntityManager::AddTempMText(MText* pText, int nHandle)
{
	m_mapTextOwner[nHandle] = pText;
}

END_NS
END_NS
