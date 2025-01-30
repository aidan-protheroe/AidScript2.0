namespace AidScript;

public class Token(string value, TokenType type)
{
    public string Value { get; set; } = value;
    public TokenType Type { get; set; } = type;

    //public int Line { get; set; }
    //public int Column { get; set; }
}

public enum TokenType
{
    Identifier,
    Number,
    String,
    ArithmeticOperator,
    Keyword,
    AssignmentOperator,
    ComparativeOperator,
    LogicalOperator,
    Boolean,
    Unassigned,
    EndOfFile,
}
