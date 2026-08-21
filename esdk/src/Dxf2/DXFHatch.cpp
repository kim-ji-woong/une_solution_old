#include "stdafx.h"
#include <fstream>

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

static double DegToRad(double dDegree)
{
	return dDegree * 3.14159265358979323846 / 180.0;
}

static double RadToDeg(double dRadian)
{
	return dRadian * 180.0 / 3.14159265358979323846;
}

//int Hatch::m_nHatchPatternSize;
//Hatch::HatchPatternGroup* Hatch::m_pArrHatchPatternGroup = Hatch::MakeHatchPattern();

/*Hatch::HatchPatternGroup* Hatch::MakeHatchPattern()
{
	static bool bFirst = true;
	if (bFirst) bFirst = false;
	else return m_pArrHatchPatternGroup;

	m_nHatchPatternSize = 21;
	static Hatch::HatchPatternGroup pArrHatchPatternGroup[21];

	pArrHatchPatternGroup[0].m_strPatternName = "AR-CONC";
	Hatch::HatchPattern pattern;

	pattern.SetData(50.0,0.0,0.0,45.546167249001,-3.984771384725771);
	pattern.m_listDashLength.push_back(4.7625);
	pattern.m_listDashLength.push_back(-52.3875);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(355.0,0.0,0.0,-8.810706493916747,47.76416072054499);
	pattern.m_listDashLength.push_back(3.81);
	pattern.m_listDashLength.push_back(-41.91);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(100.451,3.7955,-0.3320625,36.73562541648981,43.77917195507296);
	pattern.m_listDashLength.push_back(4.0475);
	pattern.m_listDashLength.push_back(-44.5225);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(46.1842,0.0,12.7,67.76976022303248,-10.51058198318179);
	pattern.m_listDashLength.push_back(7.14375);
	pattern.m_listDashLength.push_back(-78.58125);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(96.6356,5.647475,11.824125,59.35105861655928,61.8563889725123);
	pattern.m_listDashLength.push_back(6.07125);
	pattern.m_listDashLength.push_back(-66.78375);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(351.184,0.0,12.7,59.35115145348348,61.85627946514942);
	pattern.m_listDashLength.push_back(5.715);
	pattern.m_listDashLength.push_back(-62.865);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(21.0,6.35,9.525,37.90371995884582,-25.56637962919062);
	pattern.m_listDashLength.push_back(4.7625);
	pattern.m_listDashLength.push_back(-52.3875);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(326.0,6.35,9.525,15.45050708691177,46.04699153057981);
	pattern.m_listDashLength.push_back(3.81);
	pattern.m_listDashLength.push_back(-41.91);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(71.4514,9.508625,7.394475,53.35412269253112,20.48071442901436);
	pattern.m_listDashLength.push_back(4.0475);
	pattern.m_listDashLength.push_back(-44.5225);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(37.5,0.0,0.0,0.7721508126479542,21.13875971828828);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-41.402);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-42.545);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-42.06875);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(7.5,0.0,0.0,16.70491562757644,25.04514370452988);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-24.257);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-40.4495);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-16.03375);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(327.5,-14.1605,0.0,33.89764879171785,-1.432185998168364);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-15.875);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-49.53);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-65.7225);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pattern.SetData(317.5,-20.5105,0.0,37.03229534650451,6.356622758333253);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-20.6375);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-32.893);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-46.6725);
	pArrHatchPatternGroup[0].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[1].m_strPatternName = "AR-HBONE";

	pattern.SetData(45.0,0.0,0.0,0.0,35.92102448427662);
	pattern.m_listDashLength.push_back(76.2);
	pattern.m_listDashLength.push_back(-25.4);
	pArrHatchPatternGroup[1].m_listPattern.push_back(pattern);

	pattern.SetData(135.0,17.9605,17.9605,0.0,35.92102448427662);
	pattern.m_listDashLength.push_back(76.2);
	pattern.m_listDashLength.push_back(-25.4);
	pArrHatchPatternGroup[1].m_listPattern.push_back(pattern);
	
	pArrHatchPatternGroup[2].m_strPatternName = "AR-SAND";

	pattern.SetData(37.5,0.0,0.0,-0.4000078239960115,12.23533093323356);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-9.652);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-10.795);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-10.31875);
	pArrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pattern.SetData(7.5,0.0,0.0,11.23808207845006,17.92062751420886);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-5.207);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-8.6995);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-3.33375);
	pArrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pattern.SetData(327.5,-7.8105,0.0,19.77480925603645,0.0359296169636423);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-3.175);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-11.43);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-14.9225);
	pArrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pattern.SetData(317.5,-7.8105,0.0,19.08891131180743,5.573233081430405);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-1.5875);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-7.493);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-8.5725);
	pArrHatchPatternGroup[2].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[3].m_strPatternName = "CLAY";

	pattern.SetData(0.0,0.0,0.0,0.0,9.525);
	pArrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pattern.SetData(0.0,0.0,1.5875,0.0,9.525);
	pArrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pattern.SetData(0.0,0.0,3.175,0.0,9.525);
	pArrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pattern.SetData(0.0,0.0,6.35,0.0,9.525);
	pattern.m_listDashLength.push_back(9.525);
	pattern.m_listDashLength.push_back(-6.35);
	pArrHatchPatternGroup[3].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[4].m_strPatternName = "BRICK";

	pattern.SetData(0.0,0.0,0.0,0.0,25.4);
	pArrHatchPatternGroup[4].m_listPattern.push_back(pattern);

	pattern.SetData(90.0,0.0,0.0,-50.8,0.0);
	pattern.m_listDashLength.push_back(25.4);
	pattern.m_listDashLength.push_back(-25.4);
	pArrHatchPatternGroup[4].m_listPattern.push_back(pattern);

	pattern.SetData(90.0,25.4,0.0,-50.8,0.0);
	pattern.m_listDashLength.push_back(-25.4);
	pattern.m_listDashLength.push_back(25.4);
	pArrHatchPatternGroup[4].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[5].m_strPatternName = "GRAVEL";

	pattern.SetData(228.0128,73.152,101.6,-812.8006906358404,-914.4011235805031);
	pattern.m_listDashLength.push_back(13.66884);
	pattern.m_listDashLength.push_back(-1353.22);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(184.9697,64.008,91.44,1219.201972247165,101.5992830154983);
	pattern.m_listDashLength.push_back(23.4562);
	pattern.m_listDashLength.push_back(-2322.16);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(132.5104,40.64,89.408,1015.999260733267,-1117.601091698416);
	pattern.m_listDashLength.push_back(16.53928);
	pattern.m_listDashLength.push_back(-1637.388);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(267.2737,1.016,64.008,101.5995588331269,2031.999346134657);
	pattern.m_listDashLength.push_back(21.3602);
	pattern.m_listDashLength.push_back(-2114.656);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(292.8337,0.0,42.672,-508.0014422200761,1219.200693202044);
	pattern.m_listDashLength.push_back(20.94536);
	pattern.m_listDashLength.push_back(-2073.592);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(357.2737,8.128,23.368,-2031.999346134657,101.599558833127);
	pattern.m_listDashLength.push_back(21.3602);
	pattern.m_listDashLength.push_back(-2114.656);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(37.6942,29.464,22.352,-1320.800834449597,-1015.999094279395);
	pattern.m_listDashLength.push_back(28.24776);
	pattern.m_listDashLength.push_back(-2796.524);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(72.2553,51.816,39.624,711.2014226186878,2235.200648241456);
	pattern.m_listDashLength.push_back(26.6688);
	pattern.m_listDashLength.push_back(-2640.212);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(121.4296,59.944,65.024,-812.8002094105791,1320.798446398907);
	pattern.m_listDashLength.push_back(21.43252);
	pattern.m_listDashLength.push_back(-2121.82);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(175.2364,48.768,83.312,1117.599122122178,-101.5990465088462);
	pattern.m_listDashLength.push_back(24.46852);
	pattern.m_listDashLength.push_back(-1198.956);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(222.3974,24.384,85.344,-1219.200036689588,-1117.598546025479);
	pattern.m_listDashLength.push_back(31.64312);
	pattern.m_listDashLength.push_back(-3132.672);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(138.8141,101.6,62.992,-711.1995469689894,609.5990622849601);
	pattern.m_listDashLength.push_back(10.8002);
	pattern.m_listDashLength.push_back(-1069.224);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(171.4692,93.472,70.104,1320.797983823656,-203.200512001828);
	pattern.m_listDashLength.push_back(20.54728);
	pattern.m_listDashLength.push_back(-2034.184);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(225.0,73.152,73.152,0.0,-101.5999307480079);
	pattern.m_listDashLength.push_back(14.36836);
	pattern.m_listDashLength.push_back(-129.3156);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(203.1986,66.04,85.344,508.0017130236332,203.2008909722506);
	pattern.m_listDashLength.push_back(7.73764);
	pattern.m_listDashLength.push_back(-766.024);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(291.8014,58.928,82.296,-101.6000712757803,304.8001963492746);
	pattern.m_listDashLength.push_back(10.94264);
	pattern.m_listDashLength.push_back(-536.192);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(30.9638,62.992,72.136,304.799678192329,203.2001583464221);
	pattern.m_listDashLength.push_back(17.7728);
	pattern.m_listDashLength.push_back(-574.652);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(161.5651,78.232,81.28,203.2000927272393,-101.599848898603);
	pattern.m_listDashLength.push_back(12.85148);
	pattern.m_listDashLength.push_back(-308.436);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(16.3895,0.0,82.296,1016.000560558774,304.7993659987122);
	pattern.m_listDashLength.push_back(18.00352);
	pattern.m_listDashLength.push_back(-1782.352);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(70.3462,17.272,87.376,-406.3988675470603,-1117.5984195418);
	pattern.m_listDashLength.push_back(15.10396);
	pattern.m_listDashLength.push_back(-1495.288);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(293.1986,78.232,101.6,-203.2008909722506,508.0017130236333);
	pattern.m_listDashLength.push_back(15.4752);
	pattern.m_listDashLength.push_back(-758.288);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(343.6105,84.32799999999998,87.376,-1016.000560558774,304.7993659987124);
	pattern.m_listDashLength.push_back(18.00352);
	pattern.m_listDashLength.push_back(-1782.352);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(339.444,0.0,19.304,-508.0006305702363,203.1997534420055);
	pattern.m_listDashLength.push_back(17.3614);
	pattern.m_listDashLength.push_back(-850.708);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(294.7751,16.256,13.208,-507.9988161450735,1117.599481046267);
	pattern.m_listDashLength.push_back(14.54688);
	pattern.m_listDashLength.push_back(-1440.144);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(66.8014,79.248,0.0,203.2008909722507,508.0017130236332);
	pattern.m_listDashLength.push_back(15.4752);
	pattern.m_listDashLength.push_back(-758.288);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(17.354,85.344,14.224,-1320.800071274398,-406.3993870242253);
	pattern.m_listDashLength.push_back(17.03132);
	pattern.m_listDashLength.push_back(-1686.096);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(69.444,29.464,0.0,-203.1997534420058,-508.0006305702363);
	pattern.m_listDashLength.push_back(8.68072);
	pattern.m_listDashLength.push_back(-859.3919999999998);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(101.3099,73.152,0.0,-101.5994985154939,406.3986984361048);
	pattern.m_listDashLength.push_back(5.1806);
	pattern.m_listDashLength.push_back(-512.88);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(165.9638,72.136,5.08,304.8002141878869,-101.5998241649456);
	pattern.m_listDashLength.push_back(20.94536);
	pattern.m_listDashLength.push_back(-397.962);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(186.009,51.816,10.16,1015.998501777118,101.5997627419143);
	pattern.m_listDashLength.push_back(19.41068);
	pattern.m_listDashLength.push_back(-1921.656);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(303.6901,62.992,62.992,-101.6000348522893,203.1998039692736);
	pattern.m_listDashLength.push_back(14.65296);
	pattern.m_listDashLength.push_back(-351.6712);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(353.1572,71.12,50.8,1727.200714265737,-203.2009100661936);
	pattern.m_listDashLength.push_back(25.58228);
	pattern.m_listDashLength.push_back(-2532.64);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(60.9454,96.52,47.752,-406.3990546331636,-711.198304433365);
	pattern.m_listDashLength.push_back(10.46032);
	pattern.m_listDashLength.push_back(-1035.576);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(90.0,101.6,56.896,-101.6,101.6);
	pattern.m_listDashLength.push_back(6.096);
	pattern.m_listDashLength.push_back(-95.504);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(120.2564,49.784,13.208,406.3991386734444,-711.1996412991024);
	pattern.m_listDashLength.push_back(14.11468);
	pattern.m_listDashLength.push_back(-1397.356);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(48.0128,42.672,25.4,812.8006906358402,914.4011235805031);
	pattern.m_listDashLength.push_back(27.33772);
	pattern.m_listDashLength.push_back(-1339.552);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(0.0,60.96,45.72,101.6,101.6);
	pattern.m_listDashLength.push_back(26.416);
	pattern.m_listDashLength.push_back(-75.184);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(325.3048,87.376,45.72,-1015.998239638147,711.2000676193905);
	pattern.m_listDashLength.push_back(16.0644);
	pattern.m_listDashLength.push_back(-1590.372);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(254.0546,100.584,36.576,-101.6002591880589,-406.4009681281451);
	pattern.m_listDashLength.push_back(14.79316);
	pattern.m_listDashLength.push_back(-724.868);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(207.646,96.52,22.352,-1930.399321960909,-1016.000638988936);
	pattern.m_listDashLength.push_back(24.0858);
	pattern.m_listDashLength.push_back(-2384.5);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(175.4261,75.184,11.176,-1320.8003041899,101.5995084487784);
	pattern.m_listDashLength.push_back(25.48116);
	pattern.m_listDashLength.push_back(-2522.632);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pattern.SetData(175.4261,75.184,11.176,-1320.8003041899,101.5995084487784);
	pattern.m_listDashLength.push_back(25.48116);
	pattern.m_listDashLength.push_back(-2522.632);
	pArrHatchPatternGroup[5].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[6].m_strPatternName = "HONEY";

	pattern.SetData(0.0,0.0,0.0,19.05,10.99852);
	pattern.m_listDashLength.push_back(12.7);
	pattern.m_listDashLength.push_back(-25.4);
	pArrHatchPatternGroup[6].m_listPattern.push_back(pattern);

	pattern.SetData(120.0,0.0,0.0,-19.04999772403122,10.99852394209356);
	pattern.m_listDashLength.push_back(12.7);
	pattern.m_listDashLength.push_back(-25.4);
	pArrHatchPatternGroup[6].m_listPattern.push_back(pattern);

	pattern.SetData(60.0,0.0,0.0,0.0000022759687788,21.99704394209355);
	pattern.m_listDashLength.push_back(-25.4);
	pattern.m_listDashLength.push_back(12.7);
	pArrHatchPatternGroup[6].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[7].m_strPatternName = "JIS_LC_20";

	pattern.SetData(45.0,0.0,0.0,-56.56854249492379,56.5685424949238);
	pArrHatchPatternGroup[7].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,1.6,0.0,-56.56854249492379,56.5685424949238);
	pArrHatchPatternGroup[7].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[8].m_strPatternName = "STEEL";

	pattern.SetData(45.0,0.0,0.0,-8.980256121069151,8.980256121069153);
	pArrHatchPatternGroup[8].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,0.0,6.35,-8.980256121069151,8.980256121069153);
	pArrHatchPatternGroup[8].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[9].m_strPatternName = "ANSI31";

	pattern.SetData(45.0,0.0,0.0,-8.980256121069151,8.980256121069153);
	pArrHatchPatternGroup[9].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[10].m_strPatternName = "ANSI32";

	pattern.SetData(45.0,0.0,0.0,-26.94076836320747,26.94076836320747);
	pArrHatchPatternGroup[10].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,13.47039,0.0,-26.94076836320747,26.94076836320747);
	pArrHatchPatternGroup[10].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[11].m_strPatternName = "ANSI33";

	pattern.SetData(45.0,0.0,0.0,-17.960512242138293,17.960512242138293);
	pArrHatchPatternGroup[11].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,13.47039,0.0,-17.960512242138293,17.960512242138293);
	pattern.m_listDashLength.push_back(9.525);
	pattern.m_listDashLength.push_back(-4.7625);
	pArrHatchPatternGroup[11].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[12].m_strPatternName = "ANSI34";

	pattern.SetData(45.0,0.0,0.0,-53.88153672641492,53.88153672641492);
	pArrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,13.47039,0.0,-53.88153672641492,53.88153672641492);
	pArrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,26.94078,0.0,-53.88153672641492,53.88153672641492);
	pArrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,40.4112,0.0,-53.88153672641492,53.88153672641492);
	pArrHatchPatternGroup[12].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[13].m_strPatternName = "ANSI35";

	pattern.SetData(45.0,0.0,0.0,-17.960512242138293,17.960512242138293);
	pArrHatchPatternGroup[13].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,13.47039,0.0,-17.960512242138293,17.960512242138293);
	pattern.m_listDashLength.push_back(23.8125);
	pattern.m_listDashLength.push_back(-4.7625);
	pattern.m_listDashLength.push_back(-4.7625);
	pattern.m_listDashLength.push_back(0.6246936655556858);
	pArrHatchPatternGroup[13].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[14].m_strPatternName = "ANSI36";

	pattern.SetData(45.0,0.0,0.0,6.7351920908018693,24.695704332940173);
	pattern.m_listDashLength.push_back(23.8125);
	pattern.m_listDashLength.push_back(-4.7625);
	pattern.m_listDashLength.push_back(0.0);
	pattern.m_listDashLength.push_back(-4.7625);
	pArrHatchPatternGroup[14].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[15].m_strPatternName = "ANSI37";

	pattern.SetData(45.0,0.0,0.0,-8.9802561210691507,8.9802561210691507);
	pArrHatchPatternGroup[15].m_listPattern.push_back(pattern);

	pattern.SetData(135.0,0.0,0.0,-8.9802561210691507,-8.9802561210691507);
	pArrHatchPatternGroup[15].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[16].m_strPatternName = "ANSI38";

	pattern.SetData(45.0,0.0,0.0,-8.9802561210691507,8.9802561210691507);
	pArrHatchPatternGroup[16].m_listPattern.push_back(pattern);

	pattern.SetData(135.0,0.0,0.0,-26.94076836320747,8.9802561210691547);
	pattern.m_listDashLength.push_back(23.8125);
	pattern.m_listDashLength.push_back(-14.2875);
	pArrHatchPatternGroup[16].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[17].m_strPatternName = "JIS_RC_10";

	pattern.SetData(45.0,0.0,0.0,-28.284271247461893,28.284271247461893);
	pArrHatchPatternGroup[17].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,2.175,0.0,-28.284271247461893,28.284271247461893);
	pArrHatchPatternGroup[17].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,4.35,0.0,-28.284271247461893,28.284271247461893);
	pArrHatchPatternGroup[17].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[18].m_strPatternName = "JIS_RC_15";

	pattern.SetData(45.0,0.0,0.0,-42.42640687119284,42.42640687119284);
	pArrHatchPatternGroup[18].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,2.175,0.0,-42.42640687119284,42.42640687119284);
	pArrHatchPatternGroup[18].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,4.35,0.0,-42.42640687119284,42.42640687119284);
	pArrHatchPatternGroup[18].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[19].m_strPatternName = "JIS_RC_18";

	pattern.SetData(45.0,0.0,0.0,-50.911688245431413,50.911688245431413);
	pArrHatchPatternGroup[19].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,3.0,0.0,-50.911688245431413,50.911688245431413);
	pArrHatchPatternGroup[19].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,6.0,0.0,-50.911688245431413,50.911688245431413);
	pArrHatchPatternGroup[19].m_listPattern.push_back(pattern);

	pArrHatchPatternGroup[20].m_strPatternName = "JIS_RC_30";

	pattern.SetData(45.0,0.0,0.0,-84.85281374238568,84.85281374238568);
	pArrHatchPatternGroup[20].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,3.0,0.0,-84.85281374238568,84.85281374238568);
	pArrHatchPatternGroup[20].m_listPattern.push_back(pattern);

	pattern.SetData(45.0,6.0,0.0,-84.85281374238568,84.85281374238568);
	pArrHatchPatternGroup[20].m_listPattern.push_back(pattern);

	return pArrHatchPatternGroup;
}*/

