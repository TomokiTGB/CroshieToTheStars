using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState { Idle, Walking, Hit }
//public enum DirectionState { Left, Down, Up, Right }
public class CoroutineAnimation : MonoBehaviour
{
    public SpriteRenderer sR;
    public float fps;

    public AnimationState animState;
    [HideInInspector] public AnimationState animStatePrev;
    
    void Awake()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {

    }
    // Update is called once per frame
    void LateUpdate()
    {
        animStatePrev = animState;
    }

    #region Animation
    public void StartAnimationLoop(List<Sprite> sprites)
    {
        StopAllCoroutines();
        StartCoroutine(AnimationLoop(sprites));
    }
    IEnumerator AnimationLoop(List<Sprite> sprites)
    {
        while (true)
        {
            foreach (Sprite sprite in sprites)
            {
                sR.sprite = sprite;
                yield return new WaitForSeconds(1 / fps);
            }
        }
    }
    #endregion
}
[System.Serializable]
public class DirectionalAnimation
{
    public List<Sprite> left;
    public List<Sprite> down;
    public List<Sprite> up;

}