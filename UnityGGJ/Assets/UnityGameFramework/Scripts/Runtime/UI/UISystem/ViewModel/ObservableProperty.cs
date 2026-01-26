namespace UnityGameFramework.Runtime
{
    public class ObservableProperty<T>
    {
        private T _value;
        public event System.Action<T> OnValueChanged;
        public ObservableProperty() { }
        public ObservableProperty(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

}
