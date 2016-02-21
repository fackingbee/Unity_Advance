using UnityEngine;
using System.Collections;

public class gameManage : MonoBehaviour {

	void Start () {
		
	// 初期設定
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//PhotonRealtimeのサーバーへ接続、ロビーへ入室
		PhotonNetwork.ConnectUsingSettings(null);
	}

	void Update () {
	
	}

	//ロビーに入室した
	void OnJoinedLobby(){
		//どこかのルームへ入室する
		PhotonNetwork.JoinRandomRoom ();
	}

	//ロビーへの入室が失敗
	void OnFailedToConnectToPhoton(){
		Application.LoadLevel ("mainMenu");
	}

	//ルームの入室に失敗
	void OnPhotonRandomJoinFailed(){
		//自分でルームを作成して入室
		PhotonNetwork.CreateRoom (null);
	}

	//ルームへ入室した
	void OnJoinedRoom(){
			// オブジェクトを読み込み
		GameObject myPlayer = PhotonNetwork.Instantiate("character/t01", new Vector3(440f,30f,-560f),Quaternion.identity,0);
	}

	// Photon Realtimeとの接続あ切断された場合
	void OnConnectionFail(){
		Application.LoadLevel ("mainMenu");
	}

	void OnApplicationPause(bool pauseStatus){
		if (pauseStatus) {
			PhotonNetwork.Disconnect ();
		} else {
			Application.LoadLevel ("mainMenu");
		}
	}

}
