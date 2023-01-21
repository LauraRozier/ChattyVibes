﻿using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Enum choise node")]
    internal sealed class EnumChoiseNode : ChoiseNode
    {
        private Enum _tval = null;
        private Enum _fval = null;

        protected override void OnCreate()
        {
            _type = typeof(Enum);
            base.OnCreate();
            Title = "Enum Choise";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_true_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_false_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_true_in)
                    _tval = (Enum)e.TargetOption.Data;
                else
                    _fval = (Enum)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = null;
                else
                    _fval = null;
            }

            HandleCondition();
        }

        protected override void HandleCondition()
        {
            if (_condition)
                m_op_true_out.TransferData(_tval);
            else
                m_op_false_out.TransferData(_fval);
        }
    }
}
