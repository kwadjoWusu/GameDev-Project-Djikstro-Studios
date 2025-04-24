using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Find the GameController and trigger level completion
            GameController gameController = FindAnyObjectByType<GameController>();
            if (gameController != null)
            {
                gameController.CompleteLevel();
            }
        }
    }
}