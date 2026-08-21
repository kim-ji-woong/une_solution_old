using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ChartDirector;

namespace CSharpChartExplorer
{
    public partial class FrmXYZoomScroll : Form
    {
        // XY data points for the chart
        private double[] dataX0;
        private double[] dataY0;
        private double[] dataX1;
        private double[] dataY1;
        private double[] dataX2;
        private double[] dataY2;

        // Flag to indicated if initialization has been completed. Prevents events from firing before 
        // controls are properly initialized.
        private bool hasFinishedInitialization;

        public FrmXYZoomScroll()
        {
            InitializeComponent();
        }

        private void FrmXYZoomScroll_Load(object sender, EventArgs e)
        {
            // Load the data
            loadData();

            // Initialize the WinChartViewer
            initChartViewer(winChartViewer1);

            // Some Visual Studio versions do not expose the MouseWheel event in the Property window, so we
            // need to set up the event handler with our own code.
            winChartViewer1.MouseWheel += new MouseEventHandler(winChartViewer1_MouseWheel);

            // Can handle events now
            hasFinishedInitialization = true;

            // Trigger the ViewPortChanged event to draw the chart
            winChartViewer1.updateViewPort(true, true);
        }

        //
        // Load the data
        //
        private void loadData()
        {
            //
            // For simplicity, in this demo, we just use hard coded data.
            //
            dataX0 = new double[] { 10, 15, 6, -12, 14, -8, 13, -3, 16, 12, 10.5, -7, 3, -10, -5, 2, 5 };
            dataY0 = new double[] {130, 150, 80, 110, -110, -105, -130, -15, -170, 125,  125, 60, 25, 150,
                150, 15, 120};
            dataX1 = new double[] { 6, 7, -4, 3.5, 7, 8, -9, -10, -12, 11, 8, -3, -2, 8, 4, -15, 15 };
            dataY1 = new double[] {65, -40, -40, 45, -70, -80, 80, 10, -100, 105, 60, 50, 20, 170, -25, 50,
                75};
            dataX2 = new double[] { -10, -12, 11, 8, 6, 12, -4, 3.5, 7, 8, -9, 3, -13, 16, -7.5, -10, -15 };
            dataY2 = new double[] {65, -80, -40, 45, -70, -80, 80, 90, -100, 105, 60, -75, -150, -40, 120,
                -50, -30};
        }

        //
        // Initialize the WinChartViewer
        //
        private void initChartViewer(WinChartViewer viewer)
        {
            // Initially set the mouse usage to "Pointer" mode (Drag to Scroll mode)
            pointerPB.Checked = true;
        }

        //
        // The ViewPortChanged event handler. This event occurs if the user scrolls or zooms in
        // or out the chart by dragging or clicking on the chart. It can also be triggered by
        // calling WinChartViewer.updateViewPort.
        //
        private void winChartViewer1_ViewPortChanged(object sender, WinViewPortEventArgs e)
        {
            // In addition to updating the chart, we may also need to update other controls that
            // changes based on the view port.
            updateControls(winChartViewer1);

            // Update the chart if necessary
            if (e.NeedUpdateChart)
                drawChart(winChartViewer1);

            // Update the image map if necessary
            if (e.NeedUpdateImageMap)
                updateImageMap(winChartViewer1);

            // We need to update the track line too. If the mouse is moving on the chart (eg. if 
            // the user drags the mouse on the chart to scroll it), the track line will be updated
            // in the MouseMovePlotArea event. Otherwise, we need to update the track line here.
            if ((!winChartViewer1.IsInMouseMoveEvent) && winChartViewer1.IsMouseOnPlotArea)
            {
                crossHair((XYChart)winChartViewer1.Chart, winChartViewer1.PlotAreaMouseX, 
                    winChartViewer1.PlotAreaMouseY);
                winChartViewer1.updateDisplay();
            }
        }

        //
        // Update controls when the view port changed
        //
        private void updateControls(WinChartViewer viewer)
        {
            // We need to update the navigator window size and position only if the view port changes are not
            // caused by the navigateWindow itself.
            if (!navigateWindow.Capture)
            {
                // Set the navigate window to reflect the view port 
                navigateWindow.Left = (int)Math.Round(viewer.ViewPortLeft * navigatePad.ClientSize.Width);
                navigateWindow.Top = (int)Math.Round(viewer.ViewPortTop * navigatePad.ClientSize.Height);
                navigateWindow.Width = (int)Math.Max(1.0, viewer.ViewPortWidth * 
                    navigatePad.ClientSize.Width);
                navigateWindow.Height = (int)Math.Max(1.0, viewer.ViewPortHeight * 
                    navigatePad.ClientSize.Height);
            }

            // Synchronize the zoom bar value with the view port width/height
            zoomBar.Value = (int)Math.Round(Math.Min(viewer.ViewPortWidth, viewer.ViewPortHeight) * 
                zoomBar.Maximum); 
        }

