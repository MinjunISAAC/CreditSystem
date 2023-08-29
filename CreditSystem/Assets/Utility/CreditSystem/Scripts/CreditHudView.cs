// ----- C#
using System;

// ----- Unity
using UnityEngine;
using TMPro;

namespace Utility.ForCredit.UI
{
    public class CreditHudView : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private TextMeshProUGUI _TMP_value = null;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Public
        public void RefreshCreditValue(int value, bool isMax = false)
        {
            if (isMax)
            {
                _TMP_value.text = $"MAX";
                return;
            }

            _TMP_value.text = _FormatAssetValue(value);
        }

        // ----- Private
        private string _FormatAssetValue(int value)
        {
            string[] units          = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };
            string   formattedValue = $"{value:0.##}";

            if (value >= 1000)
            {
                int orderOfMagnitude = (int)Math.Floor(Math.Log10(value));
                int unitIndex        = orderOfMagnitude / 3;

                if (unitIndex < units.Length)
                    formattedValue = $"{(value / Math.Pow(10, unitIndex * 3)):0.##}{units[unitIndex]}";
            }

            return formattedValue;
        }
    }
}
