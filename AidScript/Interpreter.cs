namespace AidScript;

public class Interpreter
{
    public void Run(string rawInput)
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(rawInput);
        var parser = new Parser();
        var ast = parser.Parse(tokens);
        var executor = new Executor(ast, new Heap());
        executor.Execute();
    }
}


//biggest todos:
//while loops
//user input
//random number generator
//order of operations
//comments and inline comments

//add a base statement that all statements inherit from

//Store system function info (what arguments it can take, what it returns or performs)
