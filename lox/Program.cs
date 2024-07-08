using lox;
using lox.lox;

enum TokenType
{
    // Single-character tokens.
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
    COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

    // One or two character tokens.
    BANG, BANG_EQUAL,
    EQUAL, EQUAL_EQUAL,
    GREATER, GREATER_EQUAL,
    LESS, LESS_EQUAL,

    // Literals.
    IDENTIFIER, STRING, NUMBER,

    // Keywords.
    AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
    PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

    EOF
}

class Token(TokenType type, string lexeme, object literal, int line)
{
    public TokenType type { get; set; } = type;
    public string lexeme { get; set; } = lexeme;
    public object literal { get; set; } = literal;
    public int line { get; set; } = line;

    public override string ToString()
    {
        return $"{type} {lexeme} {literal}";
    }
}

class Lox
{
    private static bool hadError = false;
    private static bool hadRuntimeError = false;
    private static readonly Interpreter interpreter = new Interpreter();

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: slox [script]");
            Environment.Exit(0);
        }
        else if (args.Length == 1)
        {
            runFile(args[0]);
        }
        else
        {
            runPrompt();
        }
    }

    private static async void runFile(string args)
    {
        string bytes = await File.ReadAllTextAsync(Path.GetFileName(args));
        run(bytes);
        if (hadError) Environment.Exit(65);
        if (hadRuntimeError) Environment.Exit(70);
    }

    private static void runPrompt()
    {
        hadError = false;
        string? input;
        while (true)
        {
            Console.Write("> ");
            input = Console.ReadLine();
            if (input == null) break;
            run(input);
            hadError = false;
        }
    }

    private static void run(string source)
    {
        Scanner scanner = new Scanner(source);
        IList<Token> tokens = scanner.scanTokens();
        var statements = new Parser(tokens).parse();

        if (hadError) return;
        
        interpreter.interpret(statements);
    }

    public static void error(int line, string message)
    {
        report(line, "", message);
    }

    public static void error(Token token, string message)
    {
        if (token.type == TokenType.EOF)
        {
            report(token.line, " at end", message);
        }
        else
        {
            report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    private static void report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error: {where}: {message}");
        hadError = true;
    }

    public static void runtimeError(Interpreter.LoxRuntimeException exception)
    {
        Console.WriteLine(exception.Message +
        "\n[line " + exception.token.line + "]");
        hadRuntimeError = true;
    }
}

class Scanner
{
    private string _source;
    public IList<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private Dictionary<string, TokenType> keywords = new() {
        { "and",    TokenType.AND },
        { "class",  TokenType.CLASS },
        { "else",   TokenType.ELSE },
        { "false",  TokenType.FALSE },
        { "for",    TokenType.FOR },
        { "fun",    TokenType.FUN },
        { "if",     TokenType.IF },
        { "nil",    TokenType.NIL },
        { "or",     TokenType.OR },
        { "print",  TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super",  TokenType.SUPER },
        { "this",   TokenType.THIS },
        { "true",   TokenType.TRUE },
        { "var",    TokenType.VAR },
        { "while",  TokenType.WHILE },
    };

    public Scanner(string source)
    {
        _source = source;
    }

    public bool isAtEnd(int lookAhead = 0)
    {
        return current + lookAhead >= _source.Length;
    }

    public IList<Token> scanTokens()
    {
        while (!isAtEnd())
        {
            start = current;
            scanToken();
        }
        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private void scanToken()
    {
        char c = advance();
        switch (c)
        {
            case '(':
                addToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                addToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                addToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                addToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                addToken(TokenType.COMMA);
                break;
            case '.':
                addToken(TokenType.DOT);
                break;
            case '-':
                addToken(TokenType.MINUS);
                break;
            case '+':
                addToken(TokenType.PLUS);
                break;
            case ';':
                addToken(TokenType.SEMICOLON);
                break;
            case '*':
                addToken(TokenType.STAR);
                break;
            case '!':
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (match('/'))
                {
                    // A comment goes until the end of the line.
                    while (peek() != '\n' && !isAtEnd()) advance();
                }
                else
                {
                    addToken(TokenType.SLASH);
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
            case '"':
                handleString();
                break;
            default:
                if (char.IsDigit(c))
                {
                    handleNumber();
                }
                else if (char.IsLetter(c) || c == '_')
                {
                    handleIdentifier();
                }
                else
                {
                    Lox.error(line, "Unexpected character.");
                }
                break;
        }
    }

    private void handleIdentifier()
    {
        while (char.IsLetterOrDigit(peek()) || peek() == '_') advance();

        string text = _source.Substring(start, current - start);
        bool tokenType = keywords.ContainsKey(text);

        if (!tokenType)
            addToken(TokenType.IDENTIFIER);
        else
            addToken(keywords[text]);
    }

    private void handleNumber()
    {
        while (char.IsDigit(peek())) advance();

        if (peek() == '.' && char.IsDigit(peek(1)))
        {
            advance();

            while (char.IsDigit(peek())) advance();
        }

        addToken(TokenType.NUMBER, Convert.ToDouble(_source.Substring(start, current - start)));
    }

    private void handleString()
    {
        while (peek() != '"' && !isAtEnd())
        {
            if (peek() == '\n') line++;
            advance();
        }

        if (isAtEnd())
        {
            Lox.error(line, "Unterminated string.");
        }
        else
        {
            advance();

            string value = _source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, value);
        }
    }

    private char peek(int lookAhead = 0)
    {
        if (isAtEnd(lookAhead)) return '\0';
        return _source[current + lookAhead];
    }

    private bool match(char expected)
    {
        char next = peek();
        if (next == '\0' || next != expected) return false;

        current++;
        return true;
    }

    private char advance() => _source[current++];

    private void addToken(TokenType token) => addToken(token, null);

    private void addToken(TokenType token, object literal)
    {
        string text = _source.Substring(start, current - start);
        tokens.Add(new Token(token, text, literal, line));
    }
}