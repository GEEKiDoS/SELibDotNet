using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SELib.Utilities
{
    /// <summary>
    /// Generic interface for all key data
    /// </summary>
    public interface KeyData
    {
        // A generic interface for a container
    }

    /// <summary>
    /// A container for a vector (XY) (Normalized)
    /// </summary>
    public class Vector2 : KeyData
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2() { X = 0; Y = 0; }
        public Vector2(float XCoord, float YCoord) { X = XCoord; Y = YCoord; }

        public override bool Equals(object obj)
        {
            Vector2 vec = (Vector2)obj;
            return (vec.X == X && vec.Y == Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static readonly Vector2 Zero = new Vector2() { X = 0, Y = 0 };
        public static readonly Vector2 One = new Vector2() { X = 1, Y = 1 };
    }

    /// <summary>
    /// A container for a vector (XYZ) (Normalized)
    /// </summary>
    public class Vector3 : Vector2
    {
        public double Z { get; set; }

        public Vector3() { X = 0; Y = 0; Z = 0; }
        public Vector3(float XCoord, float YCoord, float ZCoord) { X = XCoord; Y = YCoord; Z = ZCoord; }
        public Vector3(double XCoord, double YCoord, double ZCoord) { X = XCoord; Y = YCoord; Z = ZCoord; }

        public override bool Equals(object obj)
        {
            Vector3 vec = (Vector3)obj;
            return (vec.X == X && vec.Y == Y && vec.Z == Z);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Z.GetHashCode();
        }

        public static Vector3 operator *(Vector3 me, Vector3 Value)
        {
            // Multiply and make a new result
            return new Vector3(me.X * Value.X, me.Y * Value.Y, me.Z * Value.Z);
        }

        public static Vector3 operator *(Vector3 me, double Value)
        {
            // Multiply and make a new result
            return new Vector3(me.X * Value, me.Y * Value, me.Z * Value);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public Vector3 (Matrix4x4 m)
        {
            X = m[0, 3];
            Y = m[1, 3];
            Z = m[2, 3];
        }

        new public static readonly Vector3 Zero = new Vector3() { X = 0, Y = 0, Z = 0 };
        new public static readonly Vector3 One = new Vector3() { X = 1, Y = 1, Z = 1 };
    }

    /// <summary>
    /// A container for a color (RGBA)
    /// </summary>
    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public Color() { R = 255; G = 255; B = 255; A = 255; }
        public Color(byte Red, byte Green, byte Blue, byte Alpha) { R = Red; G = Green; B = Blue; A = Alpha; }

        public override bool Equals(object obj)
        {
            Color vec = (Color)obj;
            return (vec.R == R && vec.G == G && vec.B == B && vec.A == A);
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        }

        public static readonly Color White = new Color(255, 255, 255, 255);
    }

    /// <summary>
    /// A container for a quaternion rotation (XYZW) (Normalized)
    /// </summary>
    public class Quaternion : Vector3
    {
        public double W { get; set; }

        public Quaternion() { X = 0; Y = 0; Z = 0; W = 1; }
        public Quaternion(float XCoord, float YCoord, float ZCoord, float WCoord) { X = XCoord; Y = YCoord; Z = ZCoord; W = WCoord; }
        public Quaternion(double XCoord, double YCoord, double ZCoord, double WCoord) { X = XCoord; Y = YCoord; Z = ZCoord; W = WCoord; }

        public Vector3 ToEulerAngles()
        {
            var sinr_cosp = 2 * (W * X + Y * Z);
            var cosr_cosp = 1 - 2 * (X * X + Y * Y);

            var roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            var sinp = 2 * (W * Y - Z * X);

            var pitch = 0.0f;

            if (Math.Abs(sinp) >= 1)
                pitch = (float)(Math.PI / 2 * Math.Sign(sinp));
            else
                pitch = (float)Math.Asin(sinp);

            var siny_cosp = 2 * (W * Z + X * Y);
            var cosy_cosp = 1 - 2 * (Y * Y + Z * Z);

            var yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return new Vector3(roll, pitch, yaw);
        }

        public static Quaternion FromEulerAngles(double roll, double pitch, double yaw)
        {
            var cy = Math.Cos(yaw * 0.5f);
            var sy = Math.Sin(yaw * 0.5f);
            var cp = Math.Cos(pitch * 0.5f);
            var sp = Math.Sin(pitch * 0.5f);
            var cr = Math.Cos(roll * 0.5f);
            var sr = Math.Sin(roll * 0.5f);

            return new Quaternion
            {
                W = cr * cp * cy + sr * sp * sy,
                X = sr * cp * cy - cr * sp * sy,
                Y = cr * sp * cy + sr * cp * sy,
                Z = cr * cp * sy - sr * sp * cy
            };
        }

        Quaternion FromAxisRotation(Vector3 Axis, float Rad)
        {
            // Calculate and retern a new quat
            var AngleRes = Math.Sin(Rad / 2.0f);
            var WCalc = Math.Cos(Rad / 2.0f);
            // Get vector
            var Result = Axis * AngleRes;
            // Return
            return new Quaternion(Result.X, Result.Y, Result.Z, WCalc);
        }

        public Quaternion(Matrix4x4 m)
        {
            double tr = m[0, 0] + m[1, 1] + m[2, 2];
            double x, y, z, w;
            if (tr > 0f)
            {
                double s = Math.Sqrt(1f + tr) * 2f;
                w = 0.25f * s;
                x = (m[2, 1] - m[1, 2]) / s;
                y = (m[0, 2] - m[2, 0]) / s;
                z = (m[1, 0] - m[0, 1]) / s;
            }
            else if ((m[0, 0] > m[1, 1]) && (m[0, 0] > m[2, 2]))
            {
                double s = Math.Sqrt(1f + m[0, 0] - m[1, 1] - m[2, 2]) * 2f;
                w = (m[2, 1] - m[1, 2]) / s;
                x = 0.25f * s;
                y = (m[0, 1] + m[1, 0]) / s;
                z = (m[0, 2] + m[2, 0]) / s;
            }
            else if (m[1, 1] > m[2, 2])
            {
                double s = Math.Sqrt(1f + m[1, 1] - m[0, 0] - m[2, 2]) * 2f;
                w = (m[0, 2] - m[2, 0]) / s;
                x = (m[0, 1] + m[1, 0]) / s;
                y = 0.25f * s;
                z = (m[1, 2] + m[2, 1]) / s;
            }
            else
            {
                double s = Math.Sqrt(1f + m[2, 2] - m[0, 0] - m[1, 1]) * 2f;
                w = (m[1, 0] - m[0, 1]) / s;
                x = (m[0, 2] + m[2, 0]) / s;
                y = (m[1, 2] + m[2, 1]) / s;
                z = 0.25f * s;
            }

            X = x; Y = y; Z = z; W = w;
        }

        public static Quaternion operator *(Quaternion left, Quaternion Value)
        {
            return new Quaternion(
                // Calculate the new X
                left.W * Value.X + left.X * Value.W + left.Y * Value.Z - left.Z * Value.Y,
                // Calculate the new Y
                left.W * Value.Y - left.X * Value.Z + left.Y * Value.W + left.Z * Value.X,
                // Calculate the new Z
                left.W * Value.Z + left.X * Value.Y - left.Y * Value.X + left.Z * Value.W,
                // Calculate the new W
                left.W * Value.W - left.X * Value.X - left.Y * Value.Y - left.Z * Value.Z);
        }

        public static Quaternion operator ~(Quaternion me)
        {
            // Conjugate
            return new Quaternion(-me.X, -me.Y, -me.Z, me.W);
        }

        public override bool Equals(object obj)
        {
            Quaternion vec = (Quaternion)obj;
            return (vec.X == X && vec.Y == Y && vec.Z == Z && vec.W == W);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ W.GetHashCode();
        }

        public static readonly Quaternion Identity = new Quaternion() { X = 0, Y = 0, Z = 0, W = 1 };
    }

    public class Matrix4x4
    {
        double[,] m = new double[4, 4];

        public Matrix4x4()
        {
            for(int i = 0; i < 16; i++)
            {
                var x = i % 4;
                var y = i / 4;

                m[x, y] = (i % 5) == 0 ? 1.0f : 0.0f;
            }
        }

        public double this[int x,int y]
        {
            get => Mat(x, y);
            set => Mat(x, y) = value;
        }

        public Matrix4x4(float[,] from)
        {
            for (int i = 0; i < 16; i++)
            {
                var x = i % 4;
                var y = i / 4;

                m[x, y] = from[x, y];
            }
        }

        public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 Value)
        {
            // The result buffer
            Matrix4x4 Result = new Matrix4x4();
            // Loop for cols
            for (int i = 0; i < 4; i++)
            {
                // Loop for rows
                for (int j = 0; j < 4; j++)
                {
                    // Buffer
                    double Buffer = 0;
                    // Loop
                    for (int k = 0; k < 4; k++)
                    {
                        // Multiply
                        Buffer += Value.Mat(i, k) * left.Mat(k, j);
                    }
                    // Set
                    Result.Mat(i, j) = Buffer;
                }
            }
            // Return result
            return Result;
        }

        public ref double Mat(int x, int y)
        {
            return ref m[x, y];
        }

        public static Matrix4x4 CreateFromQuaternion(Quaternion Value)
        {
            // Buffer
            Matrix4x4 Result = new Matrix4x4();

            // Get squared calculations
            double XX = Value.X * Value.X;
            double XY = Value.X * Value.Y;
            double XZ = Value.X * Value.Z;
            double XW = Value.X * Value.W;

            double YY = Value.Y * Value.Y;
            double YZ = Value.Y * Value.Z;
            double YW = Value.Y * Value.W;

            double ZZ = Value.Z * Value.Z;
            double ZW = Value.Z * Value.W;

            // Calculate matrix
            Result.Mat(0, 0) = 1 - 2 * (YY + ZZ);
            Result.Mat(1, 0) = 2 * (XY - ZW);
            Result.Mat(2, 0) = 2 * (XZ + YW);
            Result.Mat(3, 0) = 0;

            Result.Mat(0, 1) = 2 * (XY + ZW);
            Result.Mat(1, 1) = 1 - 2 * (XX + ZZ);
            Result.Mat(2, 1) = 2 * (YZ - XW);
            Result.Mat(3, 1) = 0;

            Result.Mat(0, 2) = 2 * (XZ - YW);
            Result.Mat(1, 2) = 2 * (YZ + XW);
            Result.Mat(2, 2) = 1 - 2 * (XX + YY);
            Result.Mat(3, 2) = 0;

            Result.Mat(0, 3) = 0;
            Result.Mat(1, 3) = 0;
            Result.Mat(2, 3) = 0;
            Result.Mat(3, 3) = 1;

            // Return it
            return Result;
        }

        public Quaternion TransformQuaternion(Quaternion quaternion)
        {
            var maq = CreateFromQuaternion(quaternion);

            var targetMatrix = this * maq;

            var quat = new Quaternion(targetMatrix);

            // IDK why but hey it works
            quat.X *= -1;
            quat.Y *= -1;
            quat.Z *= -1;

            return quat;
        }

        public Vector3 TransformVector(Vector3 Vector)
        {
            // Buffer
            Vector3 Result = new Vector3();

            // Calculate
            Result.X = (Vector.X * Mat(0, 0)) + (Vector.Y * Mat(1, 0)) + (Vector.Z * Mat(2, 0)) + Mat(3, 0);
            Result.Y = (Vector.X * Mat(0, 1)) + (Vector.Y * Mat(1, 1)) + (Vector.Z * Mat(2, 1)) + Mat(3, 1);
            Result.Z = (Vector.X * Mat(0, 2)) + (Vector.Y * Mat(1, 2)) + (Vector.Z * Mat(2, 2)) + Mat(3, 2);

            // Return result
            return Result;
        }
    }
}
