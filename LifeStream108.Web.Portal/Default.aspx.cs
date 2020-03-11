using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using NLog;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal
{
    public partial class _Default : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected ToDoCategory[] _categories = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (PortalSession.User == null) Response.Redirect("Login.aspx");
            try
            {
                Logger.Info("Load data at page load");
                /*if (!Page.IsPostBack) */LoadData(); // TODO Avoid loading data twice
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading page: " + ex);
            }
        }

        private void LoadData()
        {
            categoryButtonsHolder.Controls.Clear();

            _categories = ToDoCategoryManager.GetUserCategories(PortalSession.User.Id);

            int selectedCategoryId = PortalSession.SelectedCategoryId;
            if (selectedCategoryId <= 0 && _categories.Length > 0)
            {
                selectedCategoryId = _categories[0].Id;
                PortalSession.SelectedCategoryId = selectedCategoryId;
            }

            foreach (ToDoCategory cat in _categories)
            {
                Button categoryButton = new Button
                {
                    ID = "Button" + cat.Id,
                    CommandArgument = cat.Id.ToString(),
                    Text = cat.Name,
                    Enabled = selectedCategoryId != cat.Id,
                    CausesValidation = false,
                    CssClass = selectedCategoryId == cat.Id ? "btn btn-success btn-lg" : "btn btn-primary btn-lg"
                };
                categoryButton.Click += CategoryButton_Click;
                categoryButtonsHolder.Controls.Add(categoryButton);
                categoryButtonsHolder.Controls.Add(new Literal
                {
                    Text = "&nbsp;&nbsp;&nbsp;"
                });
            }
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            PortalSession.SelectedCategoryId = Convert.ToInt32(button.CommandArgument);
            Logger.Info("Load data at click");
            LoadData();
        }
    }
}