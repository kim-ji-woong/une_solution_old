<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SampleInputData.aspx.cs" Inherits="SensorSimulationWeb.SampleInputData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    
<style type="text/css">
    html, body {
		width: 100%;
		height: 100%;
		margin: 0;
		padding: 0;
	}

	div {
		margin: 0;
		padding: 0;
	}

	#container {
		position: absolute;
		left: 0%;
		height: 100%;
		margin-left: 20px;
		background: white;
		top: 0px;
		width: 1045px;
	}

	#header {
		height: 89px;
		background: yellow;
	}

	#contents_area {
		width: 100%;
		min-height: 100%;
		margin-top: -89px;
		margin-bottom: -40px;
		height: 571px;
	}

	#contents_area1 {
		width: 100%;
		padding-top: 89px;
		padding-bottom: 40px;
	}

	#contents {
		width: 100%;
		padding-bottom: 40px;
		background: #aaa;
	}

	* html #contents_area {
		height: 100%;
	}

	#footer {
		clear: both;
		width: 100%;
		height: 30px;
		border: 0px solid red;
	}

    .checkboxTD-style {
        width:120px;
        height:32px;
        vertical-align:bottom;
    }
    .nonCheckboxTD-style {
        width:30px;
        height:32px;
        vertical-align:bottom;
    }

    .contentsTD-style {
        vertical-align:top;
        width:150px;
    }
    .auto-style1 {
		width: 944px;
		height: auto;
	}
	.auto-style2 {
		height: 44px;
	}
	.auto-style3 {
		font-size: large;
	}
	.auto-style16 {
		height: 36px;
		width: 126px;
	}
	.auto-style20 {
		height: 2px;
	}
	.auto-style26 {
	}
    .auto-style27 {
		width: 100%;
		height: 91px;
	}
	.auto-style31 {
		width: 100%;
		height: 121px;
	}
	
    .auto-style32 {
        height: 36px;
        width: 328px;
    }
	
    .auto-style33 {
        vertical-align: top;
        width: 130px;
    }
	
</style>

<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>PreSafe</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
	<font face="맑은 고딕" color="black" size="2">
        <div id="container">
			<div id="contents_area">
				<div id="contents_area1">
					<div id="content">   
						<table cellpadding="0" cellspacing="0" class="auto-style1">
							<tr>
								<td class="auto-style32" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp; <span class="auto-style3"><strong>상황인지 알고리즘 시뮬레이션</strong></span></td>
								<td class="auto-style16">
									&nbsp;</td>
                                <td class="auto-style32" style="text-align:right;">
	                                <font face="맑은 고딕" color="black" size="2">
									    <asp:Button ID="btnSave" runat="server" Height="30px" Text="저장" Width="126px" OnClick="btnSave_Click" />
	                                </font>
                                </td>
								<td class="auto-style16" style="text-align:left">
	                                <font face="맑은 고딕" color="black" size="2">
									    <asp:Button ID="btnBack" runat="server" Height="30px" Text="취소" Width="126px" OnClick="btnBack_Click" />
	                                </font>
                                </td>
							</tr>
							<tr>
								<td class="auto-style2" style="background-color: #333333; color: #FFFFFF;" colspan="6">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
									<asp:Label ID="Label5" runat="server" Font-Bold="True" Text="데이터 입력"></asp:Label>
								</td>
							</tr>
                            <tr>
								<td class="auto-style20" colspan="6"></td>
							</tr>
                            <tr>
								<td class="auto-style2" style="background-color: #ffffff;border-bottom:1px solid #333333 ; color: #333333;" colspan="6">&nbsp;&nbsp; 
									<asp:Label ID="Label1" runat="server" Font-Size="Large" Font-Bold="True" Text="센서 값"></asp:Label>
								</td>
							</tr>
                            <tr>
                                <td class="auto-style26" colspan="6">
            <table class="auto-style31">
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        수소 이온 농도(pH)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorPH" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        용존산소(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorDO" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
                        ORP</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorORP" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        전도도(ms/cm)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorConductivity" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        수심(m)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorDepth" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        수온(<span style="font-size:9.0pt;font-family:&quot;맑은 고딕&quot;;
