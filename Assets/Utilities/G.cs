using UnityEngine;

public static class G
{   
    public static GameObject Player;
    public static InputSystem_Actions inputActions;

    static G()
    {
        inputActions = new InputSystem_Actions();
    }
}
