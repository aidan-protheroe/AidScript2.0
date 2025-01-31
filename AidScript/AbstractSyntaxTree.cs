using AidScript.Statements;
using AidScript.Functions;

namespace AidScript;

public class AbstractSyntaxTree
{
    public int CurrentSetStatement = 0;
    public int CurrentGetStatement = 0;

    //have assignemnts, method, initilaztions, ifs, all inherit from a Statement class and keep one dict?
    public Dictionary<int, Assignment> Assignments = [];
    public Dictionary<int, Function> Functions = [];
    public Dictionary<int, Initialization> Initializations = [];
    public Dictionary<int, If> IfStatements = [];
    public Dictionary<int, Else> ElseStatements = [];
    public Dictionary<int, End> EndStatements = [];

    public void Add(Assignment assignment)
    {
        Assignments.Add(CurrentSetStatement, assignment);
        CurrentSetStatement++;
    }

    public void Add(Function function)
    {
        Functions.Add(CurrentSetStatement, function);
        CurrentSetStatement++;
    }

    public void Add(Initialization initialization)
    {
        Initializations.Add(CurrentSetStatement, initialization);
        CurrentSetStatement++;
    }

    public void Add(If ifStatement)
    {
        IfStatements.Add(CurrentSetStatement, ifStatement);
        CurrentSetStatement++;
    }

    public void Add(Else elseStatement)
    {
        ElseStatements.Add(CurrentSetStatement, elseStatement);
        CurrentSetStatement++;
    }

    public void Add(End endStatement)
    {
        EndStatements.Add(CurrentSetStatement, endStatement);
        CurrentSetStatement++;
    }

    public (StatementType type, object value) GetNextStatement()
    {
        foreach (var a in Assignments)
        {
            if (a.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.Assignment, a.Value);
            }
        }
        foreach (var m in Functions)
        {
            if (m.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.Function, m.Value);
            }
        }
        foreach (var i in Initializations)
        {
            if (i.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.Initialization, i.Value);
            }
        }
        foreach (var f in IfStatements)
        {
            if (f.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.If, f.Value);
            }
        }
        foreach (var e in ElseStatements)
        {
            if (e.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.Else, e.Value);
            }
        }
        foreach (var e in EndStatements)
        {
            if (e.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.End, e.Value);
            }
        }
        return (StatementType.EndOfFile, 0);
    }

    public List<(StatementType type, object value)> GetAllStatements()
    {
        var statements = new List<(StatementType type, object value)>();
        var (type, statement) = GetNextStatement();
        while (type != StatementType.EndOfFile)
        {
            statements.Add((type, statement));
            (type, statement) = GetNextStatement();
        }
        Reset();
        return statements;
    }

    public void Reset()
    {
        CurrentGetStatement = 0;
    }
}

public enum StatementType
{
    Initialization,
    Assignment,
    Function,
    If,
    While,
    Else,
    End,
    Write,
    EndOfFile,
}

//to support ifs and whiles there will have to be a way to set CurrentGetStatement

//add a isEmpty method that returns true if all dictionaries have been traversed
//add a reset method that resets CurrentGetStatement

//should this have a method that connects ifs and elses and ends itself? why do it thru the parser?
