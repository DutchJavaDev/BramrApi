using System.Collections.Generic;
using System.Text;

namespace BramrApi.Data
{
    public class ApiResponse
    {
        public ApiResponse() { }

        public bool Success { get; private set; } = false;

        public string Message { get; private set; } = string.Empty;

        public Dictionary<string,object> RequestedData { get; private set; } = new Dictionary<string, object>();

        public ICollection<string> Errors { get; private set; } = new List<string>();

        public ApiResponse AddData(string key, object value)
        {
            RequestedData.Add(key, value);
            return this;
        }

        public T GetData<T>(string key)
        {
            return (T) RequestedData[key] ?? default;
        }

        public static ApiResponse Oke(string message = "")
        {
            return new ApiResponse { Success = true, Message = message};
        }

        public static ApiResponse Error(string message = "", ICollection<string> errors = null)
        {
            return new ApiResponse { Success = false, Message = message, Errors = errors };
        }
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Sucess: {Success}");
            builder.AppendLine($"Message: {Message}");
            builder.AppendLine("\n[Errors]");

            foreach (var error in Errors)
            {
                builder.AppendLine($"{error}");
            }

            return $"{Success} {Message} {RequestedData}";
        }

    }
}
