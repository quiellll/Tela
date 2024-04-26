// Obtenido de: https://discussions.unity.com/t/is-there-a-way-to-dynamically-check-for-variable-change/148854/3
// Empleado para detectar cambios en las variables de los objetos durante la ejecución y evitar costes computacionales innecesarios.

public class ChangeTrackingWrapper<T>
{
    private T _value;
    private bool _hasChanged;

    public ChangeTrackingWrapper()
    {
    }

    public ChangeTrackingWrapper(T initialValue)
    {
        _value = initialValue;
    }

    /// <summary>
    /// Gets or sets the wrapped value.
    /// </summary>
    public T Value
    {
        get { return _value; }
        set
        {
            if (Equals(_value, value)) return;
            _value = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the wrapped value has changed.
    /// </summary>
    public bool HasChanged
    {
        get { return _hasChanged; }
    }

    /// <summary>
    /// Resets the changed flag.
    /// </summary>
    public void ResetChangedFlag()
    {
        _hasChanged = false;
    }
}