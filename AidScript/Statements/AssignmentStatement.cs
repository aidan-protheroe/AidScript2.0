namespace AidScript.Statements;

public class Assignment(Expression expression, Token identifier)
{
    public Token Identifier = identifier;
    public Expression Expression = expression;
}