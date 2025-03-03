namespace lox;

abstract record Expr
{
    private Expr() {}

    public sealed record Binary(Expr left, Token op, Expr right) : Expr;
    public sealed record Call(Expr callee, Token paren, List<Expr> arguments): Expr;
    public sealed record Grouping(Expr expression) : Expr;
    public sealed record Literal(object value) : Expr;
    public sealed record Unary(Token op, Expr right) : Expr;
    public sealed record Variable(Token name) : Expr;
    public sealed record Assign(Token name, Expr value): Expr;
    public sealed record Logical(Expr left, Token op, Expr right): Expr;
}
