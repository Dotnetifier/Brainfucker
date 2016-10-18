using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Brainfucker
{
    public static class OutputBuffer
    {
        #region Fields

        private static bool _running = true;
        private static ConcurrentQueue<char> _writeOps;
        private static object _locker = new object();
        private static Thread writeThread;

        private static Action<byte> _writeFunction;

        #endregion

        #region Constructor

        static OutputBuffer()
        {
            _writeOps = new ConcurrentQueue<char>();
            _writeFunction = (b) => Console.Write((char)b);
        }

        #endregion

        #region Properties

        public static int BufferTime { get; private set; } = 0;

        #endregion

        #region Methods

        public static void WriteByte(byte character)
        {
            _writeFunction(character);
        }

        private static void Start()
        {
            if (writeThread != null && writeThread.IsAlive)
            {
                return;
            }

            writeThread = new Thread(() =>
            {
                lock (_locker)
                {
                    do
                    {
                        Thread.Sleep(BufferTime);
                        if (_writeOps.Count > 0)
                        {
                            char w;
                            while (_writeOps.TryDequeue(out w))
                            {
                                Console.Write(w);
                            }
                        }
                    } while (_running || _writeOps.Count > 0);
                }
            });
            _running = true;
            writeThread.Start();
        }

        public static void Open(int bufferTime)
        {
            Close();

            BufferTime = bufferTime;
            if (bufferTime >= 0)
            {
                _writeFunction = (b) => _writeOps.Enqueue((char)b); ;
                Start();
            }
            else
            {
                _writeFunction = (b) => Console.Write((char)b);
            }
        }

        public static void Close()
        {
            _running = false;
            lock (_locker) { }
        }

        #endregion
    }
}
