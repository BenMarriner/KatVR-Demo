using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour
{
    [SerializeField]
    GameObject _newWindow;

    [SerializeField]
    Window _window1;

    [SerializeField]
    Window _window2;

    [SerializeField]
    Window _window3;

    public void ToggleWindow1()
    {
        if (_window1.gameObject.activeSelf)
        {
            _window1.CloseWindow();
        }
        else
        {
            _window1.OpenWindow();
        }
    }

    public void ToggleWindow2()
    {
        if (_window2.gameObject.activeSelf)
        {
            _window2.CloseWindow();
        }
        else
        {
            _window2.OpenWindow();
        }
    }

    public void ToggleWindow3()
    {
        if (_window3.gameObject.activeSelf)
        {
            _window3.CloseWindow();
        }
        else
        {
            _window3.OpenWindow();
        }
    }

    public void ToggleNewWindow()
    {
        GameObject go = GameObject.Instantiate(_newWindow);
        go.transform.SetParent(this._window1.transform.parent, false);
        go.GetComponent<Window>().OpenWindow(true);
    }
}
