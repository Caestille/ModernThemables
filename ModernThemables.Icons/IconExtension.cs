namespace ModernThemables.Icons
{
    using System;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(Icon))]
    public class IconExtension : BaseIconExtension
    {
        public IconExtension()
        {
        }

        public IconExtension(IconType kind)
        {
            this.Kind = kind;
        }

        [ConstructorArgument("kind")]
        public IconType Kind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.GetPackIcon<Icon, IconType>(this.Kind);
        }
    }
}