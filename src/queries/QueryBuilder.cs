using System.Collections.Generic;
using System.Diagnostics;
using static neon.ComponentStorage;

namespace neon
{
    /// <summary>
    /// Specifies if the Query should be cached or not
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// Cached queries are kept in-memory for a later use.
        /// Use this if you know their is a good chance that the query will be requested again soon.
        /// </summary>
        Cached,

        /// <summary>
        /// Uncached queries a discarded after being returned.
        /// Use this if the query isn't going to be requested again anytime soon.
        /// </summary>
        Uncached
    }

    /// <summary>
    /// Provides a way to build and get Queries
    /// </summary>
    public static class QueryBuilder
    {
        private static IQueryStorage storage;

        /// <summary>
        /// The the underlying implementation of <c>IQueryStorage</c>
        /// </summary>
        public static void SetStorage(IQueryStorage storage) {
            QueryBuilder.storage = storage;
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1)> Get<T1>(Query<T1> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component
        {
            return (IEnumerable<(EntityID, T1)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1>(query, queryType);
                    return new QueryResult<T1>(iterator, mode);
                }
            );
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1, T2)> Get<T1, T2>(Query<T1, T2> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component where T2 : Component
        {
            return (IEnumerable<(EntityID, T1, T2)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2>(query, queryType);
                    return new QueryResult<T1, T2>(iterator, mode);
                }
            );
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1, T2, T3)> Get<T1, T2, T3>(Query<T1, T2, T3> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component where T2 : Component where T3 : Component
        {
            return (IEnumerable<(EntityID, T1, T2, T3)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3>(query, queryType);
                    return new QueryResult<T1, T2, T3>(iterator, mode);
                }
            );
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1, T2, T3, T4)> Get<T1, T2, T3, T4>(Query<T1, T2, T3, T4> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component where T2 : Component where T3 : Component where T4 : Component
        {
            return (IEnumerable<(EntityID, T1, T2, T3, T4)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3, T4>(query, queryType);
                    return new QueryResult<T1, T2, T3, T4>(iterator, mode);
                }
            );
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1, T2, T3, T4, T5)> Get<T1, T2, T3, T4, T5>(Query<T1, T2, T3, T4, T5> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
        {
            return (IEnumerable<(EntityID, T1, T2, T3, T4, T5)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3, T4, T5>(query, queryType);
                    return new QueryResult<T1, T2, T3, T4, T5>(iterator, mode);
                }
            );
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1, T2, T3, T4, T5, T6)> Get<T1, T2, T3, T4, T5, T6>(Query<T1, T2, T3, T4, T5, T6> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component
        {
            return (IEnumerable<(EntityID, T1, T2, T3, T4, T5, T6)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3, T4, T5, T6>(query, queryType);
                    return new QueryResult<T1, T2, T3, T4, T5, T6>(iterator, mode);
                }
            );
        }

        /// <summary>
        /// Get an Enumerator that let you iterate through the result of your Query
        /// </summary>
        public static IEnumerable<(EntityID, T1, T2, T3, T4, T5, T6, T7)> Get<T1, T2, T3, T4, T5, T6, T7>(Query<T1, T2, T3, T4, T5, T6, T7> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component where T7 : Component
        {
            return (IEnumerable<(EntityID, T1, T2, T3, T4, T5, T6, T7)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3, T4, T5, T6, T7>(query, queryType);
                    return new QueryResult<T1, T2, T3, T4, T5, T6, T7>(iterator, mode);
                }
            );
        }
    }
}
