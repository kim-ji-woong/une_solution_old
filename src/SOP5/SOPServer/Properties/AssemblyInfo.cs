using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SOPServer")]
[assembly: AssemblyDescription("SOP Server")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("U&E(c)")]
[assembly: AssemblyProduct("SOPServer")]
[assembly: AssemblyCopyright("Copyright © U&E 2013")]
[assembly: AssemblyTrademark("SOP")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("511627f7-36a7-4cbc-a965-6acc27d10f06")]

// 어셈블리의 버전 정보는 다음 네 가지 값으로 구성됩니다.
//
//      주 버전
//      부 버전 
//      빌드 번호
//      수정 버전
//
// 모든 값을 지정하거나 아래와 같이 '*'를 사용하여 빌드 번호 및 수정 버전이 자동으로
// 지정되도록 할 수 있습니다.
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: log4net.Config.XmlConfiguratorAttribute(ConfigFile = "log4net.xml", Watch = true)]

