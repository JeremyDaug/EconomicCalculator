using DotnetNoise;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Enums.EnumExtensions;
using System.Drawing.Imaging;
using System.IO;

namespace EconomicSim.DTOs.Hexmap
{
    public class Hexgrid : IEnumerable<HexColor>
    {
        #region Properties

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The width of the grid.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The bitmap backing the grid.
        /// </summary>
        public Bitmap Bitmap { get; set; }

        /// <summary>
        /// If the grid Wraps Vertically.
        /// </summary>
        public bool vWrap { get; set; }

        /// <summary>
        /// If the grid Wraps Horizontally.
        /// </summary>
        public bool hWrap { get; set; }

        #endregion Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Height">The height of the grid.</param>
        /// <param name="Width">The width of the grid.</param>
        public Hexgrid(int Height, int Width, bool vWrap = false, bool hWrap = false)
        {
            if (Height < 1)
                throw new ArgumentOutOfRangeException(nameof(Height));
            if (Width < 1)
                throw new ArgumentOutOfRangeException(nameof(Width));
            this.Height = Height;
            this.Width = Width;
            this.vWrap = vWrap;
            this.hWrap = hWrap;

            Bitmap = new Bitmap(Width, Height);
        }

        /// <summary>
        /// Get the color of a hex, if wrapping is enabled, it returns 
        /// the appropriate hex on the other side of the grid.
        /// </summary>
        /// <param name="x">The X value of the hex.</param>
        /// <param name="y">The Y value of the hex.</param>
        /// <returns>The Color of the Hex.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If x or y is outside of the grid and wrapping is
        /// not enabled for that axis.
        /// </exception>
        public Color GetHexColor(int x, int y)
        {
            if (hWrap)
            {
                x = x % Bitmap.Width;
                if (x < 0)
                    x += Bitmap.Width;
            }
            else
            {
                if (x > Bitmap.Width || x < 0)
                    throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (vWrap)
            {
                y = y % Bitmap.Height;
                if (y < 0)
                    y += Bitmap.Height;
            }
            else
            {
                if (y > Bitmap.Height || y < 0)
                    throw new ArgumentOutOfRangeException(nameof(y));
            }

            return Bitmap.GetPixel(x, y);
        }

        /// <summary>
        /// Get's the hex, automatically wrapping it regardless of the context.
        /// </summary>
        /// <param name="coord">The Coordinates.</param>
        /// <returns>The Resulting Hex Coordinate.</returns>
        public HexCoord Wrap(HexCoord coord)
        {
            // wrap x
            var x = coord.x % Bitmap.Width;
            if (x < 0)
                x += Bitmap.Width;

            // wrap y
            var y = coord.y % Bitmap.Height;
            if (y < 0)
                y += Bitmap.Height;

            return new HexCoord(x, y);
        }

        /// <summary>
        /// Get the Color of the hex, if wrapping is enabled it returns
        /// the appropriate hex on the other side of the grid.
        /// </summary>
        /// <param name="coord">The coord we want to get.</param>
        /// <returns>The Color of the Hex</returns>
        /// /// <exception cref="ArgumentOutOfRangeException">
        /// If x or y is outside of the grid and wrapping is
        /// not enabled for that axis.
        /// </exception>
        public Color GetHexColor(HexCoord coord)
        {
            return GetHexColor(coord.x, coord.y);
        }

        /// <summary>
        /// Generates a cylindrical texture for the grid.
        /// </summary>
        /// <param name="seed">The seed to for generation.</param>
        /// <param name="min">The minimum Value of the noise.</param>
        /// <param name="max">The Maximum value of the noise.</param>
        public void GeneratePerlinNoiseMapCylinder(int seed, int min, int max)
        {
            var noise = new FastNoise(seed);

            for (int x = 0; x < Bitmap.Width; ++x)
            {
                for (int y = 0; y < Bitmap.Height; ++y)
                {
                    // a cylender in 3d noise space
                    // x is a portion of a with sized rotation around the center
                    float xPos = x / Bitmap.Width;
                    // with a reverse offset for the current row.
                    xPos -= y * 1 / (Bitmap.Width * 2);
                    // get the radian angle of it.
                    var angle = xPos * 2 * (float)Math.PI;
                    // radius = 1 for general purposes.
                    float xFin = (float)Math.Sin(angle);
                    // divide y by height for consistency purposes.
                    float yFin = y / Bitmap.Height;
                    float zFin = (float)Math.Cos(angle);

                    var val = (max-min) * (noise.GetPerlin(xFin, yFin, zFin) + 1) + min;

                    Bitmap.SetPixel(x, y, Color.FromArgb((int)val));
                }
            }
        }

        /// <summary>
        /// Get the ARGB value of a hex.
        /// </summary>
        /// <param name="x">The X value of the hex.</param>
        /// <param name="y">The Y value of the hex.</param>
        /// <returns>The ARGB of the Hex.</returns>
        public int GetHexValue(int x, int y)
        {
            return GetHexColor(x, y).ToArgb();
        }

        /// <summary>
        /// Get the ARGB value of a hex.
        /// </summary>
        /// <param name="coord">The Hex to retrieve.</param>
        /// <returns>The ARGB of the Hex.</returns>
        public int GetHexValue(HexCoord coord)
        {
            return GetHexValue(coord.x, coord.y);
        }

        /// <summary>
        /// Get the uint value of a hex.
        /// </summary>
        /// <param name="x">The X value of the hex.</param>
        /// <param name="y">The Y value of the hex.</param>
        /// <returns>The uint value of the Hex.</returns>
        public uint GetHexUint(int x, int y)
        {
            return (uint)GetHexValue(x, y);
        }

        /// <summary>
        /// Get the uint value of a hex.
        /// </summary>
        /// <param name="coord">The coordinate to retrieve.</param>
        /// <returns>The uint value of the Hex.</returns>
        public uint GetHexUint(HexCoord coord)
        {
            return GetHexUint(coord.x, coord.y);
        }

        /// <summary>
        /// Processes the image, adding each hex to all of it's neighbors,
        /// capping at the maximum possible value.
        /// </summary>
        public void AdditiveSpread()
        {
            var result = new Bitmap(Bitmap);

            foreach (var pixel in this)
            {
                var coord = pixel.Coord;
                var curr = GetHexUint(coord);

                var NE = InGrid(coord.ToNE) ? GetHexUint(coord.ToNE) : 0;
                var E  = InGrid(coord.ToE)  ? GetHexUint(coord.ToE) : 0;
                var SE = InGrid(coord.ToSE) ? GetHexUint(coord.ToSE) : 0;
                var SW = InGrid(coord.ToSW) ? GetHexUint(coord.ToSW) : 0;
                var W  = InGrid(coord.ToW)  ? GetHexUint(coord.ToW) : 0;
                var NW = InGrid(coord.ToNW) ? GetHexUint(coord.ToNW) : 0;

                // add all adjacent hex values to this one.
                var argb = curr + NE + E + SE + SW + W + NW;

                // Cap at maximum possible value, if the result is somehow less
                // than the original, then we've wrapped.
                if (argb < curr)
                    argb = uint.MaxValue;

                result.SetPixel(coord.x, coord.y, Color.FromArgb((int)argb));
            }

            Bitmap = result;
        }

        /// <summary>
        /// Set's the pixel to the given arbg value
        /// </summary>
        /// <param name="x">x coord.</param>
        /// <param name="y">y coord.</param>
        /// <param name="argb">The argb value.</param>
        public void SetPixelArgb(int x, int y, int argb)
        {
            Bitmap.SetPixel(x, y, Color.FromArgb(argb));
        }

        internal void LoadFrom(string folder, string fileName)
        {
            var filepath = Path.Combine(folder, fileName + ".png");

            // check that file exists.
            if (!File.Exists(filepath))
                throw new FileNotFoundException(filepath);

            // if it does, load it.
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                Bitmap = (Bitmap)Bitmap.FromStream(fs);
                fs.Close();
            }

            Height = Bitmap.Height;
            Width = Bitmap.Width;
            // wraping is set by parent.
        }