void Hatch::HatchPattern::SetData(double dAngle, double x, double y, double dOffsetX, double dOffsetY)
{
	m_listDashLength.clear();

	m_dPatternAngle = dAngle;
	m_ptBase.m_pt[0] = x;
	m_ptBase.m_pt[1] = y;
	m_dOffsetX		 = dOffsetX;
	m_dOffsetY		 = dOffsetY;
}

Hatch::Boundary::Boundary()
{
	m_nObjectHandle = 0;
}

int Hatch::Boundary::GetObjectSize()
{
	return 1;
}

void Hatch::Boundary::WriteHandle(Utility::FileManager* pMgr)
{
	AddLine(pMgr,330,L"%X",m_nObjectHandle);
}

Hatch::PolyLineType::PolyLineType()
{
	m_nHasBulge = 0;
	//m_dBulge = 0.0;
	m_nClosed = 0;

	m_nPointSize = 0;
	m_pArrX = 0;
	m_pArrY = 0;
	
	m_nPointIndex = 0;
}

Hatch::PolyLineType::PolyLineType(int nPointSize) 
{
	m_nHasBulge = 0;
	//m_dBulge = 0.0;
	m_nClosed = 0;

	if (nPointSize > 0)
	{
		m_nPointSize = nPointSize;
		m_pArrX = new double[m_nPointSize];
		m_pArrY = new double[m_nPointSize];
		m_pArrBulge = new double[m_nPointSize];
	}
	else
	{
		m_nPointSize = 0;
		m_pArrX = 0;
		m_pArrY = 0;
		m_pArrBulge = 0;
	}

	m_nPointIndex = 0;
}

