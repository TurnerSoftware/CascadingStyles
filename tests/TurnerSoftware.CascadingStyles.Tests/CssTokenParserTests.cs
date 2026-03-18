using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.CascadingStyles.Parsing;

namespace TurnerSoftware.CascadingStyles.Tests
{
	[TestClass]
	public class CssTokenParserTests
	{
		private static IReadOnlyList<Rule> GetRules(string value)
		{
			var tokenizer = new CssTokenizer(value.AsMemory());
			var parser = new CssTokenParser(tokenizer);

			var results = new List<Rule>();
			while (parser.NextRule(out var rule))
			{
				results.Add(rule);
			}
			return results;
		}

		private static void AssertSequenceEqual(IReadOnlyList<Rule> expected, IReadOnlyList<Rule> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);
			for (var i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		[TestMethod]
		public void QualifiedRule_ClassSelector_SingleDeclaration()
		{
			var testData = @".hello-world{color:blue;}";
			var expected = new List<Rule>
			{
				new QualifiedRule
				{
					Prelude = new List<ComponentValue>
					{
						new TokenComponent { Token = new CssToken(".", CssTokenType.Delimiter) },
						new TokenComponent { Token = new CssToken("hello-world", CssTokenType.Identifier) },
					},
					Value = new SimpleBlock(SimpleBlockType.CurlyBracket)
					{
						new TokenComponent { Token = new CssToken("color", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(":", CssTokenType.Colon) },
						new TokenComponent { Token = new CssToken("blue", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(";", CssTokenType.Semicolon) }
					}
				}
			};

			var actual = GetRules(testData);

			AssertSequenceEqual(expected, actual);
		}

		[TestMethod]
		public void AtRule_Media_ClassSelector_SingleDeclaration()
		{
			var testData = @"@media screen and (max-width:14rem){.test{background:none;}}";
			var expected = new List<Rule>
			{
				new AtRule
				{
					Name = "media",
					Prelude = new List<ComponentValue>
					{
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new TokenComponent { Token = new CssToken("screen", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new TokenComponent { Token = new CssToken("and", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new SimpleBlock(SimpleBlockType.Parenthesis)
						{
							new TokenComponent { Token = new CssToken("max-width", CssTokenType.Identifier) },
							new TokenComponent { Token = new CssToken(":", CssTokenType.Colon) },
							new TokenComponent { Token = new CssToken("14", CssTokenType.Dimension, CssTokenFlag.Number_Integer, "rem") },
						}
					},
					Value = new SimpleBlock(SimpleBlockType.CurlyBracket)
					{
						new TokenComponent { Token = new CssToken(".", CssTokenType.Delimiter) },
						new TokenComponent { Token = new CssToken("test", CssTokenType.Identifier) },
						new SimpleBlock(SimpleBlockType.CurlyBracket)
						{
							new TokenComponent { Token = new CssToken("background", CssTokenType.Identifier) },
							new TokenComponent { Token = new CssToken(":", CssTokenType.Colon) },
							new TokenComponent { Token = new CssToken("none", CssTokenType.Identifier) },
							new TokenComponent { Token = new CssToken(";", CssTokenType.Semicolon) }
						},
					}
				}
			};

			var actual = GetRules(testData);

			AssertSequenceEqual(expected, actual);
		}

		[TestMethod]
		public void MixedExample()
		{
			var testData = @".hello-world  {color: blue;  }
@media screen and (max-width: 14rem){
.test{ background:none;}
}";
			var expected = new List<Rule>
			{
				new QualifiedRule
				{
					Prelude = new List<ComponentValue>
					{
						new TokenComponent { Token = new CssToken(".", CssTokenType.Delimiter) },
						new TokenComponent { Token = new CssToken("hello-world", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken("  ", CssTokenType.Whitespace) }
					},
					Value = new SimpleBlock(SimpleBlockType.CurlyBracket)
					{
						new TokenComponent { Token = new CssToken("color", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(":", CssTokenType.Colon) },
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new TokenComponent { Token = new CssToken("blue", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(";", CssTokenType.Semicolon) },
						new TokenComponent { Token = new CssToken("  ", CssTokenType.Whitespace) }
					}
				},
				new AtRule
				{
					Name = "media",
					Prelude = new List<ComponentValue>
					{
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new TokenComponent { Token = new CssToken("screen", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new TokenComponent { Token = new CssToken("and", CssTokenType.Identifier) },
						new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
						new SimpleBlock(SimpleBlockType.Parenthesis)
						{
							new TokenComponent { Token = new CssToken("max-width", CssTokenType.Identifier) },
							new TokenComponent { Token = new CssToken(":", CssTokenType.Colon) },
							new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
							new TokenComponent { Token = new CssToken("14", CssTokenType.Dimension, CssTokenFlag.Number_Integer, "rem") },
						}
					},
					Value = new SimpleBlock(SimpleBlockType.CurlyBracket)
					{
						new TokenComponent { Token = new CssToken("\r\n", CssTokenType.Whitespace) },
						new TokenComponent { Token = new CssToken(".", CssTokenType.Delimiter) },
						new TokenComponent { Token = new CssToken("test", CssTokenType.Identifier) },
						new SimpleBlock(SimpleBlockType.CurlyBracket)
						{
							new TokenComponent { Token = new CssToken(" ", CssTokenType.Whitespace) },
							new TokenComponent { Token = new CssToken("background", CssTokenType.Identifier) },
							new TokenComponent { Token = new CssToken(":", CssTokenType.Colon) },
							new TokenComponent { Token = new CssToken("none", CssTokenType.Identifier) },
							new TokenComponent { Token = new CssToken(";", CssTokenType.Semicolon) }
						},
						new TokenComponent { Token = new CssToken("\r\n", CssTokenType.Whitespace) },
					}
				}
			};

			var actual = GetRules(testData);

			AssertSequenceEqual(expected, actual);
		}
	}
}
