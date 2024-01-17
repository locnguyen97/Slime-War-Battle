using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool canDrag = true;
    private int startIndex = 0;

    private int currentIndex;
    public  List<GameObject> particleVFXs;
    [SerializeField] private List<GameLevel> levels;
    
    private TouchPoint head;

    public string curColor = "";
    public int curSlot = 0;
    public List<Car> listCurCar = new List<Car>();
    public int idxGame = 0;
    [SerializeField] private SpriteAtlas al;
    [SerializeField] private List<string> listtotalColor;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        currentIndex = startIndex;
        levels[currentIndex].gameObject.SetActive(true);
        StartCoroutine(CheckAllType());
    }

    public void CheckLevelUp()
    {
        canDrag = false;
        GameObject explosion = Instantiate(particleVFXs[Random.Range(0,particleVFXs.Count)], transform.position, transform.rotation);
        Destroy(explosion, .75f);
        Invoke(nameof(NextLevel),1.0f);
    }

    void ChoseStartPoint()
    {
        ChoseNextPoint();
    }

    void NextLevel()
    {
        levels[currentIndex].gameObject.SetActive(false);
        currentIndex++;
        idxGame = 0;
        if (currentIndex >= 3)
        {
            currentIndex = startIndex;
            canDrag = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            levels[currentIndex].gameObject.SetActive(true);
            canDrag = true;
            StartCoroutine(CheckAllType());
        }
    }

    IEnumerator CheckAllType()
    {
        yield return new WaitUntil(()=> GetCurLevel().gameObjectsPoint.Count >0);
        listtotalColor.Clear();
        foreach (var tr in GetCurLevel().gameObjectsPoint)
        {
            listtotalColor.Add(tr.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name.Remove(2));
        }
        ChoseStartPoint();
    }
    
    Vector3 offset;

    void Update()
    {
        if(!canDrag) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.y = 0;
        /*if (Input.GetMouseButtonDown(0))
        {
            var targetObject = Physics.OverlapSphere(mousePosition,0.2f);
            booxx.position = mousePosition;
            if (targetObject != null)
            {
                if (targetObject.Length != 0)
                {
                    var tp = targetObject[0].GetComponent<Car>();
                    if (tp != null)
                    {
                        if (tp.index == idxGame)
                        {
                            tp.Move();
                            canDrag = false;
                        }
                    }
                }
            }
        }*/
    }
    

    public void EnableDrag()
    {
        canDrag = true;
    }
    public GameLevel GetCurLevel()
    {
        return levels[currentIndex];
    }

    public void ChoseRight(Sprite spr)
    {
        GetCurLevel().gameObjectsSlot[idxGame].GetComponent<SpriteRenderer>().sprite = spr;
        GetCurLevel().gameObjectsSlot[idxGame].SetActive(true);
        idxGame++;
        var x = listtotalColor.Find(l => l == spr.name.Remove(2));
        if (x != null)
            listtotalColor.Remove(x);
        if (idxGame >= GetCurLevel().gameObjectsSlot.Count)
        {
            GetCurLevel().need.gameObject.SetActive(false);
            CheckLevelUp();
        }
        else
        {
            ChoseNextPoint();
        }
    }

    void ChoseNextPoint()
    {
        EnableDrag();
        curColor = listtotalColor[Random.Range(0, listtotalColor.Count)];
        GetCurLevel().need.sprite = al.GetSprite(curColor);
    }
}