Hatch::PolyLineType::~PolyLineType() 
{
	*m_pRefCount -= 1;
	if (*m_pRefCount <= 0)
	{
		delete m_pRefCount;
		delete [] m_pArrX;
		delete [] m_pArrY;
		delete [] m_pArrBulge;
	}
}

Hatch::PolyLineType::PolyLineType(const Hatch::PolyLineType& rhs)
{
	memcpy(this,&rhs,sizeof(Hatch::PolyLineType));
	*m_pRefCount += 1;
}

void Hatch::PolyLineType::operator= (const Hatch::PolyLineType& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			delete m_pRefCount;
			delete [] m_pArrX;
			delete [] m_pArrY;
			delete [] m_pArrBulge;
		}
	}

	memcpy(this,&rhs,sizeof(Hatch::PolyLineType));
	if (!bSame) *m_pRefCount += 1;
}

void Hatch::PolyLineType::SetClosedFlag()
{
	if (m_nPointSize > 0)
	{
		if (m_pArrX[0] == m_pArrX[m_nPointSize-1] && m_pArrY[0] == m_pArrY[m_nPointSize-1])
			m_nClosed = 1;
		else m_nClosed = 0;
	}
}

void Hatch::PolyLineType::Write(Utility::FileManager* pMgr, int nSolidFill)
{
	//if (nSolidFill)
	{
		AddLine(pMgr,92,L"7");
		AddLine(pMgr,72,L"%d",m_nHasBulge);
		//AddLine(pMgr,73,"%d",m_nClosed);
		AddLine(pMgr,73,L"1");
		AddLine(pMgr,93,L"%d",m_nPointSize);

		for (int i=0;i<m_nPointSize;i++)
		{
			AddLine(pMgr,10,L"%lf",m_pArrX[i]);
			AddLine(pMgr,20,L"%lf",m_pArrY[i]);
			if (m_nHasBulge)
				AddLine(pMgr, 42, L"%lf", m_pArrBulge[i]);
		}

		//if (m_nHasBulge) AddLine(pMgr,42,L"%lf",m_dBulge);
	}
	/*else
	{
		AddLine(pMgr,92,"17");
		AddLine(pMgr,93,"%d",m_nPointSize);

		for (int i=0;i<m_nPointSize-1;i++)
		{
			AddLine(pMgr,72,"1");
			AddLine(pMgr,10,"%lf",m_pArrX[i]);
			AddLine(pMgr,20,"%lf",m_pArrY[i]);
			AddLine(pMgr,11,"%lf",m_pArrX[i+1]);
			AddLine(pMgr,21,"%lf",m_pArrY[i+1]);
		}

		AddLine(pMgr,72,"1");
		AddLine(pMgr,10,"%lf",m_pArrX[m_nPointSize-1]);
		AddLine(pMgr,20,"%lf",m_pArrY[m_nPointSize-1]);
		AddLine(pMgr,11,"%lf",m_pArrX[0]);
		AddLine(pMgr,21,"%lf",m_pArrY[0]);
	}*/
}

