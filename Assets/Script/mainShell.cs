using UnityEngine;
using System.Collections;

public class mainShell : MonoBehaviour {

	public float maxRadius;
	private bool explosionSw;
	public GameObject firstParticle;
	public GameObject secondParticle;
	private float explosionTimer;
	public SphereCollider myCollider;
	private float colliderTimer;
	private float deleteTimer;
	public Rigidbody myRigid;
	//
	public float pow;

	void Start () {
		explosionSw = false;
		explosionTimer = 0f;
		colliderTimer = 0f;
		deleteTimer = 0f;
	}

	void Update () {
		if(explosionSw){
			firstParticle.SetActive(false);
			secondParticle.SetActive(true);
			myRigid.isKinematic = true;
			explosionTimer += (Time.deltaTime*10.0f);
			if(myCollider.radius < maxRadius){
				myCollider.radius = 0.5f + explosionTimer;
			}
			//destroy
			deleteTimer += Time.deltaTime;
			if(deleteTimer > 12.0f){
				Destroy(this.gameObject);
			}else if(deleteTimer > 2.0f){
				myCollider.enabled = false;
			}
		}else{
			colliderTimer += Time.deltaTime;
			if(colliderTimer > 10.0f){
				Destroy(this.gameObject);
			}else if(colliderTimer > 0.06f){
				myCollider.enabled = true;
			}
		}
	}

	void OnCollisionEnter(Collision col){
		explosionSw = true;
	}

}
