using System.Numerics;

namespace TimBreak.NoiseTB
{
    public abstract class NoiseHashes
    {
        public abstract Vector2 GradHash0(int x, int y, int seed);
        public abstract Vector2 GradHash1(int x, int y, int seed);
        public abstract Vector2 VoronoiHash(int x, int y, int seed);
        public abstract float WhiteHash(int x, int y, int seed);
    }

    public class BasicNoiseHashes : NoiseHashes
    {
        public override Vector2 GradHash0(int x, int y, int seed)
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

        public override Vector2 GradHash1(int x, int y, int seed)
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

        public override Vector2 VoronoiHash(int x, int y, int seed)
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

        public override float WhiteHash(int x, int y, int seed=42)
        {
            long h = (x * 9273655L + y * 2348575L + seed * 87513246452) & 0xffffffff;
            h >>>= 11;
            h *= 3184576864;
            h ^= h >> 15;
            h *= 2317451876;
            h ^= h << 12;

            return (h & 0xffff) / 32767.5f - 1f;
        }
    }
}