/*
Pass Document and name string as Variables
This snippet is a utility function to make working with family symbols easier
also is an example on using FilteredElementCollector filters

TESTED REVIT API: 2019
The snippet can be used as is in a Revit Application Macro for test purposes

Author: Robert Curry | https://github.com/RobertCurry0216

This file is shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
*/

public static FamilySymbol GetFamilySymbolByName(Document doc, string name)
{
    var paramId = new ElementId(BuiltInParameter.ALL_MODEL_FAMILY_NAME);
    var paramValueProvider = new ParameterValueProvider(paramId);
    var equalsRule = new FilterStringEquals();
    var filterRule = new FilterStringRule(paramValueProvider, equalsRule, name, false);
    var filter = new ElementParameterFilter(filterRule);

    var fec = new FilteredElementCollector(doc);
    fec.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().WherePasses(filter);

    if (fec.GetElementCount() == 1)
    {
        var symbol = fec.FirstElement() as FamilySymbol;
        if (!symbol.IsActive)
        {
            symbol.Activate();
            doc.Regenerate();
        }
        return symbol;
    }
    return null;
}