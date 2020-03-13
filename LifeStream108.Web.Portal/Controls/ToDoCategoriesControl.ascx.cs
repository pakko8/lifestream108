using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoCategoriesControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void LoadCaterories()
        {
            ToDoCategory[] categories = PortalSession.ToDoCategories;
            if (categories == null)
            {
                categories = ToDoCategoryManager.GetUserCategories(PortalSession.User.Id);
                PortalSession.ToDoCategories = categories;
            }

            int selectedCategoryId = GetSelectedCategoryId(categories);

            holderCategories.Controls.Clear();
            foreach (ToDoCategory category in categories)
            {
                HyperLink categoryLink = new HyperLink
                {
                    ID = "linkCat_" + category.Id,
                    NavigateUrl = $"/Default?{Constants.RequestCategoryKeyName}={category.Id}",
                    Text = category.Name,
                    Enabled = selectedCategoryId != category.Id,
                    CssClass = selectedCategoryId == category.Id ? "btn btn-success btn-lg" : "btn btn-primary btn-lg"
                };
                holderCategories.Controls.Add(categoryLink);
                holderCategories.Controls.Add(new Literal { Text = "&nbsp;&nbsp;&nbsp;" });
            }
        }

        private int GetSelectedCategoryId(ToDoCategory[] categories)
        {
            int selectedCategoryId = WebUtils.GetRequestIntValue(Constants.RequestCategoryKeyName, Request, 0);
            if (selectedCategoryId <= 0) selectedCategoryId = PortalSession.SelectedCategoryId;

            ToDoCategory foundCategory = categories.FirstOrDefault(n => n.Id == selectedCategoryId);
            if (foundCategory == null)
            {
                selectedCategoryId = categories.Length > 0 ? categories[0].Id : 0;
            }

            PortalSession.SelectedCategoryId = selectedCategoryId;
            return selectedCategoryId;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int selectedCategoryId = Convert.ToInt32(button.CommandArgument);
            PortalSession.SelectedCategoryId = selectedCategoryId;

            LoadCaterories();
            
        }
    }
}