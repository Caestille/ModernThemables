using CoreUtilities.HelperClasses;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ModernThemables
{
    public class BindingTrigger : INotifyPropertyChanged
    {
        public BindingTrigger()
        {
            Binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(Value))
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Binding Binding { get; }

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public object Value { get; }
    }

    public class InlineMultiConverter : IMultiValueConverter
    {
        public delegate object ConvertDelegate(object[] values, Type targetType, object parameter, CultureInfo culture);
        public delegate object[] ConvertBackDelegate(object value, Type[] targetTypes, object parameter, CultureInfo culture);

        public InlineMultiConverter(ConvertDelegate convert, ConvertBackDelegate convertBack = null)
        {
            this.convert = convert ?? throw new ArgumentNullException(nameof(convert));
            this.convertBack = convertBack;
        }

        private ConvertDelegate convert { get; }

        private ConvertBackDelegate convertBack { get; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return convert(values, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (convertBack != null)
                ? convertBack(value, targetTypes, parameter, culture)
                : throw new NotImplementedException();
        }
    }

    public class DynamicResourceBindingExtension : MarkupExtension
    {
        private BindingProxy bindingProxy;
        private BindingTrigger bindingTrigger;

        public DynamicResourceBindingExtension(object resourceKey)
        {
            ResourceKey = resourceKey ?? throw new ArgumentNullException(nameof(resourceKey));
        }

        public object ResourceKey { get; set; }

        public IValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        public CultureInfo ConverterCulture { get; set; }

        public string StringFormat { get; set; }

        public object TargetNullValue { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var dynamicResource = new DynamicResourceExtension(ResourceKey);
            bindingProxy = new BindingProxy(dynamicResource.ProvideValue(null)); 

            var dynamicResourceBinding = new Binding()
            {
                Source = bindingProxy,
                Path = new PropertyPath(BindingProxy.DataProperty),
                Mode = BindingMode.OneWay
            };

            var targetInfo = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            if (targetInfo.TargetObject is DependencyObject dependencyObject)
            {
                dynamicResourceBinding.Converter = Converter;
                dynamicResourceBinding.ConverterParameter = ConverterParameter;
                dynamicResourceBinding.ConverterCulture = ConverterCulture;
                dynamicResourceBinding.StringFormat = StringFormat;
                dynamicResourceBinding.TargetNullValue = TargetNullValue;

                if (dependencyObject is FrameworkElement targetFrameworkElement)
                    targetFrameworkElement.Resources[bindingProxy] = bindingProxy;

                return dynamicResourceBinding.ProvideValue(serviceProvider);
            }

            var findTargetBinding = new Binding()
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self)
            };

            bindingTrigger = new BindingTrigger();

            var wrapperBinding = new MultiBinding()
            {
                Bindings = 
                {
                    dynamicResourceBinding,
                    findTargetBinding,
                    bindingTrigger.Binding,
                },
                Converter = new InlineMultiConverter(WrapperConvert)
            };

            return wrapperBinding.ProvideValue(serviceProvider);
        }

        private object WrapperConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dynamicResourceBindingResult = values[0];
            var bindingTargetObject = values[1];

            if (Converter != null)
                dynamicResourceBindingResult = Converter.Convert(dynamicResourceBindingResult, targetType, ConverterParameter, ConverterCulture);

            if (dynamicResourceBindingResult == null)
                dynamicResourceBindingResult = TargetNullValue;

            else if (targetType == typeof(string) && StringFormat != null)
                dynamicResourceBindingResult = String.Format(StringFormat, dynamicResourceBindingResult);

            if (bindingTargetObject is FrameworkElement targetFrameworkElement && !targetFrameworkElement.Resources.Contains(bindingProxy))
            {
                targetFrameworkElement.Resources[bindingProxy] = bindingProxy;
                SynchronizationContext.Current?.Post((state) => bindingTrigger.Refresh(), null);
            }

            return dynamicResourceBindingResult;
        }
    }
}
