using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ROAMapper
{
    public class Point
    {
        public float X;
        public float Y;
    }

    public class MapTile
    {
        // Map tile to be drawn on map (Legacy from old Map project)

        // Multiplayer for WorldCoords to space out and draw better
        const int WORLD_SCALE = 8;

        public int drawWidth;
        public int drawHeight;

        public string DisplayName;
        public string Tier;
        public string mapID;
        public MapType MapType;
        public bool Drawable;

        public Dictionary<string, string> Exits;

        public Point worldPosition;

        public MapTile(string mapID, int drawWidth = 60, int drawHeight = 60)
        {
            // If an exit TargetID was used, grab the mapID portion
            if (mapID.Contains('@'))
            {
                mapID = mapID.Split('@')[1];
            }

            this.mapID = mapID;
            this.drawWidth = drawWidth;
            this.drawHeight = drawHeight;



            this.DisplayName = WorldMap.GetMapDisplayName(mapID);
            QuickType.Cluster c = WorldMap.GetClusterByID(mapID);

            Exits = WorldMap.GetMapExits(mapID);
            worldPosition = new Point();

            // Tier number is in file name
            this.Tier = c.File.Split('_')[3];

            Drawable = false;

            if (c.Type.Contains("DUNGEON"))
                MapType = MapType.Dungeon;
            else if (c.Type.Contains("OPENPVP_BLACK"))
                MapType = MapType.BlackZone;
            else if (c.Type.Contains("OPENPVP_RED"))
                MapType = MapType.RedZone;
            else if (c.Type.Contains("OPENPVP_YELLOW"))
                MapType = MapType.YellowZone;
            else if (c.Type == "SAFEAREA")
                MapType = MapType.BlueZone;
            else if (c.Type.Contains("PASSAGE"))
                MapType = MapType.Tunnel;
            else if (c.Type.Contains("PLAYERCITY"))
                MapType = MapType.City;
            else
                MapType = MapType.Unknown;


            if (HasWorldPosition(MapType))
            {
                try
                {
                    worldPosition.X = float.Parse(c.Worldmapposition.Split(' ')[0]) * WORLD_SCALE;
                    // * -1 because it's inverted from canvas expected coords
                    worldPosition.Y = float.Parse(c.Worldmapposition.Split(' ')[1]) * WORLD_SCALE * -1;

                    if (worldPosition.X != 0 && worldPosition.Y != 0)
                    {
                        Drawable = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(c.Displayname);
                    Console.WriteLine(c.Worldmapposition);
                    Console.WriteLine(e);
                }

                // Console.WriteLine(worldPosition);
            }
        }

        bool HasWorldPosition(MapType type)
        {
            switch (type)
            {
                case MapType.BlackZone:
                    return true;
                case MapType.RedZone:
                    return true;
                case MapType.YellowZone:
                    return true;
                case MapType.BlueZone:
                    return true;
                case MapType.City:
                    return true;
                default:
                    return false;
            }
        }

        // Get a cardinal corner worldPosition
        public Point GetCorner(string direction)
        {
            Point pos = new Point();

            switch (direction)
            {
                case "NW":
                    pos.X = worldPosition.X;
                    pos.Y = worldPosition.Y;
                    break;
                case "NE":
                    pos.X = worldPosition.X + drawWidth;
                    pos.Y = worldPosition.Y;
                    break;
                case "SW":
                    pos.X = worldPosition.X;
                    pos.Y = worldPosition.Y + drawHeight;
                    break;
                case "SE":
                    pos.X = worldPosition.X + drawWidth;
                    pos.Y = worldPosition.Y + drawHeight;
                    break;
            }

            return pos;
        }

    }

    public enum MapType
    {
        Dungeon,
        BlackZone,
        RedZone,
        YellowZone,
        BlueZone,
        Unknown,
        Tunnel,
        City
    }

}

    