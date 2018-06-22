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
            
            var strings = collection.Select(c => selector(c).Replace("'", "&quote;").Replace("(","").Replace(")","").Replace("\\", "").Replace("\n", " ").Replace("-", " ")).Where(
                c => c != "").Distinct().Select(c => $"'{c}'");
            return $"[{string.Join(",", strings)}]";
        }

    }
}
