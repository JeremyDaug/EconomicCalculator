using System.Drawing;

namespace EconomicSim.DTOs.Hexmap
{
    /// <summary>
    /// A class for containing both Hex location and the color contained.
    /// </summary>
    public class HexColor
    {
        private Color _color;

        public HexCoord Coord { get; }

        public Color Color => _color;

        public HexColor(int x, int y, Color color)
        {
            Coord = new HexCoord(x, y);
            _color = Color;
        }

        public void SetColor(Color newColor)
        {
            _color = newColor;
        }
    }
}
