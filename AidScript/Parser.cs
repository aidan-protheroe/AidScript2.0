using System.Reflection;

namespace AidScript;

public class Parser()
{

    public AbstractSyntaxTree Parse(List<Line> tokenLines) //this should take in the tokenLines
    {
        var ast = new AbstractSyntaxTree();
        for (int x = 0; x < tokenLines.Count; x++)
        {
            var line = tokenLines[x];
            ast = ParseLine(line, ast);
        }
        ast.SetIfElseEndPointers(); //put this method in this class

        return ast;
    }

    public AbstractSyntaxTree ParseLine(Line line, AbstractSyntaxTree ast)
    {
        int i = 0;
        while (i < line.tokens.Count)
        {
            if (
                line.tokens[i].Type == TokenType.Identifier
                && line.tokens[i + 1].Type == TokenType.AssignmentOperator //must differientate string operations and arithmetic ones(or have the interpreter do that actually?)
            )
            {
                var assignment = new Assignment(BuildExpression(line.tokens.Skip(2).ToList()), line.tokens[i]);
                ast.Add(assignment);
                break;
            }
            else if (line.tokens[i].Type == TokenType.Keyword)
            {
                if (line.tokens[i].Value == "var")
                {
                    ast.Add(new Initialization(BuildExpression(line.tokens.Skip(3).ToList()), line.tokens[i + 1]));
                    break;
                }
                else if (line.tokens[i].Value == "if")
                {
                    ast.Add(new If(BuildConditional(line.tokens.Skip(1).ToList()))); //i dunno if buildIfStatement is even needed anymore
                    break;
                }
                else if (line.tokens[i].Value == "else")
                {
                    ast.Add(new Else(line.number));
                    break;
                }
                else if (line.tokens[i].Value == "end")
                {
                    ast.Add(new End(line.number));
                    break;
                }
                else
                {
                    ast.Add(BuildMethod(line.tokens));
                    break;
                }
            }
            i++;
        }
        return ast;
    }

    public Expression BuildExpression(List<Token> tokens) //change to build math expression(or not, it could just return x = "hello world". if it retruns x = 1 + "hello world" thats the interpreters problem)
    {
        var expression = new Expression();
        var levels = 0; //this should be useful later on for the interpreter

        while (tokens.Count > 0) 
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
        expression.Levels = levels;
        return expression; //expression can just be one identifier/number/string(a monomial) or a math expression/string expression(a polynomial)
    }

    public Conditional BuildConditional(List<Token> tokens)
    {
        var conditional = new Conditional();
        var built = false;
        int i = 0;
        while (!built) //change to a for loop
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
        var method = new Method(tokens[0]);
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

public class Assignment(Expression expression, Token identifier)
{
    public Token Identifier = identifier;
    public Expression Expression = expression;
}

public class Initialization(Expression expression, Token identifier)
{
    public Token Identifier = identifier;
    public Expression Expression = expression;
}

//add initialize that works generally the same as assignment but must be used for first time initialization of a indentifier

public class Method(Token keyword)
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

public class If(Conditional conditional)
{
    public Conditional Conditional = conditional;
    public Else Else = null;
    public End End = null;
    public bool Traversed = false; // I added this bc I wasnt sure was was causing problems but I dont think its needed now
}

public class End(int line)
{
    public int Line = line;
    public bool set = false;
}

public class Else(int line)
{
    public int Line = line;
    public End? End = null; //this shouldn't be nullable, it is required
    public If? If = null; //you only need this or the conditional not both --actually since it uses If.Travertsed now you really don't need eitehr
}

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


//adding ands and ors to conditional will make it a lot more complex

//if you wanna use var to declare var inits, you need to make it so a method can take an assignment as a parameter(and create a build assigment method)
