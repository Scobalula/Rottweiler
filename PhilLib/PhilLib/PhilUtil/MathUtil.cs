/*
    Copyright (c) 2018 Philip/Scobalula - Utility Lib

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;

namespace PhilUtil.MathUtil
{
    /// <summary>
    /// Holds a 3x3 Matrix
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// X Components
        /// </summary>
        public Vector3 X = new Vector3();

        /// <summary>
        /// Y Components
        /// </summary>
        public Vector3 Y = new Vector3();

        /// <summary>
        /// Z Components
        /// </summary>
        public Vector3 Z = new Vector3();

        /// <summary>
        /// Returns this matrix as a formatted string.
        /// </summary>
        public override string ToString()
        {
            return String.Format
                (
                "[ {0:0.000000}, {1:0.000000}, {2:0.000000} ]\n[ {3:0.000000}, {4:0.000000}, {5:0.000000} ]\n[ {6:0.000000}, {7:0.000000}, {8:0.000000} ]",
                X.X, X.Y, X.Z, Y.X, Y.Y, Y.Z, Z.X, Z.Y, Z.Z);
        }
    }

    /// <summary>
    /// Holds a 3 Dimensional Vector
    /// </summary>
    public class Vector3
    {
        /// <summary>
        /// X Component
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y Component
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z Component
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Creates a new 3 Dimensional Vector
        /// </summary>
        public Vector3() { }

        /// <summary>
        /// Creates a new 3 Dimensional Vector with XYZ Components
        /// </summary>
        /// <param name="x">X Component</param>
        /// <param name="y">Y Component</param>
        /// <param name="z">Z Component</param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z, };
        }
    }

    /// <summary>
    /// Holds Quaternion Rotation Data
    /// </summary>
    public class Quaternion
    {
        /// <summary>
        /// X Component
        /// </summary>
        public double X { get; set; }
        
        /// <summary>
        /// Y Component
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z Component
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// W Component
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Converts a Quaternion to a 3x3 Matrix
        /// </summary>
        /// <returns>Resulting matrix</returns>
        public Matrix ToMatrix()
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
            Matrix matrix = new Matrix();

            double tempVar1;
            double tempVar2;

            double xSquared = X * X;
            double ySquared = Y * Y;
            double zSquared = Z * Z;
            double wSquared = W * W;

            double inverse = 1 / (xSquared + ySquared + zSquared + wSquared);

            matrix.X.X = Math.Round((xSquared - ySquared - zSquared + wSquared) * inverse, 4);
            matrix.Y.Y = Math.Round((-xSquared + ySquared - zSquared + wSquared) * inverse, 4);
            matrix.Z.Z = Math.Round((-xSquared - ySquared + zSquared + wSquared) * inverse, 4);

            tempVar1 = (X * Y);
            tempVar2 = (Z * W);

            matrix.Y.X = Math.Round(2.0 * (tempVar1 + tempVar2) * inverse, 4);
            matrix.X.Y = Math.Round(2.0 * (tempVar1 - tempVar2) * inverse, 4);

            tempVar1 = (X * Z);
            tempVar2 = (Y * W);

            matrix.Z.X = Math.Round(2.0 * (tempVar1 - tempVar2) * inverse, 4);
            matrix.X.Z = Math.Round(2.0 * (tempVar1 + tempVar2) * inverse, 4);

            tempVar1 = (Y * Z);
            tempVar2 = (X * W);

            matrix.Z.Y = Math.Round(2.0 * (tempVar1 + tempVar2) * inverse, 4);
            matrix.Y.Z = Math.Round(2.0 * (tempVar1 - tempVar2) * inverse, 4);

            return matrix;
        }
    }
}
