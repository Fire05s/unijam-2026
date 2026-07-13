using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    [Header("Screens List")]
    [SerializeField] private List<GameObject> _screens;

    private int _currentScreen;

    public void SwitchScreen(int screenNum)
    {
        if (_currentScreen == screenNum ||
            screenNum < 0 || screenNum >= _screens.Count) return;

        _screens[_currentScreen].SetActive(false);
        _currentScreen = screenNum;
        _screens[screenNum].SetActive(true);
    }
}
