// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using LrcParser.Model;
using LrcParser.Parser.Lines;
using LrcParser.Parser.Lrc.Metadata;
using LrcParser.Parser.Lrc.Utils;

namespace LrcParser.Parser.Lrc.Lines;

public class LrcLyricParser : SingleLineParser<LrcLyric>
{
    public override bool CanDecode(string text)
        => !string.IsNullOrWhiteSpace(text);

    public override LrcLyric Decode(string text)
    {
        var (startTimes, rawLyric) = LrcStartTimeUtils.SplitLyricAndTimeTag(text);

        // Word time tags can't be used if there are no or more than one line time tags
        if (startTimes.Length is 0 or > 1)
        {
            return new LrcLyric
            {
                Text = rawLyric,
                StartTimes = startTimes,
                TimeTags = new SortedDictionary<TextIndex, int>()
            };
        }

        var (lyric, timeTags) = LrcTimedTextUtils.TimedTextToObject(rawLyric, startTimes[0]);

        return new LrcLyric
        {
            Text = lyric,
            StartTimes = startTimes,
            TimeTags = timeTags,
        };
    }

    public override string Encode(LrcLyric component, int index)
    {
        var lyricWithTimeTag = LrcTimedTextUtils.ToTimedText(component.Text, component.TimeTags);
        return LrcStartTimeUtils.JoinLyricAndTimeTag(component.StartTimes, lyricWithTimeTag);
    }
}
