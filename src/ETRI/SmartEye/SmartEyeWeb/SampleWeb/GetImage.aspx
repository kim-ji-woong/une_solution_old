<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetImage.aspx.cs" Inherits="SmartEyeWeb.SampleWeb.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:Timer ID="timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>
        <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional"> 
            <ContentTemplate>
                <div>
    
                    <asp:Button ID="btnBeginDisaster" runat="server" OnClick="btnBeginDisaster_Click" Text="BeginDisaster" />
    
                    <asp:Button ID="btnGetDisasterImage" runat="server" OnClick="btnGetDisasterImage_Click" Text="GetDisasterImage" />
    
                </div>
                <br />
                <asp:Label ID="labelResult" runat="server" Text="결과"></asp:Label>
                <br />
                <br />
                <asp:Image ID="Image1" runat="server" />
            </ContentTemplate>
             <Triggers> 
                <asp:AsyncPostBackTrigger ControlID="timer1" EventName="Tick" /> 
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
