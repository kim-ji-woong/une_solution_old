<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' The data for the chart
    Dim dataY0() As Double = {4, 4.5, 5, 5.25, 5.75, 5.25, 5, 4.5, 4, 3, 2.5, 2.5}
    Dim dataX0() As Date = {DateSerial(1997, 1, 1), DateSerial(1998, 6, 25), _
        DateSerial(1999, 9, 6), DateSerial(2000, 2, 6), DateSerial(2000, 9, 21), _
        DateSerial(2001, 3, 4), DateSerial(2001, 6, 8), DateSerial(2002, 2, 4), _
        DateSerial(2002, 5, 19), DateSerial(2002, 8, 16), DateSerial(2002, 12, 1), _
        DateSerial(2003, 1, 1)}

    Dim dataY1() As Double = {7, 6.5, 6, 5, 6.5, 7, 6, 5.5, 5, 4, 3.5, 3.5}
    Dim dataX1() As Date = {DateSerial(1997, 1, 1), DateSerial(1997, 7, 1), _
        DateSerial(1997, 12, 1), DateSerial(1999, 1, 15), DateSerial(1999, 6, 9), _
        DateSerial(2000, 3, 3), DateSerial(2000, 8, 13), DateSerial(2001, 5, 5), _
        DateSerial(2001, 9, 16), DateSerial(2002, 3, 16), DateSerial(2002, 6, 1), _
        DateSerial(2003, 1, 1)}

    ' Create a XYChart object of size 500 x 270 pixels, with a pale blue (e0e0ff)
    ' background, black border, 1 pixel 3D border effect and rounded corners
    Dim c As XYChart = New XYChart(600, 300, &He0e0ff, &H000000, 1)
    c.setRoundedFrame()

    ' Set the plotarea at (55, 60) and of size 520 x 200 pixels, with white (ffffff)
    ' background. Set horizontal and vertical grid lines to grey (cccccc).
    c.setPlotArea(50, 60, 525, 200, &Hffffff, -1, -1, &Hcccccc, &Hcccccc)

    ' Add a legend box at (55, 32) (top of the chart) with horizontal layout. Use 9
    ' pts Arial Bold font. Set the background and border color to Transparent.
    c.addLegend(55, 32, False, "Arial Bold", 9).setBackground(Chart.Transparent)

    ' Add a title box to the chart using 15 pts Times Bold Italic font. The text is
    ' white (ffffff) on a deep blue (000088) background, with soft lighting effect
    ' from the right side.
    c.addTitle("Long Term Interest Rates", "Times New Roman Bold Italic", 15, _
        &Hffffff).setBackground(&H000088, -1, Chart.softLighting(Chart.Right))

    ' Set the y axis label format to display a percentage sign
    c.yAxis().setLabelFormat("{value}%")

    ' Add a red (ff0000) step line layer to the chart and set the line width to 2
    ' pixels
    Dim layer0 As StepLineLayer = c.addStepLineLayer(dataY0, &Hff0000, _
        "Country AAA")
    layer0.setXData(dataX0)
    layer0.setLineWidth(2)

    ' Add a blue (0000ff) step line layer to the chart and set the line width to 2
    ' pixels
    Dim layer1 As StepLineLayer = c.addStepLineLayer(dataY1, &H0000ff, _
        "Country BBB")
    layer1.setXData(dataX1)
    layer1.setLineWidth(2)

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='{dataSetName} change to {value}% on {x|mmm dd, yyyy}'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Step Line Chart</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Step Line Chart
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

