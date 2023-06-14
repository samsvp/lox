class Parser
{
    List<Token> tokens;
    int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }


    public List<Stmt?> Parse()
    {
        List<Stmt?> statements = new();

        while(!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }


    Stmt? Declaration()
    {
        try
        {
            if (MatchToken(TokenType.CLASS))
                return ClassDeclaration();
            if (MatchToken(TokenType.FUN))
                return Function("function");
            if (MatchToken(TokenType.VAR))
                return VarDeclaration();

            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }


    Stmt ClassDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect class name");
        
        VariableExpr? superclass = null;
        if (MatchToken(TokenType.LESS))
        {
            Consume(TokenType.IDENTIFIER, "Expect superclass name");
            superclass = new VariableExpr(PreviousToken());
        }
        
        Consume(TokenType.LEFT_BRACE, "Expect '{' before class body");

        List<FunctionStmt> methods = new();
        while(!(IsCurrTokenIsOfType(TokenType.RIGHT_BRACE)) && !IsAtEnd())
        {
            methods.Add(Function("method"));
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after class body");

        return new ClassStmt(name, superclass, methods);
    }


    Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expected variable name.");

        Expr? initializer = null;
        if (MatchToken(TokenType.EQUAL))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
        return new VarStmt(name, initializer);
    }


    Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
        
        Stmt body = Statement();
        return new WhileStmt(condition, body);
    }


    Stmt Statement()
    {
        if (MatchToken(TokenType.FOR))
            return ForStatement();
        if (MatchToken(TokenType.IF))
            return IfStatement();
        if (MatchToken(TokenType.PRINT)) 
            return PrintStatement();
        if (MatchToken(TokenType.RETURN))
            return ReturnStatement();
        if (MatchToken(TokenType.WHILE)) 
            return WhileStatement();
        if (MatchToken(TokenType.LEFT_BRACE))
            return new BlockStmt(Block());

        return ExpressionStatement();
    }


    Stmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

        Stmt? initializer;
        if (MatchToken(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (MatchToken(TokenType.VAR))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }


        Expr? condition = null;
        if (!IsCurrTokenIsOfType(TokenType.SEMICOLON))
        {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

        Expr? increment = null;
        if (!IsCurrTokenIsOfType(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

        Stmt body = Statement();

        if (!(increment is null))
        {
            body = new BlockStmt(
                new List<Stmt?>{
                    body, 
                    new ExpressionStmt(increment) });
        }

        if (condition is null) 
            condition = new LiteralExpr(true);
        body = new WhileStmt(condition, body);

        if (!(initializer is null))
        {
            body = new BlockStmt(
                new List<Stmt?> {initializer, body}
            );
        }

        return body;
    }


    Stmt IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after if.");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after if condition.");

        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;
        if (MatchToken(TokenType.ELSE))
        {
            elseBranch = Statement();
        }

        return new IfStmt(condition, thenBranch, elseBranch);
    }


    Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after value.");
        return new PrintStmt(value);
    }


    Stmt ReturnStatement()
    {
        Token keyword = PreviousToken();
        Expr? value = null;
        if (!IsCurrTokenIsOfType(TokenType.SEMICOLON))
        {
            value = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
        return new ReturnStmt(keyword, value);
    }


    Stmt ExpressionStatement()
    {
        Expr expr = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after expression.");
        return new ExpressionStmt(expr);
    }


    FunctionStmt Function(string kind)
    {
        Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
        Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
        List<Token> parameters = new();
        if (!IsCurrTokenIsOfType(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(PeekToken(), "Can't have more than 255 parameters");
                }
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expected parameter name"));
            } while (MatchToken(TokenType.COMMA));
        }
        Consume(TokenType.RIGHT_PAREN, $"Expect ')' after parameters.");
        Consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");
        List<Stmt?> body = Block();
        return new FunctionStmt(name, parameters, body);
    }


    List<Stmt?> Block()
    {
        List<Stmt?> statements = new();

        while (!IsCurrTokenIsOfType(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "Expected '}' at end of block");
        return statements;
    }


    Expr Expression()
    {
        return Assignment();
    }


    Expr Assignment()
    {
        Expr expr = Or();

        if (MatchToken(TokenType.EQUAL))
        {
            Token equals = PreviousToken();
            Expr value = Assignment();

            if (expr is VariableExpr)
            {
                Token name = ((VariableExpr)expr).name;
                return new AssignExpr(name, value);
            }
            else if (expr is GetExpr)
            {
                var get = (GetExpr)expr;
                return new SetExpr(get.object_, get.name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }


    Expr Or()
    {
        Expr expr = And();

        while (MatchToken(TokenType.OR))
        {
            Token operator_ = PreviousToken();
            Expr right = And();
            expr = new LogicalExpr(expr, operator_, right);
        }

        return expr;
    }


    Expr And()
    {
        Expr expr = Equality();

        while (MatchToken(TokenType.AND))
        {
            Token operator_ = PreviousToken();
            Expr right = Equality();
            expr = new LogicalExpr(expr, operator_, right);
        }

        return expr;
    }


    Expr Equality()
    {
        Expr comparison = Comparison();

        while (MatchToken(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token operator_ = PreviousToken();
            Expr right = Comparison();
            comparison = new BinaryExpr(comparison, operator_, right);
        }

        return comparison;
    }


    Expr Comparison()
    {
        Expr term = Term();

        while (MatchToken(TokenType.GREATER, TokenType.GREATER_EQUAL, 
                          TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token operator_ = PreviousToken();
            Expr right = Term();
            term = new BinaryExpr(term, operator_, right);
        }

        return term;
    }


    Expr Term()
    {
        Expr factor = Factor();

        while (MatchToken(TokenType.MINUS, TokenType.PLUS))
        {
            Token operator_ = PreviousToken();
            Expr right = Factor();
            factor = new BinaryExpr(factor, operator_, right);
        }
        
        return factor;
    }


    Expr Factor()
    {
        Expr unary = Unary();

        while (MatchToken(TokenType.STAR, TokenType.SLASH))
        {
            Token operator_ = PreviousToken();
            Expr right = Unary();
            unary = new BinaryExpr(unary, operator_, right);
        }
        
        return unary;
    }


    Expr Unary()
    {
        if (MatchToken(TokenType.BANG, TokenType.MINUS))
        {
            Token operator_ = PreviousToken();
            Expr right = Unary();
            return new UnaryExpr(operator_, right);
        }

        return Call();
    }


    Expr Call()
    {
        Expr expr = Primary();

        while(true)
        {
            if (MatchToken(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else if (MatchToken(TokenType.DOT))
            {
                Token name = Consume(TokenType.IDENTIFIER, "Expect property name after '.'.");
                expr = new GetExpr(expr, name);
            }
            else
            {
                break;
            }
        }

        return expr;
    }


    Expr FinishCall(Expr callee)
    {
        List<Expr?> arguments = new();

        if (!(IsCurrTokenIsOfType(TokenType.RIGHT_PAREN)))
        {
            do
            {
                if (arguments.Count > 255)
                {
                    Error(PeekToken(), "Can't have more than 255 arguments");
                }
                arguments.Add(Expression());
            } while (MatchToken(TokenType.COMMA));
        }

        Token paren = Consume(TokenType.RIGHT_PAREN, "Expeted ')' after arguments");
        return new CallExpr(callee, paren, arguments);
    }


    Expr Primary()
    {
        if (MatchToken(TokenType.FALSE)) 
            return new LiteralExpr(false);
        if (MatchToken(TokenType.TRUE)) 
            return new LiteralExpr(true);
        if (MatchToken(TokenType.NIL)) 
            return new LiteralExpr(null);

        if (MatchToken(TokenType.NUMBER, TokenType.STRING)) 
        {
            return new LiteralExpr(PreviousToken().literal!);
        }

        if (MatchToken(TokenType.SUPER))
        {
            Token keyword = PreviousToken();
            Consume(TokenType.DOT, "Expect '.' after 'super'");
            Token method = Consume(TokenType.IDENTIFIER, "Expect superclass method name.");
            return new SuperExpr(keyword, method);
        }

        if (MatchToken(TokenType.THIS))
            return new ThisExpr(PreviousToken());

        if (MatchToken(TokenType.IDENTIFIER))
        {
            return new VariableExpr(PreviousToken());
        }

        if (MatchToken(TokenType.LEFT_PAREN)) 
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new GroupingExpr(expr);
        }

        throw Error(PeekToken(), "Expected expression");
    }


    Token Consume(TokenType type, string message)
    {
        if (IsCurrTokenIsOfType(type)) return AdvanceToken();
        throw Error(PeekToken(), message);
    }


    void Synchronize() 
    {
        AdvanceToken();

        while (!IsAtEnd()) 
        {
            if (PreviousToken().type == TokenType.SEMICOLON) 
                return;

            switch (PeekToken().type) 
            {
            case TokenType.CLASS:
            case TokenType.FUN:
            case TokenType.VAR:
            case TokenType.FOR:
            case TokenType.IF:
            case TokenType.WHILE:
            case TokenType.PRINT:
            case TokenType.RETURN:
                return;
            }

            AdvanceToken();
        }
    }


    ParseError Error(Token token, String message) {
        Lox.Error(token, message);
        return new ParseError();
    }


    bool MatchToken(params TokenType[] tokentypes)
    {
        foreach (var type in tokentypes)
        {
            if (IsCurrTokenIsOfType(type))
            {
                AdvanceToken();
                return true;
            }
        }

        return false;
    }

    
    private bool IsCurrTokenIsOfType(TokenType type) 
    {
        if (IsAtEnd()) return false;
        return PeekToken().type == type;
    }


    private Token AdvanceToken() 
    {
        if (!IsAtEnd()) current++;
        return PreviousToken();
    }

    private bool IsAtEnd() 
    {
        return PeekToken().type == TokenType.EOF;
    }

    private Token PeekToken() 
    {
        return tokens[current];
    }

    private Token PreviousToken() 
    {
        return tokens[current - 1];
    }


    private class ParseError : SystemException 
    {}
}