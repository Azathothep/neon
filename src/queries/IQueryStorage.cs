using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Stores and caches <c>IQuerieResults</c> for later reuse
    /// </summary>
    public interface IQueryStorage
    {
        public IQueryResult Get(IQuery query, QueryType queryType, Func<IComponentIteratorProvider, IQueryResult> queryResultCreator);
    }
}
