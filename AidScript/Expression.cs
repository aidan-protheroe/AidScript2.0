namespace AidScript;

public class Expression
{
    public Token operatorToken = null;
    public List<Token> operands = [];
    public Expression expression = null;
    public int Levels { get; set; }
}