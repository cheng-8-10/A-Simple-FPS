using UnityEngine;

public class PlayeCamera : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private GameObject player;

    [Header("鼠标")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private bool autoLockCursor = true;

    [Header("眼睛高度（相对 Player 的世界偏移）")]
    [SerializeField] private Vector3 eyeOffset = new Vector3(0f, 1.6f, 0f);

    private float pitch;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("镜头脚本没有指定要跟随的 player", this);
            enabled = false;
            return;
        }

        // 把 Main Camera 挂到 Player 下面，让它自动跟随移动和水平转动
        transform.SetParent(player.transform, false);

        // 第一人称下隐藏角色自己的身体，避免挡住镜头
        HideFirstPersonBody();

        if (autoLockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 摆到“眼睛”高度：世界偏移转成 Player 局部坐标，由 SetParent 后的 transform 自动换算
        transform.position = player.transform.position + player.transform.rotation * eyeOffset;
        transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        // Esc 释放鼠标，左键点击后重新锁定
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 鼠标没有锁定时不转动视角（方便操作 UI）
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 水平转动：转动 Player 本身，这样 W 的前进方向也会跟着视角变
        player.transform.Rotate(0f, mouseX, 0f, Space.World);

        // 垂直转动：只改变镜头的俯仰角，不转 Player
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 因为镜头已经是 Player 的子物体，左右角度继承 Player，这里只需设自己的俯仰
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HideFirstPersonBody()
    {
        // 只隐藏 player 这个角色身体节点，避免误关挂在 Camera/Player 下的枪等模型
        Transform body = player.transform.Find("player");
        if (body == null)
        {
            return;
        }

        Renderer[] renderers = body.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }
}