Hatch::Boundary::BoundaryType Hatch::PolyLineType::GetBoundaryType()
{
	return POLYLINE;
}

/*Hatch::LineEdge::LineEdge(double dX1, double dY1, double dX2, double dY2)
{
	m_dBeginPoint[0] = dX1;
	m_dBeginPoint[1] = dY1;
	m_dEndPoint[0]   = dX2;
	m_dEndPoint[1]   = dY2;
}*/

void Hatch::LineEdge::WriteHandle(Utility::FileManager* pMgr)
{
	std::list<int>::const_iterator pIter = m_listObjectHandle.begin();
	std::list<int>::const_iterator pEnd = m_listObjectHandle.end();

	while (pIter != pEnd)
	{
		DXF::AddLine(pMgr,330,L"%X",*pIter);
		pIter++;
	}
}

int Hatch::LineEdge::GetObjectSize()
{
	return (int)m_listObjectHandle.size();
}

void Hatch::LineEdge::AddLine(const Utility::Vertex2D& ptBegin, const Utility::Vertex2D& ptEnd, int nObjectHandle)
{
	m_listBeginPoint.push_back(ptBegin);
	m_listEndPoint.push_back(ptEnd);
	m_listObjectHandle.push_back(nObjectHandle);
}

void Hatch::LineEdge::Write(Utility::FileManager* pMgr, int nSolidFill)
{
	/*AddLine(pMgr,92,"1");
	AddLine(pMgr,93,"2");
	AddLine(pMgr,72,"1");
	AddLine(pMgr,10,"%lf",m_dBeginPoint[0]);
	AddLine(pMgr,20,"%lf",m_dBeginPoint[1]);
	AddLine(pMgr,11,"%lf",m_dEndPoint[0]);
	AddLine(pMgr,21,"%lf",m_dEndPoint[1]);*/

	int nSize = (int)m_listBeginPoint.size();
	std::list<Utility::Vertex2D>::const_iterator pIter = m_listBeginPoint.begin();
	std::list<Utility::Vertex2D>::const_iterator pEnd = m_listBeginPoint.end();

	DXF::AddLine(pMgr,92,L"7");
	DXF::AddLine(pMgr,72,L"0");
	DXF::AddLine(pMgr,73,L"1");
	DXF::AddLine(pMgr,93,L"%d",nSize);

	while (pIter != pEnd)
	{
		DXF::AddLine(pMgr,10,L"%lf",pIter->m_pt[0]);
		DXF::AddLine(pMgr,20,L"%lf",pIter->m_pt[1]);
		pIter++;
	}
}

Hatch::Boundary::BoundaryType Hatch::LineEdge::GetBoundaryType()
{
	return LINEEDGE;
}

Hatch::ArcEdge::ArcEdge()
{
	m_dBeginAngle = 0.0;
	m_dEndAngle	  = 0.0;
	m_dAngle	  = 0.0;
	m_dRadius	  = 0.0;
	m_bCircle	  = false;
	m_nDirection  = 1;
}

// dBeginAngle, dEndAngle : Degree
// dAngle : Radian
Hatch::ArcEdge::ArcEdge(const Utility::Vertex2D& ptCenter, const Utility::Vertex2D& ptBegin, const Utility::Vertex2D& ptEnd, double dAngle, bool bCircle, double dBeginAngle, double dEndAngle)
{
	m_dBeginAngle = dBeginAngle;
	m_dEndAngle	  = dEndAngle;
	m_dAngle	  = dAngle;
	m_dRadius	  = sqrt((ptCenter.m_pt[0] - ptBegin.m_pt[0]) * (ptCenter.m_pt[0] - ptBegin.m_pt[0]) + (ptCenter.m_pt[1] - ptBegin.m_pt[1]) * (ptCenter.m_pt[1] - ptBegin.m_pt[1]));
	m_ptBegin	  = ptBegin;
	m_ptEnd		  = ptEnd;
	m_ptCenter	  = ptCenter;
	m_bCircle	  = bCircle;
	m_nDirection  = dAngle >= 0 ? 1 : 0;
}

void Hatch::ArcEdge::Write(Utility::FileManager* pMgr, int nSolidFill)
{
	if (nSolidFill)	// Solid Type
	{
		AddLine(pMgr,92,L"7");
		AddLine(pMgr,72,L"1");
		AddLine(pMgr,73,L"1");
		AddLine(pMgr,93,L"2");
		AddLine(pMgr,10,L"%lf",m_ptBegin.m_pt[0]);
		AddLine(pMgr,20,L"%lf",m_ptBegin.m_pt[1]);

		if (m_bCircle) AddLine(pMgr,42,L"1.0");
		else AddLine(pMgr,42,L"%lf",tan(m_dAngle/4));

		AddLine(pMgr,10,L"%lf",m_ptEnd.m_pt[0]);
		AddLine(pMgr,20,L"%lf",m_ptEnd.m_pt[1]);

		if (m_bCircle) AddLine(pMgr,42,L"1.0");
		else AddLine(pMgr,42,L"0.0");
	}
	else		// Pattern Type
	{
		AddLine(pMgr,92,L"33");
		AddLine(pMgr,93,L"1");
		AddLine(pMgr,72,L"2");

		AddLine(pMgr,10,L"%lf",m_ptCenter.m_pt[0]);
		AddLine(pMgr,20,L"%lf",m_ptCenter.m_pt[1]);
		AddLine(pMgr,40,L"%lf",m_dRadius);
		AddLine(pMgr,50,L"%lf",m_dBeginAngle);
		AddLine(pMgr,51,L"%lf",m_dEndAngle);
		AddLine(pMgr,73,L"%d",m_nDirection);
	}
}

