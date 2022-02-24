namespace Math_View
{
    /// <summary>
    /// Object that stores information on every input on an ParabolaPanel, about whether it is interactable or not
    /// </summary>
    public class ParabolaLock
    {
        /// <summary>
        /// Whether or not the parabola's name is locked
        /// </summary>
        public bool NameLocked { get; }
        /// <summary>
        /// Whether or not the expand button is locked
        /// </summary>
        public bool ExpandLocked { get; }
        /// <summary>
        /// Whether or not the delete button is locked
        /// </summary>
        public bool DeleteLocked { get; }
        /// <summary>
        /// Whether or not the parabola's color is locked
        /// </summary>
        public bool ColorLocked { get; }
        /// <summary>
        /// Whether or not the parabola's standard form is locked
        /// </summary>
        public bool StandardLocked { get; }
        /// <summary>
        /// Whether or not the parabola's vertex form is locked
        /// </summary>
        public bool VertexLocked { get; }
        /// <summary>
        /// Whether or not the parabola's parameter form is locked
        /// </summary>
        public bool ParametersLocked { get; }
        /// <summary>
        /// Whether or not the parabola's points form is locked
        /// </summary>
        public bool PointsLocked { get; }
        /// <summary>
        /// Whether or not the parabola's parameter A is locked
        /// </summary>
        public bool ALocked { get; }
        /// <summary>
        /// Whether or not the parabola's parameter B is locked
        /// </summary>
        public bool BLocked { get; }
        /// <summary>
        /// Whether or not the parabola's parameter C is locked
        /// </summary>
        public bool CLocked { get; }
        /// <summary>
        /// Whether or not the parabola's infinite toggle is locked
        /// </summary>
        public bool InfiniteLocked { get; }
        /// <summary>
        /// Whether or not the parabola's snap to grid toggle is locked
        /// </summary>
        public bool SnapLocked { get; }
        /// <summary>
        /// Whether or not the parabola's origin handle is locked
        /// </summary>
        public bool OriginLocked { get; }
        /// <summary>
        /// Whether or not the parabola's curve handle is locked
        /// </summary>
        public bool CurveLocked { get; }

        /// <summary>
        /// Constructor for a ParabolaLock. Can be used without parameters to lock everything.
        /// </summary>
        /// <param name="name">Whether or not the parabola's name is locked</param>
        /// <param name="expand">Whether or not the expand button is locked</param>
        /// <param name="delete">Whether or not the delete button is locked</param>
        /// <param name="color">Whether or not the parabola's color is locked</param>
        /// <param name="standard">Whether or not the parabola's standard form is locked</param>
        /// <param name="vertex">Whether or not the parabola's vertex form is locked</param>
        /// <param name="parameters">Whether or not the parabola's parameter form is locked</param>
        /// <param name="points">Whether or not the parabola's points form is locked</param>
        /// <param name="a">Whether or not the parabola's parameter A is locked</param>
        /// <param name="b">Whether or not the parabola's parameter B is locked</param>
        /// <param name="c">Whether or not the parabola's parameter C is locked</param>
        /// <param name="infinite">Whether or not the parabola's infinite toggle is locked</param>
        /// <param name="snap">Whether or not the parabola's snap to grid toggle is locked</param>
        /// <param name="origin">Whether or not the parabola's origin handle is locked</param>
        /// <param name="curve">Whether or not the parabola's curve handle is locked</param>
        public ParabolaLock(bool name = true, bool expand = true, bool delete = true, bool color = true, bool standard = true,
            bool vertex = true, bool parameters = true, bool points = true, bool a = true, bool b = true, bool c = true, bool infinite = true,
            bool snap = true, bool origin = true, bool curve = true)
        {
            NameLocked = name;
            ExpandLocked = expand;
            DeleteLocked = delete;
            ColorLocked = color;
            StandardLocked = standard;
            VertexLocked = vertex;
            ParametersLocked = parameters;
            PointsLocked = points;
            ALocked = a;
            BLocked = b;
            CLocked = c;
            InfiniteLocked = infinite;
            SnapLocked = snap;
            OriginLocked = origin;
            CurveLocked = curve;
        }
        /// <summary>
        /// Constructor for a ParabolaLock object.
        /// </summary>
        /// <param name="lockAll">Whether or not to lock all inputs and displays.</param>
        public ParabolaLock(bool lockAll)
        {
            NameLocked = lockAll;
            ExpandLocked = lockAll;
            DeleteLocked = lockAll;
            ColorLocked = lockAll;
            StandardLocked = lockAll;
            VertexLocked = lockAll;
            ParametersLocked = lockAll;
            PointsLocked = lockAll;
            ALocked = lockAll;
            BLocked = lockAll;
            CLocked = lockAll;
            InfiniteLocked = lockAll;
            SnapLocked = lockAll;
            OriginLocked = lockAll;
            CurveLocked = lockAll;
        }
    }
}
