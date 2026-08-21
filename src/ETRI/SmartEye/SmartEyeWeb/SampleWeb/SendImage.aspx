<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendImage.aspx.cs" Inherits="SmartEyeWeb.SampleWeb.SendImage" %>

<!DOCTYPE html>

<html lang="ko">
<head><meta charset="utf-8" />
    <title>Post Test</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0">
                <tr>
                    <td style="text-align: right;width: 100px;">ImageURL : </td>
                    <td>&nbsp;<asp:TextBox ID="txtImageURL" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label1" runat="server" Text="ImageURL : "></asp:Label>
                        <asp:TextBox ID="textBoxSendImageURL" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">위도 : </td>
                    <td><asp:TextBox id="txtLatitude" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label2" runat="server" Text="위도 : "></asp:Label>
                        <asp:TextBox ID="textBoxSendImageLatitude" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">경도 : </td>
                    <td><asp:TextBox id="txtLongitude" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label3" runat="server" Text="경도 : "></asp:Label>
                        <asp:TextBox ID="textBoxSendImageLongitude" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">시간 : </td>
                    <td><asp:TextBox id="txtTime" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label4" runat="server" Text="시간 : "></asp:Label>
                        <asp:TextBox ID="textBoxSendImageTime" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">Description : </td>
                    <td><asp:TextBox id="textBoxDisasterDescription" runat="server" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label5" runat="server" Text="Description :"></asp:Label>
&nbsp;<asp:TextBox ID="textBoxSendImageDescription" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSendDisasterImage" runat="server" OnClick="btnSendDisasterImage_Click" Text="SendDisasterImageData" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSendImage" runat="server" OnClick="btnSendImage_Click" Text="SendImageData" />
        </div>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <div>
            <table border="0">
                <tr>
                </tr>
            </table>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnEndDisaster" runat="server" OnClick="btnEndDisaster_Click" Text="EndDisaster" />
        </div>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <div>
            <table border="0">
                <tr>
                    <td rowspan="2">&nbsp;<asp:RadioButtonList ID="radioActionStep" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" OnSelectedIndexChanged="radioActionStep_SelectedIndexChanged" >
                        <asp:ListItem Text="수집" Value="수집" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="분석" Value="분석"></asp:ListItem>
                        <asp:ListItem Text="예측" Value="예측"></asp:ListItem>
                        <asp:ListItem Text="경보" Value="경보"></asp:ListItem>
                        <asp:ListItem Text="대응" Value="대응"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;width: 100px;">Description : </td>
                    <td>&nbsp;<asp:TextBox ID="textBoxActionDescription" runat="server" >단계:수집중</asp:TextBox>
                    </td>
                </tr>
            </table>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSendActionData" runat="server" OnClick="btnSendActionData_Click" Text="SendActionData" />
        </div>
    </form>
</body>
</html>
