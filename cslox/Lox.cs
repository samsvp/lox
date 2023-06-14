using System;


class Lox
{
    public static bool isRepl { get; private set; }
    private static bool hadError = false;
    private static bool hadRuntimeError = false;
    private static Interpreter interpreter = new Interpreter();


    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: lox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }


    public static void RunFile(string filepath)
    {
        string sourceCode = File.ReadAllText(filepath);
        Run(sourceCode);

        // Indicate error on exit
        if (hadError) Environment.Exit(64);
        if (hadRuntimeError) Environment.Exit(70);
    }


    public static void RunPrompt()
    {
        isRepl = true;
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line is null) break;
            Run(line);
            hadError = false;
        }
    }


    public static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens);
        List<Stmt?> statements = parser.Parse();

        // Stop if there was a syntax error.
        if (hadError) return;

        Resolver resolver = new Resolver(interpreter);
        resolver.resolve(statements);

        // Stop if there was a resolution error.
        if (hadError) return;

        interpreter.Interpret(statements!);
    }


    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }


    public static void Error(Token token, string message)
    {
        if (token.type == TokenType.EOF)
        {
            Report(token.line, " at end", message);
        }
        else
        {
            Report(token.line, $" at '{token.lexeme}'", message);
        }
    }


    public static void RuntimeError(RuntimeError err)
    {
        Console.Error.WriteLine($"{err.Message}\n[line {err.token.line}]");
        hadRuntimeError = true;
    }


    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[ {line} ] Error {where}: {message}");
        hadError = true;
    }
}