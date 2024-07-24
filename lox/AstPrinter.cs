namespace lox;

class AstPrinter : ExprVisitor<string> {
    public string print(Expr expr)
    {
        return (this as ExprVisitor<string>).accept(expr);
    }

    public string visitAssignExpr(Expr.Assign assign)
    {
        throw new NotImplementedException();
    }

    public string visitBinaryExpr(Expr.Binary expr)
    {
        return paranthesize(expr.op.lexeme, expr.left, expr.right);
    }
    public string visitGroupingExpr(Expr.Grouping expr)
    {
        return paranthesize("group", expr.expression);
    }

    public string visitLiteralExpr(Expr.Literal expr)
    {
        if (expr.value == null) 
            return "nil";
        else
            return expr.value.ToString();
    }

    public string visitLogicalExpr(Expr.Logical logical)
    {
        throw new NotImplementedException();
    }

    public string visitUnaryExpr(Expr.Unary expr) {
        return paranthesize(expr.op.lexeme, expr.right);
    }

    public string visitVariableExpr(Expr.Variable variable)
    {
        throw new NotImplementedException();
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
