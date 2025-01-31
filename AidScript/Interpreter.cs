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
//methods are not statements -- make a seperate folder and namespace for them
//add an enum for system functions(write)