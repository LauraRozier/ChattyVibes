using Buttplug;
using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using static Buttplug.ServerMessage.Types;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Intiface", "LauraRozier", "", "", "Intiface OnDeviceAdded event node")]
    internal sealed class OnDeviceAddedNode : EventNode
    {
        private STNodeOption m_op_index_out;
        private STNodeOption m_op_name_out;
        private STNodeOption m_op_can_vibrate_out;
        private STNodeOption m_op_can_rotate_out;
        private STNodeOption m_op_can_linear_out;
        private STNodeOption m_op_can_stop_out;
        private STNodeOption m_op_can_batterylevel_out;
        private STNodeOption m_op_can_rssilevel_out;

        protected override void BindEvent()
        {
            ((ButtplugDeviceAddedEvent)MainForm.EventFactory.GetEvent(EventType.ButtplugDeviceAdded)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((ButtplugDeviceAddedEvent)MainForm.EventFactory.GetEvent(EventType.ButtplugDeviceAdded)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Device Added";

            m_op_index_out = OutputOptions.Add("Index", typeof(uint), false);
            m_op_name_out = OutputOptions.Add("Name", typeof(string), false);
            m_op_can_vibrate_out = OutputOptions.Add("Can Vibrate", typeof(bool), false);
            m_op_can_rotate_out = OutputOptions.Add("Can Rotate", typeof(bool), false);
            m_op_can_linear_out = OutputOptions.Add("Can Linear", typeof(bool), false);
            m_op_can_stop_out = OutputOptions.Add("Can Stop", typeof(bool), false);
            m_op_can_batterylevel_out = OutputOptions.Add("Can BatteryLevel", typeof(bool), false);
            m_op_can_rssilevel_out = OutputOptions.Add("Can RssiLevel", typeof(bool), false);
        }

        private void OnEventNode_RaiseEvent(object sender, DeviceAddedEventArgs e)
        {
            m_op_index_out.TransferData(e.Device.Index);
            m_op_name_out.TransferData(e.Device.Name);
            m_op_can_vibrate_out.TransferData(e.Device.AllowedMessages.ContainsKey(MessageAttributeType.VibrateCmd));
            m_op_can_rotate_out.TransferData(e.Device.AllowedMessages.ContainsKey(MessageAttributeType.RotateCmd));
            m_op_can_linear_out.TransferData(e.Device.AllowedMessages.ContainsKey(MessageAttributeType.LinearCmd));
            m_op_can_stop_out.TransferData(e.Device.AllowedMessages.ContainsKey(MessageAttributeType.StopDeviceCmd));
            m_op_can_batterylevel_out.TransferData(e.Device.AllowedMessages.ContainsKey(MessageAttributeType.BatteryLevelCmd));
            m_op_can_rssilevel_out.TransferData(e.Device.AllowedMessages.ContainsKey(MessageAttributeType.RssilevelCmd));

            Trigger();
        }
    }
}
