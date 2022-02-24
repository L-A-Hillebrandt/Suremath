using System;

namespace Dialogs.Exercise_Dialog
{
    /// <summary>
    /// Utility class for storing a list of ExerciseMetaData, used in deserializing from JSON out of a server response.
    /// </summary>
    [Serializable]
    public class ExerciseList
    {
        public ExerciseMetaData[] list;
    }
}