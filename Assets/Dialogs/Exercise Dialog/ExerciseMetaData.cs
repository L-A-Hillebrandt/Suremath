using System;

namespace Dialogs.Exercise_Dialog
{
    /// <summary>
    /// Utility class for storing exercise meta data, used in deserializing data from JSON out of server responses.
    /// </summary>
    [Serializable]
    public class ExerciseMetaData
    {
        /// <summary>
        /// The ID of the exercise
        /// </summary>
        public int Exercise_Id;
        /// <summary>
        /// The title of the exercise
        /// </summary>
        public string Title;
        /// <summary>
        /// The faculty the exercise originated from
        /// </summary>
        public string Faculty;
        /// <summary>
        /// The author of the exercise
        /// </summary>
        public string Author;
    }
}
