using UnityEngine;

public class TestMinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject knob;
    private SpriteRenderer knobSprite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        knobSprite = knob.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Button1()
    {
        knob.transform.localPosition += new Vector3(0.05f, -0.05f, 0f);
    }
    
    public void Button2Down()
    {
        knobSprite.color = Color.green;
    }
    
    public void Button2Up()
    {
        knobSprite.color = Color.white;
    }
}
