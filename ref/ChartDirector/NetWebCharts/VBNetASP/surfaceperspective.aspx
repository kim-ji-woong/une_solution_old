<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Create chart
'
Protected Sub createChart(viewer As WebChartViewer, img As String)

    ' The x and y coordinates of the grid
    Dim dataX() As Double = {0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0}
    Dim dataY() As Double = {0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0}

    ' The values at the grid points. In this example, we will compute the values
    ' using the formula z = sin((x - 0.5) * 2 * pi) * sin((y - 0.5) * 2 * pi)
    Dim dataZ((UBound(dataX) + 1) * (UBound(dataY) + 1) - 1) As Double
    For yIndex As Integer = 0 To UBound(dataY)
        Dim y As Double = (dataY(yIndex) - 0.5) * 2 * 3.1416
        For xIndex As Integer = 0 To UBound(dataX)
            Dim x As Double = (dataX(xIndex) - 0.5) * 2 * 3.1416
            dataZ(yIndex * (UBound(dataX) + 1) + xIndex) = Math.Sin(x) * Math.Sin(y)
        Next
    Next

    ' the perspective level
    Dim perspective As Integer = CInt(img) * 12

    ' Create a SurfaceChart object of size 360 x 360 pixels, with white (ffffff)
    ' background and grey (888888) border.
    Dim c As SurfaceChart = New SurfaceChart(360, 360, &Hffffff, &H888888)

    ' Set the perspective level
    c.setPerspective(perspective)
    c.addTitle("Perspective = " & perspective)

    ' Set the center of the plot region at (195, 165), and set width x depth x height
    ' to 200 x 200 x 150 pixels
    c.setPlotRegion(195, 165, 200, 200, 150)

    ' Set the plot region wall thichness to 5 pixels
    c.setWallThickness(5)

    ' Set the elevation and rotation angles to 30 and 30 degrees
    c.setViewAngle(30, 30)

    ' Set the data to use to plot the chart
    c.setData(dataX, dataY, dataZ)

    ' Spline interpolate data to a 40 x 40 grid for a smooth surface
    c.setInterpolation(40, 40)

    ' Use smooth gradient coloring.
    c.colorAxis().setColorGradient()

    ' Output the chart
    viewer.Image = c.makeWebImage(Chart.JPG)

End Sub

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    createChart(WebChartViewer0, "0")
    createChart(WebChartViewer1, "1")
    createChart(WebChartViewer2, "2")
    createChart(WebChartViewer3, "3")
    createChart(WebChartViewer4, "4")
    createChart(WebChartViewer5, "5")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Surface Perspective</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Surface Perspective
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer0" runat="server" />
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
    <chart:WebChartViewer id="WebChartViewer2" runat="server" />
    <chart:WebChartViewer id="WebChartViewer3" runat="server" />
    <chart:WebChartViewer id="WebChartViewer4" runat="server" />
    <chart:WebChartViewer id="WebChartViewer5" runat="server" />
</body>
</html>

