import random
import csv
import os
import sys

# ────────────────────────────────────────────────────
# グローバル変数：質問文末に付加するコメントをここで初期化
# ────────────────────────────────────────────────────
comment = "の問題だよ！"


def count_csv_data_rows(filepath):
    """
    CSVファイル内のデータ行の個数を返します（ヘッダー行を除く）。

    Args:
        file_path (str): CSVファイルのパス。

    Returns:
        int: データ行の個数。ファイルが見つからない場合や読み込みエラーの場合は -1 を返します。
             ファイルが空の場合やヘッダーのみの場合は 0 を返します。
    """
    try:
        with open(filepath, 'r', newline='', encoding='utf-8') as csvfile:
            reader = csv.reader(csvfile)
            # 残りの行数をカウント
            row_count = sum(1 for row in reader)
            return row_count
    except FileNotFoundError:
        print(f"エラー: ファイル '{filepath}' が見つかりません。")
        return -1
    except Exception as e:
        print(f"CSVファイルの読み込み中にエラーが発生しました: {e}")
        return -1
    
def clear_console():
    """コンソール画面をクリアします。"""
    os.system('cls' if os.name == 'nt' else 'clear')


def load_quiz_data(filepath, num_cols_per_set):
    """
    CSVファイルからクイズデータを読み込みます。

    Args:
        filepath (str): CSVファイルのパス。
        num_cols_per_set (int): 1行あたり何列あるか（問題文, 正解, 選択肢…, タイトル等を含めた列数）。

    Returns:
        list of dict: 読み込んだクイズ情報。各辞書には "question", "answer", "choices" をキーとする。
    """
    quiz_list = []
    try:
        with open(filepath, 'r', newline='', encoding='utf-8') as file:
            reader = csv.reader(file)
            for i, row in enumerate(reader):
                if len(row) < num_cols_per_set:
                    print(
                        f"警告: {filepath} の {i+1} 行目は列数が足りません。"
                        f"期待: {num_cols_per_set} 列, 実際: {len(row)} 列 → スキップ"
                    )
                    continue
                # 問題文は row[0], 正解は row[1]
                question = row[0].strip()
                correct = row[1].strip()
                # 選択肢は row[2]～ row[num_cols_per_set-2] まで
                choices = [cell.strip() for cell in row[2 : num_cols_per_set - 1]]
                # 最後の列 row[num_cols_per_set-1] は「タイトル」として扱う（後で表示用）
                title = row[num_cols_per_set - 1].strip()

                # 正解が choices に含まれていなければ追加
                if correct not in choices:
                    choices.append(correct)

                quiz_list.append({
                    "question": question,
                    "answer": correct,
                    "choices": choices,
                    "title": title
                })
        return quiz_list

    except FileNotFoundError:
        print(f"エラー: ファイルが見つかりませんでした: {filepath}")
        return []
    except Exception as e:
        print(f"CSV読み込み中にエラーが発生しました: {e}")
        return []


def get_file_path():
    """
    ユーザーに CSV ファイルをドラッグ＆ドロップ、または入力させて正しいパスを返す
    """
    while True:
        filepath = input("CSVファイルをドラッグ＆ドロップして Enter を押してください: ").strip()
        filepath = filepath.strip('"')  # Windows でドラッグ＆ドロップすると "C:\path\to\file.csv" のように引用符が付くことがあるため
        if os.path.exists(filepath):
            return filepath
        else:
            print("指定されたファイルが見つかりません。もう一度お試しください。")


def check_file_size(filepath, max_size_gb=1):
    """
    ファイルサイズが指定上限を超えていないか確認する
    """
    try:
        file_size_bytes = os.path.getsize(filepath)
        file_size_gb = file_size_bytes / (1024**3)
        if file_size_gb > max_size_gb:
            print(f"警告: ファイルサイズが {max_size_gb}GB を超えています ({file_size_gb:.2f}GB)。")
            print("大きなファイルは読み込みに時間がかかる場合があります。")
            return False
        return True
    except FileNotFoundError:
        print(f"エラー: ファイルが見つからず、サイズをチェックできませんでした: {filepath}")
        return False
    except Exception as e:
        print(f"ファイルサイズ確認中にエラーが発生しました: {e}")
        return False


