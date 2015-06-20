//Microsoft Surface Manipulations and Inertia Sample for Microsoft Silverlight
//http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=4b281bde-9b01-4890-b3d4-b3b45ca2c2e4
//Overview
//Multitouch support in Windows 7 allows applications to blur the lines between computers and the real 
//world. Touch-optimized applications entice users to touch the objects on the screen, drag them across 
//the screen, rotate and resize them, and flick them across the screen by using their fingers. 
//The manipulations and inertia processor classes allow graphical user interface (GUI) components to move 
//in a natural and intuitive way. Manipulations enable users to move, rotate, and resize components by 
//using their fingers. Inertia enables users to move components by applying forces on the components, 
//such as flicking the component across the screen. The contents of this sample are covered by the 
//Microsoft Surface SDK 1.0 SP1 license agreement, with any additional restrictions noted in the Readme 
//file. The purpose of this download is educational use only and is made available "as-is" without support.

//---------------------------------------------------------------------
// <copyright file="MatrixHelper.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;

namespace MultiTouch.ManipulationLib.Silverlight
{
    /// <summary>
    /// Adds transformation methods to the Matrix class.
    /// </summary>
    public static class MatrixHelper
    {
        /// <summary>
        /// Rotates the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="angle"></param>
        public static void Rotate(ref Matrix matrix, double angle)
        {
            Matrix rotationMatrix = CreateRotationMatrix(angle, 0, 0);
            matrix = Multiply(ref matrix, ref rotationMatrix);  
        }

        /// <summary>
        /// Rotates the given matrix around the specified center.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="angle"></param>
        /// <param name="center"></param>
        public static void RotateAt(ref Matrix matrix, double angle, Point center)
        {
            Matrix rotationMatrix = CreateRotationMatrix(angle, center.X, center.Y);
            matrix = Multiply(ref matrix, ref rotationMatrix);
        }

        /// <summary>
        /// Translates the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="translateX"></param>
        /// <param name="translateY"></param>
        public static void Translate(ref Matrix matrix, double translateX, double translateY)
        {
            matrix.OffsetX += translateX;
            matrix.OffsetY += translateY;
        }

        /// <summary>
        /// Multiplies matrices.
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        private static Matrix Multiply(ref Matrix matrix1, ref Matrix matrix2)
        {
            return new Matrix((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21), 
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22), 
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21), 
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22), 
                ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX, 
                ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        /// <summary>
        /// Creates a rotation matrix.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <returns></returns>
        private static Matrix CreateRotationMatrix(double degrees, double centerX, double centerY)
        {
            double radians = degrees * Math.PI / 180;
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            double offsetX = (centerX * (1.0 - cos)) + (centerY * sin);
            double offsetY = (centerY * (1.0 - cos)) - (centerX * sin);
            Matrix matrix = new Matrix(cos, sin, -sin, cos, offsetX, offsetY);
            return matrix;

        }
    }
}
