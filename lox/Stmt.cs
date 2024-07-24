namespace lox;

abstract record Stmt
{
    private Stmt() { }

    public sealed record Expression(Expr expression) : Stmt;
    public sealed record Print(Expr expression) : Stmt;
    public sealed record Var(Token name, Expr initializer) : Stmt;
    public sealed record Block(List<Stmt> statements) : Stmt;
    public sealed record If(Expr condition, Stmt thenBranch, Stmt? elseBranch) : Stmt;
    public sealed record While(Expr condition, Stmt body) : Stmt;
}
