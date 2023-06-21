using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: XmlnsDefinition(@"http://schemas.barf1.com/apg.shared", "APG.Shared")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation", "APG.Shared")]

// I would prefer to use a period rather than underscore, which is legal, but Visual Studio doesn't like it.
// https://www.w3.org/TR/1999/REC-xml-names-19990114/#NT-PrefixedAttName
[assembly: XmlnsPrefix(@"http://schemas.barf1.com/apg.shared", "apg_shared")] 