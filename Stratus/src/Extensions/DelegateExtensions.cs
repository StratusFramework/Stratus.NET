using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Extensions
{
    public static class DelegateExtensions
    {
        public static Predicate<T> ToPredicate<T>(this Func<T, bool> func)
        {
            return new Predicate<T>(func);
        }

        public static Func<T, bool> ToFunc<T>(this Predicate<T> predicate)
        {
            return new Func<T, bool>(predicate);
        }

        public static bool TryInvoke(this Action action)
        {
            if (action != null)
            {
                action();
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1>(this Action<T1> action, T1 arg1)
        {
            if (action != null)
            {
                action(arg1);
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                action(arg1, arg2);
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3);
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
                return true;
            }
            return false;
        }

        public static Predicate<T> Or<T>(this IEnumerable<Predicate<T>> predicates)
        {
            return delegate (T item)
            {
                foreach (Predicate<T> predicate in predicates)
                {
                    if (predicate(item))
                    {
                        return true;
                    }
                }
                return false;
            };
        }

        public static Predicate<T> And<T>(this IEnumerable<Predicate<T>> predicates)
        {
            return delegate (T item)
            {
                foreach (Predicate<T> predicate in predicates)
                {
                    if (!predicate(item))
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        /// <summary>
        /// Invokes the delegates with the given arguments, removing any delegates that are invalid
        /// </summary>
        public static void InvokeNotNull(this List<Delegate> list, params object[] args)
        {
            list.RemoveAll(d => d.Method == null || d.Target == null);
            list.ForEach(d => d.DynamicInvoke(args));
        }
    }
}