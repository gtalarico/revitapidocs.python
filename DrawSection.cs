//
// (C) Copyright 2003-2016 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Autodesk;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace RenameApp
{

    [Transaction(TransactionMode.Manual)]
    public class DrawSection : IExternalCommand
    {
        UIApplication uiapp = null;
        UIDocument uidoc = null;
        Document doc = null;

        double above, side;

        ViewSection viewSec;

        private Result mepSection(Document doc, IList<MEPCurve> mep)
        {

            //check straight
            IList<LocationCurve> lc = mep.Select(x => x.Location).Cast<LocationCurve>().ToList();//duct.Location as LocationCurve;

            IList<Curve> curve = lc.Select(x => x.Curve).Cast<Curve>().ToList();//lc.Curve as Curve;

            if (curve.Count == 0)
            {
                TaskDialog.Show("...", "Unable to retrieve wall location line.");
                return Result.Failed;

            }

            // Determine view family type to use

            ViewFamilyType vft
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.Section == x.ViewFamily);

            // Determine section box
            IList<XYZ> p = curve.Select(x => x.GetEndPoint(0)).ToList();
            IList<XYZ> q = curve.Select(x => x.GetEndPoint(1)).ToList();

            IList<XYZ> v = q.Zip(p, (x, y) => x - y).ToList();
            IList<XYZ> v2 = v.Select(x => x.X).Zip(v.Select(y => y.Y), (x, y) => new XYZ(x, y, 0)).ToList(); //Z => 0

            IList<XYZ> allPoints = p.Concat(q).ToList();
            //double minAllX = allPoints.Select(x => x.X).Min();
            //double minAllY = allPoints.Select(y => y.Y).Min();
            //double minAllZ = allPoints.Select(z => z.Z).Min();

            //double maxAllX = allPoints.Select(x => x.X).Max();
            //double maxAllY = allPoints.Select(y => y.Y).Max();
            //double maxAllZ = allPoints.Select(z => z.Z).Max();

            IList<BoundingBoxXYZ> boundingBoxes = mep.Select(x => x.get_BoundingBox(doc.ActiveView)).ToList();

            double minAllX = boundingBoxes.Select(x => x.Min).Select(x => x.X).Min();
            double minAllY = boundingBoxes.Select(x => x.Min).Select(x => x.Y).Min();
            double minAllZ = boundingBoxes.Select(x => x.Min).Select(x => x.Z).Min();
            double maxAllX = boundingBoxes.Select(x => x.Max).Select(x => x.X).Max();
            double maxAllY = boundingBoxes.Select(x => x.Max).Select(x => x.Y).Max();
            double maxAllZ = boundingBoxes.Select(x => x.Max).Select(x => x.Z).Max();

            XYZ minAll = new XYZ(minAllX, minAllY, minAllZ);
            XYZ maxAll = new XYZ(maxAllX, maxAllY, maxAllZ);

            minAll.Normalize();
            maxAll.Normalize();

            BoundingBoxXYZ boundingBoxAll = new BoundingBoxXYZ();
            boundingBoxAll.Min = minAll;
            boundingBoxAll.Max = maxAll;

            //t.Origin = midpoint;
            //t.BasisX = ductdir;
            //t.BasisY = up;
            //t.BasisZ = viewdir;

            Transform transform = boundingBoxAll.Transform;
            //boundingBoxAll.Transform = Transform.CreateRotation(XYZ.BasisX, 4.71);
            //transform.BasisY = XYZ.BasisZ;

            //Transform transformAll = boundingBoxAll.Transform;

            try
            {
                using (Transaction t = new Transaction(doc, "View Section"))
                {
                    t.Start();
                    viewSec = ViewSection.CreateSection(doc, vft.Id, boundingBoxAll);
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("O", ex.Message);
            }

            return Result.Succeeded;
        }
        private Result pipeSection(Document doc, Pipe pipe)
        {

            LocationCurve lc = pipe.Location as LocationCurve;

            Curve curve = lc.Curve as Curve;

            if (null == curve)
            {
                TaskDialog.Show("...", "Unable to retrieve pipe location line.");
                return Result.Failed;

            }

            // Determine view family type to use

            ViewFamilyType vft
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.Section == x.ViewFamily);

            // Determine section box
            XYZ p = curve.GetEndPoint(0);
            XYZ q = curve.GetEndPoint(1);
            XYZ v = q - p;
            XYZ v2 = new XYZ(v.X, v.Y, 0);

            BoundingBoxXYZ bb = pipe.get_BoundingBox(null);
            double minZ = bb.Min.Z;
            double maxZ = bb.Max.Z;
            double avg = (maxZ - minZ) / 2;


            double w = v.GetLength();
            double h = maxZ - minZ;
            double d = pipe.Diameter;
            double offset = 0.1 * w;

            XYZ min = new XYZ(-w / 2 - side, -avg - above, -offset);
            XYZ max = new XYZ(w / 2 + side, avg + above, 3);

            XYZ ductdir = v2.Normalize();
            XYZ up = XYZ.BasisZ;
            XYZ viewdir = ductdir.CrossProduct(up);
            XYZ midpoint = p + 0.5 * v;

            Transform t = Transform.Identity;
            t.Origin = midpoint;
            t.BasisX = ductdir;
            t.BasisY = up;
            t.BasisZ = viewdir;

            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();

            try
            {

                sectionBox.Transform = t;
                sectionBox.Min = min;
                sectionBox.Max = max;

            }
            catch (Exception ex)
            {
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Create pipe Section View");
                    if (SectionConfigForm.vft == null)
                        viewSec = ViewSection.CreateSection(doc, vft.Id, sectionBox);
                    else
                    {

                        viewSec = ViewSection.CreateSection(doc, SectionConfigForm.vft.Id, sectionBox);
                    }

                    tx.Commit();
                }
            }
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create pipe Section View");

                if (SectionConfigForm.vft == null)
                    viewSec = ViewSection.CreateSection(doc, vft.Id, sectionBox);
                else
                {

                    viewSec = ViewSection.CreateSection(doc, SectionConfigForm.vft.Id, sectionBox);
                }

                tx.Commit();
            }
            return Result.Succeeded;
        }


        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;


            above = SectionConfigForm.above * 3.28084;
            side = SectionConfigForm.side * 3.28084;


            IList<ElementId> selectedIds = uidoc.Selection.GetElementIds().ToList();
            if (selectedIds.Count == 0)
                try { selectedIds = uidoc.Selection.PickObjects(ObjectType.Element).Select(x => x.ElementId).ToList(); } catch { return Result.Failed; };

            IList<Duct> duct = null;
            IList<Pipe> pipe = null;
            IList<MEPCurve> mep = null;
            IList<Element> elem = null;

            //string info = "Selected elements:\n";

            elem = selectedIds.Select(x => doc.GetElement(x)).ToList();
            //info += elem.GetType() + "\n";

            //TaskDialog.Show("No", elem.Count.ToString());

            mep = elem.Where(x => x.GetType().BaseType == typeof(MEPCurve)).Cast<MEPCurve>().ToList();
            //duct = elem.Where(x => x.GetType() == typeof(Duct)).Cast<Duct>().ToList();
            //pipe = elem.Where(x => x.GetType() == typeof(Pipe)).Cast<Pipe>().ToList();

            //TaskDialog.Show("Oh...", "Please enter only pipe or duct or Cable Trays ...");





            //if (duct.Count >= pipe.Count)
            mepSection(doc, mep);
            //else
            //pipeSection(doc, pipe);


            uiapp.ViewActivated += Uiapp_ViewActivated;

            if (!(viewSec == null))
                uidoc.ActiveView = viewSec as Autodesk.Revit.DB.View;
            else
            {
                message = "No Section View";
                return Result.Failed;
            }

            //TaskDialog.Show("OK", "Successful");

            return Result.Succeeded;

        }

        private void Uiapp_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {
            try
            {
                if ((!(doc.ActiveView.Id == viewSec.Id)) && SectionConfigForm.checkState)
                    using (Transaction t = new Transaction(doc, "Deletion"))
                    {
                        t.Start();
                        doc.Delete(viewSec.Id);
                        t.Commit();
                    }
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Wd", ex.Message);
            }
        }
    }
}
