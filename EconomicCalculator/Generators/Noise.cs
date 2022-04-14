using EconomicCalculator.Generators;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate double NoiseMethod(Vector3 point, double frequency);

public enum NoiseMethodType
{
    Value,
    Perlin
}

public class NoiseSet
{
    public NoiseMethodType NoiseType { get; }

    public NoiseMethod BaseNoise { get; }

    public double Frequency { get; set; }

    /// <summary>
    /// best between 1 and 8
    /// </summary>
    public int Octaves { get; set; }

    /// <summary>
    /// Best between 1 and 4
    /// </summary>
    public double Lacunarity { get; set; }

    /// <summary>
    /// Best between 0 and 1.
    /// </summary>
    public double Persistence { get; set; }

    public NoiseSet(NoiseMethodType Type, int Dimension)
    {
        NoiseType = Type;
        BaseNoise = Noise.noiseMethods[(int)Type][Dimension - 1];
        Frequency = 1;
        Octaves = 1;
        Lacunarity = 2;
        Persistence = 0.5;
    }

    public double SimpleNoise(Vector3 point, double frequency)
    {
        return BaseNoise(point, frequency);
    }

    public double DeepNoise(Vector3 point)
    {
        return Noise.Sum(BaseNoise, point, Frequency, Octaves, Lacunarity, Persistence);
    }
}

public static class Noise
{
    public static double Seed { get; set; }

    public static double Lerp(double a, double b, double t)
    {
        return (b - a) * t + a;
    }

    public static NoiseMethod[] valueMethods =
    {
        Value1D,
        Value2D,
        Value3D
    };

    public static NoiseMethod[] perlinMethods =
    {
        Perlin1D,
        Perlin2D,
        Perlin3D
    };

    public static NoiseMethod[][] noiseMethods =
    {
        valueMethods,
        perlinMethods
    };

