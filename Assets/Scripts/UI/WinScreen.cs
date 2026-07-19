using UnityEngine;
using Combat;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private ScreenTransition _transition;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _settingsGO;
    private GameObject _itemsWon;
    private GameObject _partyStatus;
    [SerializeField] private GameObject _itemWonPrefab;
    [SerializeField] private GameObject _dinoPartyPrefab;

    [Header("Audio")]
    [SerializeField] private int _audioListIndex = 0;
    List<DinosaurPart> _partsWon;

    private Dictionary<BodyPartType, Sprite> _partSprites = new Dictionary<BodyPartType, Sprite>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _itemsWon = GameObject.Find("ItemsWon");
        _partyStatus = GameObject.Find("PartyStatus");
        _partsWon = BattleDataLoader.Instance.GetPartsWon();
        List <DinosaurData> creatures = PlayerInventory.Instance.GetCreatures();
        for(int i = 0; i < creatures.Count; i++)
        {
            GameObject dino = Instantiate(_dinoPartyPrefab, _partyStatus.transform);
            dino.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "HP: " + creatures[i].GetCurrentHealth().ToString();
        }
        if(BattleDataLoader.Instance.IsRewardingNewDino)
        {
            GameObject dino = Instantiate(_dinoPartyPrefab, _partyStatus.transform);
            dino.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "New Dinosaur Unlocked";
        }
        if (_partsWon.Count > 0)
        {
            for (int i = 0; i < _partsWon.Count; i++)
            {
                GameObject item = Instantiate(_itemWonPrefab, _itemsWon.transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = _partsWon[i].Reference.Icon;
                item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _partsWon[i].Reference.Name;
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
