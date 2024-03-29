using System.Text.Json.Serialization;

namespace OrderApi.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("ApplicableTo")]
        public string ApplicableTo { get; set; }
    }
}
