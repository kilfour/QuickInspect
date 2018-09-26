using System;
using System.Collections.Generic;
using System.Reflection;

namespace QuickInspect
{
    public class NavigatorImplementation<T>
    {
        private readonly object root;

        public NavigatorImplementation(object root)
        {
            this.root = root;
        }

        public NavigatorImplementation<T> ForAllPrimitives(Action<object> action)
        {
            Apply(root, IsAPrimitive, action);
            return this;
        }

        public NavigatorImplementation<T> ForAllPrimitives(Action<PropertyInfo, object> action)
        {
            Apply(root, IsAPrimitive, action);
            return this;
        }

        public NavigatorImplementation<T> ForAllPrimitives(Func<PropertyInfo, bool> predicate, Action<object> action)
        {
            Apply(root, pi => IsAPrimitive(pi) && predicate(pi), action);
            return this;
        }

        public NavigatorImplementation<T> ForAllPrimitives(Func<PropertyInfo, bool> predicate, Action<PropertyInfo, object> action)
        {
            Apply(root, (pi, p) => IsAPrimitive(pi) && predicate(pi), action);
            return this;
        }

        public NavigatorImplementation<T> ForAll(Func<PropertyInfo, object, bool> predicate, Action<PropertyInfo, object> action)
        {
            Apply(root, predicate, action);
            return this;
        }

        private void Apply(object target, Func<PropertyInfo, object, bool> predicate, Action<PropertyInfo, object> action)
        {
            foreach (var propertyInfo in Properties(target))
            {
                var value = propertyInfo.GetValue(target, null);
                if (predicate(propertyInfo, value))
                {
                    action(propertyInfo, value);
                }
            }
        }

        public NavigatorImplementation<T> ForAll(Func<PropertyInfo, bool> predicate, Action<PropertyInfo, object> action)
        {
            Apply(root, predicate, action);
            return this;
        }

        public NavigatorImplementation<T> ForAll(Action<PropertyInfo, object> action)
        {
            Apply(root, p => true, action);
            return this;
        }

        private void Apply(object target, Func<PropertyInfo, bool> predicate, Action<PropertyInfo, object> action)
        {
            foreach (var propertyInfo in Properties(target))
            {
                if (predicate(propertyInfo))
                {
                    action(propertyInfo, propertyInfo.GetValue(target, null));
                }
            }
        }

        public NavigatorImplementation<T> ForAll(Func<PropertyInfo, bool> predicate, Action<object> action)
        {
            Apply(root, predicate, action);
            return this;
        }

        public NavigatorImplementation<T> ForAll(Action<object> action)
        {
            Apply(root, p => true, action);
            return this;
        }

        private void Apply(object target, Func<PropertyInfo, bool> predicate, Action<object> action)
        {
            foreach (var propertyInfo in Properties(target))
            {
                if (predicate(propertyInfo))
                {
                    action(propertyInfo.GetValue(target, null));
                }
            }
        }

        private IEnumerable<PropertyInfo> Properties(object target)
        {
            return target.GetType().GetProperties(MyBinding.Flags);
        }

        private bool IsAPrimitive(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.Namespace == null)
                return false;

            if (propertyInfo.PropertyType.Namespace.StartsWith("System.Collections"))
                return false;

            if (propertyInfo.PropertyType.Namespace.StartsWith("System"))
                return true;

            return false;
        }
    }
}