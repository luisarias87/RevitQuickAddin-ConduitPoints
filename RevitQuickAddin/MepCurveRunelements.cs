using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitQuickAddin
{
    internal class MepCurveRunelements
    {
        public MepCurveRunelements()
        {

        }

        public IList<Element> GetAllRefs(Element element)
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

                        Owners.Add(c.Owner);
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
                        if (c.Owner != element)
                        {
                            allRefs.Add(c);
                            Owners.Add(c.Owner);
                        }
                    }

                }
            }

            
           

            return Owners;
        }

        
    }
}
