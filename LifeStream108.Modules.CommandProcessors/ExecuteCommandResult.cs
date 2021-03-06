﻿using LifeStream108.Libs.Common;
using System.Collections.Generic;
using System.Text;

namespace LifeStream108.Modules.CommandProcessors
{
    public class ExecuteCommandResult
    {
        #region Methods for session
        private int _projectId;

        public bool NeedUpdateProjectId { get; private set; }

        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                _projectId = value;
                NeedUpdateProjectId = true;
            }
        }

        private int _commandId;

        public bool NeedUpdateCommandId { get; private set; }

        public int CommandId
        {
            get { return _commandId; }
            set
            {
                _commandId = value;
                NeedUpdateCommandId = true;
            }
        }

        private int _lifeGroupId;

        public bool NeedUpdateLifeGroupId { get; private set; }

        public int LifeGroupId
        {
            get { return _lifeGroupId; }
            set
            {
                _lifeGroupId = value;
                NeedUpdateLifeGroupId = true;
            }
        }

        private int _lifeActivityId;

        public bool NeedUpdateLifeActivityId { get; private set; }

        public int LifeActivityId
        {
            get { return _lifeActivityId; }
            set
            {
                _lifeActivityId = value;
                NeedUpdateLifeActivityId = true;
            }
        }
        #endregion

        #region Session data
        private readonly Dictionary<int, long> _dicSessionData = new Dictionary<int, long>();

        public string SessionData => CollectionUtils.Dictionary2String(_dicSessionData);

        public int AddSessionValue(long value)
        {
            int nextKey = _dicSessionData.Count + 1;
            _dicSessionData.Add(nextKey, value);

            return nextKey;
        }
        #endregion

        public bool Success { get; set; }

        public SpecialCommandForTelegramBot SpecialCommandForTelegramBot { get; internal set; } = SpecialCommandForTelegramBot.None;

        public string ResponseMessage { get; set; }

        public ChoiceItem[] ChoiceItemList { get; set; }

        public int ChoiceListShowColumnsCount { get; set; } = 0;

        public string ErrorText { get; set; } = "";

        public static ExecuteCommandResult CreateSuccessObject(string message)
        {
            return new ExecuteCommandResult
            {
                Success = true,
                ResponseMessage = message
            };
        }

        public static ExecuteCommandResult CreateErrorObject(string message)
        {
            return new ExecuteCommandResult
            {
                Success = false,
                ErrorText = message
            };
        }

        public override string ToString()
        {
            StringBuilder sbTrace = new StringBuilder();
            if (Success)
            {
                if (!string.IsNullOrEmpty(ResponseMessage)) sbTrace.Append(ResponseMessage);
                if (ChoiceItemList != null && ChoiceItemList.Length > 0)
                {
                    sbTrace.AppendLine();
                    foreach (ChoiceItem item in ChoiceItemList)
                    {
                        sbTrace.AppendLine($"{item.Command} = {item.Text}");
                    }
                }
            }
            else
            {
                sbTrace.Append(ErrorText);
            }
            return sbTrace.ToString();
        }
    }
}
