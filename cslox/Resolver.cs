class Resolver : VisitorExpr<int>, VisitorStmt<int>
{
    private readonly Interpreter interpreter;
    private readonly List<Dictionary<string, bool>> scopes = new();
    private FunctionType currentFunction = FunctionType.NONE;

    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    enum FunctionType
    {
        NONE,
        FUNCTION,
        INITIALIZER,
        METHOD
    }


    enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS
    }


    ClassType currentClass = ClassType.NONE;

    public void resolve(List<Stmt?> statements)
    {
        foreach (Stmt? statement in statements)
        {
            Resolve(statement);
        }
    }

    public int VisitBlockStmt(BlockStmt stmt)
    {
        BeginScope();
        resolve(stmt.statements);
        EndScope();
        return 0;
    }


    public int VisitClassStmt(ClassStmt stmt)
    {
        ClassType enclosingClass = currentClass;
        currentClass = ClassType.CLASS;

        Declare(stmt.name);
        Define(stmt.name);

        if (stmt.superclass != null && stmt.name.lexeme == stmt.superclass.name.lexeme)
        {
            Lox.Error(stmt.superclass.name, "A class can't inherit itself.");
        }

        if (stmt.superclass != null)
        {
            currentClass = ClassType.SUBCLASS;
            Resolve(stmt.superclass);

            BeginScope();
            scopes[scopes.Count - 1]["super"] = true;
        }

        BeginScope();
        scopes[scopes.Count - 1]["this"] = true;
        foreach (var method in stmt.methods)
        {
            var declaration = FunctionType.METHOD;
            if(method.name.lexeme == "init")
            {
                declaration = FunctionType.INITIALIZER;
            }
            ResolveFunction(method, declaration);
        }
        EndScope();

        if (stmt.superclass != null) 
            EndScope();

        currentClass = enclosingClass;
        return 0;
    }

    public int VisitExpressionStmt(ExpressionStmt stmt)
    {
        Resolve(stmt.expression);
        return 0;
    }

    public int VisitFunctionStmt(FunctionStmt stmt)
    {
        Declare(stmt.name);
        Define(stmt.name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return 0;
    }

    public int VisitIfStmt(IfStmt stmt)
    {
        Resolve(stmt.condition);
        Resolve(stmt.thenBranch);
        if (stmt.elseBranch != null)
            Resolve(stmt.elseBranch);
        return 0;
    }

    public int VisitPrintStmt(PrintStmt stmt)
    {
        Resolve(stmt.expression);
        return 0;
    }

    public int VisitReturnStmt(ReturnStmt stmt)
    {
        if (currentFunction == FunctionType.NONE)
        {
            Lox.Error(stmt.keyword, "Cannot return from top-level code.");
        }

        if (stmt.value != null)
        {
            if (currentFunction == FunctionType.INITIALIZER)
            {
                Lox.Error(stmt.keyword, "Can't return a value from an initializer");
            }
            Resolve(stmt.value);
        }

        return 0;
    }


    public int VisitVarStmt(VarStmt stmt)
    {
        Declare(stmt.name);
        if (stmt.initializer != null)
        {
            Resolve(stmt.initializer);
        }
        Define(stmt.name);
        return 0;
    }

    public int VisitWhileStmt(WhileStmt stmt)
    {
        Resolve(stmt.condition);
        Resolve(stmt.body);
        return 0;
    }

    public int VisitAssignExpr(AssignExpr expr)
    {
        Resolve(expr.value);
        ResolveLocal(expr, expr.name);
        return 0;
    }

    public int VisitBinaryExpr(BinaryExpr expr)
    {
        Resolve(expr.left);
        Resolve(expr.right);
        return 0;
    }

    public int VisitCallExpr(CallExpr expr)
    {
        Resolve(expr.callee);

        foreach (Expr? argument in expr.arguments)
        {
            Resolve(argument);
        }

        return 0;
    }


    public int VisitGetExpr(GetExpr expr)
    {
        Resolve(expr.object_);
        return 0;
    }


    public int VisitGroupingExpr(GroupingExpr expr)
    {
        Resolve(expr.expression);
        return 0;
    }

    public int VisitLiteralExpr(LiteralExpr expr)
    {
        return 0;
    }

    public int VisitLogicalExpr(LogicalExpr expr)
    {
        Resolve(expr.left);
        Resolve(expr.right);
        return 0;
    }


    public int VisitSetExpr(SetExpr expr)
    {
        Resolve(expr.value);
        Resolve(expr.object_);
        return 0;
    }


    public int VisitSuperExpr(SuperExpr expr)
    {
        if (currentClass == ClassType.NONE) 
        {
            Lox.Error(expr.keyword, "Can't use 'super' outside of a class.");
        } 
        else if (currentClass != ClassType.SUBCLASS) 
        {
            Lox.Error(expr.keyword, "Can't use 'super' in a class with no superclass.");
        }
        ResolveLocal(expr, expr.keyword);
        return 0;
    }


    public int VisitThisExpr(ThisExpr expr)
    {
        if (currentClass == ClassType.NONE)
        {
            Lox.Error(expr.keyword, "Can't use 'this' outside of a class.");
            return 0;
        }
        ResolveLocal(expr, expr.keyword);
        return 0;
    }


    public int VisitUnaryExpr(UnaryExpr expr)
    {
        Resolve(expr.right);
        return 0;
    }

    public int VisitVariableExpr(VariableExpr expr)
    {
        //jlox: (!scopes.isEmpty() && scopes.peek().get(expr.name.lexeme) == Boolean.FALSE)
        if (scopes.Count > 0 && scopes[scopes.Count - 1].ContainsKey(expr.name.lexeme))
        {
            if (scopes[scopes.Count - 1][expr.name.lexeme] == false) // declared but not yet defined
                Lox.Error(expr.name, "Cannot read local variable in its own initializer.");
        }

        ResolveLocal(expr, expr.name);
        return 0;
    }

    private void Resolve(Stmt? stmt)
    {
        stmt?.Accept(this);
    }

    private void Resolve(Expr? expr)
    {
        expr?.Accept(this);
    }

    private void ResolveFunction(FunctionStmt function, FunctionType type)
    {
        FunctionType enclosingFunction = currentFunction;
        currentFunction = type;

        BeginScope();
        foreach (Token param in function.params_)
        {
            Declare(param);
            Define(param);
        }
        resolve(function.body);
        EndScope();
        currentFunction = enclosingFunction;
    }

    private void BeginScope()
    {
        scopes.Add(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        scopes.RemoveAt(scopes.Count - 1);
    }

    private void Declare(Token name)
    {
        if (scopes.Count < 1)
            return;

        Dictionary<string, bool> scope = scopes[scopes.Count - 1];

        if (scope.ContainsKey(name.lexeme))
        {
            Lox.Error(name, "Variable with this name already declared in this scope.");
        }
        else
        {
            scope.Add(name.lexeme, false);
        }            
    }

    private void Define(Token name)
    {
        if (scopes.Count < 1)
            return;
        scopes[scopes.Count - 1][name.lexeme] = true; 
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        for (int i = scopes.Count - 1; i >= 0; i--)
        {
            if (scopes[i].ContainsKey(name.lexeme)) // Csharp doesn't access Stack<> by index (without resorting to Linq), hence used List<> instead
            {
                interpreter.Resolve(expr, scopes.Count - 1 - i);
                return;
            }
        }

        // Not found. Assume it is global.
    }
}