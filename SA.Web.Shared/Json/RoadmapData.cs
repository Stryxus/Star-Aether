using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SA.Web.Shared.Json
{
    public class RoadmapData
    {
        public List<RoadmapCard> Cards { get; set; }
    }

    public class RoadmapCard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public List<RoadmapFeature> VersionFeatures { get; set; }
        public List<RoadmapCardChangelog> Patches { get; set; }
    }

    public class RoadmapCardVersions
    {
        public List<string> Versions { get; set; }
    }

    public enum RoadmapCardCategory : int
    {
        Characters,
        Locations,
        AI,
        Gameplay,
        ShipsAndVehicles,
        WeaponsAndItems,
        CoreTech
    }

    public class RoadmapCardChangelog
    {
        public int PatchVersion { get; set; }
        public string VersionString { get; set; }
        public DateTime? PUReleaseDate { get; set; }
        public DateTime? EvocatiTestingDate { get; set; }
        public DateTime? PTUTestingStartDate { get; set; }
        public string SpectrumPatchNotes { get; set; }
    }

    public class RoadmapFeature
    {
        public string Title { get; set; }
        public RoadmapCardCategory Category { get; set; }
        public Dictionary<DateTime, RoadmapFeatureStatus> Status { get; set; }
        public Dictionary<DateTime, int> TaskCount { get; set; }
        public Dictionary<DateTime, int> TasksCompleted { get; set; }
        public string Description { get; set; }
    }

    public enum RoadmapFeatureStatus : int
    {
        Scheduled,
        InDevelopment,
        Polishing,
        Released
    }
}
