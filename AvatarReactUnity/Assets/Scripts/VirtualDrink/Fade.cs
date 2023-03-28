using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FadeIn()
    {
        StartCoroutine(Color_FadeIn());
    }
    public void FadeOut()
    {
        StartCoroutine(Color_FadeOut());
    }
    IEnumerator Color_FadeIn()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        // 画面をフェードインさせるコールチン
        // 前提：画面を覆うPanelにアタッチしている

        // 色を変えるゲームオブジェクトからImageコンポーネントを取得
        Image fade = GetComponent<Image>();

        // フェード元の色を設定（白）★変更可
        fade.color = new Color((255.0f / 255.0f), (255.0f / 255.0f), (255.0f / 255.0f), (255.0f / 255.0f));

        // フェードインにかかる時間（秒）★変更可
        const float fade_time = 0.5f;

        // ループ回数（0はエラー）★変更可
        const int loop_count = 10;

        // ウェイト時間算出
        float wait_time = fade_time / loop_count;

        // 色の間隔を算出
        float alpha_interval = 255.0f / loop_count;

        // 色を徐々に変えるループ
        for (float alpha = 255.0f; alpha >= 0.0f; alpha -= alpha_interval)
        {
            // 待ち時間
            yield return new WaitForSeconds(wait_time);

            // Alpha値を少しずつ下げる
            Color new_color = fade.color;
            new_color.a = alpha / 255.0f;
            fade.color = new_color;
        }
    }
    IEnumerator Color_FadeOut()
    {
        // 画面をフェードインさせるコールチン
        // 前提：画面を覆うPanelにアタッチしている
        // 色を変えるゲームオブジェクトからImageコンポーネントを取得
        Image fade = GetComponent<Image>();

        // フェード元の色を設定（黒）★変更可
        fade.color = new Color((255.0f / 255.0f), (255.0f / 255.0f), (255.0f / 255.0f), (0.0f / 255.0f));

        // フェードインにかかる時間（秒）★変更可
        const float fade_time = 0.5f;

        // ループ回数（0はエラー）★変更可
        const int loop_count = 10;

        // ウェイト時間算出
        float wait_time = fade_time / loop_count;

        // 色の間隔を算出
        float alpha_interval = 255.0f / loop_count;

        // 色を徐々に変えるループ
        for (float alpha = 0.0f; alpha <= 255.0f; alpha += alpha_interval)
        {
            // 待ち時間
            yield return new WaitForSeconds(wait_time);

            // Alpha値を少しずつ下げる
            Color new_color = fade.color;
            new_color.a = alpha / 255.0f;
            fade.color = new_color;
        }
    }
}
