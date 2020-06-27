using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.LifeActivityManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal class LifeActivityTester
    {
        private LifeGroup _newGroup1;
        private LifeGroup _newGroup2;
        private LifeGroupAtGroup _groupAtGroup;

        private const string GroupNewName = "Group 1new";

        public void Run()
        {
            // Test groups
            _newGroup1 = new LifeGroup
            {
                UserId = UserTester.UserId_1,
                Name = "Group 1",
                ShortName = "Group1",
            };
            _newGroup2 = new LifeGroup
            {
                UserId = UserTester.UserId_1,
                Name = "Group 2",
                ShortName = "Group2",
            };

            DeleteGroups();

            LifeGroupManager.AddGroup(_newGroup1);
            Assert.IsTrue(_newGroup1.Id > 0 && _newGroup1.UserCode > 0, "Group has not ids after adding");

            LifeGroupManager.AddGroup(_newGroup2);

            LifeGroup[] userGroups = LifeGroupManager.GetGroupsForUser(UserTester.UserId_1);
            Assert.IsTrue(userGroups.Length > 0, $"User {UserTester.UserId_1} has no groups");

            LifeGroup[] anotherUserGroups = LifeGroupManager.GetGroupsForUser(UserTester.UserId_2);
            Assert.IsTrue(anotherUserGroups.Length == 0, $"Why user {UserTester.UserId_2} has groups?");

            _newGroup1.Name = GroupNewName;
            LifeGroupManager.UpdateGroup(_newGroup1);
            LifeGroup updatedGroup = LifeGroupManager.GetGroupByCode(_newGroup1.UserCode, UserTester.UserId_1);
            Assert.IsNotNull(updatedGroup, "Get group by user code not work");
            Assert.IsTrue(updatedGroup.Name == GroupNewName, "Update group not work");

            LifeGroup getGroup = LifeGroupManager.GetGroup(_newGroup2.Id, UserTester.UserId_1);
            Assert.IsNotNull(getGroup, "Get group by id not work");

            getGroup = LifeGroupManager.GetGroupByCode(_newGroup2.UserCode, UserTester.UserId_1);
            Assert.IsNotNull(getGroup, "Get group by code not work");

            getGroup = LifeGroupManager.GetGroup(_newGroup2.Id, UserTester.UserId_2);
            Assert.IsNull(getGroup, "Why get group works for another user?");

            // Test groups at groups
            _groupAtGroup = new LifeGroupAtGroup
            {
                UserId = UserTester.UserId_1,
                LifeGroupId = _newGroup2.Id,
                ParentLifeGroupId = _newGroup1.Id
            };
            LifeGroupAtGroupManager.AddGroupAtGroup(_groupAtGroup);
            Assert.IsTrue(_groupAtGroup.Id > 0, "Group at group has not id after adding");

            DeleteGroups();
            DeleteGroupAtGroup();
        }

        private void DeleteGroups()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(LifeGroupManager));
            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from {tableName} where name in ('{_newGroup1.Name}', '{_newGroup2.Name}', '{GroupNewName}')", null);
        }

        private void DeleteGroupAtGroup()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(LifeGroupManager));
            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from {tableName} where id={_groupAtGroup.Id}", null);
        }
    }
}
