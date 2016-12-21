using Assets.Scripts;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
	private Vector3 _originalPosition;
	private Vector3 _originalScale;
	private bool _isZoomed;

    private ClientGameState GameState
    {
        get { return GameManager.Instance.Client.GameState; }
    }

    public void Zoom()
    {
        if (_isZoomed)
        {
            ZoomOut();
        }
        else
        {
            ZoomIn();
        }
    }

    public void ZoomIn()
    {
        var selectedMonster = GameState.GetSelectedMonsterPrefab();
        if (_isZoomed || selectedMonster == null) return;

        _originalPosition = Camera.main.transform.localPosition;
        _originalScale = Camera.main.transform.localScale;
        Camera.main.transform.localPosition = selectedMonster.transform.localPosition;
        Camera.main.transform.localScale *= 16;
        _isZoomed = true;
    }

    public void ZoomOut()
	{
		if (!_isZoomed) return;

		Camera.main.transform.localPosition = _originalPosition;
		Camera.main.transform.localScale = _originalScale;
		_isZoomed = false;
	}
}