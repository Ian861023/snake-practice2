using UnityEngine;

public class Food : MonoBehaviour
{   public Collider2D foodArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        RandomPosition();
    }
    void RandomPosition()
    {
        transform.position = new Vector3(
            Random.Range(foodArea.bounds.min.x, foodArea.bounds.max.x), 
            Random.Range(foodArea.bounds.min.y, foodArea.bounds.max.y), 
            0);
    }

}
