
using System;

namespace Dialogs.Exercise_Dialog
{
    /// <summary>
    /// A utilitarian container class for exercises, used for deserializing data from JSON from a server response.
    /// </summary>
    [Serializable]
    public class Exercise
    {
        /// <summary>
        /// The title of the exercise
        /// </summary>
        public string title;
        /// <summary>
        /// The author of the exercise
        /// </summary>
        public string author;
        /// <summary>
        /// The faculty the exercise originated from
        /// </summary>
        public string faculty;
        /// <summary>
        /// XML data for the exercise's prompts and shapes
        /// </summary>
        public string data;
    }
}