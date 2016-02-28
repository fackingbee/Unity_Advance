using UnityEngine;
using System.Collections;

public class gameManage : MonoBehaviour {

	// Photon用変数定義
	ExitGames.Client.Photon.Hashtable myPlayerHash;
	ExitGames.Client.Photon.Hashtable myRoomrHash;
	string[] roomProps = { "time" };
	private PhotonView scenePV;

	// 敵味方判別 //
	public int myTeamID;
	private float tagTimer;

	// スタート地点 //
	private Vector2 myStartPos;

	//勝敗情報用
	private int tc1tmp;
	private int tc2tmp;
	private float bc1tmp;
	private float bc2tmp;
	private bool sendOnce;


	// ほか
	private bool loadOnce;

	void Start () {
		
	// 初期設定
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		myTeamID = 0;
		loadOnce = false;
		scenePV = PhotonView.Get (this.gameObject);
		tagTimer = 0f;

		sendOnce = false;
		tc1tmp = variableManage.team1Rest;
		tc2tmp = variableManage.team2Rest;
		bc1tmp = variableManage.base1Rest;
		bc2tmp = variableManage.base2Rest;

	// PhotonRealtimeのサーバーへ接続、ロビーへ入室
		PhotonNetwork.ConnectUsingSettings(null);

	// スタート地点計算
		Vector2 rndPos = Vector2.zero;
		while(true){
			rndPos = Random.insideUnitCircle * 150f;
			if(rndPos.x < -20f){
				if(rndPos.y >20f){
					break;
				}
			}
		}

		myStartPos = new Vector2 ( (592f + rndPos.x), (-592 + rndPos.y) );

	}

	void Update () {
		// ルームに入室が完了していたら
		if (PhotonNetwork.inRoom){
			// チーム分けが完了したらオブジェクトを読み込み
			if(!loadOnce && myTeamID != 0){
				
				loadOnce = true;

				if (myTeamID == 2){
					myStartPos = myStartPos * -1.0f;
				}

				GameObject myPlayer = 
					PhotonNetwork.Instantiate ("character/t01",
						new Vector3(myStartPos.x , 24f, myStartPos.y),
						Quaternion.identity, 0);
				myPlayer.transform.LookAt (Vector3.zero);
				variableManage.myTeamID = myTeamID;
			}

			// 3sに一回タグ付け
			tagTimer += Time.deltaTime;
			if (tagTimer > 3.0f){
				giveEnemyFlag();
				tagTimer = 0f;
			}
			// 自分がマスタークライアントなら、変更があった際に情報送信
			if(PhotonNetwork.isMasterClient){
				if((variableManage.team1Rest != tc1tmp)
				|| (variableManage.team2Rest != tc2tmp)
				|| (variableManage.base1Rest != bc1tmp)
				|| (variableManage.base2Rest != bc2tmp)
				){
					scenePV.RPC ("sendCurrentStatus",
						PhotonTargets.All,
						tc1tmp,
						tc2tmp,
						bc1tmp,
						bc2tmp);
				}
			}

			// 撃破されたとき、マスタークライアントへ情報を送信
			if(variableManage.currentHealth <= 0f){
				if (!sendOnce) {
					sendOnce = true;
					scenePV.RPC ("sendDestruction",
						PhotonNetwork.masterClient,
						variableManage.myTeamID);
				}
			}else{
				sendOnce = false;
			}
				// 勝敗を確定
				if (PhotonNetwork.isMasterClient && !variableManage.finishedGame){
					if (variableManage.team1Rest <= 0 ||
					    variableManage.base1Rest <= 0f) {
						variableManage.finishedGame = true;
						variableManage.gameResult = 2;
						scenePV.RPC (
							"syncFinished",
							PhotonTargets.Others,
							variableManage.gameResult
						);
					} else if (variableManage.team2Rest <= 0 ||
					           variableManage.base2Rest <= 0f) {
						variableManage.finishedGame = true;
						variableManage.gameResult = 1;
						scenePV.RPC (
							"syncFinished",
							PhotonTargets.Others,
							variableManage.gameResult
						);
							
				}
			}
		}
	}