    private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };

    private const int hashMask = 255;

    private static double[] gradients1D = {
        1f, -1f
    };

    private const int gradientsMask1D = 1;

    private static Vector2[] gradients2D = {
        new Vector2( 1f, 0f),
        new Vector2(-1f, 0f),
        new Vector2( 0f, 1f),
        new Vector2( 0f,-1f),
        new Vector2( 1f, 1f).Normalized,
        new Vector2(-1f, 1f).Normalized,
        new Vector2( 1f,-1f).Normalized,
        new Vector2(-1f,-1f).Normalized
    };

    private const int gradientsMask2D = 7;

    public static double Smooth (double t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private static double Dot (Vector2 g, double x, double y)
    {
        return g.x * x + g.y * y;
    }

    private static double Dot (Vector3 g, double x, double y, double z)
    {
        return g.x * x + g.y * y + g.z * z;
    }

    private static double sqr2 = Math.Sqrt(2.0);

    private static Vector3[] gradients3D = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),

        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };

    private const int gradientsMask3D = 15;

    public static double Perlin1D(Vector3 point, double frequency)
    {
        point *= frequency;
		int i0 = (int)Math.Floor(point.x);
		double t0 = point.x - i0;
		double t1 = t0 - 1f;
		i0 &= hashMask;
		int i1 = i0 + 1;

        double g0 = gradients1D[hash[i0] & gradientsMask1D];
        double g1 = gradients1D[hash[i1] & gradientsMask1D];

        double v0 = g0 * t0;
        double v1 = g1 * t1;

		double t = Smooth(t0);
		return Lerp(v0, v1, t) * 2f;
    }

    public static double Perlin2D(Vector3 point, double frequency)
    {
        point *= frequency;
        int ix0 = (int)Math.Floor(point.x);
        int iy0 = (int)Math.Floor(point.y);
        double tx0 = point.x - ix0;
        double ty0 = point.y - iy0;
        double tx1 = tx0 - 1f;
        double ty1 = ty0 - 1f;
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        Vector2 g00 = gradients2D[hash[h0 + iy0] & gradientsMask2D];
        Vector2 g10 = gradients2D[hash[h1 + iy0] & gradientsMask2D];
        Vector2 g01 = gradients2D[hash[h0 + iy1] & gradientsMask2D];
        Vector2 g11 = gradients2D[hash[h1 + iy1] & gradientsMask2D];

        double v00 = Dot(g00, tx0, ty0);
        double v10 = Dot(g10, tx1, ty0);
        double v01 = Dot(g01, tx0, ty1);
        double v11 = Dot(g11, tx1, ty1);
        
        double tx = Smooth(tx0);
        double ty = Smooth(ty0);
        return Lerp(
            Lerp(v00, v10, tx),
            Lerp(v01, v11, tx),
            ty) * sqr2;
    }

    public static double Sum(NoiseMethod method, Vector3 point, double frequency, int octaves, double lacunarity, double persistence)
    {
        double sum = method(point, frequency);
        double amplitude = 1f;
        double range = 1f;
        for (int o = 1; o < octaves; o++)
        {
            frequency *= lacunarity;
            amplitude *= persistence;
            range += amplitude;
            sum += method(point, frequency) * amplitude;
        }
        return sum / range;
    }

    public static double Perlin3D(Vector3 point, double frequency)
    {
        point *= frequency;
        int ix0 = (int)Math.Floor(point.x);
        int iy0 = (int)Math.Floor(point.y);
        int iz0 = (int)Math.Floor(point.z);
        double tx0 = point.x - ix0;
        double ty0 = point.y - iy0;
        double tz0 = point.z - iz0;
        double tx1 = tx0 - 1f;
        double ty1 = ty0 - 1f;
        double tz1 = tz0 - 1f;
        ix0 &= hashMask;
        iy0 &= hashMask;
        iz0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;
        int iz1 = iz0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        int h00 = hash[h0 + iy0];
        int h10 = hash[h1 + iy0];
        int h01 = hash[h0 + iy1];
        int h11 = hash[h1 + iy1];
        Vector3 g000 = gradients3D[hash[h00 + iz0] & gradientsMask3D];
        Vector3 g100 = gradients3D[hash[h10 + iz0] & gradientsMask3D];
        Vector3 g010 = gradients3D[hash[h01 + iz0] & gradientsMask3D];
        Vector3 g110 = gradients3D[hash[h11 + iz0] & gradientsMask3D];
        Vector3 g001 = gradients3D[hash[h00 + iz1] & gradientsMask3D];
        Vector3 g101 = gradients3D[hash[h10 + iz1] & gradientsMask3D];
        Vector3 g011 = gradients3D[hash[h01 + iz1] & gradientsMask3D];
        Vector3 g111 = gradients3D[hash[h11 + iz1] & gradientsMask3D];

        double v000 = Dot(g000, tx0, ty0, tz0);
        double v100 = Dot(g100, tx1, ty0, tz0);
        double v010 = Dot(g010, tx0, ty1, tz0);
        double v110 = Dot(g110, tx1, ty1, tz0);
        double v001 = Dot(g001, tx0, ty0, tz1);
        double v101 = Dot(g101, tx1, ty0, tz1);
        double v011 = Dot(g011, tx0, ty1, tz1);
        double v111 = Dot(g111, tx1, ty1, tz1);

        double tx = Smooth(tx0);
        double ty = Smooth(ty0);
        double tz = Smooth(tz0);
        return Lerp(
            Lerp(Lerp(v000, v100, tx), Lerp(v010, v110, tx), ty),
            Lerp(Lerp(v001, v101, tx), Lerp(v011, v111, tx), ty),
            tz);
    }

    public static double Value1D(Vector3 point, double frequency)
    {
        point *= frequency;
        int i0 = (int)Math.Floor(point.x);
        double t = point.x - i0;
        i0 &= hashMask;
        int i1 = i0 + 1;

        int h0 = hash[i0];
        int h1 = hash[i1];

        t = Smooth(t);
        return Lerp(h0, h1, t) * (1f / hashMask);
    }

    public static double Value2D(Vector3 point, double frequency)
    {
        point *= frequency;
        int ix0 = (int)Math.Floor(point.x);
        int iy0 = (int)Math.Floor(point.y);
        double tx = point.x - ix0;
        double ty = point.y - iy0;
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        int h00 = hash[h0 + iy0];
        int h10 = hash[h1 + iy0];
        int h01 = hash[h0 + iy1];
        int h11 = hash[h1 + iy1];

        tx = Smooth(tx);
        ty = Smooth(ty);
        return Lerp(
            Lerp(h00, h10, tx),
            Lerp(h01, h11, tx),
            ty) * (1f / hashMask);
    }

    public static double Value3D(Vector3 point, double frequency)
    {
        point *= frequency;
        int ix0 = (int)Math.Floor(point.x);
        int iy0 = (int)Math.Floor(point.y);
        int iz0 = (int)Math.Floor(point.z);
        double tx = point.x - ix0;
        double ty = point.y - iy0;
        double tz = point.z - iz0;
        ix0 &= hashMask;
        iy0 &= hashMask;
        iz0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;
        int iz1 = iz0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        int h00 = hash[h0 + iy0];
        int h10 = hash[h1 + iy0];
        int h01 = hash[h0 + iy1];
        int h11 = hash[h1 + iy1];
        int h000 = hash[h00 + iz0];
        int h100 = hash[h10 + iz0];
        int h010 = hash[h01 + iz0];
        int h110 = hash[h11 + iz0];
        int h001 = hash[h00 + iz1];
        int h101 = hash[h10 + iz1];
        int h011 = hash[h01 + iz1];
        int h111 = hash[h11 + iz1];

        tx = Smooth(tx);
        ty = Smooth(ty);
        tz = Smooth(tz);
        return Lerp(
            Lerp(Lerp(h000, h100, tx), Lerp(h010, h110, tx), ty),
            Lerp(Lerp(h001, h101, tx), Lerp(h011, h111, tx), ty),
            tz) * (1f / hashMask);
    }
}
