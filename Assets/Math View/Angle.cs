using UnityEngine;
using Random = UnityEngine.Random;

namespace Math_View
{
    /// <summary>
    /// Class to store data for an angle.
    /// </summary>
    public class Angle
    {
        /// <summary>
        /// The vertex of the angle, if it exists.
        /// </summary>
        private Vector2 _vertex;

        /// <summary>
        /// One of the lines that make up this angle.
        /// </summary>
        private Line _lineA;
        /// <summary>
        /// The other one of the lines that make up this angle.
        /// </summary>
        private Line _lineB;

        /// <summary>
        /// The value of the angle in rad
        /// </summary>
        private float _radAngle;
        /// <summary>
        /// The value of the angle in degrees
        /// </summary>
        private float _degAngle;

        /// <summary>
        /// The value of the angle in degrees
        /// </summary>
        public float AngleInDeg
        {
            get => _degAngle;
            private set => _degAngle = value;
        }

        /// <summary>
        /// The value of the angle in rad
        /// </summary>
        public float AngleInRad
        {
            get => _radAngle;
            private set => _radAngle = value;
        }

        /// <summary>
        /// The name of the angle
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not the two lines in the angle intersect
        /// </summary>
        public bool HasIntersect { get; private set; }

        /// <summary>
        /// Whether or not to show the adjacent angle instead of the first found, clockwise
        /// </summary>
        public bool ShowAdjacentAngle { get; private set; }

        /// <summary>
        /// Whether or not to use radians when displaying the value of the angle
        /// </summary>
        public bool UseRadians { get; private set; }

        public AngleLock Lock { get; }

        /// <summary>
        /// Constructs a new angle object and performs all necessary initial calculations for it.
        /// </summary>
        /// <param name="angleLock">AngleLock object that specifies what inputs and fields are locked/visible</param>
        /// <param name="lineA">Line A in the angle</param>
        /// <param name="lineB">Line B in the angle</param>
        /// <param name="name">The name of the angle</param>
        /// <param name="color">The color of the angle</param>
        public Angle(AngleLock angleLock, Line lineA, Line lineB, string name, Color color)
        {
            Lock = angleLock;
            _lineA = lineA;
            _lineB = lineB;
            Name = name;
            Color = color;
            CalculateAngle();
            CalculateVertex();
            //RandomizeColor();
        }

        /// <summary>
        /// Calculates the radians and degree values of the angle
        /// </summary>
        public void CalculateAngle()
        {
            var radians = Mathf.Atan(Mathf.Abs((_lineA.FormulaValues.x - _lineB.FormulaValues.x) /
                                               (1f + (_lineA.FormulaValues.x * _lineB.FormulaValues.x))));

            if (ShowAdjacentAngle)
            {
                _radAngle = Mathf.PI - radians;
                _degAngle = 180f - radians * (180f / Mathf.PI);
            }
            else
            {
                _radAngle = radians;
                _degAngle = radians * (180f / Mathf.PI);
            }
        }

        /// <summary>
        /// Calculates the vertex of the angle, if there is one, and updates the Angle object's properties accordingly.
        /// </summary>
        public void CalculateVertex()
        {
            float mainDeterminant =
                (_lineA.CoefficientA * _lineB.CoefficientB) - (_lineA.CoefficientB * _lineB.CoefficientA);

            float determinantX = (-_lineA.CoefficientC * _lineB.CoefficientB) -
                                 (_lineA.CoefficientB * (-_lineB.CoefficientC));

            float determinantY = (_lineA.CoefficientA * (-_lineB.CoefficientC)) -
                                 ((-_lineA.CoefficientC) * _lineB.CoefficientA);
        

            if (mainDeterminant != 0)
            {
                float xCoordinate = determinantX / mainDeterminant;
                float yCoordinate = determinantY / mainDeterminant;

                Vector2 result = new Vector2(xCoordinate, yCoordinate);
                _vertex = result;
                HasIntersect = true;
            }
            else
            {
                HasIntersect = false;
            }
        }

        /// <summary>
        /// Toggles whether or not to show the adjacent angle to the one first found in clockwise order.
        /// </summary>
        public void ToggleAdjacentAngle()
        {
            if (ShowAdjacentAngle)
            {
                ShowAdjacentAngle = false;
            }
            else
            {
                ShowAdjacentAngle = true;
            }
        }
        /// <summary>
        /// Toggles whether or not to use radians
        /// </summary>
        public void ToggleRadians()
        {
            if (UseRadians)
            {
                UseRadians = false;
            }
            else
            {
                UseRadians = true;
            }
        }
        /// <summary>
        /// One of the lines that make up this angle.
        /// </summary>
        public Line LineA
        {
            get => _lineA;

            private set => _lineA = value;
        }
        /// <summary>
        /// The other one of the lines that make up this angle.
        /// </summary>
        public Line LineB
        {
            get => _lineB;

            private set => _lineB = value;
        }
        /// <summary>
        /// Coordinates of where the lines that make up this angle intersect, if they do.
        /// </summary>
        public Vector2 Intersection
        {
            get => _vertex;
            private set => _vertex = value;
        }
        /// <summary>
        /// The color this angle is rendered in.
        /// </summary>
        public Color Color { get; private set; }

        public void RandomizeColor()
        {
            Color color = new Color();

            int cycle = Random.Range(0, 5);

            if (cycle == 0)
            {
                color.r = Random.Range(0f, 1f);
                color.g = 0f;
                color.b = 1f;
            }
            else if (cycle == 1)
            {
                color.r = Random.Range(0f, 1f);
                color.g = 1f;
                color.b = 0f;
            }
            else if (cycle == 2)
            {
                color.r = 0f;
                color.g = Random.Range(0f, 1f);
                color.b = 1f;
            }
            else if (cycle == 3)
            {
                color.r = 1f;
                color.g = Random.Range(0f, 1f);
                color.b = 0f;
            }
            else if (cycle == 4)
            {
                color.r = 0f;
                color.g = 1f;
                color.b = Random.Range(0f, 1f);
            }
            else
            {
                color.r = 1f;
                color.g = 0f;
                color.b = Random.Range(0f, 1f);
            }

            color.a = 1f;
            Color = color;
        }
    }
}