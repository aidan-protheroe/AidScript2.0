using AidScript.Functions;
using AidScript.Statements;

namespace AidScript;

public class Parser()
{
    public AbstractSyntaxTree Parse(List<Line> tokenLines) //this should take in the tokenLines
    {
        var ast = new AbstractSyntaxTree();
        for (int x = 0; x < tokenLines.Count; x++)
        {
            ast = ParseLine(tokenLines[x], ast);
        }

        SetPointers(ast);

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
                var assignment = new Assignment(
                    BuildExpression(line.tokens.Skip(2).ToList()),
                    line.tokens[i]
                );
                ast.Add(assignment);
                break;
            }
            else if (line.tokens[i].Type == TokenType.Keyword)
            {
                //I tried sturning this into a switch case and it overflowed? Can't figure out why
                if (line.tokens[i].Value == "var")
                {
                    ast.Add(
                        new Initialization(
                            BuildExpression(line.tokens.Skip(3).ToList()),
                            line.tokens[i + 1]
                        )
                    );
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
                else if (line.tokens[i].Value == "while")
                {
                    ast.Add(new While(BuildConditional(line.tokens.Skip(1).ToList()), line.number));
                    break;
                }
                else
                {
                    ast.Add(BuildFunction(line.tokens));
                    break;
                }
            }
            i++;
        }
        return ast;
    }

    public Expression BuildExpression(List<Token> tokens) //support string concatenation
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

    public Function BuildFunction(List<Token> tokens) //change to function
    {
        var function = new Function(tokens[0]);
        int x = 0;
        while (x < tokens.Count) //check for conditionals first because they contain expressions
        {
            if (tokens[x].Type == TokenType.ComparativeOperator)
            {
                function.BuildArg(BuildConditional(tokens.Skip(1).ToList()));
                return function;
            }
            x++;
        }

        function.BuildArg(BuildExpression(tokens.Skip(1).ToList()));
        return function;
    }

    public void SetPointers(AbstractSyntaxTree ast)
    {
        var statements = ast.GetAllStatements();
        for (int i = 0; i < statements.Count; i++)
        {
            var (type, value) = statements[i];
            if (type == StatementType.If)
            {
                var ifStatement = (If)value;
                if (ifStatement.End == null)
                {
                    SetIfPointer(statements, i);
                }
            }
            else if (type == StatementType.While)
            {
                var whileStatement = (While)value;
                if (whileStatement.end == null)
                {
                    SetWhilePointer(statements, i);
                }
            }
        }
    }

    public int SetIfPointer(List<(StatementType type, object value)> statements, int x)
    {
        var ifStatement = (If)statements[x].value;
        int i = x + 1;
        while (i < statements.Count)
        {
            var (type, statement) = statements[i];
            if (type == StatementType.If)
            {
                i = SetIfPointer(statements, i);
            }
            else if (type == StatementType.While)
            {
                i = SetWhilePointer(statements, i);
            }
            else if (type == StatementType.Else)
            {
                var elseStatement = (Else)statement;
                elseStatement.If = ifStatement;
                ifStatement.Else = elseStatement;
            }
            else if (type == StatementType.End)
            {
                ifStatement.End = (End)statement;
                if (ifStatement.Else != null)
                {
                    ifStatement.Else.End = (End)statement;
                }
                return i;
            }
            i++;
        }
        return 0; //error
    }

    public int SetWhilePointer(List<(StatementType type, object value)> statements, int x)
    {
        var whileStatement = (While)statements[x].value;
        int i = x + 1;
        while (i < statements.Count)
        {
            var (type, statement) = statements[i];
            if (type == StatementType.If)
            {
                i = SetIfPointer(statements, i);
            }
            else if (type == StatementType.While)
            {
                i = SetWhilePointer(statements, i);
            }
            else if (type == StatementType.End)
            {
                whileStatement.end = (End)statement;
                whileStatement.end.While = whileStatement;
                return i;
            }
            i++;
        }
        return 0; //error
    }
}

public class While(Conditional conditional, int line)
{
    public Conditional conditional = conditional;
    public End end;
    public int line = line;
}


//adding ands and ors to conditional will make it a lot more complex
