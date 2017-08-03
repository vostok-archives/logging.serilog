using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vostok.Logging.Log4Net
{
    internal static class LogMessageFormatter
    {
        private static readonly Regex pattern = new Regex(@"(?<!{){@?(?<arg>[^ :{}]+)(?<format>:[^}]+)?}", RegexOptions.Compiled);

        public static string FormatStructuredMessage(string targetMessage, object[] formatParameters, out IEnumerable<string> patternMatches)
        {
            if (formatParameters.Length == 0)
            {
                patternMatches = Enumerable.Empty<string>();
                return targetMessage;
            }

            var processedArguments = new List<string>();
            patternMatches = processedArguments;

            foreach (Match match in pattern.Matches(targetMessage))
            {
                var arg = match.Groups["arg"].Value;

                int notUsed;
                if (!int.TryParse(arg, out notUsed))
                {
                    var argumentIndex = processedArguments.IndexOf(arg);
                    if (argumentIndex == -1)
                    {
                        argumentIndex = processedArguments.Count;
                        processedArguments.Add(arg);
                    }

                    targetMessage = ReplaceFirst(targetMessage, match.Value,
                                                 "{" + argumentIndex + match.Groups["format"].Value + "}");
                }
            }
            try
            {
                return string.Format(CultureInfo.InvariantCulture, targetMessage, formatParameters);
            }
            catch (FormatException ex)
            {
                throw new FormatException("The input string '" + targetMessage + "' could not be formatted using string.Format", ex);
            }
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}