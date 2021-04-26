using System;
using System.Collections.Generic;

namespace UEESA.Client.Data.States.Roadmap
{
    public class RoadmapV1State
    {
        internal Dictionary<string, bool> Roadmap_v1_IsStatisticsVisible { get; set; } = new Dictionary<string, bool>();
        internal event Action Roadmap_v1_OnStatisticsVisibilityToggle;
        internal void Roadmap_v1_ToggleStatisticsVisibility(string roadmapCardVersion)
        {
            Roadmap_v1_IsStatisticsVisible[roadmapCardVersion] = !Roadmap_v1_IsStatisticsVisible[roadmapCardVersion];
            Roadmap_v1_OnStatisticsVisibilityToggle?.Invoke();
        }

        internal Dictionary<string, bool> Roadmap_v1_IsPatchNotesVisible { get; set; } = new Dictionary<string, bool>();
        internal event Action Roadmap_v1_OnPatchNotesVisibilityToggle;
        internal void Roadmap_v1_TogglePatchNotesVisibility(string roadmapCardVersion)
        {
            Roadmap_v1_IsPatchNotesVisible[roadmapCardVersion] = !Roadmap_v1_IsPatchNotesVisible[roadmapCardVersion];
            Roadmap_v1_OnPatchNotesVisibilityToggle?.Invoke();
        }

        internal Dictionary<string, DateTime> Roadmap_v1_SelectedDates { get; set; } = new Dictionary<string, DateTime>();
        internal event Action Roadmap_v1_OnSelectedDateChange;
        internal void Roadmap_v1_ChangeSelectedDate(string roadmapCardVersion, DateTime selectedDate)
        {
            Roadmap_v1_SelectedDates[roadmapCardVersion] = selectedDate;
            Roadmap_v1_OnSelectedDateChange?.Invoke();
        }
    }
}
