using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Vector2 parallaxEffectMutiplier;
    // ��ȡ������ͷtransform
    private Transform cameraTransform;
    // ��¼���һ�����λ��
    private Vector3 lastCameraPostion;
    private float textureUnitSizeX;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPostion = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        /*
         * texture.width�����ȣ���ʵ��ͼƬ�����ؿ��˴�Ϊ320
         * sprite.pixelsPerUnit������ͼƬ��PPU���˴�Ϊ20
         * textureUnitSizeX��Ҳ�͵õ��˸�ͼƬ��ռ16����Ԫ��
         */
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        // �Ӳ�Ч��
        Vector3 deltaMovement = cameraTransform.position - lastCameraPostion;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMutiplier.x, deltaMovement.y * parallaxEffectMutiplier.y);
        lastCameraPostion = cameraTransform.position;
        
        // ѭ��Ч��
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {// �ƶ�����ͼƬ���
            // �������λ����ͼƬλ�õ�ϸ΢λ�ƣ��Ż��ν��޷�
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
    }
}
