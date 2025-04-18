using System;

namespace neon {
    public static class DictionaryExtension
    {
        public static U GetOrCreateValue<T, U>(this Dictionary<T, U> dictionary, T key) where U : new() {
            if (dictionary.TryGetValue(key, out U item) == false) {
                item = new();
                dictionary.Add(key, item);
            }

            return item;
        }
    }
}

