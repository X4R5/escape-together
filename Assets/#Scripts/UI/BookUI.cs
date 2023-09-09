using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookUI : MonoBehaviour
{
    public static BookUI Instance;

    [SerializeField] Animator _animator;

    [SerializeField] GameObject _bookUI, _forwardBtn, _backBtn;
    [SerializeField] Transform _leftPage, _rightPage;

    [SerializeField] List<GameObject> _pageContents = new List<GameObject>();

    bool _isTurningPage = false;

    int _currentPage = 0;

    private void Awake()
    {
        Instance = this;

        _bookUI.SetActive(false);

        OpenBook();
    }

    public void OpenBook()
    {
        _bookUI.SetActive(true);
        _currentPage = 0;
        DeactiveAllContents();
        _pageContents[_currentPage].SetActive(true);
        CheckButtons();
    }

    public void CloseBook()
    {
        _bookUI.SetActive(false);
    }

    public void NextPage()
    {
        if (_isTurningPage) return;

        _currentPage++;
        _rightPage.SetAsLastSibling();
        _animator.SetTrigger("forward");
        DeactiveAllContents();
        _isTurningPage = true;
    }

    public void PreviousPage()
    {
        if (_isTurningPage) return;

        _currentPage--;
        _leftPage.SetAsLastSibling();
        _animator.SetTrigger("back");
        DeactiveAllContents();
        _isTurningPage = true;
    }


    void CheckButtons()
    {
        if (_currentPage == 0)
        {
            _backBtn.SetActive(false);
        }
        else
        {
            _backBtn.SetActive(true);
        }

        if (_currentPage == _pageContents.Count - 1)
        {
            _forwardBtn.SetActive(false);
        }
        else
        {
            _forwardBtn.SetActive(true);
        }
    }

    void DeactiveAllContents()
    {
        foreach (var content in _pageContents)
        {
            content.SetActive(false);
        }
    }

    public void TurnPageComplete()
    {
        _isTurningPage = false;
        _pageContents[_currentPage].SetActive(true);
        CheckButtons();
    }
}
