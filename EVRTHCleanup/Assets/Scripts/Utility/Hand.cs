using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// Simple script to interact with the VRButton script. 
/// All uGUI objects that you want to be able to interact with need to be on World Canvases and have colliders
/// Either the uGUI elements or this object needs to have a collider that is marked as a trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hand : MonoBehaviour
{
    public VRNode vrNode;

    private new Transform transform;

    private VRButton isTouchingVrButton;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        InputTracking.Recenter();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localPosition = InputTracking.GetLocalPosition(vrNode);
        transform.localRotation = InputTracking.GetLocalRotation(vrNode);

        if (Input.GetButtonDown("Submit") && isTouchingVrButton)
        {
            isTouchingVrButton.Click(transform.position);
            print("Clicked");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VRButton>())
        {
            isTouchingVrButton = other.GetComponent<VRButton>();
            isTouchingVrButton.PointerEnter(transform.position);
            print("Touching " + isTouchingVrButton.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isTouchingVrButton && isTouchingVrButton.gameObject.Equals(other.gameObject))
        {
            isTouchingVrButton.PointerExit(transform.position);
            isTouchingVrButton = null;
        }
    }
}
