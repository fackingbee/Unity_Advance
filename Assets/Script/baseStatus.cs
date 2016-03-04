using UnityEngine;
using System.Collections;

public class baseStatus : MonoBehaviour {

	//どちらのチームの拠点か
	public int baseID;

	//拠点に被弾した
	void OnCollisionEnter(Collision col){
		if(col.gameObject.layer == 9){
			if(baseID == 1){
				variableManage.team1baseBullet = col.gameObject;
			}else{
				variableManage.team2baseBullet = col.gameObject;
			}
		}
	}

}
