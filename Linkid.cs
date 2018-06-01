using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.IO;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace RenameApp
{
    [Transaction(TransactionMode.Manual)]
    class Linkid : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection selection = commandData.Application.ActiveUIDocument.Selection;
            ICollection<Reference> selected = null;
            try { selected = selection.PickObjects(ObjectType.LinkedElement); } catch { return Result.Failed; }
            
            string info = "";
            foreach (Reference id in selected)
            {
                info += "#Linked Element ID: " + id.LinkedElementId.ToString() + "\r\n";
            }
            Form2 f2 = new Form2();
            f2.textBox1.Text = info;
            f2.ShowDialog();

            return Result.Succeeded;
        }
            
        }
    }

