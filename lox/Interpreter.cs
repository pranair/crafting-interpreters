namespace lox.lox;

class Interpreter : Visitor<object>
{
    public class LoxRuntimeException(Token token, string message) :
        Exception(message)
    {
        public readonly Token token = token;
    }

    public void interpret(Expr expression)
    {
        try
        {
            object result = evaluate(expression);
            Console.WriteLine(stringify(result));
        } 
        catch (LoxRuntimeException e)
        {
            Lox.runtimeError(e);
        }
    }

    private string stringify(object result)
    {
        if (result == null) return "nil";

        if (result is double)
        {
            string str = result.ToString();
            if (str.EndsWith(".0"))
                str = str.Remove(str.Length - 2);
            return str;
        }

        return result.ToString();
    }

    public override object visitBinaryExpr(Expr.Binary expr)
    {
        object left = evaluate(expr.left);
        object right = evaluate(expr.right);

        switch (expr.op.type)
        {
            case TokenType.MINUS:
                checkNumberOperands(expr.op, left, right);
                return (double)left - (double)right;
            case TokenType.SLASH:
                checkNumberOperands(expr.op, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                checkNumberOperands(expr.op, left, right);
                return (double)left * (double)right;
            case TokenType.PLUS:
                if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }

                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }

                throw new LoxRuntimeException(expr.op, "Operands must be numbers or string");
            
            case TokenType.GREATER:
                checkNumberOperands(expr.op, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                checkNumberOperands(expr.op, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                checkNumberOperands(expr.op, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                checkNumberOperands(expr.op, left, right);
                return (double)left <= (double)right;
            case TokenType.BANG_EQUAL:
                return !isEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return isEqual(left, right);
            default:
                throw new NotImplementedException();
        }
    }

    private bool isEqual(object left, object right)
    {
        if (left == null && right == null) return true;
        if (left == null) return false;

        return left == right;
    }

    public override object visitGroupingExpr(Expr.Grouping expr)
    {
        return evaluate(expr.expression);
    }

    public override object visitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }

    public override object visitUnaryExpr(Expr.Unary expr)
    {
        object right = evaluate(expr.right);

        return expr.op.type switch
        {
            TokenType.MINUS => -(double)right,
            TokenType.BANG => !isTruthy(right),
        };
    }

    private bool isTruthy(object value)
    {
        if (value == null) return false;
        if (value is bool) return (bool)value;
        return true;
    }

    private object evaluate(Expr expr)
    {
        return accept(expr);
    }

    private void checkNumberOperands(Token token, object left, object right)
    {
        if (left is double && right is double) return;

        throw new LoxRuntimeException(token, "Operands must be numbers");
    }
}

