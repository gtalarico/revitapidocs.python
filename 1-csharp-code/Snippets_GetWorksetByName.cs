/*
Get workset by name.
TESTED REVIT API: 2020
Author: Francisco Possetto | github.com/franpossetto
This file is shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
*/

using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;

public Workset GetWorksetByName(Document doc, string WorksetName)
{
    IList<Workset> worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
    Workset workset = worksets.Where(x => x.Name == WorksetName).FirstOrDefault();

    //if the workset does not exist, return the first one.
    return (workset != null) ? workset : worksets.FirstOrDefault();
}