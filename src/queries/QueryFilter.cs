using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace neon
{
    /// <summary>
    /// Defines the term applied to the target component of the filter
    /// </summary>
    public enum FilterTerm
    {
        /// <summary>
        /// The queried entities must have the target component
        /// </summary>
        Has,

        /// <summary>
        /// The queried entities must not have the target component
        /// </summary>
        HasNot,

        /// <summary>
        /// The queried entities might optionally have the target component
        /// </summary>
        MightHave
    }

    /// <summary>
    /// A filter applied to an <c>IQuery</c>. Target component is specified as template parameter.
    /// Use this to specify if the queried entities must or must not include a specific component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct QueryFilter<T> : IQueryFilter where T : Component
    {
        private FilterTerm m_Term;
        public FilterTerm Term => m_Term;

        private ComponentID m_ComponentID;
        public ComponentID ComponentID => m_ComponentID;

        public QueryFilter(FilterTerm term)
        {
            m_Term = term;
            m_ComponentID = Components.GetID<T>();
        }

        public override bool Equals(object obj)
        {
            return (obj is QueryFilter<T> other) && this.Term == other.Term;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 131317;
                hashCode = hashCode * 4695947 ^ m_Term.GetHashCode();
                hashCode = hashCode * 4695947 ^ m_ComponentID.GetHashCode();

                return hashCode;
            }
        }
    }

    /// <summary>
    /// A filter applied to an <c>IQuery</c>. Target component is specified as <c>ComponentID</c>.
    /// Use this to specify if the queried entities must or must not include a specific component.
    /// </summary>
    public struct QueryFilter : IQueryFilter
    {
        private FilterTerm m_Term;
        public FilterTerm Term => m_Term;

        private ComponentID m_ComponentID;
        public ComponentID ComponentID => m_ComponentID;

        public QueryFilter(FilterTerm term, ComponentID componentID)
        {
            m_Term = term;
            m_ComponentID = componentID;
        }

        public override bool Equals(object obj)
        {
            return (obj is IQueryFilter other) && this.Term == other.Term;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 131317;
                hashCode = hashCode * 4695947 ^ m_Term.GetHashCode();
                hashCode = hashCode * 4695947 ^ m_ComponentID.GetHashCode();

                return hashCode;
            }
        }
    }
}
