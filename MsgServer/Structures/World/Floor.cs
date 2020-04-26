// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Floor.cs
// Last Edit: 2016/11/23 10:27
// Created: 2016/11/23 10:26

using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace MsgServer.Structures.World
{
    /// <summary>
    /// This class encapsulates the coordinate tile grid for a map. It contains methods for loading the map from
    /// a flat binary file and for obtaining coordinate values directly from the class using indexers. The map 
    /// class inherits from this base class. If the file does not exist for the map, then a compressed map will
    /// be generated from TQ Digital's data map file. 
    /// </summary>
    public class Floor
    {
        // Local-Scope Variable Declarations:
        public Tile[] Coordinates { get; set; }     // Array containing access bits for coordinates on the map.
        public Size Boundaries { get; set; }        // Size of the map (width and height). 
        public bool Loaded { get; set; }            // True if the map has been loaded correctly.
        public string Path { get; private set; }    // The path to the map file.

        /// <summary>
        /// This class encapsulates the coordinate tile grid for a map. It contains methods for loading the map from
        /// a flat binary file and for obtaining coordinate values directly from the class using indexers. The map 
        /// class inherits from this base class. If the file does not exist for the map, then a compressed map will
        /// be generated from TQ Digital's data map file. 
        /// </summary>
        public Floor(string path)
        {
            Coordinates = null;
            Loaded = false;
            Path = path;
        }

        /// <summary>
        /// This method loads a compressed map from the server's flat file database. If the file does not exist, the
        /// server will make an attempt to find and convert a dmap version of the map into a compressed map file. 
        /// After converting the map, the map will be loaded for the server.
        /// </summary>
        public virtual bool Load()
        {
            try
            {
                // If the file exists, load the file. Else, convert the file.
                if (File.Exists(Environment.CurrentDirectory + Database.MAPS_LOCATION + Path))
                {
                    // Initialization File Streams:
                    var stream = new MemoryStream(File.ReadAllBytes(
                        Environment.CurrentDirectory + Database.MAPS_LOCATION + Path), false);
                    var reader = new BinaryReader(stream);

                    // Initialize the floor of the map:
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    Boundaries = new Size(width, height);

                    // Get the floor's compressed tile information:
                    Coordinates = new Tile[width * height];
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                            this[x, y] = new Tile((TileType)reader.ReadByte(), reader.ReadInt16(), reader.ReadInt16());

                    // Dispose File Streams:
                    reader.Close();
                    reader.Dispose();
                    stream.Close();
                    stream.Dispose();
                    return true;
                }
                return Convert(Environment.CurrentDirectory + Database.DMAPS_LOCATION + "map\\"
                               + Path.Replace(".cqm", ".dmap"));
            }
            catch (Exception e) { Console.WriteLine(e); }
            return false;
        }

        /// <summary>
        /// This method converts a data map from TQ Digital's Conquer Online client to a compressed map file that
        /// only holds access values. It does not hold tile elevation (short value), but that's alright. Most servers
        /// never use tile elevation anyways.
        /// </summary>
        /// <param name="path">The path of the dmap file from the client.</param>
        public virtual bool Convert(string path)
        {
            try
            {
                // Initialization File Streams:
                FileStream dmapStream = File.OpenRead(path);
                var dmapReader = new BinaryReader(dmapStream);
                dmapReader.BaseStream.Seek(0x10CL, SeekOrigin.Begin);

                // Initialize the floor of the map:
                //Program.save_log(@"syslog\CQ_Server", "Converting for first use " + path + "...");
                //Console.WriteLine("Converting " + path + "...");
                int width = dmapReader.ReadInt32();
                int height = dmapReader.ReadInt32();
                Boundaries = new Size(width, height);

                #region Floor Construction
                // Get the floor's initial tile information:
                Coordinates = new Tile[width * height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Read values from the data map file:
                        TileType access = dmapReader.ReadInt16() == 0 ? TileType.AVAILABLE : TileType.TERRAIN;
                        short surface = dmapReader.ReadInt16();
                        short elevation = dmapReader.ReadInt16();

                        // Edit the access type and save to the coordinate system:
                        if (surface == 16) access = TileType.MARKET_SPOT;
                        this[x, y] = new Tile(access, elevation);
                    }
                    dmapReader.BaseStream.Seek(4L, SeekOrigin.Current);
                }
                #endregion

                #region Portal Overriding
                // Get portals from the data map file:
                int totalPortals = dmapReader.ReadInt32();
                //Program.save_log(@"syslog\CQ_Server", string.Format("Found {0} portals.", totalPortals));
                for (int index = 0; index < totalPortals; index++)
                {
                    // Get the location of the portal:
                    int portalX = dmapReader.ReadInt32() - 1;
                    int portalY = dmapReader.ReadInt32() - 1;
                    int portalIndex = dmapReader.ReadInt32();
                    //Program.save_log(@"syslog\Portal",string.Format("Portal: {0} ({1}/{2})", portalIndex, portalX, portalY));
                    //dmapReader.BaseStream.Seek(4L, SeekOrigin.Current);

                    // Attempt to set the tiles:
                    for (int x = 0; x < 3; x++) for (int y = 0; y < 3; y++)
                            if (portalY + y < height && portalX + x < width)
                            {
                                Tile tile = this[portalX + x, portalY + y];
                                this[portalX + x, portalY + y] = new Tile(TileType.PORTAL, tile.Elevation, portalIndex);
                            }
                }
                #endregion

                #region Scene Overriding
                // Load scenery data to the map file:
                int amountOfScenery = dmapReader.ReadInt32();
                for (int index = 0; index < amountOfScenery; index++)
                {
                    // Switch based on the type of scenery being loaded:
                    var typeOfScenery = (SceneryType)dmapReader.ReadInt32();
                    switch (typeOfScenery)
                    {
                        case SceneryType.SCENERY_OBJECT:
                            {
                                // Get scene data from the DMap:
                                string fileName = Encoding.ASCII.GetString(dmapReader.ReadBytes(260));
                                fileName = Environment.CurrentDirectory + Database.DMAPS_LOCATION
                                    + fileName.Remove(fileName.IndexOf('\0')).Replace("map\\", "");
                                var location = new Point(dmapReader.ReadInt32(), dmapReader.ReadInt32());

                                // Get scene Data from the scene file:
                                var sceneStream = new MemoryStream(File.ReadAllBytes(fileName));
                                var sceneReader = new BinaryReader(sceneStream);
                                int amountOfParts = sceneReader.ReadInt32();

                                // Read from scenery parts:
                                for (int part = 0; part < amountOfParts; part++)
                                {
                                    // Initialize the size and starting point:
                                    sceneStream.Seek(0x14CL, SeekOrigin.Current);
                                    var size = new Size(sceneReader.ReadInt32(), sceneReader.ReadInt32());
                                    sceneStream.Seek(4L, SeekOrigin.Current);
                                    var startPosition = new Point(sceneReader.ReadInt32(), sceneReader.ReadInt32());
                                    sceneStream.Seek(4L, SeekOrigin.Current);

                                    // Set the tile information being used by the tile:
                                    for (int y = 0; y < size.Height; y++)
                                        for (int x = 0; x < size.Width; x++)
                                        {
                                            var point = new Point();
                                            point.X = location.X + startPosition.X - x;
                                            point.Y = location.Y + startPosition.Y - y;
                                            this[point.X, point.Y] = new Tile((sceneReader.ReadInt32() == 0
                                                ? TileType.AVAILABLE : TileType.TERRAIN), this[point.X, point.Y].Elevation);
                                            sceneStream.Seek(8L, SeekOrigin.Current);
                                        }
                                }

                                // Dispose of scene file reader:
                                sceneStream.Close();
                                sceneReader.Close();
                                sceneStream.Dispose();
                                sceneReader.Dispose();
                                break;
                            }
                        case SceneryType.DDS_COVER: dmapReader.BaseStream.Seek(0x1A0L, SeekOrigin.Current); break;
                        case SceneryType.EFFECT: dmapReader.BaseStream.Seek(0x48L, SeekOrigin.Current); break;
                        case SceneryType.SOUND: dmapReader.BaseStream.Seek(0x114L, SeekOrigin.Current); break;
                    }
                }
                #endregion

                // Save the file:
                Save(Environment.CurrentDirectory + Database.MAPS_LOCATION + Path);
                Loaded = true;

                // Dispose File Streams:
                dmapStream.Close();
                dmapReader.Close();
                dmapStream.Dispose();
                dmapReader.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return true;
            }
            catch (Exception e) { Console.WriteLine(e); }
            return false;
        }

        /// <summary>
        /// This method saves a data map from the client's map folder as a compressed map for the server. If the 
        /// file does not exist, the server will make an attempt save the current map as a compressed map. Warning:
        /// All changes made to the map prior to saving will be final. If the map is loaded, the save will fail.
        /// </summary>
        /// <param name="destination">The destination path for the file.</param>
        public virtual void Save(string destination)
        {
            // Start creating the new file:
            if (!File.Exists(destination) && !Loaded && Coordinates != null && Boundaries != null)
            {
                // Create an empty data file:
                var fileStream = new FileStream(destination, FileMode.Create);
                var writer = new BinaryWriter(fileStream);

                // Write Data:
                writer.Write(Boundaries.Width);
                writer.Write(Boundaries.Height);
                for (int y = 0; y < Boundaries.Height; y++)
                    for (int x = 0; x < Boundaries.Width; x++)
                    {
                        Tile tile = this[x, y];
                        writer.Write((byte)tile.Access);
                        writer.Write(tile.Elevation);
                        writer.Write((short)tile.Index);
                    }

                // Dispose File Stream:
                fileStream.Close();
                writer.Close();
                fileStream.Dispose();
                writer.Dispose();
            }
        }

        /// <summary>
        /// This indexer retrieves the access value from a tile in the floor's coordinate array. It returns the 
        /// tile structure to be processed by the server's movement handlers.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile being looked up.</param>
        /// <param name="y">The y-coordinate of the tile being looked up.</param>
        public virtual Tile this[int x, int y]
        {
            get
            {
                try { return Coordinates[(x * Boundaries.Width) + y]; }
                catch { return new Tile(TileType.TERRAIN, 999, -1); }
            }
            set { Coordinates[(x * Boundaries.Width) + y] = value; }
        }
    }
}
