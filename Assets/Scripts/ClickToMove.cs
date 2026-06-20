using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject moveMark;
    private NavMeshAgent agent;
    private PlayerControls controls;
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Click.performed += OnClick;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Click.performed -= OnClick;
        controls.Disable();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);
            Instantiate(moveMark, hit.point, Quaternion.identity);
        }
    }
}