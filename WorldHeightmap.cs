using System.Numerics;
using TimBreak.NoiseTB.Utilities;

namespace TimBreak.NoiseTB
{
    public static class WorldHeightmap
    {
        /// <summary>
        /// Generates beautiful mountains.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="ridgePower"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacunarity"></param>
        /// <param name="seed">Noise seed</param>
        /// <returns></returns>
        public static float MountainFBM(Vector2 point, float ridgePower=1f, int octaves=6, float persistance=.5f, float lacunarity=2f, int seed=42)
        {
            // i think i must
            // 1. optimize it
            // 2. understand how it works
            // (I just played with parameters, functions and "what if" question. I even want to generate plains and mountains.
            // But when I check one heightmap in 3d, I saw beautiful mountains)
            // 3. make it more adjustable and controllable

            float voronoiRaw = Noise.Voronoi(point / 2.5f, 1f, seed).f1;
            float voronoiRaw2 = voronoiRaw*voronoiRaw;
            float voronoi = MathF.Max(MathF.Pow(voronoiRaw, ridgePower), 0.05f);
            float noise = voronoiRaw * .2f;
            float totalAmplitude = .2f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < octaves; i++)
            {
                float nv = Noise.Gradient(point * frequency, seed+i) * amplitude * MathUtilities.Lerp(i / (octaves-1), voronoi*2*ridgePower, 1f);
                noise += MathUtilities.Lerp(voronoiRaw2, nv, (.5f + MathF.Abs(nv) * .2f) * .5f);
                totalAmplitude += amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }

            return noise / totalAmplitude;
        }

        /// <summary>
        /// Generates world heightmap. World has plains surrounded by moutains.
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <param name="seed">Noise seed</param>
        /// <returns>It isn't implemented yet. So it returns 0.</returns>
        static float World(Vector2 point, int seed)
        {
            // using modified voronoi noise (with beautiful name "Tectonic Voronoi")
            // i will choose plains or mountains noise.
            // and using Lerp() mix MountainFBM() (for mountains) and probably FBM() (for plains).
            // maybe i will create special noise for плоскогорий and use 2xLerp().
            // maybe i will create new class "WorldGenerator" and move
            //     MountainFBM
            //     TectonicVoronoi (not implemented yet)
            //     World
            // into it.

            return 0;
        }
    }
}