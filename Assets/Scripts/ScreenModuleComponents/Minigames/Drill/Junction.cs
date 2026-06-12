using UnityEngine;
using UnityEngine.Splines;

public class Junction : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Tooltip("TrueSprite is the position that keeps it on the main spline, switchsprite moves it to the alternative track associated with this junction.")]
    [SerializeField] private Sprite trueSprite;
    [SerializeField] private Sprite switchSprite;
    [SerializeField] public SplineContainer switchSpline;
    public bool isInTruePosition; //Keeps track of our state so we can alternate cleanly


    // Initialize
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        isInTruePosition = UnityEngine.Random.Range(0, 2) == 1;
        

        if(isInTruePosition){
            spriteRenderer.sprite=trueSprite;
        }
        else{
            spriteRenderer.sprite=switchSprite;
        }
    }

    public void OnSwitch(){ //When the corresponding switch is hit (assign both switch up and switch down to call this)
        if(isInTruePosition){
            isInTruePosition = false;
            spriteRenderer.sprite=switchSprite;
            //Debug.Log("SwitchPos");
        }
        else{
            isInTruePosition = true;
            spriteRenderer.sprite=trueSprite;
            //Debug.Log("TruePos");
        }
    }

        void OnTriggerEnter(Collider otherObject)
    {
        //Debug.Log("AAAAAAAAAAAAAAAA (Junction)");
    }

}
