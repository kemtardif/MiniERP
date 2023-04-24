namespace MiniERP.SalesOrderService.Models
{
    public class Result<T> : ResultBase
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
        private Result(T value) : base()
        {
            _value = value;
        }
        private Result(IDictionary<string, string[]> errors) : base(errors)
        {
            _value = default!;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(IDictionary<string, string[]> errors) => new(errors);
    }

    public abstract class ResultBase
    {
        private readonly IDictionary<string, string[]> _errors;
        public IDictionary<string, string[]> Errors => new Dictionary<string, string[]>(_errors);
        public bool IsSuccess => _errors.Count == 0;

        protected ResultBase()
        {
            _errors = new Dictionary<string, string[]>();
        }
        protected ResultBase(IDictionary<string, string[]> errors)
        {
            _errors = errors;
        }
    }
}
