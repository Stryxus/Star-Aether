using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SA.Web.Shared.Json
{
    public class NewsData
    {
        public List<NewsEntryData> NewsPosts { get; set; }
    }

    public class NewsEntryData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime? LastEditTime { get; set; }
        public List<string> Content { get; set; } 
    }
}
