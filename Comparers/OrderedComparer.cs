using System.Collections.Generic;
using System.Linq;

namespace nom_nom_nom.Comparers
{
    public class OrderedComparer : IComparer<string>
    {
        private readonly IComparer<string> _fallback;
        private readonly IReadOnlyDictionary<string, int> _orderedStrings;

        public OrderedComparer(IEnumerable<string> orderedStrings, IComparer<string> fallback = null)
        {
            _fallback = fallback;

            _orderedStrings = orderedStrings
                .Select((s, i) => new { key = s, index = i })
                .ToDictionary(a => a.key, a => a.index);
        }

        public int Compare(string x, string y)
        {
            var nx = x != null && _orderedStrings.TryGetValue(x, out var nxx) ? nxx : -1;
            var ny = y != null && _orderedStrings.TryGetValue(y, out var nyy) ? nyy : -1;
            if (nx >= 0 && ny == -1) return -1;
            if (nx == -1 && ny >= 0) return 1;
            if (nx >= 0 && ny >= 0) return nx.CompareTo(ny);
            return _fallback?.Compare(x, y) ?? string.CompareOrdinal(x, y);
        }
    }
}
