using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARDebugAssetChecker : MonoBehaviour
{
    [Header("AR Assets")]
    [Tooltip("The ARTrackedImageManager component in the scene")]
    public ARTrackedImageManager trackedImageManager;

    [Tooltip("Manually assigned XRReferenceImageLibrary (optional if already set in ARTrackedImageManager)")]
    public XRReferenceImageLibrary referenceImageLibrary;

    [Tooltip("The prefab to be instantiated on a tracked image")]
    public GameObject prefabToLoad;

    [Header("UI Elements")]
    [Tooltip("TMP text element to display asset status")]
    public TMP_Text debugTMPText;

    [Tooltip("UI Image to show overall status (green if OK, red if error)")]
    public Image debugImage;

    [Tooltip("Color to indicate assets are correctly assigned")]
    public Color goodColor = Color.green;

    [Tooltip("Color to indicate one or more assets are missing")]
    public Color badColor = Color.red;

    private void Start()
    {
        Debug.Log("This script exists");
        CheckAssets();
    }

    /// <summary>
    /// Checks if all required AR assets are assigned and updates UI elements accordingly.
    /// </summary>
    public void CheckAssets()
    {
        string status = "";
        bool allGood = true;

        if( debugTMPText == null)
        {
            debugTMPText = GetComponent<TMP_Text>();
        }

        // Check ARTrackedImageManager
        if (trackedImageManager == null)
        {
            status += "ARTrackedImageManager: MISSING\n";
            allGood = false;
        }
        else
        {
            status += "ARTrackedImageManager: OK\n";
            // Also verify that the ARTrackedImageManager has a reference library set up.
            if (trackedImageManager.referenceLibrary == null)
            {
                status += "  - Reference Library in ARTrackedImageManager: MISSING, Setting Reference Library Manually \n";

                trackedImageManager.referenceLibrary = referenceImageLibrary;
                /*
                if (trackedImageManager.referenceLibrary == null)
                {
                    status += "  - Reference Library in ARTrackedImageManager: STILL MISSING \n";
                    allGood = false;
                }
                else
                {
                    status += "  - Reference Library in ARTrackedImageManager: OK \n";
                }*/



            }
            else
            {
                status += "  - Reference Library in ARTrackedImageManager: OK\n";
            }
        }

        // Check XRReferenceImageLibrary if assigned separately
        if (referenceImageLibrary == null)
        {
            status += "XRReferenceImageLibrary (manual): MISSING\n";
            // Note: This might not be critical if the ARTrackedImageManager is already set.
        }
        else
        {
            status += "XRReferenceImageLibrary (manual): OK\n";
        }

        // Check the prefab to load
        if (prefabToLoad == null)
        {
            status += "Tracked Image Prefab: MISSING\n";
            allGood = false;
        }
        else
        {
            status += "Tracked Image Prefab: OK\n";
        }

        // Output the status to the TMP text element
        if (debugTMPText != null)
        {
            debugTMPText.text = status;
        }
        else
        {
            Debug.LogWarning("TMP Debug Text not assigned in the inspector.");
        }

        // Change the debug image color to indicate overall status
        if (debugImage != null)
        {
            debugImage.color = allGood ? goodColor : badColor;
        }
        else
        {
            Debug.LogWarning("Debug Image not assigned in the inspector.");
        }

        Debug.Log(status);
    }
}

