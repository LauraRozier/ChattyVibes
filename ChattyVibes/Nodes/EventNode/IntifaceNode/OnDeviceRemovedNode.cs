using Buttplug;
using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Intiface", "LauraRozier", "", "", "Intiface OnDeviceRemoved event node")]
    internal sealed class OnDeviceRemovedNode : EventNode
    {
        private STNodeOption m_op_index_out;
        private STNodeOption m_op_name_out;

        protected override void BindEvent()
        {
            ((ButtplugDeviceRemovedEvent)MainForm.EventFactory.GetEvent(EventType.ButtplugDeviceRemoved)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((ButtplugDeviceRemovedEvent)MainForm.EventFactory.GetEvent(EventType.ButtplugDeviceRemoved)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Device Removed";

            m_op_index_out = OutputOptions.Add("Index", typeof(uint), false);
            m_op_name_out = OutputOptions.Add("Name", typeof(string), false);
        }

        private void OnEventNode_RaiseEvent(object sender, DeviceRemovedEventArgs e)
        {
            m_op_index_out.TransferData(e.Device.Index);
            m_op_name_out.TransferData(e.Device.Name);

            Trigger();
        }
    }
}
