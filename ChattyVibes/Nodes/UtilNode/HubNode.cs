﻿using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.UtilNode
{
    [STNode("/Util", "LauraRozier", "", "", "Single input to many outputs")]
    public class STNodeHubSingle : STNodeHub
    {
        public STNodeHubSingle() : base(true)
        {
            Title = "HUB - One To Many";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_HUB);
        }
    }

    /*
    [STNode("/Util", "LauraRozier", "", "", "Many inputs to many outputs")]
    public class STNodeHubMulti : STNodeHub
    {
        public STNodeHubMulti() : base(false)
        {
            Title = "HUB - Many To Many";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_HUB);
        }
    }
    */
}
