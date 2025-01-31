using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class shrink : MonoBehaviour
{
    public float resetScale = 2.6f; // Scale at which the circle resets
    public float originalResetDuration = 0.6f; // Original reset duration in seconds (600ms)
    private float resetDuration; // Current reset duration
    private Vector3 originalScale; // Store the original size of the circle
    private float shrinkSpeed; // Calculated shrinking speed
    private float lastResetTime; // Time when the circle last reached reset scale
    private bool isPaused; // Tracks if the shrinking process is paused

    public Text counterText; // Reference to the UI Text element
    private int successfulPressCount = 0; // Counter for successful spacebar presses

    public GameObject square; // Reference to the Square GameObject
    private float squareInitialScaleY; // Initial Y scale of the Square
    private float squareTargetScaleY = 10f; // Target Y scale of the Square

    void Start()
    {
        // Store the original scale of the circle
        originalScale = transform.localScale;

        // Set initial reset duration to the original value
        resetDuration = originalResetDuration;

        // Calculate the initial shrink speed
        shrinkSpeed = (originalScale.x - resetScale) / resetDuration;

        // Initialize the counter display
        UpdateCounterText();

        // Store the initial Y scale of the Square
        if (square != null)
        {
            squareInitialScaleY = square.transform.localScale.y;
        }
    }

    void Update()
    {
        // If paused, do not execute shrinking logic
        if (isPaused)
            return;

        // Shrink the circle
        if (gameObject.name == "Circle")
        {
            // Calculate the shrink amount based on the shrink speed
            float shrinkAmount = shrinkSpeed * Time.deltaTime;
            transform.localScale -= new Vector3(shrinkAmount, shrinkAmount, shrinkAmount);

            // Check if the scale reaches or goes below the reset scale
            if (transform.localScale.x <= resetScale)
            {
                // Reset the circle to its original size
                transform.localScale = originalScale;

                // Record the time the circle reset to its original size
                lastResetTime = Time.time;

                // Recalculate shrink speed based on the reset duration
                shrinkSpeed = (originalScale.x - resetScale) / resetDuration;
            }
        }

        // Calculate time since the last reset
        float timeSinceLastReset = Time.time - lastResetTime;

        // Check for spacebar input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Check if space is pressed within the valid time window
            if (timeSinceLastReset >= 0.75f * resetDuration && timeSinceLastReset <= resetDuration)
            {
                // Spacebar pressed successfully
                resetDuration -= 0.05f; // Decrease reset duration by 50ms
                if (resetDuration < 0.1f) // Avoid going below a reasonable duration
                    resetDuration = 0.1f;

                // Increment the successful press counter
                successfulPressCount++;

                // Update the Square's Y scale
                UpdateSquareScale();

                // Update the counter display
                UpdateCounterText();
            }
            else
            {
                // Spacebar pressed outside the valid window
                StartCoroutine(HandleUnsuccessfulPress());
            }

            // Recalculate shrink speed based on the updated reset duration
            shrinkSpeed = (originalScale.x - resetScale) / resetDuration;
        }
    }

    // Updates the Square's Y scale based on successfulPressCount
    void UpdateSquareScale()
    {
        if (square != null)
        {
            float scaleFactor = (squareTargetScaleY - squareInitialScaleY) / 30f;
            float newYScale = squareInitialScaleY + scaleFactor * successfulPressCount;

            // Debugging output to confirm the scale change logic
            Debug.Log($"Updating Square Y Scale: {newYScale}");

            // Update only the Y scale of the Square
            square.transform.localScale = new Vector3(
                square.transform.localScale.x,
                Mathf.Clamp(newYScale, squareInitialScaleY, squareTargetScaleY), // Clamp to avoid exceeding the target
                square.transform.localScale.z
            );
        }
        else
        {
            Debug.LogError("Square GameObject is not assigned.");
        }
    }

    // Updates the counter display on the UI
    void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = "Successful Presses: " + successfulPressCount;
        }
    }

    // Handle behavior for an unsuccessful button press
    IEnumerator HandleUnsuccessfulPress()
    {
        // Pause shrinking and reset the circle to its original size
        isPaused = true;
        transform.localScale = originalScale;

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Reset resetDuration to the original value
        resetDuration = originalResetDuration;

        // Recalculate shrink speed
        shrinkSpeed = (originalScale.x - resetScale) / resetDuration;

        // Resume shrinking
        isPaused = false;
    }
}
