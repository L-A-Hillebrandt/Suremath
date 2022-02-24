using UnityEngine;

namespace Math_View
{
    /// <summary>
    /// Class to store data for a line
    /// </summary>
    public class Line
    {
        /// <summary>
        /// The ID of the line, useful if a specific line needs to be referenced in another shape (e.g. an angle)
        /// </summary>
        public int ID { get; private set; } = 0;

        Vector2 _formulaValues;
        /// <summary>
        /// m and b in the formula y = mx + b. The vector's x is m, the vector's y is b. 
        /// </summary>
        public Vector2 FormulaValues
        {
            get => _formulaValues;

            set
            {
                // limit the parameters
                value.x = Mathf.Clamp(value.x, -100f, 100f);
                value.y = Mathf.Clamp(value.y, -100f, 100f);

                _formulaValues = value;

                RecalculateHandlesFromFormula();
                RecalculateCoefficientsFromFormula();
                CalculateLength();
            }
        }

        Vector2 _pointAHandle;
        /// <summary>
        /// Coordinates of the first handle the line can be manipulated with.
        /// </summary>
        public Vector2 PointAHandle
        {
            get => _pointAHandle;

            set
            {
                _pointAHandle = value;
                RecalculateFormulaFromHandles();
                RecalculateCoefficientsFromFormula();
                CalculateLength();
            }
        }

        Vector2 _pointBHandle;
        /// <summary>
        /// Coordinates of the second handle the line can be manipulated with.
        /// </summary>
        public Vector2 PointBHandle
        {
            get => _pointBHandle;

            set
            {
                _pointBHandle = value;
                RecalculateFormulaFromHandles();
                RecalculateCoefficientsFromFormula();
                CalculateLength();
            }
        }

        /// <summary>
        /// Coefficient for calculating an intersection between two lines
        /// </summary>
        public float CoefficientA
        {
            get;
            private set;
        }

        /// <summary>
        /// Coefficient for calculating an intersection between two lines
        /// </summary>
        public float CoefficientB
        {
            get;
            private set;
        }

        /// <summary>
        /// Coefficient for calculating an intersection between two lines
        /// </summary>
        public float CoefficientC
        {
            get;
            private set;
        }

        /// <summary>
        /// The length of the line.
        /// </summary>
        public float Length
        {
            get;
            private set;
        }

        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color Color { get; private set; }

        public bool Locked { get; set; }
        /// <summary>
        /// Object that decides which inputs and fields of the line are interactable or visible
        /// </summary>
        public LineLock Lock { get; }
        /// <summary>
        /// The name of the line
        /// </summary>
        public string Name { get; set; }

        bool _snapToGrid;
        /// <summary>
        /// Whether or not the line's handles snap to the grid
        /// </summary>
        public bool SnapToGrid
        {
            get => _snapToGrid;

            set
            {
                _snapToGrid = value;

                if (_snapToGrid)
                {
                    RasterizeHandles();
                }
            }
        }
        /// <summary>
        /// Whether or not the line is infinite
        /// </summary>
        public bool Infinite { get; set; }

        /// <summary>
        /// Constructor for a line object.
        /// </summary>
        /// <param name="lineLock">LineLock object that stores accessibility for buttons and values</param>
        /// <param name="name">The name of the line</param>
        /// <param name="color">The color of the line</param>
        /// <param name="pointAHandle">Coordinates for one of the handles of the line</param>
        /// <param name="pointBHandle">Coordinates for the other handle of the line</param>
        /// <param name="infinite">Whether or not the line is infinite</param>
        /// <param name="id">The ID of the line</param>
        /// <param name="snapToGrid">Whether or not the line's handles snap to the grid</param>
        public Line(LineLock lineLock, string name, Color color, Vector2 pointAHandle, Vector2 pointBHandle, bool infinite, int id, bool snapToGrid = true)
        {
            ID = id;
            Name = name;
            Color = color;
            _pointAHandle = pointAHandle;
            _pointBHandle = pointBHandle;
            RecalculateFormulaFromHandles();
            RecalculateCoefficientsFromFormula();
            CalculateLength();
            Infinite = infinite;
            SnapToGrid = snapToGrid;
            Lock = lineLock ?? new LineLock();
        }

