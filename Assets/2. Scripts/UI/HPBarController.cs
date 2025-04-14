using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Image _hpGauge;
    [SerializeField] private bool isWorldCanvas;
    private void Update()
    {
        if (isWorldCanvas)
        {
            var cameraTransform = Camera.main.transform;//RenderMove가 world space 몬스터 hp
            transform.rotation = cameraTransform.rotation;
        }
        
        
    }
    public void SetHP(float hp)
    {
        _hpGauge.fillAmount = hp;
    }
}
