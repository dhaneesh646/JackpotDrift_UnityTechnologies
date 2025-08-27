using UnityEngine;

public class AnimationSelfOff : MonoBehaviour
{
    public void SelfOff()
    {
        gameObject.SetActive(false);
    }
}
