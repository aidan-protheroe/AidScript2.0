namespace AidScript;

public class AbstractSyntaxTree
{
    public int CurrentSetStatement = 0;
    public int CurrentGetStatement = 0;

    //have assignemnts, method, initilaztions, ifs, all inherit from a Statement class and keep one dict?
    public Dictionary<int, Assignment> Assignments = new();
    public Dictionary<int, Method> Methods = new();
    public Dictionary<int, Initialization> Initializations = new();
    public Dictionary<int, If> IfStatements = new();
    public Dictionary<int, Else> ElseStatements = new();
    public Dictionary<int, End> EndStatements = new();

    public void Add(Assignment assignment)
    {
        Assignments.Add(CurrentSetStatement, assignment);
        CurrentSetStatement++;
    }

    public void Add(Method method)
    {
        Methods.Add(CurrentSetStatement, method);
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
        foreach (var m in Methods)
        {
            if (m.Key == CurrentGetStatement)
            {
                CurrentGetStatement++;
                return (StatementType.Method, m.Value);
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

    public void SetIfElseEndPointers()
    {
        var statements = GetAllStatements();
        for (int i = 0; i < statements.Count; i++)
        {
            var s = statements[i];
            if (s.type == StatementType.If)
            {
                var ifStatement = (If)s.value;
                if (ifStatement.End == null)
                {
                    FindIfPointer(statements, i);
                }
            }
        }
    }

    public int FindIfPointer(List<(StatementType type, object value)> statements, int x)
    {
        var ifStatement = (If)statements[x].value;
        int i = x + 1;
        while (i < statements.Count)
        {
            var (type, statement) = statements[i];
            if (type == StatementType.If)
            {
                i = FindIfPointer(statements, i);
            }
            else if (type == StatementType.Else)
            {
                ifStatement.Else = (Else)statement;
                ifStatement.Else.If = ifStatement;
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
    Method,
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
