using System.ComponentModel;

namespace ChattyVibes.Events
{
    internal abstract class BaseEvent
    {
        protected EventHandlerList _events = new EventHandlerList();
    }
}
