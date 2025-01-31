namespace AidScript.Functions;

public class Function(Token keyword)
{
    public Token Keyword = keyword;
    public Argument Arg { get; set; }

    public void BuildArg(Expression expression)
    {
        Arg = new Argument(ArgumentType.Expression, expr: expression);
    }

    public void BuildArg(Conditional conditional)
    {
        Arg = new Argument(ArgumentType.Conditional, cond: conditional);
    }
}

