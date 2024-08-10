using UnityEngine;

public class RenderHint : MonoBehaviour
{
    public GameObject hintPrefab;
    private Pitcher pitcherScript;

    private const string pitcherName = "PitchingManager";
    private const float hintTimeout = 4f;

    private Vector2 uiPosition = Vector2.zero;
    private GameObject hintJustRendered;
    private RectTransform hintRectTransform;

    void Start()
    {
        pitcherScript = GameObject.Find(pitcherName)?.GetComponent<Pitcher>();
        pitcherScript.OnThrowPitch += DisplayHint;
    }

    private void DisplayHint(object sender, Pitcher.BallHintPosition hintPosition)
    {
        if (hintPrefab == null || pitcherScript == null) 
        {
            Debug.Log("Failed to set hint prefab or pitcher script");
            return;
        }

        // spawn hint on canvas and set the position
        hintJustRendered = Instantiate(hintPrefab, transform);
        hintRectTransform = hintJustRendered.GetComponent<RectTransform>();

        if (hintRectTransform != null )
        {
            // set hint position using the provided in-game position of the pitch
            uiPosition = BatPositionUtil.GameToPixelStrikeZoneOffsets(hintPosition.BallPosition);
            hintRectTransform.anchoredPosition = uiPosition;
        }

        // destroy after timeout before the next pitch
        Destroy(hintJustRendered, hintTimeout);
    }
}
