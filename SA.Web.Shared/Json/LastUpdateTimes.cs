using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SA.Web.Shared.Json
{
    public class LastUpdateTimes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime RoadmapDataUpdate { get; set; }
        public DateTime NewsDataUpdate { get; set; }
        public DateTime ChangelogDataUpdate { get; set; }
        public DateTime PhotographyDataUpdate { get; set; }
        public DateTime VideographyDataUpdate { get; set; }
    }
}
