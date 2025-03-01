using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AddressableSceneLoader : MonoBehaviour
{
    [Header("Addressable Settings")]
    [Tooltip("Key or label of the addressable scene to download and load")]
    public string sceneKey;

    [Header("UI Elements")]
    [Tooltip("TextMeshPro text element to display download progress")]
    public TMP_Text progressText;
    [Tooltip("Button to start the download")]
    public Button downloadButton;
    [Tooltip("Button to load the scene (enabled when download completes)")]
    public Button loadButton;
    [Tooltip("Button to clear the dependency cache for the scene")]
    public Button clearCacheButton;

    // Internal state
    private AsyncOperationHandle downloadHandle;
    private bool downloadCompleted = false;

    private void Start()
    {
        // Set initial UI states.
        loadButton.interactable = false;
        if (progressText != null)
            progressText.text = "Press Download to start.";

        // Hook up the button listeners.
        downloadButton.onClick.AddListener(OnDownloadButtonClicked);
        loadButton.onClick.AddListener(OnLoadButtonClicked);
        clearCacheButton.onClick.AddListener(OnClearCacheButtonClicked);
    }

    private void OnDestroy()
    {
        // Remove listeners to avoid potential memory leaks.
        downloadButton.onClick.RemoveListener(OnDownloadButtonClicked);
        loadButton.onClick.RemoveListener(OnLoadButtonClicked);
        clearCacheButton.onClick.RemoveListener(OnClearCacheButtonClicked);
    }

    private void OnDownloadButtonClicked()
    {
        // Check the download size for the specified scene key.
        AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(sceneKey);
        sizeHandle.Completed += sizeOp =>
        {
            long downloadSize = sizeOp.Result;
            if (downloadSize > 0)
            {
                // There is something to download.
                progressText.text = $"Downloading... (Size: {downloadSize} bytes)";
                // Start the download.
                downloadHandle = Addressables.DownloadDependenciesAsync(sceneKey, true);
                downloadHandle.Completed += OnDownloadCompleted;
                downloadButton.interactable = false;
            }
            else
            {
                // Assets are already cached.
                progressText.text = "Assets already cached.";
                downloadCompleted = true;
                loadButton.interactable = true;
                downloadButton.interactable = false;
            }
        };
    }


    private void Update()
    {
        if (downloadHandle.IsValid() && !downloadCompleted)
        {
            var downloadStatus = downloadHandle.GetDownloadStatus();
            float percent = downloadStatus.Percent;
            // Convert bytes to megabytes
            float downloadedMB = downloadStatus.DownloadedBytes / (1024f * 1024f);
            float totalMB = downloadStatus.TotalBytes / (1024f * 1024f);
            progressText.text = $"Downloading... {percent:P0}\n({downloadedMB:F2} MB / {totalMB:F2} MB)";
        }
    }



    private void OnDownloadCompleted(AsyncOperationHandle obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            downloadCompleted = true;
            progressText.text = "Download complete!";
            loadButton.interactable = true;
        }
        else
        {
            progressText.text = "Download failed. Please try again.";
            // Allow retrying the download.
            downloadButton.interactable = true;
        }
    }

    private void OnLoadButtonClicked()
    {
        // Load the scene via Addressables if the download is complete.
        if (downloadCompleted)
        {
            Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Single);
        }
    }

    private void OnClearCacheButtonClicked()
    {
        // Store the handle returned by ClearDependencyCacheAsync in a variable.
        AsyncOperationHandle clearCacheHandle = Addressables.ClearDependencyCacheAsync(sceneKey, true);
        clearCacheHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                progressText.text = "Cache cleared. You can download again.";
                // Reset UI states for a new download.
                downloadCompleted = false;
                loadButton.interactable = false;
                downloadButton.interactable = true;
            }
            else
            {
                progressText.text = "Failed to clear cache.";
            }
        };
    }

}
