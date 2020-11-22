using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAngle : MonoBehaviour {

    [SerializeField] private float viewAngle; // 시야각
    [SerializeField] private float viewDistance; // 시야 거리
    [SerializeField] private LayerMask targetMask; // 플레이어


	
	// Update is called once per frame
	void Update () {
        View();
	}

    private void View()
    {
        //Vector3 leftBoundary;
        //Vector3 rightBoundary;
    }
}
