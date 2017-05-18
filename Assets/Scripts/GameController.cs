using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GameController : MonoBehaviour {
    public int maxSize;
    public int currSize;
    public int xBound;
    public int yBound;
    public int score;
    public int fruitScore;
    public GameObject foodPrefab;
    public GameObject currFood;
    public GameObject snakePrefab;
    public Text curScore;
    public Snake head;
    public Snake tail;
    public int direction;
    public Vector2 nextPos;
    public float repTime;

    private int hitCount = 0;
    private KeywordRecognizer kr;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    private string[] test = { "left", "right", "up", "down", "l", "u", "d", "r","you"};

    void OnEnable()
    {
        Snake.hit += Hit;
    }

	// Use this for initialization
	void Start () {
        InvokeRepeating("Timer", 0, repTime);
        InvokeRepeating("ChangeDifficulty", 0, 5.0f);
        SpawnFood();
        keywords.Add("up", () =>
        {
            DireChange(0);
        });
        keywords.Add("down", () =>
        {
            DireChange(2);
        });
        keywords.Add("left", () =>
        {
            DireChange(3);
        });
        keywords.Add("right", () =>
        {
            DireChange(1);
        });

        //kr = new KeywordRecognizer(keywords.Keys.ToArray());
        kr = new KeywordRecognizer(test);
        kr.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        kr.Start();
    }

    void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());

        Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    void OnDisable()
    {
        Snake.hit -= Hit;
    }

    void ChangeDifficulty()
    {
        if (hitCount == 0)
        {
            repTime += 0.05f;
            fruitScore -= 1;
        }

        else
        {
            fruitScore += 1;
        }

        hitCount = 0;
    }

    // Update is called once per frame
    void Update () {
        //DireChange();
        curScore.text = score.ToString();
        if (head.transform.position.x > xBound || head.transform.position.x < -xBound || head.transform.position.y > yBound || head.transform.position.y < -yBound)
        {
            Warp();
        }
        if (Input.GetKeyDown(KeyCode.W))
            DireChange(0);
        if (Input.GetKeyDown(KeyCode.D))
            DireChange(1);
        if (Input.GetKeyDown(KeyCode.S))
            DireChange(2);
        if (Input.GetKeyDown(KeyCode.A))
            DireChange(3);
    }

    void Timer()
    {
        Movement();
        //StartCoroutine(CheckVisible());
        if(currSize >= maxSize)
        {
            MoveTail();
        }
        else
        {
            currSize++;
        }
    }

    void Movement()
    {
        GameObject temp;
        nextPos = head.transform.position;
        switch (direction)
        {
            case 0:
                nextPos = new Vector2(nextPos.x, nextPos.y + 1);
                break;
            case 1:
                nextPos = new Vector2(nextPos.x + 1, nextPos.y);
                break;
            case 2:
                nextPos = new Vector2(nextPos.x, nextPos.y - 1);
                break;
            case 3:
                nextPos = new Vector2(nextPos.x - 1, nextPos.y);
                break;
        }
        temp = (GameObject)Instantiate(snakePrefab, nextPos, transform.rotation);
        head.SetNext(temp.GetComponent<Snake>());
        head = temp.GetComponent<Snake>();

        //return;
    }

    void DireChange(int dire)
    {
        if (direction != 2 && dire == 0)
            direction = 0;
        if (direction != 0 && dire == 2)
            direction = 2;
        if (direction != 1 && dire == 3)
            direction = 3;
        if (direction != 3 && dire == 1)
            direction = 1;
    }

    void MoveTail()
    {
        Snake tempSnake = tail;
        tail = tail.GetNext();
        tempSnake.RemoveTail();
    }

    void SpawnFood()
    {
        int xPos = UnityEngine.Random.Range(-xBound+2, xBound-2);
        int yPos = UnityEngine.Random.Range(-yBound+2, yBound-2);
        currFood = (GameObject)Instantiate(foodPrefab, new Vector2(xPos, yPos), transform.rotation);
        StartCoroutine(CheckRenderer(currFood));
    }

    IEnumerator CheckRenderer(GameObject i)
    {
        yield return new WaitForEndOfFrame();
        if(i.GetComponent<Renderer>().isVisible == false)
        {
            if (i.tag == "food")
            {
                Destroy(i);
                SpawnFood();
            }
        }
    }
    void Hit(string msg)
    {
        if(msg == "food")
        {
            SpawnFood();
            maxSize++;
            hitCount++;
            score += fruitScore;
            if (repTime >= 0.1f)
            {
                repTime = repTime - 0.05f;
                CancelInvoke("Timer");
                InvokeRepeating("Timer", 0, repTime);

            }

        }
        if (msg == "snake")
        {
            CancelInvoke("Timer");
            Exit();
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("menu",LoadSceneMode.Single);
    }

    void Warp()
    {
        if(direction == 0)
        {
            head.transform.position = new Vector2(head.transform.position.x, - (head.transform.position.y - 1));
        }
        else if (direction == 1)
        {
            head.transform.position = new Vector2(- (head.transform.position.x - 1), head.transform.position.y);
        }
        else if (direction == 2)
        {
            head.transform.position = new Vector2(head.transform.position.x, - (head.transform.position.y + 1));
        }
        else if (direction == 3)
        {
            head.transform.position = new Vector2(- (head.transform.position.x + 1), head.transform.position.y);
        }
    }

    IEnumerator CheckVisible()
    {
        yield return new WaitForEndOfFrame();
        if (head.transform.position.x > xBound || head.transform.position.x< -xBound || head.transform.position.y >  yBound || head.transform.position.y < -yBound)
        {
            Warp();
        }
    }
}
