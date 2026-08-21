<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="WebApplication1.Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <script>
            function Redirect() {
                var form = document.getElementById("form1");
                form.submit();
            }
        </script>

        
    <div>
    
        <asp:Button ID="btnFire" runat="server" OnClick="btnFire_Click" Text="화재" Font-Size="XX-Large" Height="143px" Width="200px" />
    
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnPSM" runat="server" Font-Size="XX-Large" Height="143px" OnClick="btnPSM_Click" Text="오염" Width="200px" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnEarthquake" runat="server" Font-Size="XX-Large" Height="143px" OnClick="btnEarthquake_Click" Text="지진" Width="200px" />
        <br />
        <br />
        <br />
        <br />
        <asp:Button ID="btnStopFire" runat="server" Font-Size="XX-Large" Height="143px" OnClick="btnStopFire_Click" Text="화재중지" Width="200px" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnStopPSM" runat="server" Font-Size="XX-Large" Height="143px" OnClick="btnStopPSM_Click" Text="오염중지" Width="200px" />
    
    </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:Timer ID="Timer1" runat="server" Interval="1000" OnTick="Timer1_Tick">
        </asp:Timer>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers> 
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" /> 
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
