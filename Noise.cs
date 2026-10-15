using System.Numerics;
using TimBreak.NoiseTB.Utilities;

namespace TimBreak.NoiseTB
{
    public static class Noise
    {
        /// <summary>
        /// Determines which hash functions will be used by the noises.
        /// </summary>
        public static NoiseHashes hashes = new BasicNoiseHashes();

        /// <summary>
        /// Just a white noise.
        /// </summary>
        /// <param name="x">X coordinate of a sample point</param>
        /// <param name="y">Y coordinate of a sample point</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value from -1 to 1</returns>
        public static float White(int x, int y, int seed=42)
        {
            return hashes.WhiteHash(x, y, seed);
        }

        /// <summary>
        /// Noise, shimmering with smooth gradients.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value from -1 to 1</returns>
        public static float Gradient(Vector2 point, int seed=42)
        {
            Vector2 pFloor = new(MathF.Floor(point.X), MathF.Floor(point.Y));
            Vector2 t = point - pFloor;

            Vector2 v00 = hashes.GradHash0((int)pFloor.X, (int)pFloor.Y, seed);
            Vector2 v01 = hashes.GradHash0((int)pFloor.X, (int)pFloor.Y+1, seed);
            Vector2 v10 = hashes.GradHash0((int)pFloor.X+1, (int)pFloor.Y, seed);
            Vector2 v11 = hashes.GradHash0((int)pFloor.X+1, (int)pFloor.Y+1, seed);

            float d00 = Vector2.Dot(v00, t);
            float d01 = Vector2.Dot(v01, t - new Vector2(0, 1));
            float d10 = Vector2.Dot(v10, t - new Vector2(1, 0));
            float d11 = Vector2.Dot(v11, t - new Vector2(1, 1));

            Vector2 fade = new(MathUtilities.Fade(t.X), MathUtilities.Fade(t.Y));

            return MathUtilities.Blerp(fade, d00, d01, d10, d11);
        }

        /// <summary>
        /// I don’t know how to describe it.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="jitterStrength">The deflection strength of the vertices of the vector grid</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value (from -1 to 1, if jitterStrength equals 0)</returns>
        public static float JitteredGradient(Vector2 point, float jitterStrength=.2f, int seed=42)
        {
            Vector2 pFloor = new(MathF.Floor(point.X), MathF.Floor(point.Y));
            Vector2 t = point - pFloor;

            Vector2 v00 = hashes.GradHash0((int)pFloor.X, (int)pFloor.Y, seed);
            Vector2 v01 = hashes.GradHash0((int)pFloor.X, (int)pFloor.Y+1, seed);
            Vector2 v10 = hashes.GradHash0((int)pFloor.X+1, (int)pFloor.Y, seed);
            Vector2 v11 = hashes.GradHash0((int)pFloor.X+1, (int)pFloor.Y+1, seed);

            float d00 = Vector2.Dot(v00, t - hashes.GradHash1((int)pFloor.X, (int)pFloor.Y, seed) * jitterStrength);
            float d01 = Vector2.Dot(v01, t - new Vector2(0, 1) - hashes.GradHash1((int)pFloor.X, (int)pFloor.Y+1, seed) * jitterStrength);
            float d10 = Vector2.Dot(v10, t - new Vector2(1, 0) - hashes.GradHash1((int)pFloor.X+1, (int)pFloor.Y, seed) * jitterStrength);
            float d11 = Vector2.Dot(v11, t - new Vector2(1, 1) - hashes.GradHash1((int)pFloor.X+1, (int)pFloor.Y+1, seed)*jitterStrength);

            Vector2 fade = new(MathUtilities.Fade(t.X), MathUtilities.Fade(t.Y));

            return MathUtilities.Blerp(fade, d00, d01, d10, d11);
        }

        /// <summary>
        /// Gradient noise, but the input position is affected by other noise.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="warpStrength">How much can the input position change</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value from -1 to 1</returns>
        public static float WarpedGradient(Vector2 point, float warpStrength=.8f, int seed=42)
        {
            Vector2 warpedPoint = point + new Vector2(
                Gradient(point, seed+1),
                Gradient(point, seed+2)
            ) * warpStrength;
            return Gradient(warpedPoint, seed);
        }

