using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Vector2 parallaxEffectMutiplier;
    // 获取主摄像头transform
    private Transform cameraTransform;
    // 记录最后一个相机位置
    private Vector3 lastCameraPostion;
    private float textureUnitSizeX;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPostion = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        /*
         * texture.width纹理宽度，即实际图片的像素宽，此处为320
         * sprite.pixelsPerUnit，即该图片的PPU，此处为20
         * textureUnitSizeX，也就得到了该图片宽占16个单元格
         */
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        // 视差效果
        Vector3 deltaMovement = cameraTransform.position - lastCameraPostion;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMutiplier.x, deltaMovement.y * parallaxEffectMutiplier.y);
        lastCameraPostion = cameraTransform.position;
        
        // 循环效果
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {// 移动超过图片宽度
            // 计算相机位置与图片位置的细微位移，优化衔接无缝
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
    }
}
