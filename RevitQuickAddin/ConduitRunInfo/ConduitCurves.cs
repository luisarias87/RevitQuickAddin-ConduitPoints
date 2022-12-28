using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace RevitQuickAddin
{
    public class ConduitCurves
    {
        public ConduitCurves(Document doc,Conduit conduit)
        {
            Doc = doc;
            myConduit = conduit;
            LoadFamily family = new LoadFamily(doc);
            ConduitPoints(doc);
        }

        public Document Doc { get; set; }
        public Conduit myConduit { get; set; }

        public void ConduitPoints(Document doc) 
        {
            
                Doc = doc;

            var conRunsCol = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ConduitRun).Where(e => e.Id == myConduit.RunId).ToList(); 

                // list of conduits
                List<Conduit> conduits = new List<Conduit>();


                // List of Points from Conduit
                IList<XYZ> familyLocations = new List<XYZ>();

                //List of Geometryobjects
                IList<GeometryObject> geometryObjects = new List<GeometryObject>();

                // Conduit Filtererd Element Collector
                var conCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Conduit).Where(c => c is Conduit).ToList();

                // Conduits that are part of the conduit run chosen
                var myRunConduits = new List<Conduit>();

                foreach (Conduit conduit in conCollector)
                {
                    if (conduit.RunId == myConduit.RunId)
                    {
                        myRunConduits.Add(conduit);
                    }

                }

            // Conduits who are horizontal ruling out all vertial
            // conduits of slopes ones and adding them to the conduits list
            foreach (Conduit conduit in myRunConduits)
            {
                var lc = conduit.Location as LocationCurve;

                var line = lc.Curve as Line;


                var s = line.GetEndPoint(0);
                var e = line.GetEndPoint(1);
                var sz = s.Z;
                var ez = e.Z;

                if ((ez - sz) <= .1 && (ez - sz) >= (-.1))
                {
                    conduits.Add(conduit);
                }

              
            }






            // using the useful conduits.
            foreach (Conduit c in conduits)
            {
                var cLocation = (LocationCurve)c.Location;

                var locationAsLIne = (Line)cLocation.Curve;

                var directionXyz = locationAsLIne.Direction;

                var cLength = cLocation.Curve.Length;



                // Start and end Xyz of conduit
                var startPoint = cLocation.Curve.GetEndPoint(0);
                var endPoint = cLocation.Curve.GetEndPoint(1);

                var directionNormalized = directionXyz.Normalize();

                using (Transaction t = new Transaction(doc, "First Trans"))
                {


                    // First point of the conduit in the middle 
                    if (cLocation.Curve.Length < 10)
                    {
                        t.Start();
                        // this will be the first hanger 1' from the start.
                        Point firstPoint = Point.Create(startPoint + directionNormalized * cLocation.Curve.Length / 2);


                        // The direct shape for the point
                        var directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

                        // setting the 3D Point to Visualize the hanger placement
                        directShape.SetShape(new List<GeometryObject>() { firstPoint });

                        t.Commit();


                    }



                }



                // if the conduit is longet than 10

                using (Transaction t = new Transaction(doc, "Linear Array"))
                {


                    if (cLocation.Curve.Length > 12)
                    {
                        t.Start();


                        // this will be the first hanger 1' from the start.
                        Point firstPoint = Point.Create(startPoint + directionNormalized);

                        Point secondArrPoint = Point.Create(firstPoint.Coord + directionNormalized * 8);

                        // The direct shape for the point
                        DirectShape arrDirectShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));


                        DirectShape arrDirectShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));


                        arrDirectShape.SetShape(new List<GeometryObject>() { firstPoint });

                        arrDirectShape2.SetShape(new List<GeometryObject>() { secondArrPoint });


                        Line firstPointToLast = Line.CreateBound(secondArrPoint.Coord, endPoint);

                        int lineLength = (int)firstPointToLast.Length;


                        familyLocations.Add(firstPoint.Coord);

                        familyLocations.Add(secondArrPoint.Coord);

                        t.Commit();



                        if (lineLength > 8)
                        {
                            t.Start();
                            var linearArray = LinearArray.Create(doc, doc.ActiveView, arrDirectShape2.Id, lineLength / 8 + 1, directionXyz.Normalize() * 8, ArrayAnchorMember.Second);
                            t.Commit();

                        }



                    }


                    
                }

                

               
            }

        }
    }
}
