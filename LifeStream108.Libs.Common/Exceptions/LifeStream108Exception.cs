using System;

namespace LifeStream108.Libs.Common.Exceptions
{
    public class LifeStream108Exception : Exception
    {
        public ErrorType ErrorType { get; private set; }

        public string UserMessage { get; private set; }

        public string UserRequestDetails { get; private set; }

        public LifeStream108Exception(ErrorType errorType, string techMessage, string userMessage, string userRequestDetails)
            : base(techMessage)
        {
            ErrorType = errorType;
            UserMessage = userMessage;
            UserRequestDetails = userRequestDetails;
        }

        public override string ToString()
        {
            return $"{ErrorType}: {Message}, {UserMessage} [{UserRequestDetails}]";
        }
    }

    public enum ErrorType
    {
        LifeGroupNotFound,

        CommandHasNoCommandNames,

        CommandNameReferMoreThanOneCommands,

        CommandNameHasNoCommand,

        LifeActivityLogNotFound,

        LoadCommandProcessorClassError,

        SpecialTelegramCommandNotImplemented,

        UnknownFunction,

        WrangReminderSettings
    }
}
