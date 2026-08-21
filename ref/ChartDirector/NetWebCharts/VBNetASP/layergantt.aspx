<%@ Page Language="VB" Debug="true" %>
<%@ Import Namespace="ChartDirector" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<script runat="server">

'
' Page Load event handler
'
Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' the names of the tasks
    Dim labels() As String = {"Market Research", "Define Specifications", _
        "Overall Archiecture", "Project Planning", "Detail Design", _
        "Software Development", "Test Plan", "Testing and QA", "User Documentation"}

    ' the planned start dates and end dates for the tasks
    Dim startDate() As Date = {DateSerial(2004, 8, 16), DateSerial(2004, 8, 30), _
        DateSerial(2004, 9, 13), DateSerial(2004, 9, 20), DateSerial(2004, 9, 27), _
        DateSerial(2004, 10, 4), DateSerial(2004, 10, 25), DateSerial(2004, 11, 1), _
        DateSerial(2004, 11, 8)}
    Dim endDate() As Date = {DateSerial(2004, 8, 30), DateSerial(2004, 9, 13), _
        DateSerial(2004, 9, 27), DateSerial(2004, 10, 4), DateSerial(2004, 10, 11), _
        DateSerial(2004, 11, 8), DateSerial(2004, 11, 8), DateSerial(2004, 11, 22), _
        DateSerial(2004, 11, 22)}

    ' the actual start dates and end dates for the tasks up to now
    Dim actualStartDate() As Date = {DateSerial(2004, 8, 16), DateSerial(2004, 8, _
        27), DateSerial(2004, 9, 9), DateSerial(2004, 9, 18), DateSerial(2004, 9, _
        22)}
    Dim actualEndDate() As Date = {DateSerial(2004, 8, 27), DateSerial(2004, 9, 9), _
        DateSerial(2004, 9, 27), DateSerial(2004, 10, 2), DateSerial(2004, 10, 8)}

    ' Create a XYChart object of size 620 x 280 pixels. Set background color to light
    ' green (ccffcc) with 1 pixel 3D border effect.
    Dim c As XYChart = New XYChart(620, 280, &Hccffcc, &H000000, 1)

    ' Add a title to the chart using 15 points Times Bold Itatic font, with white
    ' (ffffff) text on a dark green (0x6000) background
    c.addTitle("Multi-Layer Gantt Chart Demo", "Times New Roman Bold Italic", 15, _
        &Hffffff).setBackground(&H006000)

    ' Set the plotarea at (140, 55) and of size 460 x 200 pixels. Use alternative
    ' white/grey background. Enable both horizontal and vertical grids by setting
    ' their colors to grey (c0c0c0). Set vertical major grid (represents month
    ' boundaries) 2 pixels in width
    c.setPlotArea(140, 55, 460, 200, &Hffffff, &Heeeeee, Chart.LineColor, &Hc0c0c0, _
        &Hc0c0c0).setGridWidth(2, 1, 1, 1)

    ' swap the x and y axes to create a horziontal box-whisker chart
    c.swapXY()

    ' Set the y-axis scale to be date scale from Aug 16, 2004 to Nov 22, 2004, with
    ' ticks every 7 days (1 week)
    c.yAxis().setDateScale(DateSerial(2004, 8, 16), DateSerial(2004, 11, 22), 86400 _
         * 7)

    ' Add a red (ff0000) dash line to represent the current day
    c.yAxis().addMark(Chart.CTime(DateSerial(2004, 10, 8)), c.dashLineColor( _
        &Hff0000, Chart.DashLine))

    ' Set multi-style axis label formatting. Month labels are in Arial Bold font in
    ' "mmm d" format. Weekly labels just show the day of month and use minor tick (by
    ' using '-' as first character of format string).
    c.yAxis().setMultiFormat(Chart.StartOfMonthFilter(), _
        "<*font=Arial Bold*>{value|mmm d}", Chart.StartOfDayFilter(), "-{value|d}")

    ' Set the y-axis to shown on the top (right + swapXY = top)
    c.setYAxisOnRight()

    ' Set the labels on the x axis
    c.xAxis().setLabels(labels)

    ' Reverse the x-axis scale so that it points downwards.
    c.xAxis().setReverse()

    ' Set the horizontal ticks and grid lines to be between the bars
    c.xAxis().setTickOffset(0.5)

    ' Use blue (0000aa) as the color for the planned schedule
    Dim plannedColor As Integer = &H0000aa

    ' Use a red hash pattern as the color for the actual dates. The pattern is
    ' created as a 4 x 4 bitmap defined in memory as an array of colors.
    Dim actualColor As Integer = c.patternColor(New Integer() {&Hffffff, &Hffffff, _
        &Hffffff, &Hff0000, &Hffffff, &Hffffff, &Hff0000, &Hffffff, &Hffffff, _
        &Hff0000, &Hffffff, &Hffffff, &Hff0000, &Hffffff, &Hffffff, &Hffffff}, 4)

    ' Add a box whisker layer to represent the actual dates. We add the actual dates
    ' layer first, so it will be the top layer.
    Dim actualLayer As BoxWhiskerLayer = c.addBoxLayer( _
        Chart.CTime(actualStartDate), Chart.CTime(actualEndDate), actualColor, _
        "Actual")

    ' Set the bar height to 8 pixels so they will not block the bottom bar
    actualLayer.setDataWidth(8)

    ' Add a box-whisker layer to represent the planned schedule date
    c.addBoxLayer(Chart.CTime(startDate), Chart.CTime(endDate), plannedColor, _
        "Planned").setBorderColor(Chart.SameAsMainColor)

    ' Add a legend box on the top right corner (595, 60) of the plot area with 8 pt
    ' Arial Bold font. Use a semi-transparent grey (80808080) background.
    Dim b As LegendBox = c.addLegend(595, 60, False, "Arial Bold", 8)
    b.setAlignment(Chart.TopRight)
    b.setBackground(&H80808080, -1, 2)

    ' Output the chart
    WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

    ' Include tool tip for the chart
    WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
        "title='{xLabel} ({dataSetName}): {top|mmm dd, yyyy} to {bottom|mmm dd, " & _
        "yyyy}'")

End Sub

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Multi-Layer Gantt Chart</title>
</head>
<body style="margin:5px 0px 0px 5px">
    <div style="font-size:18pt; font-family:verdana; font-weight:bold">
        Multi-Layer Gantt Chart
    </div>
    <hr style="border:solid 1px #000080" />
    <div style="font-size:10pt; font-family:verdana; margin-bottom:1.5em">
        <a href='viewsource.aspx?file=<%=Request("SCRIPT_NAME")%>'>View Source Code</a>
    </div>
    <chart:WebChartViewer id="WebChartViewer1" runat="server" />
</body>
</html>

