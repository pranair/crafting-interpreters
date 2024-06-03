namespace lox;

abstract record Expr
{
    private Expr() {}

    public sealed record Binary(Expr left, Token op, Expr right) : Expr;
    public sealed record Grouping(Expr expression) : Expr;
    public sealed record Literal(object value) : Expr;
    public sealed record Unary(Token op, Expr right) : Expr;
}
