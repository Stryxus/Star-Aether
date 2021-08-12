namespace UEESA.Data.Json
{
    public class UEESA_Json_StateSocketDataCapsule<T>
    {
        public DateTime datetime_sent { get; set; } = DateTime.UtcNow;
        public List<string> attributes { get; set; }
        public string data_type { get; set; }
        public T? data { get; set; }

        public UEESA_Json_StateSocketDataCapsule()
        {
            data_type = typeof(T).Name;
        }
    }

    public enum StateSocketDataCapsuleAttributes
    {
        GetRoadmapData
    }
}
