using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace APG.Shared
{
    public class AwesomePanel : Panel
    {
        #region Fill

        public enum Fill
        {
            /// <summary>
            /// The UIElement consumes it's measured size.
            /// </summary>
            Auto,

            /// <summary>
            /// The UIElement shares a proportion of the total available space.
            /// </summary>
            Fill,

            /// <summary>
            /// The UIElement does not take part in the layout routine.
            /// </summary>
            Ignored,
        }

        /// <summary>
        /// Determines how the child element is measured.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached
        (
            "Fill",
            typeof(Fill),
            typeof(AwesomePanel),
            new FrameworkPropertyMetadata(
                Fill.Auto,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure
            )
        );

        public static Fill GetFill(DependencyObject element)
        {
            return (Fill)element.GetValue(FillProperty);
        }

        public static void SetFill(DependencyObject element, Fill value)
        {
            element.SetValue(FillProperty, value);
        }

        #endregion

        #region MarginToNext

        public static readonly DependencyProperty MarginToNextProperty = DependencyProperty.RegisterAttached
        (
            "MarginToNext",
            typeof(double?),
            typeof(AwesomePanel),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure
            )
        );

        public static double? GetMarginToNext(DependencyObject element)
        {
            return (double?)element.GetValue(MarginToNextProperty);
        }

        public static void SetMarginToNext(DependencyObject element, double? value)
        {
            element.SetValue(MarginToNextProperty, value);
        }

        #endregion

        #region MarginBetweenChildren

        public static readonly DependencyProperty MarginBetweenChildrenProperty = DependencyProperty.Register
        (
            "MarginBetweenChildren",
            typeof(double),
            typeof(AwesomePanel),
            new FrameworkPropertyMetadata
            (
                0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure
            )
        );

        public double MarginBetweenChildren
        {
            get { return (double)GetValue(MarginBetweenChildrenProperty); }
            set { SetValue(MarginBetweenChildrenProperty, value); }
        }

        #endregion

        #region Orientation

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register
        (
            "Orientation",
            typeof(Orientation),
            typeof(AwesomePanel),
            new FrameworkPropertyMetadata
            (
                Orientation.Vertical,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure
            )
        );

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Padding

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register
        (
            "Padding",
            typeof(Thickness),
            typeof(AwesomePanel),
            new FrameworkPropertyMetadata
            (
                new Thickness(),
                FrameworkPropertyMetadataOptions.AffectsParentMeasure
            )
        );

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        #endregion

        #region Size

        /// <summary>
        /// The relative proportion of the available space consumed by this child compared to other children with Fill = Fill.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached
        (
            "Size",
            typeof(double),
            typeof(AwesomePanel),
            new FrameworkPropertyMetadata(
                1.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                null,
                new CoerceValueCallback(CoerceSize)
            )
        );

        public static object CoerceSize(DependencyObject _, object value)
        {
            var size = (double)value;

            return Math.Max(0, size);
        }

        public static double GetSize(DependencyObject element)
        {
            return (double)element.GetValue(SizeProperty);
        }

        public static void SetSize(DependencyObject element, double value)
        {
            element.SetValue(SizeProperty, value);
        }

        #endregion

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var children = InternalChildren;
            var totalChildrenCount = children.Count;

            var accumulatedLeft = Padding.Left;
            var accumulatedTop = Padding.Top;

            var isHorizontal = Orientation == Orientation.Horizontal;
            var marginBetweenChildren = MarginBetweenChildren;

            var totalMarginToAdd = CalculateTotalMarginToAdd(children, marginBetweenChildren);

            var allAutoSizedSum = 0.0;
            var sumOfFillTypeSizes = 0.0;
            foreach (var child in children.OfType<UIElement>())
            {
                var fillType = GetFill(child);
                if (fillType != Fill.Auto)
                {
                    if (child.Visibility != Visibility.Collapsed && fillType != Fill.Ignored) sumOfFillTypeSizes += GetSize(child);
                }
                else
                {
                    var desiredSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                    allAutoSizedSum += desiredSize;
                }
            }

            var remainingForFillTypes = isHorizontal
                ? Math.Max(0, arrangeSize.Width - allAutoSizedSum - totalMarginToAdd - Padding.Left - Padding.Right)
                : Math.Max(0, arrangeSize.Height - allAutoSizedSum - totalMarginToAdd - Padding.Top - Padding.Bottom);
            var fillTypeSize = remainingForFillTypes / sumOfFillTypeSizes;

            for (var i = 0; i < totalChildrenCount; ++i)
            {
                var child = children[i];
                if (child == null) { continue; }
                var childDesiredSize = child.DesiredSize;
                var fillType = GetFill(child);
                var isCollapsed = child.Visibility == Visibility.Collapsed || fillType == Fill.Ignored;
                var isLastChild = i == totalChildrenCount - 1;
                var marginToAdd = isLastChild || isCollapsed ? 0 : GetMarginToNext(child) ?? marginBetweenChildren;

                var rcChild = new Rect(
                    accumulatedLeft,
                    accumulatedTop,
                    Math.Max(0.0, arrangeSize.Width - accumulatedLeft),
                    Math.Max(0.0, arrangeSize.Height - accumulatedTop));

                var childSize = fillTypeSize * GetSize(child);

                if (isHorizontal)
                {
                    rcChild.Width = fillType == Fill.Auto || isCollapsed ? childDesiredSize.Width : childSize;
                    rcChild.Height = arrangeSize.Height - Padding.Top - Padding.Bottom;
                    accumulatedLeft += rcChild.Width + marginToAdd;
                }
                else
                {
                    rcChild.Width = arrangeSize.Width - Padding.Left - Padding.Right;
                    rcChild.Height = fillType == Fill.Auto || isCollapsed ? childDesiredSize.Height : childSize;
                    accumulatedTop += rcChild.Height + marginToAdd;
                }

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var children = InternalChildren;

            var parentWidth = 0.0;
            var parentHeight = 0.0;
            var accumulatedWidth = Padding.Left + Padding.Right;
            var accumulatedHeight = Padding.Top + Padding.Bottom;

            var isHorizontal = Orientation == Orientation.Horizontal;
            var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child == null) { continue; }

                // Handle only the Auto's first to calculate remaining space for Fill's
                if (GetFill(child) != Fill.Auto) continue;

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                var childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                                               Math.Max(0.0, constraint.Height - accumulatedHeight));

                // Measure child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (isHorizontal)
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                else
                {
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            // Add all margin to accumulated size before calculating remaining space for
            // Fill elements.
            if (isHorizontal)
            {
                accumulatedWidth += totalMarginToAdd;
            }
            else
            {
                accumulatedHeight += totalMarginToAdd;
            }

            var sumOfFillSizes = children
                .OfType<UIElement>()
                .Where(x => GetFill(x) == Fill.Fill && x.Visibility != Visibility.Collapsed)
                .Sum(x => GetSize(x));

            var availableSpaceRemaining = isHorizontal
                ? Math.Max(0, constraint.Width - accumulatedWidth)
                : Math.Max(0, constraint.Height - accumulatedHeight);

            var eachFillTypeSize = sumOfFillSizes > 0
                ? availableSpaceRemaining / sumOfFillSizes
                : 0;

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child == null) { continue; }

                // Handle all the Fill's giving them a portion of the remaining space
                if (GetFill(child) != Fill.Fill) { continue; }

                var childSize = eachFillTypeSize * GetSize(child);

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                var childConstraint = isHorizontal
                    ? new Size(childSize,
                               Math.Max(0.0, constraint.Height - accumulatedHeight))
                    : new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                               childSize);

                // Measure child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (isHorizontal)
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                else
                {
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            // Make sure the final accumulated size is reflected in parentSize.
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);
            var parent = new Size(parentWidth, parentHeight);

            return parent;
        }

        private static double CalculateTotalMarginToAdd(IEnumerable children, double marginBetweenChildren)
        {
            var totalMarginToAdd = children
                .OfType<UIElement>()
                .Where(x => x.Visibility != Visibility.Collapsed && GetFill(x) != Fill.Ignored)
                .SkipLastForFramework(1)
                .Sum(x => GetMarginToNext(x) ?? marginBetweenChildren);

            return totalMarginToAdd;
        }
    }
}
