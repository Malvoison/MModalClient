using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

using CefSharp;

namespace ClientHostCef.MVVM
{
    public class ViewModelBase : ObjectBase, INotifyPropertyChanged
    {
        private PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { if (!IsDisposed) propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        protected virtual void OnPropertyChanged<T>(T oldValue, T newValue, PropertyChangedEventArgs e)
        {
            var handlers = propertyChanged;
            if (handlers == null)
            {
                return;
            }

            handlers(this, e);
        }

        public static PropertyChangedEventArgs GetArgs<T>(Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentException("propertyExpression");
            }

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Lambda must return a property");
            }

            return new PropertyChangedEventArgs(body.Member.Name);
        }

        protected void Set<T>(ref T field, T value, PropertyChangedEventArgs e)
        {
            var oldValue = field;

            if (EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                return;
            }

            field = value;

            OnPropertyChanged(oldValue, value, e);
        }
    }
}
