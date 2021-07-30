using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TurnerSoftware.CascadingStyles.Tests
{
	[TestClass]
	public class CssReaderTests
	{
		[TestMethod]
		public void String_Good()
		{
			var reader = new CssReader("\"Hello World\"");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.String, token.Type);
			Assert.AreEqual("Hello World", token.RawValue.ToString());
		}

		[TestMethod]
		public void String_Bad_NewLine()
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
		public void Number_Float()
		{
			var reader = new CssReader("1.0");
			Assert.IsTrue(reader.NextToken(out var token));
			Assert.AreEqual(CssTokenType.Number, token.Type);
			Assert.AreEqual(CssTokenFlag.Number_Number, token.Flags);
			Assert.AreEqual("1.0", token.RawValue.ToString());
		}

		[TestMethod]
		public void Comments_NotTokenized()
		{
			var reader = new CssReader("/* Hello World */");
			Assert.IsFalse(reader.NextToken(out _));
		}
	}
}
