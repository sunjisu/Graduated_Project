using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public string gunName;
    public float attackRange; // 공격 범위
    public int power; // 공격력

    public float accuracy; // 정확성
    public float attackSpeed; // 공격 속도
    public float reloadSpeed; // 재장전 속도

    public int reloadBulletCount; // 최대 장전 개수
    public int currentBulletCount; // 장전되어있는 총알수
    public int maxBulletCount; // 최대 보유할수 있는 총알 개수
    public int carryBulletCount; // 소유하고 있는 총알의 수

    public float rebound; // 총기반동
    public float aimingRebound; // 정조준때 총기반동

    public Vector3 aimingPos; // 정조준시 총 위치
    public Animator anim; // 총 애니메이션
    public ParticleSystem effect; // 총 이펙트

    public AudioClip gunSound; // 총소리

}
