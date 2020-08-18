using System;
using Newtonsoft.Json;

namespace OperationResult.Errors
{
    /// <summary>
    /// Base error type for OperationResult
    /// </summary>
    public class Error
    {
#if !DEBUG
        [JsonIgnore]
#endif
        public Exception Exception { get; set; }
        public string Message { get; }
        public int? ErrorCode { get; }

        public Error(string message, int? errorCode = null)
        {
            Message = message;
            ErrorCode = errorCode;
        }

        [JsonConstructor]
        public Error(string message, Exception ex, int? errorCode = null)
        {
            Message = message;
            ErrorCode = errorCode;
            Exception = ex;
        }

        public static Error Create(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            return new Error(ex.ToString(), ex);
        }

        public static Error Create(string message) => new Error(message);

        public static Error Create(string message, int? errorCode) => new Error(message, errorCode);

        public override string ToString() => $"ErrorCode:{ErrorCode ?? 0}|{Message}";
    }
}
