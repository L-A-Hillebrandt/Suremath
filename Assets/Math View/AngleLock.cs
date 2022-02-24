namespace Math_View
{
    /// <summary>
    /// Object that stores information on every input on an AnglePanel, about whether it is interactable or not
    /// </summary>
    public class AngleLock
    {
        /// <summary>
        /// Whether or not the angle's name is locked
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
        /// Whether or not the color is locked
        /// </summary>
        public bool ColorLocked { get; }
        /// <summary>
        /// Whether or not the angle's value in degrees is locked
        /// </summary>
        public bool DegLocked { get; }
        /// <summary>
        /// Whether or not the angle's value in radians is locked
        /// </summary>
        public bool RadLocked { get; }
        /// <summary>
        /// Whether or not the angle's sine value is locked
        /// </summary>
        public bool SinLocked { get; }
        /// <summary>
        /// Whether or not the angle's cosine value is locked
        /// </summary>
        public bool CosLocked { get; }
        /// <summary>
        /// Whether or not the angle's tangens value is locked
        /// </summary>
        public bool TanLocked { get; }
        /// <summary>
        /// Whether or not the adjacent angle button is locked
        /// </summary>
        public bool AdjacentLocked { get; }

        /// <summary>
        /// Constructor for an AngleLock. Can be used with no parameters to lock everything.
        /// </summary>
        /// <param name="name">Whether or not the angle's name is locked</param>
        /// <param name="expand">Whether or not the expand button is locked</param>
        /// <param name="delete">Whether or not the delete button is locked</param>
        /// <param name="color">Whether or not the color is locked</param>
        /// <param name="deg">Whether or not the angle's value in degrees is locked</param>
        /// <param name="rad">Whether or not the angle's value in radians is locked</param>
        /// <param name="sin">Whether or not the angle's sine value is locked</param>
        /// <param name="cos">Whether or not the angle's cosine value is locked</param>
        /// <param name="tan">Whether or not the angle's tangens value is locked</param>
        /// <param name="adjacent">Whether or not the adjacent angle button is locked</param>
        public AngleLock(bool name = true, bool expand = true, bool delete = true, bool color = true, bool deg = true, bool rad = true,
            bool sin = true, bool cos = true, bool tan = true, bool adjacent = true)
        {
            NameLocked = name;
            ExpandLocked = expand;
            DeleteLocked = delete;
            ColorLocked = color;
            DegLocked = deg;
            RadLocked = rad;
            SinLocked = sin;
            CosLocked = cos;
            TanLocked = tan;
            AdjacentLocked = adjacent;
        }

        /// <summary>
        /// Constructor for an AngleLock.
        /// </summary>
        /// <param name="lockAll">Whether or not all inputs and values are locked</param>
        public AngleLock(bool lockAll)
        {
            NameLocked = lockAll;
            ExpandLocked = lockAll;
            DeleteLocked = lockAll;
            ColorLocked = lockAll;
            DegLocked = lockAll;
            RadLocked = lockAll;
            SinLocked = lockAll;
            CosLocked = lockAll;
            TanLocked = lockAll;
            AdjacentLocked = lockAll;
        }
    }
}
