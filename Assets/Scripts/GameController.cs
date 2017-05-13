using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public int maxSize;
    public int currSize;
    public int xBound;
    public int yBound;
    public int score;
    public GameObject foodPrefab;
    public GameObject currFood;
    public GameObject snakePrefab;
    public Text curScore;
    public Snake head;
    public Snake tail;
    public int direction;
    public Vector2 nextPos;

    void OnEnable()
    {
        Snake.hit += Hit;
    }

	// Use this for initialization
	void Start () {
        InvokeRepeating("Timer", 0, .5f);
        SpawnFood();
	}

    void OnDisable()
    {
        Snake.hit -= Hit;
    }

    // Update is called once per frame
    void Update () {
        DireChange();
        curScore.text = score.ToString();
        if (head.transform.position.x > xBound || head.transform.position.x < -xBound || head.transform.position.y > yBound || head.transform.position.y < -yBound)
        {
            Warp();
        }
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

    void DireChange()
    {
        if (direction != 2 && Input.GetKeyDown(KeyCode.W))
            direction = 0;
        if (direction != 0 && Input.GetKeyDown(KeyCode.S))
            direction = 2;
        if (direction != 1 && Input.GetKeyDown(KeyCode.A))
            direction = 3;
        if (direction != 3 && Input.GetKeyDown(KeyCode.D))
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
        int xPos = Random.Range(-xBound, xBound);
        int yPos = Random.Range(-yBound, yBound);
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
            score++;
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
