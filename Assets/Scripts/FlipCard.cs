﻿using UnityEngine;
using System.Collections;

public class FlipCard : MonoBehaviour {

    public int fps = 60;
    public float rotateDegreePerSecond;
    public bool isFaceUp = false;
    float alphaSpeed;

    const float FLIP_LIMIT_DEGREE = 180f;

    float waitTime;
    bool isAnimationProcessing = false;
    bool cw;
    int mode;
    public bool isReady;

    public Quaternion originalRotationValue;

    public Transform cardEffect;

    public Object holyblast;

	// Use this for initialization
	void Start () {
        waitTime = 1.0f / fps;
        rotateDegreePerSecond = 180f;
        originalRotationValue = transform.rotation;
        cw = true;
        alphaSpeed = 0.03f;
        isReady = false;
       // manager = GameObject.FindWithTag("Manager").GetComponent<CameraScript>();
	}
	
	// Update is called once per frame
	void Update () {
      
	}

    void OnMouseOver()
    {
        if (isAnimationProcessing || isFaceUp || !isReady)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            mode = 0;
            OnLeftClick();
        }

        if (Input.GetMouseButton(1))
        {
            mode = 1;
            OnRightClick();
        }

        if (Input.GetMouseButton(2))
            Debug.Log("Pressed middle click.");
    }

    void OnRightClick()
    {
        RandomCard randCard = GetComponentInChildren<RandomCard>();
        int cardObtained = randCard.Index;
        randCard.GetComponent<Transform>().localScale = new Vector3(randCard.factor_x, randCard.factor_y, randCard.factor_z);
        CameraScript manager = GameObject.FindWithTag("Manager").GetComponent<CameraScript>();
        manager.addCard(cardObtained);
        StartCoroutine(alpha());
    }

    IEnumerator alpha()
    {
        float alpha = 1.0f;
        isAnimationProcessing = true;

        bool done = false;
        while (!done)
        {
            alpha -= alphaSpeed;
            BackCard backCard = GetComponentInChildren<BackCard>();
            SpriteRenderer sprite = backCard.GetComponent<SpriteRenderer>();
            sprite.color = new Vector4(1f, 1f, 1f, alpha);
            if (sprite.color.a < 0.01f)
            {
                done = true;
            }
         
            yield return new WaitForSeconds(waitTime);
        }
        CameraScript manager = GameObject.FindWithTag("Manager").GetComponent<CameraScript>();
        manager.cardCounter();
        isFaceUp = true;
        isAnimationProcessing = false;
    }

    void OnLeftClick()
    {
        CameraScript manager = GameObject.FindWithTag("Manager").GetComponent<CameraScript>();
        int cardObtained = GetComponentInChildren<RandomCard>().Index;
        manager.addCard(cardObtained);
        StartCoroutine( flip() );
    }

    IEnumerator flip()
    {
        isAnimationProcessing = true;
        bool done = false;
        while (!done)
        {
  
            float degree = rotateDegreePerSecond * Time.deltaTime;

            if (cw)
            {
                transform.Rotate(new Vector3(0, degree, 0));

                if (FLIP_LIMIT_DEGREE < transform.eulerAngles.y)
                {
                    transform.Rotate(new Vector3(0, -(transform.eulerAngles.y - FLIP_LIMIT_DEGREE), 0));
                    done = true;
                }
            }
            else
            {
                transform.Rotate(new Vector3(0, -degree, 0));

                if (FLIP_LIMIT_DEGREE > transform.eulerAngles.y)
                {
                    transform.Rotate(new Vector3(0, -(transform.eulerAngles.y - FLIP_LIMIT_DEGREE), 0));
                    done = true;
                }
            }
            yield return new WaitForSeconds(waitTime);
        }
        CameraScript manager = GameObject.FindWithTag("Manager").GetComponent<CameraScript>();
        manager.cardCounter();
        isFaceUp = true;
        isAnimationProcessing = false;
    }
    public void resetCard()
    {
        if (mode == 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, Time.time * 1.0f);
        }
        else if (mode == 1)
        {
            //RandomCard randCard = GetComponentInChildren<RandomCard>();
           // randCard.GetComponent<Transform>().localScale = new Vector3(-1f, 1f, 1f);
            BackCard backCard = GetComponentInChildren<BackCard>();
            SpriteRenderer sprite = backCard.GetComponent<SpriteRenderer>();
            sprite.color = new Vector4(1f, 1f, 1f, 1f);
        }
        isFaceUp = false;
        MainCard mainCard = GetComponent<MainCard>();
        mainCard.moved = false;
        mainCard.resetZoom();
        isReady = false;
        GetComponentInChildren<RandomCard>().randomizeCards();
    }

    public void SpeedSlide(float speed)
    {
        rotateDegreePerSecond = speed;
    }
    public void changeRotation()
    {
        cw = !cw;
    }

    public bool isFlipped()
    {
        if (isAnimationProcessing || isFaceUp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AlphaSlide(float speed)
    {
        alphaSpeed = speed;
    }

    public void endCard()
    {
        cardEffect.localScale = new Vector3(10f, 10f, 1f);
        float time = 1.5f;
        holyblast = Instantiate(cardEffect, transform.position, transform.rotation);
        StartCoroutine(waitForEnd(time));
    }

    IEnumerator waitForEnd(float time)
    {
        float waitTime = 0;
        while (waitTime < time)
        {
            waitTime += Time.deltaTime;
            yield return new WaitForSeconds(1.0f/60);
        }
        Destroy((holyblast as Transform).gameObject);
    }
}
