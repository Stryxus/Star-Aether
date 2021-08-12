namespace UEESA.Shared.Data.Bson.Roadmap
{
    public class UEESA_Bson_Roadmap_Card
    {
        public string name { get; set; }
        public string description { get; set; }
        public UEESA_Bson_Roadmap_Card_Category category { get; set; }
        public DateTime creation_datetime { get; set; }
        public DateTime updated_datetime { get; set; }
        public string thumnail_path { get; set; }
        public RSI_Bson_Roadmap_Card_Status status { get; set; }
        public bool has_released { get; set; }
        public List<string> teams { get; set; }
    }
}
