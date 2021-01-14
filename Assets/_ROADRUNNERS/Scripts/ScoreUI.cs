using UnityEngine;
using UnityEngine.UI;

//TODO: Move PlayerPrefs logic to GameManager

public class ScoreUI : MonoBehaviour
{
    [SerializeField] internal GameObject Runner;
    [SerializeField] Text scoreText, highScoreText;

    float _distanceTravelled;

    private void Start()
        => highScoreText.text = $"[{PlayerPrefs.GetFloat("HighScore")} M]";

    private void Update()
    {
        _distanceTravelled = Mathf.Ceil(Runner.transform.position.x) * 2;

        scoreText.text = $"{_distanceTravelled} M";
    }

    private void OnDisable()
    {
        if (_distanceTravelled > PlayerPrefs.GetFloat("HighScore"))
            PlayerPrefs.SetFloat("HighScore", _distanceTravelled);
    }

}
