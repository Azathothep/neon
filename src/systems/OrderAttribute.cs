using System;

namespace neon
{
    public enum OrderType {
        Before,
        After
    }

    /// <summary>
    /// Specifies when this system must be updated relative to another system
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OrderAttribute : Attribute
    {
        private OrderType m_Type;
        public OrderType Type => m_Type;

        private Type m_TargetSystem;
        public Type TargetSystem => m_TargetSystem;


        public OrderAttribute(OrderType type, Type targetSystem) {
            m_Type = type;
            m_TargetSystem = targetSystem;
        }
    }
}
