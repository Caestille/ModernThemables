using System;
using System.Windows.Markup;

namespace ModernThemables.Icons
{
    [MarkupExtensionReturnType(typeof(Icon))]
    public class IconExtension : BaseIconExtension
    {
        public IconExtension()
        {
        }

        public IconExtension(IconType kind)
        {
            Kind = kind;
        }

        [ConstructorArgument("kind")]
        public IconType Kind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.GetPackIcon<Icon, IconType>(Kind);
        }
    }
}