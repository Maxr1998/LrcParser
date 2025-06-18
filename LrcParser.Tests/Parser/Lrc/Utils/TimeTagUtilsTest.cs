// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using LrcParser.Parser.Lrc.Utils;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc.Utils;

public class TimeTagUtilsTest
{
    #region Decode

    [TestCase("[00:00.00]", 0)]
    [TestCase("[00:06.00]", 6000)]
    [TestCase("[01:00.00]", 60000)]
    [TestCase("[10:00.00]", 600000)]
    [TestCase("[100:00.00]", 6000000)]
    [TestCase("[12:34.567]", 754567)]
    [TestCase("[0:00.00]", 0)]
    [TestCase("[1:00.00][1:02.00]", 60000)] // return the first tag's value if multiple tags are provided.
    public void TestConvertTimeTagToLineMilliseconds(string timeTag, int expectedMilliseconds)
    {
        var actual = TimeTagUtils.ConvertTimeTagToMilliseconds(timeTag, TimeTagMode.LineTimeTag);

        Assert.That(actual, Is.EqualTo(expectedMilliseconds));
    }

    [TestCase("[--:--.--]")]
    [TestCase("[]")]
    [TestCase("<1:00.00>")] // fail when parsing a word time tag as line time tag
    public void TestConvertLineTimeTagToMillisecondsWithInvalidValue(string timeTag)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertTimeTagToMilliseconds(timeTag, TimeTagMode.LineTimeTag));
    }

    [TestCase("<00:00.00>", 0)]
    [TestCase("<00:06.00>", 6000)]
    [TestCase("<01:00.00>", 60000)]
    [TestCase("<10:00.00>", 600000)]
    [TestCase("<100:00.00>", 6000000)]
    [TestCase("<12:34.567>", 754567)]
    [TestCase("<0:00.00>", 0)] // prevent throw error in some invalid format.
    [TestCase("<1:00.00><1:02.00>", 60000)] // return the first tag's value if multiple tags are provided.
    public void TestConvertTimeTagToWordMilliseconds(string timeTag, int expectedMilliseconds)
    {
        var actual = TimeTagUtils.ConvertTimeTagToMilliseconds(timeTag, TimeTagMode.WordTimeTag);

        Assert.That(actual, Is.EqualTo(expectedMilliseconds));
    }

    [TestCase("<--:--.-->")]
    [TestCase("<>")]
    [TestCase("[1:00.00]")] // fail when parsing a line time tag as word time tag
    public void TestConvertWordTimeTagToMillisecondsWithInvalidValue(string timeTag)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertTimeTagToMilliseconds(timeTag, TimeTagMode.WordTimeTag));
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
        var actual = TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, TimeTagMode.LineTimeTag);

        Assert.That(actual, Is.EqualTo(expectedTimeTag));
    }

    [TestCase(-1)]
    public void TestConvertMillisecondsToLineTimeTagWithInvalidValue(int milliseconds)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, TimeTagMode.LineTimeTag));
    }

    [TestCase(0, "<00:00.00>")]
    [TestCase(6000, "<00:06.00>")]
    [TestCase(60000, "<01:00.00>")]
    [TestCase(600000, "<10:00.00>")]
    [TestCase(6000000, "<100:00.00>")]
    public void TestConvertMillisecondsToWordTimeTag(int milliseconds, string expectedTimeTag)
    {
        var actual = TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, TimeTagMode.WordTimeTag);

        Assert.That(actual, Is.EqualTo(expectedTimeTag));
    }

    [TestCase(-1)]
    public void TestConvertMillisecondsToWordTimeTagWithInvalidValue(int milliseconds)
    {
        Assert.Throws<InvalidOperationException>(() => TimeTagUtils.ConvertMillisecondsToTimeTag(milliseconds, TimeTagMode.WordTimeTag));
    }

    #endregion
}
