using Assets.Scripts;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
	private Vector3 _originalPosition;
    private Quaternion _originalRotation;
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

        // update camera location & rotation
        GameObject mainCamera = GameObject.Find("CameraContainer");
        _originalPosition = mainCamera.transform.localPosition;
        _originalRotation = mainCamera.transform.localRotation;

        mainCamera.transform.localPosition = new Vector3(selectedMonster.transform.localPosition.x, 
            selectedMonster.transform.localPosition.y + .1f, selectedMonster.transform.localPosition.z);
        mainCamera.transform.localRotation = new Quaternion(_originalRotation.x, _originalRotation.y, _originalRotation.z, _originalRotation.w);

        _isZoomed = true;
    }

    public void ZoomOut()
	{
		if (!_isZoomed) return;

        // update camera location
	    var camera = GameObject.Find("CameraContainer");
        camera.transform.localPosition = _originalPosition;
        camera.transform.localRotation = new Quaternion(_originalRotation.x, _originalRotation.y, _originalRotation.z, _originalRotation.w);

        _isZoomed = false;
	}
}