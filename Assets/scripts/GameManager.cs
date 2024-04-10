using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Variables and references
    [SerializeField] GameObject leftBox, rightBox, draggableCube;
    [SerializeField] Material highlightMaterial;
    private Vector3 draggableCubeStartPosition;
    private GameObject userChoiceCube;
    [SerializeField] GameObject finishPnl; 
  
    private bool IsSelectionMade = false; 
    private const string PROBE_TAG = "Probe";
    #endregion
    // start method to initialize draggable cube position and start running trials coroutine
    void Start()
    { 
        draggableCubeStartPosition = draggableCube.transform.position;
       
        StartCoroutine(RunTrials());
    }
    // coroutine to run through trials, highlight object based on the data from CSV-File "trails.csv", then wait for user input, and record user choices in the trial data called "results.csv"
    IEnumerator RunTrials()
    {
       foreach(var trial in FileHandler.trials)
        {
            float soaInSeconds = trial.SOA / 100f;
            IsSelectionMade = false;
            GameObject probeBox = trial.isProbeLeft ? leftBox : rightBox;
            GameObject referenceBox = trial.isProbeLeft ? rightBox : leftBox;

            Renderer objRenderer = probeBox.GetComponent<Renderer>();
            Material originalMaterial = objRenderer.material;


            StartCoroutine(HighlightObject(probeBox, highlightMaterial));


            yield return new WaitForSeconds(trial.Fixation);
            if (soaInSeconds < 0)
            {

                yield return StartCoroutine(ToggleBoxVisibility(probeBox, 0.0333f));
                yield return new WaitForSeconds(soaInSeconds * -1);
                yield return StartCoroutine(ToggleBoxVisibility(referenceBox, 0.0333f));
            }
            else if (soaInSeconds > 0)
            {

                yield return StartCoroutine(ToggleBoxVisibility(referenceBox, 0.0333f));
                yield return new WaitForSeconds(soaInSeconds);
                yield return StartCoroutine(ToggleBoxVisibility(probeBox, 0.0333f));

            }
            else
            {
                yield return StartCoroutine(ToggleAllCubesVisibility( 0.0333f));
            }

            yield return new WaitUntil(() => IsSelectionMade);

            string userChoice = GetUserChoice(userChoiceCube);

            ResetValues( objRenderer, originalMaterial);

            FileHandler.ModifyCopyFile(trial, userChoice);

        }
        finishPnl.SetActive(true);
        ScaleUpUIPanel(finishPnl.GetComponent<RectTransform>(), 0.75f, 1);
    }

    // reset the position of the cube and the material of the boxes after each trial
    private void ResetValues( Renderer highlightedObjectMat, Material originalMaterial)
    {
        draggableCube.transform.position = draggableCubeStartPosition;
        highlightedObjectMat.material = originalMaterial;
        rightBox.tag = "Untagged";
        leftBox.tag = "Untagged";
    }

    // Get user choice (Probe or Reference) based on selected object's tag
    private string GetUserChoice(GameObject ChoosenObject)
    {
        string userChoice;
        if (ChoosenObject.tag == PROBE_TAG)
            userChoice = "Probe";
        else
            userChoice = "Reference";
        return userChoice;
    }

    // Register user's selection of an object, the method should be called from the script "DraggableCube"
    public void RegisterUserSelection(GameObject selectedObject)
    {
        userChoiceCube = selectedObject;
        IsSelectionMade = true;
    }

    // this Coroutine to toggle the visibility of a box with a delay 
    IEnumerator ToggleBoxVisibility(GameObject box, float delay)
    {
        box.SetActive(false);
        yield return new WaitForSeconds(delay);
        box.SetActive(true);
    }

    // this Coroutine to toggle the visibility of both boxes with a delay 
    IEnumerator ToggleAllCubesVisibility( float delay)
    {
        rightBox.SetActive(false);
        leftBox.SetActive(false);
        yield return new WaitForSeconds(delay);
        rightBox.SetActive(true);
        leftBox.SetActive(true);

    }

    // this coroutine to highlight an object with a specified material "green color"
    IEnumerator HighlightObject(GameObject obj, Material highlightMaterial)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        objRenderer.material = highlightMaterial;
        obj.tag = PROBE_TAG;
        yield return new WaitForSeconds(0.1f);
    }

    // Method to scale up a UI panel with specified duration and scale multiplier
    public void ScaleUpUIPanel(RectTransform panel, float duration, float scaleMultiplier)
    {
        panel.localScale = Vector3.zero;
        panel.DOScale(1, duration).SetEase(Ease.InOutElastic);
    }
}
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


