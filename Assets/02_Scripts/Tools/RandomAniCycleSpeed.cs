using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAniCycleSpeed : MonoBehaviour {

	// Use this for initialization

    void Awake()
    {
        anim = GetComponent<Animator>();
        cycleoffset = Random.Range(0, 1);
        randomspeed = Random.Range(0.3f, 1);
    }
         
	void Start () {
        anim.SetFloat("cycleoffset", cycleoffset);
        anim.speed = randomspeed;

    }

    private Animator anim;
    private float cycleoffset = 0;
    private float randomspeed = 0;



}
