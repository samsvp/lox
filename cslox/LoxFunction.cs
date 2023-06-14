class LoxFunction : ILoxCallable
{
    FunctionStmt declaration;
    LoxEnvironment closure;
    bool isInitializer;
    public LoxFunction(FunctionStmt declaration, LoxEnvironment closure, bool isInitializer)
    {
        this.declaration = declaration;
        this.closure = closure;
        this.isInitializer = isInitializer;
    }


    public int Arity()
    {
        return declaration.params_.Count;
    }


    public object? Call(Interpreter interpreter, List<object?> arguments)
    {
        LoxEnvironment environment = new(closure);

        for (int i = 0; i < declaration.params_.Count; i++)
        {
            environment.Define(declaration.params_[i].lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(declaration.body, environment);
        }
        catch (Return returnValue)
        {
            if (isInitializer) return closure.GetAt(0, "this");
            return returnValue.value;
        }

        if (isInitializer) return closure.GetAt(0, "this");
        return null;
    }


    public LoxFunction Bind(LoxInstance instance)
    {
        LoxEnvironment environment = new(closure);
        environment.Define("this", instance);
        return new LoxFunction(declaration, environment, isInitializer);
    }


    public override string ToString()
    {
        return $"<fn {declaration.name.lexeme}>";
    }
}