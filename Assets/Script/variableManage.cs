using UnityEngine;
using System.Collections;

public class variableManage : MonoBehaviour {

	// 移動用変数
	static public int movingYaxis;
	static public int movingXaxis;

	// 攻撃用変数
	static public bool fireWeapon;
	static public GameObject lockonTarget;
	static public bool lockoned;

	// 機体データ用変数
	static public float currentHealth;

	// ほか
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

		// 試合開始直後に破損しないように０にしない
		currentHealth = 10f;

	}

}