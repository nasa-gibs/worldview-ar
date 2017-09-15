using UnityEngine;

namespace EVRTH.Scripts.Utility
{
    public enum ScrubberResolution { Day,Month,Year}
    public class DateScrubber : MonoBehaviour
    {
        public Preset preset;
        public ScrubberResolution resolution;
        public Date startDate;
        public Date endDate;
    }
}
