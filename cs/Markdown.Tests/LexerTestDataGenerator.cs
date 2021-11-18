﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Markdown.Tokens;
using NUnit.Framework;

namespace Markdown.Tests
{
    public class LexerTestDataGenerator : IEnumerable<TestCaseData>
    {
        private readonly Token[] testTokens = 
        {
            new(TokenType.Italics, "_"), new(TokenType.Bold, "__"),
            new(TokenType.Escape, "/"), new(TokenType.NewLine, "\n"),
            new(TokenType.Header1, "#")
        };
        public IEnumerator<TestCaseData> GetEnumerator()
        {
            foreach (var testToken in testTokens)
            {
                if(testToken.TokenType != TokenType.Italics)
                    yield return GenerateCountSymbols(testToken.Value, testToken.TokenType, 3);
                yield return SurroundAllTextWith(testToken.Value, testToken.TokenType);
                yield return SurroundPartOfTextWith(testToken.Value, testToken.TokenType);
            }

            foreach (var data in GenerateCombinedTestData())
                yield return data;
        }
        
        private IEnumerable<TestCaseData> GenerateCombinedTestData()
        {
            yield return new TestCaseData("___", new Token[]
            {
                new(TokenType.Bold, "__"),
                new(TokenType.Italics, "_")
            }).SetName("Combined Bold and Italics 1");
            
            yield return new TestCaseData("___as___", new Token[]
            {
                new(TokenType.Bold, "__"), new(TokenType.Italics, "_"),
                new(TokenType.Text, "as"), new(TokenType.Bold, "__"),
                new(TokenType.Italics, "_")
            }).SetName("Combined Bold and Italics 2");
        }

        private TestCaseData GenerateCountSymbols(string symbol, TokenType tokenType, int count)
        {
            var symbols = Enumerable.Range(0, count).Aggregate(string.Empty, (s, _) => s + symbol);
            return new TestCaseData(symbols, Enumerable.Repeat(new Token(tokenType, symbol), count).ToArray())
                .SetName($"{tokenType.ToString()} repeated several times");
        }

        private TestCaseData SurroundAllTextWith(string symbol, TokenType tokenType)
        {
            const string text = "abc";
            var testString = symbol + text + symbol;
            return new TestCaseData(testString, new Token[]
            {
                new(tokenType, symbol), new(TokenType.Text, text), new(tokenType, symbol)
            }).SetName($"Text surrounded with {tokenType.ToString()}");
        }

        private TestCaseData SurroundPartOfTextWith(string symbol, TokenType tokenType)
        {
            const string firstPart = "abc";
            const string middlePart = "de";
            const string lastPart = "xyz";
            var testString = firstPart + symbol + middlePart + symbol + lastPart;
            return new TestCaseData(testString, new Token[]
            {
                new(TokenType.Text, firstPart), new(tokenType, symbol), 
                new(TokenType.Text, middlePart), new(tokenType, symbol),
                new(TokenType.Text, lastPart)
            }).SetName($"Only part of the text surrounded with {tokenType.ToString()}");
        }
        

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}