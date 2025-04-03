using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;

    int health;

    void Start(){
        health = 3;
        UpdateHearts();
    }

    void Update(){
        Heal();
        TakeLife();
    }

    void TakeLife(){
        if(Input.GetKeyDown(KeyCode.A) && health <= 3 && health > 0){
            health--;
            UpdateHearts();
        }
    }

    void Heal(){
        if(Input.GetKeyDown(KeyCode.D)  && health < 3 && health >= 0){
            health++;
            UpdateHearts();
        }
    }

    void UpdateHearts(){
        switch(health){
            case 0:
                Heart1.SetActive(false);
                Heart2.SetActive(false);
                Heart3.SetActive(false);
                break;
            case 1:
                Heart1.SetActive(true);
                Heart2.SetActive(false);
                Heart3.SetActive(false);
                break;
            case 2:
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(false);
                break;
            case 3:
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(true);
                break;
        }
    }
}
