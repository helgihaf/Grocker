using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class LogReader : IDisposable
    {
        private readonly string filePath;
        private Stream stream;

        private bool isBigEndian;

        private long index;
        private int currentByte;

        private LogReader(string filePath, bool startRead)
        {
            this.filePath = filePath;
            stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            if (startRead)
            {
                NextByte();
                DetectEncoding();
            }
        }

        public LogReader(string filePath) : this(filePath, true)
        {
        }

        public LogReader(string filePath, long index, Encoding encoding) : this(filePath, false)
        {
            this.index = index;
            this.Encoding = encoding;
            stream.Seek(index, SeekOrigin.Begin);
            NextByte();
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        public void ReadLine()
        {
            bool isNewline = false;
            while (currentByte != -1 && !isNewline)
            {
                isNewline = ReadChar();
            }
        }

        private bool ReadChar()
        {
            var result = false;

            if (Encoding == Encoding.UTF8)
            {
                int reduceBytes = 0;

                int b = currentByte;
                if (b == -1)
                {
                    return false;
                }

                if (b >= 0xFC)
                {
                    reduceBytes = 6;
                }
                else if (b >= 0xF8)
                {
                    reduceBytes = 5;
                }
                else if (b >= 0xF0)
                {
                    reduceBytes = 4;
                }
                else if (b >= 0xE0)
                {
                    reduceBytes = 3;
                }
                else if (b >= 0xC0)
                {
                    reduceBytes = 2;
                }
                else
                {
                    result = IsNewline(b);
                    reduceBytes = 1;
                }

                for (int i = 0; i < reduceBytes && currentByte != -1; i++)
                {
                    NextByte();
                }
            }
            else if (Encoding == Encoding.ASCII)
            {
                result = IsNewline(currentByte);
                NextByte();
            }
            else
            {
                throw new NotSupportedException();
            }
            return result;
        }

        private static bool IsNewline(int b)
        {
            return b == 10;
        }

        public void NextByte()
        {
            currentByte = stream.ReadByte();
            if (currentByte >= 0)
            {
                index++;
            }
        }

        public long Index
        {
            get { return index >= 0 ? index - 1 : index; }
        }

        public int CurrentByte
        {
            get
            {
                return currentByte;
            }
        }

        public Encoding Encoding { get; private set; }

        private void DetectEncoding()
        {
            this.Encoding = Encoding.ASCII;
            if (CurrentByte == 0xEF)
            {
                NextByte();
                if (CurrentByte == 0xBB)
                {
                    NextByte();
                    if (CurrentByte == 0xBF)
                    {
                        this.Encoding = Encoding.UTF8;
                        NextByte();
                    }
                }
            }
            else if (CurrentByte == 0xFE)
            {
                NextByte();
                if (CurrentByte == 0xFF)
                {
                    this.Encoding = Encoding.Unicode;
                    isBigEndian = true;
                    NextByte();
                }
            }
            else if (CurrentByte == 0xFF)
            {
                NextByte();
                if (CurrentByte == 0xFE)
                {
                    this.Encoding = Encoding.Unicode;
                    isBigEndian = false;
                    NextByte();
                }
            }
        }




    }
}
