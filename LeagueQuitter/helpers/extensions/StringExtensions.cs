using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LeagueQuitter.helpers.extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parses strings from .txt files to a specific (string, int[]) format
        /// </summary>
        /// <param name="s">string to be parsed</param>
        /// <returns>A KeyValuePair of a string and integer array</returns>
        /// <exception cref="FormatException">Throws when an invalid keyvalue pair is found</exception>
        /// <exception cref="ArgumentException">Throws when the provided string can't be parsed</exception>
        public static (string, int[]) Parse(this string s) {
            return GetParsedString(s);
        }

        /// <summary>
        /// Parses strings from .txt files to a specific (string, int[]) format
        /// </summary>
        /// <param name="s">collection of strings to be parsed</param>
        /// <returns>An array of a KeyValuePair of a string and integer array</returns>
        /// <exception cref="FormatException">Throws when an invalid keyvalue pair is found</exception>
        public static (string, int[])[] Parse(this string[] s) {
            List<(string, int[])> SIArray = new List<(string, int[])>();
            
            foreach(var s1 in s) {
                try {
                    SIArray.Add(Parse(s1));
                } catch(ArgumentException) {
                    continue;
                }
            }

            return SIArray.ToArray();
        }

        private static (string, int[]) GetParsedString(string s) {
            s = s.TrimEnd('\n', '\r');
            var match = Regex.Match(s, "^>(.*?)((?:\\d+)(?:,\\d+)*)$");

            if(match.Value == string.Empty)
                throw new ArgumentException($"Given string {s} cannot be parsed to (string, int[])");

            var memberName = Regex.Match(match.Groups[1].Value, "[a-zA-Z]+").Value;

            if(memberName == string.Empty) {
                throw new FormatException($"{memberName} is an invalid categoryname, " +
                    "it should only contain letters.");
            }

            var values = Array.ConvertAll(match.Groups[2].Value.Split(','), int.Parse);

            return (memberName, values);
        }
    }
}
