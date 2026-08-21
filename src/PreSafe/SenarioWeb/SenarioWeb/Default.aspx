<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SenarioWeb._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


        <div class ="content-wrapper" style="height: 80px; background-color: #000000;"> 
    <section class="featured" style="height: 34px; margin-bottom: 0px; background-color: #FFFFFF; width: 1478px;">
            <table style="table-layout:fixed; height: 70px; width: 1152px; margin-right: 2px;">
                <tr>
                    <td style="width: 196px; height: 14px;"> &nbsp;</td>
                    <td style="width: 584px; height: 14px;"> 
                        </td>
                    <td style="width: 200px; height: 14px;">
                        &nbsp;</td>
                    <td style="width: 57px; height: 14px;">
                        <asp:ImageButton ID="ImageButton2" runat="server" Height="33px" ImageUrl="~/Images/btnPrediction.png" Width="77px" OnClick="ImageButton2_Click" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 196px; height: 29px; text-align: center;">
                        <asp:Label ID="lblSenarioTitle" runat="server" Text="선택된 시나리오 이름" Font-Size="12pt" ForeColor="#009933" Font-Bold="True"></asp:Label>
                        </td>
                    <td style="height: 29px; text-align: right; width: 584px; color: #FFFFFF; font-weight: bold;">
                        시나리오&nbsp;&nbsp;&nbsp;&nbsp; </td>
                    <td style="width: 200px; height: 29px;">
                        <asp:TextBox ID="TextBox10" runat="server" Height="26px" Width="206px"></asp:TextBox>
                        </td>
                    <td style="width: 57px; height: 29px; text-align: right;">
                        <asp:ImageButton ID="ImageButton1" runat="server" Height="26px" ImageUrl="~/Images/btn_search.gif" style="margin-left: 0px" Width="51px" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
                </table>       
    </section>
        </div>
    <div style="height: 62px">

    </div>
    <div style="height: 390px">
            <div style="float:left; width: 400px;">
             <asp:Panel ID="Panel1" runat="server" BackColor="#00AEBD" Font-Bold="True" Font-Size="18pt" ForeColor="White" Height="207px" Width="355px">
                 <br />
                 &nbsp;&nbsp;&nbsp; 범죄 가능성<br />
                 <br />
                 <br />
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                 <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            </asp:Panel>
            </div>
            <div style="float:left; width: 359px;">
        <asp:Panel ID="Panel2" runat="server" Height="207px" Width="356px">
            <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="입력값" Font-Size="17pt"></asp:Label>
            <hr aria-orientation="horizontal" style="color: #CCFF99; border:solid; height: 1px; background-color: #CCFF33;"/>
            소리
            <asp:Label ID="lblUseSound" runat="server" Text="Label"></asp:Label>
            &nbsp;db
            <br />
            맥박
            <asp:Label ID="lblUseHeartBeat" runat="server" Text="Label"></asp:Label>
            &nbsp;회/분<br /> 위치
            <asp:Label ID="lblUseLocation" runat="server" Text="Label"></asp:Label>
            <br />
            <asp:Label ID="lblUseImpact" runat="server" Text="Label"></asp:Label>
            <br />
            속도
            <asp:Label ID="lblUseVelocity" runat="server" Text="Label"></asp:Label>
            &nbsp;km/hour</asp:Panel>
                </div>
        <div style="float:right; width: 268px; height: 385px;">
                <asp:Panel ID="Panel3" runat="server" Width="267px" Height="378px" BorderStyle="Groove">
                    시나리오 리스트<asp:Button ID="Button1" runat="server" Text="Button" />
                    <asp:Button ID="Button2" runat="server" Text="Button" />
                    <br />
                    <hr aria-sort="none" />
                    <asp:ListBox ID="ListBox1" runat="server" Height="294px" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged" Rows="8" Width="258px">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                    </asp:ListBox>
            </asp:Panel>

        </div>

                </div>
     <hr style="font-size: 20px" />
        <p>
            <table width=600px height=100px style="table-layout:fixed">
                <tr>
                    <td style="width: 354px; height: 29px;">
                        <asp:CheckBox ID="chkUseSound" runat="server" OnCheckedChanged="chkUseSound_CheckedChanged" />
                        소리 :
                        <asp:TextBox ID="txtUseSound" runat="server" Height="21px" Width="113px"></asp:TextBox>
                    &nbsp;db</td>
                    <td style="width: 377px; height: 29px;">
                        <asp:CheckBox ID="chkHeartBeat" runat="server" />
                        맥박 :&nbsp;&nbsp;
                        <asp:TextBox ID="txtHeartBeat" runat="server" Height="21px" Width="113px"></asp:TextBox>
                    &nbsp;회/분</td>
                    <td style="width: 359px; height: 29px;">
                        <asp:CheckBox ID="chkUseAlcole" runat="server"/>
                        알콜수치 :
                        <asp:TextBox ID="txtUseAlcole" runat="server" Height="21px" Width="113px"></asp:TextBox>
                    &nbsp;%</td>
                </tr>
                <tr>
                    <td style="width: 354px; font-size: x-small; height: 26px;">
                        <asp:CheckBox ID="chkUseVelocity" runat="server" style="font-size: larger" />
                        <span style="font-size: 15px">속도 : <asp:TextBox ID="txtUseVelocity" runat="server" Height="21px" Width="113px" style="font-size: small"></asp:TextBox>
                    &nbsp;km/hour</span></td>
                    <td style="width: 377px; height: 26px;">
                        <asp:CheckBox ID="chkUseAcc" runat="server" />
                        가속도 :
                        <asp:TextBox ID="txtUseAcc" runat="server" Height="21px" Width="113px" style="font-size: small"></asp:TextBox>
                    &nbsp;m/sec</td>
                    <td style="width: 359px; height: 26px;"></td>
                </tr>
                <tr>
                    <td style="width: 354px; font-size: x-small; height: 24px;">
                        <asp:CheckBox ID="chkUseLocation" runat="server" style="font-size: larger" />
                        <span style="font-size: 15px">위치
                        <br />
                        </span>
                        <asp:ListBox ID="ListUseLoaction" runat="server" Height="90px" Rows="5" Width="92px">
                            <asp:ListItem>집</asp:ListItem>
                            <asp:ListItem>직장</asp:ListItem>
                            <asp:ListItem>외곽지</asp:ListItem>
                            <asp:ListItem>접근금지 구역</asp:ListItem>
                            <asp:ListItem>기타</asp:ListItem>
                        </asp:ListBox>
                    </td>
                    <td style="width: 377px; height: 24px;">
                        <asp:CheckBox ID="chkUseImpact" runat="server" OnCheckedChanged="chkUseImpact_CheckedChanged" />
                        충격 유무<br />
                        <asp:ListBox ID="ListUseImpact" runat="server">
                            <asp:ListItem>있음</asp:ListItem>
                            <asp:ListItem>없음</asp:ListItem>
                        </asp:ListBox>
                    </td>
                    <td style="width: 359px; height: 24px;">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td style="width: 354px">&nbsp;</td>
                    <td style="width: 377px">&nbsp;</td>
                    <td style="width: 359px; text-align: right;">
                        &nbsp;</td>
                </tr>
            </table>
        </p>
        <p>
            &nbsp;</p>

</asp:Content>
