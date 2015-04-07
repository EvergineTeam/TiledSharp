using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TiledSharp
{
    public interface IDocumentLoader
    {
        XDocument Load(string path);
    }
}
