namespace lox;

interface ExprVisitor<R>
{
    public R accept(Expr expr) => expr switch
    {
        Expr.Binary binary => visitBinaryExpr(binary),
        Expr.Grouping grouping => visitGroupingExpr(grouping),
        Expr.Literal literal => visitLiteralExpr(literal),
        Expr.Unary unary => visitUnaryExpr(unary),
        Expr.Variable variable => visitVariableExpr(variable),
        Expr.Assign assign => visitAssignExpr(assign),
        _ => throw new NotImplementedException()
    };

    public R1 accept<R1>(ExprVisitor<R1> visitor)
    {
        throw new NotImplementedException();
    }
    public abstract R visitVariableExpr(Expr.Variable variable);
    public abstract R visitBinaryExpr(Expr.Binary expr);
    public abstract R visitGroupingExpr(Expr.Grouping expr);
    public abstract R visitLiteralExpr(Expr.Literal expr);
    public abstract R visitUnaryExpr(Expr.Unary expr);
    public abstract R visitAssignExpr(Expr.Assign assign);
}

interface StmtVisitor<R>
{
    public R accept(Stmt expr) => expr switch
    {
        Stmt.Print print => visitPrintStmt(print),
        Stmt.Expression exprStmt => visitExpressionStmt(exprStmt),
        Stmt.Var var => visitVariableStmt(var),
        Stmt.Block block => visitBlockStmt(block),
        _ => throw new NotImplementedException()
    };

    public R1 accept<R1>(StmtVisitor<R1> visitor)
    {
        throw new NotImplementedException();
    }
    public abstract R visitVariableStmt(Stmt.Var var);
    public abstract R visitPrintStmt(Stmt.Print expr);
    public abstract R visitExpressionStmt(Stmt.Expression expr);
    public abstract R visitBlockStmt(Stmt.Block block);
}

