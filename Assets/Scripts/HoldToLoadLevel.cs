
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1.0f;
    public Image fillCircle;
    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            if (holdTimer >= holdDuration)
            {
                // TODO: Load level
                OnHoldComplete?.Invoke();
                ResetHold();
            }
        }


    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
            holdTimer = 0;
        }
        else if (context.canceled)
        {
            //Reset holding
            ResetHold();
        }
    }
    private void ResetHold()
{
    isHolding =false;
    holdTimer = 0;
    fillCircle.fillAmount = 0;
}}

