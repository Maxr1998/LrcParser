// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using LrcParser.Model;
using LrcParser.Utils;
using static LrcParser.Parser.Lrc.Utils.TimeTagMode;

namespace LrcParser.Parser.Lrc.Utils;

internal static class LrcTimedTextUtils
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="timedText"></param>
    /// <returns></returns>
    internal static Tuple<string, SortedDictionary<TextIndex, int>> TimedTextToObject(string timedText)
    {
        if (string.IsNullOrEmpty(timedText))
            return new Tuple<string, SortedDictionary<TextIndex, int>>("", new SortedDictionary<TextIndex, int>());

        var matchTimeTags = TimeTagUtils.WORD_TIME_TAG_REGEX.Matches(timedText);

        var endTextIndex = timedText.Length;

        var startIndex = 0;

        var text = string.Empty;
        var timeTags = new SortedDictionary<TextIndex, int>();

        foreach (var match in matchTimeTags.ToArray())
        {
            var endIndex = match.Index;

            if (startIndex < endIndex)
            {
                // add the text.
                text += timedText[startIndex..endIndex];
            }

            // update the new start for next time-tag calculation.
            startIndex = endIndex + match.Length;

            // add the time-tag.
            var hasText = startIndex < endTextIndex;
            var isEmptyStringNext = hasText && timedText[startIndex] == ' ';

            var state = hasText && !isEmptyStringNext ? IndexState.Start : IndexState.End;
            var textIndex = text.Length - (state == IndexState.Start ? 0 : 1);
            var time = TimeTagUtils.ConvertTimeTagToMilliseconds(match.Value, WordTimeTag);

            // using try add because it might be possible with duplicated time-tag position in the lyric.
            timeTags.TryAdd(new TextIndex(textIndex, state), time);
        }

        // should add remaining text at the right of the end time-tag.
        text += timedText[startIndex..endTextIndex];

        return new Tuple<string, SortedDictionary<TextIndex, int>>(text, timeTags);
    }

    internal static string ToTimedText(string text, SortedDictionary<TextIndex, int> timeTags)
    {
        var insertIndex = 0;

        var timedText = text;

        foreach (var (textIndex, time) in timeTags)
        {
            var timeTagString = TimeTagUtils.ConvertMillisecondsToTimeTag(time, WordTimeTag);
            var stringIndex = TextIndexUtils.ToGapIndex(textIndex);
            timedText = timedText.Insert(insertIndex + stringIndex, timeTagString);

            insertIndex += timeTagString.Length;
        }

        return timedText;
    }
}
