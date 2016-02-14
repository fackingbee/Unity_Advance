using UnityEngine;
using System.Collections;

public class characterMove : MonoBehaviour {

	// 機体のパラメータ設定
	public float maxSpd;
	public float cornering;
	public float basePower;
	// オブジェクト格納
	public Rigidbody myRigid;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// PC動作確認
		if(!Application.isMobilePlatform){
			if(Input.GetKey(KeyCode.W)){
				variableManage.movingYaxis = 1;
			}else if(Input.GetKey(KeyCode.S)){
				variableManage.movingYaxis = -1;
			}else{
				variableManage.movingYaxis = 0;
			}
			if(Input.GetKey(KeyCode.A)){
				variableManage.movingXaxis = 1;
			}else if(Input.GetKey(KeyCode.D)){
				variableManage.movingXaxis = -1;
			}else{
				variableManage.movingXaxis = 0;
			}
		}
	
	}
		
	void FixedUpdate(){
		// 移動処理
		if(variableManage.movingYaxis != 0){
			if(myRigid.velocity.magnitude < maxSpd){
				myRigid.AddForce (transform.TransformDirection (Vector3.forward) * basePower * 11f * variableManage.movingYaxis);
			}
			// 旋回処理
			if (myRigid.angularVelocity.magnitude < (myRigid.velocity.magnitude * 0.3f)) {
				myRigid.AddTorque (transform.TransformDirection (Vector3.up) * cornering * variableManage.movingXaxis * -90f);
			} else {
				myRigid.angularVelocity = (myRigid.velocity.magnitude * 0.3f) * myRigid.angularVelocity.normalized;
			}
		}
	}
}
