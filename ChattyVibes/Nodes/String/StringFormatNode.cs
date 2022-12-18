using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using SysMath = System.Math;

namespace ChattyVibes.Nodes.String
{
    [STNode("/String", "LauraRozier", "", "", "String format node")]
    internal class StringFormatNode : StringNode
    {
        private string _format = "";

        private STNodeOption m_op_format_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Format";
            AutoSize = false;

            m_op_format_in = InputOptions.Add("", typeof(string), true);
            m_op_out = OutputOptions.Add("", typeof(string), false);

            m_op_format_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData("");
            AddInput();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _format = (string)e.TargetOption.Data;
            else
                _format = "";

            SendFormattedData();
        }

        private void SendFormattedData()
        {
            if (_format == "")
            {
                m_op_out.TransferData("");
                return;
            }

            int optCount = InputOptionsCount - 2;
            object[] args = new object[optCount];

            for (int i = 0; i < optCount; i++)
                args[i] = InputOptions[i + 1];

            int openCount = _format.Count((c) =>  c == '{');
            int closeCount = _format.Count((c) =>  c == '}');
            int reqItemCount = SysMath.Min(openCount, closeCount);

            if (optCount >= reqItemCount)
                m_op_out.TransferData(string.Format(_format, args));
            else
                m_op_out.TransferData("");
        }

        protected override void OnOwnerChanged()
        {
            base.OnOwnerChanged();

            if (Owner == null)
                return;

            using (System.Drawing.Graphics g = Owner.CreateGraphics())
            {
                Width = base.GetDefaultNodeSize(g).Width;
            }
        }

        private void AddInput()
        {
            var input = new STFormatNodeOption("", typeof(object), true);
            InputOptions.Add(input);
            input.Connected += new STNodeOptionEventHandler(input_Connected);
            input.DataTransfer += new STNodeOptionEventHandler(input_DataTransfer);
            input.DisConnected += new STNodeOptionEventHandler(input_DisConnected);
            Height = TitleHeight + (InputOptions.Count * 20);
        }

        private void input_DisConnected(object sender, STNodeOptionEventArgs e)
        {
            STNodeOption op = sender as STNodeOption;

            if (op.ConnectionCount != 0)
                return;

            InputOptions.RemoveAt(InputOptions.IndexOf(op));

            if (Owner != null)
                Owner.BuildLinePath();

            Height -= 20;
            SendFormattedData();
        }

        private void input_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            STNodeOption op = sender as STNodeOption;
            op.Data = e.TargetOption.Data;
            SendFormattedData();
        }

        private void input_Connected(object sender, STNodeOptionEventArgs e)
        {
            STNodeOption op = sender as STNodeOption;
            var t = typeof(object);

            if (op.DataType == t)
            {
                op.DataType = e.TargetOption.DataType;

                foreach (STNodeOption v in InputOptions)
                {
                    if (v.DataType == t)
                        return;
                }

                AddInput();
            }
            else
            {
                SendFormattedData();
            }
        }

        protected override void OnSaveNode(Dictionary<string, byte[]> dic) =>
            dic.Add("count", BitConverter.GetBytes(InputOptionsCount));

        protected override void OnLoadNode(Dictionary<string, byte[]> dic)
        {
            base.OnLoadNode(dic);
            int nCount = BitConverter.ToInt32(dic["count"], 0);

            while (InputOptionsCount < nCount && InputOptionsCount != nCount)
                AddInput();
        }

        public class STFormatNodeOption : STNodeOption
        {
            private readonly static Type[] _allowedTypes = new Type[]
            {
                typeof(bool),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(string),
                typeof(DateTime)
            };

            public STFormatNodeOption(string strText, Type dataType, bool bSingle) : base(strText, dataType, bSingle) { }

            public override ConnectionStatus ConnectOption(STNodeOption op)
            {
                if (op == null || op == Empty)
                    return ConnectionStatus.Reject;

                if (!_allowedTypes.Contains(op.DataType))
                    return ConnectionStatus.InvalidType;

                var t = typeof(object);

                if (DataType != t)
                    return base.ConnectOption(op);

                DataType = op.DataType;
                var ret = base.ConnectOption(op);

                if (ret != ConnectionStatus.Connected)
                    DataType = t;

                return ret;
            }

            public override ConnectionStatus CanConnect(STNodeOption op)
            {
                if (op == Empty)
                    return ConnectionStatus.EmptyOption;

                if (DataType != typeof(object))
                    return base.CanConnect(op);

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

                foreach (STNodeOption owner_input in Owner.InputOptions)
                {
                    foreach (STNodeOption o in owner_input.ConnectedOption)
                    {
                        if (o == op)
                            return ConnectionStatus.Exists;
                    }
                }

                return ConnectionStatus.Connected;
            }
        }
    }
}
