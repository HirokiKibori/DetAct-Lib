using Pidgin;

namespace DetAct.Interpreter
{
    public enum InterpreterMessageType
    {
        WARNING,
        ERROR
    }

    public abstract class InterpreterMessage
    {
        public InterpreterMessageType MessageType { get; }

        public string Message { get; }

        public SourcePos Position { get; }

        public InterpreterMessage(InterpreterMessageType messageType, string message, SourcePos position)
            => (MessageType, Message, Position) = (messageType, message, position);
    }

    public class ErrorMessage : InterpreterMessage
    {
        public ErrorMessage(string message, SourcePos position)
            : base(InterpreterMessageType.ERROR, message, position) { }

        public ErrorMessage(string message)
            : this(message, new(0, 1)) { }
    }

    public class WarningMessage : InterpreterMessage
    {
        public WarningMessage(string message, SourcePos position)
            : base(InterpreterMessageType.WARNING, message, position) { }

        public WarningMessage(string message)
            : this(message, new(0, 1)) { }
    }
}