Hatch::Boundary::BoundaryType Hatch::ArcEdge::GetBoundaryType()
{
	return ARCEDGE;
}

Hatch::EArcEdge::EArcEdge()
{
	m_dCenterPoint[0] = 0.0;
	m_dCenterPoint[1] = 0.0;
	m_dLongAxisPoint[0] = 0.0;
	m_dLongAxisPoint[1] = 0.0;
	m_dRatio = 0.0;
	m_dBeginAngle = 0.0;
	m_dEndAngle   = 0.0;
	m_nDirection  = 1;
}

Hatch::EArcEdge::EArcEdge(double dCenterX, double dCenterY, double dLongX, double dLongY, double dRatio, double dBeginAngle, double dEndAngle, int nDirection)
{
	m_dCenterPoint[0] = dCenterX;
	m_dCenterPoint[1] = dCenterY;
	m_dLongAxisPoint[0] = dLongX;
	m_dLongAxisPoint[1] = dLongY;
	m_dRatio = dRatio;
	m_dBeginAngle = dBeginAngle;
	m_dEndAngle   = dEndAngle;
	m_nDirection  = nDirection;
}

void Hatch::EArcEdge::Write(Utility::FileManager* pMgr, int nSolidFill)
{
	AddLine(pMgr,92,L"1");
	AddLine(pMgr,93,L"2");
	AddLine(pMgr,72,L"3");
	AddLine(pMgr,10,L"%lf",m_dCenterPoint[0]);
	AddLine(pMgr,20,L"%lf",m_dCenterPoint[1]);
	AddLine(pMgr,11,L"%lf",m_dLongAxisPoint[0]);
	AddLine(pMgr,21,L"%lf",m_dLongAxisPoint[1]);
	AddLine(pMgr,40,L"%lf",m_dRatio);
	AddLine(pMgr,50,L"%lf",m_dBeginAngle);
	AddLine(pMgr,51,L"%lf",m_dEndAngle);
	AddLine(pMgr,73,L"%d",m_nDirection);
}

Hatch::Boundary::BoundaryType Hatch::EArcEdge::GetBoundaryType()
{
	return EARCEDGE;
}

Hatch::SplineEdge::SplineEdge()
{
	m_nAngle	= 0;
	m_nRational = 0;
	m_nPeriodic = 0;
	m_dWeight	= 1.0;

	m_nNumOfKnots = 0;
	m_pArrKnots = 0;

	m_nNumOfControlPoints = 0;
	m_pArrControlPointX = 0;
	m_pArrControlPointY = 0;
	
	m_nKnotIndex = 0;
	m_nControlPointIndex = 0;
}

Hatch::SplineEdge::SplineEdge(int nAngle, int nRational, int nPeriodic, int nNumOfKnots, int nNumOfControlPoints, double dWeight)
{
	m_nAngle	= nAngle;
	m_nRational = nRational;
	m_nPeriodic = nPeriodic;
	m_dWeight	= dWeight;

	if (nNumOfKnots > 0)
	{
		m_nNumOfKnots = nNumOfKnots;
		m_pArrKnots = new double[m_nNumOfKnots];
	}
	else
	{
		m_nNumOfKnots = 0;
		m_pArrKnots = 0;
	}

	if (nNumOfControlPoints > 0)
	{
		m_nNumOfControlPoints = nNumOfControlPoints;
		m_pArrControlPointX = new double[m_nNumOfControlPoints];
		m_pArrControlPointY = new double[m_nNumOfControlPoints];
	}
	else
	{
		m_nNumOfControlPoints = 0;
		m_pArrControlPointX = 0;
		m_pArrControlPointY = 0;
	}

	m_nKnotIndex = 0;
	m_nControlPointIndex = 0;
}

Hatch::SplineEdge::~SplineEdge()
{
	*m_pRefCount -= 1;
	if (*m_pRefCount <= 0)
	{
		delete m_pRefCount;
		delete [] m_pArrKnots;
		delete [] m_pArrControlPointX;
		delete [] m_pArrControlPointY;
	}
}

Hatch::SplineEdge::SplineEdge(const Hatch::SplineEdge& rhs)
{
	memcpy(this,&rhs,sizeof(Hatch::SplineEdge));
	*m_pRefCount += 1;
}

void Hatch::SplineEdge::operator= (const Hatch::SplineEdge& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			delete m_pRefCount;
			delete [] m_pArrKnots;
			delete [] m_pArrControlPointX;
			delete [] m_pArrControlPointY;
		}
	}

	memcpy(this,&rhs,sizeof(Hatch::SplineEdge));
	if (!bSame) *m_pRefCount += 1;
}

void Hatch::SplineEdge::Write(Utility::FileManager* pMgr, int nSolidFill)
{
	AddLine(pMgr,92,L"1");
	AddLine(pMgr,93,L"2");
	AddLine(pMgr,72,L"4");
	AddLine(pMgr,94,L"%d",m_nAngle);
	AddLine(pMgr,73,L"%d",m_nRational);
	AddLine(pMgr,74,L"%d",m_nPeriodic);
	AddLine(pMgr,95,L"%d",m_nNumOfKnots);
	AddLine(pMgr,96,L"%d",m_nNumOfControlPoints);

	int i;
	for (i=0;i<m_nNumOfKnots;i++)
	{
		AddLine(pMgr,40,L"%lf",m_pArrKnots[i]);
	}
	for (i=0;i<m_nNumOfControlPoints;i++)
	{
		AddLine(pMgr,10,L"%lf",m_pArrControlPointX[i]);
		AddLine(pMgr,20,L"%lf",m_pArrControlPointY[i]);
	}

	if (m_dWeight != 1.0) AddLine(pMgr,42,L"%lf",m_dWeight);
}

Hatch::Boundary::BoundaryType Hatch::SplineEdge::GetBoundaryType()
{
	return SPLINEEDGE;
}

Hatch::BoundaryManager::~BoundaryManager()
{
	std::list<Hatch::Boundary*>::iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Hatch::Boundary* pBoundary = *p;
		delete pBoundary;
		++p;
	}
}

Hatch::Hatch(void)
{
	Init();
}

Hatch::~Hatch(void)
{
	std::list<Hatch::BoundaryManager*>::iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Hatch::BoundaryManager* pMgr = *p;
		delete pMgr;
		++p;
	}
}

void Hatch::Init()
{
	m_strSubClassName	= L"AcDbHatch";
	m_strEntityType		= L"HATCH";
	m_nBoundary			= 0;
	m_nHatchStyle		= 0;
	m_nHatchPatternType = 1;
	m_dHatchPatternAngle= 0.0;
	m_dPatternScale		= 1.0;
	m_bDoublePattern	= false;
	m_dElevationPoint[0] = m_dElevationPoint[1] = m_dElevationPoint[2] = 0.0;
	m_vExtrusionDirection.m_pt[0] = 0.0;
	m_vExtrusionDirection.m_pt[1] = 0.0;
	m_vExtrusionDirection.m_pt[2] = 1.0;
	m_bNoObject = false;

	m_pCurrentBoundaryManager = 0;
	m_pCurrentBoundary = 0;
}

