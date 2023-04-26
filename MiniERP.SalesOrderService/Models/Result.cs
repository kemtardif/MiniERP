using MiniERP.SalesOrderService.MessageBus.Messages;

namespace MiniERP.SalesOrderService.Models
{
    public class Result<T> : Result
    {
        private readonly T _value;
        public T Value
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("Cannot access Result on failed result");
                }
                return _value;
            }
        }
        private Result(IDictionary<string, string[]> errors, MessageBase? message = null) : base(errors, message)
        {
            _value = default!;
        }
        private Result(T value, MessageBase? message) : base(message)
        {
            _value = value;
        }
        public static Result<T> Success(T value, MessageBase? message = null) => new(value, message);
        public static Result<T> Failure(IDictionary<string, string[]> errors, MessageBase? message = null) => new(errors, message);
    }

    public abstract class Result
    {
        private readonly IDictionary<string, string[]> _errors;
        public IDictionary<string, string[]> Errors => new Dictionary<string, string[]>(_errors);
        private MessageBase? _message;

        public bool IsSuccess => _errors.Count == 0;
        public bool HasMessage => _message is not null;
        public MessageBase Message
        {
            get
            {
                if (!HasMessage)
                {
                    throw new InvalidOperationException("Cannot access Message when it is not present");
                }
                return _message!;
            }
        }


        protected Result(MessageBase? message = null)
        {
            _errors = new Dictionary<string, string[]>();
            _message = message;
        }
        protected Result(IDictionary<string, string[]> errors, MessageBase? message = null)
        {
            _errors = errors;
            _message = message;
        }
    }
}
