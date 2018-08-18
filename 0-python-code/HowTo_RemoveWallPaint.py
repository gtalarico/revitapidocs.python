"""
Remove Wall Paint

TESTED REVIT API: 2017

Author: min.naung @https://twentytwo.space/contact | https://github.com/mgjean

This file is shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
"""

from Autodesk.Revit.DB import Transaction, Reference, FilteredElementCollector


uidoc = __revit__.ActiveUIDocument
doc = __revit__.ActiveUIDocument.Document

# selected elements
selection = uidoc.Selection
# wall category id
catId = FilteredElementCollector(doc).OfClass(Wall).ToElements()[0].Category.Id

# filtered wall elements
walls = [doc.GetElement(id) for id in selection.GetElementIds() if doc.GetElement(id).Category.Id == catId]

# info message
msg = "%s walls." %len(walls) if len(walls)>1 else "%s wall." %len(walls) 

# transaction
t = Transaction(doc, 'walls paint remove')

# start transaction
t.Start()

# loop wall elements
for wall in walls:
	
	# get geometry object (solid)
	geoelem = wall.GetGeometryObjectFromReference(Reference(wall))
	
	# solid to geometry object
	geoobj = geoelem.GetEnumerator()
	
	# loop geometry object
	for obj in geoobj:
	
		# collect faces from geometry object
		for f in obj.Faces:
		
			# get each face
			doc.RemovePaint(wall.Id,f)

# print info message
print "Paint removed from %s" %(msg)
#end of transaction
t.Commit()
