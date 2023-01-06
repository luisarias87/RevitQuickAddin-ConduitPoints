using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RevitQuickAddin.MepCurveExtensions
{
    public static class MepCurveExtensions 
    {

        public static IList<Element> GetConnectedConduitElements(this MEPCurve mepCurve) 
        {

            ConnectorSet curveConnectorSet = new ConnectorSet();

            IList<Element> connectedElements = new List<Element>();
            
                curveConnectorSet = mepCurve.ConnectorManager.Connectors;
            
            foreach (Connector connector1 in curveConnectorSet)
            {
                if (connector1.IsConnected)
                {
                    foreach (Connector c in connector1.AllRefs)
                    {
                        if (c.Owner.Id != mepCurve.Id)
                        {
                            connectedElements.Add(c.Owner);
                        }
                    }
                        
                }
            }


            return connectedElements;

        }


        public static MEPSystem GetConduitOrCTrayElements(this MEPCurve mepCurve , Document doc) 
        {
            return mepCurve.MEPSystem;
        }

        public static ElementSet GetDuctOrPipeElements(this MEPCurve mepCurve,Document doc) 
        {
            var Csystem = mepCurve.MEPSystem;

            ElementSet elementSet = null;

            if (Csystem is MechanicalSystem mechanicalSystem)
            {
               elementSet  = mechanicalSystem.DuctNetwork;
                
            }
            else if (Csystem is PipingSystem pipingSystem)
            {
                elementSet =  pipingSystem.PipingNetwork;
            }

            return elementSet;

        }

    }
}
