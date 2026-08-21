<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Create chart
'
Protected Sub createChart(viewer As WebChartViewer, img As String)

    ' The x and y coordinates of the grid
    Dim dataX() As Double = {-4, -3, -2, -1, 0, 1, 2, 3, 4}
    Dim dataY() As Double = {-4, -3, -2, -1, 0, 1, 2, 3, 4}

    ' The values at the grid points. In this example, we will compute the values
    ' using the formula z = Sin(x * pi / 3) * Sin(y * pi / 3).
    Dim dataZ((UBound(dataX) + 1) * (UBound(dataY) + 1) - 1) As Double
    For yIndex As Integer = 0 To UBound(dataY)
        Dim y As Double = dataY(yIndex)
        For xIndex As Integer = 0 To UBound(dataX)
            Dim x As Double = dataX(xIndex)
            dataZ(yIndex * (UBound(dataX) + 1) + xIndex) = Math.Sin(x * 3.1416 / 3) _
                 * Math.Sin(y * 3.1416 / 3)
        Next
    Next

    ' Create a XYChart object of size 360 x 360 pixels
    Dim c As XYChart = New XYChart(360, 360)

    ' Set the plotarea at (30, 25) and of size 300 x 300 pixels. Use semi-transparent
    ' black (c0000000) for both horizontal and vertical grid lines
    c.setPlotArea(30, 25, 300, 300, -1, -1, -1, &Hc0000000, -1)

    ' Add a contour layer using the given data
    Dim layer As ContourLayer = c.addContourLayer(dataX, dataY, dataZ)

    ' Set the x-axis and y-axis scale
    c.xAxis().setLinearScale(-4, 4, 1)
    c.yAxis().setLinearScale(-4, 4, 1)

    If img = "0" Then
        ' Discrete coloring, spline surface interpolation
        c.addTitle("Spline Surface - Discrete Coloring", "Arial Bold Italic", 12)
    ElseIf img = "1" Then
        ' Discrete coloring, linear surface interpolation
        c.addTitle("Linear Surface - Discrete Coloring", "Arial Bold Italic", 12)
        layer.setSmoothInterpolation(False)
    ElseIf img = "2" Then
        ' Smooth coloring, spline surface interpolation
        c.addTitle("Spline Surface - Continuous Coloring", "Arial Bold Italic", 12)
        layer.setContourColor(Chart.Transparent)
        layer.colorAxis().setColorGradient(True)
    Else
        ' Discrete coloring, linear surface interpolation
        c.addTitle("Linear Surface - Continuous Coloring", "Arial Bold Italic", 12)
        layer.setSmoothInterpolation(False)
        layer.setContourColor(Chart.Transparent)
        layer.colorAxis().setColorGradient(True)
    End If

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

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Contour Interpolation</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Contour Interpolation
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer0" runat="server" />
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
    <chart:WebChartViewer id="WebChartViewer2" runat="server" />
    <chart:WebChartViewer id="WebChartViewer3" runat="server" />
</body>
</html>

