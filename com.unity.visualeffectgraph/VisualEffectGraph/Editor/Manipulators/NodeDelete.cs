using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental;
using UnityEditor.Experimental.Graph;
using Object = UnityEngine.Object;


namespace UnityEditor.Experimental
{
    internal class NodeDelete : IManipulate
    {

        public NodeDelete()
        {
        }

        public bool GetCaps(ManipulatorCapability cap)
        {
            if (cap == ManipulatorCapability.MultiSelection)
                return true;
            return false;
        }

        public void AttachTo(CanvasElement element)
        {
            element.KeyDown += DeleteNode;
        }

        private bool DeleteNode(CanvasElement element, Event e, Canvas2D canvas)
        {
            if (e.type == EventType.Used)
                return false;

            if (e.keyCode != KeyCode.Delete)
            {
                return false;
            }

            if (!(element is VFXEdNode) || !element.selected)
            {
                return false;
            }

            // Prepare undo
            (canvas.dataSource as VFXEdDataSource).UndoSnapshot("Deleting Node" + (element as VFXEdNode).title);

            // Delete Edges
            VFXEdNode node = element as VFXEdNode;
            List<CanvasElement> todelete = new List<CanvasElement>();

            foreach (CanvasElement ce in canvas.dataSource.FetchElements())
            {
                if (ce is Edge<VFXEdFlowAnchor>)
                {
                    if (node.inputs.Contains((ce as Edge<VFXEdFlowAnchor>).Left) || node.inputs.Contains((ce as Edge<VFXEdFlowAnchor>).Right))
                    {
                        todelete.Add(ce);
                    }
                    if (node.outputs.Contains((ce as Edge<VFXEdFlowAnchor>).Left) || node.outputs.Contains((ce as Edge<VFXEdFlowAnchor>).Right))
                    {
                        todelete.Add(ce);
                    }
                }
            }
            foreach (CanvasElement ce in todelete)
                canvas.dataSource.DeleteElement(ce);

			// Update the model
			VFXSystemModel owner = node.Model.GetOwner();
			if (owner != null)
			{
				int nbChildren = owner.GetNbChildren();
				int index = owner.GetIndex(node.Model);

				node.Model.Detach();
				if (index != 0 && index != nbChildren - 1)
				{
					// if the node is in the middle of a system, we need to create a new system
					VFXSystemModel newSystem = new VFXSystemModel();
					while (owner.GetNbChildren() > index)
						owner.GetChild(index).Attach(newSystem);
					newSystem.Attach(VFXEditor.AssetModel);
				}

				
			}
			
            // Finally 
            canvas.dataSource.DeleteElement(element);
            canvas.ReloadData();
            canvas.Repaint();

            return true;
        }

    };
}
