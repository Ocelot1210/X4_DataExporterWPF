﻿using System.Data;
using System.Data.SQLite;

namespace X4_DataExporterWPF.Export
{
    /// <summary>
    /// Export処理用インターフェイス
    /// </summary>
    interface IExport
    {
        /// <summary>
        /// エクスポート処理
        /// </summary>
        /// <param name="cmd"></param>
        void Export(SQLiteCommand cmd);
    }
}
