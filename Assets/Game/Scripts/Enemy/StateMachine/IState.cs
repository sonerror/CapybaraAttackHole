using UnityEngine.UIElements;

public interface IState<T>
{
    void OnEnter(T t);
    void OnExecute(T t);
    void OnExit(T t);
}