// Square.cs - VR Trigger-based Version
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Square : MonoBehaviour
{
    [Header("Grid Position")]
    public int row;
    public int col;

    [Header("References")]
    public TicTacToeManager manager;

    [Header("Interaction Settings")]
    public bool useActivateInsteadOfHover = true;

    // State tracking
    private bool isInteracting = false;
    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;

    // XR Input devices
    private InputDevice rightController;
    private InputDevice leftController;

    // XR Interactable component
    private XRSimpleInteractable interactable;

    void Start()
    {
        // Find manager if not assigned
        if (manager == null)
            manager = FindObjectOfType<TicTacToeManager>();

        // Get the XR Simple Interactable component
        interactable = GetComponent<XRSimpleInteractable>();

        // Setup interaction events based on preference
        if (useActivateInsteadOfHover)
        {
            // Use activate events (more precise for VR)
            if (interactable != null)
            {
                interactable.activated.AddListener(OnActivated);
                interactable.deactivated.AddListener(OnDeactivated);
            }
        }
        else
        {
            // Use hover events
            if (interactable != null)
            {
                interactable.hoverEntered.AddListener(OnHoverEntered);
                interactable.hoverExited.AddListener(OnHoverExited);
            }
        }

        // Get XR input devices
        RefreshControllers();
    }

    void Update()
    {
        // Refresh controllers periodically in case they disconnect/reconnect
        if (Time.frameCount % 60 == 0) // Every 60 frames
        {
            RefreshControllers();
        }

        // Only check for input when interacting with this square
        if (!isInteracting) return;

        // Get current trigger states from both controllers
        bool currentLeftTrigger = false;
        bool currentRightTrigger = false;

        // Check left controller
        if (leftController.isValid)
        {
            leftController.TryGetFeatureValue(CommonUsages.triggerButton, out currentLeftTrigger);
        }

        // Check right controller
        if (rightController.isValid)
        {
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out currentRightTrigger);
        }

        // Left trigger pressed (place X)
        if (currentLeftTrigger && !leftTriggerPressed)
        {
            Debug.Log($"Left trigger pressed on square [{row}, {col}] - Placing X");
            manager.MakeMoveWithTrigger(row, col, transform.position, true); // true = left trigger = X
        }

        // Right trigger pressed (place O)
        if (currentRightTrigger && !rightTriggerPressed)
        {
            Debug.Log($"Right trigger pressed on square [{row}, {col}] - Placing O");
            manager.MakeMoveWithTrigger(row, col, transform.position, false); // false = right trigger = O
        }

        // Update trigger states for debouncing
        leftTriggerPressed = currentLeftTrigger;
        rightTriggerPressed = currentRightTrigger;
    }

    private void RefreshControllers()
    {
        var inputDevices = new List<InputDevice>();

        // Get right controller
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
        if (inputDevices.Count > 0)
            rightController = inputDevices[0];

        // Get left controller
        inputDevices.Clear();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        if (inputDevices.Count > 0)
            leftController = inputDevices[0];
    }

    // Activate-based interaction (recommended for VR)
    public void OnActivated(ActivateEventArgs args)
    {
        isInteracting = true;
        Debug.Log($"Activated square [{row}, {col}] - Ready for trigger input");
    }

    public void OnDeactivated(DeactivateEventArgs args)
    {
        isInteracting = false;
        Debug.Log($"Deactivated square [{row}, {col}]");

        // Reset trigger states when deactivating
        leftTriggerPressed = false;
        rightTriggerPressed = false;
    }

    // Hover-based interaction (alternative)
    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        isInteracting = true;
        Debug.Log($"Hovering over square [{row}, {col}] - Ready for trigger input");
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        isInteracting = false;
        Debug.Log($"Stopped hovering over square [{row}, {col}]");

        // Reset trigger states when hover exits
        leftTriggerPressed = false;
        rightTriggerPressed = false;
    }

    void OnDestroy()
    {
        // Clean up event listeners
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnActivated);
            interactable.deactivated.RemoveListener(OnDeactivated);
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }
    }
}