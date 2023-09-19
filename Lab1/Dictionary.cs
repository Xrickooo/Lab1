﻿using Lab1;
using System;
using System.Collections;

namespace Lab1
{
    class Dictionary<TKey, TValue> : IEnumerable
    {
        private int size = 100;
        private Item<TKey, TValue>[] Items;
        private List<TKey> Keys = new List<TKey>();

        public event Action<TKey, TValue> ItemAdded;

        public event Action<TKey> ItemRemoved;

        public event Action DictionaryCleared;

        public Dictionary()
        {
            Items = new Item<TKey, TValue>[size];

            ItemAdded += (key, value) => Console.WriteLine($"Element with key {key} and value {value} added.");
            ItemRemoved += key => Console.WriteLine($"Element with key {key} is removed.");
            DictionaryCleared += () => Console.WriteLine("Dictionary is cleared."); 
        }

        public void Add(Item<TKey, TValue> item)
        {
            var hash = GetHash(item.Key);

            if (Keys.Contains(item.Key))
            {
                return;
            }

            if (Items[hash] == null)
            {
                Keys.Add(item.Key);
                Items[hash] = item;
                ItemAdded?.Invoke(item.Key, item.Value);
            }
            else
            {
                var placed = false;
                for (var i = hash; i < size; i++)
                {
                    if (Items[i] == null)
                    {
                        Keys.Add(item.Key);
                        Items[i] = item;
                        placed = true;
                        ItemAdded?.Invoke(item.Key, item.Value);
                        break;
                    }

                    if (Items[i].Key.Equals(item.Key))
                    {
                        return;
                    }
                }

                if (!placed)
                {
                    for (var i = 0; i < hash; i++)
                    {
                        if (Items[i] == null)
                        {
                            Keys.Add(item.Key);
                            Items[i] = item;
                            placed = true;
                            ItemAdded?.Invoke(item.Key, item.Value);
                            break;
                        }

                        if (Items[i].Key.Equals(item.Key))
                        {
                            return;
                        }
                    }
                }

                if (!placed)
                {
                    throw new Exception("Out of dictionary range");
                }
            }
        }

        public void Remove(TKey key)
        {
            var hash = GetHash(key);

            if (!Keys.Contains(key))
            {
                return;
            }

            if (Items[hash] == null)
            {
                for (var i = 0; i < size; i++)
                {
                    if (Items[i] != null && Items[i].Key.Equals(key))
                    {
                        Items[i] = null;
                        Keys.Remove(key);
                        ItemRemoved?.Invoke(key);
                        return;
                    }
                }

                return;
            }

            if (Items[hash].Key.Equals(key))
            {
                Items[hash] = null;
                Keys.Remove(key);
                ItemRemoved?.Invoke(key);
            }
            else
            {
                var placed = false;
                for (var i = hash; i < size; i++)
                {
                    if (Items[i] == null)
                    {
                        return;
                    }

                    if (Items[i].Key.Equals(key))
                    {
                        Items[i] = null;
                        Keys.Remove(key);
                        ItemRemoved?.Invoke(key);
                        return;
                    }
                }

                if (!placed)
                {
                    for (var i = 0; i < hash; i++)
                    {
                        if (Items[i] == null)
                        {
                            return;
                        }

                        if (Items[i].Key.Equals(key))
                        {
                            Items[i] = null;
                            Keys.Remove(key);
                            ItemRemoved?.Invoke(key);
                            return;
                        }
                    }
                }
            }
        }

        public TValue Search(TKey key)
        {
            var hash = GetHash(key);

            if (!Keys.Contains(key))
            {;
                return default(TValue);
            }

            if (Items[hash] == null)
            {
                foreach (var item in Items)
                {
                    if (item.Key.Equals(key))
                    {
                        return item.Value;
                    }
                }

                return default(TValue);
            }

            if (Items[hash].Key.Equals(key))
            {
                return Items[hash].Value;
            }
            else
            {
                var placed = false;
                for (var i = hash; i < size; i++)
                {
                    if (Items[i] == null)
                    {
                        return default(TValue);
                    }

                    if (Items[i].Key.Equals(key))
                    {
                        return Items[i].Value;
                    }
                }

                if (!placed)
                {
                    for (var i = 0; i < hash; i++)
                    {
                        if (Items[i] == null)
                        {
                            return default(TValue);
                        }

                        if (Items[i].Key.Equals(key))
                        {
                            return Items[i].Value;
                        }
                    }
                }
            }

            return default(TValue);
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var existingKey in Keys)
            {
                if (existingKey.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsValue(TValue value)
        {
            foreach (var item in Items)
            {
                if (item != null && item.Value.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var hash = GetHash(key);

            if (Keys.Contains(key))
            {
                if (Items[hash] == null)
                {
                    foreach (var item in Items)
                    {
                        if (item != null && item.Key.Equals(key))
                        {
                            value = item.Value;
                            return true;
                        }
                    }
                }
                else if (Items[hash].Key.Equals(key))
                {
                    value = Items[hash].Value;
                    return true;
                }
                else
                {
                    for (var i = hash; i < size; i++)
                    {
                        if (Items[i] == null)
                        {
                            break;
                        }

                        if (Items[i].Key.Equals(key))
                        {
                            value = Items[i].Value;
                            return true;
                        }
                    }

                    for (var i = 0; i < hash; i++)
                    {
                        if (Items[i] == null)
                        {
                            break;
                        }

                        if (Items[i].Key.Equals(key))
                        {
                            value = Items[i].Value;
                            return true;
                        }
                    }
                }
            }

            value = default(TValue);
            return false;
        }

        public bool TryAdd(Item<TKey, TValue> item)
        {
            if (!Keys.Contains(item.Key))
            {
                Add(item);
                return true;
            }

            return false;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in Items)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        private int GetHash(TKey key)
        {
            return key.GetHashCode() % size;
        }

        public void Clear()
        {
            var keysCopy = new List<TKey>(Keys); 

            foreach (var key in keysCopy)
            {
                Remove(key); 
            }

            DictionaryCleared?.Invoke();
        }
    }
}
