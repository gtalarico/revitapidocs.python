"""
Hide Unhide Links Levels Grids

TESTED REVIT API: 2017

Author: min.naung@https://twentytwo.space/contact | https://github.com/mgjean

This file is shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
"""

import System
from System.Collections.Generic import List
from Autodesk.Revit.DB import Transaction
from Autodesk.Revit.DB import *

doc = __revit__.ActiveUIDocument.Document


active_view = doc.ActiveView
# filter name
ifilter = "LinkLevelGrid_Quasar"
found = False

unhide = False # Edit here to hide/unhide
msg = "Unhide" if unhide else "Hide"
trans = Transaction(doc,"%s links levels grids" %(msg))
trans.Start()

allFilters = FilteredElementCollector(doc).OfClass(FilterElement).ToElements()

viewFilters = active_view.GetFilters();
viewFiltersName = [doc.GetElement(i).Name.ToString() for i in viewFilters]

for fter in allFilters:
	if ifilter == fter.Name.ToString() and ifilter not in viewFiltersName:
		active_view.AddFilter(fter.Id)
		active_view.SetFilterVisibility(fter.Id, unhide);
		found = True
	
	if ifilter == fter.Name.ToString() and ifilter in viewFiltersName:
		active_view.SetFilterVisibility(fter.Id, unhide);
		found = True
		
if not found:
	grids = FilteredElementCollector(doc).OfClass(Grid).ToElements()
	levels = FilteredElementCollector(doc).OfClass(Level).ToElements()

	CateIds = List[ElementId]([grids[0].Category.Id,levels[0].Category.Id])

	gridTypeIds = set([i.GetTypeId() for i in grids])
	levelTypeIds = set([i.GetTypeId() for i in levels])

	type_elems = [doc.GetElement(i) for i in gridTypeIds]
	type_elems.extend([doc.GetElement(l) for l in levelTypeIds])

	for elem in type_elems:
		if not "_quasar" in elem.LookupParameter("Type Name").AsString():
			elem.Name = elem.LookupParameter("Type Name").AsString() + "_quasar";

	type_names = [i.LookupParameter("Type Name").AsString() for i in type_elems]

	paramId = type_elems[0].LookupParameter("Type Name").Id
	# create filter rule
	notendswith = ParameterFilterRuleFactory.CreateNotEndsWithRule(paramId,"_quasar",False)
	# create new filter
	paramFilterElem = ParameterFilterElement.Create(doc, ifilter,CateIds,[notendswith])
	active_view.SetFilterOverrides(paramFilterElem.Id, OverrideGraphicSettings())
	active_view.SetFilterVisibility(paramFilterElem.Id, unhide)
	
print "DONE!"
trans.Commit()