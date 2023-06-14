class Interpreter : VisitorExpr<object?>,
                    VisitorStmt<int>
{
    Dictionary<Expr, int> locals = new();
    public LoxEnvironment globals = new LoxEnvironment();
    LoxEnvironment environment;

    public Interpreter()
    {
        globals.Define("clock", new Clock());
        environment = globals;
    }


    public void Interpret(Expr expression)
    {
        try
        {
            object? value = Evaluate(expression!);
            Console.WriteLine(Stringfy(value));
        }
        catch(RuntimeError err)
        {
            Lox.RuntimeError(err);
        }
    }


    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError err)
        {
            Lox.RuntimeError(err);
        }
    }


    void Execute(Stmt? stmt)
    {
        stmt?.Accept(this);
    }


    public void Resolve(Expr expr, int depth) 
    {
        locals[expr] = depth;
    }


    public void ExecuteBlock(List<Stmt?> statements, LoxEnvironment environment)
    {
        LoxEnvironment previous = this.environment;
        try
        {
            this.environment = environment;

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.environment = previous;
        }
    }


    public int VisitBlockStmt(BlockStmt stmt)
    {
        ExecuteBlock(stmt.statements, new LoxEnvironment(environment));
        return 0;
    }


    public int VisitClassStmt(ClassStmt stmt)
    {
        object? superclass = null;
        if (stmt.superclass != null)
        {
            superclass = Evaluate(stmt.superclass);
            if (!(superclass is LoxClass))
            {
                throw new RuntimeError(stmt.superclass.name, "Superclass must be a class");
            }
        }

        environment.Define(stmt.name.lexeme, null);

        if (stmt.superclass != null)
        {
            environment = new(environment);
            environment.Define("super", superclass);
        }

        Dictionary<string, LoxFunction> methods = new();
        foreach (var method in stmt.methods)
        {
            LoxFunction function = new(method, environment, method.name.lexeme == "init");
            methods[method.name.lexeme] = function;
        }
        LoxClass loxClass = new LoxClass(stmt.name.lexeme, (LoxClass?)superclass, methods);

        if (superclass != null) 
        {
            environment = environment.enclosing!;
        }

        environment.Assign(stmt.name, loxClass);
        return 0;
    }


    public int VisitExpressionStmt(ExpressionStmt stmt)
    {
        object? value = Evaluate(stmt.expression);
        if (Lox.isRepl)
            Console.WriteLine(Stringfy(value));
        return 0;
    }


    public int VisitFunctionStmt(FunctionStmt stmt)
    {
        LoxFunction function = new(stmt, environment, false);
        environment.Define(stmt.name.lexeme, function);
        return 0;
    }


    public int VisitIfStmt(IfStmt stmt)
    {
        if (IsTruthy(Evaluate(stmt.condition)))
        {
            Execute(stmt.thenBranch);
        }
        else if (!(stmt.elseBranch is null))
        {
            Execute(stmt.elseBranch);
        }
        return 0;
    }


    public int VisitPrintStmt(PrintStmt stmt)
    {
        object? value = Evaluate(stmt.expression);
        Console.WriteLine(Stringfy(value));
        return 0;
    }


    public int VisitReturnStmt(ReturnStmt stmt)
    {
        object? value = null;
        if (!(stmt.value is null)) 
        {
            value = Evaluate(stmt.value);
        }

        throw new Return(value);
    }


    public int VisitVarStmt(VarStmt stmt)
    {
        object? value = null;
        if (!(stmt.initializer is null))
        {
            value = Evaluate(stmt.initializer);
        }
        environment.Define(stmt.name.lexeme, value);
        return 0;
    }


    public int VisitWhileStmt(WhileStmt stmt)
    {
        try
        {
            while(IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
        }
        catch (BreakException)
        {

        }
        
        return 0;
    }


    public object? VisitLiteralExpr(LiteralExpr expr)
    {
        return expr.value;
    }


    public object? VisitLogicalExpr(LogicalExpr expr)
    {
        object? left = Evaluate(expr.left);

        if (expr.operator_.type == TokenType.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!(IsTruthy(left))) return left;
        }

        return Evaluate(expr.right);
    }


    public object? VisitSetExpr(SetExpr expr)
    {
        object? object_ = Evaluate(expr.object_);

        if (!(object_ is LoxInstance))
        {
            throw new RuntimeError(expr.name, "Only instances have fields.");
        }

        object? value = Evaluate(expr.value);
        ((LoxInstance)object_).Set(expr.name, value);
        return value;
    }


    public object? VisitSuperExpr(SuperExpr expr)
    {
        int distance = locals[expr];
        LoxClass superclass = (LoxClass)environment.GetAt(distance, "super")!;
        LoxInstance object_ = (LoxInstance)environment.GetAt(distance - 1, "this")!;
        LoxFunction? method = superclass.FindMethod(expr.method.lexeme)!;
        if (method is null)
        {
            throw new RuntimeError(expr.method, $"Undefined property '{expr.method.lexeme}'.");
        }
        return method.Bind(object_);
    }


    public object? VisitThisExpr(ThisExpr expr)
    {
        return LookUpVariable(expr.keyword, expr);
    }


    public object? VisitGroupingExpr(GroupingExpr expr)
    {
        return Evaluate(expr.expression);
    }


    public object? VisitUnaryExpr(UnaryExpr expr)
    {
        object? right = Evaluate(expr.right);

        switch (expr.operator_.type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                IsOperandNumber(expr.operator_, right);
                return -(float)right!;
        }

        return null;
    }


    public object? VisitBinaryExpr(BinaryExpr expr)
    {
        object? left = Evaluate(expr.left);
        object? right = Evaluate(expr.right);

        switch (expr.operator_.type)
        {
        // arithmetic
        case TokenType.PLUS:
            if (left is float && right is float)
            {
                return (float)left! + (float)right!;
            }
            if (left is string && right is string)
            {
                return (string)left! + (string)right!;
            }
            throw new RuntimeError(expr.operator_, "Operands must be two numbers or two strings.");
        
        case TokenType.MINUS:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! - (float)right!;
        
        case TokenType.SLASH:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! / (float)right!;

        case TokenType.STAR:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! * (float)right!;

        // equality
        case TokenType.GREATER:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! > (float)right!;
        case TokenType.GREATER_EQUAL:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! >= (float)right!;
        case TokenType.LESS:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! < (float)right!;
        case TokenType.LESS_EQUAL:
            AreOperandsNumber(expr.operator_, left, right);
            return (float)left! <= (float)right!;
        case TokenType.BANG_EQUAL: return !IsEqual(left, right);
        case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
        
        default:
            return null;
        }
    }


    public object? VisitCallExpr(CallExpr expr)
    {
        object? callee = Evaluate(expr.callee);

        List<object?> arguments = new();
        foreach (Expr? argument in expr.arguments)
        {
            arguments.Add(Evaluate(argument!));
        }

        if (!(callee is ILoxCallable))
        {
            throw new RuntimeError(expr.paren, "Can only call function and classes");
        }

        ILoxCallable function = (ILoxCallable)callee;
        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.paren, 
                $"Expected {function.Arity()} arguments, but got {arguments.Count}."
            );
        }
        return function.Call(this, arguments);
    }


    public object? VisitGetExpr(GetExpr expr)
    {
        object? object_ = Evaluate(expr.object_);
        if (object_ is LoxInstance)
        {
            return ((LoxInstance)object_).Get(expr.name);
        }

        throw new RuntimeError(expr.name, "Only instances have properties.");
    }


    public object? VisitVariableExpr(VariableExpr expr)
    {
        return LookUpVariable(expr.name, expr);
    }


    object? LookUpVariable(Token name, Expr expr) 
    {
        try
        {
            int distance = locals[expr];
            return environment.GetAt(distance, name.lexeme);
        }
        catch(SystemException)
        {}

        return globals.Get(name);
    }


    public object? VisitAssignExpr(AssignExpr expr)
    {
        object? value = Evaluate(expr.value);
        
        try
        {
            int distance = locals[expr];
            environment.AssignAt(distance, expr.name, value);
        }
        catch(SystemException) 
        {
            globals.Assign(expr.name, value);
        }

        return value;
    }


    object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }


    bool IsTruthy(object? object_)
    {
        if (object_ is null) return false;
        if (object_ is bool) return (bool)object_;
        return true;
    }


    bool IsEqual(object? a, object? b)
    {
        if (a is null && b is null) return true;
        if (a is null) return false;

        return a.Equals(b);
    }


    private void IsOperandNumber(Token operator_, object? operand) 
    {
        if (operand is float) return;
        throw new RuntimeError(operator_, "Operand must be a number.");
    }


    void AreOperandsNumber(Token operator_, object? left, object? right) 
    {
        if (left is float && right is float) return;
        throw new RuntimeError(operator_, "Operands must be numbers.");
    }


    string Stringfy(object? object_)
    {
        if (object_ is null) return "nil";

        if (object_ is float) 
        {
            string text = object_.ToString()!;
            if (text.EndsWith(".0")) 
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return object_.ToString()!;
    }
}


class BreakException : SystemException
{

}


class Return : SystemException
{
    public object? value;

    public Return(object? value)
    {
        this.value = value;
    }
}