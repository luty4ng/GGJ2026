using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 简单的非线程安全 List 对象池。
    /// </summary>
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> s_pool = new Stack<List<T>>();

        public static List<T> Get()
        {
            if (s_pool.Count > 0)
            {
                var list = s_pool.Pop();
                list.Clear();
                return list;
            }

            return new List<T>();
        }

        public static void Release(List<T> list)
        {
            if (list == null) return;
            list.Clear();
            s_pool.Push(list);
        }
    }
}
