using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SplashScreenLogic : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Image _image;
    public static bool SceneIsOn;
    private UniTask _appearance;
    public static event Action OnSceneReady;

    private void Awake() {
        _appearance = Appear();
    }

    private async UniTask Appear() {
        float value = 1;
        while (value>0) {

            value -= Time.deltaTime;
            _image.color = new Color(0, 0, 0, value);
            await UniTask.NextFrame();
        }

        _image.enabled = false;
        //Yes, this is kinda violates SRP and MVC 'cause there's busines logic and view at same script,
        //but c'mon, this script is only 31 rows long. In a perfect world there is separate view and  manager, but 
        //unfortunately we live in a cursed timeline where merry is 750 roubles per kilo and 1C: Производство is a thing
        SceneIsOn = true;
        OnSceneReady?.Invoke();
    }
}
