using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int numberOfCoins;
    public TextMeshProUGUI coinCountText;
    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;
    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numberOfCoins = 0;
        Coin.OnCoinCollect += IncreaseCoinAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {   
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if(survivedLevelsCount != 1)
        {
            survivedText.text += "S";
        }
        Time.timeScale = 0;

    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    void LoadLevel(int level, bool wantSurvivedIncreas)
    {
        loadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);
        player.transform.position = new Vector3(29.1f, -9, 0);
        currentLevelIndex = level;
        numberOfCoins = 0;
        if(wantSurvivedIncreas) survivedLevelsCount++;
    }

    void Update()
    {
        DisplayCoinsNumText();
    }

    void IncreaseCoinAmount(int amount)
    {
        numberOfCoins += amount;
        if (numberOfCoins >= 5)
        {
            // you get a live point
            // Debug.Log("You got a live point");
            // TODO: display an extra life point on the screen
        }
    }

    private void DisplayCoinsNumText()
    {
        coinCountText.text = " : " + numberOfCoins.ToString();
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }

    // New method to be called when the player reaches the level end
    public void CompleteLevel()
    {
        loadCanvas.SetActive(true);
        Debug.Log("Player reached level end!");
        // You could automatically transition after a delay, or keep your hold-to-load mechanism
    }
}