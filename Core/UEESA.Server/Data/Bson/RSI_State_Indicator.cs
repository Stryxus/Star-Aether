using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UEESA.Server.Data.Bson
{
    public class RSI_State_Indicator
    {
        public ObjectId Id { get; set; }
        [BsonDateTimeOptions(DateOnly = false, Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime change_date { get; set; }
        public string live_version { get; set; }
        public string ptu_version { get; set; }
    }
}
