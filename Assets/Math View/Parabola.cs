using System.Collections.Generic;
using UnityEngine;

namespace Math_View
{
    /// <summary>
    /// Class to store data for a parabola
    /// </summary>
    public class Parabola
    {
        private Vector3 _formulaValues;
        /// <summary>
        /// Formula values a, b and c (of y = ax²+bx+c). The vector's x corresponds to a, y to b, z to c.
        /// </summary>
        public Vector3 FormulaValues
        {
            get => _formulaValues;

            set
            {
                // limit the parameters
                value.x = Mathf.Clamp(value.x, -100f, 100f);
                value.y = Mathf.Clamp(value.y, -100f, 100f);
                value.z = Mathf.Clamp(value.z, -100f, 100f);

                if (Mathf.Abs(value.x) <= 0.01f)
                    // no zero or near zero parameter a
                {
                    value.x = 0.01f;
                }

                _formulaValues = value;

                RecalculateHandlesFromABC();
            }
        }

        private Vector2 _originHandle;
        /// <summary>
        /// Coordinates of the origin handle of the parabola.
        /// </summary>
        public Vector2 OriginHandle
        {
            get => _originHandle;

            set
            {
                _originHandle = value;
                RecalculateABCFromHandles();
            }
        }

        private Vector2 _curveHandle;
        /// <summary>
        /// Coordinates of the curve handle of the parabola.
        /// </summary>
        public Vector2 CurveHandle
        {
            get => _curveHandle;

            set
            {
                _curveHandle = value;
                RecalculateABCFromHandles();
            }
        }

        public Color Color { get; private set; }

        public bool Locked { get; set; }
        /// <summary>
        /// The name of the parabola.
        /// </summary>
        public string Name { get; set; }

        private bool _snapToGrid;
        /// <summary>
        /// Whether or not the parabola's handles snap to the grid
        /// </summary>
        public bool SnapToGrid
        {
            get => _snapToGrid;

            set
            {
                _snapToGrid = value;

                if (_snapToGrid)
                {
                    RasterizeOriginAndHandles();
                }
            }
        }
        /// <summary>
        /// Whether or not the parabola is infinite
        /// </summary>
        public bool Infinite { get; set; }
        /// <summary>
        /// Object that decides which inputs and fields of the parabola are interactable or visible
        /// </summary>
        public ParabolaLock Lock { get; }

        /// <summary>
        /// Constructor for a parabola object.
        /// </summary>
        /// <param name="parabolaLock">ParabolaLock object that stores accessibility for buttons and values</param>
        /// <param name="name">The name of the parabola</param>
        /// <param name="color">The color of the parabola</param>
        /// <param name="originHandle">Coordinates of the origin handle of the parabola</param>
        /// <param name="curveHandle">Coordinates of the curve handle of the parabola</param>
        /// <param name="infinite">Whether or not the parabola is infinite</param>
        public Parabola(ParabolaLock parabolaLock, string name, Color color, Vector2 originHandle, Vector2 curveHandle, bool infinite)
        {
            Lock = parabolaLock;
            Name = name;
            Color = color;
            _originHandle = originHandle;
            _curveHandle = curveHandle;
            RecalculateABCFromHandles();
            Infinite = infinite;
            SnapToGrid = true;
        }

        /// <summary>
        /// Calculates the origin of the parabola from its formula
        /// </summary>
        /// <returns>The coordinates of the origin of the parabola</returns>
        private Vector2 GetOriginFromABC()
        {
            var a = _formulaValues.x;
            var b = _formulaValues.y;

            if (Mathf.Abs(a) <= 0.01f)
            {
                return new Vector2(0f, 0f);
            }

            var x = -b / (2f * a);
            return new Vector2(x, Solve(x));
        }

