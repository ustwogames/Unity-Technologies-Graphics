﻿using Unity.GraphToolsFoundation.Editor;

namespace UnityEditor.ShaderGraph.GraphUI
{
    class RedirectNode : Node
    {
        protected override void BuildPartList()
        {
            PartList.AppendPart(InOutPortContainerPart.Create(portContainerPartName, Model, this, ussClassName));
            AddToClassList("sg-redirect-node");
            this.AddStylesheet("RedirectNode.uss");
        }
    }
}
