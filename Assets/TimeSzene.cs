using UnityEngine;
using System.Collections;

public class SceneDisplay : MonoBehaviour
{    
    public GameObject referenceBox;
    public int framesToShow = 2;
     void Start()
    {
        // Wähle eine zufällige Zeit zwischen 1 und 10 Sekunden
         float randomSeconds = Random.Range(0.3f, 0.5f);
        StartCoroutine(DisplaySceneForSeconds(randomSeconds));
        Debug.Log("Die Szene wurde für " + randomSeconds + " Sekunden angezeigt.");
         StartCoroutine(ShowReferenceBox());
    }


    IEnumerator DisplaySceneForSeconds(float seconds)
    {
        // Warte für die angegebene Zeit
        yield return new WaitForSeconds(seconds);
        Debug.Log("Die Szene wurde beendet.");
        Application.Quit();
    }
     IEnumerator ShowReferenceBox()
    {
        // Aktiviere die Reference Box
        referenceBox.SetActive(true);

        // Warte für die angegebene Anzahl von Frames
        for (int i = 0; i < framesToShow; i++)
        {
            yield return null; // Warte für einen Frame
        }

        // Deaktiviere die Reference Box
        referenceBox.SetActive(false);
    }
}