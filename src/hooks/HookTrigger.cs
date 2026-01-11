using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Provide a protected way to trigger a <c>Hook</c>.
    /// Hooks cannot be triggered globally. They can only be triggered if they have a reference to a <c>HookTrigger</c>
    /// </summary>
    public class HookTrigger<HookID> where HookID : struct, IConvertible {
        private Action<HookID, object> m_Raiser;

        public HookTrigger(Action<HookID, object> raiser) => m_Raiser = raiser;

        public void Raise(HookID hook, object o) => m_Raiser(hook, o);
    }
}
