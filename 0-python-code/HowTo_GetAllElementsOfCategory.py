"""
All elements of Category
Get all elements of the specified category from Model.

TESTED REVIT API: 2016,2017

Author: Francisco Possetto | github.com/franpossetto

Shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
"""

#Imports.
from Autodesk.Revit.DB import FilteredElementCollector, BuiltInCategory

doc = __revit__.ActiveUIDocument.Document

def all_elements_of_category(category):
	return FilteredElementCollector(doc).OfCategory(category).WhereElementIsNotElementType().ToElements()

#All Elements Of Walls Category.
walls = all_elements_of_category(BuiltInCategory.OST_Walls)

#All Elements Of Doors Category.
doors = all_elements_of_category(BuiltInCategory.OST_Doors)

#All Elements Of Windows Category.
windows = all_elements_of_category(BuiltInCategory.OST_Windows)

