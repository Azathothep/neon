using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Implements the Update() method
    /// </summary>
    public interface IUpdateSystem : ISystem
    {
        /// <summary>
        /// Called each frame
        /// </summary>
        public void Update(TimeSpan timeSpan);
    }
}
