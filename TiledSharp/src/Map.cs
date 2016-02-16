// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using System.IO;

namespace TiledSharp
{
    public class TmxMap : TmxDocument
    {
        public string Version { get; private set; }
        public RenderOrderType RenderOrder { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int? HexSideLength { get; private set; }
        public OrientationType Orientation { get; private set; }
        public StaggerAxisType StaggerAxis { get; private set; }
        public StaggerIndexType StaggerIndex { get; private set; }
        public TmxColor BackgroundColor { get; private set; }
        public int? NextObjectID { get; private set; }

        public TmxList<TmxTileset> Tilesets { get; private set; }
        public TmxList<TmxLayer> Layers { get; private set; }
        public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }
        public TmxList<TmxImageLayer> ImageLayers { get; private set; }
        public PropertyDict Properties { get; private set; }

        public TmxMap(IDocumentLoader loader, Stream fileStream, string tmxDirectory)
            : base(loader)
        {
            XDocument xDoc = ReadXml(fileStream, tmxDirectory);

            this.ParseTmxMap(xDoc);
        }

        public TmxMap(IDocumentLoader loader, string filename)
            : base(loader)
        {
            XDocument xDoc = ReadXml(filename);
            this.ParseTmxMap(xDoc);
        }

        private void ParseTmxMap(XDocument xDoc)
        {
            var xMap = xDoc.Element("map");

            Version = (string)xMap.Attribute("version");

            Width = (int)xMap.Attribute("width");
            Height = (int)xMap.Attribute("height");
            TileWidth = (int)xMap.Attribute("tilewidth");
            TileHeight = (int)xMap.Attribute("tileheight");
            HexSideLength = (int?)xMap.Attribute("hexsidelength");

            Orientation = (OrientationType)Enum.Parse(
                        typeof(OrientationType),
                        xMap.Attribute("orientation").Value,
                        true);

            // Tile render order
            var renderOrderAttr = xMap.Attribute("renderorder");

            if (renderOrderAttr != null)
            {
                var renderStr = renderOrderAttr.Value.Replace("-", "");

                RenderOrder = (RenderOrderType)Enum.Parse(
                                        typeof(RenderOrderType),
                                        renderStr,
                                        true);
            }

            // Hexagonal stagger axis
            var staggerAxisDict = new Dictionary<string, StaggerAxisType> {
                {"x", StaggerAxisType.X},
                {"y", StaggerAxisType.Y},
            };

            var staggerAxisValue = (string)xMap.Attribute("staggeraxis");
            if (staggerAxisValue != null)
                StaggerAxis = staggerAxisDict[staggerAxisValue];

            // Hexagonal stagger index
            var staggerIndexDict = new Dictionary<string, StaggerIndexType> {
                {"odd", StaggerIndexType.Odd},
                {"even", StaggerIndexType.Even},
            };

            var staggerIndexValue = (string)xMap.Attribute("staggerindex");
            if (staggerIndexValue != null)
                StaggerIndex = staggerIndexDict[staggerIndexValue];

            NextObjectID = (int?)xMap.Attribute("nextobjectid");
            BackgroundColor = new TmxColor(xMap.Attribute("backgroundcolor"));

            Tilesets = new TmxList<TmxTileset>();
            Layers = new TmxList<TmxLayer>();
            ObjectGroups = new TmxList<TmxObjectGroup>();
            ImageLayers = new TmxList<TmxImageLayer>();

            var orderIndex = 0;
            foreach (var child in xMap.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "properties":
                        Properties = new PropertyDict(child);
                        break;
                    case "tileset":
                        Tilesets.Add(new TmxTileset(this.loader, child, TmxDirectory));
                        break;
                    case "layer":
                        Layers.Add(new TmxLayer(child, Width, Height, orderIndex++));
                        break;
                    case "objectgroup":
                        ObjectGroups.Add(new TmxObjectGroup(child, orderIndex++));
                        break;
                    case "imagelayer":
                        ImageLayers.Add(new TmxImageLayer(child, orderIndex++, TmxDirectory));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public enum OrientationType
    {
        Unknown,
        Orthogonal,
        Isometric,
        Staggered,
        Hexagonal
    }

    public enum StaggerAxisType
    {
        X,
        Y
    }

    public enum StaggerIndexType
    {
        Odd,
        Even
    }

    public enum RenderOrderType
    {
        RightDown,
        RightUp,
        LeftDown,
        LeftUp
    }
}
