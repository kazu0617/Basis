# Basis – AUR パッケージ

このディレクトリには Arch Linux の AUR (Arch User Repository) 向け PKGBUILD が含まれています。

## パッケージ一覧

| パッケージ | 説明 | chroot ビルド |
|-----------|------|:---:|
| [`basis-server`](./basis-server/) | .NET 製ネットワークサーバー | ✅ 可能 |
| [`basis-client`](./basis-client/) | Unity 製 VR クライアント (Linux x86_64) | ❌ 不可 |

---

## basis-server

通常の `makepkg` でビルドできます。

### 依存関係

- `dotnet-sdk` (10.0+) — Arch 公式またはAURの `dotnet-sdk-bin`

### インストール手順

```sh
cd aur/basis-server
makepkg -si
```

### サービスの起動

```sh
# 初回起動
sudo systemctl enable --now basis-server

# ログ確認
journalctl -u basis-server -f
```

> **Note:** `basis` ユーザー・グループは `post_install` スクリプトで自動的に作成されます。

---

## basis-client

Unity 6 (6000.4.4f1) が必要なため、**chroot 環境ではビルドできません**。  
`--nosandbox` オプションを使い、ホスト環境の Unity を利用します。

### 事前準備

1. **Unity Hub をインストール**  
   - AUR: `yay -S unityhub` または https://unity.com/download
2. **Unity 6000.4.4f1 をインストール**  
   Unity Hub > Installs > Install Editor > バージョン `6000.4.4f1` を選択  
   追加モジュール:
   - ✅ Linux Build Support (IL2CPP)
   - ✅ Linux Build Support (Mono)
3. **Unity ライセンスをアクティベート**  
   Unity Hub でサインインし Personal / Pro ライセンスをアクティベートしてください。

### インストール手順

```sh
cd aur/basis-client

# 標準インストール（Unity Hub のデフォルトパスを自動検索）
makepkg --nosandbox -si

# Unity のパスを手動指定する場合
UNITY_PATH=/path/to/Unity/Hub/Editor/6000.4.4f1/Editor/Unity \
    makepkg --nosandbox -si
```

> **Note:** Unity のインポートとビルドには **初回 30〜60 分程度** かかります。  
> 2 回目以降は `Basis/Library/` キャッシュが利用されます。  
> ビルドログは `src/unity-build.log` に保存されます。

### 実行

```sh
# コマンドライン
basis-client

# または /usr/share/applications/basis-client.desktop からも起動可能
```

---

## AUR への公開

各ディレクトリで `.SRCINFO` を生成してから AUR にプッシュしてください:

```sh
cd aur/basis-server
makepkg --printsrcinfo > .SRCINFO

cd ../basis-client
makepkg --printsrcinfo > .SRCINFO
```

---

## トラブルシューティング

### basis-server: `dotnet: command not found`

```sh
# Arch 公式
sudo pacman -S dotnet-sdk

# または AUR (バイナリ)
yay -S dotnet-sdk-bin
```

### basis-client: Unity が見つからない

```sh
# Unity Hub のインストール先を確認
ls ~/Unity/Hub/Editor/

# パスを直接指定
UNITY_PATH=~/Unity/Hub/Editor/6000.4.4f1/Editor/Unity makepkg --nosandbox -si
```

### basis-client: IL2CPP ビルドエラー

Linux IL2CPP ビルドには追加のネイティブツールが必要です:

```sh
sudo pacman -S gcc lib32-gcc-libs
```
