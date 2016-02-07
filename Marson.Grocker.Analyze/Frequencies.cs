using System;
using System.Collections.Generic;
using System.Linq;

namespace Marson.Grocker.Analyze
{
    public class Frequencies<T>
    {
        protected readonly Dictionary<T, int> dictionary = new Dictionary<T, int>();

        public class Item
        {
            public T Value { get; set; }
            public int Count { get; set; }
        }

        public void Add(T value)
        {
            if (!dictionary.ContainsKey(value))
            {
                dictionary.Add(value, 1);
            }
            else
            {
                dictionary[value]++;
            }
        }

        public void Add(Frequencies<T> frequencies)
        {
            foreach (var kvp in frequencies.dictionary)
            {
                if (!dictionary.ContainsKey(kvp.Key))
                {
                    dictionary.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    dictionary[kvp.Key] += kvp.Value;
                }
            }
        }


        public IEnumerable<Item> GetItems()
        {
            return dictionary.Select(p => new Item { Value = p.Key, Count = p.Value });
        }

    }
}