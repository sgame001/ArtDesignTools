using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Collections;

public class MainGUI : MonoBehaviour
{
    private float prefabSpeed = 4;
    private GUIStyle guiStyleHeader = new GUIStyle();


    public Transform EffectPrefab;
    [ReadOnly] [SerializeField] private float duration;

    public Transform wall;
    // Use this for initialization

    private bool moving = false;

    private GameObject monkey;

    private void Start()
    {
        if (EffectPrefab != null)
        {
            monkey = GameObject.Instantiate(EffectPrefab.gameObject);
        }
    }

    private void Update()
    {
        if (moving == false)
        {
            StartCoroutine("Shoot");
        }
       
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 20, 225, 30), "Ball Speed " + (int) prefabSpeed + "m", guiStyleHeader);
        prefabSpeed = GUI.HorizontalSlider(new Rect(115, 20, 120, 30), prefabSpeed, 1.0F, 30.0F);
        //effectSettings.MoveSpeed = prefabSpeed;
    }

    IEnumerator Shoot()
    {
        moving = true;
        yield return new WaitForSeconds(2);
        monkey.SetActive(false);
        duration =5f/ prefabSpeed ;
        monkey.transform.position = this.transform.position;
        
        monkey.transform.LookAt(wall.position);
        var tweener = monkey.transform.DOMove(wall.position, duration);
        
        tweener.OnStart(() =>
        {
            monkey.SetActive(true);
            
        });
        tweener.onComplete= () =>
        {
            //
            moving = false;
        };
       
    }
}