namespace AidScript;

public class Conditional
{
    public Expression leftExpression = new Expression();
    public Expression rightExpression = new Expression();
    public Token comparativeToken { get; set; }
}