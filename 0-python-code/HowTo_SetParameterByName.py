"""
Set Parameter by Name 
Set one of element's parameters.

TESTED REVIT API: 2016,2017

Author: Francisco Possetto | github.com/franpossetto

Shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
"""

#Imports
from Autodesk.Revit.DB import Element, Transaction

doc = __revit__.ActiveUIDocument.Document
uidoc = __revit__.ActiveUIDocument
t = Transaction(doc, 'Set Parameter by Name')

#Select element from revit.
selection = [doc.GetElement(x) for x in uidoc.Selection.GetElementIds()]

def set_parameter_by_name(element, parameterName, value):
	element.LookupParameter(parameterName).Set(value)

#Start Transaction
t.Start()

for s in selection:
    #Set a new Comment
	set_parameter_by_name(s,"Comments", "Good Element")

#End Transaction
t.Commit()