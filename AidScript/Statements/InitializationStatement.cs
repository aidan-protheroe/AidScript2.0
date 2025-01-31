namespace AidScript.Statements;

public class Initialization(Expression expression, Token identifier)
{
    public Token Identifier = identifier;
    public Expression Expression = expression;
}
