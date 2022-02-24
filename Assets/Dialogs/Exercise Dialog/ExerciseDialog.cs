using Dialogs.Error_Dialog;
using Panels.Exercise_Browser_Panel;
using Panels.Exercise_Prompt_Panel;
using Panels.Shape_Panel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Assets.Dialogs;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Dialogs.Exercise_Dialog
{
    public class ExerciseDialog : MonoBehaviour
    {
        /// <summary>
        /// The viewport of the dialog, where available exercises are listed
        /// </summary>
        [SerializeField, Tooltip("The viewport of the dialog, where available exercises are listed")]
        private GameObject viewport;
        /// <summary>
        /// Prefab for list items to display exercises
        /// </summary>
        [SerializeField, Tooltip("Prefab for list items to display exercises")]
        private GameObject exerciseBrowserListItemPrefab;
        /// <summary>
        /// Prefab for the error dialog
        /// </summary>
        [SerializeField, Tooltip("Prefab for the error dialog")]
        private GameObject errorDialogPrefab;
        
        /// <summary>
        /// Container for dialogs
        /// </summary>
        private GameObject _dialogContainer;
        /// <summary>
        /// The title text of an exercise that doubles as a button to open this dialog.
        /// </summary>
        private Text _exerciseTitleText;

        private Transform _internetTab;

        /// <summary>
        /// Container for shape panels, needs to be given to the parser to create new panels
        /// </summary>
        private ShapePanelContainer _shapePanelContainer;

        /// <summary>
        /// Container for exercise panels, needs to be given to the parser to create new panels
        /// </summary>
        private ExercisePanelContainer _exercisePanelContainer;

        /// <summary>
        /// Web request for refreshing the exercise list from the internet
        /// </summary>
        private UnityWebRequest _refreshListRequest;
        /// <summary>
        /// Web request for loading a single exercise from the internet
        /// </summary>
        private UnityWebRequest _loadExerciseRequest;
        
        /// <summary>
        /// Whether or not exercises are to be loaded from the internet
        /// </summary>
        private bool _fromInternet;

        private const string ExerciseSchemaFileName = "exercise-schema.xsd";

        /// <summary>
        /// XSD schema reader for the schema to validate exercise files against
        /// </summary>
        private XmlReader _exerciseSchemaReader;

        /// <summary>
        /// Config object that stores information on where exercise files are stored or can be accessed on the web
        /// </summary>
        private Config _config;

        private ListRequestState _listRequestState;
        private ExerciseRequestState _exerciseRequestState;

        /// <summary>
        /// The state of the list request.
        /// </summary>
        private enum ListRequestState
        {
            NotStarted,
            Pending,
            Done,
            Checking,
            Parsing,
            Error,
        };

        /// <summary>
        /// The state of the exercise request.
        /// </summary>
        private enum ExerciseRequestState
        {
            NotStarted,
            Pending,
            Done,
            Checking,
            Parsing,
            Error,
        };

        private struct ParseListJob : IJob
        {
            public ExerciseDialog Dialog;

            public List<ExerciseMetaData> List;

            public GameObject ListItemPrefab, ViewPort;
            public void Execute()
            {
                if (List == null)
                {
                    return;
                }
                ExerciseParser.ParseList(List, ListItemPrefab, ViewPort, Dialog);
            }
        }

        private void Start()
        {
            _listRequestState = ListRequestState.NotStarted;
            _exerciseRequestState = ExerciseRequestState.NotStarted;
            _dialogContainer = GameObject.Find("Dialog Container");
            
            var localPath = Application.dataPath;

            _config = ConfigManager.ReadConfig();

            if (!File.Exists(localPath + "/" + ExerciseSchemaFileName))
            {
                Debug.Log("Schema file not found! Tried looking for it at " + localPath + "/" + ExerciseSchemaFileName);
            }
            var schemaFile = (TextAsset)Resources.Load("exercise-schema");
            _exerciseSchemaReader = XmlReader.Create(new MemoryStream(schemaFile.bytes));
            //_exerciseSchemaReader = XmlReader.Create(File.OpenRead(localPath + "/" + ExerciseSchemaFileName));

            var loadExerciseButtonGameObject = GameObject.Find("Load Exercise Button");
            _exerciseTitleText = loadExerciseButtonGameObject.GetComponentInChildren<Text>();

            //_internetTab = transform.Find("Panel").Find("Internet//Local Tab");
            //_internetTab.gameObject.SetActive(true);
		
            var shapePanelContainerGameObject = GameObject.Find("Shape Panel Container");
            _shapePanelContainer = shapePanelContainerGameObject.GetComponent<ShapePanelContainer>();

            var exercisePanelContainerGameObject = GameObject.Find("Exercise Panel Container");
            _exercisePanelContainer = exercisePanelContainerGameObject.GetComponent<ExercisePanelContainer>();

            //RefreshList();

            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);
        }

        private void Update()
        {
            //Initialize list with null before it's clear there is a list - job returns instantly if list is null.
            //var parseListJob = new ParseListJob()
            //{
            //    Dialog = this,
            //    List = null,
            //    ListItemPrefab = exerciseBrowserListItemPrefab,
            //    ViewPort = viewport
            //};

            //var parseExerciseJob = new ParseExerciseJob()
            //{
            //    ExercisePanelContainer = _exercisePanelContainer,
            //    Schema = _exerciseSchema,
            //    ShapePanelContainer = _shapePanelContainer,
            //    TitleText = _exerciseTitleText,
            //    Xml = "",
            //};

            //If the request is done and has no errors, stop polling the request and parse the response.
            if (_listRequestState == ListRequestState.Done)
            {
                StopCoroutine(CheckListResponse());
                //Hard to parallelize because Jobs can't use normal Collections classes.
                var exercises = ConvertExerciseListWebResponse(_refreshListRequest.downloadHandler.text);

                _listRequestState = ListRequestState.Parsing;
                ExerciseParser.ParseList(exercises, exerciseBrowserListItemPrefab, viewport, this);
                _listRequestState = ListRequestState.NotStarted;
                _refreshListRequest = null;
                //parseListJob.List = exercises;
            }
            //JobHandle listParserHandle = parseListJob.Schedule();

            //If the request is done and has no errors, stop polling the request and parse the response.
            if (_exerciseRequestState == ExerciseRequestState.Done)
            {
                StopCoroutine(CheckExerciseResponse());
                _exerciseRequestState = ExerciseRequestState.Parsing;

                var exercise = JsonUtility.FromJson<Exercise>(_loadExerciseRequest.downloadHandler.text);
                ExerciseParser.ParseExercise(exercise.data, _exerciseSchemaReader, _exerciseTitleText, _exercisePanelContainer, _shapePanelContainer, this);
                //Destroying the dialog because it's no longer needed
                Destroy(gameObject);
                //parseExerciseJob.Xml = _loadExerciseRequest.downloadHandler.text;
            }
            //JobHandle exerciseParserHandle = parseExerciseJob.Schedule();

            //If the request has an error, display an error message and destroy the dialog
            if (_listRequestState == ListRequestState.Error)
            {
                //This removes the exercise dialog and thus terminates this script
                if (_refreshListRequest != null)
                {
                    switch (_refreshListRequest.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                            DisplayErrorMessage("Netzwerkfehler", "Kommunikation mit dem Server fehlgeschlagen.");
                            break;
                        case UnityWebRequest.Result.DataProcessingError:
                            DisplayErrorMessage("Netzwerkfehler", "Antwort vom Server war fehlerhaft.");
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            DisplayErrorMessage("Netzwerkfehler", _refreshListRequest.error);
                            break;
                    }
                }
                else
                {
                    DisplayErrorMessage("Netzwerkfehler", "Es ist ein unbekannter Netzwerkfehler aufgetreten.");
                }
            }

            //If the request has an error, display an error message and destroy the dialog
            if (_exerciseRequestState == ExerciseRequestState.Error)
            {
                //This removes the exercise dialog and thus terminates this script
                switch (_loadExerciseRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        DisplayErrorMessage("Netzwerkfehler", "Kommunikation mit dem Server fehlgeschlagen.");
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        DisplayErrorMessage("Netzwerkfehler", "Antwort vom Server war fehlerhaft.");
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        DisplayErrorMessage("Netzwerkfehler", _loadExerciseRequest.error);
                        break;
                }
            }

            //If there is a pending request, start polling the request for a result
            if (_listRequestState == ListRequestState.Pending)
            {
                _listRequestState = ListRequestState.Checking;
                StartCoroutine(CheckListResponse());
            }
            //If there is a pending request, start polling the request for a result
            if (_exerciseRequestState == ExerciseRequestState.Pending)
            {
                _exerciseRequestState = ExerciseRequestState.Checking;
                StartCoroutine(CheckExerciseResponse());
            }

            //listParserHandle.Complete();

            //Parsing is complete, so reset the state and the request object
            if (_listRequestState == ListRequestState.Parsing)
            {
                _refreshListRequest = null;
                _listRequestState = ListRequestState.NotStarted;
            }

            //exerciseParserHandle.Complete();
            if (_exerciseRequestState == ExerciseRequestState.Parsing)
            {
                _loadExerciseRequest = null;
                _exerciseRequestState = ExerciseRequestState.NotStarted;
            }
        }

        /// <summary>
        /// Displays an error dialog with the given title and message and removes the exercise dialog
        /// </summary>
        /// <param name="title">The title of the error message</param>
        /// <param name="message">The error message</param>
        /// <param name="destroy">Whether or not to destroy the exercise dialog on displaying the error message</param>
        public void DisplayErrorMessage(string title, string message, bool destroy=true)
        {
            var errorDialog = Instantiate(errorDialogPrefab, _dialogContainer.transform);

            var errorDialogRectTransform = errorDialog.GetComponent<RectTransform>();
            errorDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            errorDialogRectTransform.offsetMax = new Vector2(0f, 0f);

            var dialogScript = errorDialog.GetComponent<ErrorDialog>();

            dialogScript.SetTexts(title, message);

            if (destroy)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Checks the state of the _refreshListRequest every .1 seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckListResponse()
        {
            for (;;)
            {
                if (_refreshListRequest.isDone)
                {
                    if (_refreshListRequest.result == UnityWebRequest.Result.Success)
                    {
                        _listRequestState = ListRequestState.Done;
                    }
                    else if (_refreshListRequest.result != UnityWebRequest.Result.InProgress)
                    {
                        _listRequestState = ListRequestState.Error;
                    }
                    break;
                }
                yield return new WaitForSeconds(.1f);
            }
        }

        /// <summary>
        /// Checks the state of the _loadExerciseRequest every .1 seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckExerciseResponse()
        {
            for (;;)
            {
                if (_loadExerciseRequest.isDone)
                {
                    if (_loadExerciseRequest.result == UnityWebRequest.Result.Success)
                    {
                        _exerciseRequestState = ExerciseRequestState.Done;
                    }
                    else if (_loadExerciseRequest.result != UnityWebRequest.Result.InProgress)
                    {
                        _exerciseRequestState = ExerciseRequestState.Error;
                    }
                    break;
                }
                yield return new WaitForSeconds(.1f);
            }
        }

        /// <summary>
        /// Converts the contents of the list web response to a list of ExerciseMetaData objects to generate the list of exercises
        /// </summary>
        /// <param name="input">The web response string</param>
        /// <returns>A list of ExerciseMetaData objects</returns>
        private List<ExerciseMetaData> ConvertExerciseListWebResponse(string input)
        {
            var arr = JsonUtility.FromJson<ExerciseList>("{\"list\":" + input + "}");
        
            var result = new List<ExerciseMetaData>();

            if (arr != null)
            {
                result.AddRange(arr.list);
            }

            return result;
        }

        /// <summary>
        /// Refreshes the list of exercises in the dialog
        /// </summary>
        /// <param name="fromInternet">Whether or not to load the list from the internet</param>
        public void RefreshList(bool fromInternet)
        {
            //_internetTab.gameObject.SetActive(true);
		
            for (int i = 0; i < viewport.transform.childCount; i++)
            {
                Destroy(viewport.transform.GetChild(i).gameObject);
            }

            if (fromInternet)
            {
                //Old URL: "http://public.hochschule-trier.de/~simonj/suremath/list.php"
                _refreshListRequest = UnityWebRequest.Get(_config.ExerciseListUrl);
                _refreshListRequest.SendWebRequest();
                _listRequestState = ListRequestState.Pending;
            }
            else
            {
                Debug.Log("Exercise Path: " + _config.LocalExercisePath);
                var directoryInfo = new DirectoryInfo(_config.LocalExercisePath);
                var fileInfo = directoryInfo.GetFiles("*.suremath", SearchOption.TopDirectoryOnly);
                Debug.Log("Files found: " + fileInfo.Length);

                var exList = new List<ExerciseMetaData>();

                var id = 0;

                Array.ForEach(fileInfo, file =>
                {
                    var data = new ExerciseMetaData();
                    data.Exercise_Id = id;
                    data.Author = "";
                    data.Title = file.Name;
                    data.Faculty = "";
                    exList.Add(data);
                    id++;
                });

                ExerciseParser.ParseList(exList, exerciseBrowserListItemPrefab, viewport, this);
            }

            _fromInternet = fromInternet;
        }

        /// <summary>
        /// Loads the exercise with the given ID. ID differs based on whether or not exercises are loaded from the internet.
        /// If they're loaded from the internet, IDs correspond to exercise IDs in the database. Otherwise, they're the index of the file found on disk.
        /// </summary>
        /// <param name="id">The ID of the exercise to be loaded</param>
        public void LoadExercise(int id)
        {
            for (var i = 0; i < viewport.transform.childCount; i++)
            {
                viewport.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (_fromInternet)
            {
                Debug.Log("Trying to request exercise from online: " + id);
                _loadExerciseRequest = UnityWebRequest.Get(_config.ExerciseUrl + "/" + id);
                _loadExerciseRequest.SendWebRequest();
                _exerciseRequestState = ExerciseRequestState.Pending;
            }
            else
            {
                var exerciseBrowserListItemController = viewport.transform.GetChild(id).GetComponent<ExerciseBrowserPanel>();
                var titleText = exerciseBrowserListItemController.titleTextGameObject.GetComponent<Text>();

                var exerciseString = File.ReadAllText(@""+_config.LocalExercisePath + "\\" + titleText.text);
                ExerciseParser.ParseExercise(exerciseString, _exerciseSchemaReader, _exerciseTitleText, _exercisePanelContainer, _shapePanelContainer, this);
                Destroy(gameObject);
            }
        }

        public void BackButtonClicked()
        {
            Destroy(gameObject);
        }
    }
}
