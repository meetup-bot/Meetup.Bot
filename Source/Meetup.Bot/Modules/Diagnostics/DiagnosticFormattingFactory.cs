using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meetup.Bot.Modules.Diagnostics
{
    public static class DiagnosticFormattingFactory
    {
        private static readonly Lazy<PropertyInfo[]> PropertyInfo = new Lazy<PropertyInfo[]>(() => typeof(DiagnosticData).GetProperties());

        public static DiagnosticFormatting GetFormatting(DiagnosticData data)
        {
            var propertyKvps = PropertyInfo.Value
                .Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(data)))
                .ToArray();

            return new DiagnosticFormatting(string.Join(" ", propertyKvps.Select(kvp => $"{{@{kvp.Key}}}")),
                propertyKvps.Select(kvp => kvp.Value).ToArray());
        }
    }
}