def quiz_setting():
    """
    グローバル変数 `comment` をユーザー入力で更新する
    """
    global comment
    input("クイズの設定を行います。Enterキーを押して続行...")
    print("現在のコメント設定:", comment)
    new_comment = input("n番目の問題で表示するコメントを入力してください: ").strip()
    if new_comment:
        comment = new_comment
    print("設定後のコメント:", comment)
    input("設定が完了しました。Enterキーを押して戻ります...")


def run_quiz():
    """
    インタラクティブなコンソールクイズを実行する
    """
    global comment

    clear_console()
    print("ようこそ！CSVファイルからクイズを始めましょう。")
    print("設定画面に移りたい場合は 'o' を入力し、Enterキー。続けて始めるなら Enter のみ押してください。")
    choice = input("設定: 'o', それ以外で開始: ").strip().lower()
    if choice == 'o':
        try:
            quiz_setting()
        except Exception as e:
            print(f"設定中にエラーが発生しました: {e}")
            input("Enterキーで続行します...")

    clear_console()
    filepath = get_file_path()

    if not check_file_size(filepath):
        proceed = input("続行しますか？ (y/n): ").strip().lower()
        if proceed != 'y':
            print("クイズを終了します。")
            return

    # 列数をユーザー入力させる
    while True:
        try:
            num_cols = int(input(
                "1つの問題セットに含まれるデータの数 (問題文、正解、選択肢、タイトルの合計列数) を入力してください: "
            ).strip())
            if num_cols < 3:
                print("問題セットには最低でも 3 列（問題文1 + 正解1 + タイトル1）が必要です。")
                continue
            break
        except ValueError:
            print("無効な入力です。整数で再度入力してください。")

    # CSV を読み込む
    quiz_list = load_quiz_data(filepath, num_cols)
    if not quiz_list:
        print("クイズデータを読み込めませんでした。プログラムを終了します。")
        return

    score = 0
    total_questions = 0
    remaining = quiz_list.copy()

    while True:
        clear_console()

        if not remaining:
            print("すべてのクイズを解き終えました！")
            break

        # ランダムに1問取り出して出題
        quiz = random.choice(remaining)
        total_questions += 1

        # ───────────────────────────────────
        # 「n問目は『タイトル列（row[num_cols-1]）』の問題だよ！」を表示
        title_text = quiz["title"]
        print("全", count_csv_data_rows(filepath), "問中", f"{total_questions}問目は『{title_text}』の問題{comment}")
        # ───────────────────────────────────

        print("------ 問題 ------")
        print(quiz["question"])
        print("\n選択肢:")
 

        # 選択肢をシャッフルして表示
        shuffled_choices = quiz["choices"].copy()
        random.shuffle(shuffled_choices)
        for i, choice_text in enumerate(shuffled_choices, start=1):
            print(f"{i}. {choice_text}")

        # プレイヤーの入力
        try:
            ans_input = input("\n答えの番号を入力してください (q: 終了): ").strip().lower()
            if ans_input == 'q':
                print("\nクイズを終了します。ありがとうございました！")
                break

            ans = int(ans_input)
            if 1 <= ans <= len(shuffled_choices):
                selected = shuffled_choices[ans - 1]
                if selected == quiz["answer"]:
                    print("✅ 正解！")
                    score += 1
                else:
                    print(f"❌ 不正解… 正解は「{quiz['answer']}」でした。")
            else:
                print("⚠ 無効な番号です。")
        except ValueError:
            print("⚠ 番号入力が無効です。")

        print(f"現在の正解数: {score} / {count_csv_data_rows(filepath)}問")

        # 出題済みの問題はリストから削除
        remaining.remove(quiz)

        next_action = input("\nEnterキーで次の問題へ、'q' で終了: ").strip().lower()
        if next_action == 'q':
            print("\nクイズを終了します。ありがとうございました！")
            break

    # 全問終了後の最終結果表示
    print("\n--- クイズ終了 ---")
    print(f"総出題数: {count_csv_data_rows(filepath)}問")
    print(f"正解数: {score}問")
    if count_csv_data_rows(filepath) > 0:
        accuracy = (score / count_csv_data_rows(filepath)) * 100
        print(f"正答率: {accuracy:.2f}%")
    print("\nお疲れさまでした！")


if __name__ == "__main__":
    run_quiz()
