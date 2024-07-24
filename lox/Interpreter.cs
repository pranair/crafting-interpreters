namespace lox.lox;

class Interpreter : ExprVisitor<object>, StmtVisitor<object>
{
    private LoxEnvironment environment = new();
    public class LoxRuntimeException(Token token, string message) :
        Exception(message)
    {
        public readonly Token token = token;
    }

    public void interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                execute(statement);
            }
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
        
    public object visitBinaryExpr(Expr.Binary expr)
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

        // FIXME: This does not work. Seems to be something to do with
        //        double precision.
        return left == right;
    }

    public object visitGroupingExpr(Expr.Grouping expr)
    {
        return evaluate(expr.expression);
    }

    public object visitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }

    public object visitUnaryExpr(Expr.Unary expr)
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
        return (this as ExprVisitor<object>).accept(expr);
    }

    private object execute(Stmt statement)
    {
        return (this as StmtVisitor<object>).accept(statement);
    }

    private void checkNumberOperands(Token token, object left, object right)
    {
        if (left is double && right is double) return;

        throw new LoxRuntimeException(token, "Operands must be numbers");
    }

    public object visitPrintStmt(Stmt.Print expr)
    {
        var value = evaluate(expr.expression);
        Console.WriteLine(stringify(value));
        return null;
    }

    public object visitExpressionStmt(Stmt.Expression expr)
    {
        evaluate(expr.expression);
        return null;
    }

    public object visitVariableExpr(Expr.Variable variable)
    {
        return environment.get(variable.name);
    }

    public object visitVariableStmt(Stmt.Var var)
    {
        object value = null;
        if (var.initializer != null)
            value = evaluate(var.initializer);

        environment.define(var.name.lexeme, value);
        return null;
    }

    public object visitAssignExpr(Expr.Assign assign)
    {
        object value = evaluate(assign.value);
        environment.assign(assign.name, value);
        return value;
    }

    public object visitBlockStmt(Stmt.Block block)
    {
        executeBlock(block.statements, new LoxEnvironment(environment));
        return null;
    }

    private void executeBlock(List<Stmt> statements, LoxEnvironment loxEnvironment)
    {
        LoxEnvironment previous = this.environment;
        try
        {
            environment = loxEnvironment;

            foreach (Stmt statement in statements)
                execute(statement);
        } finally
        {
            this.environment = previous;
        }
    }

    public object visitLogicalExpr(Expr.Logical logical)
    {
        object left = evaluate(logical.left);

        if (logical.op.type == TokenType.OR)
        {
            if (isTruthy(left)) return left;
        } else
        {
            if (!isTruthy(left)) return left;
        }

        return evaluate(logical.right);
    }

    public object? visitIfStmt(Stmt.If ifStmt)
    {
        if(isTruthy(evaluate(ifStmt.condition)))
        {
            execute(ifStmt.thenBranch);
        } else if (ifStmt.elseBranch != null)
        {
            execute(ifStmt.elseBranch);
        }

        return null;
    }

    public object? visitWhileStmt(Stmt.While whileStmt)
    {
        while (isTruthy(evaluate(whileStmt.condition)))
        {
            execute(whileStmt.body);
        }

        return null;
    }
}

