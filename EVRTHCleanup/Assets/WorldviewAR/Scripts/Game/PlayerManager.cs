using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Photon.PunBehaviour, IPunObservable {

    public static GameObject LocalPlayerInstance;

    public int score = 0;
    public string answer = "";
    public int state = 0;
    public bool synchronized = false;
    public Vector3 relativePosition;

    private Transform mapStage;
    private TextMesh playerText;

	void Awake () {
        mapStage = GameObject.FindWithTag("MapStage").transform;
        if(PhotonNetwork.connected && photonView.isMine){
            LocalPlayerInstance = gameObject;
            Destroy(transform.GetChild(0).gameObject);
        }
        else if(PhotonNetwork.connected && !photonView.isMine){
            playerText = GetComponentInChildren<TextMesh>();
            playerText.text = GetComponent<PhotonView>().owner.NickName;
        }
        if(!PhotonNetwork.connected){
            LocalPlayerInstance = gameObject;
            Destroy(transform.GetChild(0).gameObject);
        }
	}

	private void Update()
	{
        transform.position = Vector3.Lerp(transform.position, mapStage.transform.TransformPoint(relativePosition), Time.deltaTime * 10f);
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.isWriting){
            stream.SendNext(score);
            stream.SendNext(answer);
            stream.SendNext(state);
            stream.SendNext(synchronized);
            stream.SendNext(mapStage.InverseTransformPoint(Camera.main.transform.position));
        }
        else{
            score = (int)stream.ReceiveNext();
            answer = (string)stream.ReceiveNext();
            state = (int)stream.ReceiveNext();
            synchronized = (bool)stream.ReceiveNext();
            relativePosition = (Vector3)stream.ReceiveNext();
        }
    }
}
