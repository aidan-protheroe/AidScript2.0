namespace AidScript;

public class Heap
{
    private Dictionary<string, int> _intValues = [];
    private Dictionary<string, string> _stringValues = [];

    public void AddValue(string key, int value)
    {
        //Console.WriteLine("ADDED INT");
        _intValues.Add(key, value);
    }

    public void AddValue(string key, string value)
    {
        _stringValues.Add(key, value);
    }

    public void UpdateValue(string key, int value)
    {
        //Console.WriteLine("UPDATED INT");
        RemoveValue(key);
        AddValue(key, value);
    }

    public void UpdateValue(string key, string value)
    {
        RemoveValue(key);
        AddValue(key, value);
    }

    public void RemoveValue(string key)
    {
        _intValues.Remove(key);
        _stringValues.Remove(key);
    }

    public string GetValue(string key)
    {
        if (_intValues.TryGetValue(key, out int ivalue))
        {
            return ivalue.ToString();
        }
        else if (_stringValues.TryGetValue(key, out string? svalue))
        {
            return svalue;
        }
        return null;
    }
}
