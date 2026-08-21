using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BdingControl : MonoBehaviour {

    public GameObject mBding;
    public GameObject mBding2F;
    
    public ParticleSystem mSystem;
    public GameObject mPeople;

    [Header("_People")]
    //객실자들
    public GameObject mOldman;
    public GameObject mOldman01;
    public GameObject mChild;
    public GameObject mChild01;
    public GameObject mNormal;
    public GameObject mNormal01;
    public GameObject mFireman;

    private bool mFlagExpend = false;
    private bool mFlagMove = false;
 
	// Use this for initialization
	void Start () {
      
        
        //Fire off
        mSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        mSystem.GetComponent<AudioSource>().Stop();
        //객실자 off
        mPeople.SetActive(false);

        //객실자 멈춤
        mOldman.GetComponent<WaypointMover>().Pause();
        mOldman01.GetComponent<WaypointMover>().Pause();
        mChild.GetComponent<WaypointMover>().Pause();
        mChild01.GetComponent<WaypointMover>().Pause();
        mNormal.GetComponent<WaypointMover>().Pause();
        mNormal01.GetComponent<WaypointMover>().Pause();
        mFireman.GetComponent<WaypointMover>().Pause();

        StartCoroutine(MoveObject());
	}
	
	// Update is called once per frame
	void Update () {
        //Reset 처음부터..
        if (Input.GetKeyUp("0"))
        {
            resetEffect();
        }
        //2F Highlight
		if (Input.GetKeyUp("1")){
            mBding2F.GetComponent<Animator>().Play("aniBding2Fcolor");
        }
        //층별 벌리고 2층으로 줌인
        if(Input.GetKeyUp("2")){
            mBding2F.GetComponent<Animator>().Play("idle");
            mBding.GetComponent<Animator>().Play("btMove");            
            mFlagExpend = true;
        }
        //화재 가시화
        if (Input.GetKeyUp("3"))
        {
            if (mFlagExpend)
            {
                mSystem.Play(true);
                mSystem.GetComponent<AudioSource>().Play();
            }
        }
        //People 가시화
        if (Input.GetKeyUp("4"))
        {
                 
            mPeople.SetActive(true);
        }
        //객실자들 이동
        if (Input.GetKeyUp("5"))
        {
            pMove();
        }
            
	}

    public void pMove()
    {
        //객실자 이동 토글
        if (!mFlagMove)
        {
            mOldman.GetComponent<WaypointMover>().Unpause();
            mOldman01.GetComponent<WaypointMover>().Unpause();
            mChild.GetComponent<WaypointMover>().Unpause();
            mChild01.GetComponent<WaypointMover>().Unpause();
            mNormal.GetComponent<WaypointMover>().Unpause();
            mNormal01.GetComponent<WaypointMover>().Unpause();
            mFireman.GetComponent<WaypointMover>().Unpause();

            mFlagMove = !mFlagMove;
        }
        else
        {
            mOldman.GetComponent<WaypointMover>().Pause();
            mOldman01.GetComponent<WaypointMover>().Pause();
            mChild.GetComponent<WaypointMover>().Pause();
            mChild01.GetComponent<WaypointMover>().Pause();
            mNormal.GetComponent<WaypointMover>().Pause();
            mNormal01.GetComponent<WaypointMover>().Pause();
            mFireman.GetComponent<WaypointMover>().Pause();

            mFlagMove = !mFlagMove;
        }
        
    }

    public void resetEffect()
    {
        mBding2F.GetComponent<Animator>().Play("idle");
        mBding.GetComponent<Animator>().Play("idle");
       
        mSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        mSystem.GetComponent<AudioSource>().Stop();
        mFlagExpend = false;
        mPeople.SetActive(false);
        mFlagMove = true;

        //이동 위치 리셋
        mOldman.GetComponent<WaypointMover>().SetPosition(0);
        mOldman01.GetComponent<WaypointMover>().SetPosition(0);
        mChild.GetComponent<WaypointMover>().SetPosition(0);
        mChild01.GetComponent<WaypointMover>().SetPosition(0);
        mNormal.GetComponent<WaypointMover>().SetPosition(0);
        mNormal01.GetComponent<WaypointMover>().SetPosition(0);
        mFireman.GetComponent<WaypointMover>().SetPosition(0);
        pMove();

        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        while (true)
        {
            //흔들림 효과
            float dir1 = Random.Range(-0.001f, 0.001f);
            float dir2 = Random.Range(-0.001f, 0.001f);
            float dir3 = Random.Range(-0.001f, 0.001f);
            float dir4 = Random.Range(-0.001f, 0.001f);

            yield return new WaitForSeconds(0.2f);
            mOldman.transform.Translate(new Vector3(dir1, 0, dir2));
            mOldman01.transform.Translate(new Vector3(dir1, 0, dir3));
            mChild.transform.Translate(new Vector3(dir2, 0, dir4));
            mChild01.transform.Translate(new Vector3(dir3, 0, dir2));
            mNormal.transform.Translate(new Vector3(dir1, 0, dir4));
            mNormal01.transform.Translate(new Vector3(dir4, 0, dir2));
            mFireman.transform.Translate(new Vector3(dir3, 0, dir1));
        }
    }

    void playSound(string snd)
    {

        GameObject.Find(snd).GetComponent<AudioSource>().Play();

    }

}
