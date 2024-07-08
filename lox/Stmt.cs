namespace lox;

abstract record Stmt
{
    private Stmt() { }

    public sealed record Expression(Expr expression) : Stmt;
    public sealed record Print(Expr expression) : Stmt;
}
