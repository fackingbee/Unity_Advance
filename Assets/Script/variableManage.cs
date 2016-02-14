using UnityEngine;
using System.Collections;

public class variableManage : MonoBehaviour {

	// 移動用変数
	static public int movingYaxis;
	static public int movingXaxis;

	void Start () {
		// 変数の初期化 //
		initializeVariable();
	}

	static public void initializeVariable(){
		movingXaxis = 0;
		movingYaxis = 0;
	}

}