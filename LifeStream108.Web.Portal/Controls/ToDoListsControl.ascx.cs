using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoListsControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadLists();
        }

        public void LoadLists()
        {
            int currentCategoryId = PortalSession.SelectedCategoryId;
            if (currentCategoryId <= 0) return;

            ToDoList[] listArray = PortalSession.ToDoLists;

            bool needLoadLists = true;
            if (listArray != null && listArray.Length > 0)
            {
                needLoadLists = listArray[0].CategoryId != currentCategoryId;
            }

            if (needLoadLists)
            {
                listArray = ToDoListManager.GetCategoryLists(currentCategoryId);
                PortalSession.ToDoLists = listArray;
            }

            int selectedListId = GetSelectedList(listArray);

            divLists.Controls.Clear();
            foreach (ToDoList list in listArray)
            {
                HyperLink listLink = new HyperLink
                {
                    ID = "btnList_" + list.Id,
                    NavigateUrl = $"/Default?{Constants.RequestListKeyName}={list.Id}",
                    Enabled = selectedListId != list.Id,
                    CssClass = selectedListId == list.Id ? "btn btn-info btn-lg" : "btn btn-secondary btn-lg"
                };
                listLink.Controls.Add(new Literal { Text = $"<span class=\"pull-left\">{list.Name}</span>&nbsp;" });
                divLists.Controls.Add(listLink);
            }
        }

        private int GetSelectedList(ToDoList[] lists)
        {
            int selectedListId = WebUtils.GetRequestIntValue(Constants.RequestListKeyName, Request, 0);
            if (selectedListId <= 0) selectedListId = PortalSession.SelectedListId;

            ToDoList foundList = lists.FirstOrDefault(n => n.Id == selectedListId);
            if (foundList == null)
            {
                selectedListId = lists.Length > 0 ? lists[0].Id : 0;
            }

            PortalSession.SelectedListId = selectedListId;
            return selectedListId;
        }
    }
}
