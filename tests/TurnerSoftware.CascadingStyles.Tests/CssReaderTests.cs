using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TurnerSoftware.CascadingStyles.Tests
{
	[TestClass]
	public class CssReaderTests
	{
		[TestMethod]
		public void NextToken_NoTokens()
		{
			var reader = new CssReader(string.Empty);
			Assert.IsFalse(reader.NextToken(out _));
		}

		[TestMethod]
		public void Whitespace_Space()
		{
			var reader = new CssReader(" ");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual(" ", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_Tab()
		{
			var reader = new CssReader("\t");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\t", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_CarriageReturn()
		{
			var reader = new CssReader("\r");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\r", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_NewLine()
		{
			var reader = new CssReader("\n");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\n", token.RawValue.ToString());
		}
		[TestMethod]
		public void Whitespace_FormFeed()
		{
			var reader = new CssReader("\f");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Whitespace, token.Type);
			Assert.AreEqual("\f", token.RawValue.ToString());
		}

		[TestMethod]
		public void CDO_Partial_1()
		{
			var reader = new CssReader("<");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Delimiter, token.Type);
			Assert.AreEqual("<", token.RawValue.ToString());
		}
		[TestMethod]
		public void CDO_Partial_2()
		{
			var reader = new CssReader("<!");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Delimiter, token.Type);
			Assert.AreEqual("<", token.RawValue.ToString());
		}
		[TestMethod]
		public void CDO_Partial_3()
		{
			var reader = new CssReader("<!-");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Delimiter, token.Type);
			Assert.AreEqual("<", token.RawValue.ToString());
		}
		[TestMethod]
		public void CDO_Valid()
		{
			var reader = new CssReader("<!--");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.CDO, token.Type);
		}

		[TestMethod]
		public void SimpleToken_LeftParenthesis()
		{
			var reader = new CssReader("(");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.LeftParenthesis, token.Type);
		}
		[TestMethod]
		public void SimpleToken_RightParenthesis()
		{
			var reader = new CssReader(")");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.RightParenthesis, token.Type);
		}
		[TestMethod]
		public void SimpleToken_Comma()
		{
			var reader = new CssReader(",");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Comma, token.Type);
		}
		[TestMethod]
		public void SimpleToken_Colon()
		{
			var reader = new CssReader(":");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Colon, token.Type);
		}
		[TestMethod]
		public void SimpleToken_Semicolon()
		{
			var reader = new CssReader(";");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Semicolon, token.Type);
		}
		[TestMethod]
		public void SimpleToken_LeftSquareBracket()
		{
			var reader = new CssReader("[");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.LeftSquareBracket, token.Type);
		}
		[TestMethod]
		public void SimpleToken_RightSquareBracket()
		{
			var reader = new CssReader("]");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.RightSquareBracket, token.Type);
		}
		[TestMethod]
		public void SimpleToken_LeftCurlyBracket()
		{
			var reader = new CssReader("{");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.LeftCurlyBracket, token.Type);
		}
		[TestMethod]
		public void SimpleToken_RightCurlyBracket()
		{
			var reader = new CssReader("}");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.RightCurlyBracket, token.Type);
		}

		[TestMethod]
		public void String_Good()
		{
			var reader = new CssReader("\"Hello World\"");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.String, token.Type);
			Assert.AreEqual("Hello World", token.RawValue.ToString());
		}

		[TestMethod]
		public void BadString_NewLine()
		{
			var reader = new CssReader("\"Hello\nWorld\"");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadString, token.Type);
			Assert.AreEqual(string.Empty, token.RawValue.ToString());
		}

		[TestMethod]
		public void Number_Integer()
		{
			var reader = new CssReader("1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Integer, token.Flags);
			Assert.AreEqual("1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Integer_LeadingPlus()
		{
			var reader = new CssReader("+1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Integer, token.Flags);
			Assert.AreEqual("+1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Integer_LeadingMinus()
		{
			var reader = new CssReader("-1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Integer, token.Flags);
			Assert.AreEqual("-1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Float_SingleDigit()
		{
			var reader = new CssReader("1.0");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1.0", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_Float_MultiDigit()
		{
			var reader = new CssReader("1.01");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1.01", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Lower_Positive()
		{
			var reader = new CssReader("1e+1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1e+1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Lower_Negative()
		{
			var reader = new CssReader("1e-1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1e-1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Upper_Positive()
		{
			var reader = new CssReader("1E+1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1E+1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_Upper_Negative()
		{
			var reader = new CssReader("1E-1");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1E-1", token.RawValue.ToString());
		}
		[TestMethod]
		public void Number_ScientificNotation_MultiDigit()
		{
			var reader = new CssReader("1e+100");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1e+100", token.RawValue.ToString());
		}

		[TestMethod]
		public void Url_Common()
		{
			var reader = new CssReader("url(http://example.org/)");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_WrappedDoubleQuote()
		{
			var reader = new CssReader("url(\"http://example.org/\")");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Function, token.Type);
			Assert.AreEqual("url", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_WrappedSingleQuote()
		{
			var reader = new CssReader("url('http://example.org/')");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Function, token.Type);
			Assert.AreEqual("url", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_LeadingWhitespace()
		{
			var reader = new CssReader("url(  http://example.org/)");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_TrailingWhitespace()
		{
			var reader = new CssReader("url(http://example.org/  )");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_EmptyUrl()
		{
			var reader = new CssReader("url()");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual(string.Empty, token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_EOF()
		{
			var reader = new CssReader("url(http://example.org/");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Url, token.Type);
			Assert.AreEqual("http://example.org/", token.RawValue.ToString());
		}
		[TestMethod]
		public void Url_Invalid_Whitespace()
		{
			var reader = new CssReader("url(http:// example.org/)");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}
		[TestMethod]
		public void Url_Invalid_Whitespace_EOF()
		{
			var reader = new CssReader("url(http:// example.org/  ");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}
		[TestMethod]
		public void Url_Invalid_DoubleQuote()
		{
			var reader = new CssReader("url(http://example.org/\")");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}
		[TestMethod]
		public void Url_Invalid_SecondLeftParenthesis()
		{
			var reader = new CssReader("url((http://example.org/)");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.BadUrl, token.Type);
		}

		[TestMethod]
		public void Comments_NotTokenized()
		{
			var reader = new CssReader("/* Hello World */");
			Assert.IsFalse(reader.NextToken(out _));
		}
	}
}
