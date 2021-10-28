using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.Login.Api.Security.Extensions
{
    public static class StreamExtensions
    {
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        public static BinaryWriter CreateWriter(this Stream stream) => new (stream, DefaultEncoding, true);
        public static BinaryReader CreateReader(this Stream stream) => new (stream, DefaultEncoding, true);
    }
}
