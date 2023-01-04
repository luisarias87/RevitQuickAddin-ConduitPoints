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
        public static IList<Element> GetAllRefs(this MEPCurve mepCurve,Element element)
        {
            Element myElement = element as Element;

            ConnectorSet famConnectorSet = new ConnectorSet();

            ConnectorSet curveConnectorSet = new ConnectorSet();

            IList<Connector> allRefs = new List<Connector>();

            IList<Connector> connected = new List<Connector>();

            IList<Element> Owners = new List<Element>();

            if (myElement is FamilyInstance familyInstance)
            {
                famConnectorSet = familyInstance.MEPModel.ConnectorManager.Connectors;
            }

            if (myElement is MEPCurve curve)
            {
                curveConnectorSet = curve.ConnectorManager.Connectors;
            }

            foreach (Connector connector in famConnectorSet)
            {
                if (connector.IsConnected)
                {
                    connected.Add(connector);
                    foreach (Connector c in connector.AllRefs)
                    {
                        allRefs.Add(c);
                    }

                }
            }
            foreach (Connector connector1 in curveConnectorSet)
            {
                if (connector1.IsConnected)
                {
                    connected.Add(connector1);
                    foreach (Connector c in connector1.AllRefs)
                    {
                        allRefs.Add(c);
                    }

                }
            }

            foreach (Connector refs in allRefs)
            {
                if (refs.Owner.Id != myElement.Id)
                {
                    Owners.Add(refs.Owner);
                }

            }
            

            return Owners;
        }






        public static IList<Element> GetConnectedConduitElements(this MEPCurve mepCurve) 
        {

            


            ConnectorSet curveConnectorSet = new ConnectorSet();

            IList<Connector> allRefs = new List<Connector>();

            IList<Connector> connected = new List<Connector>();

            IList<Element> Owners = new List<Element>();

            

            if (mepCurve is MEPCurve curve)
            {
                curveConnectorSet = curve.ConnectorManager.Connectors;
            }

            foreach (Connector connector1 in curveConnectorSet)
            {
                if (connector1.IsConnected)
                {
                    connected.Add(connector1);
                    foreach (Connector c in connector1.AllRefs)
                    {
                        allRefs.Add(c);
                    }
                }
            }
            foreach (Connector refs in allRefs)
            {
                if (refs.Owner.Id != mepCurve.Id)
                {
                   Owners.Add(refs.Owner);
                }

            }

            return Owners;

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
