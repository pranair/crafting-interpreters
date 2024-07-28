namespace lox.lox;

interface LoxCallable
{
    public int arity();

    public object call(Interpreter interpreter, List<object> arguments);
}