        //
        // Draw the chart.
        //
        private void drawChart(WinChartViewer viewer)
        {
            // Create an XYChart object 500 x 480 pixels in size, with the same background color
            // as the container
            XYChart c = new XYChart(500, 480, Chart.CColor(BackColor));

            // Set the plotarea at (50, 40) and of size 400 x 400 pixels. Use light grey (c0c0c0)
            // horizontal and vertical grid lines. Set 4 quadrant coloring, where the colors of 
            // the quadrants alternate between lighter and deeper grey (dddddd/eeeeee). 
            c.setPlotArea(50, 40, 400, 400, -1, -1, -1, 0xc0c0c0, 0xc0c0c0
                ).set4QBgColor(0xdddddd, 0xeeeeee, 0xdddddd, 0xeeeeee, 0x000000);

            // Enable clipping mode to clip the part of the data that is outside the plot area.
            c.setClipping();

            // Set 4 quadrant mode, with both x and y axes symetrical around the origin
            c.setAxisAtOrigin(Chart.XYAxisAtOrigin, Chart.XAxisSymmetric + Chart.YAxisSymmetric);

            // Add a legend box at (450, 40) (top right corner of the chart) with vertical layout
            // and 8 pts Arial Bold font. Set the background color to semi-transparent grey.
            LegendBox b = c.addLegend(450, 40, true, "Arial Bold", 8);
            b.setAlignment(Chart.TopRight);
            b.setBackground(0x40dddddd);

            // Add a titles to axes
            c.xAxis().setTitle("Alpha Index");
            c.yAxis().setTitle("Beta Index");

            // Set axes width to 2 pixels
            c.xAxis().setWidth(2);
            c.yAxis().setWidth(2);

            // The default ChartDirector settings has a denser y-axis grid spacing and less-dense
            // x-axis grid spacing. In this demo, we want the tick spacing to be symmetrical.
            // We use around 50 pixels between major ticks and 25 pixels between minor ticks.
            c.xAxis().setTickDensity(50, 25);
            c.yAxis().setTickDensity(50, 25);

            //
            // In this example, we represent the data by scatter points. If you want to represent
            // the data by somethings else (lines, bars, areas, floating boxes, etc), just modify
            // the code below to use the layer type of your choice. 
            //

            // Add scatter layer, using 11 pixels red (ff33333) X shape symbols
            c.addScatterLayer(dataX0, dataY0, "Group A", Chart.Cross2Shape(), 11, 0xff3333);

            // Add scatter layer, using 11 pixels green (33ff33) circle symbols
            c.addScatterLayer(dataX1, dataY1, "Group B", Chart.CircleShape, 11, 0x33ff33);

            // Add scatter layer, using 11 pixels blue (3333ff) triangle symbols
            c.addScatterLayer(dataX2, dataY2, "Group C", Chart.TriangleSymbol, 11, 0x3333ff);

            //
            // In this example, we have not explicitly configured the full x and y range. In this case, the
            // first time syncLinearAxisWithViewPort is called, ChartDirector will auto-scale the axis and
            // assume the resulting range is the full range. In subsequent calls, ChartDirector will set the
            // axis range based on the view port and the full range.
            //
            viewer.syncLinearAxisWithViewPort("x", c.xAxis());
            viewer.syncLinearAxisWithViewPort("y", c.yAxis());

            // Set the chart image to the WinChartViewer
            viewer.Chart = c;
        }

        //
        // Update the image map
        //
        private void updateImageMap(WinChartViewer viewer)
        {
            // Include tool tip for the chart
            if (viewer.ImageMap == null)
            {
                viewer.ImageMap = viewer.Chart.getHTMLImageMap("clickable", "",
                    "title='[{dataSetName}] Alpha = {x}, Beta = {value}'");
            }
        }

