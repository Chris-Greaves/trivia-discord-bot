using System.Text.Json.Serialization;

namespace BasicDiscordBot
{
    public class TriviaResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }
        public TriviaResult[] Results { get; set; }
    }
}
