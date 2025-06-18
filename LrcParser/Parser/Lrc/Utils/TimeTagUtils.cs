using System.Text.RegularExpressions;

namespace LrcParser.Parser.Lrc.Utils;

internal static class TimeTagUtils
{
    internal static readonly Regex LINE_TIME_TAG_REGEX = new(@"\[(\d{1,}):(\d{2})\.(\d{2,3})\]");
    internal static readonly Regex WORD_TIME_TAG_REGEX = new(@"\<(\d{1,}):(\d{2})\.(\d{2,3})\>");

    /// <summary>
    /// Convert the given time tag to milliseconds.
    /// Both line time tag and word time tag are supported, as given by the mode.
    /// </summary>
    /// <param name="timeTag">
    /// A time tag string of the form [01:00.00] or &lt;01:00.00&gt;.
    /// Time tags using milliseconds are also supported.
    /// </param>
    /// <param name="mode">The mode of the time tag, which should match the passed time tag string.</param>
    /// <returns></returns>
    internal static int ConvertTimeTagToMilliseconds(string timeTag, TimeTagMode mode)
    {
        Regex regex = mode switch
        {
            TimeTagMode.LineTimeTag => LINE_TIME_TAG_REGEX,
            TimeTagMode.WordTimeTag => WORD_TIME_TAG_REGEX,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };

        Match match = regex.Match(timeTag);

        if (!match.Success)
        {
            throw new InvalidOperationException($"Invalid time tag format. Expected [01:00.00] or <01:00.00> but got {timeTag}.");
        }

        int minutes = int.Parse(match.Groups[1].Value);
        int seconds = int.Parse(match.Groups[2].Value);
        string decimalString = match.Groups[3].Value;
        int decimalPart = int.Parse(decimalString);
        int millis = decimalString.Length > 2 ? decimalPart : decimalPart * 10;

        return (minutes * 60 + seconds) * 1000 + millis;
    }

    /// <summary>
    /// Converts a time value in milliseconds to a time tag string.
    /// The returned string uses the format corresponding to the given mode:
    /// [mm:ss.xx] for line time tags and &lt;mm:ss.xx&gt; for word time tags.
    /// </summary>
    /// <param name="milliseconds">The time value in milliseconds. Must be non-negative.</param>
    /// <param name="mode">The mode to determine the tag format.</param>
    /// <returns>
    /// A time tag string formatted according to the specified mode.
    /// </returns>
    internal static string ConvertMillisecondsToTimeTag(int milliseconds, TimeTagMode mode)
    {
        if (milliseconds < 0)
        {
            throw new InvalidOperationException($"{nameof(milliseconds)} should be greater than 0.");
        }

        int totalSeconds = milliseconds / 1000;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        int hundredths = milliseconds % 1000 / 10;

        return mode switch
        {
            TimeTagMode.LineTimeTag => $"[{minutes:D2}:{seconds:D2}.{hundredths:D2}]",
            TimeTagMode.WordTimeTag => $"<{minutes:D2}:{seconds:D2}.{hundredths:D2}>",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }
}

/// <summary>
/// The time tag mode used for <see cref="TimeTagUtils"/>.
/// Must be public for tests.
/// </summary>
public enum TimeTagMode
{
    LineTimeTag,
    WordTimeTag,
}
