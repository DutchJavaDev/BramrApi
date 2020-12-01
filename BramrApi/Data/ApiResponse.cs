using System.Collections.Generic;

namespace BramrApi.Data
{
    public class ApiResponse
    {
        private ApiResponse() { }

        public bool Success { get; private set; } = false;

        public string Message { get; private set; } = string.Empty;

        public object RequestedData { get; private set; } = string.Empty;

        public ICollection<string> Errors { get; private set; } = new List<string>();

        public static ApiResponse Oke(string message = "", object data = null)
        {
            return new ApiResponse { Success = true, Message = message, RequestedData = data };
        }

        public static ApiResponse Error(string message = "", object data = null, ICollection<string> errors = null)
        {
            return new ApiResponse { Success = false, Message = message, RequestedData = data, Errors = errors };
        }
    }
}
