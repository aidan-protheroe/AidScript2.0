namespace AidScript.Functions;

public class Function
{
    public Token Keyword;
    public Argument Arg { get; set; }

    public SystemFunction systemFunction;

    public Function(Token keyword)
    {
        Keyword = keyword;
        if (keyword.Value == "write") systemFunction = SystemFunction.Write;
        else systemFunction = SystemFunction.None;
    }


    public void BuildArg(Expression expression)
    {
        Arg = new Argument(ArgumentType.Expression, expr: expression);
    }

    public void BuildArg(Conditional conditional)
    {
        Arg = new Argument(ArgumentType.Conditional, cond: conditional);
    }
}

public enum SystemFunction
{
    Write,
    Input,
    None
}