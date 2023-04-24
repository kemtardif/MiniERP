namespace MiniERP.PurchaseOrderService.Models
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
        private Result(IDictionary<string, string[]> errors) : base(errors)
        {
            _value = default!;
        }
        private Result(T value) : base()
        {
            _value = value;
        }
        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(IDictionary<string, string[]> errors) => new(errors);
    }

    public abstract class Result
    {
        private readonly IDictionary<string, string[]> _errors;
        public IDictionary<string, string[]> Errors => new Dictionary<string, string[]>(_errors);
        public bool IsSuccess => _errors.Count == 0;
        protected Result()
        {
            _errors = new Dictionary<string, string[]>();
        }
        protected Result(IDictionary<string, string[]> errors)
        {
            _errors = errors;
        }
    }
}
