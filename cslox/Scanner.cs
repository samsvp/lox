class Scanner
{
    private string source;
    private List<Token> tokens = new();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        {"and",    TokenType.AND},
        {"class",  TokenType.CLASS},
        {"else",   TokenType.ELSE},
        {"false",  TokenType.FALSE},
        {"for",    TokenType.FOR},
        {"fun",    TokenType.FUN},
        {"if",     TokenType.IF},
        {"nil",    TokenType.NIL},
        {"or",     TokenType.OR},
        {"print",  TokenType.PRINT},
        {"return", TokenType.RETURN},
        {"super",  TokenType.SUPER},
        {"this",   TokenType.THIS},
        {"true",   TokenType.TRUE},
        {"var",    TokenType.VAR},
        {"while",  TokenType.WHILE},
        {"break",  TokenType.BREAK}
    };

    public Scanner(string source)
    {
        this.source = source;
    }


    public List<Token> ScanTokens() 
    {
        while (!IsAtEnd()) 
        {
            // We are at the beginning of the next lexeme.
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }


    void ScanToken() 
    {
        char c = Advance();
        switch (c) {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/')) 
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) 
                        Advance();
                } else {
                    AddToken(TokenType.SLASH);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            case '\n':
                line++;
                break;

            case '"': ReadString(); break;

            default:
                if (Char.IsDigit(c)) 
                {
                    ReadNumber();
                } 
                else if (IsAlpha(c)) 
                {
                    ReadIdentifier();
                }
                else
                {
                    Lox.Error(line, $"Unexpected character {c}.");
                }
                break;
        }
    }


    char Advance()
    {
        return source[current++];
    }


    void AddToken(TokenType type, object? literal=null)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }


    bool IsAtEnd()
    {
        return current >= source.Length;
    }


    char Peek() 
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }


    char PeekNext() 
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    } 


    bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }


    void ReadString() 
    {
        while (Peek() != '"' && !IsAtEnd()) 
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd()) 
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = source.Substring(start + 1, current - start - 2);
        AddToken(TokenType.STRING, value);
    }


    void ReadNumber()
    {
        while (Char.IsDigit(Peek())) Advance();

        // Look for a fractional part.
        if (Peek() == '.' && Char.IsDigit(PeekNext())) 
        {
            // Consume the "."
            Advance();

            while (Char.IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.NUMBER,
            float.Parse(source.Substring(start, current - start)));
    }


    void ReadIdentifier() 
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = source.Substring(start, current - start);
        if (keywords.TryGetValue(text, out TokenType type))
        {
            AddToken(type);
        }
        else
        {
            AddToken(TokenType.IDENTIFIER);
        }
    }


    bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') 
               || (c >= 'A' && c <= 'Z') 
               || c == '_';
    }

    bool IsAlphaNumeric(char c) 
    {
        return IsAlpha(c) || Char.IsDigit(c);
    }
}