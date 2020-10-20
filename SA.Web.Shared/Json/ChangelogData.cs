using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SA.Web.Shared.Json
{
    public class ChangelogData
    {
        public List<ChangelogEntryData> ChangelogPosts { get; set; }
    }

    public class ChangelogEntryData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public UpdateType? Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Additions { get; set; } = new List<string>();
        public List<string> Removals { get; set; } = new List<string>();
        public List<string> Changes { get; set; } = new List<string>();
        public List<string> Optimizations { get; set; } = new List<string>();
        public List<string> Fixes { get; set; } = new List<string>();
    }

    public enum UpdateType
    {
        Major,
        Minor,
        Patch
    }
}
