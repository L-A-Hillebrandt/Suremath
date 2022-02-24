using System;
using System.Collections.Generic;
using Panels.Shape_Panel;
using UnityEngine;
using UnityEngine.UI;

namespace Math_View
{
    /// <summary>
    /// Renders the contents of the math view
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class MathViewRenderer : MonoBehaviour
    {
        /// <summary>
        /// Zoom factor of the camera
        /// </summary>
        public float CameraZoom { get; set; }
        /// <summary>
        /// Position of the camera in 2D space
        /// </summary>
        public Vector2 CameraPosition { get; set; }

        /// <summary>
        /// Material to render shapes in
        /// </summary>
        [SerializeField, Tooltip("Material to render shapes in")]
        private Material renderMaterial;

        /// <summary>
        /// Shape panel container where shape panels are stored
        /// </summary>
        [SerializeField, Tooltip("Shape panel container where shape panels are stored")]
        private Transform panelContainer;

        /// <summary>
        /// The color of the background
        /// </summary>
        [SerializeField, Tooltip("The color of the background")]
        private Color backgroundColor;

        /// <summary>
        /// The color of grid lines
        /// </summary>
        [SerializeField, Tooltip("The color of grid lines")]
        private Color gridLineColor;

        /// <summary>
        /// The color of the axes of the coordinate system
        /// </summary>
        [SerializeField, Tooltip("The color of the axes of the coordinate system")]
        private Color axisColor;

        /// <summary>
        /// Font for rendering text
        /// </summary>
        [SerializeField, Tooltip("Font for rendering text")]
        private Texture2D fontSprite;

        /// <summary>
        /// Font size
        /// </summary>
        [SerializeField, Tooltip("Font size")]
        private Vector2 fontGlyphSize;

        /// <summary>
        /// Render target for shapes
        /// </summary>
        private RenderTexture _renderTexture;

        /// <summary>
        /// Utility object for when an angle is being newly set up.
        /// </summary>
        private LineSelectionManager _lineSelectionManager;

        /// <summary>
        /// Calculates the modulo of values a and b. The native C# modulo operator does not do this correctly.
        /// </summary>
        /// <param name="a">Value a</param>
        /// <param name="b">Value b</param>
        /// <returns>a mod b</returns>
        public static int Mod(int a, int b)
        {
            return ((a % b) + b) % b;
        }
	
        void Start()
        {
            CameraZoom = 10f;
            CameraPosition = new Vector2(0f, 0f);

            RawImage rawImage = GetComponent<RawImage>();
            _renderTexture = (RenderTexture)rawImage.texture;
        }

        void OnRenderObject()
        {
            Graphics.SetRenderTarget(_renderTexture);

            renderMaterial.SetPass(0);

            GL.Clear(false, true, backgroundColor);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.LoadPixelMatrix(0f, 1f, 0f, 1f);

            RenderGrid();

            AnglePanel[] anglePanels = panelContainer.GetComponentsInChildren<AnglePanel>();
            foreach (var panel in anglePanels)
            {
                if (panel.SelectionModeActive)
                {
                    _lineSelectionManager = panel.LineSelectionManager;
                }

                if (panel.Angle != null && panel.Angle.HasIntersect)
                {
                    RenderAngle(panel.Angle, 100);
                }
            }

            LinePanel[] linePanels = panelContainer.GetComponentsInChildren<LinePanel>();
            foreach (var linePanel in linePanels)
            {
                RenderLine(linePanel.Line);
            }

            ParabolaPanel[] parabolaPanels = panelContainer.GetComponentsInChildren<ParabolaPanel>();
            foreach (var parabolaPanel in parabolaPanels)
            {
                RenderParabola(parabolaPanel.Parabola, 100);
            }

            GL.PopMatrix();
            Graphics.SetRenderTarget(null);
        }

        private Vector2 TransformPoint(Vector2 point)
        {
            return (point + CameraPosition) / CameraZoom + new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// Renders a point at the specified coordinates.
        /// </summary>
        /// <param name="point">The coordinates where the point is rendered</param>
        /// <param name="outlineColor">The outline color of the point</param>
        /// <param name="fillColor">The fill color of the point</param>
        /// <param name="radius">The radius of the point</param>
        void RenderPoint(Vector2 point, Color outlineColor, Color fillColor, float radius = -1f)
        {
            if (Mathf.Abs(radius) <= Mathf.Epsilon)
                return;
		
            if (radius < 0f)
                radius = CameraZoom / 20f;

            point = TransformPoint(point);

            GL.Begin(GL.QUADS);

            GL.Color(fillColor);

            GL.Vertex3(point.x - radius, point.y, 0f);
            GL.Vertex3(point.x, point.y - radius, 0f);

            GL.Vertex3(point.x, point.y - radius, 0f);
            GL.Vertex3(point.x + radius, point.y, 0f);

            GL.Vertex3(point.x + radius, point.y, 0f);
            GL.Vertex3(point.x, point.y + radius, 0f);

            GL.Vertex3(point.x, point.y + radius, 0f);
            GL.Vertex3(point.x - radius, point.y, 0f);

            GL.End();

            GL.Begin(GL.LINES);

            GL.Color(outlineColor);

            GL.Vertex3(point.x - radius,	point.y,			0f);
            GL.Vertex3(point.x,				point.y - radius,	0f);

            GL.Vertex3(point.x,				point.y - radius,	0f);
            GL.Vertex3(point.x + radius,	point.y,			0f);

            GL.Vertex3(point.x + radius,	point.y,			0f);
            GL.Vertex3(point.x,				point.y + radius,	0f);

            GL.Vertex3(point.x,				point.y + radius,	0f);
            GL.Vertex3(point.x - radius,	point.y,			0f);

            GL.End();
        }

        /// <summary>
        /// Renders a line between two points
        /// </summary>
        /// <param name="from">The start coordinates of the line</param>
        /// <param name="to">The end coordinates of the line</param>
        /// <param name="color">The color of the line</param>
        void RenderLine(Vector2 from, Vector2 to, Color color)
        {
            from = TransformPoint(from);
            to = TransformPoint(to);

            GL.Begin(GL.LINES);

            GL.Color(color);

            GL.Vertex3(from.x, from.y, 0f);
            GL.Vertex3(to.x, to.y, 0f);

            GL.End();
        }

        /// <summary>
        /// Renders a thick line between two points with a specified width
        /// </summary>
        /// <seealso cref="RenderLine(UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.Color)"/>
        /// <param name="from">The start coordinates of the line</param>
        /// <param name="to">The end coordinates of the line</param>
        /// <param name="color">The color of the line</param>
        /// <param name="width">The width of the line</param>
        private void RenderThickLine(Vector2 from, Vector2 to, float width, Color color)
        {
            var v = to - from;
            v.Normalize();
            v *= width;
		
            var w_x = -v.y / 100f;
            var w_y = v.x / 100f;

            from = TransformPoint(from);
            to = TransformPoint(to);

            GL.Begin(GL.QUADS);

            GL.Color(color);

            GL.Vertex3(from.x + w_x, from.y + w_y, 0f);
            GL.Vertex3(from.x - w_x, from.y - w_y, 0f);
            GL.Vertex3(to.x - w_x, to.y - w_y, 0f);
            GL.Vertex3(to.x + w_x, to.y + w_y, 0f);

            GL.End();
        }

        /// <summary>
        /// Renders text in a specified location
        /// </summary>
        /// <param name="text">The text to be rendered</param>
        /// <param name="position">The coordinates where the text is rendered</param>
        /// <param name="color">The color of the text</param>
        public void RenderText(string text, Vector2 position, Color color)
        {
            var numGlyphs = fontSprite.width / fontGlyphSize.x;
            var scale = 64f;

            for (var i = 0; i < text.Length; ++i)
            {
                var index = 0;

                if (text[i] > '0' && text[i] <= '9')
                    index = text[i] - '0';
                else if (text[i] == '+')
                    index = 10;
                else if (text[i] == '-')
                    index = 11;
                else if (text[i] == ',')
                    index = 12;
                else if (text[i] == '.')
                    index = 13;
                else if (text[i] == ':')
                    index = 14;
                else if (text[i] == '%')
                    index = 15;

                var sourceRect = new Rect(index / numGlyphs, 1f, fontGlyphSize.x / fontSprite.width, -1f);
                var destinationRect = new Rect(position.x + i * (fontGlyphSize.x / scale), position.y, fontGlyphSize.x / scale, fontGlyphSize.y / scale);
                Graphics.DrawTexture(destinationRect, fontSprite, sourceRect, 0, 0, 0, 0, color);
            }
        }

        /// <summary>
        /// Renders a rectangular grid with x and y axes
        /// </summary>
        private void RenderGrid()
        {
            // render grid lines
            for (var x = -1f; x >= -100f; x--)
                RenderLine(new Vector2(x, -100f), new Vector2(x, 100f), gridLineColor);
            for (var x = 1f; x <= 100f; x++)
                RenderLine(new Vector2(x, -100f), new Vector2(x, 100f), gridLineColor);
            for (var y = -1f; y >= -100f; y--)
                RenderLine(new Vector2(-100f, y), new Vector2(100f, y), gridLineColor);
            for (var y = 1f; y <= 100f; y++)
                RenderLine(new Vector2(-100f, y), new Vector2(100f, y), gridLineColor);

            // render axes
            RenderThickLine(new Vector2(-100f, 0f), new Vector2(100f, 0f), 0.35f, axisColor); // x axis
            RenderThickLine(new Vector2(0f, -100f), new Vector2(0f, 100f), 0.35f, axisColor); // y axis
            //RenderLine(new Vector2(-100f, 0f), new Vector2(100f, 0f), axisColor); // x axis
            //RenderLine(new Vector2(0f, -100f), new Vector2(0f, 100f), axisColor); // y axis

            //RenderText("1", new Vector2(1.0f - 0.01f, -1.0f), Color.white);
        }

        /// <summary>
        /// Renders a given line
        /// </summary>
        /// <param name="line">The line to be rendered</param>
        private void RenderLine(Line line)
        {
            var fromX = -CameraZoom / 2f - CameraPosition.x;
            var toX = CameraZoom / 2f - CameraPosition.x;

            if (!line.Infinite)
            {
                fromX = line.PointAHandle.x;
                toX = line.PointBHandle.x;
            }

            if (Mathf.Abs(line.PointAHandle.x - line.PointBHandle.x) <= Mathf.Epsilon)
            {
                var fromY = -CameraZoom / 2f - CameraPosition.y;
                var toY = CameraZoom / 2f - CameraPosition.y;
                var fromToX = line.PointAHandle.x;

                if (!line.Infinite)
                {
                    fromY = line.PointAHandle.y;
                    toY = line.PointBHandle.y;
                }

                if (_lineSelectionManager != null && _lineSelectionManager.CurrentlySelectedLines.Contains(line) && _lineSelectionManager.AngleSelectionModeActive)
                {
                    RenderThickLine(new Vector2(fromToX, fromY), new Vector2(fromToX, toY), 0.75f, Color.white);
                }

                RenderThickLine(new Vector2(fromToX, fromY), new Vector2(fromToX, toY), 0.5f, line.Color);
            }
            else
            {
                if (_lineSelectionManager != null && _lineSelectionManager.CurrentlySelectedLines.Contains(line) && _lineSelectionManager.AngleSelectionModeActive)
                {
                    RenderThickLine(new Vector2(fromX, line.Solve(fromX)), new Vector2(toX, line.Solve(toX)), 0.75f, Color.white);
                }

                RenderThickLine(new Vector2(fromX, line.Solve(fromX)), new Vector2(toX, line.Solve(toX)), 0.5f, line.Color);
            }

            RenderPoint(line.PointAHandle, Color.black, Color.white, 0.02f);
            RenderPoint(line.PointAHandle, Color.black, line.Color, 0.01f);

            RenderPoint(line.PointBHandle, Color.black, Color.white, 0.02f);
            RenderPoint(line.PointBHandle, Color.black, line.Color, 0.01f);
        }

        /// <summary>
        /// Renders a parabola
        /// </summary>
        /// <param name="parabola">The parabola to be rendered</param>
        /// <param name="resolution">The resolution in which to render the parabola</param>
        private void RenderParabola(Parabola parabola, int resolution)
        {
            if (resolution <= 0)
            {
                return;
            }

            float x1 = Mathf.Min(parabola.OriginHandle.x, parabola.CurveHandle.x), x2 = Mathf.Max(parabola.OriginHandle.x, parabola.CurveHandle.x);

            if (Mathf.Abs(x1 - x2) <= 0.01f)
            {
                x1 -= 0.01f;
                x2 += 0.01f;
            }

            if (parabola.Infinite)
            {
                if (parabola.OriginHandle.y < parabola.CurveHandle.y)
                    // if the origin handle is below the curve handle the parabola opens up towards the top
                {
                    List<float> intersects = parabola.Intersect(CameraZoom / 2f - CameraPosition.y);

                    if (intersects == null)
                    {
                        return;
                    }

                    if (intersects.Count != 2)
                    {
                        return;
                    }

                    x1 = intersects[0];
                    x2 = intersects[1];
                }
                else if (parabola.OriginHandle.y > parabola.CurveHandle.y)
                {
                    List<float> intersects = parabola.Intersect(-CameraZoom / 2f - CameraPosition.y);

                    if (intersects == null)
                    {
                        return;
                    }

                    if (intersects.Count != 2)
                    {
                        return;
                    }

                    x1 = intersects[1];
                    x2 = intersects[0];
                }
            }

            var spanWidth = x2 - x1;
            var stepWidth = spanWidth / resolution;

            for (var x = x1; x < x1 + spanWidth; x += stepWidth)
            {
                var from = new Vector2(x, parabola.Solve(x));
                    
                var to = new Vector2(x + stepWidth, parabola.Solve(x + stepWidth));

                RenderThickLine(from, to, 0.5f, parabola.Color);
            }

            RenderPoint(parabola.OriginHandle, Color.black, Color.white, 0.02f);
            RenderPoint(parabola.OriginHandle, Color.black, parabola.Color, 0.01f);

            RenderPoint(parabola.CurveHandle, Color.black, Color.white, 0.02f);
            RenderPoint(parabola.CurveHandle, Color.black, parabola.Color, 0.01f);

            //RenderPoint(parabola.CurveHandleCopy, Color.black, Color.white, 0.02f);
            //RenderPoint(parabola.CurveHandleCopy, Color.black, parabola.Color, 0.01f);
        }

        /// <summary>
        /// Renders an angle
        /// </summary>
        /// <param name="angle">The angle to be rendered</param>
        /// <param name="resolution">The resolution in which to render the angle</param>
        /// <param name="radius">The radius of the angle</param>
        private void RenderAngle(Angle angle, int resolution, float radius = 1.0f)
        {
            //Step 1: Find intersections between circle and both lines
            var intersects = new Vector2[4];
            var circleCenter = angle.Intersection;

            //Intersection calculation according to https://mathworld.wolfram.com/Circle-LineIntersection.html

            //Intersections for line 1

            var pointA1 = angle.LineA.PointAHandle - circleCenter;
            var pointB1 = angle.LineA.PointBHandle - circleCenter;

            var dx1 = pointB1.x - pointA1.x;
            var dy1 = pointB1.y - pointA1.y;
            var dr1 = Mathf.Sqrt(Mathf.Pow(dx1, 2) + Mathf.Pow(dy1, 2));

            var bigD1 = (pointA1.x * pointB1.y) - (pointB1.x * pointA1.y);

            float sgn_dy1;
            if (dy1 < 0)
            {
                sgn_dy1 = -1;
            }
            else
            {
                sgn_dy1 = 1;
            }

            var x1a = (bigD1 * dy1 + sgn_dy1 * dx1 * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr1, 2) - Mathf.Pow(bigD1, 2))) /
                        Mathf.Pow(dr1, 2);

            var y1a = (-bigD1 * dx1 + Mathf.Abs(dy1) * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr1, 2) - Mathf.Pow(bigD1, 2))) / Mathf.Pow(dr1, 2);

            intersects[0] = new Vector2(x1a, y1a) + circleCenter;

            var x2a = (bigD1 * dy1 - sgn_dy1 * dx1 * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr1, 2) - Mathf.Pow(bigD1, 2))) /
                        Mathf.Pow(dr1, 2);

            var y2a = (-bigD1 * dx1 - Mathf.Abs(dy1) * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr1, 2) - Mathf.Pow(bigD1, 2))) / Mathf.Pow(dr1, 2);

            intersects[2] = new Vector2(x2a, y2a) + circleCenter;

            //Intersections for line 2

            var pointA2 = angle.LineB.PointAHandle - circleCenter;
            var pointB2 = angle.LineB.PointBHandle - circleCenter;

            var dx2 = pointB2.x - pointA2.x;
            var dy2 = pointB2.y - pointA2.y;
            var dr2 = Mathf.Sqrt(Mathf.Pow(dx2, 2) + Mathf.Pow(dy2, 2));

            var bigD2 = (pointA2.x * pointB2.y) - (pointB2.x * pointA2.y);

            float sgn_dy2;
            if (dy2 < 0)
            {
                sgn_dy2 = -1;
            }
            else
            {
                sgn_dy2 = 1;
            }

            var x1b = (bigD2 * dy2 + sgn_dy2 * dx2 * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr2, 2) - Mathf.Pow(bigD2, 2))) /
                        Mathf.Pow(dr2, 2);

            var y1b = (-bigD2 * dx2 + Mathf.Abs(dy2) * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr2, 2) - Mathf.Pow(bigD2, 2))) / Mathf.Pow(dr2, 2);

            intersects[1] = new Vector2(x1b, y1b) + circleCenter;

            var x2b = (bigD2 * dy2 - sgn_dy2 * dx2 * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr2, 2) - Mathf.Pow(bigD2, 2))) /
                        Mathf.Pow(dr2, 2);

            var y2b = (-bigD2 * dx2 - Mathf.Abs(dy2) * Mathf.Sqrt(Mathf.Pow(radius, 2) * Mathf.Pow(dr2, 2) - Mathf.Pow(bigD2, 2))) / Mathf.Pow(dr2, 2);

            intersects[3] = new Vector2(x2b, y2b) + circleCenter;

            //Step 2: Render line on pitch circle

            List<Vector2> quadrant1 = new List<Vector2>();
            List<Vector2> quadrant2 = new List<Vector2>();
            List<Vector2> quadrant3 = new List<Vector2>();
            List<Vector2> quadrant4 = new List<Vector2>();

            //Separate intersections into quadrants 1-4, starting in the top right and going clockwise

            foreach (var intersection in intersects)
            {
                if (intersection.x > circleCenter.x && intersection.y >= circleCenter.y)
                {
                    quadrant1.Add(intersection);
                }
                else if (intersection.x <= circleCenter.x && intersection.y > circleCenter.y)
                {
                    quadrant4.Add(intersection);
                }
                else if (intersection.x >= circleCenter.x && intersection.y < circleCenter.y)
                {
                    quadrant2.Add(intersection);
                }
                else
                {
                    quadrant3.Add(intersection);
                }
            }

            quadrant1.Sort((Vector2 a, Vector2 b) => b.x.CompareTo(a.x));
            quadrant2.Sort((Vector2 a, Vector2 b) => a.x.CompareTo(b.x));
            quadrant3.Sort((Vector2 a, Vector2 b) => a.x.CompareTo(b.x));
            quadrant4.Sort((Vector2 a, Vector2 b) => b.x.CompareTo(a.x));

            var quadrants = new List<Vector2>[4];
            quadrants[0] = quadrant1;
            quadrants[1] = quadrant2;
            quadrants[2] = quadrant3;
            quadrants[3] = quadrant4;

            var start = new Vector2();
            var startFound = false;

            //Find first quadrant with two intersections, failing that, take the next one with only one.
            for(var i = 0; i < 4; i++)
            {

                if (quadrants[i].Count == 2)
                {
                    if (angle.ShowAdjacentAngle) //Angle shown is obtuse
                    {
                        start = quadrants[i][1];
                        startFound = true;
                        break;
                    }

                    //Angle shown is acute
                    start = quadrants[i][0];
                    startFound = true;
                    break;
                }

                if (quadrants[i].Count == 1)
                {
                    int leftIndex = Mod(i - 1, 4);
                    for(int j = 1; j < 3; j++)
                    {
                        if(quadrants[leftIndex].Count > 0)
                        {
                            break;
                        }
                        leftIndex = Mod(i - j, 4);
                    }
                    
                    var rightIndex = Mod(i + 1, 4);
                    for (int j = 1; j < 3; j++)
                    {
                        if (quadrants[rightIndex].Count > 0)
                        {
                            break;
                        }
                        leftIndex = Mod(i - j, 4);
                    }

                    var distanceLeft = Vector2.Distance(quadrants[i][0], quadrants[leftIndex][0]);
                    var distanceRight = Vector2.Distance(quadrants[i][0], quadrants[rightIndex][0]);

                    if (angle.ShowAdjacentAngle) //Angle shown is obtuse
                    {
                        if (distanceLeft > distanceRight)
                        {
                            start = quadrants[i][0];
                            startFound = true;
                            break;
                        }

                        if(distanceLeft < distanceRight)
                        {
                            start = quadrants[rightIndex][0];
                            startFound = true;
                            break;
                        }
                    
                        start = quadrants[i][0];
                        startFound = true;
                        break;
                    
                    }

                    //Angle shown is acute
                    if (distanceLeft > distanceRight)
                    {
                        start = quadrants[rightIndex][0];
                        startFound = true;
                        break;
                    }

                    if (distanceLeft < distanceRight)
                    {
                        start = quadrants[i][0];
                        startFound = true;
                        break;
                    }

                    start = quadrants[i][0];
                    startFound = true;
                    break;
                }
            }

            var circleCenterNormalizedIntersects = new Vector2[4];
            Array.Copy(intersects, circleCenterNormalizedIntersects, 4);

            for (int i = 0; i < circleCenterNormalizedIntersects.Length; i++)
            {
                circleCenterNormalizedIntersects[i] -= circleCenter;
            }


            if (startFound)
            {
                var startNormalized = start - circleCenter;

                var expandedRadOfStart = 0.0f;

                if (quadrant1.Contains(start))
                {
                    expandedRadOfStart = Mathf.Atan(startNormalized.y / startNormalized.x);
                }
                else if (quadrant2.Contains(start))
                {
                    expandedRadOfStart = 1.5f * Mathf.PI + (0.5f * Mathf.PI - Mathf.Abs(Mathf.Atan(startNormalized.y / startNormalized.x)));
                }
                else if (quadrant3.Contains(start))
                {
                    expandedRadOfStart = Mathf.PI + Mathf.Atan(startNormalized.y / startNormalized.x);
                }
                else if (quadrant4.Contains(start))
                {
                    expandedRadOfStart = 0.5f * Mathf.PI + Mathf.Atan(startNormalized.y / startNormalized.x);
                }

                var from = start;

                if (expandedRadOfStart + angle.AngleInRad > 2.0f * Mathf.PI)
                {
                    var remainingAngle = expandedRadOfStart + angle.AngleInRad - 2.0f * Mathf.PI;

                    for (var theta = expandedRadOfStart; theta < (2.0f * Mathf.PI); theta += 0.01f)
                    {

                        var to = new Vector2(Mathf.Cos(theta) * radius + circleCenter.x, Mathf.Sin(theta) * radius + circleCenter.y);

                        RenderThickLine(from, to, 0.5f, angle.Color);

                        from = to;
                    }

                    from = new Vector2(circleCenter.x + radius, circleCenter.y);

                    for (var theta = 0.0f; theta < remainingAngle; theta += 0.01f)
                    {

                        var to = new Vector2(Mathf.Cos(theta) * radius + circleCenter.x, Mathf.Sin(theta) * radius + circleCenter.y);

                        RenderThickLine(from, to, 0.5f, angle.Color);

                        from = to;
                    }
                }
                else
                {
                    for (var theta = expandedRadOfStart; theta < expandedRadOfStart + angle.AngleInRad; theta += 0.01f)
                    {

                        var to = new Vector2(Mathf.Cos(theta) * radius + circleCenter.x, Mathf.Sin(theta) * radius + circleCenter.y);

                        RenderThickLine(from, to, 0.5f, angle.Color);

                        from = to;
                    }
                }
            }
        }
    }
}
