namespace AidScript;

public class Executor
{
    private AbstractSyntaxTree _ast;
    private Heap _heap;

    public Executor(AbstractSyntaxTree ast, Heap heap)
    {
        _ast = ast;
        _heap = heap; //keep this so you can eventually have the interpreter work in cmd like python's does
    }

    public int Execute() //returns 1 if exdecution succeeded, 0 if not -- this should take in the ast and create a heap
    {
        var (type, statement) = _ast.GetNextStatement();
        while (type != StatementType.EndOfFile)
        {
            if (type == StatementType.Initialization)
            {
                var initialization = (Initialization)statement;
                var value = Evaluate(initialization.Expression);
                if (int.TryParse(value, out int intValue))
                    _heap.AddValue(initialization.Identifier.Value, intValue); 
                else
                    _heap.AddValue(initialization.Identifier.Value, value);
            }
            else if (type == StatementType.Assignment)
            {
                var assignment = (Assignment)statement;
                if (_heap.GetValue(assignment.Identifier.Value) == null)
                    return 0; //error -- identifier does not exist in heap
                var value = Evaluate(assignment.Expression);
                if (int.TryParse(value, out int intValue))
                    _heap.UpdateValue(assignment.Identifier.Value, intValue);
                else
                    _heap.UpdateValue(assignment.Identifier.Value, value);
            }
            else if (type == StatementType.If)
            {
                var ifStatement = (If)statement;
                if (Evaluate(ifStatement.Conditional) == "false")
                {
                    if (ifStatement.Else == null)
                        _ast.CurrentGetStatement = ifStatement.End.Line + 1;
                    else
                        _ast.CurrentGetStatement = ifStatement.Else.Line;
                }
                else
                {
                    ifStatement.Traversed = true;
                }
            }
            else if (type == StatementType.Else)
            {
                var elseStatement = (Else)statement;
                if (elseStatement.If.Traversed) //shouldnt be nullable
                    _ast.CurrentGetStatement = elseStatement.End.Line + 1;
            }
            else if (type == StatementType.Method)
            {
                var method = (Method)statement;
                if (method.Keyword.Value == "write")
                {
                    Console.WriteLine("Output:" + (method.Arg.Type == ArgumentType.Expression ? Evaluate(method.Arg.Expression) : Evaluate(method.Arg.Conditional)));
                }
            }
            (type, statement) = _ast.GetNextStatement(); //or ast.GetStatement(index)? for specific lines
        }
        return 1;
    }

    //public int Execute(assignemnt)
    //public int Execute(if)
    //public int Execute(else)
    //public int Execute(method)
    //all return 1 if success, 0 if not, this will help pinpoint errors

    public string Evaluate(Expression expression) //returns a string, but if it returns a value that can be parsed to an int then store it in the heap as an int
    {
        //if its a monomial return the value
        if (expression.operatorToken == null)
        {
            return expression.operands[0].Type == TokenType.Identifier
                ? _heap.GetValue(expression.operands[0].Value)
                : expression.operands[0].Value;
        }
        //if its a polynomial starting with a number return the evaluation of the math
        if (
            int.TryParse(expression.operands[0].Value, out int value)
            || int.TryParse(_heap.GetValue(expression.operands[0].Value), out value)
        )
        {
            return EvaluateMath(expression);
        }
        else 
        {
            return EvaluateString(expression); 
        }
        return "";
    }

    public string EvaluateMath(Expression originalExpression) //returns a string, but if it returns a value that can be parsed to an int then store it in the heap as an int
    {
        //this is a huge mess it can be fixed(and needs order of operations)
        var expression = originalExpression;

        int index = 1;

        int result = GetNumberValue(expression.operands[0]);

        while (true)
        {
            var operatorToken = expression.operatorToken;
            var operands = expression.operands;

            var value = GetNumberValue(operands[index]);
            switch (operatorToken.Value)
            {
                case "+":
                    result += value;
                    break;
                case "-":
                    result -= value;
                    break;
                case "*":
                    result *= value;
                    break;
                case "/":
                    result /= value;
                    break;
            }
            if (expression.expression != null)
                expression = expression.expression;
            else
                return result.ToString();
            if (index == 1)
                index = 0;
        }
    }

    public string EvaluateString(Expression originalExpression) 
    {
        var expression = originalExpression;

        int index = 1;

        string result = GetStringValue(expression.operands[0]);

        while (true)
        {
            var operatorToken = expression.operatorToken;
            var operands = expression.operands;

            var value = GetStringValue(operands[index]);
            switch (operatorToken.Value)
            {
                case "+":
                    result += value;
                    break;
                default:
                    return "0"; //error
            }
            if (expression.expression != null)
                expression = expression.expression;
            else
                return result;
            if (index == 1)
                index = 0;
        }
    }

    public string Evaluate(Conditional conditional)
    {
        //return a tokentype.Boolean with value of true or false eventually
        var left = Evaluate(conditional.leftExpression);
        var right = Evaluate(conditional.rightExpression);
        if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
        {
            switch (conditional.comparativeToken.Value)
            {
                case "==":
                    return leftInt == rightInt ? "true" : "false";
                case "!=":
                    return leftInt != rightInt ? "true" : "false";
                case "<":
                    return leftInt < rightInt ? "true" : "false";
                case "<=":
                    return leftInt <= rightInt ? "true" : "false";
                case ">":
                    return leftInt > rightInt ? "true" : "false";
                case ">=":
                    return leftInt >= rightInt ? "true" : "false";
            }
        }
        return conditional.comparativeToken.Value switch
        {
            "==" => left == right ? "true" : "false",
            "!=" => left != right ? "true" : "false",
            _ => "error",
        };
    }

    public int GetNumberValue(Token token)
    {
        return token.Type == TokenType.Identifier
            ? int.Parse(_heap.GetValue(token.Value))
            : int.Parse(token.Value);
    }

    public string GetStringValue(Token token)
    {
        return token.Type == TokenType.Identifier
            ? _heap.GetValue(token.Value)
            : token.Value;
    }
}

//simplify monomial expressions
