using UnityEngine;

namespace AHKM.UnityComponents;

/// <summary>
/// Shifts the color of the knight.
/// </summary>
internal class ColorShifter : MonoBehaviour
{
    private float _passedTime = 0f;

    private tk2dSprite _knightSprite;

    void Start() => SetupKnightSprite();

    void Update()
    {
        _passedTime += Time.deltaTime;
        if (_passedTime > 1f)
            _passedTime = 0f;
        if (_knightSprite == null)
        {
            SetupKnightSprite();
            if (_knightSprite == null)
                return;
        }
        _knightSprite.color = Color.HSVToRGB(_passedTime, 1f, 1f);
    }

    private void SetupKnightSprite()
    {
        if (HeroController.instance == null)
            return;
        _knightSprite = HeroController.instance.GetComponent<tk2dSprite>();
    }
}
