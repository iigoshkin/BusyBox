using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BusyBox.AspNetCore
{
    public class ConcurrentCache<TKey, TValue> where TKey : notnull
    {
        enum ComandType
        {
            Read,
            Remove,
            Add
        }

        record struct Command(Node? Node, ComandType Type)
        {
            public Command(ComandType type) : this(null, type)
            {
            }
        }

        class Node
        {
            public TKey Key { get; }

            public TValue? Value { get; }

            public Node? Next { get; set; }

            public Node? Previous { get; set; }

            public Node(TKey key, TValue? value)
            {
                Key = key;
                Value = value;
            }
        }

        private Node? _head;
        private Node? _tail;

        private readonly ConcurrentStack<Command> _commandQueue;

        private readonly ConcurrentDictionary<TKey, Node> _cache;

        private readonly int _cacheSize;
        private readonly int _concurrencyLevel;
        private readonly object _evictionLock = new object();

        public ConcurrentCache(int cacheSize) : this(cacheSize, Environment.ProcessorCount * 2)
        {
        }

        public ConcurrentCache(int cacheSize, int concurrencyLevel)
        {
            _cacheSize = cacheSize;
            _concurrencyLevel = concurrencyLevel;

            _cache = new ConcurrentDictionary<TKey, Node>(_concurrencyLevel, cacheSize);
            _commandQueue = new ConcurrentStack<Command>();
        }

        public bool TryAdd(TKey key, TValue? value)
        {
            ArgumentNullException.ThrowIfNull(key);
            var node = new Node(key, value);

            EnqueueCommand(node, ComandType.Add);

            Сycle();

            return true;
        }

        public TValue? this[TKey key]
        {
            get => TryGetValue(key, out TValue? value) ? value : throw new KeyNotFoundException();
            set => TryAdd(key, value);
        }

        public void Clear()
        {
            _cache.Clear();
            _head = _tail = null;
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            ArgumentNullException.ThrowIfNull(key);

            var result = _cache.TryGetValue(key, out Node? node);
            if (result && node is not null)
            {
                EnqueueCommand(node, ComandType.Read);

                Сycle();

                value = node.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key) => _cache.ContainsKey(key);

        public int Count => _cache.Count;

        private void EnqueueCommand(Node? node, ComandType type) =>
            _commandQueue.Push(new Command(node, type));

        private void EnqueueCommand(ComandType type) => EnqueueCommand(null, type);

        private void Сycle()
        {
            if (Monitor.TryEnter(_evictionLock))
            {
                try
                {
                    ReadAllCommandChannel();
                }
                finally
                {
                    Monitor.Exit(_evictionLock);
                }
            }
        }

        private void ReadAllCommandChannel()
        {
            int readerCounter = 0;

            while (_commandQueue.TryPop(out var command))
            {
                if (readerCounter < _cacheSize)
                {
                    switch (command.Type)
                    {
                        case ComandType.Add:
                            Add(command);
                            break;
                        case ComandType.Read:
                            Read(command);
                            break;
                        case ComandType.Remove:
                            RemoveLastNode();
                            break;
                    }
                    readerCounter++;
                }
                else
                    break;
            }
        }

        private void Add(Command command)
        {
            Node? currentNode = command.Node;
            if (currentNode is null)
                return;

            if (_head is null)
            {
                _head = currentNode;
                _tail = currentNode;
                _cache.TryAdd(currentNode.Key, currentNode);
                return;
            }

            _head.Previous = currentNode;
            currentNode.Next = _head;
            _head = currentNode;

            _cache.TryAdd(currentNode.Key, currentNode);
            if (_cache.Count > _cacheSize)
                EnqueueCommand(ComandType.Remove);
        }

        private void RemoveLastNode()
        {
            if (_tail is null)
                return;

            TKey key = _tail.Key;
            _tail = _tail.Previous;
            if (_tail is not null)
                _tail.Next = null;

            _cache.TryRemove(key, out _);
        }

        private void Read(Command command)
        {
            Node? node = command.Node;
            if (node is null)
                return;

            if (_head is null)
                return;

            if (node == _head)
                return;

            if (node.Previous != null && node != _tail)
                node.Previous.Next = node.Next;
            else if (node.Previous != null && node == _tail)
            {
                node.Previous.Next = null;
                _tail = node.Previous;
            }

            if (node.Next != null)
                node.Next.Previous = node.Previous;

            node.Next = _head;
            _head.Previous = node;
            node.Previous = null;

            _head = node;
        }
    }
}
