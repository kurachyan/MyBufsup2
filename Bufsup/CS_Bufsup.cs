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
        CS_Rskip rskip;             // 右側余白情報を削除
        CS_Lskip lskip;             // 左側余白情報を削除

        private String _wbuf;       // ソース情報
        private Boolean _empty;     // ソース情報有無
        private String _rem;        // 注釈情報
        private Boolean _remark;    // 注釈管理情報
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
                    if (rskip == null || lskip == null)
                    {   // 未定義？
                        rskip = new CS_Rskip();
                        lskip = new CS_Lskip();
                    }
                    rskip.Wbuf = _wbuf;
                    rskip.Exec();
                    lskip.Wbuf = rskip.Wbuf;
                    lskip.Exec();
                    _wbuf = lskip.Wbuf;

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
        }
        #endregion

        #region コンストラクタ
        public CS_Bufsup()
        {   // コンストラクタ
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            rskip = null;
            lskip = null;
        }
        #endregion

        #region モジュール
        public void Clear()
        {   // 作業領域の初期化
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            rskip = null;
            lskip = null;
        }
        public void Exec()
        {   // 構文評価を行う
            if (!_empty)
            {   // バッファーに実装有り
                // 構文評価を行う
                int _pos;       // 位置情報
                _rem = null;        // コメント情報初期化
                Boolean _judge = false;     // Rskip稼働の判断     
                if (rskip == null || lskip == null)
                {
                    rskip = new CS_Rskip();
                    lskip = new CS_Lskip();
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
                if (rskip == null || lskip == null)
                {
                    rskip = new CS_Rskip();
                    lskip = new CS_Lskip();
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
            rskip.Wbuf = _wbuf;
            rskip.Exec();
            lskip.Wbuf = rskip.Wbuf;
            lskip.Exec();
            _wbuf = lskip.Wbuf;

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
                if (rskip == null || lskip == null)
                {   // 未定義？
                    rskip = new CS_Rskip();
                    lskip = new CS_Lskip();
                }
                rskip.Wbuf = _wbuf;
                rskip.Exec();
                lskip.Wbuf = rskip.Wbuf;
                lskip.Exec();
                _wbuf = lskip.Wbuf;

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
