using lox.lox;

namespace lox;

class LoxFunction : LoxCallable
{
    private readonly Stmt.Function declaration;

    public LoxFunction(Stmt.Function _declaration)
    {
        declaration = _declaration;
    }

    public int arity()
    {
        return declaration.parameters.Count;
    }

    public object call(Interpreter interpreter, List<object> arguments)
    {
        LoxEnvironment environment = new LoxEnvironment(interpreter.globals);
        for (int i = 0; i < declaration.parameters.Count; i++) {
            environment.define(declaration.parameters[i].lexeme,
                arguments[i]);
        }

        try
        {
            interpreter.executeBlock(declaration.body, environment);
        } catch (Return ret)
        {
            return ret.value;
        }

        return null;
    }

    public string ToString()
    {
        return "<fn " + declaration.name.lexeme + ">";
    }
}
