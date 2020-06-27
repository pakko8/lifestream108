using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.LifeActivityManagement;
using NUnit.Framework;
using System;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal class LifeActivityTester
    {
        private LifeGroup _newGroup1;
        private LifeGroup _newGroup2;

        private LifeGroupAtGroup _groupAtGroup;

        private LifeActivity _newActivity;

        private const string GroupNewName = "Group 1new";
        private const string ActivityNewName = "Activity 1new";

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

            _groupAtGroup = LifeGroupAtGroupManager.GetGroupAtGroup(_groupAtGroup.Id);
            Assert.NotNull(_groupAtGroup, "Get group at group by id not works");
            Assert.IsTrue(
                _groupAtGroup.UserId > 0
                && _groupAtGroup.LifeGroupId > 0
                && _groupAtGroup.ParentLifeGroupId > 0
                && _groupAtGroup.LifeGroupId != _groupAtGroup.ParentLifeGroupId,
                "Get data for group at group works incorrectly");

            _groupAtGroup = LifeGroupAtGroupManager.GetGroupAtGroupByGroups(
                _groupAtGroup.LifeGroupId, _groupAtGroup.ParentLifeGroupId, UserTester.UserId_1);
            Assert.IsNotNull(_groupAtGroup, "Get group at group by groups not works");

            LifeGroupAtGroup groupAtGroup2 = LifeGroupAtGroupManager.GetGroupAtGroupByGroups(
                _groupAtGroup.LifeGroupId, _groupAtGroup.ParentLifeGroupId, UserTester.UserId_2);
            Assert.IsNull(groupAtGroup2, "Why get group at group works for another user?");

            LifeGroupAtGroup[] groupAtGroupList = LifeGroupAtGroupManager.GetGroupAtGroupsByGroup(
                _groupAtGroup.LifeGroupId, UserTester.UserId_1);
            Assert.IsTrue(groupAtGroupList.Length > 0, "Get group at group by group not works");

            groupAtGroupList = LifeGroupAtGroupManager.GetGroupAtGroupsByGroup(
                _groupAtGroup.LifeGroupId, UserTester.UserId_2);
            Assert.IsTrue(groupAtGroupList.Length == 0, "Why get group at group by group works for another user?");

            groupAtGroupList = LifeGroupAtGroupManager.GetGroupsAtGroupsForUser(UserTester.UserId_1);
            Assert.IsTrue(groupAtGroupList.Length > 0, "Get group at group for user not works");

            groupAtGroupList = LifeGroupAtGroupManager.GetGroupsAtGroupsForUser(UserTester.UserId_2);
            Assert.IsTrue(groupAtGroupList.Length == 0, "Why get group at group for user works for another user?");

            // Test activities
            _newActivity = new LifeActivity
            {
                UserId = UserTester.UserId_1,
                LifeGroupAtGroupId = _groupAtGroup.Id,
                Name = "Activity 1"
            };

            DeleteActivities();

            LifeActivityManager.AddActivity(_newActivity);
            Assert.IsTrue(_newActivity.Id > 0 && _newActivity.UserCode > 0, "Activity has not ids after adding");

            _newActivity.Name = ActivityNewName;
            LifeActivityManager.UpdateActivity(_newActivity);
            _newActivity = LifeActivityManager.GetActivityByName(ActivityNewName, UserTester.UserId_1);
            Assert.IsNotNull(_newActivity, "Get act by name not works");
            Assert.AreEqual(ActivityNewName, _newActivity.Name, "Update act not works");

            LifeActivity activity2 = LifeActivityManager.GetActivityByName(ActivityNewName, UserTester.UserId_2);
            Assert.IsNull(activity2, "Why get act by name works for another user?");

            LifeActivity[] acts = LifeActivityManager.GetActivitiesForUser(UserTester.UserId_1);
            Assert.IsTrue(acts.Length > 0, $"No acts for user {UserTester.UserId_1}");

            acts = LifeActivityManager.GetActivitiesForUser(UserTester.UserId_2);
            Assert.IsTrue(acts.Length == 0, "Why get acts for user works for another user?");

            _newActivity = LifeActivityManager.GetActivityByUserCode(_newActivity.UserCode, UserTester.UserId_1);
            Assert.NotNull(_newActivity, "Get act by code not works");

            // Test activity parameters
            LifeActivityParameter param1 = new LifeActivityParameter
            {
                UserId = UserTester.UserId_1,
                ActivityId = _newActivity.Id,
                Name = "Param 1",
                MeasureId = 1,
                DataType = DataType.Integer
            };
            LifeActivityParameter param2 = new LifeActivityParameter
            {
                UserId = UserTester.UserId_1,
                ActivityId = _newActivity.Id,
                Name = "Param 2",
                MeasureId = 2
            };
            LifeActivityParameterManager.AddParameters(new[] { param1, param2 });
            Assert.IsTrue(param1.Id > 0 && param1.UserCode > 0 && param1.UserId > 0 && param1.ActivityId > 0 && param1.MeasureId > 0,
                "Incorrect adding activity parameter");

            LifeActivityParameter param = LifeActivityParameterManager.GetParameterByCode(param1.UserCode, UserTester.UserId_1);
            Assert.IsNotNull(param, "Get param by code not works");

            param = LifeActivityParameterManager.GetParameterByName(param1.Name, _newActivity.Id, UserTester.UserId_1);
            Assert.IsNotNull(param, "Get param by name not works");

            LifeActivityParameter[] userParams = LifeActivityParameterManager.GetParametersForUser(UserTester.UserId_1);
            Assert.IsTrue(userParams.Length > 0, "Get param for user not works");

            param = LifeActivityParameterManager.GetParameterByCode(param1.UserCode, UserTester.UserId_2);
            Assert.IsNull(param, "Why get param by code works for another user?");

            param = LifeActivityParameterManager.GetParameterByName(param1.Name, _newActivity.Id, UserTester.UserId_2);
            Assert.IsNull(param, "Why get param by name works for another user?");

            userParams = LifeActivityParameterManager.GetParametersForUser(UserTester.UserId_2);
            Assert.IsTrue(userParams.Length == 0, "Why get param for user works for another user?");

            param1.Name = "Updated name";
            param1.MeasureId = 3;
            LifeActivityParameterManager.UpdateParameter(param1);
            LifeActivityParameter updatedParam = LifeActivityParameterManager.GetParameterByCode(param1.UserCode, UserTester.UserId_1);
            Assert.IsTrue(
                updatedParam.Name == param1.Name
                && updatedParam.MeasureId == param1.MeasureId
                && updatedParam.ActivityId == param1.ActivityId, "Update param not works");

            // Test activity with it's parameters
            var actWithParams = LifeActivityManager.GetActivityAndParamsByUserCode(_newActivity.UserCode, UserTester.UserId_1);
            Assert.IsTrue(actWithParams.Activity != null && actWithParams.Parameters.Length > 0,
                "Get act with params by code not works");

            actWithParams = LifeActivityManager.GetActivityWithParams(_newActivity.Id, UserTester.UserId_1);
            Assert.IsTrue(actWithParams.Activity != null && actWithParams.Parameters.Length > 0,
                "Get act with params by id not works");

            actWithParams = LifeActivityManager.GetActivityAndParamsByUserCode(_newActivity.UserCode, UserTester.UserId_2);
            Assert.IsNull(actWithParams.Activity, "Why get act with params by code works for another user?");

            actWithParams = LifeActivityManager.GetActivityWithParams(_newActivity.Id, UserTester.UserId_2);
            Assert.IsNull(actWithParams.Activity, "Get act with params by id works for another user?");

            // Test activity logs
            LifeActivityLog log = new LifeActivityLog
            {
                UserId = UserTester.UserId_1,
                LifeActivityId = _newActivity.Id,
                Period = DateTime.Now,
                Comment = "Comment 1"
            };
            LifeActivityLogValue logValue1 = new LifeActivityLogValue
            {
                UserId = UserTester.UserId_1,
                ActivityParamId = param1.Id,
                NumericValue = 10,
                Period = DateTime.Now
            };
            LifeActivityLogValue logValue2 = new LifeActivityLogValue
            {
                UserId = UserTester.UserId_1,
                ActivityParamId = param2.Id,
                TextValue = "Value",
                Period = DateTime.Now
            };
            LifeActivityLogManager.AddLog(log, new[] { logValue1, logValue2 });
            Assert.IsTrue(log.Id > 0, "Add log not works");
            Assert.IsTrue(logValue1.Id > 0 && logValue1.ActivityLogId > 0, "Add log value not works");

            DeleteGroups();
            DeleteGroupAtGroup();
            DeleteActivities();
            DeleteParams();
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

        private void DeleteActivities()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(LifeActivityManager));
            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from {tableName} where name in ('{_newActivity.Name}', '{ActivityNewName}')", null);
        }

        private void DeleteParams()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(LifeActivityParameterManager));
            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from {tableName} where activity_id in ({_newActivity.Id})", null);
        }
    }
}
