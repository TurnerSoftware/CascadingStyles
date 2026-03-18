using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.CascadingStyles.Parsing;

namespace TurnerSoftware.CascadingStyles.Tests
{
	[TestClass]
	public class CssTokenizerTests
	{
		[TestMethod]
		public void NextToken_NoTokens()
		{
			var tokenizer = new CssTokenizer(string.Empty.AsMemory());
			Assert.IsFalse(tokenizer.NextToken(out _));
		}

		[TestMethod]
		public void Whitespace_Space()
		{
			var tokenizer = new CssTokenizer(" ".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual(" ", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_Tab()
		{
			var tokenizer = new CssTokenizer("\t".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\t", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_CarriageReturn()
		{
			var tokenizer = new CssTokenizer("\r".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\r", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_NewLine()
		{
			var tokenizer = new CssTokenizer("\n".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\n", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_FormFeed()
		{
			var tokenizer = new CssTokenizer("\f".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\f", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_Mixed_LeadingIdentifier()
		{
			var tokenizer = new CssTokenizer("mixed  ".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out _));
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("  ", token.RawValue.ToString());
		}

		[TestMethod]
		public void CDO_Partial_1()
		{
			var tokenizer = new CssTokenizer("<".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Delimiter, token.Type);
			Assert.AreEqual("<", token.RawValue.ToString());
		}
		[TestMethod]
		public void CDO_Partial_2()
		{
			var tokenizer = new CssTokenizer("<!".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Delimiter, token.Type);
			Assert.AreEqual("<", token.RawValue.ToString());
		}
		[TestMethod]
		public void CDO_Partial_3()
		{
			var tokenizer = new CssTokenizer("<!-".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Delimiter, token.Type);
			Assert.AreEqual("<", token.RawValue.ToString());
		}
		[TestMethod]
		public void CDO_Valid()
		{
			var tokenizer = new CssTokenizer("<!--".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.CDO, token.Type);
		}

		[TestMethod]
		public void SimpleToken_LeftParenthesis()
		{
			var tokenizer = new CssTokenizer("(".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.LeftParenthesis, token.Type);
		}
		[TestMethod]
		public void SimpleToken_RightParenthesis()
		{
			var tokenizer = new CssTokenizer(")".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.RightParenthesis, token.Type);
		}
		[TestMethod]
		public void SimpleToken_Comma()
		{
			var tokenizer = new CssTokenizer(",".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Comma, token.Type);
		}
		[TestMethod]
		public void SimpleToken_Colon()
		{
			var tokenizer = new CssTokenizer(":".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Colon, token.Type);
		}
		[TestMethod]
		public void SimpleToken_Semicolon()
		{
			var tokenizer = new CssTokenizer(";".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Semicolon, token.Type);
		}
		[TestMethod]
		public void SimpleToken_LeftSquareBracket()
		{
			var tokenizer = new CssTokenizer("[".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.LeftSquareBracket, token.Type);
		}
		[TestMethod]
		public void SimpleToken_RightSquareBracket()
		{
			var tokenizer = new CssTokenizer("]".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.RightSquareBracket, token.Type);
		}
		[TestMethod]
		public void SimpleToken_LeftCurlyBracket()
		{
			var tokenizer = new CssTokenizer("{".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.LeftCurlyBracket, token.Type);
		}
		[TestMethod]
		public void SimpleToken_RightCurlyBracket()
		{
			var tokenizer = new CssTokenizer("}".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.RightCurlyBracket, token.Type);
		}

		[TestMethod]
		public void String_Good()
		{
			var tokenizer = new CssTokenizer("\"Hello World\"".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.String, token.Type);
			Assert.AreEqual("Hello World", token.RawValue.ToString());
		}

		[TestMethod]
		public void BadString_NewLine()
		{
			var tokenizer = new CssTokenizer("\"Hello\nWorld\"".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadString, token.Type);
			Assert.AreEqual(string.Empty, token.RawValue.ToString());
		}

		[TestMethod]
		public void Number_Integer()
		{
			var tokenizer = new CssTokenizer("1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Integer, token.Flags);
			Assert.AreEqual("1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Integer_LeadingPlus()
		{
			var tokenizer = new CssTokenizer("+1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Integer, token.Flags);
			Assert.AreEqual("+1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Integer_LeadingMinus()
		{
			var tokenizer = new CssTokenizer("-1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Integer, token.Flags);
			Assert.AreEqual("-1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Float_SingleDigit()
		{
			var tokenizer = new CssTokenizer("1.0".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1.0", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Float_MultiDigit()
		{
			var tokenizer = new CssTokenizer("1.01".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1.01", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Lower_Positive()
		{
			var tokenizer = new CssTokenizer("1e+1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1e+1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Lower_Negative()
		{
			var tokenizer = new CssTokenizer("1e-1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1e-1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Upper_Positive()
		{
			var tokenizer = new CssTokenizer("1E+1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1E+1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Upper_Negative()
		{
			var tokenizer = new CssTokenizer("1E-1".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1E-1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_MultiDigit()
		{
			var tokenizer = new CssTokenizer("1e+100".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1e+100", token.RawValue.ToString());
		}

		[TestMethod]
		public void Url_Common()
		{
			var tokenizer = new CssTokenizer("url(http://example.org/)".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_WrappedDoubleQuote()
		{
			var tokenizer = new CssTokenizer("url(\"http://example.org/\")".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Function, token.Type);
			Assert.AreEqual("url", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_WrappedSingleQuote()
		{
			var tokenizer = new CssTokenizer("url('http://example.org/')".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Function, token.Type);
			Assert.AreEqual("url", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_LeadingWhitespace()
		{
			var tokenizer = new CssTokenizer("url(  http://example.org/)".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_TrailingWhitespace()
		{
			var tokenizer = new CssTokenizer("url(http://example.org/  )".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_EmptyUrl()
		{
			var tokenizer = new CssTokenizer("url()".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual(string.Empty, token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_EOF()
		{
			var tokenizer = new CssTokenizer("url(http://example.org/".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_Invalid_Whitespace()
		{
			var tokenizer = new CssTokenizer("url(http:// example.org/)".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}
		[TestMethod]
		public void Url_Invalid_Whitespace_EOF()
		{
			var tokenizer = new CssTokenizer("url(http:// example.org/  ".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}
		[TestMethod]
		public void Url_Invalid_DoubleQuote()
		{
			var tokenizer = new CssTokenizer("url(http://example.org/\")".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}
		[TestMethod]
		public void Url_Invalid_SecondLeftParenthesis()
		{
			var tokenizer = new CssTokenizer("url((http://example.org/)".AsMemory());
			Assert.IsTrue(tokenizer.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}

		[TestMethod]
		public void Comments_NotTokenized()
		{
			var tokenizer = new CssTokenizer("/* Hello World */".AsMemory());
			Assert.IsFalse(tokenizer.NextToken(out _));
		}
	}
}
