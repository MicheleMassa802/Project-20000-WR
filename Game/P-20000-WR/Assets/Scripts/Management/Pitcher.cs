using System;
using UnityEngine;
using static BallLifeCycleManager;

public class Pitcher : MonoBehaviour
{

    public class BallHintPosition : EventArgs
    {
        public Vector2 BallPosition { get; set; }
    }

    public GameObject ballPrefab;
    public GameObject ezBallPrefab;

    public Transform pitcherHand;
    public GameObject strikeZone;
    public static bool easyMode;
    public int timeOut = 12;
    public float throwForce = 1.7f;

    private Options optionsScript;

    private float zVariability;
    private float yVariability;
    private float variableOffset;
    private const float ballWeight = 0.15f;
    private float flightTime;

    private Vector3 dot;
    private Vector2 pitchOffsets;
    private Vector3 pitchLocation;
    private Vector3 handPosition;

    private Vector3 velo;

    public event EventHandler<BallHintPosition> OnThrowPitch;
    public event EventHandler<BallOutcomeData> OnDisplayBallResults;

    // Start is called before the first frame update
    void Start()
    {
        easyMode = true;
        handPosition = pitcherHand.position;

        dot = strikeZone.transform.position;

        // setup args again upon starting game
        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();
        optionsScript.OnGameStart += SetupPitchingSettings;  

        SetupPitchingSettings(null, null);
        InvokeRepeating("Pitch", 3.0f, timeOut);
    }


    private void Pitch()
    {
        GameObject ball = easyMode ? ezBallPrefab : ballPrefab;
        // instantiate a ball directed at the mound
        pitchOffsets = RandomOffsets();
        pitchLocation = RandomLocation(pitchOffsets);
        Vector3 throwDirection = pitchLocation - handPosition;

        float xVelo = throwDirection.x / flightTime;
        float yVelo = (throwDirection.y + (-0.5f * Physics.gravity.y * flightTime * flightTime * ballWeight)) / flightTime;
        // 0.5 is the gravity multiplier on the ball during a pitch
        float zVelo = throwDirection.z / flightTime;
        velo = new Vector3(xVelo, yVelo, zVelo) * throwForce;

        // spawn ball and send position to hint renderer
        Vector2 pitchHintLocation = new Vector2(pitchOffsets.x, pitchOffsets.y);
        OnThrowPitch?.Invoke(this, new BallHintPosition { BallPosition = pitchHintLocation });
        GameObject instantiatedBall = Instantiate(ball, pitcherHand.position, Quaternion.identity);

        // Track the ball's events and echo them to the scorekeeper
        instantiatedBall.GetComponent<BallLifeCycleManager>().OnRegisterBallResults += (obj, eventArgs) => {
            OnDisplayBallResults?.Invoke(obj, eventArgs);
        };

        // apply
        Rigidbody ballRb = instantiatedBall.GetComponent<Rigidbody>();
        ballRb.velocity = velo;
    }

    private Vector2 RandomOffsets()
    {
        return new Vector2(
                UnityEngine.Random.Range(-zVariability, zVariability),
                UnityEngine.Random.Range(-yVariability, yVariability)
        );
    }

    private Vector3 RandomLocation(Vector2 offsets)
    {
        Vector3 pitchLoc = new Vector3(dot.x, dot.y + offsets.y, dot.z + offsets.x);
        return pitchLoc;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(handPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pitchLocation, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(handPosition, pitchLocation);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(handPosition, handPosition + velo);

    }

    private void SetupPitchingSettings(object sender, EventArgs e)
    {

        // wildness settings
        int wildness = PlayerPrefs.GetInt("wildness");

        // come up with the variability ranges based on the player's set wildness
        variableOffset = 0;

        if (wildness >= 90)
        {
            variableOffset = 1.0f;
        }
        else if (wildness >= 70)
        {
            variableOffset = 0.5f;
        }
        else if (wildness >= 50)
        {
            variableOffset = 0.2f;
        }
        // otw, no offset from full strike zone

        // set variability for randomness calculations
        zVariability = 0.0f;
        yVariability = 0.0f;

        if (wildness >= 25)
        {
            // use full strike zone + offset
            zVariability = (strikeZone.transform.localScale.x / 2) + variableOffset;
            yVariability = (strikeZone.transform.localScale.y / 2) + variableOffset;
        }
        else if (wildness >= 1)
        {
            // dots + 0.15 offset
            zVariability = 0.15f;
            yVariability = 0.15f;
        }
        // wildness == 0 means dots


        // speed settings
        int speed = PlayerPrefs.GetInt("speed");

        // 90 mph => 0.5 seconds flight time (realistically)
        // + 0.3f game offset -> to make it fun yo :<)

        flightTime = (90.0f / speed * 0.5f) + 0.3f;
    }

}
