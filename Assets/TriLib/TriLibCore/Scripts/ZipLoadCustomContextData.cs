using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace TriLibCore
{
    /// <summary>Represents a class passed as the custom data to the Asset Loader Context when loading Models from Zip files.</summary>
    public class ZipLoadCustomContextData
    {
        /// <summary>
        /// The zip file to be used.
        /// </summary>
        public ZipFile ZipFile;

        /// <summary>
        /// The stream used to load the zip file.
        /// </summary>
        public Stream Stream;

        /// <summary>
        /// The optional custom data.
        /// </summary>
        public object CustomData;
    }
}