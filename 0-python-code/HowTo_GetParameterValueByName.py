"""
Get Parameter Value by Name 
Get value of one of element's parameters.

TESTED REVIT API: 2016,2017

Author: Francisco Possetto | github.com/franpossetto

Shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
"""

#Imports.
from Autodesk.Revit.DB import Element

doc = __revit__.ActiveUIDocument.Document
uidoc = __revit__.ActiveUIDocument

def get_parameter_value_by_name(element, parameterName):
	return element.LookupParameter(parameterName).AsValueString()
	
#Select elements from revit.
selection = [doc.GetElement(x) for x in uidoc.Selection.GetElementIds()]

#Example with Walls.
for wall in selection:
	print get_parameter_value_by_name(wall, "Base Constraint")
    