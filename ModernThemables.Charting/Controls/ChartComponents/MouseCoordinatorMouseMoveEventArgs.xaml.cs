namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public class MouseCoordinatorMouseMoveEventArgs : EventArgs
{
    public MouseCoordinatorMouseMoveEventArgs(bool isUserDragging, bool isUserPanning, Point? lowerSelection, Point lastMousePoint, MouseEventArgs args)
    {
        this.IsUserDragging = isUserDragging;
        this.IsUserPanning = isUserPanning;
        this.LowerSelection = lowerSelection;
        this.LastMousePoint = lastMousePoint;
        this.Args = args;
    }

    public bool IsUserDragging { get; }

    public bool IsUserPanning { get; }

    public Point? LowerSelection { get; }

    public Point LastMousePoint { get; }

    public MouseEventArgs Args { get; }
}
