class LoxClass : ILoxCallable
{
    public string name;
    Dictionary<string, LoxFunction> methods;
    public LoxClass? superclass;

    public LoxClass(string name, LoxClass? superclass, Dictionary<string, LoxFunction> methods)
    {
        this.name = name;
        this.superclass = superclass;
        this.methods = methods;
    }


    public object? Call(Interpreter interpreter, List<object?> arguments)
    {
        LoxInstance instance = new(this);
        LoxFunction? initializer = FindMethod("init");
        if (initializer != null)
        {
            initializer.Bind(instance).Call(interpreter, arguments);
        }
        return instance;
    }


    public int Arity()
    {
        LoxFunction? initializer = FindMethod("init");
        if (initializer is null) return 0;
        return initializer.Arity();
    }


    public LoxFunction? FindMethod(string name)
    {
        if (methods.ContainsKey(name))
        {
            return methods[name];
        }

        if (superclass != null)
        {
            return superclass.FindMethod(name);
        }

        return null;
    }


    public override string ToString()
    {
        return name;
    }
}