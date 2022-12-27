using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitQuickAddin
{
    public class LoadFamily
    {
        // try to load family 
        Family family = null;

        private readonly Document Doc;
        private readonly XYZ location;

        public LoadFamily(Document doc)
        {
            this.Doc = doc;
            this.location = location;
           
        }

        public Family LoadFamilyFile()
        {
            Document doc = Doc;

            // The file that the family resides in
            String fileName = @"C:\Users\larias\Desktop\Luis Bim\SampleFamily.rfa";

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Load Family");

                try
                {
                    doc.LoadFamily(fileName, out family);

                }
                catch (Exception e)
                {

                    TaskDialog.Show("Revit", e.Message);
                }
                tx.Commit();
            }
            return family;
        }

        public void PlaceFamily(Family family,XYZ location)
        {
            Document doc = Doc;
            this.family = family;

            using (Transaction t = new Transaction(doc))
            {

                t.Start("second step");


                // loop Through table symbols and add a new table for each
                ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();

                foreach (ElementId id in familySymbolIds)
                {
                    FamilySymbol symbol = family.Document.GetElement(id) as FamilySymbol;

                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                    }


                    FamilyInstance instance = doc.Create.NewFamilyInstance(location, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                }

                t.Commit();
            }

        }








    }



    
}
