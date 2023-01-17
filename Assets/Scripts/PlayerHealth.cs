using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int lifeCount = 1;
    [SerializeField] private RectTransform lifeBar;
    [SerializeField] private GameObject lifeIcon;

    private void Start()
    {
        for (int i = 0; i < lifeCount; i++)
        {
            Instantiate(lifeIcon,lifeBar);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("PowerUp")))
        {
            if (lifeCount < 9)
            {
                lifeCount++;
                Debug.Log(lifeCount);
                Instantiate(lifeIcon, lifeBar);
            }

            Destroy(other.gameObject);

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
        {
            lifeCount--;
            Destroy(lifeBar.transform.GetChild(lifeCount).gameObject);
        }
    }
}
