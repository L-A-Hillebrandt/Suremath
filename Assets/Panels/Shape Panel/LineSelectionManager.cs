using System.Collections.Generic;
using Math_View;
using UnityEngine;

namespace Panels.Shape_Panel
{
    /// <summary>
    /// An object that manages selecting two lines to construct an angle
    /// </summary>
    public class LineSelectionManager
    {
        /// <summary>
        /// The AnglePanel's parent
        /// </summary>
        private readonly AnglePanel _parent;
        /// <summary>
        /// A list of currently selected lines
        /// </summary>
        public List<Line> CurrentlySelectedLines { get; private set; }
        /// <summary>
        /// Whether or not the selection mode is active
        /// </summary>
        public bool AngleSelectionModeActive { get; set; }

        /// <summary>
        /// Constructs a new Line Selection Manager object
        /// </summary>
        /// <param name="parent">The Angle Panel the Line Selection Manager belongs to</param>
        public LineSelectionManager(AnglePanel parent)
        {
            CurrentlySelectedLines = new List<Line>();
            _parent = parent;
        }

        public void ClearSelection()
        {
            CurrentlySelectedLines = new List<Line>();
        }

        /// <summary>
        /// Adds a line to the selection if there are less than two lines already selected. If the new line is the second line, creates a new Angle object.
        /// </summary>
        /// <param name="line">The line to be added to the selection</param>
        /// <see cref="Angle"/>
        public void AddLine(Line line)
        {
            if (CurrentlySelectedLines.Count >= 2 || CurrentlySelectedLines.Contains(line)) return;
            CurrentlySelectedLines.Add(line);

            if (CurrentlySelectedLines.Count != 2) return;
            var angleLock = new AngleLock(false);
            _parent.Angle = new Angle(angleLock,CurrentlySelectedLines[0], CurrentlySelectedLines[1], "Neuer Winkel", Color.black);
        }
    }
}
