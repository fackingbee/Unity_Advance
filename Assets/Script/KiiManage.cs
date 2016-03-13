using UnityEngine;
using System.Collections;
using JsonOrg;
using KiiCorp.Cloud.Storage;
using UnityEngine.UI;

public class KiiManage : MonoBehaviour {

	//Kii用
	private string kiiUserName;
	private string kiiPassWord;
	//UI
	public Text statusText;

	void Start () {
		//作成されたユーザがあるか判断し、なければ新規作成
		if(PlayerPrefs.HasKey("userID")){
			kiiUserName = PlayerPrefs.GetString("userID");
			kiiPassWord = PlayerPrefs.GetString("userPW");
		}else{
			kiiUserName = randomCodeGenerate(10);
			kiiPassWord = randomCodeGenerate(6);
		}
		bool registCheck = RegistUser(kiiUserName,kiiPassWord);
		if(registCheck){
			//新規登録完了
			//Kiiへログイン
			statusText.text = "Created new data";
			bool loginCheck = loginUser(kiiUserName,kiiPassWord);
			if(loginCheck){
				//ログイン成功
				//Kiiに保存するデータを初期化
				bool initData = kiiDataInitialize();
				if(!initData){
					initData = kiiDataInitialize();
				}
				if(!initData){
					//保存失敗
					Application.LoadLevel("start");
					Destroy(this.gameObject);
				}
				//初期化成功
				if(initData){
					//ローカルにIDとPWを保存しメインメニューへ
					PlayerPrefs.SetString("userID",kiiUserName);
					PlayerPrefs.SetString("userPW",kiiPassWord);
					Application.LoadLevel("mainMenu");
				}
			}else{
				//接続失敗
				Application.LoadLevel("start");
				Destroy(this.gameObject);
			}
		}else{
			//すでに登録されているユーザ
			//Kiiへログイン
			statusText.text = "Loading server data";
			bool loginCheck = loginUser(kiiUserName,kiiPassWord);
			if(loginCheck){
				//ログイン成功 データロード
				bool loadDataCheck = loadKiiData();
				if(!loadDataCheck){
					loadDataCheck = loadKiiData();
				}
				if(!loadDataCheck){
					//読み込み失敗
					Application.LoadLevel("start");
					Destroy(this.gameObject);
				}
				if(loadDataCheck){
					//読み込み成功　シーン移動
					Application.LoadLevel("mainMenu");
				}
			}else{
				//接続失敗
				Application.LoadLevel("start");
				Destroy(this.gameObject);
			}
		}
	}

	//KiiCloudへログインする
	public bool loginUser( string userName,string password ){
		KiiUser user;
		try
		{
			user = KiiUser.LogIn(userName,password);
			Debug.Log ( "Success user login : " + userName );
		}
		catch( System.Exception exception )
		{
			Debug.LogError( "Failed user login : " + userName + " : " + exception );
			user = null;
			return false;
		}

		return true;
	}

	//入力されたユーザID、パスワードのユーザが存在しなければ新規作成
	public bool RegistUser( string userName,string password )
	{
		if( !KiiUser.IsValidUserName( userName ) ||
			!KiiUser.IsValidPassword( password ) )
		{
			Debug.LogError( "Invalid user name or password : " + userName );
			return false;
		}
		KiiUser.Builder builder = KiiUser.BuilderWithName(userName);
		KiiUser _User = builder.Build ();
		try
		{
			_User.Register( password );
			Debug.Log ( "Success user regist : " + userName );
		}
		catch( System.Exception exception )
		{
			Debug.Log( "Failed user regist : " + userName + " : " + exception );
			_User = null;
			return false;
		}

		return true;
	}

	//KiiCloud上のデータ保存枠を初期化
	bool kiiDataInitialize(){
		//ユーザーバケットを定義
		KiiBucket userBucket = 
			KiiUser.CurrentUser.Bucket("myBasicData");
		KiiObject basicDataObj = userBucket.NewKiiObject();
		//保存するデータを定義
		basicDataObj["lv"] = 1;
		basicDataObj["exp"] = 0;
		basicDataObj["open2"] = false;
		basicDataObj["open3"] = false;
		basicDataObj["wp"] = 0;
		//オブジェクトを保存
		try{
			basicDataObj.Save();
		}catch(System.Exception e){
			Debug.LogError(e);
			return false;
		}

		return true;
	}

	//保存されているデータを読み込み
	bool loadKiiData(){
		KiiQuery allQuery = new KiiQuery();
		try {
			//検索条件を指定
			KiiQueryResult<KiiObject> result = 
				KiiUser.CurrentUser.Bucket("myBasicData").Query(allQuery);
			foreach(KiiObject obj in result){
				//データを読み込み
				variableManage.currentLv     = (int)obj["lv"];
				variableManage.currentExp    = (int)obj["exp"];
				variableManage.openMachine02 = (bool)obj["open2"];
				variableManage.openMachine03 = (bool)obj["open3"];
				variableManage.myWP          = (int)obj["wp"];
			}
		}catch (System.Exception e) {
			Debug.Log(e);
			return false;
		}

		return true;
	}

	//現在のデータを保存
	public static bool saveKiiData(){
		KiiQuery allQuery = new KiiQuery();
		try {
			//検索条件を指定
			KiiQueryResult<KiiObject> result = 
				KiiUser.CurrentUser.Bucket("myBasicData").Query(allQuery);

			Debug.Log("kii : "+variableManage.currentLv);

			foreach (KiiObject obj in result)
			{
				//データを保存
				obj["lv"]    = variableManage.currentLv;
				obj["exp"]   = variableManage.currentExp;
				obj["open2"] = variableManage.openMachine02;
				obj["open3"] = variableManage.openMachine03;
				obj["wp"]    = variableManage.myWP;
				obj.Save();
			}
		}
		catch (System.Exception e){
			Debug.Log(e);
			return false;
		}

		return true;
	}

	//ランダムの文字列を作成する
	string randomCodeGenerate(int codeLength){
		string allCode = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
		string outPutCode = "";
		for(int i = 0; i<codeLength; i++){
			int rndTmp = Random.Range(0,allCode.Length);
			outPutCode += allCode.Substring(rndTmp,1);
		}
		return outPutCode;
	}

}
