using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitQuickAddin
{
    internal class LinkableSelectionFilter : BaseSelectionFilter
    {
        private readonly Func<Reference, bool> _validateReference;
        private readonly Document _doc;

        public LinkableSelectionFilter(Document doc, Func<Element, bool> validateElement) : base(validateElement)
        {
            this._doc = doc;
        }

        public LinkableSelectionFilter(Func<Element, bool> validateElement, Func<Reference,bool > validateReference) : base(validateElement)
        {
            this._validateReference = validateReference;
        }

        public override bool AllowElement(Element elem)
        {
            return true;
        }

        public override bool AllowReference(Reference reference, XYZ position)
        {
            if (_doc.GetElement(reference.ElementId) is RevitLinkInstance linkInstance)
            {
                var element = linkInstance.GetLinkDocument().GetElement(reference.LinkedElementId);
                return ValidateElement(element);
            }
            return ValidateElement(_doc.GetElement(reference.ElementId));
        }
    }
}