        /// <summary>
        /// Recalculates the parabola's formula from the positions of its handles
        /// </summary>
        private void RecalculateABCFromHandles()
        {
            if (Mathf.Abs(_originHandle.x - _curveHandle.x) <= 0.01f)
            {
                return;
            }

            var curveHandleCopy = _originHandle;
            curveHandleCopy.x += _originHandle.x - _curveHandle.x;
            curveHandleCopy.y = _curveHandle.y;

            var v = new Vector4(_originHandle.y, _curveHandle.y, curveHandleCopy.y, 1f);
            var mat = new Matrix4x4();
            mat.SetRow(0, new Vector4(_originHandle.x * _originHandle.x, _originHandle.x, 1f, 0f));
            mat.SetRow(1, new Vector4(_curveHandle.x * _curveHandle.x, _curveHandle.x, 1f, 0f));
            mat.SetRow(2, new Vector4(curveHandleCopy.x * curveHandleCopy.x, curveHandleCopy.x, 1f, 0f));
            mat.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            _formulaValues = mat.inverse * v;
        }

        /// <summary>
        /// Recalculates the positions of the parabola's handles from its formula
        /// </summary>
        private void RecalculateHandlesFromABC()
        {
            var originalDistance = _curveHandle.x - _originHandle.x;
            _originHandle = GetOriginFromABC();
            _curveHandle = new Vector2(_originHandle.x + originalDistance, Solve(_originHandle.x + originalDistance));
        }

        public void RandomizeColor()
        {
            var color = new Color();

            var cycle = Random.Range(0, 5);
		
            switch (cycle)
            {
                case 0:
                    color.r = Random.Range(0f, 1f);
                    color.g = 0f;
                    color.b = 1f;
                    break;
                case 1:
                    color.r = Random.Range(0f, 1f);
                    color.g = 1f;
                    color.b = 0f;
                    break;
                case 2:
                    color.r = 0f;
                    color.g = Random.Range(0f, 1f);
                    color.b = 1f;
                    break;
                case 3:
                    color.r = 1f;
                    color.g = Random.Range(0f, 1f);
                    color.b = 0f;
                    break;
                case 4:
                    color.r = 0f;
                    color.g = 1f;
                    color.b = Random.Range(0f, 1f);
                    break;
                default:
                    color.r = 1f;
                    color.g = 0f;
                    color.b = Random.Range(0f, 1f);
                    break;
            }

            color.a = 1f;
            Color = color;
        }

        /// <summary>
        /// Forces the parabola's handles to conform to the grid.
        /// </summary>
        public void RasterizeOriginAndHandles()
        {
            var rasterizedOriginHandle = _originHandle;
            rasterizedOriginHandle.x = Mathf.Round(rasterizedOriginHandle.x);
            rasterizedOriginHandle.y = Mathf.Round(rasterizedOriginHandle.y);
            OriginHandle = rasterizedOriginHandle;

            var rasterizedCurveHandle = _curveHandle;
            rasterizedCurveHandle.x = Mathf.Round(rasterizedCurveHandle.x);
            rasterizedCurveHandle.y = Mathf.Round(rasterizedCurveHandle.y);
            CurveHandle = rasterizedCurveHandle;
        }

        /// <summary>
        /// Returns the value of the parabola's formula for a given x.
        /// </summary>
        /// <param name="x">The value for which to solve the formula.</param>
        /// <returns>The value of the parabola's formula for a given x</returns>
        public float Solve(float x)
        {
            var a = _formulaValues.x;
            var b = _formulaValues.y;
            var c = _formulaValues.z;

            return a * x * x + b * x + c;
        }

        /// <summary>
        /// Lists x coordinates for where the parabola intersects a given y value.
        /// </summary>
        /// <param name="y">The y value to intersect the parabola with</param>
        /// <returns>x coordinates for where the parabola intersects a given y value</returns>
        public List<float> Intersect(float y)
        {
            var a = _formulaValues.x;
            var b = _formulaValues.y;
            var c = _formulaValues.z;

            var output = new List<float>();

            var squareRootPart = b * b - 4.0f * a * (c - y);

            if (squareRootPart <= 0.01f)
            {
                return null;
            }

            squareRootPart = Mathf.Sqrt(squareRootPart);

            if (Mathf.Abs(a) <= 0.01f)
            {
                return null;
            }

            output.Add((-b - squareRootPart) / (2f * a));

            if (Mathf.Abs(squareRootPart) <= 0.01f)
            {
                return output;
            }

            output.Add((-b + squareRootPart) / (2f * a));

            return output;
        }
    }
}
