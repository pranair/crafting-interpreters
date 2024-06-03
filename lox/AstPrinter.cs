namespace lox;

class AstPrinter : Visitor<string> {
    public string print(Expr expr)
    {
        return accept(expr);
    }

    public override string visitBinaryExpr(Expr.Binary expr)
    {
        return paranthesize(expr.op.lexeme, expr.left, expr.right);
    }
    public override string visitGroupingExpr(Expr.Grouping expr)
    {
        return paranthesize("group", expr.expression);
    }

    public override string visitLiteralExpr(Expr.Literal expr)
    {
        if (expr.value == null) 
            return "nil";
        else
            return expr.value.ToString();
    }

    public override string visitUnaryExpr(Expr.Unary expr) {
        return paranthesize(expr.op.lexeme, expr.right);
    }

    string paranthesize(string name, params Expr[] expressions)
    {
        string ret = "(" + name;
        foreach (var expression in expressions)
        {
            ret += $" {print(expression)}";
        }
        ret += ")";

        return ret;
    }
}
