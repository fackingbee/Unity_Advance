 using UnityEngine;
using System.Collections;

public class animController : MonoBehaviour {

	public  Animator   myAnimator;
	private PhotonView myPV;
	private Rigidbody  myRigid;
	public  GameObject mySmoke;
	private bool       hitFlag;
	private float      hitFlagTimer;
	private float      currentHealth;
	private float      destroyTimer;
	// アニメーション同期用 //
	private string myAnimStatus;
	public Transform yRotObj;
	public Transform xRotObj;

	// Use this for initialization
	void Start () {
		hitFlag = false;
		hitFlagTimer = 0f;
		destroyTimer = 0f;
		myPV = PhotonView.Get (this.gameObject);
		myRigid = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(myPV.isMine){
			
			// 移動時 //
			float myAnimSpd = myRigid.velocity.magnitude / 24.0f;
			myAnimator.speed = myAnimSpd;

			// 被弾時 //
			if(hitFlag){
				myAnimator.SetLayerWeight (1, hitFlagTimer * 2.0f);
				hitFlagTimer += Time.deltaTime;
				if(hitFlagTimer > 2.0f){
					hitFlag = false;
					hitFlagTimer = 0f;
					myAnimator.SetLayerWeight (1, 0f);
				}
			}

			// 撃破時 //
			currentHealth = variableManage.currentHealth;
			if (currentHealth == 0f){
				destroyTimer += Time.deltaTime;
				myAnimator.SetLayerWeight(2, destroyTimer * 2.0f);
				mySmoke.SetActive(true);
			}else{
				myAnimator.SetLayerWeight(2, 0f);
				mySmoke.SetActive(false);
				destroyTimer = 0f;
			}

			//アニメーション同期用
			int layerWeight_1 = 0;
			int layerWeight_2 = 0;
			if (myAnimator.GetLayerWeight(1) >= 1.0f){
				layerWeight_1 = 1;
			}
			if (myAnimator.GetLayerWeight(2) >= 1.0f){
				layerWeight_2 = 1;
			}
			string yRotTmp  = Mathf.RoundToInt(yRotObj.localRotation.eulerAngles.y).ToString();
			string xRotTmp  = Mathf.RoundToInt(xRotObj.localRotation.eulerAngles.x).ToString();
			string yRotTmp2 = "";
			string xRotTmp2 = "";
			for (int i = 3; i > yRotTmp.Length ; i--){
				yRotTmp2 = "0" + yRotTmp2;
			}
			for (int i = 3; i > xRotTmp.Length ; i--){
				xRotTmp2 = "0" + xRotTmp2;
			}
			yRotTmp = yRotTmp2 + yRotTmp;
			xRotTmp = xRotTmp2 + xRotTmp;
			int myAnimSpdTmp = Mathf.RoundToInt(myAnimSpd * 100.0f);

			//各要素をつなげて格納
			myAnimStatus = yRotTmp + xRotTmp + layerWeight_1 + layerWeight_2 + myAnimSpdTmp;
		}else{
			//他のプレイヤーに映る自分自身のアニメーション
			if (myAnimStatus != "" && myAnimStatus != null){
				yRotObj.localRotation = Quaternion.Euler(new Vector3(0f,float.Parse(myAnimStatus.Substring(0,3)),0f));
				xRotObj.localRotation = Quaternion.Euler(new Vector3(float.Parse(myAnimStatus.Substring(3,3)),0f,0f));
				myAnimator.SetLayerWeight(1,float.Parse(myAnimStatus.Substring(6,1)));
				myAnimator.SetLayerWeight(2,float.Parse(myAnimStatus.Substring(7,1)));
				myAnimator.speed = float.Parse(myAnimStatus.Substring(8));
				if (myAnimator.GetLayerWeight(2) >= 1.0f){
					mySmoke.SetActive(true);
				}else{
					mySmoke.SetActive(false);
				}
			}
		}
	}

	// 衝突時に呼ばれる
	void OnCollisionEnter(Collision col){
		// bulletレイヤーに処理を限定
		if (col.gameObject.layer == 9){
			hitFlag = true;
			Debug.Log ("#hit");
		}
	}

	// 継続ひて同期する //
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext ((string)myAnimStatus);
		} else {
			myAnimStatus = (string)stream.ReceiveNext ();
		}
	}
}
