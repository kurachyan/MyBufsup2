using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRSkip;

namespace Bufsup
{
    public class CS_Bufsup
    {
        #region 共有領域
        // '16.01.13 両側余白情報削除の追加　及び、右側・左側余白処理のコメント化
        CS_LRskip lrskip;           // 両側余白情報を削除

        private static String _wbuf;       // ソース情報
        private static Boolean _empty;     // ソース情報有無
        private static String _rem;        // 注釈情報
        private static Boolean _remark;    // 注釈管理情報
        public String Wbuf
        {
            get
            {
                return (_wbuf);
            }
            set
            {
                _wbuf = value;
                if (_wbuf == null)
                {   // 設定情報は無し？
                    _empty = true;
                }
                else
                {   // 整形処理を行う
                    // 不要情報削除
                    if (lrskip == null)
                    {   // 未定義？
                        lrskip = new CS_LRskip();
                    }
                    lrskip.Exec(_wbuf);
                    _wbuf = lrskip.Wbuf;

                    // 作業の為の下処理
                    if (_wbuf.Length == 0 || _wbuf == null)
                    {   // バッファー情報無し
                        // _wbuf = null;
                        _empty = true;
                    }
                    else
                    {
                        _empty = false;
                    }

                    _rem = null;        // 注釈情報　初期化
                }
            }
        }

        public String Rem
        {
            get
            {
                return (_rem);
            }
        }

        public Boolean Remark
        {
            get
            {
                return (_remark);
            }
            set
            {   // 16.01.28 連続呼び出し時の状況設定追加
                _remark = value;
            }
        }
        #endregion

        #region コンストラクタ
        public CS_Bufsup()
        {   // コンストラクタ
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            lrskip = null;
        }
        #endregion