void Hatch::WriteHatchPattern(Utility::FileManager* pMgr)
{
	if (m_pMgr == 0)
		return;

	HatchPatternGroup* arrHathPatternGroup = m_pMgr->HatchPatternGroups();

	std::list<Hatch::HatchPattern>* pList;

	if (m_strHatchPattern == L"AR-CONC")
	{
		pList = &arrHathPatternGroup[0].m_listPattern;
	}
	else if (m_strHatchPattern == L"AR-HBONE")
	{
		pList = &arrHathPatternGroup[1].m_listPattern;
	}
	else if (m_strHatchPattern == L"AR-SAND")
	{
		pList = &arrHathPatternGroup[2].m_listPattern;
	}
	else if (m_strHatchPattern == L"CLAY")
	{
		pList = &arrHathPatternGroup[3].m_listPattern;
	}
	else if (m_strHatchPattern == L"BRICK")
	{
		pList = &arrHathPatternGroup[4].m_listPattern;
	}
	else if (m_strHatchPattern == L"GRAVEL")
	{
		pList = &arrHathPatternGroup[5].m_listPattern;
	}
	else if (m_strHatchPattern == L"HONEY")
	{
		pList = &arrHathPatternGroup[6].m_listPattern;
	}
	else if (m_strHatchPattern == L"JIS_LC_20")
	{
		pList = &arrHathPatternGroup[7].m_listPattern;
	}
	else if (m_strHatchPattern == L"STEEL")
	{
		pList = &arrHathPatternGroup[8].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI31")
	{
		pList = &arrHathPatternGroup[9].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI32")
	{
		pList = &arrHathPatternGroup[10].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI33")
	{
		pList = &arrHathPatternGroup[11].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI34")
	{
		pList = &arrHathPatternGroup[12].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI35")
	{
		pList = &arrHathPatternGroup[13].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI36")
	{
		pList = &arrHathPatternGroup[14].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI37")
	{
		pList = &arrHathPatternGroup[15].m_listPattern;
	}
	else if (m_strHatchPattern == L"ANSI38")
	{
		pList = &arrHathPatternGroup[16].m_listPattern;
	}
	else if (m_strHatchPattern == L"JIS_RC_10")
	{
		pList = &arrHathPatternGroup[17].m_listPattern;
	}
	else if (m_strHatchPattern == L"JIS_RC_15")
	{
		pList = &arrHathPatternGroup[18].m_listPattern;
	}
	else if (m_strHatchPattern == L"JIS_RC_18")
	{
		pList = &arrHathPatternGroup[19].m_listPattern;
	}
	else if (m_strHatchPattern == L"JIS_RC_30")
	{
		pList = &arrHathPatternGroup[20].m_listPattern;
	}
	else return;

	int nSize = (int)pList->size();
	AddLine(pMgr,78,L"%d",nSize);

	std::list<Hatch::HatchPattern>::const_iterator pIter = pList->begin();
	std::list<Hatch::HatchPattern>::const_iterator pEnd = pList->end();

	while (pIter != pEnd)
	{
		AddLine(pMgr,53,L"%lf",pIter->m_dPatternAngle);
		AddLine(pMgr,43,L"%lf",pIter->m_ptBase.m_pt[0] * m_dPatternScale / 4.0);
		AddLine(pMgr,44,L"%lf",pIter->m_ptBase.m_pt[1] * m_dPatternScale / 4.0);
		AddLine(pMgr,45,L"%lf",pIter->m_dOffsetX * m_dPatternScale / 4.0);
		AddLine(pMgr,46,L"%lf",pIter->m_dOffsetY * m_dPatternScale / 4.0);
		AddLine(pMgr,79,L"%d",(int)pIter->m_listDashLength.size());

		std::list<double>::const_iterator p = pIter->m_listDashLength.begin();
		std::list<double>::const_iterator pEnd1 = pIter->m_listDashLength.end();

		while (p != pEnd1)
		{
			AddLine(pMgr,49,L"%lf",*p * m_dPatternScale / 4.0);
			++p;
		}

		++pIter;
	}
}

void Hatch::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,10,L"%lf",m_dElevationPoint[0]);
	AddLine(pMgr,20,L"%lf",m_dElevationPoint[1]);
	AddLine(pMgr,30,L"%lf",m_dElevationPoint[2]);
	AddLine(pMgr,210,L"%lf",m_vExtrusionDirection.m_pt[0]);
	AddLine(pMgr,220,L"%lf",m_vExtrusionDirection.m_pt[1]);
	AddLine(pMgr,230,L"%lf",m_vExtrusionDirection.m_pt[2]);
	AddLine(pMgr,2,L"%s",m_strHatchPattern.data());
	AddLine(pMgr,70,L"%d",m_nSolidFill);
	AddLine(pMgr,71,L"%d",m_nAssociativity);

	if (m_bNoObject)
	{
		AddLine(pMgr,91,L"1");
		AddLine(pMgr,92,L"7");
		AddLine(pMgr,72,L"1");
		AddLine(pMgr,73,L"1");
		AddLine(pMgr,93,L"2");

		for (int i=1;i<3;i++)
		{
			AddLine(pMgr,10,L"%lf",m_ptArrCircle[i].m_pt[0]);
			AddLine(pMgr,20,L"%lf",m_ptArrCircle[i].m_pt[1]);
			AddLine(pMgr,42,L"1.0");
		}

		AddLine(pMgr,97,L"0");
		AddLine(pMgr,75,L"0");
		AddLine(pMgr,76,L"1");
		AddLine(pMgr,47,L"56.50726954317004");
		AddLine(pMgr,98,L"1");

		AddLine(pMgr,10,L"%lf",m_ptArrCircle[0].m_pt[0]);
		AddLine(pMgr,20,L"%lf",m_ptArrCircle[0].m_pt[1]);

		AddLine(pMgr,1001,L"ACAD");
		AddLine(pMgr,1010,L"0.0");
		AddLine(pMgr,1020,L"0.0");
		AddLine(pMgr,1030,L"0.0");
	}
	else
	{
		std::list<Hatch::BoundaryManager*>::const_iterator p = m_list.begin();
		int nSize = 0;

		while (p != m_list.end())
		{
			Hatch::BoundaryManager* pBoundaryManager = *p;
			nSize += (int)pBoundaryManager->m_list.size();
			p++;
		}

		AddLine(pMgr,91,L"%d",nSize);
		p = m_list.begin();

		while (p != m_list.end())
		{
			Hatch::BoundaryManager* pBoundaryManager = *p;
			std::list<Hatch::Boundary*>::const_iterator pIter = pBoundaryManager->m_list.begin();

			// 객체의 개수와 핸들을 각각 기입하는 방식
			while (pIter != pBoundaryManager->m_list.end())
			{
				Hatch::Boundary* pBoundary = *pIter;
				pBoundary->Write(pMgr,m_nSolidFill);

				AddLine(pMgr,97,L"%d",1);
				pBoundary->WriteHandle(pMgr);
				++pIter;
			}

			// 객체의 개수와 핸들을 한번에 모두 처리하는 방식
			/*int nObjectSize = 0;

			while (pIter != pBoundaryManager->m_list.end())
			{
				Hatch::Boundary* pBoundary = *pIter;
				pBoundary->Write(pMgr,m_nSolidFill);

				nObjectSize += pBoundary->GetObjectSize();
				++pIter;
			}

			AddLine(pMgr,97,"%d",nObjectSize);
			pIter = pBoundaryManager->m_list.begin();

			while (pIter != pBoundaryManager->m_list.end())
			{
				Hatch::Boundary* pBoundary = *pIter;
				pBoundary->WriteHandle(pMgr);
				++pIter;
			}*/

			++p;
		}

		AddLine(pMgr,75,L"%d",m_nHatchStyle);
		AddLine(pMgr,76,L"%d",m_nHatchPatternType);

		// 패턴 채우기인 경우
		if (m_nSolidFill == 0) 
		{
			AddLine(pMgr,52,L"%lf",m_dHatchPatternAngle);
			AddLine(pMgr,41,L"%lf",m_dPatternScale);
			if (m_bDoublePattern) AddLine(pMgr,77,L"1");
			else AddLine(pMgr,77,L"0");
			WriteHatchPattern(pMgr);
		}
		/*else */AddLine(pMgr,47,L"3.350433518642216");

		AddLine(pMgr,98,L"1");
		AddLine(pMgr,10,L"0.0");
		AddLine(pMgr,20,L"0.0");
	}
}

void Hatch::SetElevationPoint(double x, double y, double z)
{
	m_dElevationPoint[0] = x;
	m_dElevationPoint[1] = y;
	m_dElevationPoint[2] = z;
}

void Hatch::SetHatchPatternName(wchar_t* strHatchPattern)
{
	m_strHatchPattern = strHatchPattern;
}

void Hatch::SetSolidFillFlag(int nSolidFill)
{
	m_nSolidFill = nSolidFill;
}

void Hatch::SetAssociativityFlag(int nAssociativity)
{
	m_nAssociativity = nAssociativity;
}

void Hatch::SetBoundaryPathType(int nBoundaryPathType)
{
	m_nBoundaryPathType = nBoundaryPathType;
}

void Hatch::SetHatchStyle(int nHatchStyle)
{
	m_nHatchStyle = nHatchStyle;
}

void Hatch::SetHatchPatternType(int nPatternType)
{
	m_nHatchPatternType = nPatternType;
}

void Hatch::SetHatchPatternAngle(double dPatternAngle)
{
	m_dHatchPatternAngle = dPatternAngle;
}

void Hatch::SetPatternScale(double dScale)
{
	m_dPatternScale = dScale;
}

void Hatch::SetDoublePattern(bool bDouble)
{
	m_bDoublePattern = bDouble;
}

void Hatch::AddBoundary(Hatch::BoundaryManager* pMgr)
{
	m_nBoundary++;
	m_list.push_back(pMgr);
}

bool Hatch::ReadDatai(int nCode, int nData)
{
	bool bResult = __super::ReadDatai(nCode,nData);
	if (bResult) 
	{
		if (nCode == 5) m_bBoundaryRead = false;
		return bResult;
	}

	switch (nCode)
	{
	case 70:
		m_nSolidFill = nData;
		return true;

	case 71:
		m_nAssociativity = nData;
		return true;

	case 72:
		if (m_pCurrentBoundaryManager)
		{
			if (m_pCurrentBoundary != 0 && m_pCurrentBoundary->GetBoundaryType() == Boundary::POLYLINE)
			{
				((PolyLineType*)m_pCurrentBoundary)->m_nHasBulge = nData;
			}
			else// if (m_pCurrentBoundary == 0)
			{
				if (nData == 1)
					m_pCurrentBoundary = new LineEdge();
				else if (nData == 2)
					m_pCurrentBoundary = new ArcEdge();
				else if (nData == 3)
					m_pCurrentBoundary = new EArcEdge();
				else if (nData == 4)
					m_pCurrentBoundary = new SplineEdge();
				else
					break;

				m_pCurrentBoundaryManager->m_list.push_back(m_pCurrentBoundary);
			}
			/*else if (m_pCurrentBoundary->GetBoundaryType() == Boundary::POLYLINE)
			{
				((PolyLineType*)m_pCurrentBoundary)->m_nHasBulge = nData;
			}
			else
				break;*/

			return true;
		}
		break;

	case 73:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::POLYLINE)
				((PolyLineType*)m_pCurrentBoundary)->m_nClosed = nData;
			else if (type == Boundary::ARCEDGE)
			{
				ArcEdge* pArc = (ArcEdge*)m_pCurrentBoundary;
				pArc->m_nDirection = nData;

				if (pArc->m_nDirection == 0)		// 시계 방향
				{
					if (pArc->m_dBeginAngle > pArc->m_dEndAngle)
						pArc->m_dAngle = DegToRad(pArc->m_dBeginAngle - pArc->m_dEndAngle);
					else
						pArc->m_dAngle = DegToRad(360.0 - (pArc->m_dEndAngle - pArc->m_dBeginAngle));
				}
				else
				{
					if (pArc->m_dEndAngle > pArc->m_dBeginAngle)
						pArc->m_dAngle = DegToRad(pArc->m_dEndAngle - pArc->m_dBeginAngle);
					else
						pArc->m_dAngle = DegToRad(360.0 - (pArc->m_dBeginAngle - pArc->m_dEndAngle));
				}
			}
			else if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_nDirection = nData;
			else if (type == Boundary::SPLINEEDGE)
				((SplineEdge*)m_pCurrentBoundary)->m_nRational = nData;
			else
				return false;

			return true;
		}
		break;

	case 74:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			if (m_pCurrentBoundary->GetBoundaryType() == Boundary::SPLINEEDGE)
			{
				((SplineEdge*)m_pCurrentBoundary)->m_nPeriodic = nData;
				return true;
			}
		}
		break;

	case 75:
		m_nHatchStyle = nData;
		return true;

	case 76:
		m_nHatchPatternType = nData;
		return true;

	case 78:	// Pattern Size
		return true;

	case 91:	// Boundary Size;
		m_bBoundaryRead = true;
		m_pCurrentBoundaryManager = new BoundaryManager();
		m_list.push_back(m_pCurrentBoundaryManager);
		return true;

	case 92:
		if (m_pCurrentBoundaryManager != 0 && (nData & 2) == 2)
		{
			m_pCurrentBoundary = new PolyLineType();
			m_pCurrentBoundaryManager->m_list.push_back(m_pCurrentBoundary);
		}
		return true;

	case 93:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::POLYLINE)
			{
				((PolyLineType*)m_pCurrentBoundary)->m_nPointSize = nData;
				((PolyLineType*)m_pCurrentBoundary)->m_pArrX = new double[nData];
				((PolyLineType*)m_pCurrentBoundary)->m_pArrY = new double[nData];
				((PolyLineType*)m_pCurrentBoundary)->m_pArrBulge = new double[nData];
				return true;
			}
		}
		break;

	case 94:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			if (m_pCurrentBoundary->GetBoundaryType() == Boundary::SPLINEEDGE)
			{
				((SplineEdge*)m_pCurrentBoundary)->m_nAngle = nData;
				return true;
			}
		}
		break;

	case 95:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			if (m_pCurrentBoundary->GetBoundaryType() == Boundary::SPLINEEDGE)
			{
				((SplineEdge*)m_pCurrentBoundary)->m_nNumOfKnots = nData;
				((SplineEdge*)m_pCurrentBoundary)->m_pArrKnots = new double[nData];
				return true;
			}
		}
		break;

	case 96:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			if (m_pCurrentBoundary->GetBoundaryType() == Boundary::SPLINEEDGE)
			{
				((SplineEdge*)m_pCurrentBoundary)->m_nNumOfControlPoints = nData;
				((SplineEdge*)m_pCurrentBoundary)->m_pArrControlPointX = new double[nData];
				((SplineEdge*)m_pCurrentBoundary)->m_pArrControlPointY = new double[nData];
				return true;
			}
		}
		break;

	case 97:
		m_pCurrentBoundaryManager = 0;
		m_pCurrentBoundary = 0;
		return true;

	case 330:
		if (m_bBoundaryRead)
		{
			Entity* pEntity = m_pMgr->GetEntity(nData);
			if (pEntity) m_listBoundaryEntity.push_back(pEntity);

			m_pCurrentBoundaryManager = 0;
			m_pCurrentBoundary = 0;
		}
		return true;
	}

	return false;
}

