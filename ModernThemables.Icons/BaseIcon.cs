using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Icons
{
    /// <summary>
    /// Class PackIconControlBase which is the base class for any PackIcon control.
    /// </summary>
    public abstract class BaseIcon : Control
    {
        static BaseIcon() { }

        private static readonly DependencyPropertyKey DataPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(BaseIcon), new PropertyMetadata(""));

        public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the path data for the current icon kind.
        /// </summary>
        [TypeConverter(typeof(GeometryConverter))]
        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            protected set { SetValue(DataPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey YScalePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(YScale), typeof(int), typeof(BaseIcon), new PropertyMetadata(1));

        public static readonly DependencyProperty YScaleProperty = YScalePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the YScale property for the current icon kind.
        /// </summary>
        public int YScale
        {
            get { return (int)GetValue(YScaleProperty); }
            protected set { SetValue(YScalePropertyKey, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateData();
        }

        internal abstract void UpdateData();

        internal abstract void SetKind<TKind>(TKind iconKind);

        /// <summary>
        /// Identifies the RotationAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty
            = DependencyProperty.Register(
                nameof(RotationAngle),
                typeof(double),
                typeof(BaseIcon),
                new PropertyMetadata(0d, null, (dependencyObject, value) =>
                {
                    var val = (double)value;
                    return val < 0 ? 0d : (val > 360 ? 360d : value);
                }));

        /// <summary>
        /// Gets or sets the rotation (angle).
        /// </summary>
        /// <value>The rotation.</value>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
    }
}