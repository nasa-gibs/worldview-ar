using System;
using UnityEngine;

namespace EVRTH.Scripts.Utility
{
    public class DateScrubber : MonoBehaviour
    {
        public LayerPresetLoader presetLoader;
        [Header("Start Date")]
        public Date startDate;
        [Space]
        [Header("End Date")]
        public Date endDate;

        public void SelectDate(DateTime selectedTime)
        {
            presetLoader.date.SetFromDateTime(selectedTime);
            presetLoader.ApplyPreset(presetLoader.currentPreset);
        }

        public void SelectDate(float percentage)
        {
            TimeSpan span = endDate.ToDateTime - startDate.ToDateTime;
            int days = (int) (span.TotalDays * percentage);
            presetLoader.date.SetFromDateTime(startDate.ToDateTime.AddDays(days));
            presetLoader.ApplyPreset(presetLoader.currentPreset);
        }
    }
}
