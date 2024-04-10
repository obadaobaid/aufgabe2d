using System.Collections.Generic;
using System.IO;
using UnityEngine;

public  class FileHandler : MonoBehaviour
{
    private static string originalFilePath = "trials.csv";  // Path to the original CSV file
    private static string resultFilePath = "results.csv"; // Path to the result CSV file

    public static List<TrialData> trials { get; private set; } // List to store trial data

    // Constructor to initialize the trials list
    public FileHandler()
    {
        trials = new List<TrialData>(); 
    }

    // Awake method to set file paths, fetch CSV data, and create a copy with two new column
    private void Awake()
    {
        originalFilePath = Path.Combine(Application.streamingAssetsPath, originalFilePath);
        resultFilePath = Path.Combine(Application.dataPath, resultFilePath);
        FetchCSV_Data();
        CreateCopyWithTwoNewColumn();
    }

    // to fetch trial data from the original CSV file
    public static void FetchCSV_Data()
    {
   
        try
        {
            string[] lines = File.ReadAllLines(originalFilePath);
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

    // to create a copy of the original CSV file with an additional column
    public static void CreateCopyWithTwoNewColumn()
    {

        try
        {
            string[] lines = File.ReadAllLines(originalFilePath);
            using (StreamWriter sw = new StreamWriter(resultFilePath))
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

    // Method to modify the copied CSV file with user choices and fixation values
    public static void ModifyCopyFile( TrialData trial, string userChoice)
    {
        try
        {
         
            List<string> lines = new List<string>(File.ReadAllLines(resultFilePath));
            // Index der aktuellen Zeile bestimmen
            int currentIndex = trials.IndexOf(trial) + 1; // Zeilenindex beginnt bei 1, da die erste Zeile die Headerzeile ist

            // Die entsprechende Zeile finden und aktualisieren
            if (currentIndex < lines.Count)
            {

                string fixationValue = trial.Fixation.ToString(); // Fixationswert

                // Die aktuelle Zeile aktualisieren, indem die 'response' und 'fixation' Werte hinzugefügt werden
                string updatedLine = $"{lines[currentIndex]},{userChoice},{fixationValue}";

                // Die aktualisierte Zeile in die Liste einfügen
                lines[currentIndex] = updatedLine;


                File.WriteAllLines(resultFilePath, lines.ToArray());
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
}
