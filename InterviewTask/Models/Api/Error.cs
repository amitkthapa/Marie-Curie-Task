using Newtonsoft.Json;

namespace InterviewTask.Models.Api
{
    public class Error
    {
        [JsonProperty("cod")]
        public int ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}