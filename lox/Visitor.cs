namespace lox;

abstract class Visitor<R>
{
    public R accept(Expr expr) => expr switch
    {
        Expr.Binary binary => visitBinaryExpr(binary),
        Expr.Grouping grouping => visitGroupingExpr(grouping),
        Expr.Literal literal => visitLiteralExpr(literal),
        Expr.Unary unary => visitUnaryExpr(unary),
        _ => throw new NotImplementedException()
    };

    public R1 accept<R1>(Visitor<R1> visitor)
    {
        throw new NotImplementedException();
    }

    public abstract R visitBinaryExpr(Expr.Binary expr);
    public abstract R visitGroupingExpr(Expr.Grouping expr);
    public abstract R visitLiteralExpr(Expr.Literal expr);
    public abstract R visitUnaryExpr(Expr.Unary expr);
}