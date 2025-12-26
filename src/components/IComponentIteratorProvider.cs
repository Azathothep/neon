using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IComponentIteratorProvider
    {
        public IComponentIterator Get<T>(IQuery query, QueryType queryType) where T : Component;
        public IComponentIterator Get<T1, T2>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component;
        public IComponentIterator Get<T1, T2, T3>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component;
        public IComponentIterator Get<T1, T2, T3, T4>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component;
        public IComponentIterator Get<T1, T2, T3, T4, T5>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component;
        public IComponentIterator Get<T1, T2, T3, T4, T5, T6>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component;
        public IComponentIterator Get<T1, T2, T3, T4, T5, T6, T7>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component where T7 : Component;

    }
}