        //
        // ClickHotSpot event handler. In this demo, we just display the hot spot parameters in a pop up 
        // dialog.
        //
        private void winChartViewer1_ClickHotSpot(object sender, WinHotSpotEventArgs e)
        {
            // We show the pop up dialog only when the mouse action is not in zoom-in or zoom-out mode.
            if ((winChartViewer1.MouseUsage != WinChartMouseUsage.ZoomIn)
                && (winChartViewer1.MouseUsage != WinChartMouseUsage.ZoomOut))
                new ParamViewer().Display(sender, e);
        }

        //
        // Pointer (Drag to Scroll) button event handler
        //
        private void pointerPB_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
                winChartViewer1.MouseUsage = WinChartMouseUsage.ScrollOnDrag;
        }

        //
        // Zoom In button event handler
        //
        private void zoomInPB_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
                winChartViewer1.MouseUsage = WinChartMouseUsage.ZoomIn;
        }

        //
        // Zoom Out button event handler
        //
        private void zoomOutPB_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
                winChartViewer1.MouseUsage = WinChartMouseUsage.ZoomOut;
        }

        //
        // ValueChanged event handler for zoomBar. Zoom in around the center point and try to 
        // maintain the aspect ratio
        //
        private void zoomBar_ValueChanged(object sender, EventArgs e)
        {
            if (hasFinishedInitialization && !winChartViewer1.IsInViewPortChangedEvent)
            {
                //Remember the center point
                double centerX = winChartViewer1.ViewPortLeft + winChartViewer1.ViewPortWidth / 2;
                double centerY = winChartViewer1.ViewPortTop + winChartViewer1.ViewPortHeight / 2;

                //Aspect ratio and zoom factor
                double aspectRatio = winChartViewer1.ViewPortWidth / winChartViewer1.ViewPortHeight;
                double zoomTo = ((double)zoomBar.Value) / zoomBar.Maximum;

                //Zoom while respecting the aspect ratio
                winChartViewer1.ViewPortWidth = zoomTo * Math.Max(1, aspectRatio);
                winChartViewer1.ViewPortHeight = zoomTo * Math.Max(1, 1 / aspectRatio);

                //Adjust ViewPortLeft and ViewPortTop to keep center point unchanged
                winChartViewer1.ViewPortLeft = centerX - winChartViewer1.ViewPortWidth / 2;
                winChartViewer1.ViewPortTop = centerY - winChartViewer1.ViewPortHeight / 2;

                //update the chart, but no need to update the image map yet, as the chart is still
                //zooming and is unstable
                winChartViewer1.updateViewPort(true, false);
            }
        }		

        //
        // The navigateWindow_MouseDown, navigateWindow_MouseMove methods implement the navigate window.
        //

        // Store the mouse position when then mouse button is pressed (start dragging)
        private int mouseDownXCoor;
        private int mouseDownYCoor;

        //
        // MouseDown event handler for the navigateWindow
        //
        private void navigateWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Save the mouse position to keep track of how far the navigateWindow has been dragged.
                mouseDownXCoor = e.X;
                mouseDownYCoor = e.Y;
            }
        }

        //
        // MouseMove event handler for the navigateWindow
        //
        private void navigateWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Is currently dragging - move the navigateWindow based on the distances dragged
                int newLabelLeft = Math.Max(0, navigateWindow.Left + e.X - mouseDownXCoor);
                int newLabelTop = Math.Max(0, navigateWindow.Top + e.Y - mouseDownYCoor);

                // Ensure the navigateWindow is within the navigatePad container
                if (newLabelLeft + navigateWindow.Width > navigatePad.ClientSize.Width)
                    newLabelLeft = navigatePad.ClientSize.Width - navigateWindow.Width;
                if (newLabelTop + navigateWindow.Height > navigatePad.ClientSize.Height)
                    newLabelTop = navigatePad.ClientSize.Height - navigateWindow.Height;

                // Update the navigateWindow position as it is being dragged
                navigateWindow.Left = newLabelLeft;
                navigateWindow.Top = newLabelTop;

                // Update the WinChartViewer ViewPort as well
                winChartViewer1.ViewPortLeft = ((double)navigateWindow.Left) / navigatePad.ClientSize.Width;
                winChartViewer1.ViewPortTop = ((double)navigateWindow.Top) / navigatePad.ClientSize.Height;

                // Update the chart, but no need to update the image map yet, as the chart is still 
                // scrolling and is unstable
                winChartViewer1.updateViewPort(true, false);
            }
        }

        //
        // We want to use the mouse wheel for zoom in/out when the mouse is over the chart. To to do, we need
        // to set the WinChartViewer to be the "active control" when the mouse enters the chart.
        //

        // Variable to save the original active control
        private Control activeControlSave = null;

        //
        // Mouse Enter event handler
        //
        private void winChartViewer1_MouseEnter(object sender, System.EventArgs e)
        {
            // Save the original active control and set the WinChartViewer to be the active control, so that
            // the WinChartViewer can receive the mouse wheel event.
            activeControlSave = winChartViewer1.FindForm().ActiveControl;
            winChartViewer1.FindForm().ActiveControl = winChartViewer1;
        }

        //
        // Mouse Leave event handler
        //
        private void winChartViewer1_MouseLeave(object sender, System.EventArgs e)
        {
            // Restore the original active control.
            winChartViewer1.FindForm().ActiveControl = activeControlSave;
        }

        //
        // Mouse Wheel event handler
        //
        private void winChartViewer1_MouseWheel(object sender, MouseEventArgs e)
        {
            // We zoom in or out by 10% depending on the mouse wheel direction.
            double rx = e.Delta > 0 ? 0.9 : 1 / 0.9;
            double ry = rx;

            // We do not zoom in beyond the zoom in width or height limit.
            rx = Math.Max(rx, winChartViewer1.ZoomInWidthLimit / winChartViewer1.ViewPortWidth);
            ry = Math.Max(ry, winChartViewer1.ZoomInWidthLimit / winChartViewer1.ViewPortHeight);
            if ((rx == 1) && (ry == 1))
                return;

            XYChart c = (XYChart)winChartViewer1.Chart;

            //
            // Set the view port position and size so that it is zoom in/out around the mouse by the 
            // desired ratio.
            //

            double mouseOffset = (winChartViewer1.ChartMouseX - c.getPlotArea().getLeftX()) / 
                (double)c.getPlotArea().getWidth();
            winChartViewer1.ViewPortLeft += mouseOffset * (1 - rx) * winChartViewer1.ViewPortWidth;
            winChartViewer1.ViewPortWidth *= rx;

            double mouseOffsetY = (winChartViewer1.ChartMouseY - c.getPlotArea().getTopY()) / 
                (double)c.getPlotArea().getHeight();
            winChartViewer1.ViewPortTop += mouseOffsetY * (1 - ry) * winChartViewer1.ViewPortHeight;
            winChartViewer1.ViewPortHeight *= ry;

            // Trigger a view port changed event to update the chart
            winChartViewer1.updateViewPort(true, false);
        }

        //
        // Draw track cursor when mouse is moving over plotarea, and update image map if necessary
        //
        private void winChartViewer1_MouseMovePlotArea(object sender, MouseEventArgs e)
        {
            WinChartViewer viewer = (WinChartViewer)sender;

            // Draw crosshair track cursor
            crossHair((XYChart)viewer.Chart, viewer.PlotAreaMouseX, viewer.PlotAreaMouseY);
            viewer.updateDisplay();

            // Hide the track cursor when the mouse leaves the plot area
            viewer.removeDynamicLayer("MouseLeavePlotArea");

            // Update image map if necessary
            updateImageMap(viewer);
        }

        //
        // Draw cross hair cursor with axis labels
        //
        private void crossHair(XYChart c, int mouseX, int mouseY)
        {
            // Clear the current dynamic layer and get the DrawArea object to draw on it.
            DrawArea d = c.initDynamicLayer();

            // The plot area object
            PlotArea plotArea = c.getPlotArea();

            // Draw a vertical line and a horizontal line as the cross hair
            d.vline(plotArea.getTopY(), plotArea.getBottomY(), mouseX, d.dashLineColor(0x000000, 0x0101));
            d.hline(plotArea.getLeftX(), plotArea.getRightX(), mouseY, d.dashLineColor(0x000000, 0x0101));

            // Draw y-axis label
            string label = "<*block,bgColor=FFFFDD,margin=3,edgeColor=000000*>" + c.formatValue(c.getYValue(
                mouseY, c.yAxis()), "{value|P4}") + "<*/*>";
            TTFText t = d.text(label, "Arial Bold", 8);
            t.draw(plotArea.getLeftX() - 5, mouseY, 0x000000, Chart.Right);

            // Draw x-axis label
            label = "<*block,bgColor=FFFFDD,margin=3,edgeColor=000000*>" + c.formatValue(c.getXValue(mouseX),
                "{value|P4}") + "<*/*>";
            t = d.text(label, "Arial Bold", 8);
            t.draw(mouseX, plotArea.getBottomY() + 5, 0x000000, Chart.Top);
        }
    }
}