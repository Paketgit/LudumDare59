using UnityEngine;

public interface IContainData<T> 
{
    public T GetData();
    public int PutData(T data);
}
