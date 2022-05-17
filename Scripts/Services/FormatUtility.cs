using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameEngine
{
    public static class FormatUtility
    {
        public static string ToMd5Hash(string targetText)
        {
            var ue = new UTF8Encoding();
            var bytes = ue.GetBytes(targetText);
            var md5 = new MD5CryptoServiceProvider();
            var hashBytes = md5.ComputeHash(bytes);
            var hashString = "";
            for (var i = 0; i < hashBytes.Length; i++)
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            return hashString.PadLeft(32, '0');
        }

        public static string ToTimeString(int seconds, TimeFormats format = TimeFormats.Fully)
        {
            if (seconds < 0) seconds = 0;
            var days = 0;
            var hours = 0;
            var minutes = 0;
            var extraSeconds = false;

            if (seconds != 0)
            {
                if (format.HasFlag(TimeFormats.Days))
                {
                    days = Mathf.FloorToInt(seconds / 86400f);
                    if (days > 99) days = 99;
                    seconds -= days * 86400;
                }

                if (format.HasFlag(TimeFormats.Hours))
                {
                    hours = Mathf.FloorToInt(seconds / 3600f);
                    if (hours > 23) hours = 23;
                    seconds -= hours * 3600;
                }

                if (format.HasFlag(TimeFormats.Minutes))
                {
                    minutes = Mathf.FloorToInt(seconds / 60f);
                    if (minutes > 59) minutes = 59;
                    seconds -= minutes * 60;
                }

                if (seconds > 59)
                {
                    seconds = 59;
                    extraSeconds = true;
                }
            }

            var result = "";
            if (format.HasFlag(TimeFormats.Days)) result += (days < 10 ? "0" : "") + days + " ";
            if (format.HasFlag(TimeFormats.Hours)) result += (hours < 10 ? "0" : "") + hours + ":";
            if (format.HasFlag(TimeFormats.Minutes)) result += (minutes < 10 ? "0" : "") + minutes + ":";
            if (format.HasFlag(TimeFormats.Seconds)) result += (seconds < 10 ? "0" : "") + seconds + ":";
            result = result.Remove(result.Length - 1, 1);
            if (extraSeconds) result += "+";

            return result;
        }

        public static string ToDayString(int seconds)
        {
            return ToTimeString(seconds, TimeFormats.Days | TimeFormats.Hours | TimeFormats.Minutes);
        }

        public static string ToHourString(int seconds)
        {
            return ToTimeString(seconds, TimeFormats.Hours | TimeFormats.Minutes | TimeFormats.Seconds);
        }

        public static string ToMinuteString(int seconds)
        {
            return ToTimeString(seconds, TimeFormats.Minutes | TimeFormats.Seconds);
        }

        [Flags]
        public enum TimeFormats
        {
            Seconds = 1,
            Minutes = 2,
            Hours = 4,
            Days = 8,
            Fully = Days | Hours | Minutes | Seconds
        }
    }
}