namespace ProductShop.DTO
{
    using Newtonsoft.Json;

    [JsonObject]
    public class ExportProductModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("seller")]
        public string Seller { get; set; }
    }
}
