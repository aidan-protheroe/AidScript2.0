namespace AidScript.Statements;

public class If(Conditional conditional)
{
    public Conditional Conditional = conditional;
    public Else Else = null;
    public End End = null;
    public bool Traversed = false; // I added this bc I wasnt sure was was causing problems but I dont think its needed now
}