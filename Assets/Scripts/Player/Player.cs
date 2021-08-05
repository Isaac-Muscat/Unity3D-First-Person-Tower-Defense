using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public RectTransform healthBar;
    public GameObject basicTurretPrefab;
    public Text moneyText;
    public float money = 500f;
    public float maxHealth = 100f;
    public float health = 100;
    public string selectedTower = "Basic Turret";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float healthPercent = health/maxHealth;
        healthBar.GetChild(0).localScale = new Vector3(healthPercent, 1, 1);
        moneyText.text = "Money: $"+money;

        if(Input.GetMouseButtonDown(1)){
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            RaycastHit rayInfo;
            if(Physics.Raycast(ray, out rayInfo, 6f)){
                if(rayInfo.transform.tag == "Buildable"){
                    if(selectedTower=="Basic Turret" && money>=250){
                        money-=250;
                        Instantiate(basicTurretPrefab, rayInfo.point, transform.rotation);
                    }
                }
            }
        }
        if(health<=0){
            Destroy(gameObject);
        }
    }
}
