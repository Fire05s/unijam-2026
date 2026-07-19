using UnityEngine;
using Combat;
using System.Collections.Generic;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private ScreenTransition _transition;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _settingsGO;
    private GameObject _itemsWon;
    [SerializeField] private GameObject _itemWonPrefab;

    [Header("Audio")]
    [SerializeField] private int _audioListIndex = 0;
    List<DinosaurPart> _partsWon;

    private Dictionary<BodyPartType, Sprite> _partSprites = new Dictionary<BodyPartType, Sprite>();

    private void Start()
    {
        _itemsWon = GameObject.Find("ItemsWon");
        _partsWon = BattleDataLoader.Instance.GetPartsWon();
        if (_partsWon.Count > 0)
        {
            for (int i = 0; i < _partsWon.Count; i++)
            {
                GameObject item = Instantiate(_itemWonPrefab, _itemsWon.transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = _partsWon[i].Reference.Icon;
                item.transform.GetChild(1).GetComponent<Text>().text = _partsWon[i].Reference.Name;
            }
        }
    }
    public void Tutorial()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _transition.FadeAndLoad("CombinerTutorial", duration: 2f);
    }

    public void ContinueGame()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _transition.FadeAndLoad("MainLevel", duration: 2f);
    }

    public void Settings()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _mainMenuGO.SetActive(false);
        _settingsGO.SetActive(true);
    }

    public void Quit()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        Application.Quit();
    }
}
