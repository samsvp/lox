class LoxInstance
{
    LoxClass loxClass;
    Dictionary<string, object?> fields = new();

    public LoxInstance(LoxClass loxClass)
    {
        this.loxClass = loxClass;
    }


    public object? Get(Token name)
    {
        if (fields.ContainsKey(name.lexeme))
        {
            return fields[name.lexeme];
        }

        LoxFunction? method = loxClass.FindMethod(name.lexeme);
        if (method != null) return method.Bind(this);

        throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
    }


    public void Set(Token name, object? value)
    {
        fields[name.lexeme] = value;
    }


    public override string ToString()
    {
        return $"{loxClass.name} instance";
    }
}