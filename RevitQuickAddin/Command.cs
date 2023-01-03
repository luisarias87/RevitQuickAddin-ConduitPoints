using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using RevitQuickAddin.Extensions.UIDocumentExtensions;
using RevitQuickAddin.LoadFamilies;

namespace RevitQuickAddin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class Command : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var conduitRef = uidoc.Selection.PickObject(ObjectType.Element, new SFilter());

            // the selected Conduit
            Element myConduit = doc.GetElement(conduitRef) as Element;

            var owners = GetAllRefs(myConduit);
            owners.Remove(myConduit);
            TaskDialog.Show("Revit", owners.Count.ToString());


            

            //ConduitCurves conduitCurves = new ConduitCurves(doc, myConduit) ;

            return Result.Succeeded;
        }
        public IList<Element> GetAllRefs(Element element) 
        {
            Element myElement = element as Element;

            ConnectorSet famConnectorSet = new ConnectorSet();

            ConnectorSet curveConnectorSet = new ConnectorSet();

            ConnectorSet  allRefs = new ConnectorSet(); 

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
                    allRefs = connector.AllRefs;

                }
            }
            foreach (Connector connector1 in curveConnectorSet)
            {
                if (connector1.IsConnected)
                {
                    connected.Add(connector1);
                    allRefs = connector1.AllRefs;

                }
            }

            foreach (Connector refs in allRefs)
            {
                
                    Owners.Add(refs.Owner);
            }
            Owners.Remove(myElement);

            return Owners;
        }

        public void ElementGeometry(Autodesk.Revit.DB.Document doc,Element element) 
        {
            

            var geometry = element.get_Geometry(new Options() {View = doc.ActiveView});

            foreach (var gObject in geometry)
            {
                if (gObject is Solid)
                {
                    var solid = (Solid)gObject;
                    var edges = solid.Edges;
                    var faces = solid.Faces;


                }
                if (gObject is Point)
                {
                    var solid = (Point)gObject;
                }


            }



        }


        public class LoadOpts : IFamilyLoadOptions
        {
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }

            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
        }

        public void CreateCurve(Autodesk.Revit.DB.Document document)
        {
            ReferencePointArray rpa = new ReferencePointArray();

            XYZ xyz = document.Application.Create.NewXYZ(0, 0, 0);
            ReferencePoint rp = document.FamilyCreate.NewReferencePoint(xyz);
            rpa.Append(rp);

            xyz = document.Application.Create.NewXYZ(0, 30, 10);
            rp = document.FamilyCreate.NewReferencePoint(xyz);
            rpa.Append(rp);

            xyz = document.Application.Create.NewXYZ(0, 60, 0);
            rp = document.FamilyCreate.NewReferencePoint(xyz);
            rpa.Append(rp);

            xyz = document.Application.Create.NewXYZ(0, 100, 30);
            rp = document.FamilyCreate.NewReferencePoint(xyz);
            rpa.Append(rp);

            xyz = document.Application.Create.NewXYZ(0, 150, 0);
            rp = document.FamilyCreate.NewReferencePoint(xyz);
            rpa.Append(rp);

            CurveByPoints curve = document.FamilyCreate.NewCurveByPoints(rpa);
        }
    }
}
