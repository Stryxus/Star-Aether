using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SA.Web.Shared.Json
{
    public class MediaPhotographyData
    {
        public List<MediaPhoto> Photos { get; set; }
    }

    public class MediaPhoto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string TakenGameVersion { get; set; }
        public DateTime TakenDate { get; set; }
        public List<string> Resolutions { get; set; }
    }
}
