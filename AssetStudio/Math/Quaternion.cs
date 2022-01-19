using System;
using System.Runtime.InteropServices;

namespace AssetStudio
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Quaternion : IEquatable<Quaternion>
    {


        public float X;
        public float Y;
        public float Z;
        public float W;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Quaternion(Vector3 v, float w)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = w;
        }
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                    default: throw new ArgumentOutOfRangeException(nameof(index), "Invalid Quaternion index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index), "Invalid Quaternion index!");
                }
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2) ^ (W.GetHashCode() >> 1);
        }

        public override bool Equals(object other)
        {
            if (!(other is Quaternion))
                return false;
            return Equals((Quaternion)other);
        }

        public bool Equals(Quaternion other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        private static bool IsEqualUsingDot(float dot)
        {
            return dot > 1.0f - kEpsilon;
        }

        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return IsEqualUsingDot(Dot(lhs, rhs));
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !(lhs == rhs);
        }

        private const float kEpsilon = 0.000001F;
        private const float radToDeg = (float)(180.0 / Math.PI);
        private const float degToRad = (float)(Math.PI / 180.0);
        private static Quaternion FromEulerRad(Vector3 euler)
        {
            var yaw = euler.X;
            var pitch = euler.Y;
            var roll = euler.Z;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)System.Math.Sin((float)rollOver2);
            float cosRollOver2 = (float)System.Math.Cos((float)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)System.Math.Sin((float)pitchOver2);
            float cosPitchOver2 = (float)System.Math.Cos((float)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)System.Math.Sin((float)yawOver2);
            float cosYawOver2 = (float)System.Math.Cos((float)yawOver2);
            Quaternion result;
            result.X = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
            result.Z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.W = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            return result;

        }

        /// <summary>
        ///   <para>Returns the angle in degrees between two rotations /a/ and /b/.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Angle(Quaternion a, Quaternion b)
        {
            float f = Quaternion.Dot(a, b);
            return (float)Math.Acos(Math.Min(Math.Abs(f), 1f)) * 2f * radToDeg;
        }

        public static Quaternion Euler(float x, float y, float z)
        {
            return Quaternion.FromEulerRad(new Vector3((float)x, (float)y, (float)z) * degToRad);
        }

        /// <summary>
        ///   <para>Creates a rotation which rotates /angle/ degrees around /axis/.</para>
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            return Quaternion.AngleAxis(angle, ref axis);
        }
        private static Quaternion AngleAxis(float degress, ref Vector3 axis)
        {
            if (axis.sqrMagnitude == 0.0f)
                return identity;

            Quaternion result = identity;
            var radians = degress * degToRad;
            radians *= 0.5f;
            axis.Normalize();
            axis = axis * (float)System.Math.Sin(radians);
            result.X = axis.X;
            result.Y = axis.Y;
            result.Z = axis.Z;
            result.W = (float)System.Math.Cos(radians);

            return Normalize(result);
        }
        public void ToAngleAxis(out float angle, out Vector3 axis)
        {
            Quaternion.ToAxisAngleRad(this, out axis, out angle);
            angle *= radToDeg;
        }
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            XYZ *= scale;
            W *= scale;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            Quaternion result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref Quaternion q, out Quaternion result)
        {
            float scale = 1.0f / q.Length;
            result = new Quaternion(q.XYZ * scale, q.W * scale);
        }

        private static void ToAxisAngleRad(Quaternion q, out Vector3 axis, out float angle)
        {
            if (System.Math.Abs(q.W) > 1.0f)
                q.Normalize();
            angle = 2.0f * (float)System.Math.Acos(q.W); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.0001f)
            {
                axis = q.XYZ / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                axis = new Vector3(1, 0, 0);
            }
        }
        public Vector3 XYZ
        {
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
            get
            {
                return new Vector3(X, Y, Z);
            }
        }

        public static Quaternion identity
        {
            get
            {
                return new Quaternion(0f, 0f, 0f, 1f);
            }
        }

        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            }
        }
    }
}
