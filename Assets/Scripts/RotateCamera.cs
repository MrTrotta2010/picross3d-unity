using UnityEngine;
using System;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private GameObject puzzle = null;
    private Camera mainCamera = null;
    private Vector3 previousPosition;
    private Vector3 cameraDistance;
    private Vector3 puzzleOrigin;

    private void OnEnable()
    {
        PuzzleManager.PuzzleOriginChanged += RecenterCamera;
    }
    
    private void OnDisable()
    {
        PuzzleManager.PuzzleOriginChanged -= RecenterCamera;
    }

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        puzzleOrigin = new Vector3();

        cameraDistance = puzzleOrigin - mainCamera.transform.position;
        mainCamera.transform.position = puzzleOrigin;
        mainCamera.transform.Translate(new Vector3(0, 0, -cameraDistance.z));
    }

    void Update()
    {
        // Início do arrastar
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        // Enquanto o mouse é arrastado
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousPosition - mainCamera.ScreenToViewportPoint(Input.mousePosition);

            mainCamera.transform.position = puzzleOrigin;
            mainCamera.transform.Rotate(Vector3.right, direction.y * 180f);
            mainCamera.transform.Rotate(Vector3.up, -direction.x * 180f, Space.World);
            mainCamera.transform.Translate(new Vector3(0, 0, -cameraDistance.z));

            previousPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    private void RecenterCamera(Vector3 newOrigin)
    {
        puzzleOrigin = newOrigin;

        mainCamera.transform.position = puzzleOrigin;
        mainCamera.transform.Translate(new Vector3(0, 0, -cameraDistance.z));
    }
}