mso-ascii-font-family:&quot;맑은 고딕&quot;;mso-fareast-font-family:&quot;맑은 고딕&quot;;mso-bidi-font-family:
+mn-cs;mso-ascii-theme-font:minor-latin;mso-fareast-theme-font:minor-fareast;
mso-bidi-theme-font:minor-bidi;color:#595959;mso-color-index:1;mso-font-kerning:
12.0pt;language:en-US;font-weight:bold;mso-style-textfill-type:solid;
mso-style-textfill-fill-themecolor:text1;mso-style-textfill-fill-color:#595959;
mso-style-textfill-fill-alpha:100.0%;mso-style-textfill-fill-colortransforms:
&quot;lumm=65000 lumo=35000&quot;">°C)</span></td>
                    <td>
                        <asp:TextBox ID="textBoxSensorTemp" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
                        질산성 질소(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorNO3N" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
	<font face="맑은 고딕" color="black" size="2">
                        암모니아성 질소(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorNH4" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        총 질소(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorTN" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        인산염 인(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorPO4" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
                        총 인(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorTP" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        혼탁도</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorTurbidity" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        염록소</td>
                    <td>
                        <asp:TextBox ID="textBoxSensorChlorophyll" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                                            
                    <td>&nbsp;</td>
                </tr>
	        </table>
                                    
                                </td>
                            </tr>
                            <tr style="height:50px;">
                                <td colspan="6"></td>
                            </tr>
                            <tr>
								<td class="auto-style2" style="background-color: #ffffff;border-bottom:1px solid #333333 ;color: #333333;" colspan="6">&nbsp;&nbsp; 
									<asp:Label ID="Label2" runat="server" Font-Size="Large" Font-Bold="True" Text="측정소 값"></asp:Label>
								</td>
							</tr>
                            <tr>
                                <td class="auto-style26" colspan="6">
            <table class="auto-style27">
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        수소 이온 농도(pH)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationPH" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        용존산소(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationDO" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
	<font face="맑은 고딕" color="black" size="2">
                        총 질소(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationTN" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
	<font face="맑은 고딕" color="black" size="2">
                        총 인(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationTP" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        TOC(mg/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationTOC" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
	<font face="맑은 고딕" color="black" size="2">
                        수온(<span style="font-size:9.0pt;font-family:&quot;맑은 고딕&quot;;
mso-ascii-font-family:&quot;맑은 고딕&quot;;mso-fareast-font-family:&quot;맑은 고딕&quot;;mso-bidi-font-family:
+mn-cs;mso-ascii-theme-font:minor-latin;mso-fareast-theme-font:minor-fareast;
mso-bidi-theme-font:minor-bidi;color:#595959;mso-color-index:1;mso-font-kerning:
12.0pt;language:en-US;font-weight:bold;mso-style-textfill-type:solid;
mso-style-textfill-fill-themecolor:text1;mso-style-textfill-fill-color:#595959;
mso-style-textfill-fill-alpha:100.0%;mso-style-textfill-fill-colortransforms:
&quot;lumm=65000 lumo=35000&quot;">°C)</span></td>
                    <td>
                        <asp:TextBox ID="textBoxStationTemp" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
                        전기전도도(㎛hos/㎝)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationEC" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
	                    클로로필 a(㎎/㎥)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationChlorophyllA" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        암모니아성 질소(㎎/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationNH3N" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        질산성 질소(㎎/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationNO3N" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="auto-style33">
                        인산염인(㎎/L)</td>
                    <td>
                        <asp:TextBox ID="textBoxStationPO4P" runat="server" Width="28px"></asp:TextBox>
                    </td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                                            
                    <td>&nbsp;</td>
                </tr>
	        </table>
                                    
                                </td>
                            </tr>
    </div>
    </form>
</body>
</html>
