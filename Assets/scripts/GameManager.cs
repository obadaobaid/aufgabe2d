using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    #region Variables and references
    public GameObject left_box, right_box, cube; // Assign these in the Inspector
    public Material highlightMaterial;
    private Vector3 cubeStartPosition;
    public GameObject userChoice;
    private List<TrialData> trials = new List<TrialData>();
    private string originalFilePath = "trials.csv";
    private string copyFilePath = "results.csv";
    private bool selectionMade = false; // Flag to check if a selection has been made
    #endregion
    void Start()
    {
        originalFilePath = Path.Combine(Application.dataPath, originalFilePath);
        copyFilePath = Path.Combine(Application.dataPath, copyFilePath);
        cubeStartPosition = cube.transform.position;
        FetchCSV_Data(originalFilePath);
        CreateCopyWithResponsesColumn(copyFilePath);
        StartCoroutine(RunTrials());
    }
    IEnumerator RunTrials()
    {
        foreach (var trial in trials)
        {
            float soaInSeconds = trial.SOA / 100f;
            selectionMade = false; // Reset the flag at the start of each trial
            GameObject objectToHighlight = trial.isProbeLeft ? left_box : right_box;  // StartCoroutine(HighlightObject(objectToHighlight)); it should be deleted
            GameObject referenceBox = trial.isProbeLeft ? right_box : left_box;
            //if useranswer == trial.isproveleft :
              //      writw in DATA referce
                //else BinaryWriter probe 
            Renderer objRenderer = objectToHighlight.GetComponent<Renderer>();
            Material originalMaterial = objRenderer.material;


            StartCoroutine(HighlightObject(objectToHighlight, highlightMaterial));
            

            yield return new WaitForSeconds(trial.Fixation); // Wait for highlight duration
            // Make the object disappear for half a second and then reappear
            if (soaInSeconds < 0)
            {
                Debug.Log("soaInSeconds<0");
                yield return StartCoroutine(ToggleObjectVisibility(objectToHighlight, 0.0333f * 2));
                yield return new WaitForSeconds(soaInSeconds * -1);
                yield return StartCoroutine(ToggleObjectVisibility(referenceBox, 0.0333f * 2));
            }
            else if (soaInSeconds > 0)
            {
                Debug.Log("soaInSeconds>0");
                yield return StartCoroutine(ToggleObjectVisibility(referenceBox, 0.0333f * 2));
                yield return new WaitForSeconds(soaInSeconds);
                yield return StartCoroutine(ToggleObjectVisibility(objectToHighlight, 0.0333f * 2));

            }
            else
            {
                yield return StartCoroutine(ToggleObjectsVisibility(objectToHighlight, referenceBox, 0.0333f * 2));
            }
            // Wait here until selectionMade becomes true
            Debug.Log($"here ObjectSelectedfrom GameManager, SOA, fixation: {objectToHighlight}, {soaInSeconds}, {trial.Fixation} ");

            yield return new WaitUntil(() => selectionMade);

            //write responce and fixation in CSV
            Debug.Log($"here a in loop: {userChoice}");
            ModifyCopyFile(copyFilePath, trial, userChoice);
            // Logic to process the selection
            
            cube.transform.position = cubeStartPosition;
            objRenderer.material = originalMaterial;




        }
        // Handle all trials completed here
    }

    


    // This method is now called directly from DraggableObject when a selection is made
    public void RegisterUserSelection(GameObject selectedObject)
    {
        // Logic to determine if the selected object is the correct one
        //Debug.Log($"Selected object: {selectedObject.name}");
        // Implement logic to check if the selected object matches the expected answer
        // Then process the result and prepare for the next trial as necessary
        userChoice = selectedObject;
        Debug.Log($"here a, selectedObject.name: {userChoice},{selectedObject}");
        selectionMade = true; // Indicate that a selection has been made to continue the trials
    }

    // New method to toggle object visibility
    IEnumerator ToggleObjectVisibility(GameObject obj, float delay)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);

    }
    IEnumerator ToggleObjectsVisibility(GameObject obj1, GameObject obj2, float delay)
    {
        obj1.SetActive(false);
        obj2.SetActive(false);
        yield return new WaitForSeconds(delay);
        obj1.SetActive(true);
        obj2.SetActive(true);

    }

    IEnumerator HighlightObject(GameObject obj, Material highlightMaterial)
{
        Renderer objRenderer = obj.GetComponent<Renderer>();
        objRenderer.material = highlightMaterial;
         yield return new WaitForSeconds(0.1f); 


    }




    void FetchCSV_Data(string filePath)
        {
            //Debug.Log("here read CSV.");

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++) // Start at 1 to skip the header row
            {
                string[] columns = lines[i].Split(',');
                if (columns.Length == 2)
                {
                    float soa = float.Parse(columns[0]);
                    bool isProbeLeft = columns[1] == "1";
                    trials.Add(new TrialData(soa, isProbeLeft));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading CSV file: {e.Message}");
        }
    }

    void CreateCopyWithResponsesColumn(string copyFilePath)
    {
        Debug.Log("here copy CSV.");

        try
        {
            string[] lines = File.ReadAllLines(originalFilePath);
            using (StreamWriter sw = new StreamWriter(copyFilePath))
            {
                // Write header with an extra 'response' column
                sw.WriteLine($"{lines[0]},response,fixation");

                // Write the rest of the lines as is, but with an extra comma for the new column
                for (int i = 1; i < lines.Length; i++)
                {
                    sw.WriteLine($"{lines[i]}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating copy of CSV file: {e.Message}");
        }
    }
    void ModifyCopyFile(string filePath, TrialData trial , GameObject objectToHighlights)
    {
        try
        {
            // Die kopierte Datei öffnen und die erforderlichen Änderungen vornehmen
            List<string> lines = new List<string>(File.ReadAllLines(filePath));

            // Index der aktuellen Zeile bestimmen
            int currentIndex = trials.IndexOf(trial) + 1; // Zeilenindex beginnt bei 1, da die erste Zeile die Headerzeile ist

            // Die entsprechende Zeile finden und aktualisieren
            if (currentIndex < lines.Count)
            {
                // Die 'response' und 'fixation' Spaltenwerte für die aktuelle Zeile festlegen
                string responseValue = trial.isProbeLeft ? "left" : "right"; // Beispielwert für 'response'
                string fixationValue = trial.Fixation.ToString(); // Fixationswert

                // Die aktuelle Zeile aktualisieren, indem die 'response' und 'fixation' Werte hinzugefügt werden
                string updatedLine = $"{lines[currentIndex]},{objectToHighlights.name},{fixationValue}";

                // Die aktualisierte Zeile in die Liste einfügen
                lines[currentIndex] = updatedLine;

                // Die aktualisierten Zeilen in die Datei schreiben
                File.WriteAllLines(filePath, lines.ToArray());
            }
            else
            {
                Debug.LogError("Error: Current trial index exceeds the number of lines in the copied file.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error modifying copy of CSV file: {e.Message}");
        }
    }

    // Define a structure to hold the trial data
    public struct TrialData
    {
        public float SOA;
        public bool isProbeLeft;
        public float Fixation;

        public TrialData(float soa, bool isProbeLeft, float fixation)
        {
            this.SOA = soa;
            this.isProbeLeft = isProbeLeft;
            this.Fixation = fixation;
        }
        public TrialData(float soa, bool isProbeLeft) : this(soa, isProbeLeft, Random.Range(0.3f, 0.5f))
        {
        }
    }
    

}




