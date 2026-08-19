using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StoneVFXManager
{
    private int _row;
    private int _col;
    private StonePoolManager _stonePoolManager;
    private Transform _stoneVFX;

    public StoneVFXManager(StonePoolManager stonePoolManager, Transform StoneVFX, int row, int column)
    {
        _stonePoolManager = stonePoolManager;
        _stoneVFX = StoneVFX;

        _row = row;
        _col = column;
    }

    public void PlayFullExplosion(int r, int c, ExplosionResult result)
    {
        if (result.vfxList == null || result.vfxList.Count == 0) return;

        foreach (StoneVFXType type in result.vfxList)
        {
            GameObject vfxObject = _stonePoolManager.GetStoneVFXByType(type);
            vfxObject.transform.SetParent(_stoneVFX);

            if(type == StoneVFXType.LightHorizontal)
            {
                vfxObject.transform.localPosition = new Vector2(3.5f, r);
            }
            else if(type == StoneVFXType.LightVertical)
            {
                vfxObject.transform.localPosition = new Vector2(c, 8/2);
            }
            else vfxObject.transform.localPosition = new Vector2(c, r);
 
            ParticleSystem[] particles = vfxObject.GetComponentsInChildren<ParticleSystem>();
            foreach(var ps in particles)
            {
                var main = ps.main;
                main.startColor = GetColorForStoneType(result.type);
            }

            vfxObject.SetActive(true);

            ReleaseVFXAfterDelay(type, vfxObject);
        }
    }

    public async void ReleaseVFXAfterDelay(StoneVFXType type, GameObject fx)
    {
        await Task.Delay(500);
        _stonePoolManager.ReturnStoneVFXByType(type, fx); 
    }

    public Color GetColorForStoneType(StoneType type)
    {
        Color targetColor = Color.white; 

        switch (type)
        {
            case StoneType.Blue:
                ColorUtility.TryParseHtmlString("#4486e3", out targetColor);
                break;
                
            case StoneType.Green:
                ColorUtility.TryParseHtmlString("#6bd739", out targetColor);
                break;
                
            case StoneType.Purple:
                ColorUtility.TryParseHtmlString("#a845d6", out targetColor);
                break;
                
            case StoneType.Red:
                ColorUtility.TryParseHtmlString("#d3433c", out targetColor);
                break;
                
            case StoneType.Yellow:
                ColorUtility.TryParseHtmlString("#e6ba43", out targetColor);
                break;
        }

        return targetColor;
    }
}
