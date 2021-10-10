
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Unity.Mathematics;

namespace SLE.Systems.Targeting.Unsafe
{
    using SLE.Systems.Targeting.Data;

    public unsafe struct QuadrantHashMap<TKey, TChunkValue> : IDisposable where TKey : unmanaged where TChunkValue : unmanaged
    {
        private struct Slot
        {
            public int   hashKey;
            public int   next;

            public Chunk chunk;
        }

        public unsafe struct Chunk: IDisposable, IEnumerable<TChunkValue>
        {
            private static readonly TChunkValue _null = default(TChunkValue);

            public Chunk(uint capacity)
            {
                capacity = capacity == 0 ? 1 : capacity;

                _buffer = (TChunkValue*)NativeCode.calloc(capacity, (ulong)sizeof(TChunkValue));
                _bufferCount = 0;
                _bufferCapacity = capacity;

                IsValid = true;
            }

            private TChunkValue* _buffer;
            private uint _bufferCount;
            private uint _bufferCapacity;

            public ref readonly TChunkValue this[int index]
            {
                get
                {
                    if (index >= _bufferCapacity)
                        return ref _null;

                    return ref _buffer[index];
                }
            }
            public int Count => (int)_bufferCount;
            public bool IsValid { get; }

            public void Add(TChunkValue value)
            {
                if (_bufferCount == _bufferCapacity)
                    IncreaseBufferSize();

                _buffer[_bufferCount++] = value;
            }

            private void IncreaseBufferSize()
            {
                uint newCapacity = _bufferCount + 5;
                TChunkValue* newBuffer = (TChunkValue*)NativeCode.calloc(newCapacity, (ulong)sizeof(TChunkValue));

                NativeCode.memcpy(newBuffer, _buffer, (ulong)(newCapacity * sizeof(TChunkValue)));
                NativeCode.free(_buffer);

                _buffer = newBuffer;
                _bufferCapacity = newCapacity;
            }

            public void Remove(TChunkValue value)
            {
                int i;
                for (i = 0; i < _bufferCount; i++)
                {
                    if (_buffer[i].Equals(value))
                    {
                        _buffer[i] = default(TChunkValue);
                        return;
                    }
                }
            }
            public TChunkValue Remove(int index)
            {
                if (index >= _bufferCapacity)
                    return _null;

                TChunkValue value = _buffer[index];
                _buffer[index] = _null;

                return value;
            }
            public bool Contains(TChunkValue value)
            {
                foreach (var item in this)
                {
                    if (item.Equals(value))
                        return true;
                }

                return false;
            }

            public void Dispose()
            {
                NativeCode.free(_buffer);
            }

            public IEnumerator<TChunkValue> GetEnumerator()
            {
                return new Enumerator(this);
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public struct Enumerator: IEnumerator<TChunkValue>
            {
                private readonly TChunkValue* _chunkBuffer;
                private uint _count;
                private int _index;

                internal Enumerator(in Chunk chunk)
                {
                    if (chunk._buffer == null)
                        throw new InvalidOperationException($"The {nameof(Chunk)} wasn't properly initialized");

                    _chunkBuffer = chunk._buffer;
                    _count = chunk._bufferCount;
                    _index = -1;
                }

                public TChunkValue Current => _chunkBuffer[_index];

                object IEnumerator.Current => Current;

                public void Dispose() { }
                public bool MoveNext()
                {
                    if (++_index >= _count)
                        return false;

                    return true;
                }
                public void Reset()
                {
                    _index = -1;
                }
            }
        }

        public static int GetHashKeyFromPosition(in float3 position, in int cellSize, in int heightSize)
        {
            float x = math.floor(position.x / cellSize);
            float z = math.floor(position.z / cellSize);

            int hashKey = (int)(x + (heightSize * z));

            return hashKey;
        }
        public static int GetHashKeyFromPosition(in float3 position, in QuadrantHashMap<TKey, TChunkValue> quadrant)
        {
            float x = math.floor(position.x / quadrant._cellWidth);
            float z = math.floor(position.z / quadrant._cellWidth);

            int hashKey = (int)(x + (quadrant._cellHeight * z));

            return hashKey;
        }

        public QuadrantHashMap(int capacity, int cellWidth = 35, int cellHeight = 1000)
        {
            capacity = capacity == 0 ? 1 : capacity;

            _buffer = (Slot*)NativeCode.calloc((ulong)capacity, (ulong)sizeof(Slot));

            _bufferCount    = 0;
            _bufferCapacity = capacity;
            _cellWidth      = cellWidth;
            _cellHeight     = cellHeight;
        }

        private Slot* _buffer;
        private int   _bufferCount;
        private int   _bufferCapacity;
        private int   _cellWidth;
        private int   _cellHeight;

        public ref readonly Chunk this[TKey key]
        {
            get
            {
                int hash  = key.GetHashCode();
                int index = hash % _bufferCapacity;

                return ref _buffer[index].chunk;
            }
        }

        private int GetIndexForKey(TKey key)
        {
            int index = math.abs(key.GetHashCode());
            
            index %= _bufferCapacity;

            return index;
        }

        public void Add(TKey key, TChunkValue value)
        {
            if(_bufferCount == _bufferCapacity)
            {
                IncreaseBufferSize();

                Add(key, value);
                return;
            }

            int hash  = key.GetHashCode();
            int index = hash % _bufferCapacity;
            int nIndex;

            if (_bufferCount == 0)
            {
                ref Slot slot0 = ref _buffer[0];

                slot0.hashKey = hash;
                slot0.next    = -1;
                slot0.chunk   = new Chunk(1);
                slot0.chunk.Add(value);

                _bufferCount++;

                return;
            }

            ref Slot slot = ref _buffer[index];

            while(slot.hashKey != hash)
            {
                nIndex = index + 1;
                nIndex %= _bufferCapacity;

                slot.next = nIndex;
                index = nIndex;
                slot = ref _buffer[index];
            }

            slot.chunk.Add(value);
        }

        private void IncreaseBufferSize()
        {
            int newCapacity = _bufferCount + 5;
            Slot* newBuffer = (Slot*)NativeCode.calloc((ulong)newCapacity, (ulong)sizeof(Slot));



            NativeCode.free(_buffer);

            _buffer = newBuffer;
            _bufferCapacity = newCapacity;
        }

        public bool TryGetFirstValue(in TKey key, out TChunkValue value, out Chunk.Enumerator enumerator)
        {
            value = default;
            enumerator = default;
            // TODO

            //int index = GetIndexForKey(key);

            //enumerator = new Chunk.Enumerator(_buffer[index].chunk);
            //value = enumerator.Current;
            //
            //if (value.Equals(default(TChunkValue)))
            //    return false;

            return false;
        }
        public void Dispose()
        {
            int i;
            int length = _bufferCount;
            for (i = 0; i < length; i++)
            {
                _buffer[0].chunk.Dispose();
            }

            NativeCode.free(_buffer);
        }
    }
}
