using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Allows multiple systems of this type to be added in the SystemStorage
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AllowMultipleAttribute : Attribute { }
}
