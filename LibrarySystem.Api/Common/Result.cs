namespace LibrarySystem.Api.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public T ? Data { get; private set; }
        public IReadOnlyList<string> Errors { get; private set; } = Array.Empty<string>();
        public int StatusCode { get; private set; } = 200;
        private Result()
        {
            
        }

        public static Result<T> Success(T data, string message = "operation completed successfully ",int statusCode=200)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static Result<T> Failure( string message , params string[] errors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                StatusCode = 400
            };
        }

        public static Result<T> NotFound(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 404
            };
        }

        public static Result<T> Conflict(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 409
            };
        }


        public static Result<T> Unauthorized(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 401
            };
        }
    }
}
