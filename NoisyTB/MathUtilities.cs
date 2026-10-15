using System.Numerics;

namespace TimBreak.NoiseTB.Utilities
{
    public static class MathUtilities
    {
        /// <summary>
        /// Maps value from (-1,1) to (0,1)
        /// </summary>
        /// <returns>value * .5f + .5f</returns>
        public static float To01(float value)
        {
            return value * .5f + .5f;
        }

        /// <summary>
        /// Linear interpolation
        /// </summary>
        public static float Lerp(float time, float v0, float v1)
        {
            return v0 * (1 - time) + v1 * time;
        }

        /// <summary>
        /// Bilinear interpolation
        /// </summary>
        public static float Blerp(Vector2 point, float v00, float v01, float v10, float v11)
        {
            float x1 = Lerp(point.X, v00, v10);
            float x2 = Lerp(point.X, v01, v11);
            return Lerp(point.Y, x1, x2);
        }

        /// <summary>
        /// Normalizes vector. If v = Vector2.Zero returns Vector2(1,0).
        /// </summary>
        /// <returns>Normalized v</returns>
        public static Vector2 SafeNormalize(Vector2 v)
        {
            if (v == Vector2.Zero) return new Vector2(1, 0);
            return Vector2.Normalize(v);
        }

        /// <summary>
        /// Needed to remove diamond‑shaped artifacts.
        /// Fade(0f) = 0f
        /// Fade(0.5f) = 0.5f
        /// Fade(1f) = 1f
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static float FClamp(float x, float min, float max)
        {
            return MathF.Max(MathF.Min(x, max), min);
        }

        public static float FClamp01(float x)
        {
            return MathF.Max(MathF.Min(x, 1), 0);
        }
    }
}