using System;

namespace neon {
    public static class ListExtension
    {
        public static void InsertOrAppend<T>(this List<T> list, int index, T item) {
            if (index >= list.Count) {
                list.Add(item);
                return;
            }
            
            list.Insert(index, item);
        }

        public static void InsertOrAppendRange<T>(this List<T> list, int index, IEnumerable<T> range) {
            if (index >= list.Count) {
                list.AddRange(range);
                return;
            }
            
            list.InsertRange(index, range);
        }
    }
}

