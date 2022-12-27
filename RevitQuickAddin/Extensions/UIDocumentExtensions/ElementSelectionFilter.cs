﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitQuickAddin.Extensions.UIDocumentExtensions
{
    internal class ElementSelectionFilter : BaseSelectionFilter
    {
        private readonly Func<Reference, bool> _validateReference;

        public ElementSelectionFilter(Func<Element, bool> validateElement) : base(validateElement)
        {

        }

        public ElementSelectionFilter(Func<Element,bool> validateElement,Func<Reference,bool> validateReference):base(validateElement)
        {
            this._validateReference = validateReference;
        }

        public override bool AllowElement(Element elem)
        {
            return ValidateElement(elem);
        }

        public override bool AllowReference(Reference reference, XYZ position)
        {
            return _validateReference?.Invoke(reference) ?? true;
        }
    }
}
