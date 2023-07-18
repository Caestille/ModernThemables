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

        private static readonly DependencyPropertyKey IsFlippedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsFlipped), typeof(bool), typeof(BaseIcon), new PropertyMetadata(false));

        public static readonly DependencyProperty IsFlippedProperty = IsFlippedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsFlipped property for the current icon kind.
        /// </summary>
        public bool IsFlipped
        {
            get { return (bool)GetValue(IsFlippedProperty); }
            protected set { SetValue(IsFlippedPropertyKey, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.UpdateData();
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
            get { return (double)this.GetValue(RotationAngleProperty); }
            set { this.SetValue(RotationAngleProperty, value); }
        }
    }
}