using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Level1PuzzleManager : NetworkBehaviour
{
    [SerializeField] List<BannerSO> _banners = new List<BannerSO>();
    [SerializeField] Transform _leftBannerPos, _rightBannerPos;
    [SerializeField] List<Sprite> _symbols = new List<Sprite>();
    [SerializeField] List<Sprite> _bannerSprites = new List<Sprite>();
    [SerializeField] List<GameObject> _uiButtons = new List<GameObject>();
    [SerializeField] List<GameObject> _doorSymbolSlots = new List<GameObject>();
    [SerializeField] List<GameObject> _bookPageContents = new List<GameObject>();

    int _currentSymbolIndex = 0;


    NetworkVariable<int> _leftBanner = new NetworkVariable<int>(0);
    NetworkVariable<int> _rightBanner = new NetworkVariable<int>(0);
    NetworkVariable<int> _firstSymbol = new NetworkVariable<int>(0);
    NetworkVariable<int> _secondSymbol = new NetworkVariable<int>(0);
    NetworkVariable<int> _thirdSymbol = new NetworkVariable<int>(0);


    NetworkVariable<int> _correctPageIndex = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            _correctPageIndex.Value = Random.Range(0, _bookPageContents.Count);
            SetupPuzzleServerRpc();
        }

        if (IsClient)
        {
            PlaceBanners();
            SetBookContent();
        }
    }

    private void PlaceBanners()
    {
        var leftBannerGO = Instantiate(_banners[_leftBanner.Value]._bannerGO, _leftBannerPos.position, Quaternion.identity);
        var rightBannerGO = Instantiate(_banners[_rightBanner.Value]._bannerGO, _rightBannerPos.position, Quaternion.identity);

        leftBannerGO.transform.SetParent(_leftBannerPos);
        rightBannerGO.transform.SetParent(_rightBannerPos);
        leftBannerGO.transform.rotation = Quaternion.Euler(0, 180, 0);
        rightBannerGO.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetupPuzzleServerRpc()
    {
        foreach (var slot in _doorSymbolSlots)
        {
            slot.transform.GetChild(1).GetComponent<Image>().sprite = null;
        }

        SetUIButtonSprites();
        SetBanners();
        SetCorrectSymbols();
    }

    private void SetCorrectSymbols()
    {
        var allSymbols = new List<int>();

        for (int i = 0; i < _symbols.Count; i++)
        {
            allSymbols.Add(i);
        }

        _firstSymbol.Value = allSymbols[Random.Range(0, allSymbols.Count)];
        allSymbols.Remove(_firstSymbol.Value);
        _secondSymbol.Value = allSymbols[Random.Range(0, allSymbols.Count)];
        allSymbols.Remove(_secondSymbol.Value);
        _thirdSymbol.Value = allSymbols[Random.Range(0, allSymbols.Count)];
    }

    private void SetBookContent()
    {
        var allPages = _bookPageContents;
        var allSymbols = _symbols;

        var correctPage = allPages[_correctPageIndex.Value];
        correctPage.transform.GetChild(0).GetComponent<Image>().sprite = _banners[_leftBanner.Value]._bannerSprite;
        correctPage.transform.GetChild(1).GetComponent<Image>().sprite = _banners[_rightBanner.Value]._bannerSprite;
        correctPage.transform.GetChild(2).GetComponent<Image>().sprite = _symbols[_firstSymbol.Value];
        correctPage.transform.GetChild(3).GetComponent<Image>().sprite = _symbols[_secondSymbol.Value];
        correctPage.transform.GetChild(4).GetComponent<Image>().sprite = _symbols[_thirdSymbol.Value];

        allSymbols = _symbols;
        allPages.Remove(correctPage);



        foreach (var page in _bookPageContents)
        {
            var randomLeftBanner = _banners[Random.Range(0, _banners.Count)];
            var randomRightBanner = _banners[Random.Range(0, _banners.Count)];

            while (randomLeftBanner == _banners[_leftBanner.Value] && randomRightBanner == _banners[_rightBanner.Value])
            {
                randomLeftBanner = _banners[Random.Range(0, _banners.Count)];
                randomRightBanner = _banners[Random.Range(0, _banners.Count)];
            }


            page.transform.GetChild(0).GetComponent<Image>().sprite = randomLeftBanner._bannerSprite;
            page.transform.GetChild(1).GetComponent<Image>().sprite = randomRightBanner._bannerSprite;
            page.transform.GetChild(2).GetComponent<Image>().sprite = allSymbols[Random.Range(0, allSymbols.Count)];
            allSymbols.Remove(correctPage.transform.GetChild(2).GetComponent<Image>().sprite);
            page.transform.GetChild(3).GetComponent<Image>().sprite = allSymbols[Random.Range(0, allSymbols.Count)];
            allSymbols.Remove(correctPage.transform.GetChild(3).GetComponent<Image>().sprite);
            page.transform.GetChild(4).GetComponent<Image>().sprite = allSymbols[Random.Range(0, allSymbols.Count)];

            allSymbols = _symbols;
        }
    }

    private void SetBanners()
    {
        var allBanners = new List<int>();
        for (int i = 0; i < _banners.Count; i++)
        {
            allBanners.Add(i);
        }
        _leftBanner.Value = allBanners[Random.Range(0, allBanners.Count)];
        allBanners.Remove(_leftBanner.Value);
        _rightBanner.Value = allBanners[Random.Range(0, allBanners.Count)];
    }

    void SetUIButtonSprites()
    {
        var allSymbols = _symbols;
        var usedSymbols = new List<Sprite>();
        foreach (GameObject button in _uiButtons)
        {
            var randomSymbol = allSymbols[Random.Range(0, allSymbols.Count)];
            button.transform.GetChild(0).GetComponent<Image>().sprite = randomSymbol;

            allSymbols.Remove(randomSymbol);
            usedSymbols.Add(randomSymbol);
        }
        _symbols = usedSymbols;
    }

    public void SelectSymbolButton(int i)
    {
        _doorSymbolSlots[_currentSymbolIndex].transform.GetChild(1).GetComponent<Image>().sprite = _uiButtons[i].transform.GetChild(0).GetComponent<Image>().sprite;
        _currentSymbolIndex++;
        _uiButtons[i].GetComponent<Button>().interactable = false;
        if (_currentSymbolIndex == _doorSymbolSlots.Count)
        {
            CheckSymbols();
        }
    }

    private void CheckSymbols()
    {
        if (_doorSymbolSlots[0].transform.GetChild(1).GetComponent<Image>().sprite == _symbols[_firstSymbol.Value] &&
                       _doorSymbolSlots[1].transform.GetChild(1).GetComponent<Image>().sprite == _symbols[_secondSymbol.Value] &&
                                  _doorSymbolSlots[2].transform.GetChild(1).GetComponent<Image>().sprite == _symbols[_thirdSymbol.Value])
        {
            Debug.Log("Correct");
        }else
        {
            Debug.Log("Wrong");
            foreach (var slot in _doorSymbolSlots)
            {
                slot.transform.GetChild(1).GetComponent<Image>().sprite = null;
            }
            foreach (var button in _uiButtons)
            {
                button.GetComponent<Button>().interactable = true;
            }
            _currentSymbolIndex = 0;
        }
    }
}
