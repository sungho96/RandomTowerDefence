using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPTextBillBoard : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main; // MainCamera 태그를 가진 카메라 참조
    }
    /// <summary>
    /// 매 프레임 LateUpdate에서 UI(텍스트)가 카메라를 향하도록 회전(빌보드).
    /// </summary>
    private void LateUpdate()
    {
        if (cam == null) return;
        transform.forward = cam.transform.forward;
    }
}
