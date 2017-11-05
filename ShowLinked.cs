using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace RenameApp
{
    [Transaction(TransactionMode.Manual)]
    class ShowLinked : IExternalCommand
    {
        
public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            
            //Form2 f2 = new Form2();

            //f2.ShowDialog();

            //ICollection<ElementId> elementIDs = new List<ElementId>();
            
            //foreach (string str in f2.textBox1.Text.Split(','))
            //{
            //    ElementId id = new ElementId(int.Parse(str));
            //    elementIDs.Add(id);
            //}

            

            //ElementTransformUtils.CopyElements(doc, new ElementId( 782319), null);
                
            
            
            return Result.Succeeded;
        }
    }
}
