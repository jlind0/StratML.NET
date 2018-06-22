using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StratML.Core
{
    public static class CosmosExtensions
    {
        public static string BuildCollectionString<T>(this IEnumerable<T> collection, Func<T, string> selector)
        {
            return $"[{string.Join(",", collection.Select(t => $"'{selector(t)}'"))}]";
        }
    }
}
