interface ILoxCallable
{
    public int Arity();
    public object? Call(Interpreter interpreter, List<object?> arguments);
}


class Clock : ILoxCallable
{
    public int Arity()
    {
        return 0;
    }


    public object? Call(Interpreter interpreter, List<object?> arguments)
    {
        return (float)(System.Environment.TickCount / 1000.0);
    }


    override public string ToString()
    {
        return "native <fn> clock";
    }
}