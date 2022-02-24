using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Math_View;
using Panels.Exercise_Browser_Panel;
using Panels.Exercise_Prompt_Panel;
using Panels.Shape_Panel;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Exercise_Dialog
{
    /// <summary>
    /// Parser class offering static functions to parse exercise files
    /// </summary>
    public class ExerciseParser : MonoBehaviour
    {
        public static ExerciseParser instance;

        private const string ExerciseSchema =
            @"<xs:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
        
       <xs:element name='suremath-exercise'>  
        <xs:complexType>
            <xs:attribute name='title' type='xs:string'/>
            <xs:all>
              <xs:element name='prompts' type='promptList' minOccurs='1' maxOccurs='1'/>
              <xs:element name='shapes' type='shapeList' minOccurs='0' maxOccurs='1'/>
            </xs:all>
        </xs:complexType>  
       </xs:element>

       <xs:complexType name='value-prompt'>
            <xs:simpleContent>
              <xs:extension base='xs:string'>
                <xs:attribute type='xs:string' name='type' use='required' fixed='value'/>
                <xs:attribute type='xs:decimal' name='solution' use='required'/>
              </xs:extension>
            </xs:simpleContent>
       </xs:complexType>

        <xs:complexType name='text-prompt'>
            <xs:simpleContent>
              <xs:extension base='xs:string'>
                <xs:attribute type='xs:string' name='type' use='required' fixed='text'/>
              </xs:extension>
            </xs:simpleContent>
       </xs:complexType>

        <xs:complexType name='point-prompt'>
            <xs:simpleContent>
              <xs:extension base='xs:string'>
                <xs:attribute type='xs:string' name='type' use='required' fixed='point'/>
                <xs:attribute type='xs:decimal' name='solution_x' use='required'/>
                <xs:attribute type='xs:decimal' name='solution_y' use='required'/>
              </xs:extension>
            </xs:simpleContent>
       </xs:complexType>

        <xs:complexType name='line'>
          <xs:attribute type='xs:string' name='name'/>
          <xs:attribute type='xs:integer' name='id'/>
          <xs:sequence>
            <xs:element name='color' type='color'/>
            <xs:element name='handle1' type='handle'/>
            <xs:element name='handle2' type='handle/>
            <xs:element name='properties' type='propertyType'/>
          </xs:sequence>
        </xs:complexType>

        <xs:complexType name='parabola'>
          <xs:attribute type='xs:string' name='name'/>
          <xs:sequence>
            <xs:element name='color' type='color'/>
            <xs:element name='originhandle' type='handle'/>
            <xs:element name='curvehandle' type='handle/>
            <xs:element name='properties' type='propertyType'/>
          </xs:sequence>
        </xs:complexType>

        <xs:complexType name='angle'>
          <xs:attribute type='xs:string' name='name'/>
          <xs:sequence>
            <xs:element name='color' type='color'/>
            <xs:element name='lines' type='lineIds'/>
          </xs:sequence>
        </xs:complexType>

        <xs:complexType name='color'>
          <xs:attribute name='r' type='xs:decimal' use='required'/>
          <xs:attribute name='g' type='xs:decimal' use='required'/>
          <xs:attribute name='b' type='xs:decimal' use='required'/>
        </xs:complexType>

        <xs:complexType name='handle'>
          <xs:attribute name='x' type='xs:decimal' use='required'/>
          <xs:attribute name='y' type='xs:decimal' use='required'/>
        </xs:complexType>

        <xs:complexType name='propertyType'>
          <xs:attribute name='infinite' type='xs:boolean'/>
        </xs:complexType>

        <xs:complexType name='lineIds'>
          <xs:attribute name='a' type='xs:integer' use='required'/>
          <xs:attribute name='b' type='xs:integer' use='required'/>
        </xs:complexType>

        <xs:complexType name='promptList'>
            <xs:element name='prompt' type='value-prompt' minOccurs='0' maxOccurs='Unbounded'/>
            <xs:element name='prompt' type='text-prompt' minOccurs='0' maxOccurs='Unbounded'/>
            <xs:element name='prompt' type='point-prompt' minOccurs='0' maxOccurs='Unbounded'/>
        </xs:complexType>

        <xs:complexType name='shapeList'>
            <xs:element name='line' type='line' minOccurs='0' maxOccurs='Unbounded'/>
            <xs:element name='parabola' type='parabola' minOccurs='0' maxOccurs='Unbounded'/>
            <xs:element name='angle' type='angle' minOccurs='0' maxOccurs='Unbounded'/>
        </xs:complexType>
      </xs:schema>";

        private static readonly bool validate = true;

        void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Parses a list of exercises and fills the exercise browser list with what it parsed.
        /// </summary>
        /// <param name="exercises">The list of exercises</param>
        /// <param name="exerciseBrowserListItemPrefab">Prefab for making exercise browser list items</param>
        /// <param name="viewport">The viewport of the application</param>
        /// <param name="exerciseDialog">The exercise dialog the browser panel of which needs to be filled</param>
        public static void ParseList(List<ExerciseMetaData> exercises, GameObject exerciseBrowserListItemPrefab, GameObject viewport, ExerciseDialog exerciseDialog)
        {
            exercises.ForEach(exercise =>
            {
                var id = exercise.Exercise_Id;
                string title = exercise.Title.Equals("") ? "?" : exercise.Title, 
                    author = exercise.Author.Equals("") ? "?" : exercise.Author,
                    faculty = exercise.Faculty.Equals("") ? "?" : exercise.Faculty;

                var exerciseBrowserListItem = Instantiate(exerciseBrowserListItemPrefab, viewport.transform);
                var exerciseBrowserListItemController = exerciseBrowserListItem.GetComponent<ExerciseBrowserPanel>();
                exerciseBrowserListItemController.SetExerciseDialog(exerciseDialog);
                exerciseBrowserListItemController.SetInitialValues(id, title, author, faculty);
            });
        }

        /// <summary>
        /// Parses an exercise prompt of any type.
        /// </summary>
        /// <param name="element">The XElement containing prompt data</param>
        /// <param name="exercisePanelContainer">The exercise panel container where the parsed prompt is supposed to go</param>
        /// <param name="exerciseDialog">The exercise dialog this parse job originated from</param>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the input isn't able to be parsed</exception>
        private static void ParsePrompt(XElement element, ExercisePanelContainer exercisePanelContainer, ExerciseDialog exerciseDialog)
        {
            var type = element.Attribute("type")?.Value;

            if (type == null) return;
            switch (type)
            {
                case ("text"):
                    ParseTextPrompt(element, exercisePanelContainer);
                    break;
                case ("value"):
                    ParseValuePrompt(element, exercisePanelContainer);
                    break;
                case ("point"):
                    ParsePointPrompt(element, exercisePanelContainer);
                    break;
                default:
                    exerciseDialog.DisplayErrorMessage("Unbekannter Aufgabentyp", "Die Aufgabendatei enthält einen unbekannten Aufgabentyp. Vorgang abgebrochen.", false);
                    throw new ArgumentException("unknown prompt type");
            }
        }

        /// <summary>
        /// Parses a text prompt.
        /// </summary>
        /// <param name="element">The XElement containing prompt data</param>
        /// <param name="exercisePanelContainer">The exercise panel container where the parsed prompt is supposed to go</param>
        private static void ParseTextPrompt(XElement element, ExercisePanelContainer exercisePanelContainer)
        {
            exercisePanelContainer.AddTextPromptPanel(element.Value);
        }

        /// <summary>
        /// Parses a value prompt.
        /// </summary>
        /// <param name="element">The XElement containing prompt data</param>
        /// <param name="exercisePanelContainer">The exercise panel container where the parsed prompt is supposed to go</param>
        private static void ParseValuePrompt(XElement element, ExercisePanelContainer exercisePanelContainer)
        {
            var solution = element.Attribute("solution") != null ? float.Parse(element.Attribute("solution").Value.Replace('.', ',')) : 0f;

            exercisePanelContainer.AddValuePromptPanel(element.Value, solution);
        }

        /// <summary>
        /// Parses a point prompt.
        /// </summary>
        /// <param name="element">The XElement containing prompt data</param>
        /// <param name="exercisePanelContainer">The exercise panel container where the parsed prompt is supposed to go</param>
        private static void ParsePointPrompt(XElement element, ExercisePanelContainer exercisePanelContainer)
        {
            var solution = Vector2.zero;

            solution.x = element.Attribute("solution_x") != null ? float.Parse(element.Attribute("solution_x").Value.Replace('.', ',')) : 0f;
            solution.y = element.Attribute("solution_y") != null ? float.Parse(element.Attribute("solution_y").Value.Replace('.', ',')) : 0f;

            exercisePanelContainer.AddPointPromptPanel(element.Value, solution);
        }

        /// <summary>
        /// Parses a line.
        /// </summary>
        /// <param name="lineElement">The XElement containing line data</param>
        /// <param name="shapePanelContainer">The shape panel container where the line panel is supposed to go</param>
        /// <param name="exerciseDialog">The exercise dialog this parser belongs to</param>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the input isn't able to be parsed</exception>
        private static void ParseLine(XElement lineElement, ShapePanelContainer shapePanelContainer, ExerciseDialog exerciseDialog)
        {
            var name = lineElement.Attribute("name") != null ? lineElement.Attribute("name")?.Value : "Unbenannte Gerade";
            var id = 0;
            var idSet = false;
            var infinite = lineElement.Element("properties")?.Attribute("infinite") != null ? bool.Parse(lineElement.Element("properties").Attribute("infinite").Value) : true;
            Vector2 handle1 = Vector2.zero, handle2 = Vector2.zero;

            var colorElement = lineElement.Element("color");

            var colorR = colorElement?.Attribute("r") != null ? float.Parse(colorElement.Attribute("r").Value.Replace('.', ',')) : 0f;
            var colorG = colorElement?.Attribute("g") != null ? float.Parse(colorElement.Attribute("g").Value.Replace('.', ',')) : 0f;
            var colorB = colorElement?.Attribute("b") != null ? float.Parse(colorElement.Attribute("b").Value.Replace('.', ',')) : 0f;

            if (lineElement.Attribute("id") != null)
            {
                id = int.Parse(lineElement.Attribute("id").Value);
                idSet = true;
            }

            if (lineElement.Element("handle1") != null)
            {
                var x = float.Parse(lineElement.Element("handle1").Attribute("x").Value.Replace('.', ','));
                var y = float.Parse(lineElement.Element("handle1").Attribute("y").Value.Replace('.', ','));
                Debug.Log(x + y);
                handle1 = new Vector2(x, y);
            }
            else
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es fehlen Koordinaten eines Punkts auf einer Geraden. Vorgang abgebrochen.", false);
                throw new ArgumentException("Handle coordinates missing in line");
            }

            if (lineElement.Element("handle2") != null)
            {
                var x = float.Parse(lineElement.Element("handle2").Attribute("x").Value.Replace('.', ','));
                var y = float.Parse(lineElement.Element("handle2").Attribute("y").Value.Replace('.', ','));

                handle2 = new Vector2(x, y);
            }
            else
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es fehlen Koordinaten eines Punkts auf einer Geraden. Vorgang abgebrochen.", false);
                throw new ArgumentException("Handle coordinates missing in line");
            }

            bool lineName = true,
                expand = true,
                delete = true,
                color = true,
                m = true,
                b = true,
                points = true,
                parameters = true,
                length = true,
                lineInfinite = true,
                snap = true,
                formula = true,
                angle = true,
                handleA = true,
                handleB = true;
            if (lineElement.Element("unlocks") != null)
            {
                var unlocks = lineElement.Element("unlocks")?.Value.Split(',');
                if (unlocks != null)
                {
                    foreach (var lockString in unlocks)
                    {
                        switch (lockString)
                        {
                            case "name":
                                lineName = false;
                                break;
                            case "expand":
                                expand = false;
                                break;
                            case "delete":
                                delete = false;
                                break;
                            case "color":
                                color = false;
                                break;
                            case "m":
                                m = false;
                                break;
                            case "b":
                                b = false;
                                break;
                            case "points":
                                points = false;
                                break;
                            case "parameters":
                                parameters = false;
                                break;
                            case "length":
                                length = false;
                                break;
                            case "infinite":
                                lineInfinite = false;
                                break;
                            case "snap":
                                snap = false;
                                break;
                            case "formula":
                                formula = false;
                                break;
                            case "angle":
                                angle = false;
                                break;
                            case "handle1":
                                handleA = false;
                                break;
                            case "handle2":
                                handleB = false;
                                break;
                            default:
                                exerciseDialog.DisplayErrorMessage("Unbekannte Anzeigesperre", "Beim Einlesen der Aufgabendatei wurde eine unbekannte Anzeigesperre festgestellt. Vorgang abgebrochen.", false);
                                throw new ArgumentException("unknown display lock");
                        }
                    }
                }
            }
            var lineLock = new LineLock(lineName, expand, delete, color, formula, points, parameters, m, b,
                length, lineInfinite, snap, angle, handleA, handleB);

            GameObject linePanel;

            if (idSet)
            {
                linePanel = shapePanelContainer.AddLinePanel(lineLock, name, new Color(colorR, colorG, colorB), handle1, handle2, infinite, id, false);
            }
            else
            {
                linePanel = shapePanelContainer.AddLinePanel(lineLock, name, new Color(colorR, colorG, colorB), handle1, handle2, infinite, false);
            }
        }

        /// <summary>
        /// Parses a parabola.
        /// </summary>
        /// <param name="parabolaElement">The XElement containing parabola data</param>
        /// <param name="shapePanelContainer">The shape panel container where the parabola panel is supposed to go</param>
        /// <param name="exerciseDialog">The exercise dialog this parser belongs to</param>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the input isn't able to be parsed</exception>
        private static void ParseParabola(XElement parabolaElement, ShapePanelContainer shapePanelContainer, ExerciseDialog exerciseDialog)
        {
            var name = parabolaElement.Attribute("name") != null ? parabolaElement.Attribute("name")?.Value : "Unbenannte Parabel";

            var infinite = parabolaElement.Element("properties")?.Attribute("infinite") != null ? bool.Parse(parabolaElement.Element("properties").Attribute("infinite").Value) : true;
            Vector2 originHandle = Vector2.zero, curveHandle = Vector2.zero;

            var colorElement = parabolaElement.Element("color");

            var colorR = colorElement?.Attribute("r") != null ? float.Parse(colorElement.Attribute("r").Value.Replace('.', ',')) : 0f;
            var colorG = colorElement?.Attribute("g") != null ? float.Parse(colorElement.Attribute("g").Value.Replace('.', ',')) : 0f;
            var colorB = colorElement?.Attribute("b") != null ? float.Parse(colorElement.Attribute("b").Value.Replace('.', ',')) : 0f;

            if (parabolaElement.Element("curvehandle") != null)
            {
                var x = float.Parse(parabolaElement.Element("curvehandle").Attribute("x").Value.Replace('.', ','));
                var y = float.Parse(parabolaElement.Element("curvehandle").Attribute("y").Value.Replace('.', ','));

                curveHandle = new Vector2(x, y);
            }
            else
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es fehlen Koordinaten eines Punkts auf einer Parabel. Vorgang abgebrochen.", false);
                throw new ArgumentException("Handle coordinates missing in parabola");
            }

            if (parabolaElement.Element("originhandle") != null)
            {
                var x = float.Parse(parabolaElement.Element("originhandle").Attribute("x").Value.Replace('.', ','));
                var y = float.Parse(parabolaElement.Element("originhandle").Attribute("y").Value.Replace('.', ','));

                originHandle = new Vector2(x, y);
            }
            else
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es fehlen Koordinaten eines Punkts auf einer Parabel. Vorgang abgebrochen.", false);
                throw new ArgumentException("Handle coordinates missing in parabola");
            }

            bool parabolaName = true,
                expand = true,
                delete = true,
                color = true,
                a = true,
                b = true,
                c = true,
                standard = true,
                vertex = true,
                points = true,
                parameters = true,
                parabolaInfinite = true,
                snap = true,
                originhandle = true,
                curvehandle = true;
            if (parabolaElement.Element("unlocks") != null)
            {
                var unlocks = parabolaElement.Element("unlocks")?.Value.Split(',');
                if (unlocks != null)
                {
                    foreach (var lockString in unlocks)
                    {
                        switch (lockString)
                        {
                            case "name":
                                parabolaName = false;
                                break;
                            case "expand":
                                expand = false;
                                break;
                            case "delete":
                                delete = false;
                                break;
                            case "color":
                                color = false;
                                break;
                            case "a":
                                a = false;
                                break;
                            case "b":
                                b = false;
                                break;
                            case "c":
                                c = false;
                                break;
                            case "standard":
                                standard = false;
                                break;
                            case "vertex":
                                vertex = false;
                                break;
                            case "points":
                                points = false;
                                break;
                            case "parameters":
                                parameters = false;
                                break;
                            case "infinite":
                                parabolaInfinite = false;
                                break;
                            case "snap":
                                snap = false;
                                break;
                            case "originhandle":
                                originhandle = false;
                                break;
                            case "curvehandle":
                                curvehandle = false;
                                break;
                            default:
                                exerciseDialog.DisplayErrorMessage("Unbekannte Anzeigesperre", "Beim Einlesen der Aufgabendatei wurde eine unbekannte Anzeigesperre festgestellt. Vorgang abgebrochen.", false);
                                throw new ArgumentException("unknown display lock");
                        }
                    }
                }
            }
            var parabolaLock = new ParabolaLock(parabolaName, expand, delete, color, standard, vertex, parameters, points, a, b, c, parabolaInfinite, snap, originhandle, curvehandle);

            shapePanelContainer.AddParabolaPanel(parabolaLock, name, new Color(colorR, colorG, colorB), originHandle, curveHandle, infinite);
        }

        /// <summary>
        /// Parses an angle.
        /// </summary>
        /// <param name="angleElement">The XElement containing angle data</param>
        /// <param name="shapePanelContainer">The shape panel container where the angle panel is supposed to go</param>
        /// <param name="exerciseDialog">The exercise dialog this parser belongs to</param>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the input isn't able to be parsed</exception>
        private static void ParseAngle(XElement angleElement, ShapePanelContainer shapePanelContainer, ExerciseDialog exerciseDialog)
        {
            var name = angleElement.Attribute("name") != null ? angleElement.Attribute("name")?.Value : "Unbenannter Winkel";
            int lineAId = 0, lineBId = 0;
            bool lineASet = false, lineBSet = false;

            var linePanels = shapePanelContainer.GetComponentsInChildren<LinePanel>();
            var lineIds = new int[linePanels.Length];
            for(var i = 0; i < linePanels.Length; i++)
            {
                lineIds[i] = linePanels[i].Line.ID;
            }

            var colorElement = angleElement.Element("color");

            var colorR = colorElement?.Attribute("r") != null ? float.Parse(colorElement.Attribute("r").Value.Replace('.', ',')) : 0f;
            var colorG = colorElement?.Attribute("g") != null ? float.Parse(colorElement.Attribute("g").Value.Replace('.', ',')) : 0f;
            var colorB = colorElement?.Attribute("b") != null ? float.Parse(colorElement.Attribute("b").Value.Replace('.', ',')) : 0f;

            var linesElement = angleElement.Element("lines");

            if (linesElement != null)
            {
                if (linesElement.Attribute("a") != null)
                {
                    lineAId = int.Parse(linesElement.Attribute("a").Value);
                    lineASet = true;
                }
                else
                {
                    exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es fehlt eine Referenz zu einer Geraden für einen Winkel. Vorgang abgebrochen.", false);
                    throw new ArgumentException("line data missing from angle");
                }

                if (linesElement.Attribute("b") != null)
                {
                    lineBId = int.Parse(linesElement.Attribute("b").Value);
                    lineBSet = true;
                }
                else
                {
                    exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es Es fehlt eine Referenz zu einer Geraden für einen Winkel. Vorgang abgebrochen.", false);
                    throw new ArgumentException("line data missing from angle");
                }
            }

            if (lineASet && lineBSet && lineAId == lineBId)
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es wurde für einen Winkel zweimal dieselbe Gerade angegeben. Vorgang abgebrochen.", false);
                throw new ArgumentException("duplicate line ID");
            }

            if (lineASet && lineBSet && (!lineIds.Contains(lineAId) || !lineIds.Contains(lineBId)))
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Geraden, die für einen Winkel benötigt werden, existieren nicht. Vorgang abgebrochen.", false);
                throw new ArgumentException("referenced lines do not exist");
            }

            if (!lineASet || !lineBSet)
            {
                exerciseDialog.DisplayErrorMessage("Aufgabendatei fehlerhaft", "Es fehlen sämtliche Referenzen zu Geraden für einen Winkel. Vorgang abgebrochen.", false);
                throw new ArgumentException("lines data missing from angle");
            }

            Line lineA = null, lineB = null;
            foreach (var panel in linePanels)
            {
                if (panel.Line.ID == lineAId)
                {
                    lineA = panel.Line;
                }
                else if (panel.Line.ID == lineBId)
                {
                    lineB = panel.Line;
                }
            }

            bool angleName = true,
                expand = true,
                delete = true,
                color = true,
                rad = true,
                deg = true,
                adjacent = true,
                sin = true,
                cos = true,
                tan = true;
            if (angleElement.Element("unlocks") != null)
            {
                var unlocks = angleElement.Element("unlocks")?.Value.Split(',');
                if (unlocks != null)
                {
                    foreach (var lockString in unlocks)
                    {
                        switch (lockString)
                        {
                            case "name":
                                angleName = false;
                                break;
                            case "expand":
                                expand = false;
                                break;
                            case "delete":
                                delete = false;
                                break;
                            case "color":
                                color = false;
                                break;
                            case "rad":
                                rad = false;
                                break;
                            case "deg":
                                deg = false;
                                break;
                            case "adjacent":
                                adjacent = false;
                                break;
                            case "sin":
                                sin = false;
                                break;
                            case "cos":
                                cos = false;
                                break;
                            case "tan":
                                tan = false;
                                break;
                            default:
                                exerciseDialog.DisplayErrorMessage("Unbekannte Anzeigesperre", "Beim Einlesen der Aufgabendatei wurde eine unbekannte Anzeigesperre festgestellt. Vorgang abgebrochen.", false);
                                throw new ArgumentException("unknown display lock");
                        }
                    }
                }
            }
            var angleLock = new AngleLock(angleName, expand, delete, color, deg, rad, sin, cos, tan, adjacent);

            if (lineA != null && lineB != null)
            {
                var angle = new Angle(angleLock, lineA, lineB, name, new Color(colorR, colorG, colorB));
                var anglePanelGameObject = shapePanelContainer.AddAnglePanel();
                var anglePanel = anglePanelGameObject.GetComponent<AnglePanel>();
                anglePanel.Angle = angle;
            }
            else
            {
                exerciseDialog.DisplayErrorMessage("Anwendungsfehler", "Mindestens eine Gerade, die für einen Winkel benötigt wird, ist fehlerhaft. Vorgang abgebrochen.", false);
                throw new ArgumentException("error creating lines");
            }
        }

        IEnumerator ParseSequence(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.CompareTo("debug") == 0)
                {
                    string message = "";

                    foreach (XmlAttribute attribute in childNode.Attributes)
                    {
                        if (attribute.Name.CompareTo("message") == 0)
                        {
                            message = attribute.Value;
                        }
                    }

                    // TODO: this prints the message, waits one second and then RETURNS, cancelling everything else, I need to find a way around that.
                    Debug.Log(message);
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Aha");
                }
                /*
			else if (childNode.Name.CompareTo("wait") == 0)
			{
				float seconds = 0f;

				foreach (XmlAttribute attribute in childNode.Attributes)
				{
					if (attribute.Name.CompareTo("seconds") == 0)
					{
						seconds = float.Parse(attribute.Value);
					}
				}

				Debug.Log("Now waiting for " + seconds + " seconds...");
				yield return new WaitForSeconds(seconds);
			}
			*/
            }
        }

        /// <summary>
        /// Parses an exercise with all corresponding shapes and prompts.
        /// </summary>
        /// <param name="xml">The xml data to parse</param>
        /// <param name="schema">An xml schema to validate the xml data against</param>
        /// <param name="exerciseTitleText">The UI Text element that shows the title of the exercise</param>
        /// <param name="exercisePanelContainer">The exercise panel container where all the prompts go</param>
        /// <param name="shapePanelContainer">The shape panel container where all the shapes go</param>
        /// <param name="exerciseDialog">The exercise dialog using this parser</param>
        public static void ParseExercise(string xml, XmlReader schema, Text exerciseTitleText, ExercisePanelContainer exercisePanelContainer, ShapePanelContainer shapePanelContainer, ExerciseDialog exerciseDialog)
        {
            shapePanelContainer.ClearAllPanels();
            exercisePanelContainer.ClearPanels(false);

            var readerSettings = new XmlReaderSettings();
            var schemaSet = new XmlSchemaSet();


            try
            {
                schemaSet.Add(null, schema);
            }
            catch (XmlSchemaException e)
            {
                Debug.Log(e.Message + " " + e.LineNumber + " " + e.LinePosition);
                return;
            }
            

            var doc = XDocument.Parse(xml);

            if (validate)
            {
                var xmlValid = true;

                try
                {
                    doc.Validate(schemaSet, (o, e) =>
                    {
                        xmlValid = false;
                        Debug.Log("XML validation failed!");

                        Debug.Log(e.Message + " " + e.Exception.LineNumber + " " + e.Exception.LinePosition);
                        Debug.Log(doc.Root.ToString());
                        exerciseDialog.DisplayErrorMessage("Validierung fehlgeschlagen", "Die Aufgabendatei konnte nicht validiert werden: " + e.Message, false);
                    });
                }
                catch (XmlSchemaValidationException e)
                {
                    Debug.Log(e.Message + " " + e.LineNumber + " " + e.LinePosition);
                    Debug.Log(e.SourceObject);
                    shapePanelContainer.ClearAllPanels();
                    exercisePanelContainer.ClearPanels(true);
                    exerciseTitleText.text = "Aufgabe laden...";
                    return;
                }
                

                if (!xmlValid)
                {
                    shapePanelContainer.ClearAllPanels();
                    exercisePanelContainer.ClearPanels(true);
                    exerciseTitleText.text = "Aufgabe laden...";
                    return;
                }
            }

            var mainElement = doc.Element("suremath-exercise");
            var titleAttr = mainElement?.Attribute("title");

            if (titleAttr != null)
            {
                exerciseTitleText.text = titleAttr.Value;
            }

            var promptListElement = mainElement?.Element("prompts");

            if (promptListElement != null)
            {
                foreach (var prompt in promptListElement.Elements())
                {
                    try
                    {
                        ParsePrompt(prompt, exercisePanelContainer, exerciseDialog);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                        shapePanelContainer.ClearAllPanels();
                        exercisePanelContainer.ClearPanels(true);
                        exerciseTitleText.text = "Aufgabe laden...";
                        return;
                    }
                }
            }
            else
            {
                exerciseDialog.DisplayErrorMessage("Keine Aufgabenstellung", "Die Aufgabendatei enthält keine Aufgabenstellung.", false);
                shapePanelContainer.ClearAllPanels();
                exercisePanelContainer.ClearPanels(true);
                exerciseTitleText.text = "Aufgabe laden...";
                return;
            }

            var shapeListElement = mainElement?.Element("shapes");

            if (shapeListElement == null) return;
            {
                foreach (var line in shapeListElement.Elements("line"))
                {
                    try
                    {
                        ParseLine(line, shapePanelContainer, exerciseDialog);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                        shapePanelContainer.ClearAllPanels();
                        exercisePanelContainer.ClearPanels(true);
                        exerciseTitleText.text = "Aufgabe laden...";
                        return;
                    }
                
                }

                foreach (var parabola in shapeListElement.Elements("parabola"))
                {
                    try
                    {
                        ParseParabola(parabola, shapePanelContainer, exerciseDialog);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                        shapePanelContainer.ClearAllPanels();
                        exercisePanelContainer.ClearPanels(true);
                        exerciseTitleText.text = "Aufgabe laden...";
                        return;
                    }
                }

                foreach (var angle in shapeListElement.Elements("angle"))
                {
                    try
                    {
                        ParseAngle(angle, shapePanelContainer, exerciseDialog);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                        shapePanelContainer.ClearAllPanels();
                        exercisePanelContainer.ClearPanels(true);
                        exerciseTitleText.text = "Aufgabe laden...";
                        return;
                    }
                }
            }
        }
    }
}
