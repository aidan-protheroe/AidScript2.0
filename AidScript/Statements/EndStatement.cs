namespace AidScript.Statements;

public class End(int line)
{
    public int Line = line;
    public While? While = null;
}