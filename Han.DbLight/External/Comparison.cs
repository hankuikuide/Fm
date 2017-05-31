namespace Han.DbLight.External
{
    /// <summary>
    /// SQL Comparison Operators
    /// </summary>
    public enum Comparison
    {
        Equals,
        NotEquals,
        Like,
        NotLike,
        GreaterThan,
        GreaterOrEquals,
        LessThan,
        LessOrEquals,
        Blank,
        Is,
        IsNot,
        In,
        NotIn,
        OpenParentheses,
        CloseParentheses,
        BetweenAnd,
        StartsWith,
        EndsWith
    }
}