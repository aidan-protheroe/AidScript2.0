namespace AidScript;

public class Interpreter
{
    public void Run(string rawInput)
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(rawInput);
        var parser = new Parser(tokens);
        var ast = parser.Parse();
        var executor = new Executor(ast, new Heap());
        executor.Execute();
    }
}

//comments and inline comments
