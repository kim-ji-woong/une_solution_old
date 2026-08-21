<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sample.aspx.cs" Inherits="SensorSimulationWeb.Sample" %>

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
	.auto-style1 {
		width: 944px;
		height: 430px;
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
		width: 40px;
	}
	.auto-style19 {
		text-align: right;
		height: 44px;
	}
	.auto-style20 {
		height: 2px;
	}
	.auto-style25 {
		height: 195px;
	}
	.auto-style26 {
	}
	.auto-style27 {
		height: 195px;
		width: 284px;
	}
	.auto-style28 {
		height: 195px;
		width: 368px;
	}
	.auto-style29 {
		height: 36px;
		width: 120px;
	}
	.auto-style30 {
		height: 22px;
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

</style>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>PreSafe</title>
</head>
<body>
	<form id="form2" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	<div>
	<font face="맑은 고딕" color="black" size="2">
		<div id="container">
			<div id="contents_area">
				<div id="contents_area1">
					<div id="content">   
						<table cellpadding="0" cellspacing="0" class="auto-style1">
							<tr>
								<td class="auto-style4" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp; <span class="auto-style3"><strong>상황인지 알고리즘 시뮬레이션</strong></span></td>
								<td class="auto-style29">
	                                <font face="맑은 고딕" color="black" size="2">
									    <asp:Button ID="btnAddData" runat="server" Height="30px" Text="측정값 입력" Width="126px" OnClick="btnAddData_Click" />
	                                </font>
								</td>
								<td class="auto-style16">
	                                <font face="맑은 고딕" color="black" size="2">
                                        <asp:Button ID="btnRun" runat="server" Height="30px" Text="결과 보기" Width="126px" OnClick="btnRun_Click" />
	                                </font>
								</td>
							</tr>
							<tr>
								<td class="auto-style2" style="background-color: #333333; color: #FFFFFF;" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
									<asp:Label ID="labelSelectedScenario" runat="server" Font-Bold="True" Text="선택된 시나리오"></asp:Label>
								</td>
								<td class="auto-style19" colspan="3" style="background-color: #333333; color: #FFFFFF;">&nbsp;&nbsp; </td>
							</tr>
							<tr>
								<td class="auto-style20" colspan="5"></td>
							</tr>
							<tr>
								<td class="auto-style27">
									<asp:Panel ID="Panel1" runat="server" Height="183px" BackColor="#0099FF" Width="262px">
										<br />
										<br />
										&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:Label ID="labelResultTitle" runat="server" Text="수질 현황" ForeColor="White" style="font-size: x-large; font-weight: 700"></asp:Label>
										<br />
										&nbsp;&nbsp;
										<br />
										&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<br />									&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:Label ID="labelResult" runat="server" Text="0점" ForeColor="White" style="font-weight: 700; font-size: medium"></asp:Label>
									</asp:Panel>
								</td>
								<td class="auto-style28" Style="vertical-align:top">
									<asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Names="맑은 고딕" Font-Size="Large" ForeColor="#999999" Text="상황인지 카테고리"></asp:Label>
                                    <br />
									<hr style="color: #00FF00; height: 2px;" />
	                                <asp:RadioButtonList ID="radioMode" runat="server" AutoPostBack="True" OnSelectedIndexChanged="radioMode_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">수집데이터 모니터링</asp:ListItem>
                                        <asp:ListItem>예측 모델링</asp:ListItem>
                                    </asp:RadioButtonList>
									<br />

								</td>
								<td class="auto-style25" colspan="3">
	<font face="맑은 고딕" color="black" size="2">
		&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Names="맑은 고딕" Font-Size="Large" ForeColor="#999999" Text="시나리오 리스트"></asp:Label>
	</font>
									<br />
&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:ListBox ID="listBoxScenario" runat="server" Height="165px" Width="225px" AutoPostBack="True">
										<asp:ListItem></asp:ListItem>
										<asp:ListItem></asp:ListItem>
									</asp:ListBox>
								</td>
							</tr>
							<tr>
								<td class="auto-style30" colspan="5"><hr /></td>
							</tr>
							<tr>
								<td class="auto-style26" colspan="5">

			                        &nbsp;</td>
							</tr>
						</table>
					</div>
				</div>
			</div>
			<div id="footer">
            </div>
		</div>
	</font>
	</div>
	</form>
</body>
</html>