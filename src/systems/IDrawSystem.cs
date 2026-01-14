using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Implements the Draw() method
    /// </summary>
    public interface IDrawSystem : ISystem
    {
        /// <summary>
        /// Called before the screen renders
        /// </summary>
        public void Draw();
    }
}
