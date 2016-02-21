using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIbattle : MonoBehaviour {
	
	//テキスト格納
	public Text infoText;
	public Text timerText;
	public Text wepCurrentText;
	public Text wepStandbyText;
	public Text redTeamText;
	public Text blueTeamText;
	public Text healthText;
	public Text returnText;
	//オブジェクト格納
	public GameObject returnMenu;
	public GameObject mapUIobj;
	// 仮想操作パッド関連 //
	private float currentXpos;
	private float currentYpos;
	private float startXpos;
	private float startYpos;
	private bool touchStart;

	void Start () {
		currentXpos = 0f;
		currentYpos = 0f;
		touchStart = false;
	}

	void Update () {
	// 仮想操作パッド
		// 位置取得関連 //
		for(int i = 0; i < Input.touchCount; i++){
			// 画面の左下に指があるか判定 //
			if (Input.GetTouch (i).position.x < (Screen.width / 2.5f)) {
				if (Input.GetTouch (i).position.y < (Screen.height / 2.0f)) {
					// 指があった場合、座標を格納 //
					currentXpos = Input.GetTouch (i).position.x;
					currentYpos = Input.GetTouch (i).position.y;
					if (!touchStart) {
						// タッチした瞬間の座標を保存 //
						startXpos = currentXpos;
						startYpos = currentYpos;
						touchStart = true;
					}
				}
			}
		}
	if(Input.touchCount == 0){
			// 画面に指が触れていないときは座標を初期化 //
			currentXpos = 0f;
			currentYpos = 0f;
			startXpos = 0f;
			startYpos = 0f;
			touchStart = false;
		}
		// モバイル時のみ動作
		if (Application.isMobilePlatform) {


			// 移動量計算 - X軸
			if ((startXpos - currentXpos) < (Screen.width * -0.05f)) {
				// 右を入力
				variableManage.movingXaxis = 1;
			} else if ((startXpos - currentXpos) > (Screen.width * 0.05f)) {
				// 左を入力
				variableManage.movingXaxis = -1;
			} else {
				// 0入力
				variableManage.movingXaxis = 0;
			}
			// 移動量計算 - Y軸
			if ((startYpos - currentYpos) < (Screen.height * -0.08f)) {
				// 上を入力
				variableManage.movingYaxis = 1;
			} else if ((startYpos - currentYpos) > (Screen.height * 0.05f)) {
				// 下を入力
				variableManage.movingYaxis = -1;
			} else {
				// 0入力
				variableManage.movingYaxis = 0;
			}
		}

		// 画面表示
		healthText.text = "HP:" + variableManage.currentHealth;


//		// デバック用 後で削除すること！
//		infoText.text = "X : " + variableManage.movingXaxis + "Y : " + variableManage.movingYaxis;

	}

	//コンフィグ表示用ボタン
	public void configToggle(){
		if(returnMenu.GetActive()){
			returnMenu.SetActive(false);
		}else{
			returnMenu.SetActive(true);
		}
	}

	//メインメニューへ戻る
	public void returnMainMenu(){
		Application.LoadLevel("mainMenu");
	}

	// 武器発射ボタン
	public void fireWep(){
		variableManage.fireWeapon = true;
	}

}
