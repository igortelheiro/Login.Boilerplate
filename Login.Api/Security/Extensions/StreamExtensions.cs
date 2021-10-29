using System.IO;
using System.Text;

namespace Login.Api.Security.Extensions
{
    public static class StreamExtensions
    {
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        public static BinaryWriter CreateWriter(this Stream stream) => new (stream, DefaultEncoding, true);
        public static BinaryReader CreateReader(this Stream stream) => new (stream, DefaultEncoding, true);
    }
}
