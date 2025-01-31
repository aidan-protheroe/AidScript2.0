namespace AidScript.Statements;

public class Else(int line)
{
    public int Line = line;
    public End? End = null; //this shouldn't be nullable, it is required
    public If? If = null; //you only need this or the conditional not both --actually since it uses If.Travertsed now you really don't need eitehr
}