        /// <summary>
        /// FBM noise.
        /// </summary>
        /// <param name="noiseFunction">Noise function to use</param>
        /// <param name="point">Sample point</param>
        /// <param name="octaves">Octaves count</param>
        /// <param name="persistence">By how many times will the amplitude change with each octave</param>
        /// <param name="lacunarity">By how many times will the frequency change with each octave</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value</returns>
        public static float FBM(Func<Vector2, int, float> noiseFunction,
                                Vector2 point, int octaves=6, float persistence=.5f, 
                                float lacunarity = 2f, int seed=42)
        {
            Vector2 perOctaveOffset = Vector2.One * (.9f / octaves);

            float value = 0;
            float amplitude = 1;
            float totalAmplitude = 0;
            float frequency = 1;

            for (int i = 0; i < octaves; i++)
            {
                totalAmplitude += amplitude;
                value += noiseFunction((point + perOctaveOffset * i) * frequency, seed+i) * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return value / totalAmplitude;
        }

        /// <summary>
        /// FBM gradient noise.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="octaves">Octaves count</param>
        /// <param name="persistence">By how many times will the amplitude change with each octave</param>
        /// <param name="lacunarity">By how many times will the frequency change with each octave</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value from -1 to 1</returns>
        public static float GradientFBM(Vector2 point, int octaves=6, float persistence=.5f, float lacunarity = 2f, int seed=42)
        {
            /* Vector2 perOctaveOffset = Vector2.One * (.9f / octaves);

            float value = 0;
            float amplitude = 1;
            float totalAmplitude = 0;
            float frequency = 1;

            for (int i = 0; i < octaves; i++)
            {
                totalAmplitude += amplitude;
                value += Gradient((point + perOctaveOffset * i) * frequency, seed+i) * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return value / totalAmplitude; */
            return FBM(Gradient, point, octaves, persistence, lacunarity, seed);
        }

        /// <summary>
        /// FBM jittered gradient noise.
        /// It produces beautiful images when warpStrength is crazy (about 5).
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="jitterStrength">The deflection strength of the vertices of the vector grid</param>
        /// <param name="octaves">Octaves count</param>
        /// <param name="persistence">By how many times will the amplitude decrease with each octave</param>
        /// <param name="lacunarity">By how many times will the frequency increase with each octave</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Float value (from -1 to 1, if jitterStrength equals 0)</returns>
        public static float JitteredGradientFBM(Vector2 point, float jitterStrength=.2f, int octaves=6, float persistence=.5f, float lacunarity = 2f, int seed=42)
        {
            /* float value = 0;
            float amplitude = 1;
            float totalAmplitude = 0;
            float frequency = 1;

            for (int i = 0; i < octaves; i++)
            {
                totalAmplitude += amplitude;
                value += JitteredGradient(point * frequency, jitterStrength, seed+i) * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return value / totalAmplitude; */
            return FBM((Vector2 p, int s) => JitteredGradient(p, jitterStrength, s),
                       point, octaves, persistence, lacunarity, seed);
        }

        /// <summary>
        /// Voronoi noise.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="offsetStrength">How much are the points deviated from the uniform grid</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>f1 - distance to nearest feature point; f2 - distance to second nearest feature point</returns>
        public static (float f1, float f2) Voronoi(Vector2 point, float offsetStrength=1f, int seed=42)
        {
            int x = (int)MathF.Floor(point.X);
            int y = (int)MathF.Floor(point.Y);

            float f1 = float.MaxValue;
            float f2 = float.MaxValue;

            for (int xo = -1; xo <= 1; xo++)
            {
                int cx = x + xo;
                for (int yo = -1; yo <= 1; yo++)
                {
                    int cy = y + yo;
                    Vector2 feature = new Vector2(cx, cy) + hashes.VoronoiHash(cx, cy, seed) * offsetStrength;
                    float sqrD = Vector2.DistanceSquared(point, feature);
                    if (sqrD < f1)
                    {
                        f2 = f1;
                        f1 = sqrD;
                    } else if (sqrD < f2)
                        f2 = sqrD;
                }
            }

            return (MathF.Sqrt(f1), MathF.Sqrt(f2));
        }

        /// <summary>
        /// Voronoi noise, but results are squared. Faster than Voronoi().
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="offsetStrength">How much are the points deviated from the uniform grid</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>f1 - squared distance to nearest feature point; f2 - squared distance to second nearest feature point</returns>
        public static (float f1, float f2) VoronoiSquared(Vector2 point, float offsetStrength=1f, int seed=42)
        {
            int x = (int)MathF.Floor(point.X);
            int y = (int)MathF.Floor(point.Y);

            float f1 = float.MaxValue;
            float f2 = float.MaxValue;

            for (int xo = -1; xo <= 1; xo++)
            {
                int cx = x + xo;
                for (int yo = -1; yo <= 1; yo++)
                {
                    int cy = y + yo;
                    Vector2 feature = new Vector2(cx, cy) + hashes.VoronoiHash(cx, cy, seed) * offsetStrength;
                    float sqrD = Vector2.DistanceSquared(point, feature);
                    if (sqrD < f1)
                    {
                        f2 = f1;
                        f1 = sqrD;
                    } else if (sqrD < f2)
                        f2 = sqrD;
                }
            }

            return (f1, f2);
        }

        /// <summary>
        /// Voronoi noise, but only f1 (distance to nearest feature point) returned.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="offsetStrength">How much are the points deviated from the uniform grid</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Distance to nearest feature point</returns>
        public static float VoronoiCells(Vector2 point, float offsetStrength=1f, int seed = 42)
        {
            return Voronoi(point, offsetStrength, seed).f1;
        }

        /// <summary>
        /// Voronoi noise, but f2 - f1 returned.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="offsetStrength">How much are the points deviated from the uniform grid</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>Distance to edge of cell</returns>
        public static float VoronoiEdges(Vector2 point, float offsetStrength=1f, int seed = 42)
        {
            var (f1, f2) = Voronoi(point, offsetStrength, seed);
            return f2 - f1;
        }
    }
}