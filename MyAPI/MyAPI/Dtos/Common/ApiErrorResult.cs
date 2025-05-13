namespace MyAPI.Dtos.Common
{
    public class ApiErrorResult<T> : ApiResult<T>
    {
        public List<string> Errors { set; get; }
        public ApiErrorResult(int statusCode, string message)
          : base(statusCode, false, message)
        {
        }

        public ApiErrorResult(int statusCode, List<string> errors)
        : base(statusCode, false)
        {
            Errors = errors;
        }
    }
}
