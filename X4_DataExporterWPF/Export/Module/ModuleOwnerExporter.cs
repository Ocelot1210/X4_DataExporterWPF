﻿using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using X4_DataExporterWPF.Entity;

namespace X4_DataExporterWPF.Export
{
    /// <summary>
    /// モジュール所有派閥情報抽出用クラス
    /// </summary>
    public class ModuleOwnerExporter : IExporter
    {
        /// <summary>
        /// 情報xml
        /// </summary>
        private readonly XDocument _WaresXml;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="waresXml">ウェア情報xml</param>
        public ModuleOwnerExporter(XDocument waresXml)
        {
            _WaresXml = waresXml;
        }


        /// <summary>
        /// 抽出処理
        /// </summary>
        /// <param name="cmd"></param>
        public void Export(SQLiteCommand cmd)
        {
            //////////////////
            // テーブル作成 //
            //////////////////
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS ModuleOwner
(
    ModuleID    TEXT    NOT NULL,
    FactionID   TEXT    NOT NULL,
    PRIMARY KEY (ModuleID, FactionID),
    FOREIGN KEY (ModuleID)  REFERENCES Module(ModuleID),
    FOREIGN KEY (FactionID) REFERENCES Faction(FactionID)
) WITHOUT ROWID";
                cmd.ExecuteNonQuery();
            }


            ////////////////
            // データ抽出 //
            ////////////////
            {
                var items = _WaresXml.Root.XPathSelectElements("ware[@tags='module']").SelectMany
                (
                    module => module.XPathSelectElements("owner").Select
                    (
                        owner =>
                        {
                            var moduleID = module.Attribute("id")?.Value;
                            if (string.IsNullOrEmpty(moduleID)) return null;
                            var factionID = owner.Attribute("faction")?.Value;
                            if (string.IsNullOrEmpty(factionID)) return null;
                            return new ModuleOwner(moduleID, factionID);
                        }
                    )
                )
                .Where
                (
                    x => x != null
                );


                cmd.CommandText = "INSERT INTO ModuleOwner (ModuleID, FactionID) values (@moduleID, @factionID)";
                foreach (var item in items)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("moduleID", item.ModuleID);
                    cmd.Parameters.AddWithValue("factionID", item.FactionID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
