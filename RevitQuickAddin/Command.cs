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
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using RevitQuickAddin.Extensions.UIDocumentExtensions;
using RevitQuickAddin.LoadFamilies;
using RevitQuickAddin.MepCurveExtensions;

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
            MEPCurve myConduit = doc.GetElement(conduitRef) as MEPCurve;


            // the list of elements in the run 
            var runElements = new List<Element>();

            // list of to add and remove
            var addRemove = new List<Element>();

            var curve = new MepCurveRunelements();

            // Element connected to my conduit
            var elementsConnectedTomyconduit =  myConduit.GetConnectedConduitElements();

            runElements.Add(myConduit as Conduit);
            


            Element next = null;

            foreach (var element in elementsConnectedTomyconduit)
            {
                // add the elements to my run elements list
                
                next = elementsConnectedTomyconduit.Last();



            }
            
            while (next != null)
            {
                var iterate =  curve.GetAllRefs(next);
                runElements.Add(iterate.First());
                if (next is MEPCurve)
                {
                    next = iterate.First();
                }
                else
                {
                    next = iterate.Last();
                }
                

            }
            foreach (var ele in runElements)
            {
                TaskDialog.Show("Revit", ele.Id.ToString()) ;
            }
            


            //ConduitCurves conduitCurves = new ConduitCurves(doc, myConduit) ;

            return Result.Succeeded;
        }
        public IList<Element> GetAllRefs(Element element) 
        {
            Element myElement = element as Element;

            ConnectorSet famConnectorSet = new ConnectorSet();

            ConnectorSet curveConnectorSet = new ConnectorSet();

            IList<Connector>  allRefs = new List<Connector>(); 

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
            //Owners.Remove(myElement);

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
