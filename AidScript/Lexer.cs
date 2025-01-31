namespace AidScript;

public class Lexer
{
    private readonly List<Line> _tokenLines = [];

    //testing only
    private string _input = "";

    public Lexer() { }

    public List<Line> Tokenize() //for testing internal input
    {
        return Tokenize(_input);
    }

    public List<Line> Tokenize(string input)
    {
        var rawLines = input.Split('\n');

        for (int i = 0 ; i < rawLines.Length; i++)
        {
            var rawLine = rawLines[i];
            var line = TokenizeLine(rawLine, i);
            _tokenLines.Add(line);
        }
        return _tokenLines;
    }

    public Line TokenizeLine(string rawLine, int lineNumber)
    {
        List<Token> tokens = [];
        int i = 0;
        while (i < rawLine.Length)
        {
            var value = "";
            var type = TokenType.Unassigned;

            if (char.IsNumber(rawLine[i])) //for numbers
            {
                (value, i) = BuildNumber(rawLine, i);
                type = TokenType.Number;
            }
            else if (char.IsLetter(rawLine[i])) //for any indenifier, checks for keywords first and then just assumes variable name
            {
                (value, i) = BuildIdentifier(rawLine, i);
                if (ReservedTokens.keywords.Contains(value))
                    type = TokenType.Keyword;
                else
                    type = TokenType.Identifier;
            }
            else if (char.IsWhiteSpace(rawLine[i])) //skip whitespace
            {
                i++;
            }
            else if (rawLine[i] == '\'') //for strings
            {
                (value, i) = BuildString(rawLine, i);
                type = TokenType.String;
            }
            else
            { 
                foreach (KeyValuePair<TokenType, List<string>> kvp in ReservedTokens.operatorTokens) //check for double char token
                {
                    if (kvp.Value.Contains(rawLine[i].ToString() + rawLine[i + 1].ToString()))
                    {
                        value = rawLine[i].ToString() + rawLine[i + 1].ToString();
                        type = kvp.Key;
                        i += 2;
                    }
                }

                foreach (KeyValuePair<TokenType, List<string>> kvp in ReservedTokens.operatorTokens) //check for single char token
                {
                    if (kvp.Value.Contains(rawLine[i].ToString()))
                    {
                        value = rawLine[i].ToString();
                        type = kvp.Key;
                        i++;
                    }
                }
            }

            if (type != TokenType.Unassigned)
            {
                tokens.Add(new Token(value, type));
            }
        }
        return new Line(tokens, lineNumber);
    }

    private static (string, int) BuildNumber(string input, int index)
    {
        var value = input[index].ToString();
        var building = true;
        while (building)
        {
            var nextChar = GetNextChar(input, index);
            if (char.IsNumber(nextChar))
                value += nextChar;
            else
                building = false;
            index++;
        }
        return (value, index);
    }

    private static (string, int) BuildIdentifier(string input, int index)
    {
        var value = input[index].ToString();
        var building = true;
        while (building)
        {
            var nextChar = GetNextChar(input, index);
            if (char.IsLetter(nextChar))
                value += nextChar;
            else
                building = false;
            index++;
        }
        return (value, index);
    }

    private static (string, int) BuildString(string input, int index)
    {
        index++;
        var value = input[index].ToString();
        var building = true;
        while (building)
        {
            var nextChar = GetNextChar(input, index);
            if (nextChar != '\'') //there are other next chars that are not accepted
                value += nextChar;
            else
                building = false;
            index++;
        }
        index++;
        return (value, index);
    }

    private static char GetNextChar(string input, int index)
    {
        if (index < input.Length - 1)
            return input[index + 1];
        else
            return '\n';
    }
}

public static class ReservedTokens //reserved values?
{
    public static readonly List<string> keywords =
    [
        "var",
        "if",
        "else",
        "for",
        "while",
        "break",
        "continue",
        "return",
        "write",
        "end",
    ];
    public static readonly List<string> comparativeOperators = ["<", ">", "<=", ">=", "==", "!="];
    public static readonly List<string> logicalOperators = ["&", "|", "!"];
    public static readonly List<string> assignmentOperators = ["=", "+=", "-=", "*=", "/="];
    public static readonly List<string> arithmeticOperators = ["+", "-", "*", "/"];

    public static readonly Dictionary<TokenType, List<string>> operatorTokens = new()
    {
        { TokenType.ComparativeOperator, comparativeOperators },
        { TokenType.LogicalOperator, logicalOperators },
        { TokenType.AssignmentOperator, assignmentOperators },
        { TokenType.ArithmeticOperator, arithmeticOperators },
    };
}

public class Line(List<Token> tokens, int lineNumber)
{
    public List<Token> tokens = tokens;
    public int number = lineNumber;
}