using UnityEngine;
using System.Collections;

public class variableManage : MonoBehaviour {

	//移動用変数
	static public int movingYaxis;
	static public int movingXaxis;

	//攻撃用変数
	static public bool fireWeapon;
	static public GameObject lockonTarget;
	static public bool lockoned;

	//機体データ用変数
	static public float currentHealth;

	//ゲーム管理用変数
	static public int myTeamID;
	static public bool mapEnabled;
	static public GameObject team1baseBullet;
	static public GameObject team2baseBullet;
	static public float startTime;

	//勝敗用変数
	static public bool finishedGame;//勝敗あ確定されたか
	static public int team1Rest;//チーム１の残り撃破数
	static public int team2Rest;// チーム２の残り撃破数
	static public float base1Rest;// チーム１の拠点の残りHP
	static public float base2Rest;// チーム２の拠点の残りHP
	static public float timeRest;//ゲームの残り時間
	static public int gameResult; // 1-teamID=1の勝利、２−teamID＝２の勝利

	//画面表示用変数
	static public int informationMessage;

	//プレイヤー情報
	static public int  currentExp = 0;
	static public int  nextExp = 100;
	static public int  currentLv = 1;
	static public bool showLvupMes   = false;
	static public bool openMachine02 = false;
	static public bool openMachine03 = false;
	static public int  myWP = 0;



	//ほか
	static public bool controlLock;

	void Start () {
		// 変数の初期化 //
		initializeVariable();
	}

	static public void initializeVariable(){
		movingXaxis = 0;
		movingYaxis = 0;
		fireWeapon = false;
		lockoned = false;
		controlLock = false;
		myTeamID = 0;
		mapEnabled = false;
		informationMessage = 0;

		// 試合開始直後に破損しないように０にしない
		currentHealth = 10f;

		// 勝敗用
		finishedGame = false;
		team1Rest = 20;
		team2Rest = 20;
		base1Rest = 9999f;
		base2Rest = 9999f;
		timeRest = 400f;
		gameResult = 0;
	}
}
