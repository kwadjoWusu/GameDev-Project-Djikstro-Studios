using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public Image backgroundImage;
    
    public Sprite[] backgrounds;
    public string[] storyTexts;
    
    public float displayTime = 5f;
    public string gameSceneName = "GameScene";
    
    public GameObject menuPanel; // Reference to the menu panel
    
    private int currentIndex = 0;
    private float timer;

    public AudioClip introMusic;
    private AudioManager audioManager;
    
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    if (audioManager != null && introMusic != null)
        audioManager.PlayMusic(introMusic);

        audioSource = GetComponent<AudioSource>();

        // Hide menu at start
        if (menuPanel != null)
            menuPanel.SetActive(false);
            
        // Display the first screen
        if (backgrounds.Length > 0)
            backgroundImage.sprite = backgrounds[0];
        
        if (storyTexts.Length > 0)
            storyText.text = storyTexts[0];
            
        timer = displayTime;
    }
    
    void Update()
    {
        // If menu is active, don't process intro controls
        if (menuPanel != null && menuPanel.activeSelf)
            return;
            
        // Count down timer
        timer -= Time.deltaTime;
        
        // Auto advance or manual advance
        if (timer <= 0 || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextScreen();
        }
        
        // Skip intro
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu();
        }
    }
    
    void NextScreen()
    {
        currentIndex++;
        
        // If we have more screens, show the next one
        if (currentIndex < backgrounds.Length && currentIndex < storyTexts.Length)
        {
            backgroundImage.sprite = backgrounds[currentIndex];
            storyText.text = storyTexts[currentIndex];
            timer = displayTime;
        }
        else
        {
            // No more screens, show menu
            ShowMenu();
        }
    }
    
    private AudioSource audioSource;



void ShowMenu()
{
    // Hide the intro elements
    storyText.gameObject.SetActive(false);
    
    // Show the menu
    if (menuPanel != null)
        menuPanel.SetActive(true);
        
    // Start fading out the music
    StartCoroutine(FadeOutMusic());

    // Optional: stop the music entirely
    if (audioManager != null)
        audioManager.StopMusic();
}

    IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / 2; // 2 second fade
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume for future use
    }
    
    // Called by the Start Game button
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Called by the Exit button
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}