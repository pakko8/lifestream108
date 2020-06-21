using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement;
using LifeStream108.Web.Portal.App_Code;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoListsControl : CommonUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void LoadLists()
        {
            int currentCategoryId = PortalSession.SelectedCategoryId;
            if (currentCategoryId <= 0) return;

            ToDoList[] listArray = GetLists(currentCategoryId);
            int selectedListId = GetSelectedListId(listArray);

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

            PortalSession.SelectedTaskId = 0;
        }

        private ToDoList[] GetLists(int currentCategoryId)
        {
            ToDoList[] listArray = PortalSession.ToDoLists;

            bool needLoadListsFromDb = true;
            if (listArray != null && listArray.Length > 0)
            {
                needLoadListsFromDb = listArray[0].CategoryId != currentCategoryId;
            }

            if (needLoadListsFromDb)
            {
                listArray = ToDoListManager.GetCategoryLists(currentCategoryId).Where(n => n.Active).ToArray();
                PortalSession.ToDoLists = listArray;
            }

            return listArray;
        }

        private int GetSelectedListId(ToDoList[] lists)
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
