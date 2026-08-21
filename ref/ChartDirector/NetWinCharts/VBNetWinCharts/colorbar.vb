Imports System
Imports Microsoft.VisualBasic
Imports ChartDirector

Public Class colorbar
    Implements DemoModule

    'Name of demo module
    Public Function getName() As String Implements DemoModule.getName
        Return "Multi-Color Bar Chart"
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
        Dim data() As Double = {85, 156, 179.5, 211, 123}

        ' The labels for the bar chart
        Dim labels() As String = {"Mon", "Tue", "Wed", "Thu", "Fri"}

        ' The colors for the bar chart
        Dim colors() As Integer = {&Hb8bc9c, &Ha0bdc4, &H999966, &H333366, &Hc3c3e6}

        ' Create a XYChart object of size 300 x 220 pixels. Use golden background
        ' color. Use a 2 pixel 3D border.
        Dim c As XYChart = New XYChart(300, 220, Chart.goldColor(), -1, 2)

        ' Add a title box using 10 point Arial Bold font. Set the background color to
        ' metallic blue (9999FF) Use a 1 pixel 3D border.
        c.addTitle("Daily Network Load", "Arial Bold", 10).setBackground( _
            Chart.metalColor(&H9999ff), -1, 1)

        ' Set the plotarea at (40, 40) and of 240 x 150 pixels in size
        c.setPlotArea(40, 40, 240, 150)

        ' Add a multi-color bar chart layer using the given data and colors. Use a 1
        ' pixel 3D border for the bars.
        c.addBarLayer3(data, colors).setBorderColor(-1, 1)

        ' Set the labels on the x axis.
        c.xAxis().setLabels(labels)

        ' Output the chart
        viewer.Chart = c

        'include tool tip for the chart
        viewer.ImageMap = c.getHTMLImageMap("clickable", "", _
            "title='{xLabel}: {value} GBytes'")

    End Sub

End Class

