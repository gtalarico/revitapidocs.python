"""
Reload All Link Documents

TESTED REVIT API: 2017

Author: min.naung@https://twentytwo.space/contact | https://github.com/mgjean

This file is shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
"""

from Autodesk.Revit.DB import FilteredElementCollector,RevitLinkInstance

uidoc = __revit__.ActiveUIDocument

linkInstances = FilteredElementCollector(doc).OfClass(RevitLinkInstance).ToElements()

load = []

for link in linkInstances:
	linkType  = doc.GetElement(link.GetTypeId());
	filepath = linkType.GetExternalFileReference().GetAbsolutePath();
	try:
		linkType.LoadFrom(filepath,None);
		load.append(link.Name.split(" : ")[0]+" <Loaded>");
	except:
		load.append(link.Name.split(" : ")[0]+" <File Not Found>")
		pass
for i in load:
	print i
