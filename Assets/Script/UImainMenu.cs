using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UImainMenu : MonoBehaviour {

	//テキスト格納
	public Text playerStatusText;
	public Text battleStartBtn;
	public Text unlockText;
	public Text unlockBtn;
	public Text lvupNum;

	//オブジェクト関連
	public GameObject unlockBtnObj;
	public GameObject lvupObj;


	//レベルアップメッセージ用
	private float mesTimer;


	public GameObject[] machineObj = new GameObject[2];

//	private float getTimer;

	//機体選択用
	private int currentShowMachineNum;

	void Start () {
		
		mesTimer = 0f;



//  //appc cloud停止中につき、コメントアウト中↓(2016.03.19)
//		getTimer = 0f;


		//選択マシンをリセット
		currentShowMachineNum = 1;
		variableManage.mySelectMachine = 1;
		machineObj [0].SetActive (true);
		machineObj[1].SetActive(false);


	}

	void Update () {
		//画面表示
		lvupNum.text = variableManage.currentLv.ToString();
		playerStatusText.text = 
			"PlayerClass :" + variableManage.currentLv +
		" NextClass " + variableManage.currentExp +
		" / " + variableManage.nextExp;

		//レベルアップメッセージ
		if(variableManage.showLvupMes){
			if (mesTimer == 0f) {
				lvupObj.SetActive (true);
			} else if (mesTimer > 3.0f){
				mesTimer = 0f;
				variableManage.showLvupMes = false;
				lvupObj.SetActive (false);
			}
			mesTimer += Time.deltaTime;
		}



//		// マシン選択関連
		if(currentShowMachineNum == 1){
			//マシン１を表示中
			machineObj[0].SetActive(true);
			machineObj[1].SetActive(false);
			variableManage.mySelectMachine = 1;
			unlockBtnObj.SetActive(false);
		}else if(currentShowMachineNum == 2){
			//マシン２を表示中
			machineObj[0].SetActive(false);
			machineObj[1].SetActive(true);
			if(variableManage.openMachine02 || variableManage.currentLv >= 5){
				variableManage.mySelectMachine = 2;
				unlockBtnObj.SetActive(false);
			}else{
				unlockBtnObj.SetActive(true);
				unlockText.text = 
					"この機体はロックされています\nプレイヤークラス05で解除";
			}
		}


// 		// appc cloud停止中につき、コメントアウト中↓(2016.03.19)
//		// 購入状況を取得し、反映
//		getTimer += Time.deltaTime;
//		if(getTimer > 0.5f){
//			if(adsManage.appCCloud.Purchase.GetItemCount("OPEN2") == 1){
//				variableManage.openMachine02 = true;
//			}
//		}



	}

	public void jumpBattleScene(){
		variableManage.initializeVariable();
		//Debug.Log ("敵減");
		Application.LoadLevel("battle");
	}



	// 右矢印ボタン
	public void selectRightBtn(){
		currentShowMachineNum += 1;
		if(currentShowMachineNum > 2){
			currentShowMachineNum = 2;
		}
	}


	//左矢印ボタン
	public void selectLeftBtn(){
		currentShowMachineNum -= 1;
		if(currentShowMachineNum < 1){
			currentShowMachineNum = 1;
//			Debug.Log (currentShowMachineNum);
		}
	}
//
//	//強制解除ボタン
//	public void forceMachineUnlock(){
//		//購入画面を開く
//		adsManage.appCCloud.Purchase.OpenPurchaseView();
//	}



}
