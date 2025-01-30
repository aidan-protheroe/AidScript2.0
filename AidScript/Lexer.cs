namespace AidScript;

public class Lexer
{
    private readonly List<List<Token>> _tokenLines = [];
    private List<Token> _tokens = [];

    //testing only
    private string _input = "";

    public Lexer() { }

    public List<List<Token>> Tokenize() //for testing internal input
    {
        return Tokenize(_input);
    }

    public List<List<Token>> Tokenize(string input)
    {
        var lines = input.Split('\n');

        string value;
        TokenType type;

        foreach (var line in lines)
        {
            int i = 0;
            while (i < line.Length)
            {
                value = "";
                type = TokenType.Unassigned;

                if (char.IsNumber(line[i])) //for numbers
                {
                    (value, i) = BuildNumber(line, i);
                    type = TokenType.Number;
                }
                else if (char.IsLetter(line[i])) //for any indenifier, checks for keywords first and then just assumes variable name
                {
                    (value, i) = BuildIdentifier(line, i);
                    if (ReservedTokens.keywords.Contains(value))
                        type = TokenType.Keyword;
                    else
                        type = TokenType.Identifier;
                }
                else if (char.IsWhiteSpace(line[i])) //skip whitespace
                {
                    i++;
                }
                else if (line[i] == '\'') //for strings
                {
                    (value, i) = BuildString(line, i);
                    type = TokenType.String;
                }
                else
                { //this must be simplified
                    foreach (KeyValuePair<TokenType, List<string>> kvp in ReservedTokens.operatorTokens)
                    {
                        if (kvp.Value.Contains(line[i].ToString() + line[i + 1].ToString()))
                        {
                            value = line[i].ToString() + line[i + 1].ToString();
                            type = kvp.Key;
                            i += 2;
                        }
                    }

                    foreach (
                        KeyValuePair<TokenType, List<string>> kvp in ReservedTokens.operatorTokens
                    )
                    {
                        if (kvp.Value.Contains(line[i].ToString()))
                        {
                            value = line[i].ToString();
                            type = kvp.Key;
                            i++;
                        }
                    }
                }

                if (type != TokenType.Unassigned)
                {
                    _tokens.Add(new Token(value, type));
                }
            }
            _tokenLines.Add(_tokens);
            _tokens = [];
        }
        return _tokenLines;
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