bool Hatch::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::POLYLINE)
				((PolyLineType*)m_pCurrentBoundary)->m_pArrX[((PolyLineType*)m_pCurrentBoundary)->m_nPointIndex] = dData;
			else if (type == Boundary::ARCEDGE)
				((ArcEdge*)m_pCurrentBoundary)->m_ptCenter.m_pt[0] = dData;
			else if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dCenterPoint[0] = dData;
			else if (type == Boundary::SPLINEEDGE)
				((SplineEdge*)m_pCurrentBoundary)->m_pArrControlPointX[((SplineEdge*)m_pCurrentBoundary)->m_nControlPointIndex] = dData;
			else if (type == Boundary::LINEEDGE)
			{
				((LineEdge*)m_pCurrentBoundary)->m_listBeginPoint.push_back(Utility::Vertex2D(dData, 0.0));
			}
			else
				break;

			return true;
		}
		break;

	case 11:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dLongAxisPoint[0] = dData;
			else if (type == Boundary::LINEEDGE)
			{
				((LineEdge*)m_pCurrentBoundary)->m_listEndPoint.push_back(Utility::Vertex2D(dData, 0.0));
			}
			else
				break;

			return true;
		}
		break;

	case 20:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::POLYLINE)
			{
				PolyLineType* polyline = (PolyLineType*)m_pCurrentBoundary;
				polyline->m_pArrY[polyline->m_nPointIndex] = dData;

				if (polyline->m_nHasBulge == 0)
					polyline->m_nPointIndex++;
			}
			else if (type == Boundary::ARCEDGE)
				((ArcEdge*)m_pCurrentBoundary)->m_ptCenter.m_pt[1] = dData;
			else if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dCenterPoint[1] = dData;
			else if (type == Boundary::SPLINEEDGE)
				((SplineEdge*)m_pCurrentBoundary)->m_pArrControlPointY[((SplineEdge*)m_pCurrentBoundary)->m_nControlPointIndex++] = dData;
			else if (type == Boundary::LINEEDGE)
			{
				std::list<Utility::Vertex2D>::iterator iter = ((LineEdge*)m_pCurrentBoundary)->m_listBeginPoint.end();
				iter--;
				iter->m_pt[1] = dData;
			}
			else
				break;

			return true;
		}
		break;

	case 21:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dLongAxisPoint[1] = dData;
			else if (type == Boundary::LINEEDGE)
			{
				std::list<Utility::Vertex2D>::iterator iter = ((LineEdge*)m_pCurrentBoundary)->m_listEndPoint.end();
				iter--;
				iter->m_pt[1] = dData;
			}
			else
				break;

			return true;
		}
		break;

	case 40:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::ARCEDGE)
				((ArcEdge*)m_pCurrentBoundary)->m_dRadius = dData;
			else if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dRatio = dData;
			else if (type == Boundary::SPLINEEDGE)
				((SplineEdge*)m_pCurrentBoundary)->m_pArrKnots[((SplineEdge*)m_pCurrentBoundary)->m_nKnotIndex++] = dData;
			else
				break;

			return true;
		}
		break;

	case 41:
		m_dPatternScale = dData;
		return true;

	case 42:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			if (m_pCurrentBoundary->GetBoundaryType() == Boundary::SPLINEEDGE)
			{
				((SplineEdge*)m_pCurrentBoundary)->m_dWeight = dData;
				return true;
			}
			else if (m_pCurrentBoundary->GetBoundaryType() == Boundary::POLYLINE)
			{
				PolyLineType* polyline = (PolyLineType*)m_pCurrentBoundary;

				if (polyline->m_nHasBulge == 1)
					polyline->m_pArrBulge[polyline->m_nPointIndex++] = dData;
			}
		}
		break;

	case 50:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::ARCEDGE)
			{
				ArcEdge* pArc = (ArcEdge*)m_pCurrentBoundary;
				pArc->m_dBeginAngle = dData;

				double dTheta = DegToRad(dData);

				pArc->m_ptBegin.m_pt[0] = pArc->m_ptCenter.m_pt[0] + pArc->m_dRadius * cos(dTheta);
				pArc->m_ptBegin.m_pt[1] = pArc->m_ptCenter.m_pt[1] + pArc->m_dRadius * sin(dTheta);
			}
			else if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dBeginAngle = dData;
			else
				break;

			return true;
		}
		break;

	case 51:
		if (m_pCurrentBoundaryManager && m_pCurrentBoundary)
		{
			Boundary::BoundaryType type = m_pCurrentBoundary->GetBoundaryType();

			if (type == Boundary::ARCEDGE)
			{
				ArcEdge* pArc = (ArcEdge*)m_pCurrentBoundary;
				pArc->m_dEndAngle = dData;

				double dTheta = DegToRad(dData);

				pArc->m_ptEnd.m_pt[0] = pArc->m_ptCenter.m_pt[0] + pArc->m_dRadius * cos(dTheta);
				pArc->m_ptEnd.m_pt[1] = pArc->m_ptCenter.m_pt[1] + pArc->m_dRadius * sin(dTheta);

				if (pArc->m_nDirection == 0)		// 시계 방향
				{
					if (pArc->m_dBeginAngle > pArc->m_dEndAngle)
						pArc->m_dAngle = DegToRad(pArc->m_dBeginAngle - pArc->m_dEndAngle);
					else
						pArc->m_dAngle = DegToRad(360.0 - (pArc->m_dEndAngle - pArc->m_dBeginAngle));
				}
				else
				{
					if (pArc->m_dEndAngle > pArc->m_dBeginAngle)
						pArc->m_dAngle = DegToRad(pArc->m_dEndAngle - pArc->m_dBeginAngle);
					else
						pArc->m_dAngle = DegToRad(360.0 - (pArc->m_dBeginAngle - pArc->m_dEndAngle));
				}

				if (fabs(fabs(pArc->m_dBeginAngle - pArc->m_dEndAngle) - 360.0) < 0.0001)
					pArc->m_bCircle = true;
			}
			else if (type == Boundary::EARCEDGE)
				((EArcEdge*)m_pCurrentBoundary)->m_dEndAngle = dData;
			else
				break;

			return true;
		}
		break;
	}

	return false;
}

