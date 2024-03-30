using Microsoft.Xna.Framework;
using System;

namespace InfraSonic;

internal static class Extensions
{
    public static Vector2 RoundEven(this Vector2 vector) => new((int)MathF.Floor(vector.X) / 2 * 2, (int)MathF.Floor(vector.Y) / 2 * 2);
}
