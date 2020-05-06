using System;

public interface ITriviaToklet<out T> : IToklet<T> where T : TriviaToken
{ }