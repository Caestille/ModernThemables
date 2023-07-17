using System;
using System.Windows.Markup;

namespace ModernThemables.Icons.Icons
{
    [MarkupExtensionReturnType(typeof(Icon))]
    public class IconExtension : BaseIconExtension
    {
        public IconExtension()
        {
        }

        public IconExtension(IconEnum kind)
        {
            Kind = kind;
        }

        [ConstructorArgument("kind")]
        public IconEnum Kind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.GetPackIcon<Icon, IconEnum>(Kind);
        }
    }
}