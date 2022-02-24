using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Dialogs
{
    /// <summary>
    /// Utility class for storing config data
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The local path where exercise files are stored
        /// </summary>
        public string LocalExercisePath { get; }
        /// <summary>
        /// The URL where the exercise list can be accessed on the web
        /// </summary>
        public string ExerciseListUrl { get; }
        /// <summary>
        /// The base URL where a single exercise can be accessed on the web. Append a slash and an ID to fetch one.
        /// </summary>
        public string ExerciseUrl { get; }

        /// <summary>
        /// Constructor for a Config object
        /// </summary>
        /// <param name="localExercisePath">The local path where exercise files are stored</param>
        /// <param name="exerciseListUrl">The URL where the exercise list can be accessed on the web</param>
        /// <param name="exerciseUrl">The base URL where a single exercise can be accessed on the web. Append a slash and an ID to fetch one.</param>
        public Config(string localExercisePath, string exerciseListUrl, string exerciseUrl)
        {
            LocalExercisePath = localExercisePath;
            ExerciseListUrl = exerciseListUrl;
            ExerciseUrl = exerciseUrl;
        }
    }
}
