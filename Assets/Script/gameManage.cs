using UnityEngine;
using System.Collections;

public class gameManage : MonoBehaviour {

	//Photon用変数定義
	ExitGames.Client.Photon.Hashtable myPlayerHash;
	ExitGames.Client.Photon.Hashtable myRoomHash;
	string[] roomProps = { "time","lv","wp" };
	private PhotonView scenePV;

	//敵味方判別 //
	public int    myTeamID;
	private float tagTimer;

	//スタート地点 //
	private Vector2 myStartPos;

	//勝敗情報用
	private int   tc1tmp;
	private int   tc2tmp;
	private float bc1tmp;
	private float bc2tmp;
	private bool  sendOnce;

	private string standardTime;
	private string serverTime;
	private bool   countStart;

	//ほか
	private bool  loadOnce;
	private float shiftTimer;

	void Start () {
	//初期設定
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		myTeamID = 0;
		loadOnce = false;
		scenePV  = PhotonView.Get (this.gameObject);
		tagTimer = 0f;
		sendOnce = false;
		tc1tmp   = variableManage.team1Rest;
		tc2tmp   = variableManage.team2Rest;
		bc1tmp   = variableManage.base1Rest;
		bc2tmp   = variableManage.base2Rest;

		standardTime = "";
		serverTime   = "";
		countStart   = false;
		shiftTimer   = 0f;

	//PhotonRealtimeのサーバーへ接続、ロビーへ入室
		PhotonNetwork.ConnectUsingSettings(null);

	//スタート地点計算
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
			//ルームのカスタムプロパティへ基準時間を設定
			if(PhotonNetwork.isMasterClient && !countStart){
				myRoomHash["time"] = PhotonNetwork.time.ToString();
				PhotonNetwork.room.SetCustomProperties(myRoomHash);
				countStart = true;

				variableManage.startTime = variableManage.timeRest;

			}else if(!countStart){
				//ルームの基準時間を取得
				if(standardTime == "" && standardTime != "0"){
					standardTime = 
						PhotonNetwork.room.customProperties["time"].ToString();
				}
				//現在の基準時間を取得
				if(serverTime == "" && serverTime != "0"){
					serverTime = PhotonNetwork.time.ToString();
				}
				//時間を比較し、残り時間を算出
				if(standardTime != "" && standardTime != "0" && serverTime != "" && serverTime != "0"){
					float svT = float.Parse(double.Parse(serverTime).ToString());
					float stT = float.Parse(double.Parse(standardTime).ToString());
					Debug.Log(svT + "_" + stT);
					variableManage.timeRest = 
						variableManage.timeRest - Mathf.Round(svT - stT);
					countStart = true;
					variableManage.startTime = variableManage.timeRest;
				}
			}

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

			//3sに一回タグ付け
			tagTimer += Time.deltaTime;
			if (tagTimer > 3.0f){
				giveEnemyFlag();
				tagTimer = 0f;
			}
			//自分がマスタークライアントなら、変更があった際に情報送信
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

			//撃破されたとき、マスタークライアントへ情報を送信
			//撃破されたとき。全プレイヤーに情報を送信しメッセージを表示
			if(variableManage.currentHealth <= 0f){
				if (!sendOnce) {
					sendOnce = true;
					scenePV.RPC ("sendDestruction",
						PhotonNetwork.masterClient,
						variableManage.myTeamID);
					scenePV.RPC ("sendDestructionAll",
								PhotonTargets.All,
								variableManage.myTeamID);
				}
			}else{
				sendOnce = false;
			}

			// マスタークライアントで拠点が攻撃された場合、全クライアントへ送信
			if(PhotonNetwork.isMasterClient){
				// デバッグの為にユーザーが一人でもダメージが通るにしたければコメントアウト
				if(PhotonNetwork.room.playerCount > 1) {
					// 拠点1の耐久力を減らす
					if (variableManage.team1baseBullet != null) {
						bc1tmp -= variableManage.team1baseBullet.GetComponent<mainShell> ().pow;
						if (bc1tmp < 0) {
							bc1tmp = 0;
							//Debug.Log ("1のダメージ");
						}
						variableManage.team1baseBullet = null;
					}

					//拠点2の耐久力を減らす
					if (variableManage.team2baseBullet != null) {
						bc2tmp -= variableManage.team2baseBullet.GetComponent<mainShell> ().pow;
						if (bc2tmp < 0) {
							bc2tmp = 0;
						}
						variableManage.team2baseBullet = null;
					}
				} else {
					variableManage.team2baseBullet = null;
				}
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
					       variableManage.base2Rest <= 0f) 
				{
						   variableManage.finishedGame = true;
						   variableManage.gameResult = 1;
							scenePV.RPC ("syncFinished", PhotonTargets.Others, variableManage.gameResult);
				}


				//時間切れによる決着 より撃破数が多いほうが勝ち
				//ただし引き分けの場合は拠点の耐久力を参照し
				//それでもダメならチーム１の勝ち
				if(variableManage.timeRest <= 0){
					if(variableManage.team1Rest > variableManage.team2Rest){
						//t1 win
						variableManage.finishedGame = true;
						variableManage.gameResult = 1;
						scenePV.RPC(
							"syncFinished",
							PhotonTargets.Others,
							variableManage.gameResult);
					}else if(variableManage.team1Rest < variableManage.team2Rest){
						//t2 win
						variableManage.finishedGame = true;
						variableManage.gameResult = 2;
						scenePV.RPC(
							"syncFinished",
							PhotonTargets.Others,
							variableManage.gameResult);
					}else{
						//draw
						if(variableManage.base1Rest >= variableManage.base2Rest){
							variableManage.finishedGame = true;
							variableManage.gameResult = 1;
							scenePV.RPC(
								"syncFinished",
								PhotonTargets.Others,
								variableManage.gameResult);
						}else{
							variableManage.finishedGame = true;
							variableManage.gameResult = 2;
							scenePV.RPC(
								"syncFinished",
								PhotonTargets.Others,
								variableManage.gameResult);
						}
					}
				}
			}
			//時間経過
			if(countStart){
				variableManage.timeRest -= Time.deltaTime;
				if(variableManage.timeRest < 0){
					variableManage.timeRest = 0;
				}
			}

