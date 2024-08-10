using System;
using System.Collections;
using UnityEngine;

public class BatSwinger : MonoBehaviour
{
    private const float swingAnimationLength = 2.5f;
    private const float swingAnimationSpeed = 2.5f;

    // event arg
    public class RegisterSideSwitchEventArgs : EventArgs
    {
        public bool IsRighty { get; set; }
    }

    private Animator batAnimController;
    private Options optionsScript;  // fires off start event
    private MovePlayer movePlayerScript;  // fires off unlocking event
    private DetectBatClient detectBatClient;
    private DetectMouseInput detectMouseInput;

    private bool isRighty = true;

    public bool isSwingAnimationRunning = false;
    private float swingAnimationTime = swingAnimationLength / swingAnimationSpeed;

    [SerializeField] private bool checkForSwing = false;

    public event EventHandler<RegisterSideSwitchEventArgs> OnRegisterSideSwitch;

    // Start is called before the first frame update
    void Start()
    {
        batAnimController = GetComponent<Animator>();

        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();
        detectMouseInput = transform.parent.GetComponent<DetectMouseInput>();
        detectBatClient = transform.parent.GetComponent<DetectBatClient>();

        // event handling
        optionsScript.OnGameStart += AtBatStart;
        movePlayerScript.OnPlayerUnlock += AtBatEnd;
    }


    private void Update()
    {
        if (isSwingAnimationRunning)
        {
            return; 
        }

        if (checkForSwing)
        {
            if (detectMouseInput.batMKSwung || detectBatClient.batTMSwung)
            {
                // generate a swing of the bat via an animation
                string swingAnim = isRighty ? "BatRighty" : "BatLefty";
                string idleAnim = isRighty ? "PrepRighty" : "PrepLefty";

                // play animations
                batAnimController.Play(swingAnim);
                StartCoroutine(nameof(TrackSwingAnimation));
                // immediately play idle to return to idle state after swing
                // (as the transitions have exit time)
                batAnimController.Play(idleAnim);
            }
        }
    }


    private void AtBatStart(object sender, EventArgs e)
    {
        // determine batting side
        int rightyInt = PlayerPrefs.GetInt("isRighty");  // updated after player steps in
        isRighty = rightyInt == 1;
        // shoot of side switch event
        OnRegisterSideSwitch?.Invoke(this, new RegisterSideSwitchEventArgs { IsRighty=isRighty });

        // go into prep <correct side> animation
        string idleAnim = isRighty ? "PrepRighty" : "PrepLefty";
        batAnimController.Play(idleAnim);

        checkForSwing = true;  // start checking for swings
            
    }

    private void AtBatEnd(object sender, EventArgs e)
    {
        // stop checking for swing
        checkForSwing = false;
        // go back to idle animation
        batAnimController.Play("Idle");
    }

    IEnumerator TrackSwingAnimation()
    {
        isSwingAnimationRunning = true;
        yield return new WaitForSeconds(swingAnimationTime);
        isSwingAnimationRunning = false;
    }
}
