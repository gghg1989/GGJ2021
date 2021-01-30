using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    float spacingInterval;
    Vector3 targetPosition;
    Vector3 currentPosition;

    public float lerpTime;
    LevelGenerator theLevelGenerator;

    GameManager theGameManager;

    [SerializeField] bool isMoving;

    AudioManager theAudioManager;
    public float bpm;
    TimerClass bpmTimer;

    AudioSource compSource;
    AudioSource highHatSource;
    AudioSource chordSource;

    public float timerTolerance;

    public int currentAttack;
    public int currentHealth;

    public int previousAttack;
    public int previousHealth;

    public int maxAttack;
    float secondsPerEighthNote;
    float secondsPerBeat;

    public int numberOfNotesToBuffer;

    public GameObject theSword;
    GameObject theSwordInstance;

    public int playerLife;
    public int maxPlayerLife;

    bool isHit;

    TimerClass hitTimer;

    public float hurtTime;

    SpriteRenderer playerSpriteRenderer;

    GameObject heart1;
    GameObject heart2;
    GameObject heart3;
    GameObject heart4;

    InputManager theInputManager;

    bool isInvulnerable;

    public bool isKillable;

    public bool tutorialMode;
    
    enum arrowNotes
    {
        Up,
        Down,
        Left,
        Right,
        Null
        
    }

    arrowNotes[] noteBuffer;

    int noteIndex;

    arrowNotes[] attackUp1;
    arrowNotes[] attackDown2;
    arrowNotes[] attackLeft3;
    arrowNotes[] attackRight4;
    arrowNotes[] attackSpecialFirst5;
    arrowNotes[] attackSpecialSecond6;

    double nextNote;
    bool noteWillPlay;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        currentPosition = transform.position;
        
        theLevelGenerator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();
        spacingInterval = theLevelGenerator.spacingInterval;

        theAudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        compSource = GameObject.FindGameObjectWithTag("CompSource").GetComponent<AudioSource>();
        chordSource = GameObject.Find("ChordLow").GetComponent<AudioSource>();

        theInputManager = GameObject.Find("InputManager").GetComponent<InputManager>();


        highHatSource = GameObject.FindGameObjectWithTag("HighHatSource").GetComponent<AudioSource>();

        noteBuffer = new arrowNotes[numberOfNotesToBuffer];
        noteIndex = 0;

        attackUp1 = new arrowNotes[] { arrowNotes.Up, arrowNotes.Left, arrowNotes.Right };
        //attackUp1 = new arrowNotes[] { arrowNotes.Up, arrowNotes.Up, arrowNotes.Up };
        attackDown2 = new arrowNotes[] { arrowNotes.Down, arrowNotes.Left, arrowNotes.Right };
        //attackDown2 = new arrowNotes[] { arrowNotes.Down, arrowNotes.Down, arrowNotes.Down };
        attackLeft3 = new arrowNotes[] { arrowNotes.Left, arrowNotes.Down, arrowNotes.Right };
        attackRight4 = new arrowNotes[] { arrowNotes.Right, arrowNotes.Down, arrowNotes.Left };
        attackSpecialFirst5 = new arrowNotes[] { arrowNotes.Right, arrowNotes.Up, arrowNotes.Left, arrowNotes.Right };
        attackSpecialSecond6 = new arrowNotes[] { arrowNotes.Right, arrowNotes.Left, arrowNotes.Right, arrowNotes.Left };
        hitTimer = new TimerClass();

        theGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        heart1 = GameObject.Find("Heart1");
        heart2 = GameObject.Find("Heart2");
        heart3 = GameObject.Find("Heart3");
        heart4 = GameObject.Find("Heart4");
        nextNote = theAudioManager.nextNoteTime;

    }

    void Update()
    {


        PollForInput();
        
        //MoveToTarget();

        CheckPlayerState();
        
    }

    void FixedUpdate()
    {
        MoveToTarget();    
    }


    void PollForInput()
    {
        int notePatternOutput = 0;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!isMoving)
            {
                targetPosition = new Vector3(currentPosition.x, currentPosition.y + spacingInterval, 0);
                isMoving = true;
                //theAudioManager.Play2ndNoteUp();

            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!isMoving)
            {
                targetPosition = new Vector3(currentPosition.x, currentPosition.y - spacingInterval, 0);
                isMoving = true;
                //theAudioManager.Play2ndNoteDown();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.A))
        {

            if (!isMoving)
            {
                targetPosition = new Vector3(currentPosition.x - spacingInterval, currentPosition.y, 0);
                isMoving = true;
                //theAudioManager.PlayNextNoteDown();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!isMoving)
            {
                targetPosition = new Vector3(currentPosition.x + spacingInterval, currentPosition.y, 0);
                isMoving = true;
                //theAudioManager.PlayNextNoteUp();
                
            }
            
           
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            theAudioManager.Play2ndNoteUp();
            AddToNoteBuffer(arrowNotes.Up);
            notePatternOutput = CheckNotePattern();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            theAudioManager.Play2ndNoteDown();
            AddToNoteBuffer(arrowNotes.Down);
            notePatternOutput = CheckNotePattern();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            theAudioManager.PlayNextNoteDown();
            AddToNoteBuffer(arrowNotes.Left);
            notePatternOutput = CheckNotePattern();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            theAudioManager.PlayNextNoteUp();
            AddToNoteBuffer(arrowNotes.Right);
            notePatternOutput = CheckNotePattern();
        }

        if(notePatternOutput != 0)
        {
            theAudioManager.PlaySuccess();
            noteWillPlay = true;
            nextNote = theAudioManager.nextNoteTime;
            CheckAttack(notePatternOutput);
            ClearNoteBuffer();
        }

        if(noteWillPlay && AudioSettings.dspTime > nextNote)
        {
            ClearNoteBuffer();
            noteWillPlay = false;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //check note buffer for move sequence
        //    if(CheckNotePattern() == 1)
        //    {
        //        //play chord
        //        theAudioManager.PlaySuccess();
        //        return;
        //    }

        //    if(CheckNotePattern() == 2)
        //    {
        //        theAudioManager.PlaySuccess();
        //        return;
        //    }

        //    else
        //    {
        //        theAudioManager.PlayFail();
        //        return;
        //    }

        //}



    }

    void MoveToTarget()
    {

        if (!IsValidMove(targetPosition))
        {
            isMoving = false;
            targetPosition = currentPosition;
            return;
        }

        if (IsValidMove(targetPosition))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, lerpTime*Time.deltaTime);

        }


        if (transform.position == targetPosition)
        {
            isMoving = false;
            currentPosition = targetPosition;
        }



    }

    bool IsValidMove(Vector3 targetPosition)
    {
        bool validMove;
        
        int xCoordinate;
        int yCoordinate;

        xCoordinate = (int)(targetPosition.x / theLevelGenerator.spacingInterval);
        yCoordinate = (int)(targetPosition.y / theLevelGenerator.spacingInterval);

        if (theLevelGenerator.levelLayout[xCoordinate, yCoordinate])
        {
            validMove = true;
        }

        else
        {
            validMove = false;
        }



        return validMove;
    }

    void CheckForAttackChange()
    {

    }

    void AddToNoteBuffer(arrowNotes theArrow)
    {
        noteBuffer[noteIndex] = theArrow;
        noteIndex++;

        if(noteIndex >= numberOfNotesToBuffer)
        {
            noteIndex = 0;
        }

    }

    int CheckNotePattern()
    {
        int[] patternSequence = new int[numberOfNotesToBuffer];
        int wrappedNoteIndex;

        wrappedNoteIndex = noteIndex;

        //bool[] isNoteCorrect = new bool[numberOfNotesToBuffer];
        int numberofCorrectNotesPattern1 = 0;
        int numberofCorrectNotesPattern2 = 0;
        int numberofCorrectNotesPattern3 = 0;
        int numberofCorrectNotesPattern4 = 0;
        int numberofCorrectNotesPattern5 = 0;
        int numberofCorrectNotesPattern6 = 0;

        for (int x = 0; x< numberOfNotesToBuffer; x++)
        {
            wrappedNoteIndex--;
            if (wrappedNoteIndex < 0)
            {
                wrappedNoteIndex = numberOfNotesToBuffer - 1;
            }

            patternSequence[x] = wrappedNoteIndex;
            
        }

        for(int x = 0; x < 4; x++)
        {

            if (x < 3)
            {
                if (noteBuffer[patternSequence[x]] == attackUp1[x])
                {
                    numberofCorrectNotesPattern1++;
                }
                if (numberofCorrectNotesPattern1 > 2)
                {
                    return 1;
                }

                if (noteBuffer[patternSequence[x]] == attackDown2[x])
                {
                    numberofCorrectNotesPattern2++;
                }
                if (numberofCorrectNotesPattern2 > 2)
                {
                    return 2;
                }

                if (noteBuffer[patternSequence[x]] == attackLeft3[x])
                {
                    numberofCorrectNotesPattern3++;
                }
                if (numberofCorrectNotesPattern3 > 2)
                {
                    return 3;
                }
                
                if (noteBuffer[patternSequence[x]] == attackRight4[x])
                {
                    numberofCorrectNotesPattern4++;
                }
                if (numberofCorrectNotesPattern4 > 2)
                {
                    return 4;
                }
            }





            

            

            if (noteBuffer[patternSequence[x]] == attackSpecialFirst5[x])
            {
                numberofCorrectNotesPattern5++;
            }
            if (numberofCorrectNotesPattern5 > 3)
            {
                return 5;
            }

            if (noteBuffer[patternSequence[x]] == attackSpecialSecond6[x])
            {
                numberofCorrectNotesPattern6++;
            }
            if (numberofCorrectNotesPattern6 > 3)
            {
                return 6;
            }

        }

        return 0;





    }

    void CheckAttack(int attackType)
    {
        switch (attackType)
        {
            case 1:
                AttackUp();
                break;
            case 2:
                AttackDown();
                break;
            case 3:
                AttackLeft();
                break;
            case 4:
                AttackRight();
                break;
            default:
                break;
        }
    }

    void AttackUp()
    {
        theSwordInstance = Instantiate(theSword);
        theSwordInstance.transform.position = gameObject.transform.position;
        theSwordInstance.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    void AttackDown()
    {
        theSwordInstance = Instantiate(theSword);
        theSwordInstance.transform.position = gameObject.transform.position;
        theSwordInstance.transform.eulerAngles = new Vector3(0f, 0f, 180f);
    }

    void AttackLeft()
    {
        theSwordInstance = Instantiate(theSword);
        theSwordInstance.transform.position = gameObject.transform.position;
        theSwordInstance.transform.eulerAngles = new Vector3(0f, 0f, 90f);
    }

    void AttackRight()
    {
        theSwordInstance = Instantiate(theSword);
        theSwordInstance.transform.position = gameObject.transform.position;
        theSwordInstance.transform.eulerAngles = new Vector3(0f, 0f, 270f);
    }

    void ClearNoteBuffer()
    {
        for(int x = 0; x< noteBuffer.Length; x++)
        {
            noteBuffer[x] = arrowNotes.Null;
        }
    }

    void PushPlayerBack(int direction)
    {

        switch (direction)
        {
            case 1:
                targetPosition = new Vector3(currentPosition.x + spacingInterval, currentPosition.y, 0);
                isMoving = true;
                break;
            case 2:
                targetPosition = new Vector3(currentPosition.x - spacingInterval, currentPosition.y, 0);
                isMoving = true;
                break;
            case 3:
                targetPosition = new Vector3(currentPosition.x , currentPosition.y + spacingInterval, 0);
                isMoving = true;
                break;
            case 4:
                targetPosition = new Vector3(currentPosition.x, currentPosition.y - spacingInterval, 0);
                isMoving = true;
                break;
        }

        MoveToTarget();
        

    }

    int DeterminePushDirection(Vector3 enemyPosition)
    {
        //1 = +x
        //2 = -x
        //3 = +y
        //4 = -y



        int playerPushDirection = 0;
        Vector3 enemyDirection;

        enemyDirection = gameObject.transform.position - enemyPosition;


        if (Mathf.Abs(enemyDirection.x) >= Mathf.Abs(enemyDirection.y))
        {
            //move in +x or -x

            if (enemyDirection.x >= 0)
            {
                playerPushDirection = 1;

            }

            if (enemyDirection.x < 0)
            {
                playerPushDirection = 2;
            }

        }

        if (Mathf.Abs(enemyDirection.x) < Mathf.Abs(enemyDirection.y))
        {
            //move in +x or -x

            if (enemyDirection.y >= 0)
            {
                playerPushDirection = 3;
            }

            if (enemyDirection.y < 0)
            {
                playerPushDirection = 4;
            }

        }

        return playerPushDirection;
    }


    void PlayerHit(int damage)
    {
        //play player hit sound
        isHit = true;
        if (!isInvulnerable)
        {
            playerLife -= damage;
        } 
            

        DisplayHearts();
        if(playerLife == 0)
        {
            PlayerDeath();
        }

    }

    void PlayerDeath()
    {
        //play death sound
        //end scene
        if (isKillable)
        {
            Debug.Log("player death");
            theInputManager.PlayerDeath();

        }
        

    }

    void ChangeHealth(int healthChange)
    {

        playerLife += healthChange;
        DisplayHearts();

        if(playerLife <= 0)
        {
            PlayerDeath();
        }

        if(playerLife > maxPlayerLife)
        {
            playerLife = maxPlayerLife;
        }


    }

    void CheckPlayerState()
    {
        if (isHit)
        {
            hitTimer.StartTimer();
            playerSpriteRenderer.color = Color.red;
            isHit = false;
            isInvulnerable = true;
        }

        if(hitTimer.GetTime() > hurtTime)
        {
            playerSpriteRenderer.color = Color.white;
            hitTimer.ResetTimer();
            hitTimer.StopTimer();
            isInvulnerable = false;
        }


    }

    void DisplayHearts()
    {
        switch (playerLife)
        {
            case 1:
                heart1.SetActive(true);
                heart2.SetActive(false);
                heart3.SetActive(false);
                heart4.SetActive(false);
                break;
            case 2:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(false);
                heart4.SetActive(false);
                break;
            case 3:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(true);
                heart4.SetActive(false);
                break;
            case 4:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(true);
                heart4.SetActive(true);
                break;
            default:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(true);
                heart4.SetActive(true);
                break;

        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        int pushDirection;
        int damage;


        if(collision.gameObject.tag == "Enemy" && !tutorialMode)
        {
            pushDirection = DeterminePushDirection(collision.transform.position);
            PushPlayerBack(pushDirection);

            damage = collision.gameObject.GetComponent<EnemyScript>().enemyAttackPower;

            PlayerHit(damage);


        }

        if(collision.gameObject.tag == "HeartPickup")
        {
            ChangeHealth(1);
            collision.gameObject.SetActive(false);
        }


        if(collision.gameObject.tag == "Exit")
        {

            if (tutorialMode)
            {
                theGameManager.LoadMainLevel();
            }
            else
            {
                theGameManager.LoadNextLevel();
            }
            
            
        }

    }
}