			//ゲーム開始5秒後、ルームを開放する
			if(variableManage.timeRest <395f){
				if(!PhotonNetwork.room.open){
					PhotonNetwork.room.open = true;
					PhotonNetwork.room.visible = true;
				}
			}
			//決着後、メインメニューへ移動
			if(variableManage.finishedGame && variableManage.gameResult != 0){
				//ルームを閉じる
				PhotonNetwork.room.open = false;
				PhotonNetwork.room.visible = false;

				shiftTimer += Time.deltaTime;

				//5秒後に移動
				if(shiftTimer > 5.0f){
					shiftTimer = 0f;

					//経験値計算
					if (variableManage.myTeamID == variableManage.gameResult) {
						// 自分のチームが勝利
						variableManage.currentExp += Mathf.RoundToInt (variableManage.startTime * 0.4f);
						if(variableManage.myWP < 5){
							variableManage.myWP += 1;
						}
					} else {
						//自分のチームが敗北
						variableManage.currentExp += Mathf.RoundToInt(variableManage.startTime * 0.15f);
						if (variableManage.myWP > 5) {
							variableManage.myWP -= 1;
						}
					}

					//データを保存してシーン移動
					bool svChk = KiiManage.saveKiiData();
					if(!svChk){
						svChk = KiiManage.saveKiiData ();
					}
					PhotonNetwork.Disconnect();

					// 対戦が終わったら広告表示する変数
					variableManage.showAds = true;

					Application.LoadLevel("mainMenu");
				}
			}
		}
	}

	// ロビーへ入室完了
	void OnJoinedLobby(){
		//どこかのルームへ入室する（Capter8で不要）
		//PhotonNetwork.JoinRandomRoom ();

		//プレイヤーのプロパティの設定
		myPlayerHash = new ExitGames.Client.Photon.Hashtable();
		myPlayerHash.Add ("lv", variableManage.currentLv);
		myPlayerHash.Add ("wp", variableManage.myWP);
		PhotonNetwork.SetPlayerCustomProperties (myPlayerHash);
		//入室処理
		betterRoomSearch();
	}

	//ロビーへの入室が失敗
	void OnFailedToConnectToPhoton(){
		Application.LoadLevel ("mainMenu");
	}

	//ルームの入室失敗
	void OnPhotonJoinRoomFailed(){
		betterRoomSearch ();
	}

	//void OnPhotonRandomJoinFailed(){
	//	自分でルームを作成
	//	PhotonNetwork.CreateRoom (null);
	//	myRoomHash = new ExitGames.Client.Photon.Hashtable();
	//	myRoomHash.Add ("time", "0");
	//	PhotonNetwork.CreateRoom (Random.Range(1.0f, 100f).ToString(), true, true, 8, myRoomHash, roomProps);
	//	myTeamID = 1;
	//}

	//無事にルームへ入室
	void OnJoinedRoom(){
			// オブジェクトを読み込み
		//GameObject myPlayer = PhotonNetwork.Instantiate("character/t01", new Vector3(440f,30f,-560f),Quaternion.identity,0);
	}

	//Photon Realtimeとの接続が切断された場合
	void OnConnectionFail(){
		Application.LoadLevel ("mainMenu");
	}

	//ルームに誰かベスのプレイヤーが入室したとき
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		//自分がマスタークライアントだったとき
		if (PhotonNetwork.isMasterClient){
			
			// メンバー振り分け処理
			int allocateNumber = 0;

			// 現在のチーム状況を取得
			GameObject[] team1Players = GameObject.FindGameObjectsWithTag("Player");
			GameObject[] team2Players = GameObject.FindGameObjectsWithTag("enemy");

			if ((team1Players.Length - 1) >= team2Players.Length) {
				// Payerの方が多い場合 //
				if (myTeamID == 1) {
					allocateNumber = 2;
				} else {
					allocateNumber = 1;
				}
				scenePV.RPC ("allocateTeam", newPlayer, allocateNumber);

			} else if ((team1Players.Length - 1) < team2Players.Length) {
				// enemyの方が多い場合
				if (myTeamID == 2) {
					allocateNumber = 2;
				} else {
					allocateNumber = 1;
				}
				scenePV.RPC ("allocateTeam", newPlayer, allocateNumber);
			}else{
				//同数だった場合
				float tmpRest = variableManage.team1Rest / variableManage.team2Rest;
				float tmpBass = variableManage.base1Rest / variableManage.base2Rest;
				float tmpValue = (tmpRest + tmpBass) * 0.5f;
				if (tmpRest < 1.0f) {
					//チーム1劣勢
					if (myTeamID == 2) {
						allocateNumber = 2;
					} else {
						allocateNumber = 1;
					}
					scenePV.RPC ("allocateTeam", newPlayer, allocateNumber);
				} else {
					//チーム2劣勢
					if (myTeamID == 1) {
						allocateNumber = 2;
					} else {
						allocateNumber = 1;
					}
					scenePV.RPC ("allocateTeam", newPlayer, allocateNumber);
				}

			}


			// 現在の戦況を送信
			scenePV.RPC("sendCurrentStatus",
				newPlayer,
				variableManage.team1Rest,
				variableManage.team2Rest,
				variableManage.base1Rest,
				variableManage.base2Rest);

			//ルームのプロパティを更新
			int apLv = 0;
			int apWp = 0;

			foreach(PhotonPlayer player in PhotonNetwork.playerList){
				apLv += (int)player.customProperties["lv"];
				apWp += (int)player.customProperties["wp"];
			}

			apLv = Mathf.RoundToInt (apLv / PhotonNetwork.room.playerCount);
			apWp = Mathf.RoundToInt (apWp / PhotonNetwork.room.playerCount);
			myRoomHash = PhotonNetwork.room.customProperties;

			myRoomHash ["lv"] = apLv;
			myRoomHash ["wp"] = apWp;

			PhotonNetwork.room.SetCustomProperties (myRoomHash);
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

	// ルームを検索し、よい部屋があれば入室
	void betterRoomSearch(){
		if(PhotonNetwork.countOfRooms > 1){
			//複数のルームがある場合、より自分のレベルと近い方へ入室
			int bLv = 99;
			int bWp = 10;
			string roomName = "";

			foreach(RoomInfo game in PhotonNetwork.GetRoomList()){
				// 取得したルームからカスタムプロパティを抜き出す
				int rWp = (int)game.customProperties["wp"];
				int rLv = (int)game.customProperties["lv"];
				if(rWp == variableManage.myWP){
					//勝敗スコアが同一であればレベルを比較
				if (rLv == variableManage.currentLv) {
						//レベルも同一なら入室確定
					roomName = game.name;
					break;
				} else {
					//レベルが異なる場合、最も差が少ないルームへ入室
					int tmpLv = Mathf.Abs(variableManage.currentLv - rLv);
					if(bLv >tmpLv){
						bLv = tmpLv;
						roomName = game.name;
					}
				}
			}else{
				//勝敗スコアが異なる　
					int tmpWp = Mathf.Abs(variableManage.myWP - rWp);
					if(bWp > tmpWp){
						bWp = tmpWp;
						roomName = game.name;
				}
			}
		}

		// 入室
			PhotonNetwork.JoinRoom(roomName);
		}else if(PhotonNetwork.countOfRooms == 1){
			//ルームが一つしか無い場合はそこに入室
			PhotonNetwork.JoinRandomRoom();
		}else{
			//ルームが存在しない場合は自分で作成
			myRoomHash = new ExitGames.Client.Photon.Hashtable();
			myRoomHash.Add ("time", "0");
			myRoomHash.Add ("lv", variableManage.currentLv);
			myRoomHash.Add ("wp", variableManage.myWP);
			PhotonNetwork.CreateRoom (Random.Range(1.0f, 100f).ToString(), false, false, 8, myRoomHash, roomProps);
			myTeamID = 1;
		}
	}


	//自分が撃破されたことを送信するRPC
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

	//自分が撃破された事を全プレイヤーに送信するRPC
	[RPC]
	void sendDestructionAll(int tID){
		if(myTeamID == tID){
			variableManage.informationMessage = 1;
		}else{
			variableManage.informationMessage = 2;
		}
	}

	//現在対戦状況を送信するRPC
	[RPC]
	void sendCurrentStatus(int tc1, int tc2, float bc1, float bc2){
		variableManage.team1Rest = tc1;
		variableManage.team2Rest = tc2;
		variableManage.base1Rest = bc1;
		variableManage.base2Rest = bc2;
	}

	//ゲーム終了を通知するRPC
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
