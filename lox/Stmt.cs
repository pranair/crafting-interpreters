namespace lox;

abstract record Stmt
{
    private Stmt() { }

    public sealed record Expression(Expr expression) : Stmt;
    public sealed record Print(Expr expression) : Stmt;
    public sealed record Var(Token name, Expr initializer) : Stmt;
    public sealed record Block(List<Stmt> statements) : Stmt;
}
