using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Represent a component filter for an <c>IQuery</c>, describing the component targetted and the <c>FilterTerm</c> applied to it
    /// </summary>
    public interface IQueryFilter
    {
        public FilterTerm Term { get; }

        public ComponentID ComponentID { get; }
    }
}
