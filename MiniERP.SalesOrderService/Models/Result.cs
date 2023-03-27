namespace MiniERP.SalesOrderService.Models
{
    public class Result<T>
    {
        private readonly IDictionary<string, string[]> _errors;
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
        public IDictionary<string, string[]> Errors => new Dictionary<string, string[]>(_errors);
        public bool IsSuccess => _errors.Count == 0;

        public Result(T value)
        {
            _value = value;
            _errors = new Dictionary<string, string[]>();
        }
        public Result(IDictionary<string, string[]> error)
        {
            _value = default!;
            _errors = error;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(IDictionary<string, string[]> errors) => new(errors);
    }

    public class Result
    {
        private readonly IDictionary<string, string[]> _errors;
        public IDictionary<string, string[]> Errors => new Dictionary<string, string[]>(_errors);
        public bool IsSuccess => _errors.Count == 0;

        public Result()
        {
            _errors = new Dictionary<string, string[]>();
        }
        public Result(IDictionary<string, string[]> error)
        {
            _errors = error;
        }

        public static Result Success() => new();
        public static Result Failure(IDictionary<string, string[]> errors) => new(errors);
    }
}