        #region モジュール
        public void Clear()
        {   // 作業領域の初期化
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            lrskip = null;
        }
        public void Exec()
        {   // 構文評価を行う
            if (!_empty)
            {   // バッファーに実装有り
                // 構文評価を行う
                int _pos;       // 位置情報
                _rem = null;        // コメント情報初期化
                Boolean _judge = false;     // Rskip稼働の判断     

                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskip();
                }

                do
                {
                    if (_judge == true)
                    {   // Rskip稼働？                      
                        Reskip();          // Rskip・Lskip稼働
                        _judge = false;
                        if (_wbuf == null)
                        {   // 評価対象が存在しない？
                            break;
                        }
                    }

                    _pos = _wbuf.IndexOf(@"//");
                    if (_pos != -1)
                    {   // コメント"//"検出？
                        Supsub(_pos);
                        break;
                    }

                    _pos = _wbuf.IndexOf(@"/*");
                    if (_pos != -1)
                    {   // コメント"/*"検出？
                        Supsub(_pos);
                        _remark = true;         // コメント開始
                        _judge = true;          // Rskip稼働
                    }

                    _pos = _wbuf.IndexOf(@"*/");
                    if (_pos != -1)
                    {   // コメント"*/"検出？
                        RSupsub(_pos);
                        _remark = false;        // コメント終了
                        _judge = true;          // Rskip稼働
                    }

                    if (_rem != null)
                    {   // コメント設定有り？
                        _pos = _rem.IndexOf(@"*/");
                        if (_pos != -1)
                        {   // コメント"*/"検出？
                            RRSupsub(_pos);
                            _remark = false;        // コメント終了
                            _judge = true;          // Rskip稼働
                        }

                    }
                } while (_pos > 0);

                Reskip();              // Rskip稼働
                if (_wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }

            }

        }
        public void Exec(String msg)
        {   // 構文評価を行う
            Setbuf(msg);                 // 入力内容の作業領域設定

            if (!_empty)
            {   // バッファーに実装有り
                // 構文評価を行う
                int _pos;       // 位置情報
                _rem = null;        // コメント情報初期化
                Boolean _judge = false;     // Rskip稼働の判断     

                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskip();
                }

                do
                {
                    if (_judge == true)
                    {   // Rskip稼働？                      
                        Reskip();          // Rskip・Lskip稼働
                        _judge = false;
                        if (_wbuf == null)
                        {   // 評価対象が存在しない？
                            break;
                        }
                    }

                    _pos = _wbuf.IndexOf(@"//");
                    if (_pos != -1)
                    {   // コメント"//"検出？
                        Supsub(_pos);
                        break;
                    }

                    _pos = _wbuf.IndexOf(@"/*");
                    if (_pos != -1)
                    {   // コメント"/*"検出？
                        Supsub(_pos);
                        _remark = true;         // コメント開始
                        _judge = true;          // Rskip稼働
                    }

                    _pos = _wbuf.IndexOf(@"*/");
                    if (_pos != -1)
                    {   // コメント"*/"検出？
                        RSupsub(_pos);
                        _remark = false;        // コメント終了
                        _judge = true;          // Rskip稼働
                    }

                    if (_rem != null)
                    {   // コメント設定有り？
                        _pos = _rem.IndexOf(@"*/");
                        if (_pos != -1)
                        {   // コメント"*/"検出？
                            RRSupsub(_pos);
                            _remark = false;        // コメント終了
                            _judge = true;          // Rskip稼働
                        }

                    }
                } while (_pos > 0);

                Reskip();              // Rskip稼働
                if (_wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }

            }
        }
        public Boolean Exec(Boolean remflg, String msg)
        {   // 構文評価を行う
            Setbuf(msg);                            // 入力内容の作業領域設定
            _remark = remflg;                       // コメント情報を設定

            if (!_empty)
            {   // バッファーに実装有り
                // 構文評価を行う
                int _pos;       // 位置情報
                _rem = null;        // コメント情報初期化
                Boolean _judge = false;     // Rskip稼働の判断     

                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskip();
                }

                do
                {
                    if (_judge == true)
                    {   // Rskip稼働？                      
                        Reskip();          // Rskip・Lskip稼働
                        _judge = false;
                        if (_wbuf == null)
                        {   // 評価対象が存在しない？
                            break;
                        }
                    }

                    _pos = _wbuf.IndexOf(@"//");
                    if (_pos != -1)
                    {   // コメント"//"検出？
                        Supsub(_pos);
                        break;
                    }

                    _pos = _wbuf.IndexOf(@"/*");
                    if (_pos != -1)
                    {   // コメント"/*"検出？
                        Supsub(_pos);
                        _remark = true;         // コメント開始
                        _judge = true;          // Rskip稼働
                    }

                    _pos = _wbuf.IndexOf(@"*/");
                    if (_pos != -1)
                    {   // コメント"*/"検出？
                        RSupsub(_pos);
                        _remark = false;        // コメント終了
                        _judge = true;          // Rskip稼働
                    }

                    if (_rem != null)
                    {   // コメント設定有り？
                        _pos = _rem.IndexOf(@"*/");
                        if (_pos != -1)
                        {   // コメント"*/"検出？
                            RRSupsub(_pos);
                            _remark = false;        // コメント終了
                            _judge = true;          // Rskip稼働
                        }

                    }
                } while (_pos > 0);

                Reskip();              // Rskip稼働
                if (_wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }

            }

            return _remark;                     // コメントの継続情報を返す
        }
        #endregion

        #region サブ・モジュール
        private void Supsub(int __pos)
        {
            String __wbuf;  // コード情報

            __wbuf = _wbuf.Substring(0, __pos);      // コード抜き出し
            _rem += _wbuf.Substring(__pos + 2, _wbuf.Length - __pos - 2);  // コメント抜き出し
            _wbuf = __wbuf;

        }
        private void RSupsub(int __pos)
        {
            _rem += _wbuf.Substring(0, __pos + 2);  // コメント抜き出し
            _wbuf = _wbuf.Substring(__pos + 2, _wbuf.Length - __pos - 2);      // コード抜き出し

        }
        private void RRSupsub(int __pos)
        {
            String __rem;       //コメント情報

            __rem = _rem.Substring(0, __pos);  // コメント抜き出し
            _wbuf += _rem.Substring(__pos + 2, _rem.Length - __pos - 2);      // コード抜き出し
            _rem = __rem;

        }
        private void Reskip()
        {
            lrskip.Exec(_wbuf);
            _wbuf = lrskip.Wbuf;

        }

        private void Setbuf(String _strbuf)
        {   // [_wbuf]情報設定
            _wbuf = _strbuf;
            if (_wbuf == null)
            {   // 設定情報は無し？
                _empty = true;
            }
            else
            {   // 整形処理を行う
                // 不要情報削除
                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskip();
                }
                lrskip.Exec(_wbuf);
                _wbuf = lrskip.Wbuf;

                // 作業の為の下処理
                if (_wbuf.Length == 0 || _wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }
                else
                {
                    _empty = false;
                }

                _rem = null;        // 注釈情報　初期化
            }
        }
        #endregion
    }
}
