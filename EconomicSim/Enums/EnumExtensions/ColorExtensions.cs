using System.Drawing;

namespace EconomicSim.Enums.EnumExtensions
{
    /// <summary>
    /// Extensions for the Color class which make using UInts easier.
    /// </summary>
    public static class ColorExtensions
    {
        public static uint GetUArgb(this Color color)
        {
            return (uint)color.ToArgb();
        }

        /// <summary>
        /// Gets the Channels of the color in Uint form.
        /// </summary>
        /// <param name="color"/>
        /// <param name="A"/>
        /// <param name="R"/>
        /// <param name="G"/>
        /// <param name="B"/>
        /// <returns>The values of the specific channels.</returns>
        public static uint GetUChannels(this Color color, bool A, bool R, bool G, bool B)
        {
            var result = color.GetUArgb();

            // if excluded, remove the channel.
            if (!A)
                result = result & 0x00FFFFFF;
            if (!R)
                result = result & 0xFF00FFFF;
            if (!G)
                result = result & 0xFFFF00FF;
            if (!B)
                result = result & 0xFFFFFF00;

            return result;
        }

        /// <summary>
        /// Gets the Channels of the color in Uint form.
        /// </summary>
        /// <param name="color"/>
        /// <param name="A"/>
        /// <param name="R"/>
        /// <param name="G"/>
        /// <param name="B"/>
        /// <returns>The values of the specific channels.</returns>
        public static int GetChannels(this Color color, bool A, bool R, bool G, bool B)
        {
            var result = color.ToArgb();

            // if excluded, remove the channel.
            if (!A)
                result = result & 0x00FFFFFF;
            if (!R)
                result = (int)(result & 0xFF00FFFF);
            if (!G)
                result = (int)(result & 0xFFFF00FF);
            if (!B)
                result = (int)(result & 0xFFFFFF00);

            return result;
        }

        /// <summary>
        /// Bit Offset for Alpha channel.
        /// </summary>
        public static int ChannelOffsetA(this Color color) => 24;
        /// <summary>
        /// Bit Offset for Red channel.
        /// </summary>
        public static int ChannelOffsetR(this Color color) => 16;
        /// <summary>
        /// Bit Offset for Green channel.
        /// </summary>
        public static int ChannelOffsetG(this Color color) => 8;
        /// <summary>
        /// Bit Offset for Blue channel.
        /// </summary>
        public static int ChannelOffsetB(this Color color) => 0;

        /// <summary>
        /// Takes a value and offsets to to the appropriate channel.
        /// </summary>
        /// <param name="color"/>
        /// <param name="channel">The channel to offset to.</param>
        /// <param name="value">The value to offset, any excess will be lost.</param>
        /// <returns>The ofset Value</returns>
        public static uint OffsetToChannel(this Color color, char channel, uint value)
        {
            switch(channel)
            {
                case 'a':
                case 'A':
                    return value << color.ChannelOffsetA();
                case 'R':
                case 'r':
                    return value << color.ChannelOffsetR();
                case 'g':
                case 'G':
                    return value << color.ChannelOffsetG();
                case 'b':
                case 'B':
                    return value << color.ChannelOffsetB();
                default:
                    throw new ArgumentException("Channel must be A, R, G, or B");
            }
        }

        public static Color FromUArgb(this Color color, uint argb)
        {
            return Color.FromArgb((int)argb);
        }

        /// <summary>
        /// Moves one collor channel to another.
        /// If not the target, Alpha channel will be 255 (full opacity)
        /// If not the target, color channel will be 0.
        /// </summary>
        /// <param name="color">The color startpoint.</param>
        /// <param name="original">The Original Channel.</param>
        /// <param name="target">The final Channel.</param>
        /// <returns>The new Color.</returns>
        public static Color ShiftChannelToChannel(this Color color, char original, char target)
        {
            Color result;
            int start;
            switch (original)
            {
                case 'a':
                case 'A':
                    start = color.A;
                    break;
                case 'r':
                case 'R':
                    start = color.R;
                    break;
                case 'g':
                case 'G':
                    start = color.G;
                    break;
                case 'b':
                case 'B':
                    start = color.B;
                    break;
                default:
                    throw new ArgumentException("Original must be A, R, G, or B");
            }
            switch (target)
            {
                case 'a':
                case 'A':
                    result = Color.FromArgb(start, 0, 0, 0);
                    break;
                case 'r':
                case 'R':
                    result = Color.FromArgb(255, start, 0, 0);
                    break;
                case 'g':
                case 'G':
                    result = Color.FromArgb(255, 0, start, 0);
                    break;
                case 'b':
                case 'B':
                    result = Color.FromArgb(255, 0, 0, start);
                    break;
                default:
                    throw new ArgumentException("Target must be A, R, G, or B");
            }
            return result;
        }

        /// <summary>
        /// Gets the channel by character.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="channel">The channel we want.</param>
        /// <returns></returns>
        public static int GetChannel(this Color color, char channel)
        {
            switch (channel)
            {
                case 'a':
                case 'A':
                    return color.A;
                case 'r':
                case 'R':
                    return color.R;
                case 'g':
                case 'G':
                    return color.G;
                case 'b':
                case 'B':
                    return color.B;
                default:
                    throw new ArgumentException("Target must be A, R, G, or B");
            }
        }

        /// <summary>
        /// Rearranges the channels of this color to another.
        /// Can only apply one other channel to another, but you can pull from the
        /// same channel to different channels. You may also choose to skip a channel.
        /// If you skip alpha it's set to 255, all others default to 0.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="toAlpha">The channel to put to alpha.</param>
        /// <param name="toRed">The channel to put to red.</param>
        /// <param name="toGreen">The channel to put to green.</param>
        /// <param name="toBlue">The channel to put to blue.</param>
        /// <returns>The color with rearranged Channels.</returns>
        public static Color RearrangeChannels(this Color color, char toAlpha = (char)0, 
            char toRed = (char)0, char toGreen = (char)0, char toBlue = (char)0)
        {
            var a = toAlpha != (char)0 ? color.GetChannel(toAlpha) : 255;
            var r = toRed != (char)0 ? color.GetChannel(toRed) : 0;
            var g = toGreen != (char)0 ? color.GetChannel(toGreen) : 0;
            var b = toBlue != (char)0 ? color.GetChannel(toBlue) : 0;

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Creates a color based on this but with a new alpha.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color SetAlpha(this Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /// <summary>
        /// Creates a color based on this but with a new red.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="red"></param>
        /// <returns></returns>
        public static Color SetRed(this Color color, int red)
        {
            return Color.FromArgb(color.A, red, color.G, color.B);
        }

        /// <summary>
        /// Creates a color based on this but with a new green.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="green"></param>
        /// <returns></returns>
        public static Color SetGreen(this Color color, int green)
        {
            return Color.FromArgb(color.A, color.R, green, color.B);
        }

        /// <summary>
        /// Creates a color based on this but with a new blue.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static Color SetBlue(this Color color, int blue)
        {
            return Color.FromArgb(color.A, color.R, color.G, blue);
        }
    }
}
