/*
Hide Category.
TESTED REVIT API: 2020
Hide / Unhide Category in the active view.
Author: Francisco Possetto | github.com/franpossetto
This file is shared on www.revitapidocs.com
For more information visit http://github.com/gtalarico/revitapidocs
License: http://github.com/gtalarico/revitapidocs/blob/master/LICENSE.md
*/

using Autodesk.Revit.DB;

public void HideCategory(Document doc, string categoryName, bool hide)
{
    View activeView = doc.ActiveView;
    Categories categories = doc.Settings.Categories;
    Category category = null;

    // Get the category by name
    foreach (Category cat in categories)
        if (cat.Name == categoryName) category = cat;

    if (category == null) return;

    using (Transaction t = new Transaction(doc, "Hide/Unhide Category"))
    {
        t.Start();
        activeView.SetCategoryHidden(category.Id, hide);
        t.Commit();
    }
}
