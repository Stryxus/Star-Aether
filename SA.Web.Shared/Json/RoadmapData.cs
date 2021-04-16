using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SA.Web.Shared.Json
{
    public class RoadmapData
    {
        public List<RoadmapCardData> Cards { get; set; }
    }

    public class RoadmapCardData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public List<RoadmapFeatureData> VersionFeatures { get; set; }
        public List<RoadmapCardChangelogData> Patches { get; set; }
    }

    public class RoadmapCardVersionsData
    {
        public List<string> Versions { get; set; }
    }

    public enum RoadmapCardCategoryData : int
    {
        Characters,
        Locations,
        AI,
        Gameplay,
        ShipsAndVehicles,
        WeaponsAndItems,
        CoreTech
    }

    public class RoadmapCardChangelogData
    {
        public int PatchVersion { get; set; }
        public string VersionString { get; set; }
        public DateTime? PUReleaseDate { get; set; }
        public DateTime? EvocatiTestingDate { get; set; }
        public DateTime? PTUTestingStartDate { get; set; }
        public string SpectrumPatchNotes { get; set; }
    }

    public class RoadmapFeatureData
    {
        public string Title { get; set; }
        public RoadmapCardCategoryData Category { get; set; }
        public Dictionary<DateTime, RoadmapFeatureStatusData> Status { get; set; }
        public Dictionary<DateTime, int> TaskCount { get; set; }
        public Dictionary<DateTime, int> TasksCompleted { get; set; }
        public string Description { get; set; }
    }

    public enum RoadmapFeatureStatusData : int
    {
        Scheduled,
        InDevelopment,
        Polishing,
        Released
    }
}
