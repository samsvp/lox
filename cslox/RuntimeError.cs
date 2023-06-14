class RuntimeError : SystemException
{
    public Token token;

    public RuntimeError(Token token, String message) : base(message)
    {
        this.token = token;
    }
}