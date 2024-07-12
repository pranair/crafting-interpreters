namespace lox.lox;

public class LoxEnvironment(LoxEnvironment? enclosing = null)
{
    private readonly Dictionary<string, object> values = new();

    public void define(string key, object value) => values[key] = value;

    public object get(Token key) {
        if (values.ContainsKey(key.lexeme))
            return values[key.lexeme];

        if (enclosing != null)
            return enclosing.get(key);

        throw new Interpreter.LoxRuntimeException(key,
            "Undefined variable '" + key.lexeme + "'.");
    }

    public void assign(Token key, object value)
    {
        if (values.ContainsKey(key.lexeme))
        {
            values[key.lexeme] = value;
            return;
        }

        if (enclosing != null)
        {
            enclosing.assign(key, value);
            return;
        }

        throw new Interpreter.LoxRuntimeException(key,
            "Undefined variable '" + key.lexeme + "'.");
    }
}