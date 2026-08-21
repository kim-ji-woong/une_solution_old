Imports System
Imports Microsoft.VisualBasic
Imports ChartDirector

Public Class roundmeter
    Implements DemoModule

    'Name of demo module
    Public Function getName() As String Implements DemoModule.getName
        Return "Round Meter"
    End Function

    'Number of charts produced in this demo module
    Public Function getNoOfCharts() As Integer Implements DemoModule.getNoOfCharts
        Return 1
    End Function

    'Main code for creating chart.
    'Note: the argument img is unused because this demo only has 1 chart.
    Public Sub createChart(viewer As WinChartViewer, img As String) _
        Implements DemoModule.createChart

        ' The value to display on the meter
        Dim value As Double = 45.17

        ' Create an AugularMeter object of size 200 x 200 pixels, with silver
        ' background, black border, 2 pixel 3D depressed border and rounded corners.
        Dim m As AngularMeter = New AngularMeter(200, 200, Chart.silverColor(), _
            &H000000, -2)
        m.setRoundedFrame()

        ' Set the meter center at (100, 100), with radius 85 pixels, and span from
        ' -135 to +135 degress
        m.setMeter(100, 100, 85, -135, 135)

        ' Meter scale is 0 - 100, with major tick every 10 units, minor tick every 5
        ' units, and micro tick every 1 units
        m.setScale(0, 100, 10, 5, 1)

        ' Disable default angular arc by setting its width to 0. Set 2 pixels line
        ' width for major tick, and 1 pixel line width for minor ticks.
        m.setLineWidth(0, 2, 1)

        ' Set the circular meter surface as metallic blue (9999DD)
        m.addRing(0, 90, Chart.metalColor(&H9999dd))

        ' Add a blue (6666FF) ring between radii 88 - 90 as decoration
        m.addRing(88, 90, &H6666ff)

        ' Set 0 - 60 as green (99FF99) zone, 60 - 80 as yellow (FFFF00) zone, and 80
        ' - 100 as red (FF3333) zone
        m.addZone(0, 60, &H99ff99)
        m.addZone(60, 80, &Hffff00)
        m.addZone(80, 100, &Hff3333)

        ' Add a text label centered at (100, 135) with 15 pts Arial Bold font
        m.addText(100, 135, "CPU", "Arial Bold", 15, Chart.TextColor, Chart.Center)

        ' Add a text box centered at (100, 165) showing the value formatted to 2
        ' decimal places, using white text on a black background, and with 1 pixel 3D
        ' depressed border
        m.addText(100, 165, m.formatValue(value, "2"), "Arial", 8, &Hffffff, _
            Chart.Center).setBackground(&H000000, &H000000, -1)

        ' Add a semi-transparent blue (40333399) pointer at the specified value
        m.addPointer(value, &H40333399)

        ' Output the chart
        viewer.Chart = m

    End Sub

End Class

