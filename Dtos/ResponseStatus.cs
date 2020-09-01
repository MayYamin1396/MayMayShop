using Newtonsoft.Json;

namespace MayMayShop.API.Dtos
{
    public class ResponseStatus
    {
        [JsonIgnore]
        public int StatusCode { get; set; }

        public string Message { get; set; }
    }
}
