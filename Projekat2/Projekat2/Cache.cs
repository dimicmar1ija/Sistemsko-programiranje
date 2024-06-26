﻿using System;
using System.Collections.Generic;

namespace WebServer
{
    public class LRUCache
    {
        class ListNode // dvostruko spregnuta lancana lista
        {
            public string key;
            public byte[] val;
            public ListNode prev, next;
            public ListNode(string k, byte[] v)
            {
                key = k;
                val = v;
                prev = null;
                next = null;
            }
        }

        private int capacity, size;
        private ListNode head, tail;

        private readonly Dictionary<string, ListNode> map;
        private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public LRUCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be greater than zero.");
            }

            this.capacity = capacity;
            size = 0;
            head = new ListNode(null, null);
            tail = new ListNode(null, null);
            tail.prev = head;
            head.next = tail;
            map = new Dictionary<string, ListNode>();
        }

        public bool ContainsKey(string key)
        {
            return map.ContainsKey(key);
        }

        public byte[] Get(string key)
        {
            try
            {
                locker.EnterWriteLock();
                if (map.TryGetValue(key, out ListNode target))
                {
                    Remove(target);
                    AddToLast(target);
                    return target.val!;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                locker.ExitWriteLock();
            }
            return null;
        }

        public void Set(string key, byte[] value)
        {
            try
            {
                locker.EnterWriteLock();
                if (map.TryGetValue(key, out ListNode target))
                {
                    target.val = value;
                    Remove(target);
                    AddToLast(target);
                }
                else
                {
                    if (size == capacity)
                    {
                        map.Remove(head.next.key);
                        Remove(head.next);
                        --size;
                    }

                    ListNode newNode = new ListNode(key, value);
                    map.Add(key, newNode);
                    AddToLast(newNode);
                    ++size;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private void AddToLast(ListNode target)
        {
            target.next = tail;
            target.prev = tail.prev;
            tail.prev.next = target;
            tail.prev = target;
        }

        private void Remove(ListNode target)
        {
            target.next.prev = target.prev;
            target.prev.next = target.next;
            Console.WriteLine($"Removed {target.key} from cache because it was the least recently used.");
        }
        public void Clear()
        {
            try
            {
                locker.EnterWriteLock();
                map.Clear();
                head.next = tail;
                tail.prev = head;
                size = 0;
                Console.WriteLine("Cache cleared.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}