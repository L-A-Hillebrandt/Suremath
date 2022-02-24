namespace Math_View
{
    /// <summary>
    /// Object that stores information on every input on an LinePanel, about whether it is interactable or not
    /// </summary>
    public class LineLock
    {
        /// <summary>
        /// Whether or not the line's name is locked
        /// </summary>
        public bool NameLocked { get; }
        /// <summary>
        /// Whether or not the line's expand button is locked
        /// </summary>
        public bool ExpandLocked { get;  }
        /// <summary>
        /// Whether or not the line's delete button is locked
        /// </summary>
        public bool DeleteLocked { get;  }
        /// <summary>
        /// Whether or not the line's color is locked
        /// </summary>
        public bool ColorLocked { get;  }
        /// <summary>
        /// Whether or not the line's formula is locked
        /// </summary>
        public bool FormulaLocked { get;  }
        /// <summary>
        /// Whether or not the line's points form is locked
        /// </summary>
        public bool PointsLocked { get;  }
        /// <summary>
        /// Whether or not the line's parameter form is locked
        /// </summary>
        public bool ParametersLocked { get;  }
        /// <summary>
        /// Whether or not the line's parameter m is locked
        /// </summary>
        public bool MLocked { get;  }
        /// <summary>
        /// Whether or not the line's parameter b is locked
        /// </summary>
        public bool BLocked { get;  }
        /// <summary>
        /// Whether or not the line's length is locked
        /// </summary>
        public bool LengthLocked { get;  }
        /// <summary>
        /// Whether or not the line's infinite toggle is locked
        /// </summary>
        public bool InfiniteLocked { get;  }
        /// <summary>
        /// Whether or not the line's snap to grid toggle is locked
        /// </summary>
        public bool SnapLocked { get;  }
        /// <summary>
        /// Whether or not the line's angle button is locked
        /// </summary>
        public bool AngleLocked { get; }
        /// <summary>
        /// Whether or not the line's handle A is locked
        /// </summary>
        public bool HandleALocked { get; }
        /// <summary>
        /// Whether or not the line's handle B is locked
        /// </summary>
        public bool HandleBLocked { get; }

        /// <summary>
        /// Constructor for a LineLock object. Can be used without parameters to lock everything.
        /// </summary>
        /// <param name="name">Whether or not the line's name is locked</param>
        /// <param name="expand">Whether or not the line's expand button is locked</param>
        /// <param name="delete">Whether or not the line's delete button is locked</param>
        /// <param name="color">Whether or not the line's color is locked</param>
        /// <param name="formula">Whether or not the line's formula is locked</param>
        /// <param name="points">Whether or not the line's points form is locked</param>
        /// <param name="parameters">Whether or not the line's parameter form is locked</param>
        /// <param name="m">Whether or not the line's parameter m is locked</param>
        /// <param name="b">Whether or not the line's parameter b is locked</param>
        /// <param name="length">Whether or not the line's length is locked</param>
        /// <param name="infinite">Whether or not the line's infinite toggle is locked</param>
        /// <param name="snap">Whether or not the line's snap to grid toggle is locked</param>
        /// <param name="angle">Whether or not the line's angle button is locked</param>
        /// <param name="handleA">Whether or not the line's handle A is locked</param>
        /// <param name="handleB">Whether or not the line's handle B is locked</param>
        public LineLock(bool name = true, bool expand = true, bool delete = true, bool color = true, bool formula = true,
            bool points = true, bool parameters = true, bool m = true, bool b = true, bool length = true, bool infinite = true,
            bool snap = true, bool angle = true, bool handleA = true, bool handleB = true)
        {
            NameLocked = name;
            ExpandLocked = expand;
            DeleteLocked = delete;
            ColorLocked = color;
            FormulaLocked = formula;
            PointsLocked = points;
            ParametersLocked = parameters;
            MLocked = m;
            BLocked = b;
            LengthLocked = length;
            InfiniteLocked = infinite;
            SnapLocked = snap;
            AngleLocked = angle;
            HandleALocked = handleA;
            HandleBLocked = handleB;
        }

        /// <summary>
        /// Constructor for a LineLock object.
        /// </summary>
        /// <param name="lockAll">Whether or not to lock everything</param>
        public LineLock(bool lockAll)
        {
            NameLocked = lockAll;
            ExpandLocked = lockAll;
            DeleteLocked = lockAll;
            ColorLocked = lockAll;
            FormulaLocked = lockAll;
            PointsLocked = lockAll;
            ParametersLocked = lockAll;
            MLocked = lockAll;
            BLocked = lockAll;
            LengthLocked = lockAll;
            InfiniteLocked = lockAll;
            SnapLocked = lockAll;
            AngleLocked = lockAll;
            HandleALocked = lockAll;
            HandleBLocked = lockAll;
        }
    }
}
