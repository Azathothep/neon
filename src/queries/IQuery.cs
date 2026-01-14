using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Represents a component query, describing the requested components and the <c>IQueryFilter</c> applied
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Must the query include inactive components ?
        /// </summary>
        public bool IncludeInactive { get; }

        public IQueryFilter[] Filters { get; }

        public ComponentID[] ReturnValues { get; }
    }
}
