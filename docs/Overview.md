# Overview

## Introduction

The role of a parser is to transform a stream of characters (e.g. a text file) into a sequence of high-level structures, like for-loops, variable declarations, operation, etc...

But the parser needs an intermediate step, between characters and structures : tokens. Tokens are the unit right above characters, they represent groups of characters that are to be interpreted together : numbers, identifiers, strings, etc... The process used to create those tokens is called *tokenizing* or *lexing* (or sometimes *scanning*, but that can mean something different).

## Tokenizing

Tokens have five properties :

- The textual representation

- The [location](#Location) of the token in the input

- The ["kind"](#Token_Kind) of token this is

- The [trivia](#Trivia_Tokens) before the token

- The [trivia](#Trivia_Tokens) after the token

*See [Tokens](#Tokens) for a more detailed explanation.*

Tokens are identified by their *pattern*. It basically describes how to recognize a certain kind of token and how to build said token from the input. Most (if not all) of those patterns can be described in [EBNF grammar](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form) or written as a regex.

Since this parser is supposed to be easily extendable, the tokenizer needs to be extendable too. This is achieved with *toklets*.

### Toklets

Toklets are a 
