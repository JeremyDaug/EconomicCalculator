using DotnetNoise;
using EconomicCalculator.Enums;
using EconomicCalculator.Storage.Hexmap;
using EconomicCalculator.Enums.EnumExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Planet
{
    /// <summary>
    /// Organizer for planetary bodies ranging from a small as asteroids
    /// up to jovians or even stars.
    /// </summary>
    public class Planet
    {
        public Planet()
        {
            Terrain = new Hexgrid(100, 200);
            Terrain.hWrap = true;
            TerrainEx = new Hexgrid(100, 200);
            TerrainEx.hWrap = true;
            Climate = new Hexgrid(100, 200);
            Climate.hWrap = true;
            Regions = new List<Hexgrid>();
            Height = 100;
            Width = 200;
        }

        public Planet(int height, int seed)
        {
            Terrain = new Hexgrid(height, height*2);
            Terrain.hWrap = true;
            TerrainEx = new Hexgrid(height, height * 2);
            TerrainEx.hWrap = true;
            Climate = new Hexgrid(height, height * 2);
            Climate.hWrap = true;
            Regions = new List<Hexgrid>();
            Height = height;
            Width = height * 2;
            Seed = seed;
        }

        /// <summary>
        /// The Id of the Planet
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The planet Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The generation seed for the original planet surface.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// The planet type, lots of varieties here.
        /// </summary>
        public PlanetType Type { get; set; }

        // TODO Alterantive Chemistry (ie water vs hydrocarbon)

        /// <summary>
        /// The Shape of the planet.
        /// TODO, fix this and create alternatives for different planets with different shapes.
        /// </summary>
        public PlanetTopography Shape => PlanetTopography.Sphere;

        /// <summary>
        /// Marks a planet as being effectively fixed and
        /// dead. Actions will not occur barring events.
        /// This will save cycles.
        /// </summary>
        public bool Dead { get; set; }

        /// <summary>
        /// The Available Planet resources that can be found and
        /// tapped. When a territory creates a pile. When generated, untapped resources
        /// should add up to the mass of the planet (allow an error of 0.0000001% for sanity)
        /// Note, The mass of the planet is a double, while this is a list of decimals.
        /// Doubles have a lot more room, but any object above 10^32 is a black hole and
        /// has no breakdown.
        /// </summary>
        //public ICollection<PlanetResources> Untapped { get; set; }

        /// <summary>
        /// The mass of the planet in kg. Only Updated when needed, IE, when mass leaves the
        /// planet.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// A catch to ensure no mass removed from the planet is lost. Removed mass is
        /// first moved to here, then, if this will change the Actual mass, then it is removed
        /// from that mass.
        /// </summary>
        public double LossSafe { get; set; }

        /// <summary>
        /// The area of the planet's surface in whole km^2.
        /// </summary>
        public decimal SurfaceArea { get; set; }

        /// <summary>
        /// The Height that the sea level is at.
        /// </summary>
        public int SeaLevel { get; set; }

        /// <summary>
        /// The average air pressure of the planet. Mostly used
        /// for habitability.
        /// </summary>
        public decimal AirPressure { get; set; }

        /// <summary>
        /// The average tempurature of the planet.
        /// </summary>
        public decimal Tempurature { get; set; }

        /// <summary>
        /// The number of rows in the territory grid.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The number of colums in the grid.
        /// </summary>
        public int Width { get; set; }

        // TODO, Split this into more maps possibly.

        /// <summary>
        /// The terrain hexgrid of the planet.
        /// A contains Sea Coverage (0 to 250 km^2).
        /// R is river/coast positioning info, (2 bits of free space remain)
        /// The GB values of terrain are the height.
        /// See water flags for more information.
        /// </summary>
        public Hexgrid Terrain { get; set; }

        #region TerrainHelpers

        public int GetRiverCoastPlacement(HexCoord coord)
        {
            return Terrain.GetHexColor(coord).R;
        }

        /// <summary>
        /// Get the water coverage of a territory.
        /// </summary>
        /// <param name="coord"/>
        /// <returns>The coverage in sq. km.</returns>
        public int GetTerrainWaterCoverage(HexCoord coord)
        {
            return Terrain.GetHexColor(coord).A;
        }

        /// <summary>
        /// Whether a territory is land or not.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns>True if not totally covered in water.</returns>
        public bool IsLand(HexCoord coord)
        {
            // any territory with 250 coverage is sea, otherwise it's considered
            // land.
            return GetTerrainWaterCoverage(coord) != 250;
        }

        /// <summary>
        /// Whether a territory is sea or not.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns>True if no land is visible.</returns>
        public bool IsSea (HexCoord coord)
        {
            // Any territory that is totally water is sea.
            return GetTerrainWaterCoverage(coord) == 250;
        }

        /// <summary>
        /// Retrieves the height of the terrain.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns>The height of the terrain.</returns>
        public uint GetTerrainHeight(HexCoord coord)
        {
            // get just G and B.
            return Terrain.GetHexColor(coord).GetUChannels(false, false, true, true);
        }

        #endregion TerrainHelpers

        /// <summary>
        /// Terrain details expanded for new details.
        /// A and R are the additional depth of water
        /// in the tile. This is primarily used for sea
        /// tiles that are detached from normal sea tiles.
        /// 1 point is 1 meter.
        /// G is the fertility of the land with 0 being
        /// uncapable of sustaining life and 255 meaning
        /// rainforest lush.
        /// B is both roughness and exploration.
        /// bottom four bits (0-15) is roughness
        /// top 4 bits (0-15) is subsurface exploration.
        /// </summary>
        public Hexgrid TerrainEx { get; set; }

        /// <summary>
        /// The map of sunlight and heat data.
        /// A
        /// R is the albedo based on R/250.
        /// G and B is the Absorbed Wattage of the territory.
        /// </summary>
        public Hexgrid SolarMap { get; set; }

        /// <summary>
        /// The Climate Hexgrid of the planet.
        /// A is unused
        /// R is the rainfall of the area.
        /// G is humitity in absolute terms.
        /// B is the Temperature difference from planetary average.
        /// </summary>
        public Hexgrid Climate { get; set; }

        /// <summary>
        /// Additional climate details of the map.
        /// A and R is unused.
        /// G is the prevailing air currents of the territory.
        /// B is the ocean currents of the territory.
        /// </summary>
        public Hexgrid ClimateEx { get; set; }

        /// <summary>
        /// The Bitmaps for regions in the grid.
        /// </summary>
        public IList<Hexgrid> Regions { get; set; }

        /// <summary>
        /// The name of the regions
        /// </summary>
        public IDictionary<int, string> RegionNames { get; set; }

        //public ICollection<Territory> Territories { get; set; }

        /// <summary>
        /// Generates terrain for the planet, just the terrain and oceans 
        /// and some seas.
        /// </summary>
        public void GenerateSimpleTerrain()
        {
            // setup noise
            var noise = new FastNoise(Seed);

            var scale = 1;

            // To simplify terrain generation select sea level height first.
            // use the origin of the noise to get a random value between -1 and 1.
            // We then modify that value to be between 0.5 and 1.5 so we can get a narrower range of 
            // sea levels for now. We can alter it later for more or less variety.
            var SeaLevelRange = noise.GetPerlin(0, 0, 0) * 0.5 + 1;
            // Sea level is between 0 and 65,536,
            // should be between 0.5 and 1.5 of 32_768
            SeaLevel = (int)(32_768 * SeaLevelRange);

            // set the level of the terrain and sea level if terrain is lower than sea level.
            foreach (var pixel in Terrain)
            {
                // sample the cylinder
                var val = CylinderSample(pixel.Coord.x, pixel.Coord.y, noise) + 1;
                // limit to between 0 and 2^16-1
                val = 65_536 * val / 2;

                // if the terrain value is less than sea level, then set it to
                // fully covored.
                if (val < SeaLevel)
                {
                    // Alpha channel is sea coverage.
                    val += 250 << pixel.Color.ChannelOffsetA();
                }

                // base terroin set, coasts need to be added/calculated.
                Terrain.SetPixelArgb(pixel.Coord, (int)val);

                // lastly, get roughness for the terrain. We'll cap it between 
                // 0 and 10, For the random value, select a point of noise 10x
                // further from the current cylinder
                var difficulty = (int)(5 * (1 + CylinderSample(pixel.Coord.x, pixel.Coord.y, noise)));

                TerrainEx.SetPixelArgb(pixel.Coord, difficulty);

                // We can add in basic albedo to current surfaces.
                if (IsLand(pixel.Coord))
                {
                    // 50 == 0.2 Albedo.
                    SolarMap.SetPixelArgb(pixel.Coord, 50 << pixel.Color.ChannelOffsetR());
                } // sea has an albedo of 0 and so does not need to be set.
            }

            // Check and add coasts to land next to sea.
            foreach (var pixel in Terrain)
            {
                var coord = pixel.Coord;
                // if water covarage is 0 then it's land.
                bool isLand = IsLand(coord);

                // if it's not land then skip it, 
                // coasts can only be added to
                // land.
                if (!isLand)
                    continue;

                // set basic value
                var val = 0;

                // check each neighbor to see if they are a sea, if they are
                // add that coast to the value.
                if (Terrain.InGrid(coord.ToNE) && IsSea(coord.ToNE))
                    val += (int)WaterFlags.NE;
                if (Terrain.InGrid(coord.ToE) && IsSea(coord.ToE))
                    val += (int)WaterFlags.E;
                if (Terrain.InGrid(coord.ToSE) && IsSea(coord.ToSE))
                    val += (int)WaterFlags.SE;
                if (Terrain.InGrid(coord.ToSW) && IsSea(coord.ToSW))
                    val += (int)WaterFlags.SW;
                if (Terrain.InGrid(coord.ToW) && IsSea(coord.ToW))
                    val += (int)WaterFlags.W;
                if (Terrain.InGrid(coord.ToNW) && IsSea(coord.ToNW))
                    val += (int)WaterFlags.NW;

                // with the value, add coastal data.
                var update = pixel.Color.ToArgb() + (val << pixel.Color.ChannelOffsetR());
                // also add in some coverage for coastal tiles, assume 20km^2 for ease right now.
                // TODO calculate/Generate partial coverage for coastal territories.
                update += 20 << pixel.Color.ChannelOffsetA();
                // set pixel color.
                Terrain.SetPixelArgb(coord, update);
            }
        }

        private int Direction(byte direction)
        {
            return direction & 0x0F;
        }

        private int Speed(byte val)
        {
            return val & 0xF0 >> 4;
        }

        private int GetDirection(string dir, bool clockwise)
        {
            int result;
            switch (dir)
            {
                case "NE":
                    result = 0;
                    break;
                case "E":
                    result = 2;
                    break;
                case "SE":
                    result = 4;
                    break;
                case "SW":
                    result = 6;
                    break;
                case "W":
                    result = 8;
                    break;
                case "NW":
                    result = 10;
                    break;
                default:
                    throw new ArgumentException(nameof(dir));
            }
            if(clockwise)
            {
                result += 1;
            }
            return result;
        }

        private int ToSpeed(int val)
        {
            return val << 4;
        }

        /// <summary>
        /// Takes the existing map of the planet and creates a climate
        /// map for the planet.
        /// </summary>
        public void GenerateClimate()
        {
            // Watt / m^2 average
            // TODO make dynamic based on star(s) and distance of planet.
            var fullSun = 1000;
            // TODO, parameterize this to make it available.
            var airZones = 6;

            // Use the Planet seed, but offset by 1 to get a different map.
            var noise = new FastNoise(Seed + 1);
            var secNoise = new FastNoise(Seed + 2);

            // setup SolarMap for heat application. TODO, add back in later.
            #region SolarMapAndPrevailingWind
            /*
            for (int row = 0; row < Height; ++row)
            {
                // The amount that hits is relative to the height with equator
                // getting full and poles getting almost none.
                var correctedSun = (int)(Math.Sin(row / Height * Math.PI) * fullSun);
                // just in case, ensure only G and B is effected.
                correctedSun = correctedSun & 0x0000FFFF;

                // we can also generate prevailing winds this way.
                // we default to 6 cells the 2 at the center point equator wise and NE to SW
                // TODO parameterize this.
                var zone = (int)(row / Height * airZones);
                int prevailingWind = 0;

                // 0 NW, 2 W, 4 SW, 6 SE, 8 E, 10 NE
                if (zone < airZones / 2)
                { // north hemisphere.
                    if (zone % 2 == 1) // if odd
                    {// 2 hex/day SW
                        prevailingWind = GetDirection("SW", false) + ToSpeed(2);
                    }
                    else
                    {// 2 hex/day NE
                        prevailingWind = GetDirection("NE", false) + ToSpeed(2);
                    }
                }
                else // if above half
                {// Southern Hemisphere
                    if (zone % 2 == 1)
                    {// 2 hex/day NW
                        prevailingWind = GetDirection("NW", false) + ToSpeed(2);
                    }
                    else
                    {// 2 hex/day SE
                        prevailingWind = GetDirection("SE", false) + ToSpeed(2);
                    }
                }

                for (int col = 0; col < Width; ++col)
                {
                    // add existing albedo to corrected sun, then set.
                    var val = SolarMap.GetHexValue(col, row) + correctedSun;
                    SolarMap.SetPixelArgb(col, row, val);
                    // also add prevailing winds to the green channel.

                    ClimateEx.SetHexGreen(col, row, prevailingWind);
                }
            }*/
            #endregion SolarMapAndPrevailingWind

            int scale = 1;
            // TODO create real calculation for environment
            // For a shortcut, just make a selection of high and low tempurature selections.
            foreach (var pixel in Terrain)
            {
                // select random value between 0 and 255 for the tempurature 
                // restricted further to 128 +/- 64
                var tempurature = (int)(255 * (1 + CylinderSample(pixel.Coord.x, pixel.Coord.y, noise) / 4));
                Climate.SetHexBlue(pixel.Coord, tempurature);

                // set humidity between 0 and 255.
                var hum = (int)(255 * (1 + CylinderSample(pixel.Coord.x, pixel.Coord.y, secNoise)) / 2);
                Climate.SetHexGreen(pixel.Coord, hum);

                // set rainfall equal to humidity for now
                Climate.SetHexRed(pixel.Coord, hum);
            }
        }

        /// <summary>
        /// A wrapper for sampling a value from a perlin noise generator.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public float CylinderSample(int x, int y, FastNoise noise)
        {
            var scale = 1;
            // a cylender in 3d noise space
            // x is a portion of a with sized rotation around the center
            float xPos = x / Width;
            // with a reverse offset for the current row.
            xPos -= y / (Width * 2);
            // get the radian angle of it.
            var angle = xPos * 2 * (float)Math.PI;
            // radius = 1 for general purposes.
            float xFin = (float)Math.Sin(angle) * scale;
            // divide y by height for consistency purposes.
            float yFin = y / Height * scale;
            float zFin = (float)Math.Cos(angle) * scale;

            // select random value between 0 and 2^16-1 for the height.
            return noise.GetPerlin(xFin, yFin, zFin);
        }
    }
}
