namespace TurnerSoftware.CascadingStyles.Parsing
{
	public struct CssTokenParser
	{
		private CssTokenizer Tokenizer;

		private CssToken CurrentToken;
		private CssTokenType CurrentType;

		public CssTokenParser(CssTokenizer tokenizer)
		{
			Tokenizer = tokenizer;
			Tokenizer.NextToken(out CurrentToken);
			CurrentType = CurrentToken.Type;
		}

		private void ConsumeToken()
		{
			Tokenizer.NextToken(out CurrentToken);
			CurrentType = CurrentToken.Type;
		}

		public bool NextRule(out Rule rule)
		{
			Start:
			switch (CurrentType)
			{
				case CssTokenType.EndOfFile:
					rule = default;
					return false;
				case CssTokenType.Whitespace:
					ConsumeToken();
					goto Start;
				//TODO: CDO/CDC token processing
				case CssTokenType.AtKeyword:
					// Consume an at-rule, and append the returned
					// value to the list of rules. 
					rule = ConsumeAtRule();
					break;
				default:
					// Consume a qualified rule. If anything is returned,
					// append it to the list of rules. 
					if (!TryConsumeQualifiedRule(out rule))
					{
						goto Start;
					}
					break;
			}

			ConsumeToken();
			return true;
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-qualified-rule">5.4.3. Consume a qualified rule</a>
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		private bool TryConsumeQualifiedRule(out Rule rule)
		{
			// Create a new qualified rule with its prelude initially
			// set to an empty list, and its value initially set to nothing.
			rule = new QualifiedRule();

			while (true)
			{
				switch (CurrentType)
				{
					case CssTokenType.EndOfFile:
						// This is a parse error. Return nothing.
						rule = default;
						return false;
					case CssTokenType.LeftCurlyBracket:
						// Consume a simple block and assign it to the qualified
						// rule’s block. Return the qualified rule. 
						var block = ConsumeSimpleBlock();
						rule.Value = block;
						return true;
					default:
						// Consume a component value.
						// Append the returned value to the qualified rule’s prelude. 
						var component = ConsumeComponentValue();
						rule.Prelude.Add(component);
						break;
				}

				ConsumeToken();
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-an-at-rule">5.4.2. Consume an at-rule</a>
		/// </summary>
		/// <returns></returns>
		private AtRule ConsumeAtRule()
		{
			// Create a new at-rule with its name set to the value of the current input token,
			// its prelude initially set to an empty list, and its value initially set to nothing.
			var rule = new AtRule
			{
				Name = CurrentToken.RawValue
			};

			// Repeatedly consume the next input token:
			while (true)
			{
				ConsumeToken();

				switch (CurrentType)
				{
					case CssTokenType.Semicolon:
					case CssTokenType.EndOfFile:
						// This is a parse error for EOF.
						goto End;
					case CssTokenType.LeftCurlyBracket:
						// Consume a simple block and assign it to the at-rule’s block.
						// Return the at-rule.
						var block = ConsumeSimpleBlock();
						rule.Value = block;
						goto End;
					default:
						// Consume a component value.
						// Append the returned value to the at-rule’s prelude.
						var component = ConsumeComponentValue();
						rule.Prelude.Add(component);
						continue;
				}
			}

			End:
			return rule;
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-simple-block">5.4.7. Consume a simple block</a>
		/// </summary>
		/// <returns></returns>
		private SimpleBlock ConsumeSimpleBlock()
		{
			var simpleBlock = new SimpleBlock();

			// The ending token is the mirror variant of the current input token.
			// (E.g. if it was called with <[-token>, the ending token is <]-token>.)
			var endingType = CssTokenType.EndOfFile;
			switch (CurrentType)
			{
				case CssTokenType.LeftCurlyBracket:
					endingType = CssTokenType.RightCurlyBracket;
					simpleBlock.Type = SimpleBlockType.CurlyBracket;
					break;
				case CssTokenType.LeftSquareBracket:
					endingType = CssTokenType.RightSquareBracket;
					simpleBlock.Type = SimpleBlockType.SquareBracket;
					break;
				case CssTokenType.LeftParenthesis:
					endingType = CssTokenType.RightParenthesis;
					simpleBlock.Type = SimpleBlockType.Parenthesis;
					break;
			}

			// Repeatedly consume the next input token and process it as follows:
			while (true)
			{
				ConsumeToken();

				// Ending token: Return the block.
				if (CurrentType == endingType)
				{
					break;
				}
				// <EOF-token>: This is a parse error. Return the block. 
				else if (CurrentType == CssTokenType.EndOfFile)
				{
					break;
				}
				// Reconsume the current input token.
				// Consume a component value and append it to the value of the block. 
				else
				{
					var component = ConsumeComponentValue();
					simpleBlock.Value.Add(component);
				}
			}

			return simpleBlock;
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-component-value">5.4.6. Consume a component value</a>
		/// </summary>
		/// <returns></returns>
		private ComponentValue ConsumeComponentValue()
		{
			return CurrentType switch
			{
				// If the current input token is a <{-token>, <[-token>, or <(-token>,
				// consume a simple block and return it.
				CssTokenType.LeftCurlyBracket or CssTokenType.LeftSquareBracket or CssTokenType.LeftParenthesis => ConsumeSimpleBlock(),
				// Otherwise, if the current input token is a <function-token>,
				// consume a function and return it.
				CssTokenType.Function => ConsumeFunction(),
				// Otherwise, return the current input token.
				_ => new TokenComponent { Token = CurrentToken }
			};
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-function">5.4.8. Consume a function</a>
		/// </summary>
		/// <returns></returns>
		private CssFunction ConsumeFunction()
		{
			// Create a function with its name equal to the value of the current input token
			// and with its value initially set to an empty list.
			var function = new CssFunction
			{
				Name = CurrentToken.RawValue
			};

			// Repeatedly consume the next input token and process it as follows:
			while (true)
			{
				ConsumeToken();
				switch (CurrentType)
				{
					case CssTokenType.RightParenthesis:
						// <)-token>: Return the function.
					case CssTokenType.EndOfFile:
						// <EOF-token>: This is a parse error. Return the function.
						return function;
					default:
						// Reconsume the current input token. Consume a component value
						// and append the returned value to the function’s value. 
						var component = ConsumeComponentValue();
						function.Value.Add(component);
						continue;
				}
			}
		}
	}
}
