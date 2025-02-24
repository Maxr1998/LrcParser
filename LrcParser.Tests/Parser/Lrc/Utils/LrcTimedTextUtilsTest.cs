// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using LrcParser.Parser.Lrc.Utils;
using LrcParser.Tests.Helper;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc.Utils;

public class LrcTimedTextUtilsTest
{
    #region Decode

    [TestCase("<00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22>", "帰り道は", new[] { "[0,start]:17970", "[1,start]:18370", "[2,start]:18550", "[3,start]:18940", "[3,end]:19220" })]
    [TestCase(" <00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22>", " 帰り道は", new[] { "[1,start]:17970", "[2,start]:18370", "[3,start]:18550", "[4,start]:18940", "[4,end]:19220" })]
    [TestCase("<00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22> ", "帰り道は ", new[] { "[0,start]:17970", "[1,start]:18370", "[2,start]:18550", "[3,start]:18940", "[3,end]:19220" })]
    [TestCase("帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22>", "帰り道は", new[] { "[1,start]:18370", "[2,start]:18550", "[3,start]:18940", "[3,end]:19220" })]
    [TestCase("<00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は", "帰り道は", new[] { "[0,start]:17970", "[1,start]:18370", "[2,start]:18550", "[3,start]:18940" })]
    [TestCase("帰り道は", "帰り道は", new string[] { })]
    [TestCase("", "", new string[] { })]
    [TestCase(null, "", new string[] { })]
    public void TestDecode(string text, string expectedText, string[] expectedTimeTags)
    {
        var (actualText, actualTimeTags) = LrcTimedTextUtils.TimedTextToObject(text);

        Assert.That(actualText, Is.EqualTo(expectedText));
        Assert.That(actualTimeTags, Is.EqualTo(TestCaseTagHelper.ParseTimeTags(expectedTimeTags)));
    }

    [TestCase("<00:51.00><01:29.99><01:48.29><02:31.00><02:41.99>You gotta fight !", "You gotta fight !", new[] { "[0,start]:51000" })] // decode with invalid format.
    public void TestDecodeWithInvalidFormat(string text, string expectedText, string[] expectedTimeTags)
    {
        var (actualText, actualTimeTags) = LrcTimedTextUtils.TimedTextToObject(text);

        Assert.That(actualText, Is.EqualTo(expectedText));
        Assert.That(actualTimeTags, Is.EqualTo(TestCaseTagHelper.ParseTimeTags(expectedTimeTags)));
    }

    [TestCase("<00:00.00>", 0)]
    [TestCase("<00:06.00>", 6000)]
    [TestCase("<01:00.00>", 60000)]
    [TestCase("<10:00.00>", 600000)]
    [TestCase("<100:00.00>", 6000000)]
    [TestCase("<12:34.567>", 754560)]
    [TestCase("<0:00.00>", 0)] // prevent throw error in some invalid format.
    [TestCase("<0:0.0>", 0)] // prevent throw error in some invalid format.
    [TestCase("<1:00.00><1:02.00>", 60000)] // rarely to get this case, so return the first one.
    public void TestConvertTimeTagToMillionSecond(string timeTag, int expectedMillionSecond)
    {
        var actual = LrcTimedTextUtils.ConvertTimeTagToMillionSecond(timeTag);

        Assert.That(actual, Is.EqualTo(expectedMillionSecond));
    }

    [TestCase("<--:--.-->")]
    [TestCase("<>")]
    [TestCase("[1:00.00]")] // should not contains start time-tag.
    public void TestConvertTimeTagToMillionSecondWithInvalidValue(string timeTag)
    {
        Assert.Throws<InvalidOperationException>(() => LrcTimedTextUtils.ConvertTimeTagToMillionSecond(timeTag));
    }

    #endregion

    #region Encode

    [TestCase("帰り道は", new[] { "[0,start]:17970", "[1,start]:18370", "[2,start]:18550", "[3,start]:18940", "[3,end]:19220" }, "<00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22>")]
    [TestCase(" 帰り道は", new[] { "[1,start]:17970", "[2,start]:18370", "[3,start]:18550", "[4,start]:18940", "[4,end]:19220" }, " <00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22>")]
    [TestCase("帰り道は ", new[] { "[0,start]:17970", "[1,start]:18370", "[2,start]:18550", "[3,start]:18940", "[3,end]:19220" }, "<00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22> ")]
    [TestCase("帰り道は", new[] { "[1,start]:18370", "[2,start]:18550", "[3,start]:18940", "[3,end]:19220" }, "帰<00:18.37>り<00:18.55>道<00:18.94>は<00:19.22>")]
    [TestCase("帰り道は", new[] { "[0,start]:17970", "[1,start]:18370", "[2,start]:18550", "[3,start]:18940" }, "<00:17.97>帰<00:18.37>り<00:18.55>道<00:18.94>は")]
    [TestCase("帰り道は", new string[] { }, "帰り道は")]
    [TestCase("", new string[] { }, "")]
    public void TestEncode(string text, string[] timeTags, string expected)
    {
        var actual = LrcTimedTextUtils.ToTimedText(text, TestCaseTagHelper.ParseTimeTags(timeTags));

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, "<00:00.00>")]
    [TestCase(6000, "<00:06.00>")]
    [TestCase(60000, "<01:00.00>")]
    [TestCase(600000, "<10:00.00>")]
    [TestCase(6000000, "<100:00.00>")]
    public void TestConvertMillionSecondToTimeTag(int millionSecond, string expectedTimeTag)
    {
        var actual = LrcTimedTextUtils.ConvertMillionSecondToTimeTag(millionSecond);

        Assert.That(actual, Is.EqualTo(expectedTimeTag));
    }

    [TestCase(-1)]
    public void TestConvertMillionSecondToTimeTagWithInvalidValue(int millionSecond)
    {
        Assert.Throws<InvalidOperationException>(() => LrcTimedTextUtils.ConvertMillionSecondToTimeTag(millionSecond));
    }

    #endregion
}
