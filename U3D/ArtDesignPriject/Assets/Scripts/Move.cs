using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Move : MonoBehaviour
{

	public GameObject monkey;

	public List<Vector3> targetPosList;


	private Vector3 targetPos;

	private void Start()
	{
		targetPos = targetPosList[0];
		DoMove();
	}

	void DoMove()
	{
		monkey.transform.LookAt(targetPos);
		monkey.transform.DOMove(targetPos, 1).onComplete = () =>
		{
			int ind =Random.Range(0, targetPosList.Count);
			Vector3 nextPoint = targetPos;
			while (nextPoint==targetPos)
			{
				nextPoint = targetPosList[ind];
			}

			targetPos = nextPoint;
			DoMove();
		};
	}
}
