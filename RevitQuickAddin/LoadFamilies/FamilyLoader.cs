using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitQuickAddin.LoadFamilies
{

    public class FamilyLoader
    {
        private readonly Document doc;

        public FamilyLoader(Document doc)
        {
            this.doc = doc;
        }

        public void CreateTables()
        {
            String fileName = @"C:\Users\larias\Desktop\Luis Bim\Table-Dining Round w Chairs.rfa";

            // try to load family 
            Family family = null;

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

            using (Transaction t = new Transaction(doc))
            {
                t.Start("second step");

                // loop through the table symbols and add a new table for each 

                ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();



                double x = 0.0, y = 0.0;

                foreach (ElementId id in familySymbolIds)
                {

                    FamilySymbol symbol = family.Document.GetElement(id) as FamilySymbol ;

                    if (!symbol.IsActive) 
                    {
                       symbol.Activate();
                        doc.Regenerate();  

                    }
                    XYZ location = new XYZ(x, y, 10.0);
                    FamilyInstance instance = doc.Create.NewFamilyInstance(location, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    x += 10;




                }


                t.Commit();
            }









        }
}   }
