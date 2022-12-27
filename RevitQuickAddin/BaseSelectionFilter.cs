using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitQuickAddin
{
    public abstract class BaseSelectionFilter : ISelectionFilter
    {
        protected readonly Func<Element, bool> ValidateElement;

        protected BaseSelectionFilter(Func<Element,bool > validateElement)
        {
            this.ValidateElement = validateElement;
        }

        public abstract bool AllowElement(Element elem);


        public abstract bool AllowReference(Reference reference, XYZ position);
       
    }
}
