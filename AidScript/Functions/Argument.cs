namespace AidScript.Functions;

public class Argument(ArgumentType type, Expression? expr = null, Conditional? cond = null)
{
    public Expression? Expression = expr;
    public Conditional? Conditional = cond;
    public ArgumentType Type = type;
}

public enum ArgumentType
{
    Expression,
    Conditional,
}