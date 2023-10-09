using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {None, Walkable, Buildable}
public class BoardTile : MonoBehaviour
{
    private Vector2 id;

    private Dictionary<Material, UnityEngine.Color> defaultColors = new Dictionary<Material, UnityEngine.Color>();
    private bool blinking = false;

    public virtual void SetUp(Vector2 id)
    {
        this.id = id;
        blinking = false;

        if (Application.isPlaying)
        {
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                foreach (Material mat in renderer.materials)
                {
                    defaultColors.Add(mat, mat.color);
                }
            }
        }
    }

    public bool Is(Vector2 id)
    {
        if (this.id == id)
        {
            return true;
        }

        return false;
    }


    private void Update()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate()
    {

    }

    public void StartBlinking(List<Color> blinkColors, float defaultBlinkDuration = 1f, float initialBlinkDuration = 0.3f, bool autoEnd = false)
    {
        if (blinking)
        {
            return;
        }
        blinking = true;

        StartCoroutine(Blink());
        IEnumerator Blink()
        {
            int currentColorIndex = 0;
            yield return StartCoroutine(LerpToColor(blinkColors[currentColorIndex], initialBlinkDuration));

            do
            {
                currentColorIndex++;
                if (currentColorIndex > blinkColors.Count - 1)
                {
                    currentColorIndex = 0;
                }

                yield return StartCoroutine(LerpToColor(blinkColors[currentColorIndex], defaultBlinkDuration));
            }
            while (blinking && !autoEnd);

            yield return StartCoroutine(LerpToDefaultColor(initialBlinkDuration));
            blinking = false;
        }

        #region subfunctions
        IEnumerator LerpToColor(UnityEngine.Color targetColor, float blinkDuration)
        {
            float currentTime = 0;

            Dictionary<Material, UnityEngine.Color> previousColor = GetCurrentColors();

            while (currentTime < blinkDuration && blinking)
            {
                foreach (KeyValuePair<Material, UnityEngine.Color> pair in defaultColors)
                {
                    pair.Key.color = UnityEngine.Color.Lerp(previousColor[pair.Key], targetColor, currentTime / blinkDuration);
                }

                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        IEnumerator LerpToDefaultColor(float blinkDuration)
        {
            float currentTime = 0;

            Dictionary<Material, UnityEngine.Color> previousColor = GetCurrentColors();

            while (currentTime < blinkDuration)
            {
                foreach (KeyValuePair<Material, UnityEngine.Color> pair in defaultColors)
                {
                    pair.Key.color = UnityEngine.Color.Lerp(previousColor[pair.Key], defaultColors[pair.Key], currentTime / blinkDuration);
                }

                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        Dictionary<Material, UnityEngine.Color> GetCurrentColors()
        {
            Dictionary<Material, UnityEngine.Color> colors = new Dictionary<Material, UnityEngine.Color>();
            foreach (KeyValuePair<Material, UnityEngine.Color> pair in defaultColors)
            {
                colors.Add(pair.Key, pair.Key.color);
            }
            return colors;
        }
        #endregion
    }

    protected void StopBlinking()
    {
        blinking = false;
    }
}