        /// <summary>
        /// Saves the hexmap's immage at the folder and file specified.
        /// </summary>
        /// <param name="folder">The folder it's saved to.</param>
        /// <param name="file">The file name.</param>
        public void SaveAt(string folder, string file)
        {
            var filepath = Path.Combine(folder, file + ".png");

            if (File.Exists(filepath))
            { // if it already exists then we need to delete it.
                Bitmap.Save(filepath);
            }
            else
            {
                Bitmap.Save(filepath, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Set's the pixel to the given arbg value
        /// </summary>
        /// <param name="coord">The coordinate to place it at.</param>
        /// <param name="argb">The argb value.</param>
        public void SetPixelArgb(HexCoord coord, int argb)
        {
            Bitmap.SetPixel(coord.x, coord.y, Color.FromArgb(argb));
        }

        /// <summary>
        /// Checks the Location is in the grid or not.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>If in the grid or not.</returns>
        public bool InGrid(int x, int y)
        {
            if (!vWrap && (y < 0 || y >= Height))
                return false;
            if (!hWrap && (x < 0 || x >= Width))
                return false;
            return true;
        }

        /// <summary>
        /// Checks if the Location is in the grid or not.
        /// </summary>
        /// <param name="coord">Coordinates of the location.</param>
        /// <returns>If in the grid or not.</returns>
        public bool InGrid(HexCoord coord)
        {
            return InGrid(coord.x, coord.y);
        }

        /// <summary>
        /// Set the alpha of a hex.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        public void SetHexAlpha(int x, int y, int alpha)
        {
            var color = GetHexColor(x, y);

            var newColor = color.SetAlpha(alpha);

            Bitmap.SetPixel(x, y, newColor);
        }

        /// <summary>
        /// Set the alpha of a hex.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="alpha"></param>
        public void SetHexAlpha(HexCoord coord, int alpha)
        {
            SetHexAlpha(coord.x, coord.y, alpha);
        }

        /// <summary>
        /// Set the red channel of a hex.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="green"></param>
        public void SetHexRed(int x, int y, int red)
        {
            var color = GetHexColor(x, y);

            var newColor = color.SetRed(red);

            Bitmap.SetPixel(x, y, newColor);
        }

        /// <summary>
        /// Set the red channel of a hex.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="blue"></param>
        public void SetHexRed(HexCoord coord, int red)
        {
            SetHexRed(coord.x, coord.y, red);
        }

        /// <summary>
        /// Set the blue channel of a hex.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blue"></param>
        public void SetHexBlue(int x, int y, int blue)
        {
            var color = GetHexColor(x, y);

            var newColor = color.SetBlue(blue);

            Bitmap.SetPixel(x, y, newColor);
        }

        /// <summary>
        /// Set the blue channel of a hex.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="blue"></param>
        public void SetHexBlue(HexCoord coord, int blue)
        {
            SetHexBlue(coord.x, coord.y, blue);
        }

        /// <summary>
        /// Set the green channel of a hex.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="green"></param>
        public void SetHexGreen(int x, int y, int green)
        {
            var color = GetHexColor(x, y);

            var newColor = color.SetGreen(green);

            Bitmap.SetPixel(x, y, newColor);
        }

        /// <summary>
        /// Set the green channel of a hex.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="green"></param>
        public void SetHexGreen(HexCoord coord, int green)
        {
            SetHexGreen(coord.x, coord.y, green);
        }

        /// <summary>
        /// Creates a dupilcate of the Hexgrid's bitmap, but offsets
        /// it to create an image that's more accurate to the 
        /// map image.
        /// </summary>
        /// <returns></returns>
        public Bitmap MakeOffsetImage()
        {
            var result = new Bitmap(Bitmap);

            for (int y = 0; y < Bitmap.Height; ++y)
            {
                var offset = y / 2;
                for (int x = 0; x < Bitmap.Width; ++x)
                {
                    var color = Bitmap.GetPixel(x, y);
                    int newX = offset + x;
                    if (newX > Bitmap.Width)
                        newX -= Bitmap.Width;
                    result.SetPixel(x, y, color);
                }
            }

            return result;
        }

        public IEnumerator<HexColor> GetEnumerator()
        {
            for(int x = 0; x < Width; ++x)
            {
                for(int y = 0; y < Height; ++y)
                {
                    yield return new HexColor(x, y, GetHexColor(x, y));
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
