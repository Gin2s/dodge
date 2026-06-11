# dodge

Unity で作成した 2D 避けゲームです。5 レーンを左右に移動して、上から降ってくる障害物を避け続け、生存時間を競います。

## 遊び方

| 操作 | キー |
| --- | --- |
| 左に移動 | ← / A |
| 右に移動 | → / D |
| スタート / リトライ | Enter |

- タイトル画面で **Enter** を押すとゲーム開始
- 障害物に当たるとゲームオーバー。**Enter** でリトライ
- 生存時間がスコアになり、ベストスコアが記録されます

## ゲーム内容

- **5 レーン制** … スライド移動で隣のレーンへ
- **障害物** … 棘つき玉（遅め）とナイフ（速め）がランダムに出現
- **エンドレス** … 時間経過で難易度（出現頻度・速度）が上昇
- **スコア** … 生存時間でスコアリング、ベストスコアを保存

## 開発環境

- Unity **6000.4.9f1**
- 2D (URP)

## プロジェクト構成

```
Assets/
├── Scripts/          ゲームロジック (C#)
│   ├── GameManager.cs       ゲーム進行・スコア管理
│   ├── PlayerController.cs   プレイヤーのレーン移動
│   ├── ObstacleSpawner.cs    障害物の生成・難易度調整
│   ├── Obstacle.cs           障害物の挙動
│   └── RetryButton.cs        リトライ処理
├── Editor/
│   └── SceneSetup.cs   メニュー「Dodge → Setup Scene」でシーンを自動構築
├── Sprites/          スプライト (Player.png, Obstacle.png)
└── Scenes/
    └── SampleScene.unity   メインシーン
```

## セットアップ

1. リポジトリをクローン
   ```sh
   git clone https://github.com/Gin2s/dodge.git
   ```
2. Unity Hub に追加し、Unity **6000.4.9f1** で開く
3. `Assets/Scenes/SampleScene.unity` を開いて再生

> シーンを作り直したい場合は、メニューの **Dodge → Setup Scene** からシーンを自動構築できます。
