<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Create chart
'
Protected Sub createChart(viewer As WebChartViewer, img As String)

    ' The data for the chart
    Dim data() As Double = {100, 125, 265, 147, 67, 105}
    Dim labels() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun"}

    ' Create a XYChart object of size 250 x 250 pixels
    Dim c As XYChart = New XYChart(250, 250)

    ' Set the plot area at (27, 25) and of size 200 x 200 pixels
    c.setPlotArea(27, 25, 200, 200)

    If img = "1" Then
        ' High tick density, uses 10 pixels as tick spacing
        c.addTitle("Tick Density = 10 pixels")
        c.yAxis().setTickDensity(10)
    Else
        ' Normal tick density, just use the default setting
        c.addTitle("Default Tick Density")
    End If

    ' Set the labels on the x axis
    c.xAxis().setLabels(labels)

    ' Add a color bar layer using the given data. Use a 1 pixel 3D border for the
    ' bars.
    c.addBarLayer3(data).setBorderColor(-1, 1)

    ' Output the chart
    viewer.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    viewer.ImageMap = c.getHTMLImageMap("", "", _
        "title='Revenue for {xLabel}: US${value}M'")

End Sub

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    createChart(WebChartViewer0, "0")
    createChart(WebChartViewer1, "1")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Tick Density</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Tick Density
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer0" runat="server" />
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