        /// <summary>
        /// Constructor for a line object.
        /// </summary>
        /// <param name="lineLock">LineLock object that stores accessibility for buttons and values</param>
        /// <param name="name">The name of the line</param>
        /// <param name="color">The color of the line</param>
        /// <param name="pointAHandle">Coordinates for one of the handles of the line</param>
        /// <param name="pointBHandle">Coordinates for the other handle of the line</param>
        /// <param name="infinite">Whether or not the line is infinite</param>
        /// <param name="snapToGrid">Whether or not the line's handles snap to the grid</param>
        public Line(LineLock lineLock, string name, Color color, Vector2 pointAHandle, Vector2 pointBHandle, bool infinite, bool snapToGrid = true)
        {
            Name = name;
            Color = color;
            _pointAHandle = pointAHandle;
            _pointBHandle = pointBHandle;
            RecalculateFormulaFromHandles();
            RecalculateCoefficientsFromFormula();
            CalculateLength();
            Infinite = infinite;
            SnapToGrid = snapToGrid;
            Lock = lineLock ?? new LineLock();
        }

        /// <summary>
        /// Recalculates the formula values m and b (of y = mx + b) from the positions of the line's handles
        /// </summary>
        private void RecalculateFormulaFromHandles()
        {
            if (Mathf.Abs(_pointAHandle.x - _pointBHandle.x) <= Mathf.Epsilon)
            {
                return;
            }

            var m = 0f;
            var divisor = _pointBHandle.x - _pointAHandle.x;

            if (divisor < -Mathf.Epsilon || divisor > Mathf.Epsilon)
            {
                m = (_pointBHandle.y - _pointAHandle.y) / divisor;
            }

            var b = PointAHandle.y - m * PointAHandle.x;

            _formulaValues = new Vector2(m, b);
        }
        /// <summary>
        /// Recalculates the positions of the line's handles from the formula values m and b (of y = mx + b)
        /// </summary>
        private void RecalculateHandlesFromFormula()
        {
            _pointAHandle = new Vector2(_pointAHandle.x, Solve(_pointAHandle.x));
            _pointBHandle = new Vector2(_pointBHandle.x, Solve(_pointBHandle.x));
        }
        /// <summary>
        /// Recalculates the line's coefficients A, B and C from the formula values m and b (of y = mx + b)
        /// </summary>
        private void RecalculateCoefficientsFromFormula()
        {
            CoefficientA = _pointAHandle.y - _pointBHandle.y;
            CoefficientB = _pointBHandle.x - _pointAHandle.x;
            CoefficientC = (_pointAHandle.x * _pointBHandle.y) - (_pointBHandle.x * _pointAHandle.y);
        }
        /// <summary>
        /// Forces the line's handles onto the nearest grid position
        /// </summary>
        public void RasterizeHandles()
        {
            var rasterizedPointAHandle = _pointAHandle;
            rasterizedPointAHandle.x = Mathf.Round(rasterizedPointAHandle.x);
            rasterizedPointAHandle.y = Mathf.Round(rasterizedPointAHandle.y);
            PointAHandle = rasterizedPointAHandle;

            var rasterizedPointBHandle = _pointBHandle;
            rasterizedPointBHandle.x = Mathf.Round(rasterizedPointBHandle.x);
            rasterizedPointBHandle.y = Mathf.Round(rasterizedPointBHandle.y);
            PointBHandle = rasterizedPointBHandle;
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
        /// Solves the line's formula y = mx + b for x.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float Solve(float x)
        {
            var m = _formulaValues.x;
            var b = _formulaValues.y;

            return m * x + b;
        }

        public void CalculateLength()
        {
            var firstBracket = Mathf.Pow(PointBHandle.x - PointAHandle.x, 2);
            var secondBracket = Mathf.Pow(PointBHandle.y - PointAHandle.y, 2);

            Length = Mathf.Sqrt(firstBracket + secondBracket);
        }
    }
}
