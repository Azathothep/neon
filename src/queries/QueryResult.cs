using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    /// <summary>
    /// Specifies if the iterator should be <c>Safe</c> or not
    /// </summary>
    public enum QueryResultMode
    {
        /// <summary>
        /// The QueryResult iterates through a list copy of the <c>QueryIterator</c>.
        /// Use this if you plan to add or remove components of a type specified in your Query during iteration.
        /// This will cost more memory but will avoid messing the results.
        /// </summary>
        Safe,

        /// <summary>
        /// The QueryResult iterates directly through the <c>Archetypes</c> and their columns.
        /// This will cost less memory but might mess with the results if you add or remove components of a type specified in your Query during iteration.
        /// </summary>
        Unsafe
    }

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T> : QueryResult, IEnumerable<(EntityID, T)> where T : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T)> GetEnumerator() => GetEnumerator<(EntityID, T)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T1, T2> : QueryResult, IEnumerable<(EntityID, T1, T2)> where T1 : Component where T2 : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T1, T2, T3> : QueryResult, IEnumerable<(EntityID, T1, T2, T3)> where T1 : Component where T2 : Component where T3 : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T1, T2, T3, T4> : QueryResult, IEnumerable<(EntityID, T1, T2, T3, T4)> where T1 : Component where T2 : Component where T3 : Component where T4 : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3, T4)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3, T4)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T1, T2, T3, T4, T5> : QueryResult, IEnumerable<(EntityID, T1, T2, T3, T4, T5)> where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3, T4, T5)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3, T4, T5)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T1, T2, T3, T4, T5, T6> : QueryResult, IEnumerable<(EntityID, T1, T2, T3, T4, T5, T6)> where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3, T4, T5, T6)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3, T4, T5, T6)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    /// <summary>
    /// Derives from <c>QueryIterator</c> and implements <c>IEnumerable</c> to iterate through the result of a <c>Query</c>
    /// </summary>
    public class QueryResult<T1, T2, T3, T4, T5, T6, T7> : QueryResult, IEnumerable<(EntityID, T1, T2, T3, T4, T5, T6, T7)> where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component where T7 : Component
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3, T4, T5, T6, T7)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3, T4, T5, T6, T7)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    /// <summary>
    /// Stores an <c>IQueryIterator</c> related to a <c>Query</c>, and provides ways to interact with it
    /// </summary>
    public abstract class QueryResult : IQueryResult
    {
        protected bool m_IsDirty = true;

        /// <summary>
        /// Set when the ComponentStorage has updated a component type specified in the related Query.
        /// </summary>
        public bool IsDirty => m_IsDirty;

		private IEnumerable m_Storage;

        private QueryResultMode m_Mode;

        private IComponentIterator m_IterableQuery;

        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe)
        {
			m_IterableQuery = iterableQuery;
			m_Mode = mode;
        }

        public void SetDirty() => m_IsDirty = true;

        /// <summary>
        /// Copies the iterator values in a dedicated list
        /// </summary>
		protected IEnumerable<T> MakeStorage<T>(IEnumerator<T> enumerator)
		{
			var storage = new List<T>();

			while (enumerator.MoveNext())
				storage.Add(enumerator.Current);

			return storage;
		}

        protected IEnumerator<T> GetEnumerator<T>()
        {
            if (m_Mode == QueryResultMode.Safe && !m_IsDirty)
            {
                return (IEnumerator<T>)m_Storage.GetEnumerator();
            }

            var enumerator = (IEnumerator<T>)m_IterableQuery.Create();

			if (m_Mode == QueryResultMode.Safe)
			{
                // Debug.WriteLine("Dirty: rebuilding");

				m_Storage = MakeStorage(enumerator);
                m_IsDirty = false;
				return (IEnumerator<T>)m_Storage.GetEnumerator();
			}

            return enumerator;
        }
    }
}
