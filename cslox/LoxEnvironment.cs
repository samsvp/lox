class LoxEnvironment
{
    public LoxEnvironment? enclosing;
    Dictionary<string, object?> values = new();


    public LoxEnvironment(LoxEnvironment? enclosing=null)
    {
        this.enclosing = enclosing;
    }


    public void Define(string name, object? value)
    {
        values[name] = value;
    }

    public void Assign(Token name, object? value)
    {
        if (values.ContainsKey(name.lexeme))
        {
            values[name.lexeme] = value;
            return;
        }

        if (enclosing != null)
        {
            enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable {name.lexeme}.");
    }


    public void AssignAt(int distance, Token name, object? value) 
    {
        Ancestor(distance).values[name.lexeme] = value;
    }


    public object? Get(Token name)
    {
        if (values.ContainsKey(name.lexeme))
        {
            return values[name.lexeme];
        }

        if (!(enclosing is null)) 
        {
            return enclosing.Get(name);
        }

        throw new RuntimeError(name, $"Undefined variable {name.lexeme}.");
    }


    public object? GetAt(int distance, string name) 
    {
        return Ancestor(distance).values[name];
    }


    LoxEnvironment Ancestor(int distance) 
    {
        LoxEnvironment environment = this;
        for (int i = 0; i < distance; i++) 
        {
            environment = environment.enclosing!; 
        }

        return environment;
    }
}