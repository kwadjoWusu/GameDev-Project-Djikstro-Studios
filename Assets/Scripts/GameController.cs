using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    int numberOfCoins;
    public TextMeshProUGUI coinCountText;
    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numberOfCoins = 0;
        Coin.OnCoinCollect += IncreaseCoinAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        loadCanvas.SetActive(false);
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
        loadCanvas.SetActive(false);
        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[nextLevelIndex].gameObject.SetActive(true);
        player.transform.position = new Vector3(29.1f, -9, 0);
        currentLevelIndex = nextLevelIndex;
        numberOfCoins = 0;
    }

    // New method to be called when the player reaches the level end
    public void CompleteLevel()
    {
        loadCanvas.SetActive(true);
        Debug.Log("Player reached level end!");
        // You could automatically transition after a delay, or keep your hold-to-load mechanism
    }
}