using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ValueContainer : MonoBehaviour, IContainData<int>
{
    [SerializeField] float TriggerRadius = 0.5f;
    [SerializeField] int Energy;
    [SerializeField] int maxEnergyContain;
    float NewNormalizedT;
    CircleCollider2D circleCollider2D;

    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = TriggerRadius;
        circleCollider2D.isTrigger = true;
    }


    private void CheckContainedEnergy()
    {
        if(Energy < maxEnergyContain)
        {
            Debug.Log("I need Energy");
        }
        else if(Energy == maxEnergyContain)
        {
            Debug.Log("All ok");
        }
        else
        {
            Debug.Log("I have external energy");
        }

    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Triggered " + other.gameObject.tag);

            PlayerLogic playerLogic = other.gameObject.GetComponent<PlayerLogic>();
            playerLogic.currentEnergyContainer = gameObject.GetComponent<ValueContainer>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Triggered " + other.gameObject.tag);

            PlayerLogic playerLogic = other.gameObject.GetComponent<PlayerLogic>();
            playerLogic.currentEnergyContainer = null;
        }
    }

    public int GetData()
    {
        var tmp = Energy;
        Energy = 0;
        CheckContainedEnergy();
        return tmp;
    }

    public int PutData(int data)
    {
        if(maxEnergyContain <= Energy){ return 0; }
        if(data > maxEnergyContain)
        {
            Energy = maxEnergyContain;
            return data - maxEnergyContain;
        }
        if(data + Energy > maxEnergyContain)
        {
            data = maxEnergyContain - Energy - data;
            Energy = maxEnergyContain;
            return data;
        }
        Energy += data;
        CheckContainedEnergy();
        return 0;
    }

}
