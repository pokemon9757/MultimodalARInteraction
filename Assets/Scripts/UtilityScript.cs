using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace MMI
{
    public static class UtilityScript
    {
        /// <summary>
        /// Get slot value from voiceEvent.EventName
        /// </summary>
        /// <param name="inputString">event name from ML voice event</param>
        /// <param name="keyword">Data slot</param>
        /// <returns>Slot value, null if not found</returns>

        public static string GetSlotValue(string inputString, string keyword)
        {
            string pattern = $@"{{\s*{keyword}\s*(.*?)}}"; // Regular expression pattern
            Match match = Regex.Match(inputString, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                // Return the captured group value
                return match.Groups[1].Value.Trim();
            }
            // Keyword not found in the string
            return null;
        }
        /// <summary>
        /// Convert a string to color
        /// </summary>
        /// <param name="colorName">String of color name</param>
        /// <returns>A color, grey by default if the color name is invalid</returns>
        public static Color? StringToColor(string colorName)
        {
            Color color;
            if (!ColorUtility.TryParseHtmlString(colorName.ToLower(), out color))
            {
                Debug.LogError("Invalid color name " + colorName);
                return null;
            }
            return color;
        }

    }
}