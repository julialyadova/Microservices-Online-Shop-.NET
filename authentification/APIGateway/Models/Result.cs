namespace APIGateway.Models
{
    public class Result<T>
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }

        public static Result<T> CreateSuccess(T value)
        {
            return new Result<T> { Successful = true, Value = value };
        }

        public static Result<T> CreateError(string message)
        {
            return new Result<T> { Successful = false, Message=message, Value = default};
        }
    }
}
