namespace AidScript;

public class Parser(List<List<Token>> tokenLines)
{
    private List<List<Token>> _tokenLines = tokenLines;
    private AbstractSyntaxTree _ast = new();

    public AbstractSyntaxTree Parse() //this should take in the tokenLines
    {
        for (int x = 0; x < _tokenLines.Count; x++)
        {
            var tokens = _tokenLines[x];
            int i = 0;
            while (i < tokens.Count)
            {
                if (
                    tokens[i].Type == TokenType.Identifier
                    && tokens[i + 1].Type == TokenType.AssignmentOperator //must differientate string operations and arithmetic ones(or have the interpreter do that actually?)
                )
                {
                    var assignment = new Assignment()
                    {
                        Identifier = tokens[i],
                        Expression = BuildExpression(tokens.Skip(2).ToList()), //Expression = BuildExpression() when you don't need the debug logs
                    };
                    _ast.Add(assignment);
                    break;
                }
                else if (tokens[i].Type == TokenType.Keyword)
                {
                    if (tokens[i].Value == "var") //should you add classes for if statements, whiles, write, etc?
                    {
                        var initialization = new Initialization()
                        {
                            Identifier = tokens[i + 1],
                            Expression = BuildExpression(tokens.Skip(3).ToList()),
                        };
                        _ast.Add(initialization);
                        break;
                    }
                    else if (tokens[i].Value == "if")
                    {
                        var ifStatement = BuildIfStatement(tokens);
                        ifStatement.StartLine = x;
                        _ast.Add(ifStatement);
                        break;
                    }
                    else if (tokens[i].Value == "else")
                    {
                        var elseStatement = new Else
                        {
                            StartLine = x
                        };
                        _ast.Add(elseStatement);
                        break;
                    }
                    else if (tokens[i].Value == "end")
                    {
                        var endStatement = new End
                        {
                            Line = x
                        };
                        _ast.Add(endStatement);
                        break;
                    }
                    else
                    {
                        var method = BuildMethod(tokens);
                        _ast.Add(method);
                        break;
                    }
                }
                i++;
            }
        }
        _ast.SetIfElseEndPointers(); //put this method in this class

        return _ast;
    }

    public Expression BuildExpression(List<Token> tokens) //change to build math expression(or not, it could just return x = "hello world". if it retruns x = 1 + "hello world" thats the interpreters problem)
    {
        //add acheck for if the expression is a monomial? and just return immediatley?
        var expression = new Expression();
        var built = false;
        var levels = 0; //this should be useful later on for the interpreter

        while (!built) //just change to while tokens.Count > 0
        {
            if (tokens.Count > 0)
            {
                if (
                    tokens[0].Type == TokenType.Number
                    || tokens[0].Type == TokenType.Identifier
                    || tokens[0].Type == TokenType.String
                )
                {
                    expression.operands.Add(tokens[0]);
                    tokens.RemoveAt(0);
                }
                if (tokens.Count > 0 && tokens[0].Type == TokenType.ArithmeticOperator)
                {
                    expression.operatorToken = tokens[0];
                    tokens.RemoveAt(0);
                    expression.operands.Add(tokens[0]);
                    tokens.RemoveAt(0);
                }
                if (tokens.Count > 1)
                {
                    expression.expression = BuildExpression(tokens);
                    levels++;
                    levels += expression.expression.Levels;
                }
            }
            else
            {
                built = true;
            }
        }
        expression.Levels = levels;
        return expression; //expression can just be one identifier/number/string(a monomial) or a math expression/string expression(a polynomial)
    }

    public Conditional BuildConditional(List<Token> tokens)
    {
        var conditional = new Conditional();
        var built = false;
        int i = 0;
        while (!built)
        {
            if (tokens[i].Type == TokenType.ComparativeOperator)
            {
                conditional.comparativeToken = tokens[i];
                conditional.leftExpression = BuildExpression(tokens.Take(i).ToList());
                conditional.rightExpression = BuildExpression(tokens.Skip(i + 1).ToList());
                built = true;
            }
            i++;
        }
        return conditional;
    }

    public Method BuildMethod(List<Token> tokens) //change to function
    {
        var method = new Method();

        method.Keyword = tokens[0]; //gonna have to figure this out -- for now you can just assume any use of a method is going to be at column 0, but if its ever not-- you have to know how many columns you are in for BuildExpression (instead of skip 1)
        int x = 0;
        while (x < tokens.Count) //check for conditionals first because they contain expressions
        {
            if (tokens[x].Type == TokenType.ComparativeOperator)
            {
                method.BuildArg(BuildConditional(tokens.Skip(1).ToList()));
                return method;
            }
            x++;
        }

        method.BuildArg(BuildExpression(tokens.Skip(1).ToList()));
        return method;
    }

    public If BuildIfStatement(List<Token> tokens) //i think this will need some more data like start index or smth
    {
        //pass all of the lines (starting with the one with the if statement) so it can look for an else as the end point? or make else class and after everything is built connect if's and else's
        var ifStatement = new If();
        ifStatement.Conditional = BuildConditional(tokens.Skip(1).ToList());
        return ifStatement;
    }
}

public class Expression
{
    public Token operatorToken = null;
    public List<Token> operands = [];
    public Expression expression = null;
    public int Levels { get; set; }
}

public class Conditional
{
    public Expression leftExpression = new Expression();
    public Expression rightExpression = new Expression();
    public Token comparativeToken { get; set; }
}

public class Assignment
{
    public Token Identifier { get; set; }
    public Expression Expression { get; set; }
}

public class Initialization
{
    public Token Identifier { get; set; }
    public Expression Expression { get; set; }
}

//add initialize that works generally the same as assignment but must be used for first time initialization of a indentifier

public class Method
{
    public Token Keyword { get; set; }
    public Argument Arg { get; set; }

    public void BuildArg(Expression expression)
    {
        Arg = new Argument();
        Arg.Type = ArgumentType.Expression;
        Arg.Expression = expression;
    }

    public void BuildArg(Conditional conditional)
    {
        Arg = new Argument();
        Arg.Type = ArgumentType.Conditional;
        Arg.Conditional = conditional;
    }
}

public class If
{
    public Conditional Conditional { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; } //change line to index
    public Else Else = null;
    public End End = null;

    public bool Traversed = false; // I added this bc I wasnt sure was was causing problems but I dont think its needed now
}

public class End
{
    public int Line { get; set; }
    public bool set = false;
}

public class Else
{
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public Conditional conditional { get; set; } //set this to the same as the if conditional, so when an else is found
    public End End = null;
    public If If = null; //you only need this or the conditional not both
}

public class Argument
{
    public Expression Expression { get; set; }
    public Conditional Conditional { get; set; }
    public ArgumentType Type { get; set; }

    public Object Value()
    {
        return Type == ArgumentType.Conditional ? Conditional : Expression;
    }
}

public enum ArgumentType
{
    Expression,
    Conditional,
}


//adding ands and ors to conditional will make it a lot more complex

//if you wanna use var to declare var inits, you need to make it so a method can take an assignment as a parameter(and create a build assigment method)
