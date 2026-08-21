<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreSafeInputData.aspx.cs" Inherits="SenarioWeb.PreSafeInputData" %>

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
	.auto-style4 {
		height: 36px;
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
	.auto-style29 {
		height: 36px;
		width: 300px;
	}
	.auto-style31 {
		width: 100%;
		height: 121px;
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
								<td class="auto-style4" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp; <span class="auto-style3"><strong>PreSafe</strong></span></td>
								<td class="auto-style29">
									&nbsp;</td>
                                <td class="auto-style29" style="text-align:right;">
	<font face="맑은 고딕" color="black" size="2">
									<asp:Button ID="btnBack" runat="server" Height="30px" Text="취소" Width="126px" OnClick="btnBack_Click" />
	</font>
                                    </td>
								<td class="auto-style16" style="text-align:left">
	<font face="맑은 고딕" color="black" size="2">
									<asp:Button ID="btnSave" runat="server" Height="30px" Text="저장" Width="126px" OnClick="btnSave_Click" />
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
                                <td class="auto-style26" colspan="6">
            <table class="auto-style31">
		        <tr>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
			        <td class="checkboxTD-style">
				        <asp:CheckBox ID="chkLocation" runat="server" OnCheckedChanged="chkLocation_CheckedChanged" Text="위치" />
			        </td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
			        <td class="checkboxTD-style">
				        <asp:CheckBox ID="chkHeartRate" runat="server" OnCheckedChanged="chkHeartRate_CheckedChanged" Text="심박수" />
			        </td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
			        <td class="checkboxTD-style">
				        <asp:CheckBox ID="chkAcc" runat="server" OnCheckedChanged="chkAcc_CheckedChanged" Text="가속도" />
			        </td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
			        <td class="checkboxTD-style">
				        <asp:CheckBox ID="chkAlcohol" runat="server" OnCheckedChanged="chkAlcohol_CheckedChanged" Text="음주" />
			        </td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
			        <td class="checkboxTD-style">
				        <asp:CheckBox ID="chkScream" runat="server" OnCheckedChanged="chkScream_CheckedChanged" Text="비명" />
			        </td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
			        <td class="checkboxTD-style">
				        <asp:CheckBox ID="chkImpact" runat="server" OnCheckedChanged="chkImpact_CheckedChanged" Text="충격" />
			        </td>
                    <td class="nonCheckboxTD-style">&nbsp;</td>
		        </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        <asp:DropDownList ID="cmbLocation" runat="server" Width ="100">
                            <asp:ListItem>실내</asp:ListItem>
                            <asp:ListItem>실외</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        <asp:TextBox ID="txtHeartRate" runat="server" Width="40"></asp:TextBox>
                        회/분</td>
                    <td>&nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        <asp:DropDownList ID="cmbAcc" runat="server" Width ="100">
                            <asp:ListItem>정지</asp:ListItem>
                            <asp:ListItem>걷기</asp:ListItem>
                            <asp:ListItem>뛰기</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        <asp:DropDownList ID="cmbAlcohol" runat="server" Width ="100">
                            <asp:ListItem>유</asp:ListItem>
                            <asp:ListItem>무</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        <asp:DropDownList ID="cmbScream" runat="server" Width ="100">
                            <asp:ListItem>유</asp:ListItem>
                            <asp:ListItem>무</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                                            
                    <td>&nbsp;</td>
                    <td class="contentsTD-style">
                        <asp:DropDownList ID="cmbImpact" runat="server" Width ="100">
                            <asp:ListItem>유</asp:ListItem>
                            <asp:ListItem>무</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
	        </table>
                                    
                                </td>
                            </tr>
    </div>
    </form>
</body>
</html>
