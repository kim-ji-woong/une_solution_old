Imports ChartDirector

Public Class FrmXYZoomScroll

    ' Data arrays
    Dim dataX0 As Double()
    Dim dataY0 As Double()
    Dim dataX1 As Double()
    Dim dataY1 As Double()
    Dim dataX2 As Double()
    Dim dataY2 As Double()

    ' Flag to indicated if initialization has been completed. Prevents events from firing before 
    ' controls are properly initialized.
    Private hasFinishedInitialization As Boolean

    Private Sub FrmXYZoomScroll_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load

        ' Load the data
        loadData()

        ' Initialize the WinChartViewer
        initChartViewer(winChartViewer1)

        ' Can handle events now
        hasFinishedInitialization = True

        ' Trigger the ViewPortChanged event to draw the chart
        winChartViewer1.updateViewPort(True, True)

    End Sub

    '
    ' Load the data
    '
    Private Sub loadData()

        '
        ' For simplicity, in this demo, we just use hard coded data.
        '
        dataX0 = New Double() {10, 15, 6, -12, 14, -8, 13, -3, 16, 12, 10.5, -7, 3, -10, -5, 2, 5}
        dataY0 = New Double() {130, 150, 80, 110, -110, -105, -130, -15, -170, 125, 125, 60, 25, 150, _
            150, 15, 120}
        dataX1 = New Double() {6, 7, -4, 3.5, 7, 8, -9, -10, -12, 11, 8, -3, -2, 8, 4, -15, 15}
        dataY1 = New Double() {65, -40, -40, 45, -70, -80, 80, 10, -100, 105, 60, 50, 20, 170, -25, 50, 75}
        dataX2 = New Double() {-10, -12, 11, 8, 6, 12, -4, 3.5, 7, 8, -9, 3, -13, 16, -7.5, -10, -15}
        dataY2 = New Double() {65, -80, -40, 45, -70, -80, 80, 90, -100, 105, 60, -75, -150, -40, 120, _
            -50, -30}

    End Sub

    '
    ' Initialize the WinChartViewer
    '
    Private Sub initChartViewer(ByVal viewer As WinChartViewer)

        ' Initially set the mouse usage to "Pointer" mode (Drag to Scroll mode)
        pointerPB.Checked = True

    End Sub

    '
    ' The ViewPortChanged event handler. This event occurs if the user scrolls or zooms in
    ' or out the chart by dragging or clicking on the chart. It can also be triggered by
    ' calling WinChartViewer.updateViewPort.
    '
    Private Sub winChartViewer1_ViewPortChanged(ByVal sender As Object, _
        ByVal e As WinViewPortEventArgs) Handles winChartViewer1.ViewPortChanged

        ' In addition to updating the chart, we may also need to update other controls that
        ' changes based on the view port.
        updateControls(winChartViewer1)

        ' Update the chart if necessary
        If e.NeedUpdateChart Then
            drawChart(winChartViewer1)
        End If

        ' Update the image map if necessary
        If e.NeedUpdateImageMap Then
            updateImageMap(winChartViewer1)
        End If

        ' We need to update the track line too. If the mouse is moving on the chart (eg. if 
        ' the user drags the mouse on the chart to scroll it), the track line will be updated
        ' in the MouseMovePlotArea event. Otherwise, we need to update the track line here.
        If (Not winChartViewer1.IsInMouseMoveEvent) And winChartViewer1.IsMouseOnPlotArea Then
            crosshair(winChartViewer1.Chart, winChartViewer1.PlotAreaMouseX, winChartViewer1.PlotAreaMouseY)
            winChartViewer1.updateDisplay()
        End If

    End Sub

    '
    ' Update controls when the view port changed
    '
    Private Sub updateControls(ByVal viewer As WinChartViewer)

        ' We need to update the navigator window size and position only if the view port changes are not
        ' caused by the navigateWindow itself.
        If Not navigateWindow.Capture Then
            ' Set the navigate window to reflect the view port 
            navigateWindow.Left = Math.Round(viewer.ViewPortLeft * navigatePad.ClientSize.Width)
            navigateWindow.Top = Math.Round(viewer.ViewPortTop * navigatePad.ClientSize.Height)
            navigateWindow.Width = Int(Math.Max(1.0, viewer.ViewPortWidth * navigatePad.ClientSize.Width))
            navigateWindow.Height = Int(Math.Max(1.0, viewer.ViewPortHeight * navigatePad.ClientSize.Height))
        End If

        ' Synchronize the zoom bar value with the view port width/height
        zoomBar.Value = Math.Round(Math.Min(viewer.ViewPortWidth, viewer.ViewPortHeight) * zoomBar.Maximum)

    End Sub

    '
    ' Draw the chart.
    '
    Private Sub drawChart(ByVal viewer As WinChartViewer)

        ' Create an XYChart object 500 x 480 pixels in size, with the same background color
        ' as the container
        Dim c As XYChart = New XYChart(500, 480, Chart.CColor(BackColor))

        ' Set the plotarea at (50, 40) and of size 400 x 400 pixels. Use light grey (c0c0c0)
        ' horizontal and vertical grid lines. Set 4 quadrant coloring, where the colors of 
        ' the quadrants alternate between lighter and deeper grey (dddddd/eeeeee). 
        c.setPlotArea(50, 40, 400, 400, -1, -1, -1, &HC0C0C0, &HC0C0C0 _
            ).set4QBgColor(&HDDDDDD, &HEEEEEE, &HDDDDDD, &HEEEEEE, &H0)

        ' Enable clipping mode to clip the part of the data that is outside the plot area.
        c.setClipping()

        ' Set 4 quadrant mode, with both x and y axes symetrical around the origin
        c.setAxisAtOrigin(Chart.XYAxisAtOrigin, Chart.XAxisSymmetric + Chart.YAxisSymmetric)

        ' Add a legend box at (450, 40) (top right corner of the chart) with vertical layout
        ' and 8 pts Arial Bold font. Set the background color to semi-transparent grey.
        Dim b As LegendBox = c.addLegend(450, 40, True, "Arial Bold", 8)
        b.setAlignment(Chart.TopRight)
        b.setBackground(&H40DDDDDD)

        ' Add a titles to axes
        c.xAxis().setTitle("Alpha Index")
        c.yAxis().setTitle("Beta Index")

        ' Set axes width to 2 pixels
        c.xAxis().setWidth(2)
        c.yAxis().setWidth(2)

        ' The default ChartDirector settings has a denser y-axis grid spacing and less-dense
        ' x-axis grid spacing. In this demo, we want the tick spacing to be symmetrical.
        ' We use around 50 pixels between major ticks and 25 pixels between minor ticks.
        c.xAxis().setTickDensity(50, 25)
        c.yAxis().setTickDensity(50, 25)

        '
        ' In this example, we represent the data by scatter points. If you want to represent
        ' the data by somethings else (lines, bars, areas, floating boxes, etc), just modify
        ' the code below to use the layer type of your choice. 
        '

        ' Add scatter layer, using 11 pixels red (ff33333) X shape symbols
        c.addScatterLayer(dataX0, dataY0, "Group A", Chart.Cross2Shape(), 11, &HFF3333)

        ' Add scatter layer, using 11 pixels green (33ff33) circle symbols
        c.addScatterLayer(dataX1, dataY1, "Group B", Chart.CircleShape, 11, &H33FF33)

        ' Add scatter layer, using 11 pixels blue (3333ff) triangle symbols
        c.addScatterLayer(dataX2, dataY2, "Group C", Chart.TriangleSymbol, 11, &H3333FF)

        '
        ' In this example, we have not explicitly configured the full x and y range. In this case, the
        ' first time syncLinearAxisWithViewPort is called, ChartDirector will auto-scale the axis and
        ' assume the resulting range is the full range. In subsequent calls, ChartDirector will set the
        ' axis range based on the view port and the full range.
        '
        viewer.SyncLinearAxisWithViewPort("x", c.xAxis())
        viewer.SyncLinearAxisWithViewPort("y", c.yAxis())

        ' Set the chart image to the WinChartViewer
        viewer.Chart = c

    End Sub

    '
    ' Update the image map
    '
    Private Sub updateImageMap(ByVal viewer As WinChartViewer)

        ' Include tool tip for the chart
        If IsNothing(viewer.ImageMap) Then
            viewer.ImageMap = viewer.Chart.getHTMLImageMap("clickable", "", _
                "title='[{dataSetName}] Alpha = {x}, Beta = {value}'")
        End If

    End Sub

    '
    ' ClickHotSpot event handler. In this demo, we just display the hot spot parameters in a pop up 
    ' dialog.
    '
    Private Sub winChartViewer1_ClickHotSpot(ByVal sender As Object, ByVal e As WinHotSpotEventArgs) _
        Handles winChartViewer1.ClickHotSpot

        ' We show the pop up dialog only when the mouse action is not in zoom-in or zoom-out mode.
        If winChartViewer1.MouseUsage <> WinChartMouseUsage.ZoomIn And _
            winChartViewer1.MouseUsage <> WinChartMouseUsage.ZoomOut Then
            Dim f As New ParamViewer()
            f.Display(sender, e)
        End If

    End Sub

    '
    ' Pointer (Drag to Scroll) button event handler
    '
    Private Sub pointerPB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles pointerPB.CheckedChanged

        If sender.Checked Then
            winChartViewer1.MouseUsage = WinChartMouseUsage.ScrollOnDrag
        End If

    End Sub

    '
    ' Zoom In button event handler
    '
    Private Sub zoomInPB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles zoomInPB.CheckedChanged

        If sender.Checked Then
            winChartViewer1.MouseUsage = WinChartMouseUsage.ZoomIn
        End If

    End Sub

    '
    ' Zoom Out button event handler
    '
    Private Sub zoomOutPB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles zoomOutPB.CheckedChanged

        If sender.Checked Then
            winChartViewer1.MouseUsage = WinChartMouseUsage.ZoomOut
        End If

    End Sub


    '
    ' ValueChanged event handler for zoomBar. Zoom in around the center point and try to 
    ' maintain the aspect ratio
    '
    Private Sub zoomBar_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles zoomBar.ValueChanged

        If Not winChartViewer1.IsInViewPortChangedEvent Then
            'Remember the center point
            Dim centerX As Double = winChartViewer1.ViewPortLeft + winChartViewer1.ViewPortWidth / 2
            Dim centerY As Double = winChartViewer1.ViewPortTop + winChartViewer1.ViewPortHeight / 2

            'Aspect ratio and zoom factor
            Dim aspectRatio As Double = winChartViewer1.ViewPortWidth / winChartViewer1.ViewPortHeight
            Dim zoomTo As Double = CDbl(zoomBar.Value) / zoomBar.Maximum

            'Zoom while respecting the aspect ratio
            winChartViewer1.ViewPortWidth = zoomTo * Math.Max(1, aspectRatio)
            winChartViewer1.ViewPortHeight = zoomTo * Math.Max(1, 1 / aspectRatio)

            'Adjust ViewPortLeft and ViewPortTop to keep center point unchanged
            winChartViewer1.ViewPortLeft = centerX - winChartViewer1.ViewPortWidth / 2
            winChartViewer1.ViewPortTop = centerY - winChartViewer1.ViewPortHeight / 2

            'update the chart, but no need to update the image map yet, as the chart is still 
            'zooming and is unstable
            winChartViewer1.updateViewPort(True, False)
        End If

    End Sub

    '
    ' The navigateWindow_MouseDown, navigateWindow_MouseMove methods implement the navigate window.
    '

    ' Internal variables to keep track of mouse positions during dragging of navigateWindow. 
    Private mouseDownXCoor As Integer
    Private mouseDownYCoor As Integer

    '
    ' MouseDown event handler for the navigateWindow
    '
    Private Sub navigateWindow_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles navigateWindow.MouseDown

        If e.Button = System.Windows.Forms.MouseButtons.Left Then
            ' Save the mouse coordinates to keep track of how far the navigateWindow has been 
            ' dragged.
            mouseDownXCoor = e.X
            mouseDownYCoor = e.Y
        End If

    End Sub

    '
    ' MouseMove event handler for the navigateWindow
    '
    Private Sub navigateWindow_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles navigateWindow.MouseMove

        If e.Button = System.Windows.Forms.MouseButtons.Left Then
            ' Is currently dragging - move the navigateWindow based on the distances dragged
            Dim newLabelLeft As Integer = Math.Max(0, navigateWindow.Left + e.X - mouseDownXCoor)
            Dim newLabelTop As Integer = Math.Max(0, navigateWindow.Top + e.Y - mouseDownYCoor)

            ' Ensure the navigateWindow is within the navigatePad container
            If newLabelLeft + navigateWindow.Width > navigatePad.ClientSize.Width Then
                newLabelLeft = navigatePad.ClientSize.Width - navigateWindow.Width
            End If
            If newLabelTop + navigateWindow.Height > navigatePad.ClientSize.Height Then
                newLabelTop = navigatePad.ClientSize.Height - navigateWindow.Height
            End If

            ' Update the navigateWindow position as it is being dragged
            navigateWindow.Left = newLabelLeft
            navigateWindow.Top = newLabelTop

            ' Update the WinChartViewer ViewPort as well
            winChartViewer1.ViewPortLeft = CDbl(navigateWindow.Left) / navigatePad.ClientSize.Width
            winChartViewer1.ViewPortTop = CDbl(navigateWindow.Top) / navigatePad.ClientSize.Height

            ' Update the chart, but no need to update the image map yet, as the chart is still 
            ' scrolling and is unstable
            winChartViewer1.updateViewPort(True, False)

        End If

    End Sub

    '
    ' We want to use the mouse wheel for zoom in/out when the mouse is over the chart. To to do, we need
    ' to set the WinChartViewer to be the "active control" when the mouse enters the chart.
    '

    ' Variable to save the original active control
    Private activeControlSave As Control = Nothing

    '
    ' Mouse Enter event handler
    '
    Private Sub winChartViewer1_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) _
        Handles winChartViewer1.MouseEnter

        ' Save the original active control and set the WinChartViewer to be the active control, so that
        ' the WinChartViewer can receive the mouse wheel event.
        activeControlSave = winChartViewer1.FindForm().ActiveControl
        winChartViewer1.FindForm().ActiveControl = winChartViewer1

    End Sub

    '
    ' Mouse Leave event handler
    '
    Private Sub winChartViewer1_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) _
        Handles winChartViewer1.MouseLeave

        ' Restore the original active control.
        winChartViewer1.FindForm().ActiveControl = activeControlSave

    End Sub

    '
    ' Mouse Wheel event handler
    '
    Private Sub winChartViewer1_MouseWheel(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.MouseEventArgs) Handles winChartViewer1.MouseWheel

        ' We zoom in or out by 10% depending on the mouse wheel direction.
        Dim rx As Double = IIf(e.Delta > 0, 0.9, 1 / 0.9)
        Dim ry As Double = rx

        ' We do not zoom in beyond the zoom in width or height limit.
        rx = Math.Max(rx, winChartViewer1.ZoomInWidthLimit / winChartViewer1.ViewPortWidth)
        ry = Math.Max(ry, winChartViewer1.ZoomInWidthLimit / winChartViewer1.ViewPortHeight)
        If rx = 1 And ry = 1 Then
            Return
        End If

        Dim c As XYChart = winChartViewer1.Chart

        '
        ' Set the view port position and size so that it is zoom in/out around the mouse by the 
        ' desired ratio.
        '

        Dim mouseOffset As Double = (winChartViewer1.ChartMouseX - c.getPlotArea().getLeftX()) / _
            c.getPlotArea().getWidth()
        winChartViewer1.ViewPortLeft += mouseOffset * (1 - rx) * winChartViewer1.ViewPortWidth
        winChartViewer1.ViewPortWidth *= rx

        Dim mouseOffsetY As Double = (winChartViewer1.ChartMouseY - c.getPlotArea().getTopY()) / _
            c.getPlotArea().getHeight()
        winChartViewer1.ViewPortTop += mouseOffsetY * (1 - ry) * winChartViewer1.ViewPortHeight
        winChartViewer1.ViewPortHeight *= ry

        ' Trigger a view port changed event to update the chart
        winChartViewer1.updateViewPort(True, False)

    End Sub

    '
    ' Draw track cursor when mouse is moving over plotarea
    '
    Private Sub winChartViewer1_MouseMovePlotArea(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.MouseEventArgs) Handles winChartViewer1.MouseMovePlotArea

        Dim viewer As WinChartViewer = sender
        crossHair(viewer.Chart, viewer.PlotAreaMouseX, viewer.PlotAreaMouseY)
        viewer.updateDisplay()

        ' Hide the track cursor when the mouse leaves the plot area
        viewer.removeDynamicLayer("MouseLeavePlotArea")

        ' Update image map if necessary
        updateImageMap(viewer)

    End Sub

    '
    ' Draw cross hair cursor with axis labels
    '
    Private Sub crossHair(ByVal c As XYChart, ByVal mouseX As Integer, ByVal mouseY As Integer)

        ' Clear the current dynamic layer and get the DrawArea object to draw on it.
        Dim d As DrawArea = c.initDynamicLayer()

        ' The plot area object
        Dim plotArea As PlotArea = c.getPlotArea()

        ' Draw a vertical line and a horizontal line as the cross hair
        d.vline(plotArea.getTopY(), plotArea.getBottomY(), mouseX, d.dashLineColor(&H0, &H101))
        d.hline(plotArea.getLeftX(), plotArea.getRightX(), mouseY, d.dashLineColor(&H0, &H101))

        ' Draw y-axis label
        Dim label As String = "<*block,bgColor=FFFFDD,margin=3,edgeColor=000000*>" & c.formatValue( _
            c.getYValue(mouseY, c.yAxis()), "{value|P4}") & "<*/*>"
        Dim t As TTFText = d.text(label, "Arial Bold", 8)
        t.draw(plotArea.getLeftX() - 5, mouseY, &H0, Chart.Right)

        ' Draw x-axis label
        label = "<*block,bgColor=FFFFDD,margin=3,edgeColor=000000*>" & c.formatValue(c.getXValue(mouseX), _
            "{value|P4}") & "<*/*>"
        t = d.text(label, "Arial Bold", 8)
        t.draw(mouseX, plotArea.getBottomY() + 5, &H0, Chart.Top)

    End Sub

End Class