using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI messageText;
    public GameObject restartButton;

    [Header("Game Settings")]
    public Health playerHealth;
    public float timeRemaining = 25f;
    public float deathDelay = 2.0f; // Delay for death animation

    private bool isGameOver = false;

    private void Start()
    {
        restartButton.SetActive(false);
        // Initialize the text immediately so it doesn't start empty
        messageText.text = timeRemaining.ToString("F0");
        messageText.alignment = TextAlignmentOptions.Left;
    }

    private void Update()
    {
        // Stop updating everything if the game is over
        if (isGameOver) return;

        // 1. Check Win Condition (Time runs out)
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            // --- THIS WAS MISSING ---
            // Update the UI every frame to show the countdown
            messageText.text = timeRemaining.ToString("F0");
        }
        else
        {
            EndGame(true);
        }

        // 2. Check Lose Condition (Player died)
        if (playerHealth.isDead)
        {
            // Start the delay sequence (wait for animation)
            StartCoroutine(HandleDeathSequence());
        }
    }

    private IEnumerator HandleDeathSequence()
    {
        isGameOver = true;

        // Clear the timer text so it doesn't freeze on "15" while they die
        messageText.text = "";

        // Wait for the animation to finish
        yield return new WaitForSeconds(deathDelay);

        // NOW show the Game Over screen
        EndGame(false);
    }

    private void EndGame(bool playerWon)
    {
        isGameOver = true;
        restartButton.SetActive(true);
        Time.timeScale = 0f; // Freeze game
        messageText.alignment = TextAlignmentOptions.Center;
        if (playerWon)
        {
            messageText.text = "YOU SURVIVED!";
            messageText.color = Color.green;
        }
        else
        {
            messageText.text = "GAME OVER";
            messageText.color = Color.red;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Unfreeze time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}