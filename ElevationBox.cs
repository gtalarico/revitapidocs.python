using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using System.Diagnostics;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace RenameApp
{
    [Transaction(TransactionMode.Manual)]
    class ElevationBox : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {



            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            IEnumerable<ElementId> selElSet = uidoc.Selection.GetElementIds();
            IList<double> XX = new List<double>(), YY = new List<double>(), ZZ = new List<double>();
            XX.Clear(); YY.Clear(); ZZ.Clear();
            if (selElSet.Count().Equals(0)) return Result.Failed;
            foreach (ElementId elll in selElSet)
            {
                double n = doc.GetElement(elll).get_BoundingBox(doc.ActiveView).Min.X;
                XX.Add(n);
                n = doc.GetElement(elll).get_BoundingBox(doc.ActiveView).Max.X;
                XX.Add(n);

                n = doc.GetElement(elll).get_BoundingBox(doc.ActiveView).Min.Y;
                YY.Add(n);
                n = doc.GetElement(elll).get_BoundingBox(doc.ActiveView).Max.Y;
                YY.Add(n);

                n = doc.GetElement(elll).get_BoundingBox(doc.ActiveView).Min.Z;
                ZZ.Add(n);
                n = doc.GetElement(elll).get_BoundingBox(doc.ActiveView).Max.Z;
                ZZ.Add(n);

            }

            FilteredElementCollector lvlcol = new FilteredElementCollector(doc).
                OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
            double minElevation = 0, maxElevation = 0;
            foreach (Level lvl in lvlcol)
            {
                if (lvl.Elevation > maxElevation)
                    maxElevation = lvl.Elevation;
                if (lvl.Elevation < minElevation)
                    minElevation = lvl.Elevation;
            }


            XYZ min = new XYZ(XX.Min(), YY.Min(), minElevation - 5);
            XYZ max = new XYZ(XX.Max(), YY.Max(), maxElevation + 5);

            if (max.Subtract(min).GetLength() < 5)
            {
                min.Add(min.Multiply(1.5));
                max.Add(max.Multiply(1.5));
            }

            Transform t = Transform.Identity;
            t.BasisX = XYZ.BasisX;
            t.BasisY = XYZ.BasisY;
            t.BasisZ = XYZ.BasisZ;

            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
            sectionBox.Transform = t;
            sectionBox.Min = min;
            sectionBox.Max = max;



            ViewFamilyType vft
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.Section == x.ViewFamily);

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create Section View");

                View Myview = ViewSection.CreateSection(doc, vft.Id, sectionBox);
                #region Test/////////////////3D 

                ViewFamilyType vft3d
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.ThreeDimensional == x.ViewFamily);

                View3D view3dEX = View3D.CreateIsometric(doc, vft3d.Id);

                view3dEX.SetSectionBox(sectionBox);
                #endregion


                tx.Commit();
                uidoc.ActiveView = view3dEX;
            }
            return Result.Succeeded;
        }
    }
}

