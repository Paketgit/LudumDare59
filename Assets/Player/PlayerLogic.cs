using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour
{
    private int energy;
    public int Energy { 
        get {return energy;} 
        set{ energy = value < 0 ? energy : value;}}
    public IContainData<int> currentEnergyContainer;
    void Start()
    {
        G.inputActions.Player.Getinteractions.started += GetDataEnergy;
        G.inputActions.Player.PutInteractions.started += PutEnergy;
    }

    private void PutEnergy(InputAction.CallbackContext context)
    {
        if(currentEnergyContainer == null) return;
        energy = currentEnergyContainer.PutData(energy);
    }

    private void GetDataEnergy(InputAction.CallbackContext context)
    {
        if(currentEnergyContainer == null) return;
        energy += currentEnergyContainer.GetData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
