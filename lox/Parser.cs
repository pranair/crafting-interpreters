namespace lox;

class Parser(IList<Token> tokens)
{
    private class ParseException : Exception
    {
    }

    private int current = 0;
    public List<Stmt> parse()
    {
        try
        {
            var statements = new List<Stmt>();

            while (!isAtEnd())
                statements.Add(declaration());

            return statements;
        } catch (ParseException)
        {
            return null;
        }
    }

    private Stmt declaration()
    {
        try
        {
            if (match(TokenType.VAR)) return varDeclaration();

            return statement();
        } catch(ParseException)
        {
            synchronize();
            return null;
        }
    }

    private Stmt varDeclaration()
    {
        Token name = consume(TokenType.IDENTIFIER, "Expect variable name");

        Expr? initializer = null;
        if (match(TokenType.EQUAL))
        {
            initializer = expression();
        }

        consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
        return new Stmt.Var(name, initializer);
    }

    private Stmt statement()
    {
        if (match(TokenType.PRINT)) return printStatement();
        if (match(TokenType.LEFT_BRACE)) return new Stmt.Block(block());
        return expressionStatement();
    }

    private List<Stmt> block()
    {
        List<Stmt> statements = new();

        while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            statements.Add(declaration());

        consume(TokenType.RIGHT_BRACE, "Except '}' after block.");
        return statements;
    }

    private Stmt expressionStatement()
    {
        Expr expr = expression();
        consume(TokenType.SEMICOLON, "Expected ';' after value.");
        return new Stmt.Expression(expr);
    }

    private Stmt printStatement()
    {
        Expr expr = expression();
        consume(TokenType.SEMICOLON, "Expected ';' after value.");
        return new Stmt.Print(expr);
    }

    private Expr equality()
    {
        Expr expr = comparison();

        while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op = previous();
            Expr right = comparison();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Token previous()
    {
        return tokens[current - 1];
    }

    private Expr comparison()
    {
        Expr expr = term();

        while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = previous();
            Expr right = term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr term()
    {
        Expr expr = factor();

        while (match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = previous();
            Expr right = factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr factor()
    {
        Expr expr = unary();

        while (match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = previous();
            Expr right = unary();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr unary()
    {
        if (match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = previous();
            Expr right = unary();
            return new Expr.Unary(op, right);
        }

        return primary();
    }

    private Expr primary()
    {
        if (match(TokenType.FALSE)) return new Expr.Literal(false);
        if (match(TokenType.TRUE)) return new Expr.Literal(true);
        if (match(TokenType.NIL)) return new Expr.Literal(null);

        if (match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(previous().literal);
        }

        if (match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(previous());
        }

        if (match(TokenType.LEFT_PAREN))
        {
            Expr expr = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw error(peek(), "Invalid expression");
    }

    private Token consume(TokenType type, string message)
    {
        if (check(type)) return advance();

        throw error(peek(), message);
    }

    private ParseException error(Token token, string message)
    {
        Lox.error(token, message);
        return new ParseException();
    }

    private Expr expression()
    {
        return assignment();
    }

    private Expr assignment()
    {
        Expr expr = equality();

        if (match(TokenType.EQUAL))
        {
            Token equals = previous();
            Expr value = assignment();

            if (expr is Expr.Variable)
            {
                Token name = ((Expr.Variable)expr).name;
                return new Expr.Assign(name, value);
            }

            error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private bool match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (check(type))
            {
                advance();
                return true;
            }
        }

        return false;
    }

    private void synchronize()
    {
        advance();

        while (!isAtEnd())
        {
            if (previous().type == TokenType.SEMICOLON) return;

            switch (peek().type)
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

            advance();
        }
    }

    private Token advance()
    {
        if (!isAtEnd()) current++;
        return previous();
    }

    private bool check(TokenType type)
    {
        if (isAtEnd()) return false;
        return peek().type == type;
    }

    private Token peek()
    {
        return tokens[current];
    }

    private bool isAtEnd()
    {
        return peek().type == TokenType.EOF;
    }
}
