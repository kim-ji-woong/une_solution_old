Imports System
Imports Microsoft.VisualBasic
Imports ChartDirector

Public Class stackedbar
    Implements DemoModule

    'Name of demo module
    Public Function getName() As String Implements DemoModule.getName
        Return "Stacked Bar Chart"
    End Function

    'Number of charts produced in this demo module
    Public Function getNoOfCharts() As Integer Implements DemoModule.getNoOfCharts
        Return 1
    End Function

    'Main code for creating chart.
    'Note: the argument img is unused because this demo only has 1 chart.
    Public Sub createChart(viewer As WinChartViewer, img As String) _
        Implements DemoModule.createChart

        ' The data for the bar chart
        Dim data0() As Double = {100, 125, 245, 147, 67}
        Dim data1() As Double = {85, 156, 179, 211, 123}
        Dim data2() As Double = {97, 87, 56, 267, 157}

        ' The labels for the bar chart
        Dim labels() As String = {"Mon", "Tue", "Wed", "Thu", "Fri"}

        ' Create a XYChart object of size 500 x 320 pixels
        Dim c As XYChart = New XYChart(500, 320)

        ' Set the plotarea at (100, 40) and of size 280 x 240 pixels
        c.setPlotArea(100, 40, 280, 240)

        ' Add a legend box at (400, 100)
        c.addLegend(400, 100)

        ' Add a title to the chart using 14 points Times Bold Itatic font
        c.addTitle("Weekday Network Load", "Times New Roman Bold Italic", 14)

        ' Add a title to the y axis. Draw the title upright (font angle = 0)
        c.yAxis().setTitle("Average<*br*>Workload<*br*>(MBytes<*br*>Per Hour)" _
            ).setFontAngle(0)

        ' Set the labels on the x axis
        c.xAxis().setLabels(labels)

        ' Add a stacked bar layer and set the layer 3D depth to 8 pixels
        Dim layer As BarLayer = c.addBarLayer2(Chart.Stack, 8)

        ' Add the three data sets to the bar layer
        layer.addDataSet(data0, &Hff8080, "Server # 1")
        layer.addDataSet(data1, &H80ff80, "Server # 2")
        layer.addDataSet(data2, &H8080ff, "Server # 3")

        ' Enable bar label for the whole bar
        layer.setAggregateLabelStyle()

        ' Enable bar label for each segment of the stacked bar
        layer.setDataLabelStyle()

        ' Output the chart
        viewer.Chart = c

        'include tool tip for the chart
        viewer.ImageMap = c.getHTMLImageMap("clickable", "", _
            "title='{dataSetName} on {xLabel}: {value} MBytes/hour'")

    End Sub

End Class

