using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class mainMenuManage : MonoBehaviour {

	// ゲームに実装する広告格納変数
	private int selectAds;
	private bool adsFinished;


	// Use this for initialization
	void Start () {
		//レベルアップ処理
		if(variableManage.currentExp >= variableManage.nextExp){
			variableManage.currentLv += 1;
			Debug.Log (variableManage.currentLv);
		  //variableManage.currentExp  = variableManage.nextExp - variableManage.currentExp; // 誤植：引き算が逆
			variableManage.currentExp  = variableManage.currentExp - variableManage.nextExp ;
			variableManage.showLvupMes = true;
		}

		//レベルから次の必要経験値を計算
		variableManage.nextExp = variableManage.currentLv * 100;

		//広告選択 0-unityAds, 1-AppC
		selectAds = Random.Range(0,2);
		if(selectAds == 0){
			//UnityAds関連
			if(Advertisement.isSupported){
				if(!Advertisement.isInitialized){
					//広告が初期化されていない場合
					if(Application.platform == RuntimePlatform.Android){
						//android
						Advertisement.Initialize ("ゲームID");
					}else{
						//iOS
						Advertisement.Initialize ("ゲームID");
					}
				}
			}

			adsFinished = false;

		}

//		//appcが使えないので保留中
//		else{
//			if(variableManage.showAds){
//				//appc カットイン広告の表示
//				adsManage.appCCloud.Ad.ShowCutinView();
//				variableManage.showAds = false;
//			}
//		}



	}


	// Update is called once per frame
	void Update () {


//		//unity ads広告表示
//		if(Advertisement.isReady() && !adsFinished && variableManage.showAds){
//			Advertisement.Show(null, new ShowOptions {
//				pause = true,
//				resultCallback = result => {
//					//広告の再生終了後に実行される箇所
//					adsFinished = true;
//					variableManage.showAds = false;
//				}
//			});
//		}
	

	}
}
