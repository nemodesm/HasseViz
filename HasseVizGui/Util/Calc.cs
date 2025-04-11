using System;
using Microsoft.Xna.Framework;

namespace Mapper.Util;

public static class Calc
{
    #region Random

    #region Choose

    /// <summary>
    /// Chooses a random element from <paramref name="choices"/>
    /// </summary>
    /// <param name="random">The random number generator to use</param>
    /// <param name="choices">The choices</param>
    /// <typeparam name="T">The type of the choices</typeparam>
    /// <returns><see cref="T"/> A random element from <paramref name="choices"/></returns>
    public static T Choose<T>(this Random random, params T[] choices)
    {
        return choices[random.Next(choices.Length)];
    }

    #endregion

    #endregion

    #region Math

    /// <summary>
    /// Returns an integer that indicates the sign of a single-precision floating-point number with a 'deadzone' of
    /// <paramref name="threshold"/>.
    /// </summary>
    /// <param name="value">A signed number</param>
    /// <param name="threshold">The 'deadzone'</param>
    /// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.
    /// <list type="table">
    ///     <listheader>
    ///         <term> Return value</term>
    ///         <description> Meaning</description>
    ///     </listheader>
    ///     <item>
    ///         <term>-1</term>
    ///         <description> <paramref name="value" /> is less than -<paramref name="threshold" />.</description>
    ///     </item>
    ///     <item>
    ///         <term>0</term>
    ///         <description><paramref name="value" /> is between -<paramref name="threshold" /> and <paramref name="threshold" />.</description>
    ///     </item>
    ///     <item>
    ///         <term> 1</term>
    ///         <description><paramref name="value" /> is greater than <paramref name="threshold" />.</description>
    ///     </item>
    /// </list>
    /// </returns>
    public static sbyte SignThreshold(float value, float threshold)
    {
        if (Math.Abs(value) >= threshold)
        {
            if (value > 0)
            {
                return 1;
            }
            
            return -1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Clamps <paramref name="value"/> between <paramref name="min"/> and <paramref name="max"/>
    /// </summary>
    /// <param name="value">A value</param>
    /// <param name="min">The minimum value to output</param>
    /// <param name="max">The maximum value to output</param>
    /// <returns><see cref="Vector2"/> The clamped value</returns>
    public static int Clamp(int value, int min, int max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    /// <summary>
    /// Clamps <paramref name="value"/> between <paramref name="min"/> and <paramref name="max"/>
    /// </summary>
    /// <param name="value">A value</param>
    /// <param name="min">The minimum value to output</param>
    /// <param name="max">The maximum value to output</param>
    /// <returns><see cref="Vector2"/> The clamped value</returns>
    public static byte Clamp(byte value, byte min, byte max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    /// <summary>
    /// Safely normalizes <paramref name="vector"/>, or returns <see cref="Vector2.Zero"/> if <paramref name="vector"/> is zero
    /// </summary>
    /// <param name="vector">The vector to normalize</param>
    /// <returns><see cref="Vector2"/> A normalized vector</returns>
    public static Vector2 SafeNormalize(this Vector2 vector)
    {
        return vector.SafeNormalize(Vector2.Zero);
    }

    /// <summary>
    /// Safely normalizes <paramref name="vector"/>, or returns <paramref name="ifZero"/> if <paramref name="vector"/> is zero
    /// </summary>
    /// <param name="vector">The vector to normalize</param>
    /// <param name="ifZero">The vector to return if <paramref name="vector"/> is zero</param>
    /// <returns><see cref="Vector2"/> A normalized vector</returns>
    public static Vector2 SafeNormalize(this Vector2 vector, Vector2 ifZero)
    {
        if (vector == Vector2.Zero)
        {
            return ifZero;
        }
        vector.Normalize();
        return vector;
    }

    /// <summary>
    /// Returns the closest point on the line from <paramref name="lineA"/> to <paramref name="lineB"/> that is closest
    /// to the point <paramref name="closestTo"/>
    /// </summary>
    /// <param name="lineA">The first point of the line</param>
    /// <param name="lineB">The second point of the line</param>
    /// <param name="closestTo">The point to find the closest point to</param>
    /// <returns><see cref="Vector2"/> The closest point on the line</returns>
    public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
    {
        var v = lineB - lineA;
        var w = closestTo - lineA;
        var t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
        t = MathHelper.Clamp(t, 0, 1);

        return lineA + v * t;
    }

    /// <summary>
    /// Get the vector for the angle <paramref name="angleRadians"/> on the trigonometric circle, multiplied by
    /// <paramref name="length"/>
    /// </summary>
    /// <param name="angleRadians">The angle in radians</param>
    /// <param name="length">The magnitude of the output vector</param>
    /// <returns><see cref="Vector2"/> The vector for the angle</returns>
    public static Vector2 AngleToVector(float angleRadians, float length)
    {
        return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
    }

    /// <summary>
    /// Gets the angle difference between <paramref name="radiansA"/> and <paramref name="radiansB"/>
    /// </summary>
    /// <param name="radiansA">The first angle in radians</param>
    /// <param name="radiansB">The second angle in radians</param>
    /// <returns><see cref="float"/> The angle difference between <paramref name="radiansA"/> and
    /// <paramref name="radiansB"/> in radians</returns>
    public static float AngleDiff(float radiansA, float radiansB)
    {
        var diff = (radiansB - radiansA + 3 * MathF.PI) % (2 * MathF.PI) - MathF.PI;
        return diff;
        
        /*var diff = radiansB - radiansA;
        while (diff > MathF.PI)
        {
            diff -= MathF.PI * 2;
        }
        while (diff <= MathF.PI)
        {
            diff += MathF.PI * 2;
        }
        return diff;*/
    }
    
    /// <summary>
    /// Gets the absolute value of the angle difference between <paramref name="radiansA"/> and
    /// <paramref name="radiansB"/>
    /// </summary>
    /// <param name="radiansA">The first angle in radians</param>
    /// <param name="radiansB">The second angle in radians</param>
    /// <returns><see cref="float"/> The angle difference between <paramref name="radiansA"/> and
    /// <paramref name="radiansB"/> in radians</returns>
    public static float AbsAngleDiff(float radiansA, float radiansB)
    {
        return Math.Abs(Calc.AngleDiff(radiansA, radiansB));
    }

    /// <summary>
    /// The angle between <paramref name="from"/> and <paramref name="to"/>
    /// </summary>
    /// <param name="from">The first vector</param>
    /// <param name="to">The second vector</param>
    /// <returns><see cref="float"/> The angle between the two vectors</returns>
    public static float Angle(Vector2 from, Vector2 to)
    {
        return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
    }
    
    /// <summary>
    /// Approaches <paramref name="val"/> towards <paramref name="target"/> by at max <paramref name="maxMove"/>
    /// </summary>
    /// <param name="val">The value to approach</param>
    /// <param name="target">The target value</param>
    /// <param name="maxMove">The maximum amount to move</param>
    /// <returns><see cref="float"/> The new value</returns>
    public static float Approach(float val, float target, float maxMove)
    {
        if (!(val > target))
        {
            return Math.Min(val + maxMove, target);
        }
        return Math.Max(val - maxMove, target);
    }

    #endregion

    #region Vector2

    /// <summary>
    /// Returns a vector that is perpendicular to <paramref name="vector"/>
    /// </summary>
    /// <param name="vector">A vector</param>
    /// <returns><see cref="Vector2"/> A perpendicular vector</returns>
    public static Vector2 Perpendicular(this Vector2 vector)
    {
        return new Vector2(-vector.Y, vector.X);
    }

    /// <summary>
    /// Returns the angle of <paramref name="vector"/> compared to the positive x axis
    /// </summary>
    /// <param name="vector">A vector</param>
    /// <returns><see cref="float"/> The angle</returns>
    public static float Angle(this Vector2 vector)
    {
        return (float)Math.Atan2(vector.Y, vector.X);
    }

    /// <summary>
    /// Returns a <see cref="Vector2"/> with floor applied to each component of <paramref name="vector"/>
    /// </summary>
    /// <param name="vector">A vector</param>
    /// <returns><see cref="Vector2"/> A vector with floor applied</returns>
    public static Vector2 Floor(this Vector2 vector)
    {
        return new Vector2((int)Math.Floor(vector.X), (int)Math.Floor(vector.Y));
    }

    /// <summary>
    /// Snap <paramref name="vector"/> to the nearest multiple of <paramref name="slices"/> and normalize the result
    /// </summary>
    /// <param name="vector">The vector to snap</param>
    /// <param name="slices">The number of slices to snap to</param>
    /// <returns><see cref="Vector2"/> The normalized snapped vector</returns>
    public static Vector2 SnappedNormal(this Vector2 vector, float slices)
    {
        var divider = MathHelper.TwoPi / slices;

        var angle = vector.Angle();
        angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
        return AngleToVector(angle, 1f);
    }

    /// <summary>
    /// Snap <paramref name="vector"/> to the nearest multiple of <paramref name="slices"/>
    /// </summary>
    /// <param name="vector">The vector to snap</param>
    /// <param name="slices">The number of slices to snap to</param>
    /// <returns><see cref="Vector2"/> The snapped vector</returns>
    public static Vector2 Snapped(this Vector2 vector, float slices)
    {
        var divider = MathHelper.TwoPi / slices;

        var angle = vector.Angle();
        angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
        return AngleToVector(angle, vector.Length());
    }
    
    /// <summary>
    /// Approaches <paramref name="value"/> towards <paramref name="target"/> by at max <paramref name="maxMove"/>
    /// </summary>
    /// <param name="value">The value to approach</param>
    /// <param name="target">The target value</param>
    /// <param name="maxMove">The maximum amount to move</param>
    /// <returns><see cref="Vector2"/> The new value</returns>
    public static Vector2 Approach(Vector2 value, Vector2 target, float maxMove)
    {
        if (maxMove == 0f || value == target)
        {
            return value;
        }
        var diff = target - value;
        if (diff.Length() < maxMove)
        {
            return target;
        }
        diff.Normalize();
        return value + diff * maxMove;
    }
    
    /// <summary>
    /// Approaches <paramref name="value"/> towards <paramref name="target"/> by at max <paramref name="maxMove"/> on
    /// each axis
    /// </summary>
    /// <param name="value">The value to approach</param>
    /// <param name="target">The target value</param>
    /// <param name="maxMove">The maximum amount to move</param>
    /// <returns><see cref="Vector2"/> The new value</returns>
    public static Vector2 Approach(Vector2 value, Vector2 target, Vector2 maxMove)
    {
        return new Vector2(
            Approach(value.X, target.X, MathF.Abs(maxMove.X)),
            Approach(value.Y, target.Y, MathF.Abs(maxMove.Y)));
    }
    
    /// <summary>
    /// Linearly interpolates between <paramref name="value"/> and <paramref name="target"/>
    /// </summary>
    /// <param name="value">The first value</param>
    /// <param name="target">The second value</param>
    /// <param name="amount">The amount to lerp</param>
    /// <returns>The interpolated value</returns>
    public static Vector2 Lerp(Vector2 value, Vector2 target, float amount)
    {
        return value + (target - value) * amount;
    }

    #endregion
}