	// ロビーに入室した
	void OnJoinedLobby(){
		//どこかのルームへ入室する
		PhotonNetwork.JoinRandomRoom ();
	}

	// ロビーへの入室が失敗
	void OnFailedToConnectToPhoton(){
		Application.LoadLevel ("mainMenu");
	}

	// ルームの入室に失敗
	void OnPhotonRandomJoinFailed(){
		//自分でルームを作成
		//PhotonNetwork.CreateRoom (null);
		myRoomrHash = new ExitGames.Client.Photon.Hashtable();
		myRoomrHash.Add ("time", "0");
		PhotonNetwork.CreateRoom (Random.Range(1.0f, 100f).ToString(), true, true, 8, myRoomrHash, roomProps);
		myTeamID = 1;
	}

	// 無事にルームへ入室
	void OnJoinedRoom(){
			// オブジェクトを読み込み
		//GameObject myPlayer = PhotonNetwork.Instantiate("character/t01", new Vector3(440f,30f,-560f),Quaternion.identity,0);
	}

	// Photon Realtimeとの接続あ切断された場合
	void OnConnectionFail(){
		Application.LoadLevel ("mainMenu");
	}

	// ルームに誰かベスのプレイヤーが入室したとき //
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		//自分がマスタークライアントだったとき
		if (PhotonNetwork.isMasterClient){
			// メンバー振り分け処理
			int allocateNumber = 0;
			// 現在のチーム状況を取得
			GameObject[] team1Players = GameObject.FindGameObjectsWithTag("Player");
			GameObject[] team2Players = GameObject.FindGameObjectsWithTag("enemy");
			if ( (team1Players.Length - 1) >= team2Players.Length ){
				// Payerの方が多い場合 //
				if (myTeamID == 1) {
					allocateNumber = 2;
				}else{
					allocateNumber = 1;
				}
				scenePV.RPC ("allocateTeam", newPlayer, allocateNumber);
			}else{
				// enemyの方が多い場合
				if (myTeamID == 2) {
					allocateNumber = 2;
				}else{
					allocateNumber = 1;
				}
				scenePV.RPC ("allocateTeam", newPlayer, allocateNumber);
			}
			// 現在の戦況を送信
			scenePV.RPC("sendCurrentStatus",
				newPlayer,
				variableManage.team1Rest,
				variableManage.team2Rest,
				variableManage.base1Rest,
				variableManage.base2Rest);
		}
	}

	void OnApplicationPause(bool pauseStatus){
		if (pauseStatus) {
			PhotonNetwork.Disconnect();
		}else{
			Application.LoadLevel ("mainMenu");
		}
	}

	// 敵に対してタグ付けを行う //
	void giveEnemyFlag(){
		// チームIDが定義されていたら
		if (myTeamID != 0 ){
			int enID = 1;
			
			if (myTeamID == 1 ){
					enID = 2;
			}

			GameObject[] allFriendPlayer = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject player in allFriendPlayer ){
				int id = player.GetComponent<characterStatus>().myTeamID;
				if (id == enID){
					player.tag = "enemy";
				}
			}
		}
	}

	// 自分が撃破されたことを送信するRPC
	[RPC]
	void sendDestruction(int tID){
		if (tID == 1) {
			tc1tmp -= 1;
			if (tc1tmp < 0) {
				tc1tmp = 0;
			}

		} else {
			tc2tmp -= 1;
			if(tc2tmp < 0){
				tc2tmp = 0;
			}
		}
	}

	// 現在お対戦状況を送信するRPC
	[RPC]
	void sendCurrentStatus(int tc1, int tc2, float bc1, float bc2){
		variableManage.team1Rest = tc1;
		variableManage.team2Rest = tc2;
		variableManage.base1Rest = bc1;
		variableManage.base1Rest = bc2;
	}

	// ゲーム終了を通知するRPC
	[RPC]
	void syncFinished(int winID){
		variableManage.finishedGame = true;
		variableManage.gameResult = winID;
	}

	[RPC]
	void allocateTeam(int teamID){
		if (myTeamID == 0){
			myTeamID = teamID;
		}
	}
}

//
