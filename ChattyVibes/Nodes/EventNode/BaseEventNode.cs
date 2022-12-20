namespace ChattyVibes.Nodes.EventNode
{
    internal abstract class BaseEventNode : EventNode
    {
        protected abstract void BindEvent();

        protected abstract void UnbindEvent();

        ~BaseEventNode()
        {
            UnbindEvent();
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.Out;
            base.OnCreate();
            BindEvent();
        }
    }
}
