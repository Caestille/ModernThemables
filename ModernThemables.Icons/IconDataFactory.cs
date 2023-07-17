using System;
using System.Collections.Generic;

namespace ModernThemables.Icons
{
    public static class IconDataFactory
    {
        public static Lazy<IDictionary<IconEnum, string>> DataIndex { get; }

        static IconDataFactory()
        {
            if (DataIndex == null)
            {
                DataIndex = new Lazy<IDictionary<IconEnum, string>>(Create);
            }
        }

        public static IDictionary<IconEnum, string> Create()
        {
            return new Dictionary<IconEnum, string>
            {
                { IconEnum.None, "" },
                { IconEnum.Bin, "" },
                { IconEnum.Building, "" },
                { IconEnum.Calculator, "" },
                { IconEnum.Calendar, "" },
                { IconEnum.Close, "" },
                { IconEnum.Collapse, "" },
                { IconEnum.Download, "" },
                { IconEnum.Expand, "" },
                { IconEnum.Gear, "" },
                { IconEnum.Graph, "" },
                { IconEnum.List, "" },
                { IconEnum.MagnifyingGlass, "" },
                { IconEnum.Maximise, "" },
                { IconEnum.Menu, "" },
                { IconEnum.Minimise, "" },
                { IconEnum.Monitor, "" },
                { IconEnum.Moon, "" },
                { IconEnum.OpenFile, "" },
                { IconEnum.Pen, "" },
                { IconEnum.Pin, "" },
                { IconEnum.Play, "" },
                { IconEnum.Plus, "" },
                { IconEnum.Restore, "" },
                { IconEnum.Save, "" },
                { IconEnum.Sun, "" },
                { IconEnum.Tag, "" },
                { IconEnum.Tick, "" },
            };
        }
    }
}