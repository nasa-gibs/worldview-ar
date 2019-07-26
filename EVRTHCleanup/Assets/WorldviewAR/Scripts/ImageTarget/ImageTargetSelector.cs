using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTargetSelector : MonoBehaviour {
    [SerializeField]
    private GameObject[] imageTargets;

	void Awake () {
        for (int i = 0; i < imageTargets.Length; i++){
            if(i == UserInformation.TargetIndex){
                transform.parent = imageTargets[UserInformation.TargetIndex].transform;
                transform.position = new Vector3(0, 0, 0); 
            }
            else{
                Destroy(imageTargets[i]);
            }
        }
	}
}
