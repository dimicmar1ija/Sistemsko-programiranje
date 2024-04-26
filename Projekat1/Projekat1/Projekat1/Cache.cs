using System;
using System.Collections.Generic;

namespace WebServer
{
    public class LRUCache
    {
        class ListNode // Lancana lista
        {
            public string Key;
            public byte[] Value;
            public ListNode Prev, Next;

            public ListNode(string k, byte[] v)
            {
                Key = k;
                Value = v;
                Prev = null;
                Next = null;
            }
        }

        private int _capacity, _size;
        private ListNode _head, _tail;

        private readonly Dictionary<string, ListNode> _map;

        public bool ArgumentOk = true;

        public LRUCache(int capacity) // Konstruktor
        {
            if (capacity <= 0)
            {
                // Izmena: Bacanje izuzetka umesto postavljanja flaga
                throw new ArgumentException("Capacity must be greater than zero.");
            }

            _capacity = capacity;
            _size = 0;
            _head = new ListNode(null, null);
            _tail = new ListNode(null, null);
            _tail.Prev = _head;
            _head.Next = _tail;
            _map = new Dictionary<string, ListNode>();
        }

        public bool ContainsKey(string key)
        {
            return _map.ContainsKey(key);
        }

        public byte[] Get(string key)
        {
            if (_map.TryGetValue(key, out ListNode target))
            {
                Remove(target);
                AddToLast(target);
                return target.Value;
            }
            return null;
        }

        public void Set(string key, byte[] value)
        {
            if (_map.TryGetValue(key, out ListNode target))
            {
                target.Value = value;
                Remove(target);
                AddToLast(target);
            }
            else
            {
                if (_size == _capacity)
                {
                    _map.Remove(_head.Next.Key);
                    Remove(_head.Next);
                    --_size;
                }

                ListNode newNode = new ListNode(key, value);
                _map.Add(key, newNode);
                AddToLast(newNode);
                ++_size;
            }
        }


        private void AddToLast(ListNode target)
        {
            target.Next = _tail;
            target.Prev = _tail.Prev;
            _tail.Prev.Next = target;
            _tail.Prev = target;
        }

        private void Remove(ListNode target)
        {
            target.Next.Prev = target.Prev;
            target.Prev.Next = target.Next;
            Console.WriteLine($"Removed {target.Key} from cache because it was the least recently used.");
        }
    }
}
