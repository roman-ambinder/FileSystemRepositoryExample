using System;
using System.Collections.Generic;

namespace Roman.Ambinder.Infra.Common.DataTypes
{
    public readonly struct NotNullGuard<T> :
        IEquatable<NotNullGuard<T>>
        where T : class
    {
        private readonly T _value;

        public NotNullGuard(T value)
        {
            _value = value;
        }

        public override bool Equals(object obj)
            => obj is NotNullGuard<T> guard && Equals(guard);

        public bool Equals(NotNullGuard<T> other)
            => EqualityComparer<T>.Default.Equals(_value, other._value);

        public override int GetHashCode()
            => HashCode.Combine(_value);

        public void Use(Action<T> usageInfNotNull,
            Action actionIfNull = null)
        {
            if (_value != null)
                usageInfNotNull(_value);
            else
                actionIfNull?.Invoke();
        }

        public TReturn Use<TReturn>(Func<T, TReturn> usageInfNotNull,
            TReturn defaultValue = default,
            Action actionIfNull = null)
        {
            if (_value != null)
                return usageInfNotNull(_value);

            actionIfNull?.Invoke();

            return defaultValue;
        }

        public static implicit operator NotNullGuard<T>(T source)
            => new NotNullGuard<T>(source);

        public static implicit operator T(NotNullGuard<T> source)
            => source._value;
    }
}