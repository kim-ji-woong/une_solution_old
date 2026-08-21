function SetSplit()
{
	sel=document.camera_time.Split
	for (i=sel.length-1; i>=0; i--)
		sel.options[i] = null
	sel.options[0] = new Option("전체화면",0)
	sel.options[1] = new Option("1",1)
	sel.options[2] = new Option("4",2)
	sel.options[3] = new Option("7",3)
	sel.options[4] = new Option("9",4)
	sel.options[5] = new Option("13",5)
	sel.options[6] = new Option("16",6)
	sel.selectedIndex = 1;
}
function SetCamera()
{
	sel=document.camera_time.Camera
	for (i=sel.length-1; i>=0; i--)
		sel.options[i] = null
	sel.options[0] = new Option("PRI CWP",0)
	sel.options[1] = new Option("냉각탑",1)
	sel.options[2] = new Option("축열조",2)
	sel.options[3] = new Option("C.C.W",3)
	sel.options[4] = new Option("G.T.G",4)
	sel.options[5] = new Option("FUEL GAS",5)
	sel.options[6] = new Option("154 KV",6)
	sel.options[7] = new Option("S.T.G TR",7)
	sel.options[8] = new Option("D.H 펌프실",8)
	sel.options[9] = new Option("공기압축기실",9)
	sel.options[10] = new Option("폐수처리장",10)
	sel.options[11] = new Option("S.T.G",11)
	sel.options[12] = new Option("STACK",12)
	sel.options[13] = new Option("PLB #1",13)
	sel.options[14] = new Option("PLB #2",14)
	for (i=sel.length-1; i>=0; i--){
		if(sel.options[i].value == g_curchannel){
			sel.selectedIndex = i;
			selected_camera = i;
			return true;
		}
	}
	selected_camera = 0;
}
function Search_SetCamera()
{
	sel=document.camera_time.Camera
	for (i=sel.length-1; i>=0; i--)
		sel.options[i] = null
	sel.options[0] = new Option("PRI CWP",0)
	sel.options[1] = new Option("냉각탑",1)
	sel.options[2] = new Option("축열조",2)
	sel.options[3] = new Option("C.C.W",3)
	sel.options[4] = new Option("G.T.G",4)
	sel.options[5] = new Option("FUEL GAS",5)
	sel.options[6] = new Option("154 KV",6)
	sel.options[7] = new Option("S.T.G TR",7)
	sel.options[8] = new Option("D.H 펌프실",8)
	sel.options[9] = new Option("공기압축기실",9)
	sel.options[10] = new Option("폐수처리장",10)
	sel.options[11] = new Option("S.T.G",11)
	sel.options[12] = new Option("STACK",12)
	sel.options[13] = new Option("PLB #1",13)
	sel.options[14] = new Option("PLB #2",14)
	sel.options[15] = new Option("Camera16",15)
	for (i=sel.length-1; i>=0; i--){
		if(sel.options[i].value == g_curchannel){
			sel.selectedIndex = i;
			selected_camera = i;
			return true;
		}
	}
	selected_camera = 0;
}
function Emapexist()
{
	WebClient.Emap_ex = 0;
}
function AutoLogin()
{
	return 1;
}
var g_version ="5,6,0,0"
