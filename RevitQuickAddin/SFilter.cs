using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitQuickAddin
{
    internal class SFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
           return  elem is Element;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
