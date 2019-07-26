using System;
using UnityEngine;
using UnityEngine.UI;

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

        [Space]
        [Space]
        [Header("Display")]
        public Text currDateText;

        public Text nextDateText;

        public Slider scrubberSlider;
        public Image scrubberBackground;
        public Image scrubberFill;
        public Image scrubberHandle;
        public Image updateButton;

		private void Start()
		{
            DateTime today = DateTime.Today;
            startDate.SetFromDateTime(presetLoader.date.ToDateTime);
            endDate.SetFromDateTime(today.AddDays(-1));
            currDateText.text = string.Format("{0:MM/dd/yyyy}", presetLoader.date.ToDateTime);
            ShowNextDate(0);
        }

		private void Update()
		{
            SetScrubberColor();
		}

        private void SetScrubberColor(){
            if((currDateText.text == nextDateText.text && scrubberFill.color == Color.green) ||
               (currDateText.text != nextDateText.text && scrubberFill.color == Color.red)){
                return;
            }
            Color sliderColor = Color.green;
            Color buttonColor = Color.grey;
            if (currDateText.text != nextDateText.text)
            {
                sliderColor = Color.red;
                buttonColor = Color.green;
            }
            scrubberBackground.color = sliderColor;
            scrubberFill.color = sliderColor;
            scrubberHandle.color = sliderColor;
            updateButton.color = buttonColor;
        }

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
            currDateText.text = string.Format("{0:MM/dd/yyyy}", presetLoader.date.ToDateTime);
        }

        public void ShowNextDate(float percentage)
        {
            TimeSpan span = endDate.ToDateTime - startDate.ToDateTime;
            int days = (int)(span.TotalDays * percentage);
            nextDateText.text = string.Format("{0:MM/dd/yyyy}", startDate.ToDateTime.AddDays(days));
        }

        public void GoToNextDate()
        {
            SelectDate(scrubberSlider.value);
        }
    }
}
