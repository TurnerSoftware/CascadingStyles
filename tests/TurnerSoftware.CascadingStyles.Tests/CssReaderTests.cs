using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TurnerSoftware.CascadingStyles.Tests
{
	[TestClass]
	public class CssReaderTests
	{
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
