using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace SLibrary.Util
{
    public static class RandomHelper
    {
        private static int _counter = 0;


        public static int ResetCount { set; private get; }

        private static void Increase()
        {
            _counter++;
            if (_counter < ResetCount) return;
            UnityEngine.Random.InitState((int) DateTimeOffset.Now.ToUnixTimeSeconds());
            _counter = 0;
        }

        public static int Range(int minValue, int maxValue)
        {
            Increase();
            return UnityEngine.Random.Range(minValue, maxValue);
        }

        public static float Range(float minValue, float maxValue)
        {
            Increase();
            return UnityEngine.Random.Range(minValue, maxValue);
        }

        public static Vector3 insideUnitSphere
        {
            get
            {
                Increase();
                return Random.insideUnitSphere;
            }
        }

        public static Vector2 insideUnitCircle
        {
            get
            {
                Increase();
                return Random.insideUnitCircle;
            }
        }

        public static float value
        {
            get
            {
                Increase();
                return Random.value;
            }
        }

        public static Vector3 onUnitSphere
        {
            get
            {
                Increase();
                return Random.onUnitSphere;
            }
        }

        /// <summary>
        ///   <para>Returns a random rotation (Read Only).</para>
        /// </summary>
        public static UnityEngine.Quaternion rotation
        {
            get
            {
                Increase();
                return Random.rotation;
            }
        }

        /// <summary>
        ///   <para>Returns a random rotation with uniform distribution (Read Only).</para>
        /// </summary>
        public static Quaternion rotationUniform
        {
            get
            {
                Increase();
                return Random.rotationUniform;
            }
        }

        /// <summary>
        ///   <para>Generates a random color from HSV and alpha ranges.</para>
        /// </summary>
        /// <param name="hueMin">Minimum hue [0..1].</param>
        /// <param name="hueMax">Maximum hue [0..1].</param>
        /// <param name="saturationMin">Minimum saturation [0..1].</param>
        /// <param name="saturationMax">Maximum saturation[0..1].</param>
        /// <param name="valueMin">Minimum value [0..1].</param>
        /// <param name="valueMax">Maximum value [0..1].</param>
        /// <param name="alphaMin">Minimum alpha [0..1].</param>
        /// <param name="alphaMax">Maximum alpha [0..1].</param>
        /// <returns>
        ///   <para>A random color with HSV and alpha values in the input ranges.</para>
        /// </returns>
        public static Color ColorHSV()
        {
            Increase();
            return Random.ColorHSV(0.0f, 1f, 0.0f, 1f, 0.0f, 1f, 1f, 1f);
        }

        /// <summary>
        ///   <para>Generates a random color from HSV and alpha ranges.</para>
        /// </summary>
        /// <param name="hueMin">Minimum hue [0..1].</param>
        /// <param name="hueMax">Maximum hue [0..1].</param>
        /// <param name="saturationMin">Minimum saturation [0..1].</param>
        /// <param name="saturationMax">Maximum saturation[0..1].</param>
        /// <param name="valueMin">Minimum value [0..1].</param>
        /// <param name="valueMax">Maximum value [0..1].</param>
        /// <param name="alphaMin">Minimum alpha [0..1].</param>
        /// <param name="alphaMax">Maximum alpha [0..1].</param>
        /// <returns>
        ///   <para>A random color with HSV and alpha values in the input ranges.</para>
        /// </returns>
        public static Color ColorHSV(float hueMin, float hueMax)
        {
            Increase();
            return Random.ColorHSV(hueMin, hueMax, 0.0f, 1f, 0.0f, 1f, 1f, 1f);
        }

        /// <summary>
        ///   <para>Generates a random color from HSV and alpha ranges.</para>
        /// </summary>
        /// <param name="hueMin">Minimum hue [0..1].</param>
        /// <param name="hueMax">Maximum hue [0..1].</param>
        /// <param name="saturationMin">Minimum saturation [0..1].</param>
        /// <param name="saturationMax">Maximum saturation[0..1].</param>
        /// <param name="valueMin">Minimum value [0..1].</param>
        /// <param name="valueMax">Maximum value [0..1].</param>
        /// <param name="alphaMin">Minimum alpha [0..1].</param>
        /// <param name="alphaMax">Maximum alpha [0..1].</param>
        /// <returns>
        ///   <para>A random color with HSV and alpha values in the input ranges.</para>
        /// </returns>
        public static Color ColorHSV(
            float hueMin,
            float hueMax,
            float saturationMin,
            float saturationMax)
        {
            Increase();
            return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, 0.0f, 1f, 1f, 1f);
        }

        /// <summary>
        ///   <para>Generates a random color from HSV and alpha ranges.</para>
        /// </summary>
        /// <param name="hueMin">Minimum hue [0..1].</param>
        /// <param name="hueMax">Maximum hue [0..1].</param>
        /// <param name="saturationMin">Minimum saturation [0..1].</param>
        /// <param name="saturationMax">Maximum saturation[0..1].</param>
        /// <param name="valueMin">Minimum value [0..1].</param>
        /// <param name="valueMax">Maximum value [0..1].</param>
        /// <param name="alphaMin">Minimum alpha [0..1].</param>
        /// <param name="alphaMax">Maximum alpha [0..1].</param>
        /// <returns>
        ///   <para>A random color with HSV and alpha values in the input ranges.</para>
        /// </returns>
        public static Color ColorHSV(
            float hueMin,
            float hueMax,
            float saturationMin,
            float saturationMax,
            float valueMin,
            float valueMax)
        {
            Increase();
            return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, 1f, 1f);
        }

        /// <summary>
        ///   <para>Generates a random color from HSV and alpha ranges.</para>
        /// </summary>
        /// <param name="hueMin">Minimum hue [0..1].</param>
        /// <param name="hueMax">Maximum hue [0..1].</param>
        /// <param name="saturationMin">Minimum saturation [0..1].</param>
        /// <param name="saturationMax">Maximum saturation[0..1].</param>
        /// <param name="valueMin">Minimum value [0..1].</param>
        /// <param name="valueMax">Maximum value [0..1].</param>
        /// <param name="alphaMin">Minimum alpha [0..1].</param>
        /// <param name="alphaMax">Maximum alpha [0..1].</param>
        /// <returns>
        ///   <para>A random color with HSV and alpha values in the input ranges.</para>
        /// </returns>
        public static Color ColorHSV(
            float hueMin,
            float hueMax,
            float saturationMin,
            float saturationMax,
            float valueMin,
            float valueMax,
            float alphaMin,
            float alphaMax)
        {
            Increase();
            return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, alphaMin,
                alphaMax);
        }
    }
}