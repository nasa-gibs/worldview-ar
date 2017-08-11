using UnityEngine;
using System.Collections;
using UnityEngine.VR;

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
