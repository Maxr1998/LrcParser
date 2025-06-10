// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using LrcParser.Parser.Lrc.Utils;
using NUnit.Framework;
using static LrcParser.Parser.Lrc.Utils.TimeTagMode;

namespace LrcParser.Tests.Parser.Lrc.Utils;

public class TimeTagUtilsTest
{
    #region Decode

    // LINE TIME TAG TESTS
    [TestCase("[00:00.00]", LineTimeTag, 0)]
    [TestCase("[00:06.00]", LineTimeTag, 6000)]
    [TestCase("[01:00.00]", LineTimeTag, 60000)]
    [TestCase("[10:00.00]", LineTimeTag, 600000)]
    [TestCase("[100:00.00]", LineTimeTag, 6000000)]
    [TestCase("[12:34.567]", LineTimeTag, 754567)]
    [TestCase("[0:00.00]", LineTimeTag, 0)]
    [TestCase("[1:00.00][1:02.00]", LineTimeTag, 60000)] // return the first tag's value if multiple tags are provided.
    // WORD TIME TAG TESTS
    [TestCase("<00:00.00>", WordTimeTag, 0)]
    [TestCase("<00:06.00>", WordTimeTag, 6000)]
    [TestCase("<01:00.00>", WordTimeTag, 60000)]
    [TestCase("<10:00.00>", WordTimeTag, 600000)]
    [TestCase("<100:00.00>", WordTimeTag, 6000000)]
    [TestCase("<12:34.567>", WordTimeTag, 754567)]
    [TestCase("<0:00.00>", WordTimeTag, 0)] // prevent throw error in some invalid format.
    [TestCase("<1:00.00><1:02.00>", WordTimeTag, 60000)] // return the first tag's value if multiple tags are provided.
    public void TestConvertTimeTagToMilliseconds(string timeTag, TimeTagMode mode, int expectedMilliseconds)
    {
        var actual = TimeTagUtils.ConvertTimeTagToMilliseconds(timeTag, mode);

        Assert.That(actual, Is.EqualTo(expectedMilliseconds));
    }

    // LINE TIME TAG TESTS
    [TestCase("[--:--.--]", LineTimeTag)]
    [TestCase("[]", LineTimeTag)]
    [TestCase("<1:00.00>", LineTimeTag)] // fail when parsing a word time tag as line time tag
    // WORD TIME TAG TESTS
    [TestCase("<--:--.-->", WordTimeTag)]
    [TestCase("<>", WordTimeTag)]
    [TestCase("[1:00.00]", WordTimeTag)] // fail when parsing a line time tag as word time tag
    public void TestConvertTimeTagToMillisecondsWithInvalidValue(string timeTag, TimeTagMode mode)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertTimeTagToMilliseconds(timeTag, mode));
    }

    #endregion

    #region Encode

    [TestCase(0, "[00:00.00]")]
    [TestCase(6000, "[00:06.00]")]
    [TestCase(60000, "[01:00.00]")]
    [TestCase(600000, "[10:00.00]")]
    [TestCase(6000000, "[100:00.00]")]
    public void TestConvertMillisecondsToLineTimeTag(int milliseconds, string expectedTimeTag)
    {
        var actual = TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, LineTimeTag);

        Assert.That(actual, Is.EqualTo(expectedTimeTag));
    }

    [TestCase(-1)]
    public void TestConvertMillisecondsToLineTimeTagWithInvalidValue(int milliseconds)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, LineTimeTag));
    }

    [TestCase(0, "<00:00.00>")]
    [TestCase(6000, "<00:06.00>")]
    [TestCase(60000, "<01:00.00>")]
    [TestCase(600000, "<10:00.00>")]
    [TestCase(6000000, "<100:00.00>")]
    public void TestConvertMillisecondsToWordTimeTag(int milliseconds, string expectedTimeTag)
    {
        var actual = TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, WordTimeTag);

        Assert.That(actual, Is.EqualTo(expectedTimeTag));
    }

    [TestCase(-1)]
    public void TestConvertMillisecondsToWordTimeTagWithInvalidValue(int milliseconds)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, WordTimeTag));
    }

    #endregion
}
