using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TiledSharp;

namespace TiledSharpTesting
{
    public class DefaultDocumentLoader : IDocumentLoader
    {
        #region Public Methods
        /// <summary>
        /// Loads the specified document.
        /// </summary>
        /// <param name="path">The path.</param>
        public XDocument Load(string path)
        {
            XDocument xDoc =  XDocument.Load(path);
            
            return xDoc;
        }
        #endregion
    }
}
