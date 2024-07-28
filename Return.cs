namespace lox.lox;

class Return : Exception
{
    public object? value;

    public Return(object? value)
    {
        this.value = value;
    }
}