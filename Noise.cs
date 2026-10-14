using System.Numerics;

namespace TimBreak.NoiseTB
{
    public static class Noise
    {
        static Vector2 GradHash0(int x, int y, int seed)
        {
            long h = (x * 2412564L + y * 7865245L + seed * 4262455264235) & 0xffffffff;
            h >>>= 16;
            h *= 3918265249;
            h ^= h >> 13;
            h *= 9238139864;
            h ^= h << 15;

            float ox = (h & 0xffff) / 32767.5f - 1f;
            float oy = ((h >> 16) & 0xffff) / 32767.5f - 1f;

            return new Vector2(ox, oy);
        }

        static Vector2 GradHash1(int x, int y, int seed)
        {
            long h = (x * 3452356L + y * 6541835L + seed * 18711325254) & 0xffffffff;
            h >>>= 14;
            h *= 3235547654;
            h ^= h >> 17;
            h *= 2414561456;
            h ^= h << 11;

            float ox = (h & 0xffff) / 32767.5f - 1f;
            float oy = ((h >> 16) & 0xffff) / 32767.5f - 1f;

            return new Vector2(ox, oy);
        }

        static Vector2 VoronoiHash(int x, int y, int seed)
        {
            long h = (x * 34501758L + y * 13487568L + seed * 18723457214345) & 0xffffffff;
            h >>>= 17;
            h *= 9823568165;
            h ^= h >> 11;
            h *= 2837465498;
            h ^= h << 19;

            float ox = (h & 0xffff) / 65535f;
            float oy = ((h >> 16) & 0xffff) / 65535f;

            return new Vector2(ox, oy);
        }

        static float Lerp(float time, float v0, float v1)
        {
            return v0 * (1 - time) + v1 * time;
        }

        static float Serp(Vector2 point, float v00, float v01, float v10, float v11)
        {
            float x1 = Lerp(point.X, v00, v10);
            float x2 = Lerp(point.X, v01, v11);
            return Lerp(point.Y, x1, x2);
        }

        static Vector2 SafeNormalize(Vector2 v)
        {
            if (v == Vector2.Zero) return new Vector2(1, 0);
            return Vector2.Normalize(v);
        }

        static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static float White(int x, int y, int seed=42)
        {
            long h = (x * 9273655L + y * 2348575L + seed * 87513246452) & 0xffffffff;
            h >>>= 11;
            h *= 3184576864;
            h ^= h >> 15;
            h *= 2317451876;
            h ^= h << 12;

            return (h & 0xffff) / 32767.5f - 1f;
        }

        public static float Gradient(Vector2 point, int seed=42)
        {
            Vector2 pFloor = new(MathF.Floor(point.X), MathF.Floor(point.Y));
            Vector2 t = point - pFloor;

            Vector2 v00 = GradHash0((int)pFloor.X, (int)pFloor.Y, seed);
            Vector2 v01 = GradHash0((int)pFloor.X, (int)pFloor.Y+1, seed);
            Vector2 v10 = GradHash0((int)pFloor.X+1, (int)pFloor.Y, seed);
            Vector2 v11 = GradHash0((int)pFloor.X+1, (int)pFloor.Y+1, seed);

            float d00 = Vector2.Dot(v00, t);
            float d01 = Vector2.Dot(v01, t - new Vector2(0, 1));
            float d10 = Vector2.Dot(v10, t - new Vector2(1, 0));
            float d11 = Vector2.Dot(v11, t - new Vector2(1, 1));

            Vector2 fade = new(Fade(t.X), Fade(t.Y));

            return Serp(fade, d00, d01, d10, d11);
        }

        public static float JitteredGradient(Vector2 point, float jitterStrength=.2f, int seed=42)
        {
            Vector2 pFloor = new(MathF.Floor(point.X), MathF.Floor(point.Y));
            Vector2 t = point - pFloor;

            Vector2 v00 = GradHash0((int)pFloor.X, (int)pFloor.Y, seed);
            Vector2 v01 = GradHash0((int)pFloor.X, (int)pFloor.Y+1, seed);
            Vector2 v10 = GradHash0((int)pFloor.X+1, (int)pFloor.Y, seed);
            Vector2 v11 = GradHash0((int)pFloor.X+1, (int)pFloor.Y+1, seed);

            float d00 = Vector2.Dot(v00, t - GradHash1((int)pFloor.X, (int)pFloor.Y, seed) * jitterStrength);
            float d01 = Vector2.Dot(v01, t - new Vector2(0, 1) - GradHash1((int)pFloor.X, (int)pFloor.Y+1, seed) * jitterStrength);
            float d10 = Vector2.Dot(v10, t - new Vector2(1, 0) - GradHash1((int)pFloor.X+1, (int)pFloor.Y, seed) * jitterStrength);
            float d11 = Vector2.Dot(v11, t - new Vector2(1, 1) - GradHash1((int)pFloor.X+1, (int)pFloor.Y+1, seed)*jitterStrength);

            Vector2 fade = new(Fade(t.X), Fade(t.Y));

            return Serp(fade, d00, d01, d10, d11);
        }

        public static float WarpedGradient(Vector2 point, float warpStrength=.8f, int seed=42)
        {
            Vector2 warpedPoint = point + new Vector2(
                Gradient(point, seed+1),
                Gradient(point, seed+2)
            ) * warpStrength;
            return Gradient(warpedPoint, seed);
        }

        public static float FBM(Vector2 point, int octaves=6, float persistence=.5f, float lacunarity = 2f, int seed=42)
        {
            Vector2 perOctaveOffset = Vector2.One * (.9f / octaves);

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

            return value / totalAmplitude;
        }

        public static float JitteredFBM(Vector2 point, float warpStrength=.2f, int octaves=6, float persistence=.5f, float lacunarity = 2f, int seed=42)
        {
            float value = 0;
            float amplitude = 1;
            float totalAmplitude = 0;
            float frequency = 1;

            for (int i = 0; i < octaves; i++)
            {
                totalAmplitude += amplitude;
                value += JitteredGradient(point * frequency, warpStrength, seed+i) * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return value / totalAmplitude;
        }
    
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
                    Vector2 feature = new Vector2(cx, cy) + VoronoiHash(cx, cy, seed) * offsetStrength;
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

        public static float VoronoiCells(Vector2 point, float offsetStrength=1f, int seed = 42)
        {
            return Voronoi(point, offsetStrength, seed).f1;
        }

        public static float VoronoiEdges(Vector2 point, float offsetStrength=1f, int seed = 42)
        {
            var (f1, f2) = Voronoi(point, offsetStrength, seed);
            return f2 - f1;
        }
    }
}