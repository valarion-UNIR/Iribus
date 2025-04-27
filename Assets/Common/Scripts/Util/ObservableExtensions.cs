public static class ObservableExtensions
{
    public delegate void ObservableChangedCallback<T>(T oldValue, T newValue);

    public static void SetValue<T>(ref T currentValue, T newValue, ObservableChangedCallback<T> beforeOnChangeEvent, ObservableChangedCallback<T> onChangeEvent)
    {
        if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return;
        var oldValue = currentValue;
        currentValue = newValue;
        beforeOnChangeEvent?.Invoke(oldValue, newValue);
        onChangeEvent?.Invoke(oldValue, newValue);
    }
}