bool Hatch::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 2:
		m_strHatchPattern = strData;
		return true;
	}

	return false;
}

double Hatch::GetPatternScale()
{
	return m_dPatternScale;
}

// pID : Entity 정보를 담고 있는 링크드 리스트 노드의 포인터
Entity* Hatch::GetBoundaryEntity(void*& pID)
{
	if (pID == 0) m_pIterEntity = m_listBoundaryEntity.begin();
	else
	{
		m_pIterEntity = *(std::list<Entity*>::iterator*)pID;
	}

	if (m_pIterEntity != m_listBoundaryEntity.end())
	{
		Entity* pEntity = *m_pIterEntity;
		m_pIterEntity++;
		pID = &m_pIterEntity;

		return pEntity;
	}

	return 0;
}

// pID : BoundaryManager 정보를 담고 있는 링크드 리스트 노드의 포인터
Hatch::BoundaryManager* Hatch::GetBoundaryManager(void*& pID)
{
	if (pID == 0) m_pIterBoundaryManager = m_list.begin();
	else
	{
		m_pIterBoundaryManager = *(std::list<BoundaryManager*>::iterator*)pID;
	}

	if (m_pIterBoundaryManager != m_list.end())
	{
		BoundaryManager* pManager = *m_pIterBoundaryManager;
		m_pIterBoundaryManager++;
		pID = &m_pIterBoundaryManager;

		return pManager;
	}

	return 0;
}

bool Hatch::IsSolidType()
{
	if (m_nSolidFill) return true;
	return false;
}

std::wstring& Hatch::GetHatchPatternName()
{
	return m_strHatchPattern;
}

void Hatch::SetNoObject(bool bNoObject)
{
	m_bNoObject = bNoObject;
}

void Hatch::SetNoObjectCircle(const Utility::Vertex2D& ptCenter, const Utility::Vertex2D& pt1, const Utility::Vertex2D& pt2)
{
	m_ptArrCircle[0] = ptCenter;
	m_ptArrCircle[1] = pt1;
	m_ptArrCircle[2] = pt2;
}

END_NS
END_NS
