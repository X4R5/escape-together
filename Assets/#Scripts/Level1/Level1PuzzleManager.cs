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
    BannerSO _leftBanner, _rightBanner;
    Sprite _firstSymbol, _secondSymbol, _thirdSymbol;
    private void Start()
    {
        foreach (var slot in _doorSymbolSlots)
        {
            slot.transform.GetChild(1).GetComponent<Image>().sprite = null;
        }
        SetCorrectSymbols();
        SetUIButtonSprites();
        SetBanners();
        SetBookContent();
    }

    private void SetCorrectSymbols()
    {
        var allSymbols = new List<Sprite>();

        foreach (var symbol in _symbols)
        {
            allSymbols.Add(symbol);
        }

        _firstSymbol = allSymbols[Random.Range(0, allSymbols.Count)];
        allSymbols.Remove(_firstSymbol);
        _secondSymbol = allSymbols[Random.Range(0, allSymbols.Count)];
        allSymbols.Remove(_secondSymbol);
        _thirdSymbol = allSymbols[Random.Range(0, allSymbols.Count)];
    }

    private void SetBookContent()
    {
        var allPages = _bookPageContents;
        var allSymbols = _symbols;

        var correctPage = allPages[Random.Range(0, allPages.Count)];
        correctPage.transform.GetChild(0).GetComponent<Image>().sprite = _leftBanner._bannerSprite;
        correctPage.transform.GetChild(1).GetComponent<Image>().sprite = _rightBanner._bannerSprite;
        correctPage.transform.GetChild(2).GetComponent<Image>().sprite = _firstSymbol;
        correctPage.transform.GetChild(3).GetComponent<Image>().sprite = _secondSymbol;
        correctPage.transform.GetChild(4).GetComponent<Image>().sprite = _thirdSymbol;

        allSymbols = _symbols;
        allPages.Remove(correctPage);



        foreach (var page in _bookPageContents)
        {
            var randomLeftBanner = _banners[Random.Range(0, _banners.Count)];
            var randomRightBanner = _banners[Random.Range(0, _banners.Count)];

            while (randomLeftBanner == _leftBanner && randomRightBanner == _rightBanner)
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
        var allBanners = _banners;
        var leftBanner = allBanners[Random.Range(0, allBanners.Count)];
        allBanners.Remove(leftBanner);
        var rightBanner = allBanners[Random.Range(0, allBanners.Count)];

        _leftBanner = leftBanner;
        _rightBanner = rightBanner;

        var leftBannerGO = Instantiate(_leftBanner._bannerGO, _leftBannerPos.position, Quaternion.identity);
        var rightBannerGO = Instantiate(_rightBanner._bannerGO, _rightBannerPos.position, Quaternion.identity);

        leftBannerGO.transform.SetParent(_leftBannerPos);
        rightBannerGO.transform.SetParent(_rightBannerPos);
        leftBannerGO.transform.rotation = Quaternion.Euler(0, 180, 0);
        rightBannerGO.transform.rotation = Quaternion.Euler(0, 180, 0);
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
        if (_doorSymbolSlots[0].transform.GetChild(1).GetComponent<Image>().sprite == _firstSymbol &&
                       _doorSymbolSlots[1].transform.GetChild(1).GetComponent<Image>().sprite == _secondSymbol &&
                                  _doorSymbolSlots[2].transform.GetChild(1).GetComponent<Image>().sprite == _thirdSymbol)
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
