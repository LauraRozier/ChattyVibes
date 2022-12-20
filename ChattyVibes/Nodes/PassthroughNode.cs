using ST.Library.UI.NodeEditor;
using System;
using System.Drawing;
using System.Linq;
using static ChattyVibes.Nodes.StringNode.StringFormatNode;
using static ST.Library.UI.NodeEditor.STNodeHub;

namespace ChattyVibes.Nodes
{
    [STNode("/", "LauraRozier", "", "", "Passthrough node")]
    internal sealed class PassthroughNode : STNode
    {
        private readonly static Type C_OBJ_TYPE = typeof(object);

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Pass";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_HUB);

            m_op_in = new STPassthroughNodeOption("IN", typeof(object), true);
            InputOptions.Add(m_op_in);
            m_op_out = new STPassthroughNodeOption("OUT", typeof(object), false);
            OutputOptions.Add(m_op_out);

            m_op_in.Connecting += new STNodeOptionConnectionEventHandler(m_Connecting);
            m_op_out.Connecting += new STNodeOptionConnectionEventHandler(m_Connecting);

            m_op_in.Connected += new STNodeOptionEventHandler(m_Connected);
            m_op_out.Connected += new STNodeOptionEventHandler(m_Connected);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);

            m_op_in.DisConnected += new STNodeOptionEventHandler(m_DisConnected);
            m_op_out.DisConnected += new STNodeOptionEventHandler(m_DisConnected);
        }

        private bool m_Connecting(object sender, STNodeOptionEventArgs e)
        {
            if (e.TargetOption is STFormatNodeOption || e.TargetOption is STNodeHubOption)
            {
                if ((sender as STNodeOption).DataType != e.TargetOption.DataType)
                    return true;
            }

            if (
                (sender == m_op_in && m_op_out.ConnectionCount > 0 && m_op_out.DataType != e.TargetOption.DataType) ||
                (sender == m_op_out && m_op_in.ConnectionCount > 0 && m_op_in.DataType != e.TargetOption.DataType)
            )
                return false;

            return true;
        }

        private void m_Connected(object sender, STNodeOptionEventArgs e)
        {
            if (sender == m_op_in && m_op_out.ConnectionCount <= 0 && m_op_out.DataType != e.TargetOption.DataType)
                m_op_out.DataType = e.TargetOption.DataType;
            else if (sender == m_op_out && m_op_in.ConnectionCount <= 0 && m_op_in.DataType != e.TargetOption.DataType)
                m_op_in.DataType = e.TargetOption.DataType;

            (sender as STNodeOption).DataType = e.TargetOption.DataType;
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e) =>
            m_op_out.TransferData(e.TargetOption.Data);

        private void m_DisConnected(object sender, STNodeOptionEventArgs e)
        {
            if (sender == m_op_in)
                m_op_out.TransferData(null);

            if (m_op_in.ConnectionCount + m_op_out.ConnectionCount > 0)
                return;

            m_op_in.DataType = C_OBJ_TYPE;
            m_op_out.DataType = C_OBJ_TYPE;
            Owner?.BuildLinePath();
        }

        public class STPassthroughNodeOption : STNodeOption
        {
            private readonly static Type[] _allowedTypes = new Type[]
            {
                typeof(object),
                typeof(bool),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(char),
                typeof(string),
                typeof(DateTime),
                typeof(Enum)
            };

            public STPassthroughNodeOption(string strText, Type dataType, bool bSingle) : base(strText, dataType, bSingle) { }

            public override ConnectionStatus ConnectOption(STNodeOption op)
            {
                if (op == null || op == Empty)
                    return ConnectionStatus.Reject;

                if (!_allowedTypes.Contains(op.DataType))
                    return ConnectionStatus.InvalidType;

                if (DataType != C_OBJ_TYPE)
                    return base.ConnectOption(op);

                DataType = op.DataType;
                var ret = base.ConnectOption(op);

                if (ret != ConnectionStatus.Connected)
                    DataType = C_OBJ_TYPE;

                return ret;
            }

            public override ConnectionStatus CanConnect(STNodeOption op)
            {
                if (op == Empty)
                    return ConnectionStatus.EmptyOption;

                if (IsInput == op.IsInput)
                    return ConnectionStatus.SameInputOrOutput;

                if (op.Owner == null || Owner == null)
                    return ConnectionStatus.NoOwner;

                if (op.Owner == Owner)
                    return ConnectionStatus.SameOwner;

                if (Owner.LockOption || op.Owner.LockOption)
                    return ConnectionStatus.Locked;

                if (IsSingle && m_hs_connected.Count == 1)
                    return ConnectionStatus.SingleOption;

                if (op.IsInput && STNodeEditor.CanFindNodePath(op.Owner, Owner))
                    return ConnectionStatus.Loop;
                
                if (m_hs_connected.Contains(op))
                    return ConnectionStatus.Exists;

                if (!_allowedTypes.Contains(op.DataType))
                    return ConnectionStatus.InvalidType;

                if (!IsInput)
                    return ConnectionStatus.Connected;

                return ConnectionStatus.Connected;
            }
        }
    }
}
