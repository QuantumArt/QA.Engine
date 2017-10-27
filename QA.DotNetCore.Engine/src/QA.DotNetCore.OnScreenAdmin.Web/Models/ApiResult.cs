namespace QA.DotNetCore.OnScreenAdmin.Web.Models
{
    public class ApiResult<T> : ApiResult
    {
        public ApiResult(T data, string status, string errorText)
            : base(status, errorText)
        {
            Data = data;
        }

        public T Data { get; private set; }
    }

    public class ApiResult
    {
        public ApiResult(string status, string errorText)
        {
            Status = status;
            ErrorText = errorText;
        }

        public string Status { get; protected set; }

        public string ErrorText { get; protected set; }

        public static ApiResult Success()
        {
            return new ApiResult(ResultStatus.OK, null);
        }

        public static ApiResult<TData> Success<TData>(TData data)
        {
            return new ApiResult<TData>(data, ResultStatus.OK, null);
        }

        public static ApiResult Error(string errorText)
        {
            return new ApiResult(ResultStatus.Error, errorText);
        }

        public static ApiResult<TData> Error<TData>(TData data, string errorText)
        {
            return new ApiResult<TData>(data, ResultStatus.Error, errorText);
        }
    }

    public class ResultStatus
    {
        public const string OK = "OK";
        public const string Error = "Error";
    }
}
